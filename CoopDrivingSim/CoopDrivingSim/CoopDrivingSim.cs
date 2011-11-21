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

        public CoopDrivingSim()
        {
            this.graphics = new GraphicsDeviceManager(this);
            this.Content.RootDirectory = "Content";
            this.IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Simulator.InitializeSimulator(this.graphics, this.Content);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            this.arrowCar = new Car(new Vector2(200));
            Car obstacle = new Car(new Vector2(500));
            
            //Hier het scenario opbouwen, door bijvoorbeeld auto's aan te maken:
            AutonomousCar autoCar = new AutonomousCar(new Vector2(300));
            AutonomousCar anotherAutoCar = new AutonomousCar(new Vector2(300, 400));
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
