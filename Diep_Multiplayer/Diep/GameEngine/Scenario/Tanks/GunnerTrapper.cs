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
    public class GunnerTrapper : Tank
    {
        private Cannon trapperCannon;
        private Cannon leftCannon;
        private Cannon rightCannon;

        public GunnerTrapper(GameScreen Screen, TeamColor TeamColor, float Weight) : base(Screen, TeamColor, Weight)
        {
            // Simplest Constructor
        }

        public override void SetCannons()
        {
            // Tests Purpose
            //base.SetCannons();
            // Creating Cannons
            trapperCannon = new TrapperCannon();
            leftCannon = new Cannon();
            rightCannon = new Cannon();
            // Defining Cannons
            var Cannons = new Cannon[]
            {
                trapperCannon,
                leftCannon,
                rightCannon
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
                var cannonHeight = bounds.Height * .5f;
                //
                // Common Information
                foreach (var cannon in Cannons)
                {
                    cannon.Support = this;
                }
                var cannonBounds = new RectangleF(bounds.X, bounds.Y, bounds.Width * .8f, cannonHeight);
                // Trapper Cannon
                trapperCannon.Bounds = cannonBounds;
                trapperCannon.Angle = (Single)Math.PI;
                trapperCannon.TankPosition = new SizeF(.5f, .5f);
                // Frontal Cannons
                const float frontalTankHeight = .3f;
                cannonHeight = bounds.Height * frontalTankHeight;
                cannonBounds.Height = cannonHeight;
                cannonBounds.Width *= .9f;
                leftCannon.TankPosition = new SizeF(.5f, frontalTankHeight);
                rightCannon.TankPosition = new SizeF(.5f, 1 - frontalTankHeight);
                leftCannon.Bounds = cannonBounds;
                rightCannon.Bounds = cannonBounds;
            }
        }

        protected override void ExecuteShoot()
        {
            if (ShootStep == 0)
            {
                trapperCannon.Shoot();
                ShootStep--;
                leftCannon.Shoot();
            }
            else if (ShootStep == 1)
            {
                trapperCannon.Shoot();
                ShootStep--;
                rightCannon.Shoot();
            }
            else
            {
                ShootStep = 0;
            }
        }
    }
}
