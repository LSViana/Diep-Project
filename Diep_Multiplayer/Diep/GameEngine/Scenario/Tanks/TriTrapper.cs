using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Diep.GameEngine.Scenario.Cannons;
using Diep.GameEngine.Shared;

namespace Diep.GameEngine.Scenario.Tanks
{
    public class TriTrapper : Tank
    {
        private Cannon zeroCannon;
        private Cannon oneCannon;
        private Cannon twoCannon;

        public TriTrapper(GameScreen Screen, TeamColor TeamColor, float Weight) : base(Screen, TeamColor, Weight)
        {
            // Simplest Constructor
        }

        public override void SetCannons()
        {
            // Tests Purpose
            //base.SetCannons();
            // Creating Cannons
            zeroCannon = new TrapperCannon();
            oneCannon = new TrapperCannon();
            twoCannon = new TrapperCannon();
            // Defining Cannons
            var Cannons = new Cannon[]
            {
                zeroCannon,
                oneCannon,
                twoCannon
            };
            //
            this.Cannons = Cannons;
            RepositionCannons(Bounds);
        }

        public override void RepositionCannons(RectangleF value)
        {
            // Make sure the SetCannons method has been already called
            if (Cannons != null)
            {
                var angleInterval = 2 * (Single)Math.PI / 3;
                var cannonAngle = 0f;
                var cannonHeight = bounds.Height * .5f;
                //
                var cannonBounds = new RectangleF(bounds.X, bounds.Y, bounds.Width * .8f, cannonHeight);
                //
                foreach (var cannon in Cannons)
                {
                    cannon.Support = this;
                    cannon.Bounds = cannonBounds;
                    cannon.TankPosition = new SizeF(.5f, .5f);
                    //
                    cannon.Angle = cannonAngle;
                    cannonAngle += angleInterval;
                }
            }
        }
    }
}