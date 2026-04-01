using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace TechnoShop.Infrastructure {
    public class VnPayLibrary {
        // Khai báo rõ ràng kiểu dữ liệu cho SortedList
        private readonly SortedList<string, string> _requestData = new SortedList<string, string>(new VnPayCompare());
        private readonly SortedList<string, string> _responseData = new SortedList<string, string>(new VnPayCompare());

        public void AddRequestData(string key, string value) {
            if (!string.IsNullOrEmpty(value)) {
                if (_requestData.ContainsKey(key)) _requestData[key] = value;
                else _requestData.Add(key, value);
            }
        }

        public void AddResponseData(string key, string value) {
            if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_")) {
                if (_responseData.ContainsKey(key)) _responseData[key] = value;
                else _responseData.Add(key, value);
            }
        }

        public string CreateRequestUrl(string baseUrl, string vnp_HashSecret) {
            StringBuilder data = new StringBuilder();
            StringBuilder rawData = new StringBuilder();

            // SỬA LỖI ĐỎ: Phải dùng KeyValuePair<string, string> thay vì var
            foreach (KeyValuePair<string, string> kv in _requestData) {
                if (!string.IsNullOrEmpty(kv.Value)) {
                    rawData.Append(kv.Key + "=" + kv.Value + "&");
                    data.Append(WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value) + "&");
                }
            }

            string queryString = data.ToString();
            string signData = rawData.ToString().TrimEnd('&');
            string vnp_SecureHash = HmacSHA512(vnp_HashSecret, signData);

            return baseUrl + "?" + queryString + "vnp_SecureHash=" + vnp_SecureHash;
        }

        public bool ValidateSignature(string inputHash, string secretKey) {
            StringBuilder data = new StringBuilder();
            foreach (KeyValuePair<string, string> kv in _responseData) {
                if (!string.IsNullOrEmpty(kv.Key) && kv.Key != "vnp_SecureHash") {
                    data.Append(kv.Key + "=" + kv.Value + "&");
                }
            }
            string checkSum = HmacSHA512(secretKey, data.ToString().TrimEnd('&'));
            return checkSum.Equals(inputHash, StringComparison.InvariantCultureIgnoreCase);
        }

        private string HmacSHA512(string key, string inputData) {
            var hash = new StringBuilder();
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputData);
            using (var hmac = new HMACSHA512(keyBytes)) {
                byte[] hashValue = hmac.ComputeHash(inputBytes);
                foreach (var theByte in hashValue) hash.Append(theByte.ToString("X2"));
            }
            return hash.ToString();
        }
    }

    public class VnPayCompare : IComparer<string> {
        public int Compare(string x, string y) => string.CompareOrdinal(x, y);
    }
}