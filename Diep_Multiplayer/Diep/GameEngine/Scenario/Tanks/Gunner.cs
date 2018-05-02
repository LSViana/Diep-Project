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
    public class Gunner : Tank
    {
        private Cannon topMostCannon;
        private Cannon topCannon;
        private Cannon bottomCannon;
        private Cannon bottomMostCannon;

        public Gunner(GameScreen Screen, TeamColor TeamColor, float Weight) : base(Screen, TeamColor, Weight)
        {
            // Simplest Constructor
        }

        public override void SetCannons()
        {
            // Tests Purpose
            //base.SetCannons();
            // Creating Cannons
            topMostCannon = new Cannon();
            topCannon = new Cannon();
            bottomCannon = new Cannon();
            bottomMostCannon = new Cannon();
            // Defining Cannons
            var Cannons = new Cannon[]
            {
                topMostCannon,
                topCannon,
                bottomMostCannon,
                bottomCannon,
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
                var cannonPercentage = .3f;
                var cannonMiddlePercentage = .32f;
                var cannonHeight = bounds.Height * cannonPercentage;
                // Defining Specific Properties
                topMostCannon.TankPosition = new SizeF(.5f, cannonPercentage / 2);
                bottomMostCannon.TankPosition = new SizeF(.5f, 1 - cannonPercentage / 2);
                topCannon.TankPosition = new SizeF(.5f, cannonMiddlePercentage);
                bottomCannon.TankPosition = new SizeF(.5f, 1 - cannonMiddlePercentage);
                RectangleF cannonBounds = new RectangleF(bounds.X, bounds.Y, bounds.Width, cannonHeight);
                // Defining Similar Properties
                foreach (var cannon in Cannons)
                {
                    cannon.Support = this;
                }
                //
                topCannon.Bounds = cannonBounds;
                bottomCannon.Bounds = cannonBounds;
                //
                cannonBounds.Width *= .8f;
                cannonBounds.X *= 1.25f;
                //
                topMostCannon.Bounds = cannonBounds;
                bottomMostCannon.Bounds = cannonBounds;
            }
        }

        protected override void ExecuteShoot()
        {
            if(ShootStep == 0)
            {
                topCannon.Shoot();
            }
            else if (ShootStep == 1)
            {
                bottomCannon.Shoot();
            }
            else if (ShootStep == 2)
            {
                topMostCannon.Shoot();
            }
            else if (ShootStep == 3)
            {
                bottomMostCannon.Shoot();
            }
            else
            {
                ShootStep = 0;
            }
        }
    }
}
