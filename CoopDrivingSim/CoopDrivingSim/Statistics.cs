using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CoopDrivingSim
{
    public class Statistics : Component2D
    {
        private SpriteFont font;
        private string text;
        
        public Statistics()
            : base()
        {
            this.font = Simulator.Content.Load<SpriteFont>("SimFont");
            this.Position = new Vector2(2,2);
        }

        public override void Update()
        {
            this.text =
                "Car generation every: " + Simulator.CarGenerationRate + " s  (1,2)\n" +
                "Path following stimulus: " + Simulator.PathFollowingStimulus + " (Q,W)\n" +
                "Separation stimulus: " + Simulator.SeparationStimulus + " (A,S)\n" +
                "Leader following stimulus: " + Simulator.LeaderFollowingStimulus + " (Z,X)\n" +
                "Throughput: " + (int)(Simulator.CarsFinished / Simulator.SimTime.TotalGameTime.TotalMinutes) + " cars/min\n" +
                "Crashes: " + Simulator.Crashes / 2;
            
            base.Update();
        }

        public override void Draw()
        {
            Simulator.SpriteBatch.DrawString(
                this.font,
                this.text,
                this.Position,
                Color.White);
            
            base.Draw();
        }
    }
}
