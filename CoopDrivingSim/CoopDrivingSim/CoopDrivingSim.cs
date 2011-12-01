using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace CoopDrivingSim
{
    public class CoopDrivingSim : Game
    {
        private GraphicsDeviceManager graphics;
        private Car arrowCar;

        public const int ROAD_TOP = 280;
        public const int LANE_SEP = 360;
        public const int ROAD_BOTTOM = 440;
        public const int NARROW_START = 600;
        public const int NARROW_END = 1100;

        public CoopDrivingSim()
        {
            this.graphics = new GraphicsDeviceManager(this);
            this.Content.RootDirectory = "Content";
            this.IsMouseVisible = true;

            this.graphics.PreferredBackBufferWidth = 1280;
            this.graphics.PreferredBackBufferHeight = 720;
            this.graphics.IsFullScreen = false;
            this.graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            Simulator.InitializeSimulator(this.graphics, this.Content);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            Sprite road = new Sprite(Simulator.Content.Load<Texture2D>("Road"));
            road.Position = new Vector2(640, 360);
            this.arrowCar = new Car(new Vector2(200));
            Car obstacle = new Car(new Vector2(385));
            
            //Hier het scenario opbouwen, door bijvoorbeeld auto's aan te maken:
            //AutonomousCar autoCar = new AutonomousCar(new Vector2(300));
            //AutonomousCar anotherAutoCar = new AutonomousCar(new Vector2(300, 400));
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboard = Keyboard.GetState();
            
            if (keyboard.IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }

            //arrowCar besturing
            Vector2 force = Vector2.Zero;
            if (keyboard.IsKeyDown(Keys.Down)) force.Y += 50f;
            if (keyboard.IsKeyDown(Keys.Up)) force.Y -= 50f;
            if (keyboard.IsKeyDown(Keys.Right)) force.X += 50f;
            if (keyboard.IsKeyDown(Keys.Left)) force.X -= 50f;
            this.arrowCar.Force = force;
            //einde arrowCar besturing

            Simulator.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            Simulator.Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}
