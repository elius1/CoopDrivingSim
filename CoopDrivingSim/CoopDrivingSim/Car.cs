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

        public Car(Vector2 position)
            : base(Simulator.Content.Load<Texture2D>("Car"))
        {
            this.Position = position;
        }

        public override void Update()
        {
            //Movement
            float secondsElapsed = (float)Simulator.SimTime.ElapsedGameTime.TotalSeconds;

            // right place?
            this.force = this.Seek(new Vector2(40, 40));

            this.force = Util.Truncate(this.force, Car.MAX_FORCE);

            this.acceleration = this.force / this.mass;

            this.velocity += this.acceleration * secondsElapsed;
            this.velocity = Util.Truncate(this.velocity, Car.MAX_VELOCITY);
                        
            this.Position += this.Velocity * secondsElapsed;

            if (this.velocity.Length() != 0)
            {
                this.Rotation = (float)Math.Atan(this.velocity.Y / this.velocity.X);
            }

            //Collision Detection
            float collisionDist = (float)Math.Sqrt(Math.Pow(this.Width, 2) + Math.Pow(this.Height, 2));
            foreach (Component2D component in Simulator.Components)
            {
                if (component is Car && component != this)
                {
                    float dist = (this.Position - component.Position).Length();
                    if (dist < collisionDist)
                    {
                        Console.WriteLine("!!!Collision!!!");
                    }
                }
            }

            base.Update();
        }

        public Vector2 Seek(Vector2 target)
        {
            Vector2 desiredVelocity = this.Position - target;
            desiredVelocity.Normalize();
            desiredVelocity = desiredVelocity * MAX_VELOCITY;

            Vector2 steering = desiredVelocity - velocity;


            return steering;
        }

        /*
        public Vector2 Pursuit(Car quarry)
        {
        }

        public Vector2 Arrive(Vector2 target)
        {
        }

        public Vector2 AvoidObstacles()
        {
        }

        public Vector2 FollowLeader(Car leader)
        {
        }

        public Vector2 FollowPath()
        {
        }
         
        public Vector2 Interpose(Car car1, Car car2)
        {
        }
        */
    }
}
