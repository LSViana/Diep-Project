using Diep.GameEngine.Scenario.Cannons;
using Diep.GameEngine.Shared;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diep.GameEngine.Scenario.Shoots
{
    public class Rocketeer : Shoot, IToolSupport
    {
        private Cannon cannon;

        public Cannon[] Cannons { get; private set; }

        public Rocketeer()
        {
            // Simplest Constructor
            health *= 1.5f;
        }

        public override RectangleF Bounds { get => base.Bounds; set { base.Bounds = value; SetCannonBounds(); } }

        public virtual void SetCannonBounds()
        {
            if (Cannons != null)
            {
                var tankPosition = new SizeF(.5f, .5f);
                var angleInterval = (Single)Math.PI;
                var cannonAngle = (Single)Math.PI;
                var bounds = DrawingBounds;
                var cannonHeight = bounds.Height * .8f;
                var cannonBounds = new RectangleF(bounds.X, bounds.Y, bounds.Width * .8f, cannonHeight);
                //
                foreach (var cannon in Cannons)
                {
                    cannon.Support = this;
                    cannon.TankPosition = tankPosition;
                    cannon.Bounds = cannonBounds;
                    cannon.Angle = cannonAngle;
                    //
                    cannonAngle += angleInterval;
                }
            }
        }

        public override Single Angle
        {
            get { return angle; }
            set { base.Angle = value; angle = value; }
        }

        public TeamColor TeamColor { get; set; }

        public virtual void SetCannons()
        {
            cannon = new RocketeerSubCannon()
            {
                ShootRate = Skills_Manager.SkillsManager.MaxShootRate,
                Obliquity = .5f,
                AutoShoot = true,
                TeamColor = TeamColor,
                BaseSupport = Cannon.Support,
            };
            //
            Cannons = new Cannon[]
            {
                cannon
            };
            //
            SetCannonBounds();
        }

        protected override void PerformDraw(Graphics g)
        {
            var db = DrawingBounds;
            var o = g.Transform;
            var t = g.Transform;
            // Rotating
            t.RotateAt(base.Angle.ToDegree(), db.GetCenter());
            // Transforming
            g.Transform = t;
            // Drawing Cannons
            foreach (var cannon in Cannons)
            {
                cannon.Draw(g);
            }
            // Drawing Shoot
            var internalEllipse = db;
            internalEllipse.Inflate(-PenWidth / 2, -PenWidth / 2);
            var opacity = GetOpacityByte();
            var brush = Screen.GraphicsSupplier.GetSolidBrush(Cannon.Support.TeamColor, opacity);
            var pen = Screen.GraphicsSupplier.GetPen(opacity);
            g.FillEllipse(brush, db);
            g.DrawEllipse(pen, internalEllipse);
            // Detransforming
            g.Transform = o;
        }

        public override void Step(IEnumerable<object> data)
        {
            base.Step(data);
            //
            cannon.Step(data);
        }
    }
}
