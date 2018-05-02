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
    public class Destroyer : Shoot, IToolSupport
    {
        private Cannon leftCannon;
        private Cannon rightCannon;

        public Destroyer()
        {
            // Simplest Constructor
            Stability = MaxStability;
        }

        public override RectangleF Bounds { get => base.Bounds; set { base.Bounds = value; SetCannonBounds(); } }

        public long Id { get; set; }

        public virtual void SetCannons()
        {
            leftCannon = new DeployerSubCannon()
            {
                ShootRate = 8,
                Support = this,
                BaseSupport = Cannon.Support,
            };
            rightCannon = new DeployerSubCannon()
            {
                ShootRate = 8,
                Support = this,
                BaseSupport = Cannon.Support,
            };
            //
            Cannons = new Cannon[]
            {
                leftCannon,
                rightCannon
            };
            //
            SetCannonBounds();
        }

        private void SetCannonBounds()
        {
            if (Cannons != null)
            {
                var tankPosition = new SizeF(.5f, .5f);
                var angleInterval = (Single)Math.PI;
                var cannonAngle = 0f;
                var bounds = DrawingBounds;
                var cannonHeight = bounds.Height * .5f;
                var cannonBounds = new RectangleF(bounds.X, bounds.Y, bounds.Width * .75f, cannonHeight);
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

        public Cannon[] Cannons { get; set; }

        public Single SpinningMultiplier { get; set; } = .0000002f;

        public TeamColor TeamColor { get; set; }

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
            base.Angle = base.Angle + (Single)Math.PI * SpinningMultiplier * Screen.Elapsed.Ticks;
            //
            foreach (var cannon in Cannons)
            {
                cannon.Shoot();
            }
        }

        public override bool VerifyCollision(ITouchable other)
        {
            if (other is DestroyerBullet destroyerBullet)
            {
                var otherDestroyer = destroyerBullet.Cannon.Support as Destroyer;
                if (otherDestroyer == this)
                {
                    return false;
                }
                else if (otherDestroyer.Cannon.Support == Cannon.Support)
                {
                    return false;
                }
            }
            else if (other is Destroyer destroyer)
            {
                if (destroyer.Cannon.Support == Cannon.Support)
                {
                    return false;
                }
            }
            return base.VerifyCollision(other);
        }
    }
}
