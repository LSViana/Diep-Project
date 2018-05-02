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
    public class Twin : Tank
    {
        private Cannon leftCannon;
        private Cannon rightCannon;

        public Twin(GameScreen Screen, TeamColor TeamColor, float Weight) : base(Screen, TeamColor, Weight)
        {
            // Simplest Constructor
            ShootSteps = 2;
        }

        public override void SetCannons()
        {
            // Tests Purpose
            //base.SetCannons();
            // Creating Cannons
            leftCannon = new Cannon();
            rightCannon = new Cannon();
            // Defining Cannons
            var Cannons = new Cannon[]
            {
                leftCannon,
                rightCannon
            };
            //
            this.Cannons = Cannons;
            RepositionCannons(Bounds);
        }

        public override void RepositionCannons(RectangleF bounds)
        {
            if (Cannons != null)
            {
                var offsetY = Bounds.Height * .225f;
                foreach (var cannon in Cannons)
                {
                    cannon.TankPosition = new SizeF(.5f, offsetY / bounds.Height);
                    cannon.Support = this;
                    cannon.Bounds = new RectangleF(new PointF(bounds.Location.X, bounds.Location.Y + offsetY), new SizeF(bounds.Width, bounds.Height * .45f));
                    offsetY += bounds.Height - cannon.Bounds.Height;
                }
            }
        }

        protected override void ExecuteShoot()
        {
            
            if(ShootStep == 0)
            {
                leftCannon.Shoot();
            }
            else if(ShootStep == 1)
            {
                rightCannon.Shoot();
            }
            else
            {
                ShootStep = 0;
            }
        }
    }
}