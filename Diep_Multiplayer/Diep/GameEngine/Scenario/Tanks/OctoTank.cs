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
    public class OctoTank : Tank
    {
        private Cannon oneCannon;
        private Cannon twoCannon;
        private Cannon threeCannon;
        private Cannon fourCannon;
        private Cannon fiveCannon;
        private Cannon sixCannon;
        private Cannon sevenCannon;
        private Cannon eightCannon;

        public OctoTank(GameScreen Screen, TeamColor TeamColor, float Weight) : base(Screen, TeamColor, Weight)
        {
            // Simplest Constructor
        }

        public override void SetCannons()
        {
            // Tests Purpose
            //base.SetCannons();
            // Creating Cannons
            oneCannon = new Cannon();
            twoCannon = new Cannon();
            threeCannon = new Cannon();
            fourCannon = new Cannon();
            fiveCannon = new Cannon();
            sixCannon = new Cannon();
            sevenCannon = new Cannon();
            eightCannon = new Cannon();
            // Defining Cannons
            var Cannons = new Cannon[]
            {
                oneCannon,
                twoCannon,
                threeCannon,
                fourCannon,
                fiveCannon,
                sixCannon,
                sevenCannon,
                eightCannon
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
                var cannonAngle = 0f;
                var angleInterval = (Single)Math.PI / 4;
                var tankPosition = new SizeF(.5f, .5f);
                var cannonHeight = bounds.Height * .4f;
                foreach (var cannon in Cannons)
                {
                    cannon.Support = this;
                    cannon.Angle = cannonAngle;
                    cannon.TankPosition = tankPosition;
                    cannon.Bounds = new RectangleF(bounds.X, bounds.Y, bounds.Width, cannonHeight);
                    //
                    cannonAngle += angleInterval;
                }
            }
        }

        protected override void ExecuteShoot()
        {
            if(ShootStep == 0)
            {
                oneCannon.Shoot();
                threeCannon.Shoot();
                fiveCannon.Shoot();
                sevenCannon.Shoot();
                ShootStep -= Cannons.Length / 2 - 1;
            }
            else if (ShootStep == 1)
            {
                twoCannon.Shoot();
                fourCannon.Shoot();
                sixCannon.Shoot();
                eightCannon.Shoot();
                ShootStep -= Cannons.Length / 2 - 1;
            }
            else
            {
                ShootStep = 0;
            }
        }
    }
}
