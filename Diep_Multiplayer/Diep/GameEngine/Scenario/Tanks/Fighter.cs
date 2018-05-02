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
    public class Fighter : Tank
    {
        private Cannon frontalCannon;
        private Cannon leftCannon;
        private Cannon backLeftCannon;
        private Cannon backRightCannon;
        private Cannon rightCannon;

        public Fighter(GameScreen Screen, TeamColor TeamColor, float Weight) : base(Screen, TeamColor, Weight)
        {
            // Simplest Constructor
        }

        public override void SetCannons()
        {
            // Tests Purpose
            //base.SetCannons();
            // Creating Cannons
            frontalCannon = new Cannon();
            leftCannon = new Cannon();
            backLeftCannon = new Cannon();
            rightCannon = new Cannon();
            backRightCannon = new Cannon();
            // Defining Cannons
            var Cannons = new Cannon[]
            {
                leftCannon,
                backLeftCannon,
                backRightCannon,
                rightCannon,
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
                var cannonAngle = (Single)Math.PI / 2;
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
                // Frontal Cannon
                frontalCannon.Bounds = cannonBounds;
                cannonBounds.Width *= .8f;
                //
                // Back Lateral Cannons
                rightCannon.Bounds = cannonBounds;
                rightCannon.Angle = cannonAngle;
                leftCannon.Bounds = cannonBounds;
                leftCannon.Angle = -cannonAngle;
                cannonAngle *= 5f / 3;
                // Small Back Lateral Cannons
                backRightCannon.Bounds = cannonBounds;
                backRightCannon.Angle = cannonAngle;
                backLeftCannon.Bounds = cannonBounds;
                backLeftCannon.Angle = -cannonAngle;
            }

        }
        protected override void ExecuteShoot()
        {
            if(ShootStep == 0)
            {
                frontalCannon.Shoot();
                leftCannon.Shoot();
                rightCannon.Shoot();
                ShootStep -= 2;
            }
            else if (ShootStep == 1)
            {
                backLeftCannon.Shoot();
                backRightCannon.Shoot();
                ShootStep -= 1;
            }
            else
            {
                ShootStep = 0;
            }
        }
    }
}
