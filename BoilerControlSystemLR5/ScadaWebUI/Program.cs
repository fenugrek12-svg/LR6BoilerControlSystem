using ScadaWebUI.Services;

var builder = WebApplication.CreateBuilder(args);

// Добавляем поддержку MVC (контроллеры и представления)
builder.Services.AddControllersWithViews();

// 1. Регистрируем хранилище данных датчиков как Singleton (один экземпляр на всё приложение)
builder.Services.AddSingleton<ISensorDataService, SensorDataService>();

// 2. Регистрируем фоновый сервис MQTT для постоянного приёма сообщений
builder.Services.AddHostedService<MqttBackgroundService>();

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