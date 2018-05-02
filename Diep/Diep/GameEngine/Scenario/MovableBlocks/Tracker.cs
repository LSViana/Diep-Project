using Diep.GameEngine.Scenario.Blocks;
using Diep.GameEngine.Scenario.Shoots;
using Diep.GameEngine.Shared;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diep.GameEngine.Scenario.MovableBlocks
{
    public class Tracker : Block
    {
        private IMovable target;
        private PointF[] trianglePoints;
        private PointF[] internalTrianglePoints;
        private RectangleF bounds;

        public Tracker(GameScreen Screen, TeamColor TeamColor) : base(Screen, TeamColor)
        {
            // Simplest Constructor
            Impact = StandardImpact;
            Stability = MinStability;
            Health = 100f;
            Power = .4f;
            RadarSize = new SizeF(200, 200);
            CanLeaveMap = true;
        }

        public override RectangleF Bounds
        {
            get { return bounds; }
            set { bounds = value; CalculateTrianglePoints(); }
        }

        public RectangleF RadarBounds
        {
            get
            {
                var bounds = DrawingBounds;
                bounds.Inflate(RadarSize);
                return bounds;
            }
        }

        public SizeF RadarSize { get; }

        public PointF Target { get; set; }

        private void CalculateTrianglePoints()
        {
            var db = DrawingBounds;
            // Getting Points with DrawingBounds
            trianglePoints = new PointF[]
            {
                new PointF(db.X, db.Y),
                new PointF(db.X + db.Width, db.Y + db.Height / 2),
                new PointF(db.X, db.Y + db.Height)
            };
            internalTrianglePoints = new PointF[]
            {
                new PointF(
                    PenWidth / 2 + db.X,
                    PenWidth / 2 + db.Y),
                new PointF(
                    -PenWidth + db.X + db.Width,
                    db.Y + db.Height / 2),
                new PointF(
                    PenWidth / 2 + db.X,
                    -PenWidth / 2 + db.Y + db.Height)
            };
        }

        public override void Draw(Graphics g)
        {
            // Avoid Drawing when out of Screen
            if (Bounds.IntersectsWith(Screen.DrawArea))
            {
                var db = DrawingBounds;
                base.Draw(g);
                // Rotating
                var o = g.Transform;
                var t = g.Transform;
                t.RotateAt(angle.ToDegree(), db.GetCenter());
                g.Transform = t;
                var alpha = GetOpacityByte();
                // Drawing
                g.FillPolygon(Screen.GraphicsSupplier.GetSolidBrush(TeamColor, alpha), trianglePoints);
                var internalBounds = DrawingBounds;
                internalBounds.Inflate(-PenWidth / 2, -PenWidth / 2);
                g.DrawPolygon(Screen.GraphicsSupplier.GetPen(alpha), internalTrianglePoints);
                // Treating Collision
                if (colliding)
                    g.FillPolygon(Screen.GraphicsSupplier.GetSolidBrush(TeamColor.Red, Byte.MaxValue), trianglePoints);
                // Derotating
                g.Transform = o;
                // Tests Purpose
                //g.FillRectangle(new SolidBrush(Color.Blue), new RectangleF(BoundsCenter, new SizeF(1, 1)));
                //g.DrawString(Health.ToString(), new Font("Segoe UI", 8), new SolidBrush(Color.Black), Bounds);
                //g.DrawString(angle.ToDegree().ToString(), new Font("Segoe UI", 8), new SolidBrush(Color.Black), new RectangleF(Bounds.GetCenter(), new SizeF(100, 100)));
                //g.DrawRectangle(new Pen(Color.Black, 1), Rectangle.Round(RadarBounds));
                //g.FillRectangle(new SolidBrush(Color.Blue), Bounds);
                colliding = false;
            }
        }

        public override bool VerifyCollision(ITouchable other)
        {
            var colliding = CollisionBounds.IntersectsWith(other.CollisionBounds);
            if (!(other is Block) && colliding)
                this.colliding = true;
            return colliding;
        }

        public override void Step(IEnumerable<object> data)
        {
            var userTank = Screen.UserTank;
            if (userTank.DrawingBounds.IntersectsWith(RadarBounds))
            {
                target = userTank;
                if (target is null)
                {
                    var boundsCenter = BoundsCenter;
                    Single distance = 0f, minimumDistance = Single.MaxValue;
                    foreach (var movableShape in Screen.MovableShapes.Where(a => !(a is Tracker)))
                    {
                        distance = (Single)Math.Sqrt(Math.Pow(boundsCenter.X - movableShape.Bounds.X, 2) + Math.Pow(boundsCenter.Y - movableShape.Bounds.Y, 2));
                        if (distance < minimumDistance)
                        {
                            minimumDistance = distance;
                            target = movableShape;
                        }
                    }
                }
            }
            long elapsed = Screen.Elapsed.Ticks;
            if (target != null)
            {
                Target = target.Bounds.GetCenter();
                var freshAngle = BoundsCenter.GetAngle(Target);
                Angle = freshAngle;
                var tempVector = Extensions.GetVectorFromAngle(freshAngle);
                movementVector.X += tempVector.X * .00001f * elapsed;
                movementVector.Y += tempVector.Y * .00001f * elapsed;
                Move(movementVector);
            }
            else
            {
                angle += .0000001f * elapsed;
                var vector = Extensions.GetVectorFromAngle(angle);
                Move(vector);
            }
            //
            //if (Angle > Math.PI * 2)
            //    Angle -= (Single)Math.PI * 2;
            //if (Math.Cos(freshAngle) > 0)
            //{
            //    var fSin = Math.Sin(freshAngle);
            //    var sin = Math.Sin(angle);
            //    //
            //    if (fSin > 0 && sin < 0)
            //    {

            //    }
            //    else if (fSin < 0 && sin > 0)
            //    {

            //    }
            //}
            //Angle += (freshAngle - Angle) / 10000;
        }

        public override void Collide(ITouchable other)
        {
            if (!(other is Block))
            {
                // Damage to this
                Health -= other.Power * Screen.Elapsed.Ticks;
                if (Health <= 0)
                {
                    Active = false;
                    //Screen.MovableShapes.Remove(this);
                }
            }
            if (Active)
            {
                // Movement to opposite side
                var otherCenter = other.CollisionBounds.GetCenter();
                var collisionAngle = otherCenter.GetAngle(Bounds.GetCenter());
                if (Math.Abs(movementVector.Y) < 1)
                    movementVector.Y += (Single)(Math.Sin(collisionAngle) / Stability) * other.Impact;
                if (Math.Abs(movementVector.X) < 1)
                    movementVector.X += (Single)(Math.Cos(collisionAngle) / Stability) * other.Impact;
            }
        }
    }
}
