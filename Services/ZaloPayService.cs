using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Web;
using TechnoShop.Models;

namespace TechnoShop.Services
{
    public class ZaloPayService : IPaymentService
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;

        public ZaloPayService(IConfiguration config, HttpClient httpClient)
        {
            _config = config;
            _httpClient = httpClient;
            _httpClient.Timeout = TimeSpan.FromSeconds(20);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<PaymentResult> CreatePaymentAsync(HttpContext context, Order order)
        {
            string appId = _config["ZaloPay:AppId"] ?? "";
            string key1 = _config["ZaloPay:Key1"] ?? "";
            string key2 = _config["ZaloPay:Key2"] ?? "";
            string callbackUrl = _config["ZaloPay:CallbackUrl"] ?? "http://localhost:5000/PaymentResult";
            string redirectUrl = _config["ZaloPay:ReturnUrl"] ?? "http://localhost:5000/PaymentResult";
            string zaloEndpoint = _config["ZaloPay:Endpoint"] ?? "https://sb-openapi.zalopay.vn/v2/create";

            // ZaloPay yêu cầu app_trans_id theo chuẩn: yyMMdd_xxx, ít hơn 32 ký tự và phải unique hàng ngày.
            var baseTransId = DateTime.UtcNow.ToString("yyMMdd") + "_" + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var appTransId = baseTransId;

            // ZaloPay app_amount yêu cầu đơn vị VND (số nguyên), không nhân 100.
            // order.TotalPrice ở đây đã tính theo VND (ví dụ 47999000 = 47,999,000đ).
            var amount = (long)Math.Round(order.TotalPrice);
            var orderInfo = $"Thanh toán đơn hàng {order.OrderID}";
            var extraData = ""; // để trống nếu không cần, hoặc JSON encoded

            // Dữ liệu tạo hash theo Zalopay docs:
            // app_id|app_trans_id|app_user|app_time|app_amount|app_description|app_callback_url|app_return_url|app_extra_data
            var appTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            var data = string.Join("|", new object[] {
                appId,
                appTransId,
                order.Email ?? "guest",
                appTime,
                amount,
                orderInfo,
                callbackUrl,
                redirectUrl,
                extraData
            });

            string mac = ComputeHmacSha256(key1, data);

            var requestPayload = new
            {
                app_id = appId,
                app_trans_id = appTransId,
                app_user = order.Email ?? "guest",
                app_time = appTime,
                app_amount = amount,
                app_description = orderInfo,
                app_callback_url = callbackUrl,
                app_return_url = redirectUrl,
                app_locale = "vi",
                app_currency = "VND",
                app_extras = extraData,
                app_hash = mac
            };

            var httpContent = new StringContent(JsonSerializer.Serialize(requestPayload), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(zaloEndpoint, httpContent);
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"ZaloPay create payment HTTP {response.StatusCode}: {body}");
            }

            var bodyText = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(bodyText);
            var root = doc.RootElement;
            var returnCode = root.GetProperty("return_code").GetInt32();
            var returnMessage = root.GetProperty("return_message").GetString() ?? "No message";
            if (returnCode != 1)
            {
                Console.WriteLine("[ZaloPay] REQUEST: " + JsonSerializer.Serialize(requestPayload));
                Console.WriteLine("[ZaloPay] RESPONSE: " + bodyText);
                throw new InvalidOperationException($"ZaloPay error {returnCode}: {returnMessage}. request={JsonSerializer.Serialize(requestPayload)}; response={bodyText}");
            }

            var orderUrl = root.GetProperty("order_url").GetString() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(orderUrl))
            {
                Console.WriteLine("[ZaloPay] REQUEST (missing order_url): " + JsonSerializer.Serialize(requestPayload));
                Console.WriteLine("[ZaloPay] RESPONSE (missing order_url): " + bodyText);
                throw new InvalidOperationException($"ZaloPay trả về return_code=1 nhưng không có order_url. request={JsonSerializer.Serialize(requestPayload)}; response={bodyText}");
            }

            var qrCodeUrl = "https://chart.googleapis.com/chart?chs=300x300&cht=qr&chl=" + HttpUtility.UrlEncode(orderUrl) + "&choe=UTF-8";

            // Gán app_trans_id để dùng lookup đơn hàng callback
            order.OrderGuid = appTransId;

            return new PaymentResult
            {
                CheckoutUrl = orderUrl,
                QrCodeUrl = qrCodeUrl,
                AppTransId = appTransId
            };
        }

        public bool ValidateSignature(IQueryCollection collections, string secret)
        {
            // ZaloPay callback has key 'mac' from Zalo template
            string mac = collections["mac"];
            if (string.IsNullOrEmpty(mac))
            {
                return false;
            }

            if (!long.TryParse(collections["app_id"], out _)) return false;
            var data = string.Join("|", new[] {
                collections["app_id"],
                collections["app_trans_id"],
                collections["app_time"],
                collections["app_amount"],
                collections["app_currency"],
                collections["app_user"],
                collections["app_state"],
                collections["app_trans_type"]
            });

            var computed = ComputeHmacSha256(secret, data);
            return string.Equals(mac, computed, StringComparison.OrdinalIgnoreCase);
        }

        private static string ComputeHmacSha256(string key, string data)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }
    }
}
