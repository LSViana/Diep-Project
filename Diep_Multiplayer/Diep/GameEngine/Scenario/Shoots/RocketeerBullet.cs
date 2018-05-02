using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Diep.GameEngine.Scenario.Tanks;
using Diep.GameEngine.Shared;

namespace Diep.GameEngine.Scenario.Shoots
{
    public class RocketeerBullet : Shoot
    {
        public RocketeerBullet()
        {
            // Simplest Constructor
        }

        public override bool VerifyCollision(ITouchable other)
        {
            if (other is Rocketeer rocketeer && rocketeer.Cannon.Support == BaseSupport)
            {
                return false;
            }
            else if (other is Tank tank && tank == BaseSupport)
            {
                return false;
            }
            else if (other is RocketeerBullet rocketeerBullet && rocketeerBullet.BaseSupport == BaseSupport)
            {
                return false;
            }
            return base.VerifyCollision(other);
        }
    }
}
