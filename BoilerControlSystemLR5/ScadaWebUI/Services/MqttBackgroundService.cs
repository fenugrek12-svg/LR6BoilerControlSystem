using MQTTnet;
using ScadaWebUI.Services;
using System.Text;
using System.Text.Json;

namespace ScadaWebUI.Services;

public class MqttBackgroundService : BackgroundService
{
    private readonly IMqttClient _mqttClient;
    private readonly MqttClientOptions _options;
    private readonly ISensorDataService _sensorDataService;

    public MqttBackgroundService(ISensorDataService sensorDataService)
    {
        _sensorDataService = sensorDataService;
        var factory = new MqttClientFactory();
        _mqttClient = factory.CreateMqttClient();

        _options = new MqttClientOptionsBuilder()
            .WithTcpServer("localhost", 1883)
            .Build();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _mqttClient.ApplicationMessageReceivedAsync += e =>
        {
            var topic = e.ApplicationMessage.Topic;
            var payload = System.Text.Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

            try
            {
                // Парсим JSON {"value": 51.3, ...}
                using var doc = JsonDocument.Parse(payload);
                if (doc.RootElement.TryGetProperty("value", out var valElement))
                {
                    double val = valElement.GetDouble();
                    _sensorDataService.UpdateValue(topic, val);
                }
            }
            catch
            {
                // Если пришло обычное число без JSON
                if (double.TryParse(payload, out double val))
                {
                    _sensorDataService.UpdateValue(topic, val);
                }
            }

            return Task.CompletedTask;
        };

        await _mqttClient.ConnectAsync(_options, stoppingToken);
        await _mqttClient.SubscribeAsync("boiler/#", cancellationToken: stoppingToken);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}