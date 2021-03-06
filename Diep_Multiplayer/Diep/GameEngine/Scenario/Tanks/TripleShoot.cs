﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Diep.GameEngine.Scenario.Cannons;
using Diep.GameEngine.Shared;

namespace Diep.GameEngine.Scenario.Tanks
{
    public class TripleShoot : Tank
    {
        public TripleShoot(GameScreen Screen, TeamColor TeamColor, float Weight) : base(Screen, TeamColor, Weight)
        {
            // Simplest Constructor
        }

        public override void SetCannons()
        {
            // Tests Purpose
            //base.SetCannons();
            // Creating Cannons
            var leftCannon = new Cannon();
            var centerCannon = new Cannon();
            var rightCannon = new Cannon();
            // Defining Cannons
            var Cannons = new Cannon[]
            {
                leftCannon,
                centerCannon,
                rightCannon
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
                var cannonAngle = -(Single)Math.PI / 4;
                var tankPosition = new SizeF(.5f, .5f);
                var cannonHeight = bounds.Height * .45f;
                foreach (var cannon in Cannons)
                {   
                    cannon.Support = this;
                    cannon.Angle = cannonAngle;
                    cannon.TankPosition = tankPosition;
                    cannon.Bounds = new RectangleF(bounds.X, bounds.Y, bounds.Width, cannonHeight);
                    //
                    cannonAngle += (Single)Math.PI / 4;
                }
            }
        }
    }
}
