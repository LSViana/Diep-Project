using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Diep.GameEngine.Scenario.MovableBlocks;
using Diep.GameEngine.Scenario.Screens;
using Diep.GameEngine.Shared;

namespace Diep.GameEngine.Scenario.Shoots
{
    public class Drone : Shoot
    {
        public event Action DroneDisposed;
        protected PointF[] externalPolygon;
        protected PointF[] internalPolygon;
        protected const float StandardSurroundAngleMultiplier = .0000005f;
        protected const float SurroundSupportSpeedMultiplier = .6f;
        protected PointF supportCenter;
        protected float angleCorrectionRate = .00005f;
        //protected float angleCorrectionRate = .0005f;
        protected bool dynamicTarget;
        protected PointF lastMouseLocation;
        protected bool followingMouse;
        protected bool runningFromMouse;
        protected Single speed;
        protected bool surroundingSupport;
        protected float surroundAngleMultiplier;

        public Drone() : base()
        {
            // Simplest Constructor
            FrayRate = StandardFrayRate / 15;
            Power *= 1.25f;
            surroundAngleMultiplier = StandardSurroundAngleMultiplier;
        }

        public override RectangleF Bounds { get => base.Bounds; set { base.Bounds = value; CalculateDrawingPolygon(); } }

        public Single SurroundAngleMultiplier
        {
            get { return surroundingSupport ? surroundAngleMultiplier * SurroundSupportSpeedMultiplier : surroundAngleMultiplier; }
            set { surroundAngleMultiplier = value; }
        }

        public Boolean Controllable { get; set; }

        public Single SurroundAngle { get; set; }

        public PointF SurroundTarget { get; private set; }

        public Single SurroundRadius { get; set; } = 2f;

        public Single RadarSize { get; set; } = 600;

        public IMovable Target { get; private set; }

        public override Single Speed
        {
            get { return surroundingSupport ? speed * SurroundSupportSpeedMultiplier : speed; }
            set { speed = value; }
        }

        private void CalculateDrawingPolygon()
        {
            var db = DrawingBounds;
            //
            externalPolygon = new PointF[]
            {
                new PointF(db.X, db.Y),
                new PointF(db.X + db.Width, db.Y + db.Height / 2),
                new PointF(db.X, db.Y + db.Height),
            };
            //
            internalPolygon = new PointF[]
            {
                new PointF(PenWidth / 2 + db.X, PenWidth + db.Y),
                new PointF(-PenWidth + db.X + db.Width, PenWidth / 10 + db.Y + db.Height / 2),
                new PointF(PenWidth / 2 + db.X, -PenWidth + db.Y + db.Height),
            };
        }

        protected override void PerformDraw(Graphics g)
        {
            var db = DrawingBounds;
            // Transforming
            var o = g.Transform;
            var t = g.Transform;
            t.RotateAt(Angle.ToDegree(), db.GetCenter());
            g.Transform = t;
            // Drawing
            var brush = Screen.GraphicsSupplier.GetSolidBrush(Cannon.Support.TeamColor, GetOpacityByte());
            var pen = Screen.GraphicsSupplier.GetPen(GetOpacityByte());
            g.FillPolygon(brush, externalPolygon);
            g.DrawPolygon(pen, internalPolygon);
            // Detransforming
            g.Transform = o;
            // Tests Purpose
            //g.FillRectangle(new SolidBrush(Color.Red), Bounds);
            //g.FillRectangle(new SolidBrush(Color.Red), new RectangleF(SurroundTarget, new SizeF(10, 10)));
            //g.FillRectangle(new SolidBrush(Color.Red), new RectangleF(lastMouseLocation, new SizeF(10, 10)));
        }

