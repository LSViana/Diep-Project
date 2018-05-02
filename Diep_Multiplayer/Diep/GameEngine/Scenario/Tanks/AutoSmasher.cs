using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Diep.GameEngine.Scenario.Cannons;
using Diep.GameEngine.Scenario.Shields;
using Diep.GameEngine.Shared;

namespace Diep.GameEngine.Scenario.Tanks
{
    public class AutoSmasher : Tank
    {
        protected const Single SpinningMultiplier = .0000002f;
        private AutoCannon autoCannon;
        public Cannon[] OverCannons { get; private set; }

        public AutoSmasher(GameScreen Screen, TeamColor TeamColor, float Weight) : base(Screen, TeamColor, Weight)
        {
            // Simplest Constructor
            Screen.ZoomFactor = .95f;
        }

        public override float Angle
        {
            get => base.Angle;
            set { var difference = base.Angle - value; autoCannon.BarrelAngle += difference; base.Angle = value; }
        }

        public override void SetCannons()
        {
            // Defining Cannons
            autoCannon = new AutoCannon()
            {
                Controllable = false,
                RangeLimit = -1
            };
            var Cannons = new Cannon[]
            {
                autoCannon
            };
            var OverCannons = new Cannon[]
            {
                autoCannon
            };
            // Defining Shields
            var shield = new Shield()
            {
                Support = this,
                TeamColor = TeamColor.DimGray,
                Angle = 2 * (Single)Math.PI / 12,
                SpinningMultiplier = SpinningMultiplier
            };
            Shields = new Shield[] { shield };
            Power *= 3;
            // Defining Standard Property Values
            this.Cannons = Cannons;
            this.OverCannons = OverCannons;
            RepositionCannons(Bounds);
        }

        public override void RepositionCannons(RectangleF bounds)
        {
            if (Cannons != null)
            {
                var cannonHeight = bounds.Height * .35f;
                var cannonBounds = new RectangleF(bounds.X, bounds.Y, bounds.Width * .8f, cannonHeight);
                var tankPosition = new SizeF(.225f, .5f);
                //
                foreach (var cannon in Cannons)
                {
                    cannon.Support = this;
                    cannon.TankPosition = tankPosition;
                    cannon.Bounds = cannonBounds;
                }
            }
            if (Shields != null)
            {
                foreach (var shield in Shields)
                {
                    shield.Bounds = bounds;
                }
            }
        }

        protected override void DrawUnderToolsUnrotated(Graphics g)
        {
            if (Shields != null)
                foreach (var shield in Shields)
                {
                    shield.Draw(g);
                }
        }

        protected override void DrawOverTools(Graphics g)
        {
            if(OverCannons != null)
                foreach (var cannon in OverCannons)
                {
                    cannon.Draw(g);
                }
        }

        protected override Single GetAngle(Point mouseLocation)
        {
            return Angle + (Single)Math.PI * SpinningMultiplier * Screen.Elapsed.Ticks;
        }
    }
}
