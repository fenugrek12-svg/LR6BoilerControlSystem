namespace ScadaWebUI.Data;

public class SensorSample
{
    public int Id { get; set; }
    public string Topic { get; set; } = string.Empty;
    public double Value { get; set; }
    public string Unit { get; set; } = string.Empty;
    public DateTimeOffset MeasuredAtUtc { get; set; }
    public string SavedBy { get; set; } = string.Empty;
}