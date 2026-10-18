using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using MQTTnet;
using BoilerControlSystem;

namespace BoilerControlSystem
{
    // Запис для прив'язки сенсора до MQTT-теми та одиниці вимірювання
    public record SensorConfiguration(
        Sensor Sensor,
        string Topic,
        string Unit
    );

    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=========================================================");
            Console.WriteLine(" ЛР4: Передавання даних за протоколом MQTT (Варіант 14)");
            Console.WriteLine("=========================================================");

            var boiler = new Boiler();

            // 1. Конфігурація MQTT-тем для сенсорів котла (Варіант 14)
            var sensorConfigs = new List<SensorConfiguration>
            {
                new SensorConfiguration(boiler.WaterTempSensor, "boiler/1/sensors/water/temperature", "°C"),
                new SensorConfiguration(boiler.BoilerPressureSensor, "boiler/1/sensors/drum/pressure", "bar"),
                new SensorConfiguration(boiler.DrumWaterLevelSensor, "boiler/1/sensors/drum/level", "%")
            };

            // 2. Створення та налаштування клієнта MQTT
            var factory = new MqttClientFactory();
            using var mqttClient = factory.CreateMqttClient();

            var options = new MqttClientOptionsBuilder()
                .WithTcpServer("127.0.0.1", 1883)
                .Build();

            Console.WriteLine("Підключення до MQTT-брокера (127.0.0.1:1883)...");
            try
            {
                await mqttClient.ConnectAsync(options);
                Console.WriteLine("Успішно підключено до брокера!\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка підключення до брокера: {ex.Message}");
                Console.WriteLine("Перевірте, чи запущена служба Mosquitto!");
                return;
            }

            // 3. Цикл роботи симулятора (10 ітерацій із затримкою 2 с)
            for (int iteration = 1; iteration <= 10; iteration++)
            {
                boiler.SimulateIteration(iteration);

                foreach (var config in sensorConfigs)
                {
                    // Формування JSON
                    string payload = JsonSerializer.Serialize(new
                    {
                        value = Math.Round(config.Sensor.Value, 2),
                        unit = config.Unit,
                        measuredAtUtc = DateTimeOffset.UtcNow
                    });

                    // Формування повідомлення MQTT
                    var message = new MqttApplicationMessageBuilder()
                        .WithTopic(config.Topic)
                        .WithPayload(payload)
                        .Build();

                    // Публікація в брокер
                    await mqttClient.PublishAsync(message);
                    Console.WriteLine($"[MQTT TX] -> {config.Topic} : {payload}");
                }

                await Task.Delay(TimeSpan.FromSeconds(2));
            }

            // 4. Завершення сесії
            await mqttClient.DisconnectAsync();
            Console.WriteLine("\nВсі дані передано. Клієнт відключено від брокера.");
        }
    }
}