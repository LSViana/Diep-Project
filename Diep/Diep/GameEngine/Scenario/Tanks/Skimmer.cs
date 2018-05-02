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
    public class Skimmer : Tank
    {
        private Cannon stereoCannon;
        private DeployerCannon deployerCannon;

        public Skimmer(GameScreen Screen, TeamColor TeamColor, float Weight) : base(Screen, TeamColor, Weight)
        {
            // Simplest Constructor
        }

        public override void SetCannons()
        {
            // Tests Purpose
            //base.SetCannons();
            // Creating Cannons
            stereoCannon = new Cannon()
            {
                Stereo = true,
                Obliquity = .3f
            };
            deployerCannon = new DeployerCannon();
            // Defining Cannons
            var Cannons = new Cannon[]
            {
                stereoCannon,
                deployerCannon
            };
            //
            this.Cannons = Cannons;
            RepositionCannons(Bounds);
        }

        public override void RepositionCannons(RectangleF bounds)
        {
            if(Cannons != null)
            {
                var tankPosition = new SizeF(.5f, .5f);
                var cannonHeight = bounds.Height * .6f;
                var cannonBounds = new RectangleF(bounds.Location.X, bounds.Location.Y + (bounds.Height * .1f), bounds.Width, cannonHeight);
                //
                foreach (var cannon in Cannons)
                {
                    cannon.TankPosition = tankPosition;
                    cannon.Support = this;
                }
                //
                stereoCannon.Bounds = cannonBounds;
                cannonBounds.Width *= .9f;
                cannonHeight = bounds.Height * .8f;
                cannonBounds.Height = cannonHeight;
                //
                deployerCannon.Bounds = cannonBounds;
            }
        }
    }
}
