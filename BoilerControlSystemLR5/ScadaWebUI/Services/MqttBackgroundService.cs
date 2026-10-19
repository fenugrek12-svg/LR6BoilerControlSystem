using System.Text;
using System.Text.Json;
using MQTTnet;

namespace ScadaWebUI.Services
{
    public class MqttBackgroundService : BackgroundService
    {
        private readonly ISensorDataService _sensorDataService;
        private readonly ILogger<MqttBackgroundService> _logger;
        private IMqttClient? _mqttClient;

        public MqttBackgroundService(ISensorDataService sensorDataService, ILogger<MqttBackgroundService> logger)
        {
            _sensorDataService = sensorDataService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new MqttClientFactory();
            _mqttClient = factory.CreateMqttClient();

            var options = new MqttClientOptionsBuilder()
                .WithTcpServer("127.0.0.1", 1883)
                .WithClientId($"ScadaWebUI_{Guid.NewGuid()}")
                .Build();

            _mqttClient.ApplicationMessageReceivedAsync += e =>
            {
                var topic = e.ApplicationMessage.Topic;

                // Для версії MQTTnet 5.x використовуємо e.ApplicationMessage.Payload
                var payloadBytes = e.ApplicationMessage.Payload;
                var payload = payloadBytes.Length > 0 ? Encoding.UTF8.GetString(payloadBytes) : string.Empty;

                if (!string.IsNullOrEmpty(payload))
                {
                    try
                    {
                        // Парсинг JSON {"value": 12.34, "unit": "..."}
                        using var doc = JsonDocument.Parse(payload);
                        if (doc.RootElement.TryGetProperty("value", out var valProp))
                        {
                            double val = valProp.GetDouble();
                            _sensorDataService.UpdateValue(topic, val);
                        }
                    }
                    catch
                    {
                        // Якщо передано просте числове значення
                        if (double.TryParse(payload, System.Globalization.CultureInfo.InvariantCulture, out double val))
                        {
                            _sensorDataService.UpdateValue(topic, val);
                        }
                    }
                }

                return Task.CompletedTask;
            };

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (!_mqttClient.IsConnected)
                    {
                        await _mqttClient.ConnectAsync(options, stoppingToken);
                        _logger.LogInformation("SCADA підключилася до MQTT-брокера!");

                        // Підписка на топіки датчиків котла
                        await _mqttClient.SubscribeAsync("boiler/1/sensors/#", cancellationToken: stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Помилка зв'язку з MQTT: {ex.Message}");
                }

                await Task.Delay(4000, stoppingToken);
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_mqttClient != null && _mqttClient.IsConnected)
            {
                await _mqttClient.DisconnectAsync(cancellationToken: cancellationToken);
            }
            await base.StopAsync(cancellationToken);
        }
    }
}