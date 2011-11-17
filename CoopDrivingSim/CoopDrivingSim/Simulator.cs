using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace CoopDrivingSim
{
    public static class Simulator
    {
        private static bool isInitialized = false;
        public static bool IsInitialized
        {
            get { return Simulator.isInitialized; }
        }

        public static GraphicsDevice GraphicsDevice;

        public static SpriteBatch SpriteBatch;

        public static ContentManager Content;

        public static GameTime SimTime;

        public static List<Component2D> Components;

        public static Color ClearColor = Color.CornflowerBlue;

        public static void InitializeSimulator(IGraphicsDeviceService graphicsDeviceService, ContentManager content)
        {
            Simulator.GraphicsDevice = graphicsDeviceService.GraphicsDevice;
            Simulator.SpriteBatch = new SpriteBatch(Simulator.GraphicsDevice);
            Simulator.Content = content;
            Simulator.Components = new List<Component2D>();

            Simulator.isInitialized = true;
        }

        public static void Update(GameTime simTime)
        {
            Simulator.SimTime = simTime;

            List<Component2D> updating = new List<Component2D>();
            foreach (Component2D component in Simulator.Components)
            {
                updating.Add(component);
            }

            foreach (Component2D component in updating)
            {
                component.Update();
            }
        }

        public static void Draw(GameTime simTime)
        {
            Simulator.SimTime = simTime;

            Simulator.GraphicsDevice.Clear(Simulator.ClearColor);

            List<Component2D> drawing = new List<Component2D>();
            foreach (Component2D component in Simulator.Components)
            {
                if (component.Visible)
                {
                    drawing.Add(component);
                }
            }

            Simulator.SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.None);
            foreach (Component2D component in drawing)
            {
                component.Draw();
            }
            Simulator.SpriteBatch.End();
        }
    }
}
