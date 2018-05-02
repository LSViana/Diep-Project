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
    public class Overtrapper : Tank
    {
        private TrapperCannon trapperCannon;
        private DroneCannon leftBackDroneCannon;
        private DroneCannon rightBackDroneCannon;

        public Overtrapper(GameScreen Screen, TeamColor TeamColor, float Weight) : base(Screen, TeamColor, Weight)
        {
            // Simplest Constructor
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
            leftBackDroneCannon = new DroneCannon()
            {
                Controllable = false,
                DroneLimit = 1,
            };
            rightBackDroneCannon = new DroneCannon()
            {
                Controllable = false,
                DroneLimit = 1,
            };
            // Defining Cannons
            var Cannons = new Cannon[]
            {
                trapperCannon,
                rightBackDroneCannon,
                leftBackDroneCannon,
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
                var cannonHeight = bounds.Height * .4f;
                //
                var cannonAngle = 0f;
                var angleInterval = 2 * (Single)Math.PI / 3;
                var cannonBounds = new RectangleF(bounds.X, bounds.Y, bounds.Width * .8f, cannonHeight);
                var tankPosition = new SizeF(.5f, .5f);
                foreach (var cannon in Cannons)
                {
                    cannon.Support = this;
                    cannon.TankPosition = tankPosition;
                    cannon.Angle = cannonAngle;
                    //
                    cannonAngle += angleInterval;
                }
                // Trapper Cannon
                trapperCannon.Bounds = cannonBounds;
                trapperCannon.FunnelRange = .5f;
                cannonBounds.Width *= .9f;
                cannonHeight *= 2;
                cannonBounds.Height = cannonHeight;
                //
                leftBackDroneCannon.Bounds = cannonBounds;
                rightBackDroneCannon.Bounds = cannonBounds;
                leftBackDroneCannon.Obliquity = .4f;
                rightBackDroneCannon.Obliquity = .4f;
            }
        }
    }
}
