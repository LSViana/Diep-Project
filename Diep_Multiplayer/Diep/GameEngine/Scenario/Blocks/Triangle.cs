using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Diep.GameEngine.Shared;

namespace Diep.GameEngine.Scenario.Blocks
{
    public class Triangle : Shape
    {
        private PointF[] trianglePoints;
        private PointF[] internalTrianglePoints;
        private RectangleF bounds;

        public override RectangleF Bounds
        {
            get { return bounds; }
            set { bounds = value; CalculateTrianglePoints(); }
        }

        private void CalculateTrianglePoints()
        {
            #region Common Triangle
            //trianglePoints = new PointF[]
            //{
            //    new PointF(Bounds.X, Bounds.Y + Bounds.Height),
            //    new PointF(Bounds.X + Bounds.Width / 2, Bounds.Y),
            //    new PointF(Bounds.X + Bounds.Width, Bounds.Y + Bounds.Height)
            //};
            //internalTrianglePoints = new PointF[]
            //{
            //    new PointF(
            //        PenWidth / 2 + Bounds.X,
            //        -PenWidth / 2 + Bounds.Y + Bounds.Height),
            //    new PointF(
            //        Bounds.X + Bounds.Width / 2,
            //        PenWidth / 2 + Bounds.Y + ((Single)Math.Sqrt(Bounds.Width * Bounds.Width / 4 + Bounds.Height * Bounds.Height) - Bounds.Width)),
            //    new PointF(
            //        -PenWidth / 2 + Bounds.X + Bounds.Width,
            //        -PenWidth / 2 + Bounds.Y + Bounds.Height)
            //}; 
            #endregion
            // Getting Points with DrawingBounds
            trianglePoints = new PointF[]
            {
                new PointF(DrawingBounds.X, DrawingBounds.Y + DrawingBounds.Height),
                new PointF(DrawingBounds.X + DrawingBounds.Width / 2, DrawingBounds.Y),
                new PointF(DrawingBounds.X + DrawingBounds.Width, DrawingBounds.Y + DrawingBounds.Height)
            };
            internalTrianglePoints = new PointF[]
            {
                new PointF(
                    PenWidth / 2 + DrawingBounds.X,
                    -PenWidth / 2 + DrawingBounds.Y + DrawingBounds.Height),
                new PointF(
                    DrawingBounds.X + DrawingBounds.Width / 2,
                    PenWidth / 2 + DrawingBounds.Y),
                new PointF(
                    -PenWidth / 2 + DrawingBounds.X + DrawingBounds.Width,
                    -PenWidth / 2 + DrawingBounds.Y + DrawingBounds.Height)
            };
        }

        public Triangle(GameScreen Screen, TeamColor TeamColor) : base(Screen, TeamColor)
        {
            // Standard Property Values
            Health = 160f;
            Power = .35f;
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
                t.RotateAt(Spin, db.GetCenter());
                g.Transform = t;
                var alpha = GetOpacityByte();
                // Drawing
                g.FillPolygon(Screen.GraphicsSupplier.GetSolidBrush(TeamColor, alpha), trianglePoints);
                g.DrawPolygon(Screen.GraphicsSupplier.GetPen(alpha), internalTrianglePoints);
                // Treating Collision
                if (colliding)
                    g.FillPolygon(new SolidBrush(Color.FromArgb(255, Color.Red)), trianglePoints);
                // Derotating
                g.Transform = o;
                // Tests Purpose
                //g.FillRectangle(new SolidBrush(Color.Blue), new RectangleF(Bounds.GetCenter(), new SizeF(1, 1)));
                //g.FillRectangle(new SolidBrush(Color.Green), Bounds);
                //
                colliding = false;
            }
        }

        public override bool VerifyCollision(ITouchable other)
        {
            var colliding = CollisionBounds.IntersectsWith(other.CollisionBounds);
            if (!(other is Shape) && colliding)
                return this.colliding = true;
            else
                return colliding;
        }
    }
}
