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
    public class Auto5 : Tank
    {
        protected const Single SpinningMultiplier = .0000001f;
        private Cannon zeroCannon;
        private Cannon oneCannon;
        private Cannon twoCannon;
        private AutoCannon threeCannon;
        private AutoCannon fourCannon;

        public Auto5(GameScreen Screen, TeamColor TeamColor, float Weight) : base(Screen, TeamColor, Weight)
        {
            // Simplest Constructor
        }

        public override void SetCannons()
        {
            // Tests Purpose
            //base.SetCannons();
            // Creating Cannons
            zeroCannon = new AutoCannon();
            oneCannon = new AutoCannon();
            twoCannon = new AutoCannon();
            threeCannon = new AutoCannon();
            fourCannon = new AutoCannon();

            // Defining Cannons
            var Cannons = new Cannon[]
            {
                zeroCannon,
                oneCannon,
                twoCannon,
                threeCannon,
                fourCannon
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
                const float angleInterval = 2 * (Single)Math.PI / 5;
                var cannonAngle = 0f;
                var tankPosition = new SizeF(.5f, .5f);
                var cannonHeight = bounds.Height * .4f;
                var cannonBounds = new RectangleF(bounds.X, bounds.Y, bounds.Width, cannonHeight);
                //
                foreach (var cannon in Cannons)
                {
                    cannon.Support = this;
                    cannon.TankPosition = tankPosition;
                    cannon.Bounds = cannonBounds;
                    cannon.Angle = cannonAngle;
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
