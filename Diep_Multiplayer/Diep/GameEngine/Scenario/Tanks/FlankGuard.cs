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
    public class FlankGuard : Tank
    {
        private Cannon frontalCannon;
        private Cannon backCannon;

        public FlankGuard(GameScreen Screen, TeamColor TeamColor, float Weight) : base(Screen, TeamColor, Weight)
        {
            // Simplest Constructor
        }

        public override void SetCannons()
        {
            // Tests Purpose
            //base.SetCannons();
            // Creating Cannons
            frontalCannon = new Cannon();
            backCannon = new Cannon();
            // Defining Cannons
            var Cannons = new Cannon[]
            {
                frontalCannon,
                backCannon
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
                // Angle of Cannons
                backCannon.Angle = (Single)Math.PI;
                // Size of Cannons
                frontalCannon.TankPosition = new SizeF(.5f, .5f);
                backCannon.TankPosition = new SizeF(.8f, .5f);
                var cannonHeight = bounds.Height * .5f;
                frontalCannon.Bounds = new RectangleF(bounds.X, bounds.Width, bounds.Width, cannonHeight);
                backCannon.Bounds = new RectangleF(bounds.X, bounds.Width, bounds.Width / 2, cannonHeight);
                // Setting Tank Reference
                frontalCannon.Support = this;
                backCannon.Support = this;
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
                backCannon.Shoot();
            }
            else
            {
                ShootStep = 0;
            }
        }
    }
}
