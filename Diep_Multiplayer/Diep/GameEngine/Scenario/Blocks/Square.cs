using Diep.GameEngine.Scenario.MovableBlocks;
using Diep.GameEngine.Scenario.Screens;
using Diep.GameEngine.Shared;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diep.GameEngine.Scenario.Blocks
{
    public class Square : Shape
    {
        public Square(GameScreen Screen, TeamColor TeamColor) : base(Screen, TeamColor)
        {
            // Standard Property Values
            Health = 100f;
            Power = .2f;
        }

        public override PointF BoundsCenter
        {
            get { return PointF.Empty; }
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
                g.FillRectangle(Screen.GraphicsSupplier.GetSolidBrush(TeamColor, alpha), db);
                var internalBounds = DrawingBounds;
                internalBounds.Inflate(-PenWidth / 2, -PenWidth / 2);
                g.DrawRectangle(Screen.GraphicsSupplier.GetPen(alpha), Rectangle.Round(internalBounds));
                // Treating Collision
                if (colliding)
                    g.FillRectangle(Screen.GraphicsSupplier.GetSolidBrush(TeamColor.Red, Byte.MaxValue), db);
                // Derotating
                g.Transform = o;
                // Tests Purpose
                //g.FillRectangle(new SolidBrush(Color.Blue), new RectangleF(BoundsCenter, new SizeF(1, 1)));
                //g.DrawString(Health.ToString(), new Font("Segoe UI", 8), new SolidBrush(Color.Black), Bounds);
                //g.FillRectangle(new SolidBrush(Color.Blue), Bounds);
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
