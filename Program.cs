using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using TechnoShop.Models;
using TechnoShop.Services;

var builder = WebApplication.CreateBuilder(args);

// --- 1. ĐĂNG KÝ SERVICES ---
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddDbContext<TechnoShopDbContext>(opts =>
{
    opts.UseSqlServer(
        builder.Configuration["ConnectionStrings:TechnoShopConnection"]);
});

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<TechnoShopDbContext>();

builder.Services.AddScoped<IProductRepository, EFProductRepository>();
builder.Services.AddScoped<IOrderRepository, EFOrderRepository>();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddScoped<Cart>(sp => SessionCart.GetCart(sp));
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<IVnpayService, VnpayService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAntiforgery();

var app = builder.Build();

// --- 2. CẤU HÌNH MIDDLEWARE (THỨ TỰ RẤT QUAN TRỌNG) ---
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication(); // Phải đứng trước Authorization
app.UseAuthorization();
app.UseSession();

// --- 3. ĐỊNH TUYẾN (ROUTING) - QUY TẮC: CỤ THỂ TRƯỚC, CHUNG CHUNG SAU ---

// Ưu tiên 1: Route mặc định cho Controller/Admin (Để /Admin chạy đúng)
app.MapDefaultControllerRoute();

// Ưu tiên 2: Các route có tiền tố rõ ràng
app.MapControllerRoute("pagination",
    "Products/Page{productPage}",
    new { Controller = "Home", action = "Index", productPage = 1 });

// Ưu tiên 3: Các route có phân trang theo Category
app.MapControllerRoute("catpage",
    "{category}/Page{productPage:int}",
    new { Controller = "Home", action = "Index" });

app.MapControllerRoute("page", "Page{productPage:int}",
    new { Controller = "Home", action = "Index", productPage = 1 });

// Ưu tiên cuối cùng: Route "Tham lam" (Catch-all) - Chỉ nhận những gì còn sót lại
app.MapControllerRoute("category", "{category}",
    new { Controller = "Home", action = "Index", productPage = 1 });

app.MapRazorPages();

// --- 4. KHỞI TẠO DATA ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<TechnoShopDbContext>();
        // Lưu ý: EnsureCreated() không hỗ trợ Migration. 
        // Nếu ông đã tạo bảng bằng tay trong SQL thì dòng này sẽ không làm gì cả.
        context.Database.EnsureCreated(); 
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Có lỗi xảy ra khi khởi tạo Database.");
    }
}

// Chạy hàm Seed dữ liệu (Admin, Role, Sản phẩm mẫu)
await SeedData.EnsurePopulated(app);

app.Run();