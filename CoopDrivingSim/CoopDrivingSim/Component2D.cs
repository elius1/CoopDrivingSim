using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace CoopDrivingSim
{
    /// <summary>
    /// A base class for all 2D components.
    /// </summary>
    public abstract class Component2D
    {
        private Vector2 position = Vector2.Zero;
        /// <summary>
        /// Gets or sets the position of this component in pixels.
        /// </summary>
        public virtual Vector2 Position
        {
            get { return this.position; }
            set { this.position = value; }
        }

        private int width = 0;
        /// <summary>
        /// Gets or sets the width of this component in pixels.
        /// </summary>
        public virtual int Width
        {
            get { return this.width; }
            set { this.width = value; }
        }

        private int height = 0;
        /// <summary>
        /// Gets or sets the height of this component in pixels.
        /// </summary>
        public virtual int Height
        {
            get { return this.height; }
            set { this.height = value; }
        }

        /// <summary>
        /// Gets or sets whether this component is visible or not.
        /// </summary>
        public bool Visible = true;
        
        /// <summary>
        /// Initializes the component.
        /// </summary>
        public Component2D()
        {
            Simulator.Components.Add(this);
        }

        /// <summary>
        /// Updates the component.
        /// </summary>
        public virtual void Update()
        {
        }

        /// <summary>
        /// Draws the component.
        /// </summary>
        public virtual void Draw()
        {
        }

        /// <summary>
        /// Destroys the component by removing it from the simulator.
        /// </summary>
        public virtual void Dispose()
        {
            Simulator.Components.Remove(this);
        }
    }
}
