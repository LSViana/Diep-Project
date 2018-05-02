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
    public class AutoGunner : Tank
    {
        private Cannon topMostCannon;
        private Cannon topCannon;
        private Cannon bottomCannon;
        private Cannon bottomMostCannon;
        private AutoCannon autoCannon;

        public Cannon[] OverCannons { get; private set; }

        public AutoGunner(GameScreen Screen, TeamColor TeamColor, float Weight) : base(Screen, TeamColor, Weight)
        {
            // Simplest Constructor
        }

        public override float Angle
        {
            get => base.Angle;
            set { var difference = base.Angle - value; autoCannon.BarrelAngle += difference; base.Angle = value; }
        }

        public override void SetCannons()
        {
            // Tests Purpose
            //base.SetCannons();
            // Creating Cannons
            topMostCannon = new Cannon();
            topCannon = new Cannon();
            bottomCannon = new Cannon();
            bottomMostCannon = new Cannon();
            autoCannon = new AutoCannon()
            {
                RangeLimit = -1f,
                Controllable = false,
            };
            // Defining Cannons
            var Cannons = new Cannon[]
            {
                topMostCannon,
                topCannon,
                bottomMostCannon,
                bottomCannon,
                autoCannon
            };
            var OverCannons = new Cannon[]
            {
                autoCannon
            };
            //
            this.OverCannons = OverCannons;
            this.Cannons = Cannons;
            RepositionCannons(Bounds);
        }

        public override void RepositionCannons(RectangleF value)
        {
            // Make sure the SetCannons method has been already called
            if (Cannons != null)
            {
                var cannonPercentage = .3f;
                var cannonMiddlePercentage = .32f;
                var cannonHeight = bounds.Height * cannonPercentage;
                // Defining Specific Properties
                topMostCannon.TankPosition = new SizeF(.5f, cannonPercentage / 2);
                bottomMostCannon.TankPosition = new SizeF(.5f, 1 - cannonPercentage / 2);
                topCannon.TankPosition = new SizeF(.5f, cannonMiddlePercentage);
                bottomCannon.TankPosition = new SizeF(.5f, 1 - cannonMiddlePercentage);
                autoCannon.TankPosition = new SizeF(.26f, .5f); // .26f instead .25f because it seems there is a little deviation at cannon ellipse position
                RectangleF cannonBounds = new RectangleF(bounds.X, bounds.Y, bounds.Width, cannonHeight);
                // Defining Similar Properties
                foreach (var cannon in Cannons)
                {
                    cannon.Support = this;
                }
                autoCannon.Support = this;
                //
                topCannon.Bounds = cannonBounds;
                bottomCannon.Bounds = cannonBounds;
                //
                cannonBounds.Width *= .8f;
                cannonBounds.X *= 1.25f;
                //
                topMostCannon.Bounds = cannonBounds;
                bottomMostCannon.Bounds = cannonBounds;
                cannonBounds.Width *= 1.1f;
                //
                autoCannon.Bounds = cannonBounds;
            }
        }

        protected override void DrawOverTools(Graphics g)
        {
            if (OverCannons != null)
            {
                foreach (var cannon in OverCannons)
                {
                    cannon.Draw(g);
                }
            }
        }
    }
}
