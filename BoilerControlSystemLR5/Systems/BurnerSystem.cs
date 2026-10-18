using System;

namespace BoilerControlSystem.Systems
{
    public class BurnerSystem
    {
        public bool IsActive { get; private set; }

        public void TurnOn()
        {
            if (!IsActive)
            {
                IsActive = true;
                Console.WriteLine("BurnerSystem turned on.");
            }
        }

        public void TurnOff()
        {
            if (IsActive)
            {
                IsActive = false;
                Console.WriteLine("BurnerSystem turned off.");
            }
        }
    }
}