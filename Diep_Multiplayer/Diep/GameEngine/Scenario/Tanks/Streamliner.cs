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
    public class Streamliner : Tank
    {
        private Cannon oneCannon;
        private Cannon twoCannon;
        private Cannon threeCannon;
        private Cannon fourCannon;
        private Cannon fiveCannon;

        public Streamliner(GameScreen Screen, TeamColor TeamColor, float Weight) : base(Screen, TeamColor, Weight)
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
            oneCannon = new Cannon();
            twoCannon = new Cannon();
            threeCannon = new Cannon();
            fourCannon = new Cannon();
            fiveCannon = new Cannon();
            // Defining Cannons
            var Cannons = new Cannon[]
            {
                oneCannon,
                twoCannon,
                threeCannon,
                fourCannon,
                fiveCannon,
            };
            //
            this.Cannons = Cannons;
            //
            foreach (var cannon in Cannons)
            {
                cannon.ShootRate = SkillsManager.StandardShootRate * .4f;
            }
            RepositionCannons(Bounds);
        }

        public override void RepositionCannons(RectangleF value)
        {
            // Make sure the SetCannons method has been already called
            if (Cannons != null)
            {
                var cannonHeightPercentage = .5f;
                var cannonWidthPercentage = 1.2f;
                var cannonHeight = bounds.Height * cannonHeightPercentage;
                //
                var cannonBounds = new RectangleF(bounds.X, bounds.Y, bounds.Width * cannonWidthPercentage, cannonHeight);
                //
                foreach (var cannon in Cannons)
                {
                    cannon.Support = this;
                    cannon.Bounds = cannonBounds;
                    cannon.TankPosition = new SizeF(.5f, .5f);
                    //
                    cannonWidthPercentage -= .1f;
                    cannonBounds.Width = bounds.Width * cannonWidthPercentage;
                }
            }
        }

        protected override void ExecuteShoot()
        {
            // Asynchronous Shoot Mode
            Cannons[lastCannonIndex].Shoot();
            lastCannonIndex++;
            if (lastCannonIndex >= Cannons.Length)
                lastCannonIndex = 0;
        }
    }
}
