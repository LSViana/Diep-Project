using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Diep.GameEngine.Scenario.Shoots;
using Diep.GameEngine.Shared;

namespace Diep.GameEngine.Scenario.Cannons
{
    public class DeployerCannon : Cannon
    {
        public DeployerCannon()
        {
            // Simplest Constructor
            Speed = Skills_Manager.SkillsManager.MinShootSpeed;
        }

        public override Shoot GetShoot()
        {
            var destroyer = new Destroyer()
            {
                Cannon = this,
                TeamColor = Support.TeamColor,
                BaseSupport = Support,
            };
            destroyer.SetCannons();
            return destroyer;
        }
    }
}
