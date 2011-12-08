using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace CoopDrivingSim
{
    /// <summary>
    /// A Car that is controlled by steering behaviors.
    /// </summary>
    public class AutonomousCar : Car
    {
        private List<Car> neighborhood = new List<Car>();
        private float targetY;

        /// <summary>
        /// Initializes an autonomous car using the specified starting position.
        /// </summary>
        /// <param name="position">The position the car should start.</param>
        public AutonomousCar(Vector2 position)
            : base(position, new Vector2(100,0))
        {
            //Remember the initial lane of this car.
            this.targetY = position.Y;
        }

        /// <summary>
        /// Updates the autonomous car's force using steering behaviors.
        /// </summary>
        public override void Update()
        {
            //Get the current neighborhood.
            this.neighborhood.Clear();
            foreach (Component2D component in Simulator.Components)
            {
                if (component is Car && component != this)
                {
                    Car car = component as Car;
                    //Only use cars within the neighborhood radius.
                    if ((car.GPSPosition - this.GPSPosition).Length() < 150f) this.neighborhood.Add(component as Car);
                }
            }

            //Take the sum of all steering behaviors times their stimulus as the new steering force.
            this.Force = Vector2.Zero;
            this.Force += this.Seek(new Vector2(2000, this.targetY)); //Seek behavior
            this.Force += this.FollowPath() * Simulator.PathFollowingStimulus; //Path following behavior
            this.Force += this.Separate() * Simulator.SeparationStimulus; //Separation behavior
            Car shouldFollow = ShouldFollow();
            //if (shouldFollow != null) this.Force += this.FollowLeader(shouldFollow) * Simulator.LeaderFollowingStimulus; //Old leader following behavior
            if (shouldFollow != null && this.targetY == Road.TOP_LANE) this.Force += this.Arrive(new Vector2(Road.NARROW_START + 100, Road.TOP_LANE)) * Simulator.LeaderFollowingStimulus; //Leader following/breaking behavior

            base.Update();
        }

        /// <summary>
        /// Predicts the position of the specified car after the specified time.
        /// </summary>
        /// <param name="car">The car of which the position should be predicted.</param>
        /// <param name="t">The point in time at which the position should be predicted, in seconds.</param>
        /// <returns>The predicted future position.</returns>
        private static Vector2 PredictFuturePosition(Car car, float t)
        {
            return car.GPSPosition + car.Velocity * t;
        }

        /// <summary>
        /// Executes the seeking behavior.
        /// </summary>
        /// <param name="target">The position that this behavior should 'seek'.</param>
        /// <returns>The seeking steering force.</returns>
        private Vector2 Seek(Vector2 target)
        {
            Vector2 desiredVelocity = Vector2.Normalize(target - this.GPSPosition) * Car.MAX_VELOCITY;
            Vector2 steering = desiredVelocity - this.Velocity;

            return steering;
        }

        /// <summary>
        /// Executes the pursuit behavior.
        /// </summary>
        /// <param name="quarry">The car that should be pursuited.</param>
        /// <returns>The pursuit steering force.</returns>
        private Vector2 Pursuit(Car quarry)
        {
            Vector2 distance = quarry.GPSPosition - this.GPSPosition;
            //t is predicted time until interception.
            float t = distance.Length() / (this.Velocity - quarry.Velocity).Length();
 
            //Seek predicted position.
            return Seek(AutonomousCar.PredictFuturePosition(quarry, t));
        }

        /// <summary>
        /// Executes the arrival behavior.
        /// </summary>
        /// <param name="target">The position at which to arrive.</param>
        /// <returns>The arrival steering force.</returns>
        private Vector2 Arrive(Vector2 target)
        {
            float slowingDistance = 300;

            Vector2 targetOffset = target - this.GPSPosition;
            float distance = targetOffset.Length();
            float rampedSpeed = Car.MAX_VELOCITY * (distance / slowingDistance);
            float clippedSpeed = Math.Min(rampedSpeed, Car.MAX_VELOCITY);
            Vector2 desiredVelocity = (clippedSpeed / distance) * targetOffset;
            
            return desiredVelocity - this.Velocity;
        }

        /// <summary>
        /// Executes the leader following behavior.
        /// </summary>
        /// <param name="leader">The car that is designated as leader.</param>
        /// <returns>The leader following steering force.</returns>
        private Vector2 FollowLeader(Car leader)
        {
            float distanceOffset = this.CollisionDist;
            return Arrive(leader.GPSPosition - distanceOffset * leader.Velocity);
        }

        /// <summary>
        /// Executes the interpose behavior.
        /// </summary>
        /// <param name="car1">The first car.</param>
        /// <param name="car2">The second car.</param>
        /// <returns>The interpose steering force.</returns>
        private Vector2 Interpose(Car car1, Car car2)
        {
            float distance = (((car1.GPSPosition + car2.GPSPosition) / 2) - this.GPSPosition).Length();
            float T = distance / this.Velocity.Length();

            Vector2 desiredPosition = (PredictFuturePosition(car1, T) + PredictFuturePosition(car2, T)) / 2;
            return Seek(desiredPosition);
        }

        /// <summary>
        /// Not implemented!
        /// </summary>
        /// <returns></returns>
        private Vector2 AvoidObstacles()
        {
            return Vector2.Zero;
        }

        /// <summary>
        /// Executes the path following behavior.
        /// </summary>
        /// <returns>The path following steering force.</returns>
        private Vector2 FollowPath()
        {
            Vector2 futurePos = AutonomousCar.PredictFuturePosition(this, (float)Simulator.SimTime.ElapsedGameTime.TotalSeconds);

            float outside = AutonomousCar.OnPath(futurePos);
            bool correctDirection = futurePos.X >= this.GPSPosition.X;

            if (outside == 0 && correctDirection) //no correction is needed
            {
                return Vector2.Zero;
            }
            else //correction is needed
            {
                Vector2 target = new Vector2(futurePos.X, Road.TOP_LANE);
                return this.Seek(target);
            }
        }

        /// <summary>
        /// Determines if the specified position is on the road.
        /// </summary>
        /// <param name="futurePos">The position that should be checked.</param>
        /// <returns>A float indicating how far the Y position is from the road.</returns>
        private static float OnPath(Vector2 futurePos)
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

        /// <summary>
        /// Not implemented!
        /// </summary>
        /// <returns></returns>
        private Vector2 Queue()
        {
            return Vector2.Zero;// seek, braking and separation
        }

        /// <summary>
        /// Executes the separation behavior.
        /// </summary>
        /// <returns>The separation steering force.</returns>
        private Vector2 Separate()
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

        /// <summary>
        /// Determines whether this car should follow another car or not.
        /// </summary>
        /// <returns>The car that should be followed. Null if no car should be followed.</returns>
        private Car ShouldFollow()
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
