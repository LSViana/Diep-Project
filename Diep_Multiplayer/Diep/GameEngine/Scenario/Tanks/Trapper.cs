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
    public class Trapper : Tank
    {
        private Cannon cannon;

        public Trapper(GameScreen Screen, TeamColor TeamColor, float Weight) : base(Screen, TeamColor, Weight)
        {
            // Simplest Constructor
        }

        public override void SetCannons()
        {
            // Tests Purpose
            //base.SetCannons();
            // Creating Cannons
            cannon = new TrapperCannon()
            {
                FunnelRange = .3f
            };
            // Defining Cannons
            var Cannons = new Cannon[]
            {
                cannon,
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
                var cannonBounds = new RectangleF(bounds.X, bounds.Y, bounds.Width * .8f, cannonHeight);
                //
                cannon.Support = this;
                cannon.Bounds = cannonBounds;
                cannon.TankPosition = new SizeF(.5f, .5f);
            }
        }
    }
}
