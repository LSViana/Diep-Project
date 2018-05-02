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
    public class Sprayer : Tank
    {
        private Cannon spreadCannon;
        private Cannon smallCannon;

        public Sprayer(GameScreen Screen, TeamColor TeamColor, float Weight) : base(Screen, TeamColor, Weight)
        {
            // Simplest Constructor
        }

        public override void SetCannons()
        {
            // Tests Purpose
            //base.SetCannons();
            // Creating Cannons
            spreadCannon = new Cannon()
            {
                ShootRate = 1
            };
            smallCannon = new Cannon()
            {
                ShootRate = .66f
            };
            spreadCannon.ShootSpread = Cannon.MaxShootSpread;
            // Defining Cannons
            var Cannons = new Cannon[]
            {
                smallCannon,
                spreadCannon,
            };
            //
            this.Cannons = Cannons;
            RepositionCannons(Bounds);
        }

        public override void RepositionCannons(RectangleF bounds)
        {
            if (spreadCannon != null)
            {
                var tankPosition = new SizeF(.5f, .5f);
                var cannonBounds = new RectangleF(new PointF(bounds.Location.X, bounds.Location.Y + (bounds.Height * .1f)), new SizeF(bounds.Width, bounds.Height * .5f));
                //
                foreach (var cannon in Cannons)
                {
                    cannon.Support = this;
                }
                // Small Cannon
                smallCannon.TankPosition = tankPosition;
                smallCannon.Bounds = cannonBounds;
                cannonBounds.Width *= .85f;
                cannonBounds.Height *= 1.6f;
                // Spread Cannon
                spreadCannon.TankPosition = tankPosition;               
                spreadCannon.Bounds = cannonBounds;
                spreadCannon.Obliquity = .4f;
            }
        }
    }
}