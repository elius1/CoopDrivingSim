using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace CoopDrivingSim
{
    /// <summary>
    /// A simulator that can update and draw a collection of components.
    /// </summary>
    public static class Simulator
    {
        private static bool isInitialized = false;
        /// <summary>
        /// Gets whether the simulator is initialized or not.
        /// </summary>
        public static bool IsInitialized
        {
            get { return Simulator.isInitialized; }
        }

        /// <summary>
        /// The graphics device that the simulator is using.
        /// </summary>
        public static GraphicsDevice GraphicsDevice;

        /// <summary>
        /// The sprite batch that the simulator is using.
        /// </summary>
        public static SpriteBatch SpriteBatch;

        /// <summary>
        /// The content manager that the simulator is using.
        /// </summary>
        public static ContentManager Content;

        /// <summary>
        /// The current simulator time.
        /// </summary>
        public static GameTime SimTime;
        public static float TotalMinutesSinceLastReset = 0f;

        /// <summary>
        /// A list containing all components in the simulator.
        /// </summary>
        public static List<Component2D> Components;

        /// <summary>
        /// The color that should be used by the simulator to clear the screen.
        /// </summary>
        public static Color ClearColor = Color.CornflowerBlue;

        public static float CarGenerationRate = 2f;
        public static float PathFollowingStimulus = 10000f;
        public static float SeparationStimulus = 10000f;
        public static float LeaderFollowingStimulus = 0f;
        public static int CarsFinished = 0;
        public static int Crashes = 0;

        /// <summary>
        /// Initialize the simulator using the specified graphics device and content manager.
        /// </summary>
        /// <param name="graphicsDeviceService"></param>
        /// <param name="content"></param>
        public static void InitializeSimulator(IGraphicsDeviceService graphicsDeviceService, ContentManager content)
        {
            Simulator.GraphicsDevice = graphicsDeviceService.GraphicsDevice;
            Simulator.SpriteBatch = new SpriteBatch(Simulator.GraphicsDevice);
            Simulator.Content = content;
            Simulator.Components = new List<Component2D>();

            Simulator.isInitialized = true;
        }

        /// <summary>
        /// Updates the simulator and all of its components.
        /// </summary>
        /// <param name="simTime"></param>
        public static void Update(GameTime simTime)
        {
            Simulator.SimTime = simTime;
            Simulator.TotalMinutesSinceLastReset += (float)simTime.ElapsedGameTime.TotalMinutes;

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

        /// <summary>
        /// Draws the simulator and all of its components.
        /// </summary>
        /// <param name="simTime"></param>
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
