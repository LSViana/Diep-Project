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
    public class TwinFlank : Tank
    {
        private Cannon leftTopCannon;
        private Cannon leftBottomCannon;
        private Cannon rightTopCannon;
        private Cannon rightBottomCannon;

        public TwinFlank(GameScreen Screen, TeamColor TeamColor, float Weight) : base(Screen, TeamColor, Weight)
        {
            // Simplest Constructor
        }

        public override void SetCannons()
        {
            // Tests Purpose
            //base.SetCannons();
            // Creating Cannons
            leftTopCannon = new Cannon();
            leftBottomCannon = new Cannon();
            rightTopCannon = new Cannon();
            rightBottomCannon = new Cannon();
            // Defining Cannons
            var Cannons = new Cannon[]
            {
                leftTopCannon,
                leftBottomCannon,
                rightTopCannon,
                rightBottomCannon
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
                var cannonAngle = (Single)Math.PI;
                var cannonPercentage = .45f;
                var cannonHeight = bounds.Height * cannonPercentage;
                // Defining Specific Properties
                SizeF topPosition = new SizeF(.5f, cannonPercentage / 2), bottomPosition = new SizeF(.5f, 1 - cannonPercentage / 2);
                leftTopCannon.TankPosition = topPosition;
                leftBottomCannon.TankPosition = bottomPosition;
                rightTopCannon.TankPosition = bottomPosition;
                rightBottomCannon.TankPosition = topPosition;
                rightTopCannon.Angle = cannonAngle;
                rightBottomCannon.Angle = cannonAngle;
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
                leftTopCannon.Shoot();
                rightBottomCannon.Shoot();
                ShootStep--;
            }
            else if(ShootStep == 1)
            {
                leftBottomCannon.Shoot();
                rightTopCannon.Shoot();
                ShootStep--;
            }
            else
            {
                ShootStep = 0;
            }
        }
    }
}
