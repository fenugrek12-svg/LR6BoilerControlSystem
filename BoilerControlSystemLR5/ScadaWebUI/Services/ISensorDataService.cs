namespace ScadaWebUI.Services;

public class SensorThresholdItem
{
    public string Name { get; set; } = string.Empty;
    public string Topic { get; set; } = string.Empty;
    public double Max { get; set; }
}

public interface ISensorDataService
{
    void UpdateValue(string topic, double value);
    IReadOnlyDictionary<string, double> GetAllValues();
    IReadOnlyList<SensorThresholdItem> GetThresholds();
    bool UpdateMax(string topic, double max);
}