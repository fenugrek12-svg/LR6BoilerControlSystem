using System;

namespace BoilerControlSystem
{
    public class WaterLevelSensor : Sensor
    {
        private static readonly Random _rnd = new Random();
        public double CurrentLevel { get; private set; }
        public override double Value => CurrentLevel;

        public WaterLevelSensor(string name) : base(name)
        {
            CurrentLevel = 50.0;
        }

        public override void Measure()
        {
            CurrentLevel += (_rnd.NextDouble() * 10.0 - 5.5);
            Console.WriteLine($"{Name} measured water level {CurrentLevel:F2}.");
        }
    }
}