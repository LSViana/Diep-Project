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
    public class AutoTrapper : Tank
    {
        private TrapperCannon trapperCannon;
        private AutoCannon autoCannon;

        public Cannon[] OverCannons { get; private set; }

        public AutoTrapper(GameScreen Screen, TeamColor TeamColor, float Weight) : base(Screen, TeamColor, Weight)
        {
            // Simplest Constructor
        }

        public override float Angle
        {
            get => base.Angle;
            set { var difference = base.Angle - value; autoCannon.BarrelAngle += difference; base.Angle = value; }
        }

        public override void SetCannons()
        {
            // Tests Purpose
            //base.SetCannons();
            // Creating Cannons
            trapperCannon = new TrapperCannon()
            {
                FunnelRange = .3f
            };
            autoCannon = new AutoCannon()
            {
                RangeLimit = -1f,
                Controllable = false,
            };
            // Defining Cannons
            var Cannons = new Cannon[]
            {
                trapperCannon,
                autoCannon,
            };
            var OverCannons = new Cannon[]
            {
                autoCannon
            };
            //
            this.OverCannons = OverCannons;
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
                autoCannon.TankPosition = new SizeF(.26f, .5f); // .26f instead .25f because it seems there is a little deviation at cannon ellipse position
                RectangleF cannonBounds = new RectangleF(bounds.X, bounds.Y, bounds.Width * .8f, cannonHeight);
                // Defining Similar Properties
                foreach (var cannon in Cannons)
                {
                    cannon.Support = this;
                }
                //
                autoCannon.Bounds = cannonBounds;
                //
                cannonPercentage = .5f;
                cannonHeight = bounds.Height * cannonPercentage;
                cannonBounds.Height = cannonHeight;
                trapperCannon.Bounds = cannonBounds;
                trapperCannon.TankPosition = new SizeF(.5f, .5f);
            }
        }

        protected override void DrawOverTools(Graphics g)
        {
            if (OverCannons != null)
            {
                foreach (var cannon in OverCannons)
                {
                    cannon.Draw(g);
                }
            }
        }
    }
}
