using Diep.GameEngine.Scenario.Shoots;
using Diep.GameEngine.Shared;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diep.GameEngine.Scenario.Cannons
{
    public class RocketeerSubCannon : Cannon
    {
        public RocketeerSubCannon()
        {
            // Simplest Constructor
        }

        public IToolSupport BaseSupport { get; set; }

        public override Shoot GetShoot()
        {
            return new RocketeerBullet()
            {
                BaseSupport = BaseSupport
            };
        }

        public override void PullSupport(SizeF shootSize, Vector2D vector)
        {
            var supportMV = Support.MovementVector;
            supportMV.X -= (vector.X * shootSize.Height / 20) / Support.Stability * (1 + Power) * SkillsManager.ShootImpact;
            supportMV.Y -= (vector.Y * shootSize.Height / 20) / Support.Stability * (1 + Power) * SkillsManager.ShootImpact;
            Support.MovementVector = supportMV;
        }
    }
}
