using System;

namespace BoilerControlSystem
{
    public class PressureSensor : Sensor
    {
        private static readonly Random _rnd = new Random();
        public double CurrentPressure { get; private set; }
        public override double Value => CurrentPressure;

        public PressureSensor(string name) : base(name)
        {
            CurrentPressure = 2.0;
        }

        public override void Measure()
        {
            CurrentPressure += (_rnd.NextDouble() * 0.6 - 0.25);
            Console.WriteLine($"{Name} measured pressure {CurrentPressure:F2}.");
        }
    }
}