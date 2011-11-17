using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace CoopDrivingSim
{
    public class AutonomousCar : Car
    {
        public AutonomousCar(Vector2 position)
            : base(position)
        {
        }

        public override void Update()
        {
            //Hier steering behaviour toevoegen, bijvoorbeeld:
            Vector2 target = new Vector2(600, 300);
            Vector2 desiredVelocity = Vector2.Normalize(target - this.GPSPosition) * Car.MAX_VELOCITY; //Gebruik GPSPosition ipv daadwerkelijke positie.
            Vector2 steering = desiredVelocity - this.Velocity;

            this.Force = steering;

            //Informatie van andere auto's ophalen:
            foreach (Component2D component in Simulator.Components)
            {
                if (component is Car && component != this)
                {
                    //...
                }
            }

            //base.Update() laten staan om te zorgen dat opgegeven force vertaald wordt
            //naar versnelling, naar snelheid, naar positie, naar rotatie. En om
            //collisions waar te nemen.
            base.Update();
        }
    }
}
