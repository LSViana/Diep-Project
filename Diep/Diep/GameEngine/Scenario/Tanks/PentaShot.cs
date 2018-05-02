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
    public class PentaShot : Tank
    {
        private Cannon leftCannon;
        private Cannon centralCannon;
        private Cannon rightCannon;
        private Cannon leftMostCannon;
        private Cannon rightMostCannon;

        public PentaShot(GameScreen Screen, TeamColor TeamColor, float Weight) : base(Screen, TeamColor, Weight)
        {
            // Simplest Constructor
        }

        public override void SetCannons()
        {
            // Tests Purpose
            //base.SetCannons();
            // Creating Cannons
            leftCannon = new Cannon();
            centralCannon = new Cannon();
            rightCannon = new Cannon();
            leftMostCannon = new Cannon();
            rightMostCannon = new Cannon();
            // Defining Cannons
            var Cannons = new Cannon[]
            {
                leftMostCannon,
                rightMostCannon,
                leftCannon,
                rightCannon,
                centralCannon,
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
                var cannonAngle = -(Single)Math.PI / 8;
                var tankPosition = new SizeF(.5f, .5f);
                var cannonHeight = bounds.Height * .5f;
                var cannonBounds = new RectangleF(bounds.X, bounds.Y, bounds.Width, cannonHeight);
                //
                foreach (var cannon in Cannons)
                {
                    cannon.Support = this;
                    cannon.TankPosition = tankPosition;
                }
                //
                centralCannon.Bounds = cannonBounds;
                cannonBounds.Width *= .9f;
                //
                leftCannon.Bounds = cannonBounds;
                leftCannon.Angle = -cannonAngle;
                rightCannon.Bounds = cannonBounds;
                rightCannon.Angle = cannonAngle;
                cannonBounds.Width *= .9f;
                cannonAngle -= (Single)Math.PI / 8;
                //
                leftMostCannon.Bounds = cannonBounds;
                leftMostCannon.Angle = -cannonAngle;
                rightMostCannon.Bounds = cannonBounds;
                rightMostCannon.Angle = cannonAngle;
            }
        }

        protected override void ExecuteShoot()
        {
            if(ShootStep == 0)
            {
                centralCannon.Shoot();
            }
            else if (ShootStep == 1)
            {
                leftCannon.Shoot();
                rightCannon.Shoot();
                ShootStep--;
            }
            else if (ShootStep == 2)
            {
                leftMostCannon.Shoot();
                rightMostCannon.Shoot();
                ShootStep--;
            }
            else
            {
                ShootStep = 0;
            }
        }
    }
}
