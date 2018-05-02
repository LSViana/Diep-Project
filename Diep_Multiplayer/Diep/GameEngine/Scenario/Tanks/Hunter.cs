using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Diep.GameEngine.Scenario.Cannons;
using Diep.GameEngine.Scenario.Skills_Manager;
using Diep.GameEngine.Shared;

namespace Diep.GameEngine.Scenario.Tanks
{
    public class Hunter : Tank
    {
        private Cannon cannon;
        private Cannon smallCannon;

        public Hunter(GameScreen Screen, TeamColor TeamColor, float Weight) : base(Screen, TeamColor, Weight)
        {
            // Simplest Constructor
            ZoomFactor = .85f;
            Screen.ZoomFactor = ZoomFactor;
            Speed = MinSpeed;
        }

        public override void SetCannons()
        {
            // Tests Purpose
            //base.SetCannons();
            // Creating Cannons
            cannon = new Cannon();
            smallCannon = new Cannon();
            cannon.ShootRate = SkillsManager.StandardShootRate * .8f;
            smallCannon.ShootRate = SkillsManager.StandardShootRate * .8f;
            // Defining Cannons
            var Cannons = new Cannon[]
            {
                smallCannon,
                cannon,
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
                var cannonPercentage = .55f;
                //
                var cannonBounds = new RectangleF(bounds.X, bounds.Y, bounds.Width * .9f, bounds.Height * cannonPercentage);
                //
                foreach (var cannon in Cannons)
                {
                    cannon.Support = this;
                }
                //
                cannon.Bounds = cannonBounds;
                cannon.TankPosition = new SizeF(.5f, .5f);
                cannonPercentage = .4f;
                cannonBounds.Height = bounds.Height * cannonPercentage;
                cannonBounds.Width = bounds.Width;
                //
                smallCannon.Bounds = cannonBounds;
                smallCannon.TankPosition = new SizeF(.5f, .5f);
            }
        }

        protected override void ExecuteShoot()
        {
            if(ShootStep == 0)
            {
                cannon.Shoot();
            }
            else if(ShootStep == 1)
            {
                smallCannon.Shoot();
            }
            else
            {
                ShootStep = 0;
            }
        }
    }
}
