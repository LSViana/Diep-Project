using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Diep.GameEngine.Scenario.Shoots;

namespace Diep.GameEngine.Scenario.Cannons
{
    public class DroneCannon : Cannon
    {
        private Boolean autoShoot;

        public DroneCannon()
        {
            // Standard Property Values
            ShootRate *= .5f;
            Obliquity = .3f;
            autoShoot = true;
            DroneLimit = 4;
            Controllable = true;
        }

        public override Boolean AutoShoot
        {
            get { return autoShoot; }
            set { /* Nothing */ }
        }

        public Boolean Controllable { get; set; }

        public Int32 DroneLimit { get; set; }

        public Int32 DroneCount { get; private set; }

        public override Shoot GetShoot()
        {
            Drone drone = new Drone()
            {
                Controllable = Controllable
            };
            drone.DroneDisposed += Drone_DroneDisposed;
            return drone;
        }

        private void Drone_DroneDisposed()
        {
            DroneCount--;
        }

        protected override SizeF GetShootSize()
        {
            var shootHeight = Bounds.Height;
            var triangleSide = shootHeight * .86f; // 0.86 is equals 1/(1.16), that came from visual tests at PowerPoint
            //
            SizeF shootSize = new SizeF(triangleSide, bounds.Height);
            return shootSize;
        }

        public override void Shoot()
        {
            if (!VerifyShootAllowed())
                return;
            lastShootTime = default(DateTime);
            if (DroneCount < DroneLimit)
            {
                base.Shoot();
                DroneCount++;
            }
        }

        protected override void SetShootProperties(Shoot shoot)
        {
            base.SetShootProperties(shoot);
            //
            shoot.Speed *= (Single)(Shared.Extensions.Random.NextDouble() * .6f + .7f);
        }
    }
}