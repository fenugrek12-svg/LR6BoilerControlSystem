using System;

namespace BoilerControlSystem.Systems
{
    public class FeedWaterSystem
    {
        public bool IsActive { get; private set; }

        public void TurnOn()
        {
            if (!IsActive)
            {
                IsActive = true;
                Console.WriteLine("FeedWaterSystem turned on.");
            }
        }

        public void TurnOff()
        {
            if (IsActive)
            {
                IsActive = false;
                Console.WriteLine("FeedWaterSystem turned off.");
            }
        }
    }
}