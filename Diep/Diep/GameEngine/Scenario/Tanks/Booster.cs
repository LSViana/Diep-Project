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
    public class Booster : Tank
    {
        private Cannon frontalCannon;
        private Cannon backLeftCannon;
        private Cannon smallBackLeftCannon;
        private Cannon backRightCannon;
        private Cannon smallBackRightCannon;

        public Booster(GameScreen Screen, TeamColor TeamColor, float Weight) : base(Screen, TeamColor, Weight)
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
            smallBackLeftCannon = new Cannon();
            backRightCannon = new Cannon();
            smallBackRightCannon = new Cannon();
            // Defining Cannons
            var Cannons = new Cannon[]
            {
                smallBackLeftCannon,
                smallBackRightCannon,
                backLeftCannon,
                backRightCannon,
                frontalCannon,
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
                var cannonBounds = new RectangleF(bounds.X, bounds.Y, bounds.Width, cannonHeight);
                // Frontal Cannon
                frontalCannon.Bounds = cannonBounds;
                cannonBounds.Width *= .8f;
                //
                // Back Lateral Cannons
                backRightCannon.Bounds = cannonBounds;
                backRightCannon.Angle = cannonAngle;
                backLeftCannon.Bounds = cannonBounds;
                backLeftCannon.Angle = -cannonAngle;
                cannonBounds.Width *= .8f;
                cannonAngle = 2 * (Single)Math.PI / 3;
                // Small Back Lateral Cannons
                smallBackRightCannon.Bounds = cannonBounds;
                smallBackRightCannon.Angle = cannonAngle;
                smallBackLeftCannon.Bounds = cannonBounds;
                smallBackLeftCannon.Angle = -cannonAngle;
                //
                foreach (var cannon in Cannons)
                {
                    cannon.Support = this;
                    cannon.TankPosition = tankPosition;
                }
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
                ShootStep -= 1;
            }
            else if (ShootStep == 2)
            {
                smallBackLeftCannon.Shoot();
                smallBackRightCannon.Shoot();
                ShootStep -= 1;
            }
            else
            {
                ShootStep = 0;
            }
        }
    }
}