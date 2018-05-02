using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Diep.GameEngine.Shared;

namespace Diep.GameEngine.Scenario.Blocks
{
    public class Circle : Shape
    {
        public Circle(GameScreen Screen, TeamColor TeamColor) : base(Screen, TeamColor)
        {
            // Standard Property Values
            Health = 120f;
            Power = .275f;
        }

        public override void Draw(Graphics g)
        {
            // Avoid Drawing when out of Screen
            if (Bounds.IntersectsWith(Screen.DrawArea))
            {
                base.Draw(g);
                // Rotating
                var db = DrawingBounds;
                var o = g.Transform;
                var t = g.Transform;
                t.RotateAt(Spin, db.GetCenter());
                g.Transform = t;
                var alpha = GetOpacityByte();
                // Drawing
                g.FillEllipse(Screen.GraphicsSupplier.GetSolidBrush(TeamColor, alpha), db);
                var internalBounds = db;
                internalBounds.Inflate(-PenWidth / 2, -PenWidth / 2);
                g.DrawEllipse(Screen.GraphicsSupplier.GetPen(alpha), internalBounds);
                // Treating Rectangle
                if (colliding)
                    g.FillEllipse(Screen.GraphicsSupplier.GetSolidBrush(TeamColor.Red, Byte.MaxValue), db);
                // Derotating
                g.Transform = o;
                // Tests Purpose
                //g.FillRectangle(new SolidBrush(Color.Blue), new RectangleF(Bounds.GetCenter(), new SizeF(1, 1)));
                //g.FillEllipse(new SolidBrush(Color.Blue), Bounds);
                colliding = false;
            }
            else
            {

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