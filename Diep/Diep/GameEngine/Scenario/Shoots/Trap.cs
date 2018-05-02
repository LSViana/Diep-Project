using Diep.GameEngine.Scenario.Screens;
using Diep.GameEngine.Scenario.Tanks;
using Diep.GameEngine.Shared;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diep.GameEngine.Scenario.Shoots
{
    public class Trap : Shoot
    {
        private PointF[] externalPolygon;
        private PointF[] internalPolygon;

        public Trap()
        {
            // Simplest Constructor
            CavityPercentage = .2f;
            FrayRate = StandardFrayRate / 15;
            Impact = .1f;
            Power *= 2;
        }

        public Single CavityPercentage { get; set; }

        public Single Spin { get; set; }

        public override RectangleF Bounds
        {
            get => base.Bounds;
            set { base.Bounds = value; CalculateDrawingPolygon(); }
        }

        private void CalculateDrawingPolygon()
        {
            var db = DrawingBounds;
            var triangleHeight = db.Height;
            var triangleCenter = new PointF(db.X + db.Width / 2, db.Y + (triangleHeight * .668f));
            var distanceToMove = triangleHeight * .33f * (1 - CavityPercentage);
            //
            externalPolygon = new PointF[]
            {
                new PointF(db.X, db.Y + db.Height),
                triangleCenter.Move(distanceToMove, Extensions.GetVectorFromAngle(7 * (Single)Math.PI / 6)),
                new PointF(db.X + db.Width / 2, db.Y),
                triangleCenter.Move(distanceToMove, Extensions.GetVectorFromAngle(11 * (Single)Math.PI / 6)),
                new PointF(db.X + db.Width, db.Y + db.Height),
                triangleCenter.Move(distanceToMove, new Vector2D(0, 1)),
            };
            //
            internalPolygon = new PointF[]
            {
                new PointF(PenWidth / 1.3f + db.X, -PenWidth / 2 + db.Y + db.Height),
                new PointF(PenWidth / 2.2f + externalPolygon[1].X, PenWidth / 2.2f + externalPolygon[1].Y),
                new PointF(db.X + db.Width / 2, PenWidth / 1.3f + db.Y),
                new PointF(-PenWidth / 2.2f + externalPolygon[3].X, PenWidth / 2.2f + externalPolygon[3].Y),
                new PointF(-PenWidth / 1.3f + db.X + db.Width, -PenWidth / 2 + db.Y + db.Height),
                new PointF(externalPolygon[5].X, -PenWidth / 2 +externalPolygon[5].Y),
            };
        }

        protected override void PerformDraw(Graphics g)
        {
            var db = DrawingBounds;
            // Transforming
            var o = g.Transform;
            var t = g.Transform;
            t.RotateAt(Spin.ToDegree(), db.GetCenter());
            g.Transform = t;
            // Drawing
            var brush = Screen.GraphicsSupplier.GetSolidBrush(Cannon.Support.TeamColor, GetOpacityByte());
            var pen = Screen.GraphicsSupplier.GetPen(GetOpacityByte());
            g.FillPolygon(brush, externalPolygon);
            g.DrawPolygon(pen, internalPolygon);
            // Detransforming
            g.Transform = o;
        }

        protected override void MoveShoot(Vector2D vector, long elapsedTicks)
        {
            if (Bounds.X < -100 || Bounds.X > Cannon.Support.Screen.Map.Bounds.Width + 100 || Bounds.Y < -100 || Bounds.Y > Cannon.Support.Screen.Map.Bounds.Height + 100)
                if (Cannon.Support.Screen is PlayScreen PScreen)
                    PScreen.MovableShoots.Remove(this);
            var next = Bounds.Location;
            var distance = Speed * elapsedTicks;
            Bounds = new RectangleF(next.Move(distance, vector), Bounds.Size);
            // Deceleration
            movementVector.X *= 1 - (elapsedTicks * .000001f);
            movementVector.Y *= 1 - (elapsedTicks * .000001f);
        }

        public override bool VerifyCollision(ITouchable other)
        {
            if (other is Trap trap)
            {
                return CollisionBounds.IntersectsWith(trap.CollisionBounds);
            }
            if (other is Shoot shoot)
                if (shoot.Cannon.Support == Cannon.Support)
                    return false;
            if (other is Tank tank)
                if (tank == Cannon.Support)
                    return false;
            return CollisionBounds.IntersectsWith(other.CollisionBounds);
        }

        public override void Collide(ITouchable other)
        {
            // Avoiding Fire Friend
            if (other is Tank tank)
                if (tank == Cannon.Support)
                    return;
            // Damage to this
            Boolean sameTank = false;
            if (other is Trap trap)
            {
                sameTank = trap.Cannon.Support == Cannon.Support;
                if (!sameTank)
                {
                    health -= (1 - Endurance) * other.Power * Screen.Elapsed.Ticks;
                    if (Health <= 0)
                    {
                        Active = false;
                        //Cannon.Tank.Screen.MovableShoots.Remove(this);
                    }
                }
            }
            if (Active)
            {
                // Movement to opposite side
                var otherCenter = other.CollisionBounds.GetCenter();
                var collisionAngle = otherCenter.GetAngle(Bounds.GetCenter());
                var impact = other.Impact;
                if (sameTank)
                    impact *= .001f;
                if (Math.Abs(movementVector.Y) < 1)
                    movementVector.Y += (Single)(Math.Sin(collisionAngle) / Stability) * impact;
                if (Math.Abs(movementVector.X) < 1)
                    movementVector.X += (Single)(Math.Cos(collisionAngle) / Stability) * impact;
            }
        }
    }
}
