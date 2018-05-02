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
    public class Battleship : Tank
    {
        private DroneCannon leftTopCannon;
        private DroneCannon leftDownCannon;
        private DroneCannon rightTopCannon;
        private DroneCannon rightDownCannon;

        public Battleship(GameScreen Screen, TeamColor TeamColor, float Weight) : base(Screen, TeamColor, Weight)
        {
            // Simplest Constructor
        }

        public override void SetCannons()
        {
            // Creating Cannons
            leftTopCannon = new DroneCannon();
            leftDownCannon = new DroneCannon();
            rightTopCannon = new DroneCannon();
            rightDownCannon = new DroneCannon();
            // Setting Cannons
            Cannons = new Cannon[]
            {
                leftTopCannon,
                leftDownCannon,
                rightTopCannon,
                rightDownCannon
            };
            RepositionCannons(Bounds);
        }

        public override void RepositionCannons(RectangleF value)
        {
            // Make sure the SetCannons method has been already called
            if (Cannons != null)
            {
                var tankPosition = new SizeF(.5f, .5f);
                var cannonPercentage = .5f;
                var cannonHeight = bounds.Height * cannonPercentage;
                var cannonBounds = new RectangleF(bounds.X, bounds.Y, bounds.Width * .8f, cannonHeight);
                //
                leftTopCannon.TankPosition = new SizeF(.5f, .3f);
                leftDownCannon.TankPosition = new SizeF(.5f, .7f);
                leftTopCannon.Angle = (Single)Math.PI / 2;
                leftDownCannon.Angle = (Single)Math.PI / 2;
                //
                rightTopCannon.TankPosition = new SizeF(.5f, .3f);
                rightDownCannon.TankPosition = new SizeF(.5f, .7f);
                rightTopCannon.Angle = 3 * (Single)Math.PI / 2;
                rightDownCannon.Angle = 3 * (Single)Math.PI / 2;
                //
                foreach (var cannon in Cannons)
                {
                    cannon.Support = this;
                    cannon.Bounds = cannonBounds;
                    cannon.Obliquity = -.3f;
                }
            }
        }

        protected override void ExecuteShoot()
        {
            // This Tank must not execute any shoot when user clicks or press 'E'
        }
    }
}
