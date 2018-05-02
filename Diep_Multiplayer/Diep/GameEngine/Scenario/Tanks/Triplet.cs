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
    public class Triplet : Tank
    {
        private Cannon frontalCannon;
        private Cannon frontalLeftCannon;
        private Cannon frontalRightCannon;

        public Triplet(GameScreen Screen, TeamColor TeamColor, float Weight) : base(Screen, TeamColor, Weight)
        {
            // Simplest Constructor
        }

        public override void SetCannons()
        {
            // Tests Purpose
            //base.SetCannons();
            // Creating Cannons
            frontalCannon = new Cannon();
            frontalLeftCannon = new Cannon();
            frontalRightCannon = new Cannon();
            // Defining Cannons
            var Cannons = new Cannon[]
            {
                frontalLeftCannon,
                frontalRightCannon,
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
                var cannonPercentage = .5f;
                var cannonHeight = bounds.Height * cannonPercentage;
                //
                foreach (var cannon in Cannons)
                {
                    cannon.Support = this;
                }
                //
                var cannonBounds = new RectangleF(bounds.X, bounds.Y, bounds.Width, cannonHeight);
                //
                frontalCannon.Bounds = cannonBounds;
                frontalCannon.TankPosition = new SizeF(.5f, .5f);
                //
                cannonBounds.Width *= .8f;
                //
                frontalLeftCannon.Bounds = cannonBounds;
                frontalRightCannon.Bounds = cannonBounds;
                frontalLeftCannon.TankPosition = new SizeF(.5f, cannonPercentage / 2);
                frontalRightCannon.TankPosition = new SizeF(.5f, 1 - cannonPercentage / 2);
            }
        }

        protected override void ExecuteShoot()
        {
            if(ShootStep == 0)
            {
                frontalCannon.Shoot();
            }
            else if(ShootStep == 1)
            {
                frontalLeftCannon.Shoot();
                frontalRightCannon.Shoot();
                ShootStep--;
            }
            else
            {
                ShootStep = 0;
            }
        }
    }
}
