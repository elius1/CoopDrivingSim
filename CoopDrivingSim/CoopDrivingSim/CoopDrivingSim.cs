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

        private float CreateTimeElapsed = 0f;

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

            Vector2 test = Vector2.Zero;
            Vector2.Normalize(test);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            Sprite road = new Sprite(Simulator.Content.Load<Texture2D>("Road"));
            road.Position = new Vector2(640, 360);

            Statistics statistics = new Statistics();
        }

        protected override void Update(GameTime gameTime)
        {
            this.CreateTimeElapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (this.CreateTimeElapsed > Simulator.CarGenerationRate)
            {
                this.CreateTimeElapsed = 0f;
                Vector2 carPos = new Vector2(-50, 0);
                Random rand = new Random();
                if (rand.Next(0, 2) == 1)
                {
                    carPos.Y = Road.TOP_LANE;
                }
                else
                {
                    carPos.Y = Road.BOTTOM_LANE;
                }
                new AutonomousCar(carPos);
            }

            KeyboardState keyboard = Keyboard.GetState();

            if (keyboard.IsKeyDown(Keys.D1)) Simulator.CarGenerationRate -= 0.01f;
            if (keyboard.IsKeyDown(Keys.D2)) Simulator.CarGenerationRate += 0.01f;
            if (keyboard.IsKeyDown(Keys.Q)) Simulator.PathFollowingStimulus -= 10f;
            if (keyboard.IsKeyDown(Keys.W)) Simulator.PathFollowingStimulus += 10f;
            if (keyboard.IsKeyDown(Keys.A)) Simulator.SeparationStimulus -= 10f;
            if (keyboard.IsKeyDown(Keys.S)) Simulator.SeparationStimulus += 10f;
            if (keyboard.IsKeyDown(Keys.Z)) Simulator.LeaderFollowingStimulus -= 10f;
            if (keyboard.IsKeyDown(Keys.X)) Simulator.LeaderFollowingStimulus += 10f;

            if (keyboard.IsKeyDown(Keys.R))
            {
                List<Component2D> delete = new List<Component2D>();
                foreach (Component2D component in Simulator.Components)
                {
                    if (component is Car) delete.Add(component);
                }
                foreach (Component2D component in delete) component.Dispose();
                Simulator.CarsFinished = 0;
                Simulator.Crashes = 0;
            }
            if (keyboard.IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }

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
