using Microsoft.EntityFrameworkCore;
using ScadaWebUI.Data;
using ScadaWebUI.Services;

var builder = WebApplication.CreateBuilder(args);

// Додаємо DbContext для EF Core (ЛР8)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Реєстрація сервісів з попередніх ЛР
builder.Services.AddSingleton<ISensorDataService, SensorDataService>();
builder.Services.AddHostedService<MqttBackgroundService>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();