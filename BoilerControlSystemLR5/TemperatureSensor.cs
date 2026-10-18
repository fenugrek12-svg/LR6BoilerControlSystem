using System;

namespace BoilerControlSystem
{
    public class TemperatureSensor : Sensor
    {
        private static readonly Random _rnd = new Random();
        public double CurrentTemperature { get; private set; }
        public override double Value => CurrentTemperature;

        public TemperatureSensor(string name) : base(name)
        {
            CurrentTemperature = 65.0;
        }

        public override void Measure()
        {
            CurrentTemperature += (_rnd.NextDouble() * 8.0 - 3.5);
            Console.WriteLine($"{Name} measured temperature {CurrentTemperature:F2}.");
        }
    }
}