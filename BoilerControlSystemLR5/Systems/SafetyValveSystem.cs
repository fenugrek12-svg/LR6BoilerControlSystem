using System;

namespace BoilerControlSystem.Systems
{
    public class SafetyValveSystem
    {
        public bool IsActive { get; private set; }

        public void TurnOn()
        {
            if (!IsActive)
            {
                IsActive = true;
                Console.WriteLine("SafetyValveSystem turned on.");
            }
        }

        public void TurnOff()
        {
            if (IsActive)
            {
                IsActive = false;
                Console.WriteLine("SafetyValveSystem turned off.");
            }
        }
    }
}