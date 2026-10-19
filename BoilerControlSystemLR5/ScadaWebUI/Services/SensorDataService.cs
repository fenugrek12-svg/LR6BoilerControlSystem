using System.Collections.Concurrent;

namespace ScadaWebUI.Services
{
    public interface ISensorDataService
    {
        void UpdateValue(string topic, double value);
        IReadOnlyDictionary<string, double> GetAllValues();
    }

    public class SensorDataService : ISensorDataService
    {
        // Потокобезопасный словарь: Ключ = топик (или имя датчика), Значение = текущее число
        private readonly ConcurrentDictionary<string, double> _data = new();

        public void UpdateValue(string topic, double value)
        {
            _data[topic] = value;
        }

        public IReadOnlyDictionary<string, double> GetAllValues()
        {
            return _data;
        }
    }
}