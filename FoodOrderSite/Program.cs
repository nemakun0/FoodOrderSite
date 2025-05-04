using FoodOrderSite.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddViewOptions(options =>
    {
        options.HtmlHelperOptions.ClientValidationEnabled = true;
    })
    .AddDataAnnotationsLocalization();

// Session servisini ekleyelim
builder.Services.AddDistributedMemoryCache(); // Oturum verilerini bellekte tutmak için
//builder.Services.AddSession(options =>
//{
//    options.IdleTimeout = TimeSpan.FromMinutes(30); // Oturum zaman aþýmýný belirliyoruz
//    options.Cookie.HttpOnly = true; // Güvenlik için HttpOnly olarak ayarlýyoruz
//});

// Add services to the container
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(7); // RememberMe için 7 gün
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Remember me butonu için cookie ayarlamasý
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/SignIn/Index";
        options.AccessDeniedPath = "/SignIn/AccessDenied"; // Opsiyonel
    });

// DbContext servisini ekle
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlCon")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Session middleware'ini ekliyoruz
app.UseRouting();
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

//using FoodOrderSite.Models;
//using Microsoft.AspNetCore.Authentication.Cookies;
//using Microsoft.EntityFrameworkCore;

//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
////builder.Services.AddControllersWithViews();
//builder.Services.AddControllersWithViews()
//    .AddViewOptions(options =>
//    {
//        options.HtmlHelperOptions.ClientValidationEnabled = true;
//    })
//    .AddDataAnnotationsLocalization();

////Remember me butonu için cookie ayarlamasý
//builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
//    .AddCookie(options =>
//    {
//        options.LoginPath = "/SignIn/Index";
//        options.AccessDeniedPath = "/SignIn/AccessDenied"; // Opsiyonel
//    });


//// DbContext servisini ekle
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlCon")));

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Home/Error");
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}

//app.UseHttpsRedirection();
//app.UseStaticFiles();

//app.UseRouting();

//app.UseAuthorization();
//app.UseAuthentication();

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");

//app.Run();