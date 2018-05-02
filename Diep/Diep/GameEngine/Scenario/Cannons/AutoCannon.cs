using Diep.GameEngine.Scenario.MovableBlocks;
using Diep.GameEngine.Scenario.Screens;
using Diep.GameEngine.Shared;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diep.GameEngine.Scenario.Cannons
{
    public class AutoCannon : Cannon
    {
        protected const float angleCorrectionRate = 10;
        protected float AimCorrectionRate = .000007f;
        protected SizeF ellipseSize;
        private PointF ellipseCenter;
        protected RectangleF ellipseBounds;
        private RectangleF internalEllipseBounds;
        protected IMovable nearestMovable;
        private PointF ellipseDrawingCenter;

        public AutoCannon()
        {
            // Standard Property Values
            Controllable = true;
        }

        public override RectangleF Bounds
        {
            get { return base.Bounds; }
            set { base.Bounds = value; RecalculateEllipse(); }
        }

        public Single RangeLimit { get; set; } = .5f;

        public Single BarrelAngle { get; set; }

        public override float Angle { get => base.Angle; set { base.Angle = value; } }

        public bool Controllable { get; set; }

        public float TargetBarrelAngle { get; set; }

        private void RecalculateEllipse()
        {
            var size = Bounds.Height * 1.6f;
            ellipseSize = new SizeF(size, size);
            ellipseCenter = center;
            ellipseCenter.Y += bounds.Height / 2 - size / 2;
            ellipseBounds = new RectangleF(ellipseCenter, ellipseSize);
            ellipseCenter.X += ellipseSize.Width / 2;
            ellipseCenter.Y += ellipseSize.Height / 2;
            internalEllipseBounds = ellipseBounds;
            internalEllipseBounds.Inflate(-PenWidth / 2, -PenWidth / 2);
        }

        public override void Draw(Graphics g)
        {
            originalMatrix = g.Transform;
            // Transforming
            transformedMatrix = g.Transform;
            // Tank Angle Rotation
            transformedMatrix.RotateAt((Angle).ToDegree(), Support.DrawingBounds.GetCenter());
            g.Transform = transformedMatrix;
            // Barrel Rotation
            transformedMatrix.RotateAt((BarrelAngle).ToDegree(), ellipseCenter);
            g.Transform = transformedMatrix;
            // Drawing Barrel
            g.FillPolygon(BackBrush, externalPolygon);
            g.DrawPolygon(BackPen, internalPolygon);
            // Drawing Ellipse
            g.FillEllipse(BackBrush, ellipseBounds);
            g.DrawEllipse(BackPen, internalEllipseBounds);
            //if (nearestMovable != null)
            //    g.DrawString((BarrelAngle.ToDegree()).ToString(), new Font("Segoe UI", 12), new SolidBrush(Color.Black), new PointF(externalPolygon[0].X + 50, externalPolygon[0].Y));
            // Detransforming
            g.Transform = originalMatrix;
            // Tests Purpose
            //if (nearestMovable != null)
            //{
            //    g.FillRectangle(new SolidBrush(Color.White), new RectangleF(ellipseDrawingCenter, new SizeF(3, 3)));
            //    g.DrawLine(new Pen(Color.Red), ellipseDrawingCenter, nearestMovable.DrawingBounds.GetCenter());
            //}
        }

        protected override void CalculateDrawingPolygon()
        {
            base.CalculateDrawingPolygon();
            // Reposition points to not draw cannon out of base circle area
            externalPolygon[0].X += ellipseSize.Width / 2;
            externalPolygon[3].X += ellipseSize.Width / 2;
            //
            internalPolygon[0].X += ellipseSize.Width / 2;
            internalPolygon[3].X += ellipseSize.Width / 2;
        }

        protected float GetBarrelTankAngle()
        {
            //if (Math.Sin(Tank.Angle) <= 0)
            //    return Tank.Angle - (Single)Math.PI * 2;
            //    //return Tank.Angle;
            //else
            return Support.Angle;
        }

        protected override PointF GetShootLocation(float angle, ref SizeF shootSize)
        {
            var baseLocation = base.GetShootLocation(angle, ref shootSize);
            //
            return baseLocation;
        }

        protected override float GetShootAngle()
        {
            return Support.Angle + Angle + BarrelAngle;
        }

        public override void Step(IEnumerable<object> data)
        {
            // Getting Nearer to Target Angle
            var targetVector = Extensions.GetVectorFromAngle(TargetBarrelAngle);
            var currentVector = Extensions.GetVectorFromAngle(BarrelAngle);
            var elapsed = Support.Screen.Elapsed.Ticks;
            var delta = new Vector2D(currentVector.X + (targetVector.X - currentVector.X) * AimCorrectionRate * elapsed, currentVector.Y + (targetVector.Y - currentVector.Y) * AimCorrectionRate * elapsed);
            BarrelAngle = Extensions.GetAngleFromVector(delta);
            //
            if (nearestMovable != null || AutoShoot)
            {
                Shoot();
            }
            UpdateVisualRecoil();
            // Treating Manual (Mouse) Aim
            if (Controllable && Support.Screen.MessageFilter.IsPressed(System.Windows.Forms.Keys.LButton))
            {
                foreach (var item in data)
                {
                    if (item is Point mouseLocation)
                    {
                        PointF boundsCenter = Support.Bounds.GetCenter();
                        if (Support.Screen is PlayScreen PScreen)
                        {
                            mouseLocation.Offset((int)PScreen.CameraPoint.X, (int)PScreen.CameraPoint.Y);
                        }
                        var angle = boundsCenter.GetAngle(mouseLocation);
                        angle -= Angle + Support.Angle;
                        if (Math.Cos(angle) > RangeLimit)
                        {
                            TargetBarrelAngle = angle;
                            return;
                        }
                    }
                }
            }
            //
            ellipseDrawingCenter = Support.DrawingBounds.GetCenter();
            ellipseDrawingCenter = ellipseDrawingCenter.Move(Support.DrawingBounds.Width / 2, Extensions.GetVectorFromAngle(Angle));
            //
            nearestMovable = Support.Screen.MovableShapes
                .Where(a => !(a is Stroller))
                .Where(a => ValidateAngle(a))
                .OrderBy(a => ellipseDrawingCenter.GetDistance(a.Bounds.GetCenter()))
                .FirstOrDefault();
            if (nearestMovable is null)
            {
                TargetBarrelAngle = 0f;
            }
            else
            {
                TargetBarrelAngle = GetAngle(nearestMovable);
            }
        }

        private Boolean ValidateAngle(IMovable movable)
        {
            var angle = GetAngle(movable);
            return Math.Cos(angle) >= RangeLimit;
        }

        private float GetAngle(IMovable movable)
        {
            return ellipseDrawingCenter.GetAngle(movable.DrawingBounds.GetCenter()) - Angle - GetBarrelTankAngle();
        }
    }
}
