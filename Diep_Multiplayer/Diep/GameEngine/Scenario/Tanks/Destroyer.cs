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
    public class Destroyer : Tank
    {
        private Cannon cannon;
        private float cannonPercentage = .75f;

        public Destroyer(GameScreen Screen, TeamColor TeamColor, float Weight) : base(Screen, TeamColor, Weight)
        {
            // Simplest Constructor
        }

        public override void SetCannons()
        {
            // Tests Purpose
            //base.SetCannons();
            // Creating Cannons
            cannon = new Cannon();
            // Defining Cannons
            var Cannons = new Cannon[]
            {
                cannon
            };
            //
            this.Cannons = Cannons;
            RepositionCannons(Bounds);
        }

        public override void RepositionCannons(RectangleF bounds)
        {
            if (cannon != null)
            {
                var tankPosition = new SizeF(.5f, .5f);
                cannon.TankPosition = tankPosition;
                cannon.Bounds = new RectangleF(new PointF(bounds.Location.X, bounds.Location.Y + (bounds.Height * .1f)), new SizeF(bounds.Width, bounds.Height * cannonPercentage));
                cannon.Support = this;
            }
        }
    }
}
