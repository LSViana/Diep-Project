﻿using System;
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
    public class Assassin : Tank
    {
        private Cannon cannon;

        public Assassin(GameScreen Screen, TeamColor TeamColor, float Weight) : base(Screen, TeamColor, Weight)
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
            cannon.ShootRate = SkillsManager.StandardShootRate * .4f;
            // Defining Cannons
            var Cannons = new Cannon[]
            {
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
                var cannonPercentage = .5f;
                var cannonHeight = bounds.Height * cannonPercentage;
                //
                foreach (var cannon in Cannons)
                {
                    cannon.Support = this;
                }
                //
                var cannonBounds = new RectangleF(bounds.X, bounds.Y, bounds.Width * 1.2f, cannonHeight);
                //
                cannon.TankPosition = new SizeF(.5f, .5f);
                cannon.Bounds = cannonBounds;
            }
        }
    }
}