using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ScadaWebUI.Data;
using ScadaWebUI.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. БД та EF Core
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// 2. Додаємо Identity (Автентифікація через Cookie та базові сторінки Login/Register)
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>();

// 3. Сервіси SCADA
builder.Services.AddSingleton<ISensorDataService, SensorDataService>();
builder.Services.AddHostedService<MqttBackgroundService>();

// 4. Налаштування авторизації та FallbackPolicy (ЛР9)
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages(); // Для сторінок Identity (Login, Register)

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();

    options.AddPolicy("CanReadSavings", policy =>
    {
        policy.RequireAuthenticatedUser();
    });
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// 5. Порядок Middleware: Routing -> Authentication -> Authorization
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages(); // Підключаємо маршрути Identity

app.Run();