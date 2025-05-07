using System;
using FoodOrderSite.Models;       // yeni: DbContext burada
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDistributedMemoryCache();

// 2) Session servisini ekleyin
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);    // Örnek timeout
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;                 // GDPR uyumluluðu
});

builder.Services.AddHttpContextAccessor();

// Cookie Authentication servisini kaydet
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/SignIn/Index";      // Giriþ sayfanýzýn route’u
        options.AccessDeniedPath = "/SignIn/AccessDenied"; // Ýsterseniz ayrý bir eriþim reddedildi sayfasý
        options.ExpireTimeSpan = TimeSpan.FromDays(7);  // Opsiyonel: cookie ömrü
        options.SlidingExpiration = true;                 // Opsiyonel
    });

// Yetkilendirme servislerini ekleyin (MVC zaten bunu çaðýrýr ama açýkça koymak da iyi)
builder.Services.AddAuthorization();

// 3) DbContext ve MVC desteði
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlCon"))
);

// Add services to the container.
builder.Services.AddControllersWithViews();


var app = builder.Build();

// ----------------- MIDDLEWARE PIPELINE -----------------

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Session middleware'ini ekliyoruz
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

// MapControllerRoute çaðý ve noktalý virgül unutulmuþtu:
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Uygulamayý baþlatmayý unutmayýn:
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