using System.Collections.Concurrent;

namespace ScadaWebUI.Services;

public class SensorDataService : ISensorDataService
{
    private readonly ConcurrentDictionary<string, double> _values = new();
    private readonly List<SensorThresholdItem> _thresholds = new()
    {
        new SensorThresholdItem { Name = "Температура води (Temp)", Topic = "boiler/1/sensors/water/temperature", Max = 90 },
        new SensorThresholdItem { Name = "Тиск у котлі (Pressure)", Topic = "boiler/1/sensors/drum/pressure", Max = 10 },
        new SensorThresholdItem { Name = "Рівень води (Water Level)", Topic = "boiler/1/sensors/drum/level", Max = 85 }
    };

    public void UpdateValue(string topic, double value)
    {
        _values[topic] = value;
    }

    public IReadOnlyDictionary<string, double> GetAllValues()
    {
        return _values;
    }

    public IReadOnlyList<SensorThresholdItem> GetThresholds()
    {
        return _thresholds;
    }

    public bool UpdateMax(string topic, double max)
    {
        var item = _thresholds.FirstOrDefault(s => s.Topic == topic);
        if (item == null) return false;

        item.Max = max;
        return true;
    }
}