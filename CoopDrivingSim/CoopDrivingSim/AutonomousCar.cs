using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace CoopDrivingSim
{

    public class AutonomousCar : Car
    {
        enum Behaviours {Seek, Pursuit, Arrive, AvoidObstacles, FollowLeader, FollowPath, Interpose};

        // behaviour
        private Behaviours behaviour;

        public AutonomousCar(Vector2 position)
            : base(position)
        {
        }

        public override void Update()
        {
            Car quarry = null;
            
            //Informatie van andere auto's ophalen:
            foreach (Component2D component in Simulator.Components)
            {
                if (component is Car && component != this)
                {
                    quarry = (Car) component;
                }
            }

            this.behaviour = Behaviours.Arrive;

            switch (behaviour)
            {
                case Behaviours.Seek:
                    this.Force = Seek(new Vector2(600, 400));
                    break;
                case Behaviours.Arrive:
                    this.Force = Arrive(new Vector2(700, 500));
                    break;
                case Behaviours.Pursuit:
                    this.Force = Pursuit(quarry);
                    break;
                case Behaviours.FollowLeader:
                    this.Force = FollowLeader(quarry);
                    break;
                case Behaviours.Interpose:
                    //this.Force = Interpose(null, null);
                    break;
                case Behaviours.FollowPath:
                default:
                    //this.FollowPath();
                    break;
            }

            

            //base.Update() laten staan om te zorgen dat opgegeven force vertaald wordt
            //naar versnelling, naar snelheid, naar positie, naar rotatie. En om
            //collisions waar te nemen.
            base.Update();
        }

        private Vector2 PredictFuturePosition(Car car, float T)
        {
            return car.Velocity * T;
        }

        public Vector2 Seek(Vector2 target)
        {
            Vector2 desiredVelocity = Vector2.Normalize(target - this.GPSPosition) * Car.MAX_VELOCITY; //Gebruik GPSPosition ipv daadwerkelijke positie.
            Vector2 steering = desiredVelocity - this.Velocity;

            return steering;
        }

        public Vector2 Pursuit(Car quarry)
        {
            Vector2 distance = quarry.GPSPosition - this.GPSPosition;
            // T is predicted time until interception
            float T = distance.Length() / (this.Velocity - quarry.Velocity).Length();
 
            // seek predicted position
            return Seek(PredictFuturePosition(quarry, T));
        }

        public Vector2 Arrive(Vector2 target)
        {
            float slowingDistance = 300;

            Vector2 targetOffset = target - this.GPSPosition;
            float distance = targetOffset.Length();
            float rampedSpeed = Car.MAX_VELOCITY * (distance / slowingDistance);
            float clippedSpeed = Math.Min(rampedSpeed, Car.MAX_VELOCITY);
            Vector2 desiredVelocity = (clippedSpeed / distance) * targetOffset;
            
            return desiredVelocity - this.Velocity;
        }

        public Vector2 FollowLeader(Car leader)
        {
            float distanceOffset = 0.1f;
            return Arrive(leader.GPSPosition - distanceOffset * leader.Velocity);
        }

        public Vector2 Interpose(Car car1, Car car2)
        {
            float distance = (((car1.GPSPosition + car2.GPSPosition) / 2) - this.GPSPosition).Length();
            float T = distance / this.Velocity.Length();

            Vector2 desiredPosition = (PredictFuturePosition(car1, T) + PredictFuturePosition(car2, T)) / 2;
            return Seek(desiredPosition);
        }

        /*
        public Vector2 AvoidObstacles()
        {
        }

        public Vector2 FollowPath()
        {
        }

        public Vector2 Queue()
        {
            // seek, braking and separation
        }

        public Vector2 Separate()
        {

        }
        */
    }
}
