using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CoopDrivingSim
{
    public class Car : Sprite
    {
        public const float MAX_VELOCITY = 100f;
        public const float MAX_FORCE = 100f;

        public Vector2 GPSPosition
        {
            get { return this.Position; }
        }

        private float mass = 1f;
        public float Mass
        {
            get { return this.mass; }
            set { this.mass = value; }
        }

        private Vector2 force = Vector2.Zero;
        public Vector2 Force
        {
            get { return this.force; }
            set { this.force = value; }
        }

        private Vector2 acceleration = Vector2.Zero;
        public Vector2 Acceleration
        {
            get { return this.acceleration; }
        }

        private Vector2 velocity = Vector2.Zero;
        public Vector2 Velocity
        {
            get { return this.velocity; }
        }

        private float collisionDist;
        public float CollisionDist
        {
            get { return this.collisionDist; }
        }

        private List<Car> collidedCars = new List<Car>();

        public Car(Vector2 position, Vector2 startingVelocity)
            : base(Simulator.Content.Load<Texture2D>("Car2"))
        {
            this.Position = position;
            this.velocity = startingVelocity;
            this.collisionDist = (float)Math.Sqrt(Math.Pow(this.Width, 2) + Math.Pow(this.Height, 2));
        }

        public override void Update()
        {
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

        public override void Dispose()
        {
            Simulator.CarsFinished++;
            base.Dispose();
        }
    }
}
