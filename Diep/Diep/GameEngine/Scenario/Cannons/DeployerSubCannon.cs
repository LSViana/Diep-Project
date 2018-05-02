using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Diep.GameEngine.Scenario.Shoots;
using Diep.GameEngine.Scenario.Tanks;
using Diep.GameEngine.Shared;

namespace Diep.GameEngine.Scenario.Cannons
{
    public class DeployerSubCannon : Cannon
    {
        public DeployerSubCannon()
        {
            // Simplest Constructor
        }

        public IToolSupport BaseSupport { get; set; }

        public override Shoot GetShoot()
        {
            return new DestroyerBullet()
            {
                BaseSupport = BaseSupport,
            };
        }
    }
}
