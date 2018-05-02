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
    public class Hybrid : Tank
    {
        private Cannon frontalCannon;
        private DroneCannon backDroneCannon;

        public Hybrid(GameScreen Screen, TeamColor TeamColor, float Weight) : base(Screen, TeamColor, Weight)
        {
            // Simplest Constructor
        }

        public override void SetCannons()
        {
            // Tests Purpose
            //base.SetCannons();
            // Creating Cannons
            frontalCannon = new Cannon();
            backDroneCannon = new DroneCannon()
            {
                Controllable = false,
                DroneLimit = 2
            };
            // Defining Cannons
            var Cannons = new Cannon[]
            {
                frontalCannon,
                backDroneCannon,
            };
            //
            this.Cannons = Cannons;
            RepositionCannons(Bounds);
        }

        public override void RepositionCannons(RectangleF bounds)
        {
            if (frontalCannon != null)
            {
                var cannonPercentage = .75f;
                var cannonHeight = bounds.Height * cannonPercentage;
                var tankPosition = new SizeF(.5f, .5f);
                var cannonBounds = new RectangleF(bounds.Location.X, bounds.Location.Y + (bounds.Height * .1f), bounds.Width, cannonHeight);
                // Common Information
                foreach (var cannon in Cannons)
                {
                    cannon.Support = this;
                }
                // Frontal Cannon
                frontalCannon.TankPosition = tankPosition;
                frontalCannon.Bounds = cannonBounds;
                // Back Cannon
                cannonBounds.Width *= .7f;
                cannonPercentage = .8f;
                cannonHeight = bounds.Height * cannonPercentage;
                cannonBounds.Height = cannonHeight;
                backDroneCannon.Obliquity = .5f;
                backDroneCannon.Bounds = cannonBounds;
                backDroneCannon.TankPosition = tankPosition;
                backDroneCannon.Angle = (Single)Math.PI;
}
        }
    }
}
