using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CoopDrivingSim
{
    public class Sprite : Component2D
    {
        private Texture2D spriteTexture;
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

        public virtual Vector2 Origin { get; protected set; }

        public virtual float Rotation { get; set; }

        private SpriteEffects effects = SpriteEffects.None;
        public virtual SpriteEffects Effects
        {
            get { return this.effects; }
            set { this.effects = value; }
        }

        public Sprite(Texture2D spriteTexture)
            : base()
        {
            this.SpriteTexture = spriteTexture;
        }

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
