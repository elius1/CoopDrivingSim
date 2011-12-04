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
        private List<Car> neighborhood = new List<Car>();
        private float targetY;

        public AutonomousCar(Vector2 position)
            : base(position, new Vector2(100,0))
        {
            this.targetY = position.Y;
        }

        public override void Update()
        {
            Car quarry = null;

            //Console.WriteLine(this.Velocity);
            this.neighborhood.Clear();
            //Informatie van andere auto's ophalen:
            foreach (Component2D component in Simulator.Components)
            {
                if (component is Car && component != this)
                {
                    this.neighborhood.Add(component as Car);
                    quarry = (Car) component;
                }
            }

            this.behaviour = Behaviours.FollowPath;

            this.Force += this.Seek(new Vector2(2000, this.targetY));
            Vector2 followPath = this.FollowPath() * Simulator.PathFollowingStimulus;
            //Console.WriteLine("pat: " + followPath);
            this.Force += followPath;
            Vector2 separate = this.Separate() * Simulator.SeparationStimulus;
            //Console.WriteLine("sep: " + separate);
            this.Force += separate;

            Car shouldFollow = ShouldFollow();
            if (shouldFollow != null) this.Force += this.FollowLeader(shouldFollow) * Simulator.LeaderFollowingStimulus;

            //switch (behaviour)
            //{
            //    case Behaviours.Seek:
            //        this.Force = Seek(new Vector2(600, 400));
            //        break;
            //    case Behaviours.Arrive:
            //        this.Force = Arrive(new Vector2(700, 500));
            //        break;
            //    case Behaviours.Pursuit:
            //        this.Force = Pursuit(quarry);
            //        break;
            //    case Behaviours.FollowLeader:
            //        this.Force = FollowLeader(quarry);
            //        break;
            //    case Behaviours.Interpose:
            //        //this.Force = Interpose(null, null);
            //        break;
            //    case Behaviours.FollowPath:
            //        this.Force += this.FollowPath();
            //        break;
            //    default:
            //        //this.FollowPath();
            //        break;
            //}

            

            //base.Update() laten staan om te zorgen dat opgegeven force vertaald wordt
            //naar versnelling, naar snelheid, naar positie, naar rotatie. En om
            //collisions waar te nemen.
            base.Update();
        }

        private static Vector2 PredictFuturePosition(Car car, float T)
        {
            return car.GPSPosition + car.Velocity * T;
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
            float distanceOffset = this.CollisionDist;
            return Arrive(leader.GPSPosition - distanceOffset * leader.Velocity);
        }

        public Vector2 Interpose(Car car1, Car car2)
        {
            float distance = (((car1.GPSPosition + car2.GPSPosition) / 2) - this.GPSPosition).Length();
            float T = distance / this.Velocity.Length();

            Vector2 desiredPosition = (PredictFuturePosition(car1, T) + PredictFuturePosition(car2, T)) / 2;
            return Seek(desiredPosition);
        }

        public Vector2 AvoidObstacles()
        {
            return Vector2.Zero;
        }

        public Vector2 FollowPath()
        {
            Vector2 futurePos = AutonomousCar.PredictFuturePosition(this, (float)Simulator.SimTime.ElapsedGameTime.TotalSeconds);

            float outside = this.onPath(futurePos);
            bool correctDirection = futurePos.X >= this.GPSPosition.X;

            if (outside == 0 && correctDirection)
            {
                return Vector2.Zero;
            }
            else
            {
                Vector2 target = new Vector2(futurePos.X, Road.TOP_LANE);
                return this.Seek(target);
            }
        }

        public float onPath(Vector2 futurePos)
        {
            if (futurePos.X > Road.NARROW_START - 50 && futurePos.X < Road.NARROW_END - 50)
            {
                if (futurePos.Y < Road.TOP_LANE - Road.LANE_RADIUS / 2)
                {
                    return (Road.TOP_LANE - Road.LANE_RADIUS / 2) - futurePos.Y;
                }
                else if (futurePos.Y > Road.TOP_LANE + Road.LANE_RADIUS / 2)
                {
                    return (Road.TOP_LANE + Road.LANE_RADIUS / 2) + futurePos.Y;
                }
                else
                {
                    return 0f;
                }
            }
            else
            {
                if (futurePos.Y < Road.LANE_SEP - Road.LANE_RADIUS)
                {
                    return (Road.LANE_SEP - Road.LANE_RADIUS) - futurePos.Y;
                }
                else if (futurePos.Y > Road.LANE_SEP + Road.LANE_RADIUS)
                {
                    return (Road.LANE_SEP + Road.LANE_RADIUS) + futurePos.Y;
                }
                else
                {
                    return 0f;
                }
            }
        }

        public Vector2 Queue()
        {
            return Vector2.Zero;// seek, braking and separation
        }

        public Vector2 Separate()
        {            
            Vector2 separation = Vector2.Zero;
            foreach (Car car in this.neighborhood)
            {
                Vector2 distance = this.GPSPosition - car.GPSPosition;
                float r = distance.Length() - this.CollisionDist;
                distance.Normalize();
                distance *= (1 / (float)Math.Pow(r, 2));
                separation += distance;
            }

            return separation;
        }

        public Car ShouldFollow()
        {
            if (this.GPSPosition.X > Road.NARROW_START - 150 && this.GPSPosition.X < Road.NARROW_START + 100)
            {
                foreach (Car car in this.neighborhood)
                {
                    if (car.GPSPosition.X > this.GPSPosition.X && car.GPSPosition.Y > this.GPSPosition.Y
                        && car.GPSPosition.X < Road.NARROW_START + 100)
                    {
                        return car;
                    }
                }
                return null;
            }
            else
            {
                return null;
            }
        }
    }
}
