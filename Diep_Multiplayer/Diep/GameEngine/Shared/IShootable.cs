using Diep.GameEngine.Scenario.Shoots;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diep.GameEngine.Shared
{
    public interface IShootable
    {
        Shoot GetShoot();
        void Shoot();
    }
}
