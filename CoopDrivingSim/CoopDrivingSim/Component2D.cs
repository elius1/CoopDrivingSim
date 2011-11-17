using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace CoopDrivingSim
{
    public class Component2D
    {
        private Vector2 position = Vector2.Zero;
        public virtual Vector2 Position
        {
            get { return this.position; }
            set { this.position = value; }
        }

        private int width = 0;
        public virtual int Width
        {
            get { return this.width; }
            set { this.width = value; }
        }

        private int height = 0;
        public virtual int Height
        {
            get { return this.height; }
            set { this.height = value; }
        }

        public bool Visible = true;
        
        public Component2D()
        {
            Simulator.Components.Add(this);
        }

        public virtual void Update()
        {
        }

        public virtual void Draw()
        {
        }
    }
}