        public override void Step(IEnumerable<object> data)
        {
            // Surround the Origin Support
            Keys mouseKeyPressed = GetMouseButtonPressedKeys();
            if (Controllable && mouseKeyPressed != default(Keys))
            {
                foreach (var item in data)
                {
                    if (item is Point mouseLocation)
                    {
                        if ((Screen.MessageFilter.IsPressed(System.Windows.Forms.Keys.LButton) || Screen.MessageFilter.IsPressed(System.Windows.Forms.Keys.E)))
                        {
                            runningFromMouse = false;
                            followingMouse = true;
                            surroundingSupport = false;
                        }
                        else if ((Screen.MessageFilter.IsPressed(System.Windows.Forms.Keys.RButton)))
                        {
                            runningFromMouse = true;
                            followingMouse = false;
                            surroundingSupport = false;
                        }
                        var result = PointF.Empty;
                        var gameBounds = Screen.Controller.Bounds;
                        var mouseHorizontalPercentage = mouseLocation.X / gameBounds.Width;
                        var mouseVerticalPercentage = mouseLocation.Y / gameBounds.Height;
                        var drawArea = Screen.DrawArea;
                        lastMouseLocation.X = drawArea.Width * mouseHorizontalPercentage + drawArea.X;
                        lastMouseLocation.Y = drawArea.Height * mouseVerticalPercentage + drawArea.Y;
                        //
                        dynamicTarget = false;
                    }
                }
            }
            else
            {
                runningFromMouse = false;
                followingMouse = false;
                // Look for a target and follow it
                var boundsCenter = Bounds.GetCenter();
                Target = Screen.Shapes
                    .Where(a => !(a is Stroller))
                    .Select(a => new
                    {
                        Movable = a,
                        Distance = boundsCenter.GetDistance(a.Bounds.GetCenter())
                    })
                    .Where(a => a.Distance < RadarSize)
                    .OrderBy(a => a.Distance)
                    .FirstOrDefault()?
                    .Movable;
                //
                dynamicTarget = !(Target is null);
                surroundingSupport = !dynamicTarget;
                supportCenter = Cannon.Support.Bounds.GetCenter();
            }
            // Base Step implementation, that performs the Move Operation
            base.Step(data);
        }

        private Keys GetMouseButtonPressedKeys()
        {
            if (Screen.MessageFilter.IsPressed(System.Windows.Forms.Keys.LButton))
                return Keys.LButton;
            else if (Screen.MessageFilter.IsPressed(System.Windows.Forms.Keys.E))
                return Keys.E;
            else if (Screen.MessageFilter.IsPressed(Keys.RButton))
                return Keys.RButton;
            else
                return default(Keys);
        }

        protected override void MoveShoot(Vector2D vector, long elapsedTicks)
        {
            var next = Bounds.Location;
            var distance = Speed * elapsedTicks;
            Vector2D targetVector = Vector2D.Empty;
            var boundsCenter = Bounds.GetCenter();
            //
            if (followingMouse)
            {
                SurroundTarget = lastMouseLocation;
            }
            else if (runningFromMouse)
            {
                var target = lastMouseLocation;
                var angle = boundsCenter.GetAngle(target) + (Single)Math.PI;
                SurroundTarget = boundsCenter.Move(SurroundRadius, Extensions.GetVectorFromAngle(angle));
            }
            else if (dynamicTarget)
            {
                SurroundTarget = Target.Bounds.GetCenter();
            }
            else
            {
                var surroundLimit = Cannon.Support.Bounds.Width * SurroundRadius;
                var distanceToTarget = boundsCenter.GetDistance(supportCenter);
                if (distanceToTarget > surroundLimit * 1.5f)
                {
                    surroundingSupport = false;
                    SurroundTarget = supportCenter;
                }
                else
                {
                    surroundingSupport = true;
                    SurroundTarget = supportCenter.Move(surroundLimit, Extensions.GetVectorFromAngle(SurroundAngle));
                }
                //
                SurroundAngle += elapsedTicks * (Single)Math.PI * SurroundAngleMultiplier;
            }
            targetVector = Extensions.GetVectorFromAngle(boundsCenter.GetAngle(SurroundTarget));
            movementVector.X += (targetVector.X - movementVector.X) * angleCorrectionRate;
            movementVector.Y += (targetVector.Y - movementVector.Y) * angleCorrectionRate;
            Angle = Extensions.GetAngleFromVector(movementVector);
            //
            Bounds = new RectangleF(next.Move(distance, movementVector), Bounds.Size);
        }

        public override void Dispose()
        {
            base.Dispose();
            DroneDisposed?.Invoke();
        }
    }
}