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
    public class Auto3 : Tank
    {
        protected const Single SpinningMultiplier = .00000002f;
        private Cannon frontalCannon;
        private Cannon backLeftCannon;
        private Cannon backRightCannon;

        public Auto3(GameScreen Screen, TeamColor TeamColor, float Weight) : base(Screen, TeamColor, Weight)
        {
            // Simplest Constructor
        }

        public override void SetCannons()
        {
            // Tests Purpose
            //base.SetCannons();
            // Creating Cannons
            frontalCannon = new AutoCannon();
            backLeftCannon = new AutoCannon();
            backRightCannon = new AutoCannon();
            // Defining Cannons
            var Cannons = new Cannon[]
            {
                frontalCannon,
                backLeftCannon,
                backRightCannon,
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
                const float angleInterval = 2 * (Single)Math.PI / 3;
                var cannonAngle = 0f;
                var tankPosition = new SizeF(.5f, .5f);
                var cannonHeight = bounds.Height * .4f;
                var cannonBounds = new RectangleF(bounds.X, bounds.Y, bounds.Width, cannonHeight);
                //
                foreach (var cannon in Cannons)
                {
                    cannon.Support = this;
                    cannon.TankPosition = tankPosition;
                    cannon.Angle = cannonAngle;
                    cannon.Bounds = cannonBounds;
                    //
                    cannonAngle += angleInterval;
                }
            }
        }

        protected override Single GetAngle(Point mouseLocation)
        {
            return Angle + (Single)Math.PI * SpinningMultiplier * Screen.Elapsed.Ticks;
        }
    }
}
