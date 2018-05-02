using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Diep.GameEngine.Shared;
using Diep.GameEngine.Scenario.Cannons;

namespace Diep.GameEngine.Scenario.Tanks
{
    public class Rocketeer : Tank
    {
        private RocketeerCannon rocketeerCannon;
        private Cannon stereoCannon;

        public Rocketeer(GameScreen Screen, TeamColor TeamColor, float Weight) : base(Screen, TeamColor, Weight)
        {
            // Simplest Constructor
        }

        public override void SetCannons()
        {
            rocketeerCannon = new RocketeerCannon();
            stereoCannon = new Cannon()
            {
                Stereo = true,
                ShootRate = -Int32.MaxValue,
            };
            //
            Cannons = new Cannon[]
            {
                rocketeerCannon,
                stereoCannon
            };
            RepositionCannons(Bounds);
        }

        public override void RepositionCannons(RectangleF value)
        {
            if (Cannons != null)
            {
                var tankPosition = new SizeF(.5f, .5f);
                var cannonHeight = bounds.Height * .6f;
                var cannonBounds = new RectangleF(bounds.X, bounds.Y, bounds.Width, cannonHeight);
                //
                foreach (var cannon in Cannons)
                {
                    cannon.TankPosition = tankPosition;
                    cannon.Support = this;
                }
                //
                rocketeerCannon.Obliquity = .3f;
                rocketeerCannon.Bounds = cannonBounds;
                cannonHeight = bounds.Height * .8f;
                cannonBounds.Height = cannonHeight;
                cannonBounds.Width *= .85f;
                //
                stereoCannon.Obliquity = -.3f;
                stereoCannon.Bounds = cannonBounds;
            }
        }
    }
}
