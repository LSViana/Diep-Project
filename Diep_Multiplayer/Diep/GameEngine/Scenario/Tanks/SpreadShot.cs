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
    public class SpreadShot : Tank
    {
        private Cannon leftZeroCannon;
        private Cannon leftOneCannon;
        private Cannon leftTwoCannon;
        private Cannon leftThreeCannon;
        private Cannon leftFourCannon;
        private Cannon rightZeroCannon;
        private Cannon rightOneCannon;
        private Cannon rightTwoCannon;
        private Cannon rightThreeCannon;
        private Cannon rightFourCannon;
        private Cannon centralCannon;
        private Cannon[] lateralCannons;

        public SpreadShot(GameScreen Screen, TeamColor TeamColor, float Weight) : base(Screen, TeamColor, Weight)
        {
            // Simplest Constructor
        }

        public override void SetCannons()
        {
            // Tests Purpose
            //base.SetCannons();
            // Creating Cannons
            leftZeroCannon = new Cannon();
            leftOneCannon = new Cannon();
            leftTwoCannon = new Cannon();
            leftThreeCannon = new Cannon();
            leftFourCannon = new Cannon();
            rightZeroCannon = new Cannon();
            rightOneCannon = new Cannon();
            rightTwoCannon = new Cannon();
            rightThreeCannon = new Cannon();
            rightFourCannon = new Cannon();
            centralCannon = new Cannon();
            // Defining Cannons
            var Cannons = new Cannon[]
            {

                leftZeroCannon,
                leftOneCannon,
                leftTwoCannon,
                leftThreeCannon,
                leftFourCannon,
                rightFourCannon,
                rightThreeCannon,
                rightTwoCannon,
                rightOneCannon,
                rightZeroCannon,
                centralCannon
            };
            lateralCannons = new Cannon[]
            {
                leftZeroCannon,
                leftOneCannon,
                leftTwoCannon,
                leftThreeCannon,
                leftFourCannon,
                rightZeroCannon,
                rightOneCannon,
                rightTwoCannon,
                rightThreeCannon,
                rightFourCannon,
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
                var cannonAngle = 4 * -(Single)Math.PI / 9;
                var angleInterval = 4 * (Single)Math.PI / 45;
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
                cannonBounds.Height *= .8f;
                var originalWidth = cannonBounds.Width;
                //
                for (int i = 0; i < lateralCannons.Length; i++)
                {
                    if (i == lateralCannons.Length / 2)
                    {
                        cannonAngle += angleInterval;
                    }
                    //
                    var cannon = lateralCannons[i];
                    var angle = cannonAngle;
                    cannonAngle += angleInterval;
                    //
                    cannonBounds.Width = originalWidth * (1 - Math.Abs((lateralCannons.Length - 1) / 2f - i) * .07f);
                    cannon.Bounds = cannonBounds;
                    cannon.Angle = angle;
                }
            }
        }

        protected override void ExecuteShoot()
        {
            if(ShootStep == 0)
            {
                centralCannon.Shoot();
            }
            else if (ShootStep == 5)
            {
                leftZeroCannon.Shoot();
                rightFourCannon.Shoot();
                ShootStep--;
            }
            else if (ShootStep == 4)
            {
                leftOneCannon.Shoot();
                rightThreeCannon.Shoot();
                ShootStep--;
            }
            else if (ShootStep == 3)
            {
                leftTwoCannon.Shoot();
                rightTwoCannon.Shoot();
                ShootStep--;
            }
            else if (ShootStep == 2)
            {
                leftThreeCannon.Shoot();
                rightOneCannon.Shoot();
                ShootStep--;
            }
            else if (ShootStep == 1)
            {
                leftFourCannon.Shoot();
                rightZeroCannon.Shoot();
                ShootStep--;
            }
            else
            {
                ShootStep = 0;
            }
        }
    }
}
