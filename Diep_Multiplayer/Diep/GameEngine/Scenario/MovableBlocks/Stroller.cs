using Diep.GameEngine.Scenario.Blocks;
using Diep.GameEngine.Shared;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diep.GameEngine.Scenario.MovableBlocks
{
    public class Stroller : Shape
    {
        private PointF[] trianglePoints;
        private PointF[] internalTrianglePoints;
        private RectangleF bounds;
        private bool targetAchieved;

        public IMovable TargetSource { get; private set; }

        public Stroller(GameScreen Screen, TeamColor TeamColor) : base(Screen, TeamColor)
        {
            // Simplest Constructor
            Impact = StandardImpact;
            Stability = MinStability;
            CanLeaveMap = true;
        }

        public override RectangleF Bounds
        {
            get { return bounds; }
            set { bounds = value; CalculateTrianglePoints(); }
        }

        private void CalculateTrianglePoints()
        {
            // Getting Points with DrawingBounds
            trianglePoints = new PointF[]
            {
                new PointF(DrawingBounds.X, DrawingBounds.Y),
                new PointF(DrawingBounds.X + DrawingBounds.Width, DrawingBounds.Y + DrawingBounds.Height / 2),
                new PointF(DrawingBounds.X, DrawingBounds.Y + DrawingBounds.Height)
            };
            internalTrianglePoints = new PointF[]
            {
                new PointF(
                    PenWidth / 2 + DrawingBounds.X,
                    PenWidth / 2 + DrawingBounds.Y),
                new PointF(
                    -PenWidth + DrawingBounds.X + DrawingBounds.Width,
                    DrawingBounds.Y + DrawingBounds.Height / 2),
                new PointF(
                    PenWidth / 2 + DrawingBounds.X,
                    -PenWidth / 2 + DrawingBounds.Y + DrawingBounds.Height)
            };
        }

        private bool dynamicTarget;

        public PointF Target { get; set; }

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
                //g.FillRectangle(new SolidBrush(Color.Blue), Bounds);
                colliding = false;
            }
            //
        }

        public override bool VerifyCollision(ITouchable other)
        {
            return CollisionBounds.IntersectsWith(other.CollisionBounds);
        }

        public override void Step(IEnumerable<object> data)
        {
            if (targetAchieved || Target == PointF.Empty)
            {
                RandomizeTarget();
            }
            // Updating Target
            if (dynamicTarget && TargetSource != null)
            {
                if (TargetSource is Shape targetBlock)
                {
                    if (!targetBlock.Active)
                        RandomizeTarget();
                }
                Target = TargetSource.Bounds.GetCenter();
            }
            //
            var freshAngle = BoundsCenter.GetAngle(Target, false);
            if ((Math.Sin(freshAngle) > 0 && Math.Sin(Angle) > 0) || (Math.Sin(freshAngle) < 0 && Math.Sin(Angle) < 0))
                Angle += (freshAngle - Angle) / 10000;
            else
            {
                Angle += (freshAngle - Angle) / 100000;
            }
            //
            Move(movementVector);
            // Verifying target reaching
            //var center = Bounds.GetCenter();
            //var distance = Math.Sqrt(Math.Pow(Target.X - center.X, 2) + Math.Pow(Target.Y - center.Y, 2));
            //if (distance < 5)
            //{
            //    RandomizeTarget();
            //}
        }

        private void RandomizeTarget()
        {
            int avaliableTargets = Screen.Shapes.ToArray().Count(a => !(a is Stroller || a is Tracker) && (a as Shape).Active);
            if (avaliableTargets > 0)
            {
                var target = Screen.Shapes.ToArray().Where(a => !(a is Stroller || a is Tracker)).OrderBy(a => Guid.NewGuid()).FirstOrDefault();
                if (target != null)
                {
                    var targetCenter = target.Bounds.GetCenter();
                    if (TargetSource != null && target == TargetSource && avaliableTargets > 1)
                        RandomizeTarget();
                    Target = targetCenter;
                    targetAchieved = false;
                    TargetSource = target;
                    dynamicTarget = true;
                }
                else
                    RandomizeTarget();
            }
            else
            {
                dynamicTarget = false;
                Target = new PointF((Single)(Extensions.Random.NextDouble() * Screen.Map.Bounds.Width), (Single)(Extensions.Random.NextDouble() * Screen.Map.Bounds.Height));
            }
        }

        public override void Collide(ITouchable other)
        {
            // Movement to opposite side
            var otherCenter = other.CollisionBounds.GetCenter();
            var center = Bounds.GetCenter();
            var collisionAngle = otherCenter.GetAngle(center);
            var difference = collisionAngle - Angle;
            if (Math.Abs(movementVector.Y) < 1)
                movementVector.Y += (Single)(Math.Sin(collisionAngle) / Stability) * other.Impact;
            if (Math.Abs(movementVector.X) < 1)
                movementVector.X += (Single)(Math.Cos(collisionAngle) / Stability) * other.Impact;
            //
            RandomizeTarget();
        }
    }
}
