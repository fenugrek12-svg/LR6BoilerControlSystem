using System;
using BoilerControlSystem.Systems;

namespace BoilerControlSystem
{
    public class Boiler
    {
        public TemperatureSensor WaterTempSensor { get; }
        public PressureSensor BoilerPressureSensor { get; }
        public WaterLevelSensor DrumWaterLevelSensor { get; }

        public BurnerSystem Burner { get; }
        public FeedWaterSystem WaterSupply { get; }
        public SafetyValveSystem SafetyValve { get; }

        public Boiler()
        {
            WaterTempSensor = new TemperatureSensor("Water temperature");
            BoilerPressureSensor = new PressureSensor("Boiler pressure");
            DrumWaterLevelSensor = new WaterLevelSensor("Drum water level");

            Burner = new BurnerSystem();
            WaterSupply = new FeedWaterSystem();
            SafetyValve = new SafetyValveSystem();
        }

        public void SimulateIteration(int iterationNumber)
        {
            Console.WriteLine($"\nIteration {iterationNumber}");

            WaterTempSensor.Measure();
            BoilerPressureSensor.Measure();
            DrumWaterLevelSensor.Measure();

            if (BoilerPressureSensor.CurrentPressure > 3.0)
            {
                SafetyValve.TurnOn();
                Burner.TurnOff();
            }
            else
            {
                SafetyValve.TurnOff();

                if (WaterTempSensor.CurrentTemperature < 70.0)
                {
                    Burner.TurnOn();
                }
                else
                {
                    Burner.TurnOff();
                }
            }

            if (DrumWaterLevelSensor.CurrentLevel < 40.0)
            {
                WaterSupply.TurnOn();
            }
            else if (DrumWaterLevelSensor.CurrentLevel > 65.0)
            {
                WaterSupply.TurnOff();
            }
        }
    }
}