using Diep.GameEngine.Scenario.Shoots;
using Diep.GameEngine.Scenario.Skills_Manager;
using Diep.GameEngine.Scenario.Tanks;
using Diep.GameEngine.Shared;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Diep.GameEngine.Scenario.Cannons
{
    public class Cannon : IMovable, IShootable, IGameObject
    {
        public event EventHandler Shooting;

        public const Single PenWidth = GameController.PenWidth;
        public const float PenAlpha = GameController.PenAlpha;
        public const float GrowthRate = Tank.GrowthRate;
        public const float MinShootSpread = .03f;
        public const float MaxShootSpread = .12f;
        public const float MinRecoil = .8f;
        public const float MaxRecoil = 1f;
        protected DateTime lastShootTime;
        protected Single recoil;
        protected RectangleF bounds;
        protected bool reducingRecoil;
        protected PointF[] externalPolygon;
        protected PointF[] internalPolygon;
        protected PointF center;
        protected Matrix originalMatrix;
        protected Matrix transformedMatrix;
        protected Single obliquity;
        protected SizeF tankPosition;
        protected RectangleF lastSupportDb;

        public Cannon()
        {
            // Standard Property Values
            TeamColor = TeamColor.DarkGray;
            lastShootTime = DateTime.Now;
            SkillsManager = new SkillsManager();
            recoil = MaxRecoil;
        }

        public Single Power
        {
            get { return SkillsManager.Power; }
            set { SkillsManager.Power = value; }
        }

        public Single VisualRecoil
        {
            get { return recoil; }
            set { if (value > MaxRecoil) value = MaxRecoil; else if (value < MinRecoil) value = MinRecoil; recoil = value; }
        }

        public virtual SolidBrush BackBrush => Support.Screen.GraphicsSupplier.GetSolidBrush(TeamColor, GetOpacityByte());

        public virtual Pen BackPen => Support.Screen.GraphicsSupplier.GetPen(GetOpacityByte());

        public Single Opacity { get { return Support.Opacity; } set { Support.Opacity = value; } }

        public TeamColor TeamColor { get; set; }

        public virtual Boolean AutoShoot { get; set; }

        public float ShootSpread { get; set; } = MinShootSpread;

        public Single ShootRate { get { return SkillsManager.ShootRate; } set { SkillsManager.ShootRate = value; } }

        public Single Speed { get { return SkillsManager.ShootSpeed; } set { SkillsManager.ShootSpeed = value; } }

        public SkillsManager SkillsManager { get; set; }

        public virtual float Angle { get; set; }

        public virtual RectangleF Bounds
        {
            get { return bounds; }
            set { bounds = value; CalculateDrawingPolygon(); }
        }

        public Single Obliquity
        {
            get { return obliquity; }
            set { obliquity = value; CalculateDrawingPolygon(); }
        }

        public IToolSupport Support { get; set; }

        public SizeF TankPosition
        {
            get { return tankPosition; }
            set { tankPosition = value; CalculateDrawingPolygon(); }
        }

        public RectangleF DrawingBounds
        {
            get { throw new InvalidOperationException(); }
        }

        public virtual PointF[] DrawingPolygon
        {
            get
            {
                return externalPolygon;
            }
        }

        public Vector2D MovementVector { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool Stereo { get; set; }

        protected virtual void CalculateDrawingPolygon()
        {
            if (Support is null)
                return;
            lastSupportDb = Support.DrawingBounds;
            if (Support is Tank tank && tank.UserTank)
            {
                center = lastSupportDb.Location;
            }
            else
            {
                center = Bounds.Location;
            }
            center.X += TankPosition.Width * lastSupportDb.Width;
            center.Y += TankPosition.Height * lastSupportDb.Height - Bounds.Height / 2;
            //
            var obliquityDivergence = (Obliquity * .5f * Bounds.Height);
            if (Obliquity >= 0)
            {
                // External Polygon
                externalPolygon = new PointF[]
                {
                    new PointF(center.X, center.Y + obliquityDivergence),
                    new PointF(center.X + (Bounds.Width * VisualRecoil), center.Y),
                    new PointF(center.X + (Bounds.Width * VisualRecoil), center.Y + Bounds.Height),
                    new PointF(center.X, center.Y + Bounds.Height - obliquityDivergence)
                };
                // Internal Polygon
                internalPolygon = new PointF[]
                {
                    new PointF(center.X + PenWidth / 2, center.Y + obliquityDivergence + PenWidth / 2),
                    new PointF(center.X + (Bounds.Width * VisualRecoil) - PenWidth / 2, center.Y + PenWidth / 2),
                    new PointF(center.X + (Bounds.Width * VisualRecoil) - PenWidth / 2, center.Y + Bounds.Height - PenWidth / 2),
                    new PointF(center.X + PenWidth / 2, center.Y + Bounds.Height - obliquityDivergence - PenWidth / 2)
                };
            }
            else
            {
                // External Polygon
                externalPolygon = new PointF[]
                {
                    new PointF(center.X, center.Y),
                    new PointF(center.X + (Bounds.Width * VisualRecoil), center.Y - obliquityDivergence),
                    new PointF(center.X + (Bounds.Width * VisualRecoil), center.Y + Bounds.Height + obliquityDivergence),
                    new PointF(center.X, center.Y + Bounds.Height)
                };
                // Internal Polygon
                internalPolygon = new PointF[]
                {
                    new PointF(center.X + PenWidth / 2, center.Y + PenWidth / 2),
                    new PointF(center.X + (Bounds.Width * VisualRecoil) - PenWidth / 2, center.Y - obliquityDivergence + PenWidth / 2),
                    new PointF(center.X + (Bounds.Width * VisualRecoil) - PenWidth / 2, center.Y + Bounds.Height + obliquityDivergence - PenWidth / 2),
                    new PointF(center.X + PenWidth / 2, center.Y + Bounds.Height - PenWidth / 2)
                };
            }
        }

        public virtual Byte GetOpacityByte()
        {
            return (byte)(Opacity * Byte.MaxValue);
        }

        public virtual void Draw(Graphics g)
        {
            originalMatrix = g.Transform;
            // Transforming
            transformedMatrix = g.Transform;
            transformedMatrix.RotateAt((Angle).ToDegree(), Support.DrawingBounds.GetCenter());
            g.Transform = transformedMatrix;
            // Drawing
            g.FillPolygon(BackBrush, externalPolygon);
            g.DrawPolygon(BackPen, internalPolygon);
            // Detransforming
            g.Transform = originalMatrix;
            // Tests Purpose
            //g.FillRectangle(new SolidBrush(Color.Orange), new RectangleF(Bounds.Location, new SizeF(3, 3)));
        }

        public void RecalculateSize()
        {
            var bounds = Bounds;
            bounds.Inflate(Bounds.Width * GrowthRate, Bounds.Height * GrowthRate);
            var tankCenter = Support.Bounds.GetCenter();
            bounds.Y = tankCenter.Y - bounds.Height / 2;
            bounds.X = tankCenter.X;
            Bounds = bounds;
        }

        public virtual void Move(Vector2D vector)
        {
            Support?.Move(vector);
        }

        public virtual void Shoot()
        {
            if (!VerifyShootAllowed())
                return;
            Shooting?.Invoke(this, EventArgs.Empty);
            var shoot = GetReadyShoot();
            // Pull the ship the inverse direction
            PullSupport(shoot.Bounds.Size, Extensions.GetVectorFromAngle(shoot.Angle));
            // Adding to Current Screen
            Support?.Screen?.MovableShoots.Add(shoot);
            // Recoil to the Cannon
            reducingRecoil = true;
            // Incrementing Shoot Step of Support
            if (Support is Tank tank)
            {
                tank.ShootStep++;
                // Tests Purpose, it was used to up levels quickly
                tank.Score += 1000;
            }
        }

        public Shoot GetReadyShoot()
        {
            var shoot = GetShoot();
            // If support is Tank, set it as BaseSupport
            if (Support is Tank tankSupport)
                shoot.BaseSupport = tankSupport;
            //
            var angle = GetShootAngle();
            var tankBounds = Support.Bounds;
            shoot.Endurance = SkillsManager.Endurance;
            shoot.Cannon = this;
            shoot.Screen = Support.Screen;
            var shootSize = GetShootSize();
            var shootLocation = GetShootLocation(angle, ref shootSize);
            // Centering Shoot at Supplied Location
            shootLocation.X -= shootSize.Width / 2;
            shootLocation.Y -= shootSize.Height / 2;
            // Defining Movement Vector
            var vector = Extensions.GetVectorFromAngle(angle);
            var shootBounds = new RectangleF(shootLocation, shootSize);
            shootBounds.Inflate(-PenWidth / 2f, -PenWidth / 2f);
            shoot.Bounds = shootBounds;
            //shoot.Speed = 10;
            SetShootProperties(shoot);
            shoot.BackBrush = Support.Screen.GraphicsSupplier.GetSolidBrush(Support.TeamColor, 255);
            //
            return shoot;
        }

        public virtual void PullSupport(SizeF shootSize, Vector2D vector)
        {
            var supportMV = Support.MovementVector;
            supportMV.X -= (vector.X * shootSize.Height / 20) / Support.Stability * (1 + Power) * SkillsManager.ShootImpact;
            if (Math.Abs(supportMV.X) > 1)
                supportMV.X /= Math.Abs(supportMV.X);
            supportMV.Y -= (vector.Y * shootSize.Height / 20) / Support.Stability * (1 + Power) * SkillsManager.ShootImpact;
            if (Math.Abs(supportMV.Y) > 1)
                supportMV.Y /= Math.Abs(supportMV.Y);
            Support.MovementVector = supportMV;
        }

        protected virtual bool VerifyShootAllowed()
        {
            var now = DateTime.Now;
            var result = !Stereo && (now - lastShootTime).TotalMilliseconds > SkillsManager.MillisecondsPerShoot;
            if (result)
                lastShootTime = now;
            return result;
        }

        protected virtual void SetShootProperties(Shoot shoot)
        {
            shoot.Speed = SkillsManager.ShootSpeed;
            shoot.Impact = SkillsManager.ShootImpact;
            shoot.Angle = GetShootAngle() + (((Single)Math.PI * 2 * ShootSpread / 2) - (Single)Extensions.Random.NextDouble() * ((Single)Math.PI * 2 * ShootSpread));
            shoot.MovementVector = Extensions.GetVectorFromAngle(shoot.Angle);
            shoot.Power *= 1 + Power;
        }

        protected virtual SizeF GetShootSize()
        {
            SizeF shootSize;
            var shootDimension = Bounds.Height;
            if (Obliquity < 0)
            {
                shootDimension *= 1 + Obliquity;
            }
            shootSize = new SizeF(shootDimension, shootDimension);
            return shootSize;
        }

        protected virtual float GetShootAngle()
        {
            return Angle + Support.Angle;
        }

        protected virtual PointF GetShootLocation(float angle, ref SizeF shootSize)
        {
            var tankBounds = Support.Bounds;
            var shootLocation = Support.Bounds.GetCenter();
            // Moving inside Tank
            shootLocation.X += (Single)(Math.Sin(angle) * tankBounds.Width * (.5 - TankPosition.Height));
            shootLocation.Y += (Single)(-Math.Cos(angle) * (.5 - TankPosition.Height) * tankBounds.Height);
            // Moving according to Angle
            shootLocation = shootLocation.Move(Bounds.Width, Extensions.GetVectorFromAngle(angle));
            return shootLocation;
        }

        public void UpdateVisualRecoil()
        {
            if (VisualRecoil == MinRecoil)
                reducingRecoil = false;
            VisualRecoil += (reducingRecoil ? -.0000005f : .0000001f) * Support.Screen.Elapsed.Ticks;
            //
            if (VisualRecoil != 1)
            {
                if (Obliquity >= 0)
                {
                    // External Polygon
                    externalPolygon[1].X = center.X + (Bounds.Width * VisualRecoil);
                    externalPolygon[2].X = center.X + (Bounds.Width * VisualRecoil);
                    // Internal Polygon
                    internalPolygon[1].X = center.X + (Bounds.Width * VisualRecoil) - PenWidth / 2;
                    internalPolygon[2].X = center.X + (Bounds.Width * VisualRecoil) - PenWidth / 2;
                }
                else
                {
                    var obliquityDivergence = (Obliquity * .5f * Bounds.Height);
                    // External Polygon
                    externalPolygon[1].X = center.X + (Bounds.Width * VisualRecoil);
                    externalPolygon[2].X = center.X + (Bounds.Width * VisualRecoil);
                    // Internal Polygon
                    internalPolygon[1].X = center.X + (Bounds.Width * VisualRecoil) - PenWidth / 2;
                    internalPolygon[2].X = center.X + (Bounds.Width * VisualRecoil) - PenWidth / 2;
                }
            }
        }

        public virtual Shoot GetShoot()
        {
            return new Shoot();
        }

        public bool VerifyCollision(IMovable other)
        {
            return false;
        }

        public void Collide(IMovable other)
        {
            throw new NotImplementedException();
        }

        public virtual void Step(IEnumerable<object> data)
        {
            if (AutoShoot)
            {
                Shoot();
            }
            UpdateVisualRecoil();
        }
    }
}
