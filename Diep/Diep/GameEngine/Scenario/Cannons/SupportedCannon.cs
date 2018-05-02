using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diep.GameEngine.Scenario.Cannons
{
    public class SupportedCannon : Cannon
    {
        private PointF[] supportExternalPolygon;
        private PointF[] supportInternalPolygon;

        public SupportedCannon()
        {
            // Simplest Constructor
            SupportSize = .5f;
        }

        public Single SupportSize { get; set; }

        protected override void CalculateDrawingPolygon()
        {
            if (Support is null)
                return;
            base.CalculateDrawingPolygon();
            // Bringing Center to the Actual Center Point Again
            center.Y += bounds.Height / 2;
            var tankSize = Support.Bounds.Size;
            // External Support
            supportExternalPolygon = new PointF[]
            {
                new PointF(center.X, center.Y - tankSize.Height / 2),
                new PointF(center.X + (bounds.Width * SupportSize), center.Y - bounds.Height / 2),
                new PointF(center.X + (bounds.Width * SupportSize), center.Y + bounds.Height / 2),
                new PointF(center.X, center.Y + tankSize.Height / 2),
            };
            // Internal Support
            supportInternalPolygon = new PointF[]
            {
                new PointF(PenWidth / 2 + center.X, PenWidth / 2 + center.Y - tankSize.Height / 2),
                new PointF(-PenWidth / 2 + center.X + (bounds.Width * SupportSize), PenWidth / 2 + center.Y - bounds.Height / 2),
                new PointF(-PenWidth / 2 + center.X + (bounds.Width * SupportSize), -PenWidth / 2 + center.Y + bounds.Height / 2),
                new PointF(PenWidth / 2 + center.X, -PenWidth / 2 + center.Y + tankSize.Height / 2),
            };
        }

        public override void Draw(Graphics g)
        {
            base.Draw(g);
            // Drawing the Support
            g.FillPolygon(BackBrush, supportExternalPolygon);
            g.DrawPolygon(BackPen, supportInternalPolygon);
        }
    }
}
