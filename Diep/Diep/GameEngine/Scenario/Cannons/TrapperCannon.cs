using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Diep.GameEngine.Scenario.Shoots;
using Diep.GameEngine.Shared;

namespace Diep.GameEngine.Scenario.Cannons
{
    public class TrapperCannon : Cannon
    {
        private PointF[] funnelExternalPolygon;
        private PointF[] funnelInternalPolygon;

        public TrapperCannon()
        {
            // Simplest Constructor
            FunnelRange = .5f;
            FunnelStart = .7f;
        }

        public Single FunnelRange { get; set; }

        public Single FunnelStart { get; set; }

        protected override void CalculateDrawingPolygon()
        {
            if (Support is null)
                return;
            base.CalculateDrawingPolygon();
            // Bringing Center to the Actual Center Point Again
            center.Y += bounds.Height / 2;
            var tankSize = Support.Bounds.Size;
            // External Support
            funnelExternalPolygon = new PointF[]
            {
                new PointF(center.X + (bounds.Width * FunnelStart), center.Y - bounds.Height / 2),
                new PointF(center.X + bounds.Width, center.Y - (bounds.Height * (.5f + FunnelRange))),
                new PointF(center.X + bounds.Width, center.Y + (bounds.Height * (.5f + FunnelRange))),
                new PointF(center.X + (bounds.Width * FunnelStart), center.Y + bounds.Height / 2),
            };
            // Internal Support
            funnelInternalPolygon = new PointF[]
            {
                new PointF(PenWidth / 2 + center.X + (bounds.Width * FunnelStart), PenWidth / 2 + center.Y - bounds.Height / 2),
                new PointF(-PenWidth / 2 + center.X + bounds.Width, PenWidth + center.Y - (bounds.Height * (.5f + FunnelRange))),
                new PointF(-PenWidth / 2 + center.X + bounds.Width, -PenWidth + center.Y + (bounds.Height * (.5f + FunnelRange))),
                new PointF(PenWidth / 2 + center.X + (bounds.Width * FunnelStart), -PenWidth / 2 + center.Y + bounds.Height / 2),
            };
        }

        public override Shoot GetShoot()
        {
            return new Trap()
            {
                Spin = (Single)(Shared.Extensions.Random.NextDouble() * Math.PI)
            };
        }

        protected override SizeF GetShootSize()
        {
            var shootHeight = Bounds.Height;
            var triangleSide = shootHeight * .86f; // 0.86 is equals 1/(1.16), that came from visual tests at PowerPoint
            //
            SizeF shootSize = new SizeF(bounds.Height * (1 + FunnelRange), triangleSide * (1 + FunnelRange));
            return shootSize;
        }

        public override void Draw(Graphics g)
        {
            originalMatrix = g.Transform;
            // Transforming
            transformedMatrix = g.Transform;
            transformedMatrix.RotateAt((Angle).ToDegree(), Support.DrawingBounds.GetCenter());
            g.Transform = transformedMatrix;
            // Drawing
            g.FillPolygon(BackBrush, externalPolygon);
            g.DrawPolygon(BackPen, internalPolygon);
            // Drawing the Support
            g.FillPolygon(BackBrush, funnelExternalPolygon);
            g.DrawPolygon(BackPen, funnelInternalPolygon);
            // Detransforming
            g.Transform = originalMatrix;
        }
    }
}
