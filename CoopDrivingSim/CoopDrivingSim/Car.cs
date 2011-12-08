using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CoopDrivingSim
{
    /// <summary>
    /// A Car sprite that updates its own position based on newton's 2nd law.
    /// </summary>
    public class Car : Sprite
    {
        /// <summary>
        /// A Car's maximum velocity.
        /// </summary>
        public const float MAX_VELOCITY = 100f;
        /// <summary>
        /// A Car's maximum steering force.
        /// </summary>
        public const float MAX_FORCE = 100f;

        /// <summary>
        /// Gets the position of this car according to the GPS.
        /// </summary>
        public Vector2 GPSPosition
        {
            get { return this.Position; }
        }

        private float mass = 1f;
        /// <summary>
        /// Gets or sets the mass of this car.
        /// </summary>
        public float Mass
        {
            get { return this.mass; }
            set { this.mass = value; }
        }

        private Vector2 force = Vector2.Zero;
        /// <summary>
        /// Gets or sets the steering force of this car.
        /// </summary>
        public Vector2 Force
        {
            get { return this.force; }
            set { this.force = value; }
        }

        private Vector2 acceleration = Vector2.Zero;
        /// <summary>
        /// Gets the acceleration of this car.
        /// </summary>
        public Vector2 Acceleration
        {
            get { return this.acceleration; }
        }

        private Vector2 velocity = Vector2.Zero;
        /// <summary>
        /// Gets the velocity of this car.
        /// </summary>
        public Vector2 Velocity
        {
            get { return this.velocity; }
        }

        private float collisionDist;
        /// <summary>
        /// Gets the distance at which a collision between cars is measured.
        /// </summary>
        public float CollisionDist
        {
            get { return this.collisionDist; }
        }

        private List<Car> collidedCars = new List<Car>();

        /// <summary>
        /// Initializes the Car at the specified position with the specified starting velocity.
        /// </summary>
        /// <param name="position">The position at which this car should start.</param>
        /// <param name="startingVelocity">The initial velocity of this car.</param>
        public Car(Vector2 position, Vector2 startingVelocity)
            : base(Simulator.Content.Load<Texture2D>("Car2"))
        {
            this.Position = position;
            this.velocity = startingVelocity;
            this.collisionDist = (float)Math.Sqrt(Math.Pow(this.Width, 2) + Math.Pow(this.Height, 2));
        }

        /// <summary>
        /// Updates the car's position using the current steering force.
        /// </summary>
        public override void Update()
        {
            //Delete cars that have reached the end of the road.
            if (this.Position.X > 1300) this.Dispose();
            
            //Movement
            float secondsElapsed = (float)Simulator.SimTime.ElapsedGameTime.TotalSeconds;
            
            this.force = Util.Truncate(this.force, Car.MAX_FORCE);

            this.acceleration = this.force / this.mass;

            this.velocity += this.acceleration * secondsElapsed;
            this.velocity = Util.Truncate(this.velocity, Car.MAX_VELOCITY);
                        
            this.Position += this.Velocity * secondsElapsed;

            if (this.velocity.Length() != 0)
            {
                this.Rotation = (float)Math.Atan2(this.velocity.Y, this.velocity.X);
            }

            //Collision Detection            
            foreach (Component2D component in Simulator.Components)
            {
                if (component is Car && component != this)
                {
                    float dist = (this.Position - component.Position).Length();
                    if (dist < this.collisionDist)
                    {
                        if (this.collidedCars.Contains(component as Car))
                        {
                        }
                        else
                        {
                            this.collidedCars.Add(component as Car);
                            Simulator.Crashes++;
                            Console.WriteLine("!!!Collision!!!");
                        }
                    }
                    else
                    {
                        if (this.collidedCars.Contains(component as Car))
                        {
                            this.collidedCars.Remove(component as Car);
                        }
                        else
                        {
                        }
                    }
                }
            }

            base.Update();
        }

        /// <summary>
        /// Count this car as finished and then destroy it.
        /// </summary>
        public override void Dispose()
        {
            Simulator.CarsFinished++;
            base.Dispose();
        }
    }
}
