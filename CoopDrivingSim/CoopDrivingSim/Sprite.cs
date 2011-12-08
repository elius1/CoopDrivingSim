using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CoopDrivingSim
{
    /// <summary>
    /// Represents a 2D texture in 2D space.
    /// </summary>
    public class Sprite : Component2D
    {
        private Texture2D spriteTexture;
        /// <summary>
        /// Gets or sets the texture that this sprite should display.
        /// </summary>
        public virtual Texture2D SpriteTexture
        {
            get { return this.spriteTexture; }
            set
            {
                this.spriteTexture = value;
                this.Width = value.Width;
                this.Height = value.Height;
                this.Origin = new Vector2(this.Width / 2f, this.Height / 2f);
            }
        }

        /// <summary>
        /// The point around which this sprite should rotate.
        /// </summary>
        public virtual Vector2 Origin { get; protected set; }

        /// <summary>
        /// The current rotation of this sprite in radians.
        /// </summary>
        public virtual float Rotation { get; set; }

        private SpriteEffects effects = SpriteEffects.None;
        /// <summary>
        /// The sprite effects that this sprite should use when drawing.
        /// </summary>
        public virtual SpriteEffects Effects
        {
            get { return this.effects; }
            set { this.effects = value; }
        }

        /// <summary>
        /// Initializes the sprite using the specified texture.
        /// </summary>
        /// <param name="spriteTexture"></param>
        public Sprite(Texture2D spriteTexture)
            : base()
        {
            this.SpriteTexture = spriteTexture;
        }

        /// <summary>
        /// Draws this sprite.
        /// </summary>
        public override void Draw()
        {
            Rectangle destination = new Rectangle((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height);

            Simulator.SpriteBatch.Draw(
                this.SpriteTexture,
                destination,
                null,
                Color.White,
                this.Rotation,
                this.Origin,
                this.Effects,
                0);

            base.Draw();
        }
    }
}
