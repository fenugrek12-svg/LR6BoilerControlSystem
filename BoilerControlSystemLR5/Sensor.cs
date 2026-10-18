namespace BoilerControlSystem
{
    public abstract class Sensor
    {
        public string Name { get; protected set; }
        public abstract double Value { get; }

        protected Sensor(string name)
        {
            Name = name;
        }

        public abstract void Measure();
    }
}