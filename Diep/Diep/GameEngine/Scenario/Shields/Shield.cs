using Diep.GameEngine.Shared;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diep.GameEngine.Scenario.Shields
{
    public class Shield : IGameObject, IDrawable
    {
        protected const Single PenWidth = GameController.PenWidth;
        protected PointF[] externalDrawingPolygon;
        private PointF[] internalDrawingPolygon;
        protected RectangleF bounds;

        public Shield()
        {
            // Standard Property Values
            Roughness = .16f;
            Thickness = 0;
            Edges = 6;
            Opacity = 1;
        }

        public Single SpinningMultiplier { get; set; } = .00000004f;

        public IToolSupport Support { get; set; }

        public Single Opacity { get; set; }

        public Single Thickness { get; set; }

        public Single Roughness { get; set; }

        public TeamColor TeamColor { get; set; }

        public Int32 Edges { get; set; }

        public RectangleF Bounds
        {
            get { return bounds; }
            set { bounds = value; CalculateDrawingPolygon(); }
        }

        public virtual Byte GetOpacityByte()
        {
            return (byte)(Opacity * Byte.MaxValue);
        }

        public Single Angle { get; set; }

        private void CalculateDrawingPolygon()
        {
            var tempExternal = new PointF[Edges * 2];
            var tempInternal = new PointF[Edges * 2];
            // Considering the Support is a Circle
            var supportRadius = bounds.Width / 2;
            var supportCenter = Support.DrawingBounds.GetCenter();
            var edgeRadius = supportRadius * (1 + Roughness);
            // Calculating Points
            var angle = 0f;
            var angleInterval = 2 * (Single)Math.PI / (Edges * 2);
            for (int i = 0; i < Edges * 2; i += 2)
            {
                tempExternal[i] = supportCenter.Move(edgeRadius, Extensions.GetVectorFromAngle(angle));
                tempInternal[i] = supportCenter.Move(edgeRadius - PenWidth / 2, Extensions.GetVectorFromAngle(angle));
                angle += angleInterval;
                tempExternal[i + 1] = supportCenter.Move(supportRadius * (1 + Thickness), Extensions.GetVectorFromAngle(angle));
                tempInternal[i + 1] = supportCenter.Move(supportRadius * (1 + Thickness) - PenWidth / 2, Extensions.GetVectorFromAngle(angle));
                angle += angleInterval;
            }
            // Assigning new Arrays
            externalDrawingPolygon = tempExternal;
            internalDrawingPolygon = tempInternal;
        }

        public RectangleF DrawingBounds
        {
            get
            {
                var loc = Bounds.Location;
                var cp = Support.Screen.CameraPoint;
                return new RectangleF(new PointF(loc.X - cp.X, loc.Y - cp.Y), Bounds.Size);
            }
        }

        protected Single GetAngle()
        {
            return Angle + (Single)Math.PI * SpinningMultiplier * Support.Screen.Elapsed.Ticks;
        }

        public void Draw(Graphics g)
        {
            var o = g.Transform;
            var t = g.Transform;
            // Rotating
            t.RotateAt(Angle.ToDegree(), Support.DrawingBounds.GetCenter());
            // Transforming
            g.Transform = t;
            //
            byte alpha = GetOpacityByte();
            g.FillPolygon(Support.Screen.GraphicsSupplier.GetSolidBrush(TeamColor, alpha), externalDrawingPolygon);
            g.DrawPolygon(Support.Screen.GraphicsSupplier.GetPen(alpha), internalDrawingPolygon);
            // Detransforming
            g.Transform = o;
        }

        public void Step(IEnumerable<object> data)
        {
            Angle = GetAngle();
        }
    }
}
