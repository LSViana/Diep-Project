using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Diep.GameEngine.Scenario.Tanks;
using Diep.GameEngine.Shared;

namespace Diep.GameEngine.Scenario.Shoots
{
    public class DestroyerBullet : Shoot
    {
        public DestroyerBullet()
        {
            // Simplest Constructor
        }

        public override bool VerifyCollision(ITouchable other)
        {
            if (other is Destroyer destroyer && destroyer.Cannon.Support == BaseSupport)
            {
                return false;
            }
            else if (other is Tank tank && tank == BaseSupport)
            {
                return false;
            }
            else if (other is DestroyerBullet destroyerBullet && destroyerBullet.BaseSupport == BaseSupport)
            {
                return false;
            }
            return base.VerifyCollision(other);
        }
    }
}
