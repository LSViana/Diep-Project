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
    public class Smasher : Tank
    {
        protected const Single SpinningMultiplier = .0000001f;

        public Smasher(GameScreen Screen, TeamColor TeamColor, float Weight) : base(Screen, TeamColor, Weight)
        {
            // Simplest Constructor
            Screen.ZoomFactor = .95f;
        }

        public override void SetCannons()
        {
            // This Tank must not have Cannons
            Cannons = new Cannon[] { };
            // Defining Shields
            var shield = new Shield()
            {
                Support = this,
                TeamColor = TeamColor.DimGray,
            };
            Shields = new Shield[] { shield };
            // Defining Standard Property Values
            Power *= 3;
            RepositionCannons(Bounds);
        }

        public override void RepositionCannons(RectangleF bounds)
        {
            // This Tank must not have Cannons
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

        protected override Single GetAngle(Point mouseLocation)
        {
            return Angle + (Single)Math.PI * SpinningMultiplier * Screen.Elapsed.Ticks;
        }
    }
}
