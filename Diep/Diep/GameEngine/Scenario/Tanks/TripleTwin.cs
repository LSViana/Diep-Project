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
    public class TripleTwin : Tank
    {
        private Cannon zeroTop;
        private Cannon zeroDown;
        private Cannon oneTop;
        private Cannon oneDown;
        private Cannon twoTop;
        private Cannon twoDown;

        public TripleTwin(GameScreen Screen, TeamColor TeamColor, float Weight) : base(Screen, TeamColor, Weight)
        {
            // Simplest Constructor
        }

        public override void SetCannons()
        {
            // Tests Purpose
            //base.SetCannons();
            // Creating Cannons
            zeroTop = new Cannon();
            zeroDown = new Cannon();
            oneTop = new Cannon();
            oneDown = new Cannon();
            twoTop = new Cannon();
            twoDown = new Cannon();
            // Defining Cannons
            var Cannons = new Cannon[]
            {
                zeroTop,
                zeroDown,
                oneTop,
                oneDown,
                twoTop,
                twoDown,
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
                var cannonAngle = 0f;
                var angleInterval = 2 * (Single)Math.PI / 3;
                var cannonPercentage = .48f;
                var cannonHeight = bounds.Height * cannonPercentage;
                // Defining Specific Properties
                SizeF topPosition = new SizeF(.5f, cannonPercentage / 2), bottomPosition = new SizeF(.5f, 1 - cannonPercentage / 2);
                zeroTop.TankPosition = topPosition;
                oneTop.TankPosition = topPosition;
                twoTop.TankPosition = topPosition;
                zeroDown.TankPosition = bottomPosition;
                oneDown.TankPosition = bottomPosition;
                twoDown.TankPosition = bottomPosition;
                //
                cannonAngle += angleInterval;
                //
                oneTop.Angle = cannonAngle;
                oneDown.Angle = cannonAngle;
                //
                cannonAngle += angleInterval;
                //
                twoTop.Angle = cannonAngle;
                twoDown.Angle = cannonAngle;
                // Defining Similar Properties
                foreach (var cannon in Cannons)
                {
                    cannon.Support = this;
                    cannon.Bounds = new RectangleF(bounds.X, bounds.Y, bounds.Width, cannonHeight);
                }
            }
        }

        protected override void ExecuteShoot()
        {
            if(ShootStep == 0)
            {
                zeroTop.Shoot();
                oneTop.Shoot();
                twoTop.Shoot();
                //
                ShootStep -= 2;
            }
            else if (ShootStep == 1)
            {
                zeroDown.Shoot();
                oneDown.Shoot();
                twoDown.Shoot();
                ShootStep -= 2;
            }
            else
            {
                ShootStep = 0;
            }
        }
    }
}
