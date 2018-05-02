using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Diep.GameEngine.Scenario.Shoots;

namespace Diep.GameEngine.Scenario.Cannons
{
    public class RocketeerCannon : Cannon
    {
        public RocketeerCannon()
        {
            // Simplest Constructor
            SkillsManager.ShootSpeed = Skills_Manager.SkillsManager.MinShootSpeed;
        }

        public override Shoot GetShoot()
        {
            var rocketeer = new Rocketeer()
            {
                Cannon = this,
                TeamColor = Support.TeamColor,
            };
            rocketeer.SetCannons();
            return rocketeer;
        }

        protected override void SetShootProperties(Shoot shoot)
        {
            base.SetShootProperties(shoot);
        }
    }
}