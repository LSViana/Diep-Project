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
    public class TriAngle : Tank
    {
        private Cannon frontalCannon;
        private Cannon backLeftCannon;
        private Cannon backRightCannon;

        public TriAngle(GameScreen Screen, TeamColor TeamColor, float Weight) : base(Screen, TeamColor, Weight)
        {
            // Simplest Constructor
        }

        public override void SetCannons()
        {
            // Tests Purpose
            //base.SetCannons();
            // Creating Cannons
            frontalCannon = new Cannon();
            backLeftCannon = new Cannon();
            backRightCannon = new Cannon();
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
                var cannonAngle = 3 * (Single)Math.PI / 4;
                var tankPosition = new SizeF(.5f, .5f);
                var cannonHeight = bounds.Height * .5f;
                //
                foreach (var cannon in Cannons)
                {
                    cannon.Support = this;
                    cannon.TankPosition = tankPosition;
                }
                //
                var cannonBounds = new RectangleF(bounds.X, bounds.Y, bounds.Width, cannonHeight);
                frontalCannon.Bounds = cannonBounds;
                cannonBounds.Width *= .8f;
                backLeftCannon.Bounds = cannonBounds;
                backLeftCannon.Angle = cannonAngle;
                //
                cannonAngle += (Single)Math.PI / 2;
                //
                backRightCannon.Bounds = cannonBounds;
                backRightCannon.Angle = cannonAngle;
            }
        }

        protected override void ExecuteShoot()
        {
            if(ShootStep == 0)
            {
                frontalCannon.Shoot();
            }
            else if (ShootStep == 1)
            {
                backLeftCannon.Shoot();
                backRightCannon.Shoot();
                ShootStep--;
            }
            else
            {
                ShootStep = 0;
            }
        }
    }
}
