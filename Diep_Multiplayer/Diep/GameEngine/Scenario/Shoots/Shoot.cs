using Diep.GameEngine.Scenario.Cannons;
using Diep.GameEngine.Scenario.Screens;
using Diep.GameEngine.Scenario.Skills_Manager;
using Diep.GameEngine.Scenario.Tanks;
using Diep.GameEngine.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Diep.GameEngine.Scenario.Shoots
{
    public class Shoot : IGameObject, IMovable, ITouchable
    {
        protected const Single PenWidth = GameController.PenWidth;
        public const Single PenAlpha = GameController.PenAlpha;
        public const float StandardFrayRate = .0000002f;
        public const float DeathTimeOut = .000001f;
        protected const float MinStability = 12f;
        protected const float MaxStability = 18f;
        protected const float Deceleration = .0001f;
        protected Pen backPen;
        protected SolidBrush backBrush;
        protected Single health;
        protected Single healthBackup;
        protected Color color;
        protected float angle;
        protected Vector2D movementVector;

        public Shoot()
        {
            // Standard Property Values
            Health = 300;
            Power = .075f;
            Active = true;
            backPen = new Pen(Color.FromArgb((Int32)PenAlpha, Color.Black), PenWidth);
            Opacity = 1;
            Stability = MinStability;
        }

        public long Id { get; set; }

        public IToolSupport BaseSupport { get; set; }

        public Vector2D MovementVector { get { return movementVector; } set { movementVector = value; } }

        public SolidBrush BackBrush
        {
            get { return backBrush; }
            set { backBrush = value; }
        }

        public Single FrayRate { get; set; } = StandardFrayRate;

        public Color Color
        {
            get { return color; }
            set { color = value; backBrush.Color = value; }
        }

        public virtual float Angle
        {
            get
            {
                return angle;
            }
            set
            {
                angle = value;
            }
        }

        public float Power { get; set; }

        public float Endurance { get; set; }

        public virtual RectangleF Bounds { get; set; }

        public virtual float Speed { get; set; }

        public GameScreen Screen { get; set; }

        public Cannon Cannon { get; set; }

        public Single Health
        {
            get { return health; }
            set { health = value; healthBackup = value; }
        }


        public RectangleF CollisionBounds
        {
            get
            {
                return new RectangleF(Bounds.X - Bounds.Width / 2, Bounds.Y - Bounds.Height / 2, Bounds.Width * 2, Bounds.Height * 2);
            }
        }

        public bool Active { get; set; }

        public float Opacity { get; set; }

        public float Stability { get; set; }

        public RectangleF DrawingBounds
        {
            get
            {
                var loc = Bounds.Location;
                var cp = Cannon.Support.Screen.CameraPoint;
                return new RectangleF(new PointF(loc.X - cp.X, loc.Y - cp.Y), Bounds.Size);
            }
        }

        public float Impact { get; set; }

        public virtual void Collide(ITouchable other)
        {
            // Avoiding Fire Friend
            if (other is Tank tank)
                if (tank == Cannon.Support)
                    return;
            // Damage to this
            if (other is Shoot shoot && shoot.BaseSupport == BaseSupport)
            {
                // No health injurious, but collision happens normally
            }
            else
            {
                health -= (1 - Endurance) * other.Power * Screen.Elapsed.Ticks;
                if (Health <= 0)
                {
                    Active = false;
                    //Cannon.Tank.Screen.MovableShoots.Remove(this);
                }
            }
            if (Active)
            {
                // Movement to opposite side
                var otherCenter = other.CollisionBounds.GetCenter();
                var collisionAngle = otherCenter.GetAngle(Bounds.GetCenter());
                if (Math.Abs(movementVector.Y) < 1)
                    movementVector.Y += (Single)(Math.Sin(collisionAngle) / Stability) * other.Impact;
                if (Math.Abs(movementVector.X) < 1)
                    movementVector.X += (Single)(Math.Cos(collisionAngle) / Stability) * other.Impact;
            }
        }

        public void Draw(Graphics g)
        {
            // Avoid Drawing when out of Screen
            if (Bounds.IntersectsWith(Screen.DrawArea))
            {
                PerformDraw(g);
            }
        }

        protected virtual void PerformDraw(Graphics g)
        {
            // Drawing Shoot
            var db = DrawingBounds;
            var internalEllipse = db;
            internalEllipse.Inflate(-PenWidth / 2, -PenWidth / 2);
            var opacity = GetOpacityByte();
            var brush = Screen.GraphicsSupplier.GetSolidBrush(Cannon.Support.TeamColor, opacity);
            var pen = Screen.GraphicsSupplier.GetPen(opacity);
            g.FillEllipse(brush, db);
            g.DrawEllipse(pen, internalEllipse);
            // Tests Purpose
            //var c = DrawingBounds.GetCenter();
            //g.DrawString(Health.ToString(), new Font("Segoe UI", 8), new SolidBrush(Color.Black), new PointF(c.X - 10, c.Y - 10));
            //g.DrawLine(new Pen(Color.Red), Bounds.Location, new PointF(Cannon.Tank.Screen.Bounds.Width / 2, Cannon.Tank.Screen.Bounds.Height / 2));
            //g.FillEllipse(new SolidBrush(Color.Pink), new RectangleF(Bounds.Location, new SizeF(3, 3)));
        }

        protected byte GetOpacityByte()
        {
            return (byte)(Opacity * 255);
        }

        public void Move(Vector2D vector)
        {
            // Treating about Transparency
            if (!Active)
            {
                if (Opacity > 0)
                {
                    float delta = DeathTimeOut * Screen.Elapsed.Ticks;
                    Opacity -= delta;
                    var bounds = Bounds;
                    bounds.Inflate(delta * 5, delta * 5);
                    Bounds = bounds;
                    if (Opacity < 0)
                        Opacity = 0;
                }
                else
                {
                    if (Cannon.Support.Screen is PlayScreen PScreen)
                    {
                        Dispose();
                        return;
                    }
                }
            }
            // Moving Shoot
            long elapsedTicks = Cannon.Support.Screen.Elapsed.Ticks;
            MoveShoot(vector, elapsedTicks);
            // Fraying the shoot
            FrayShoot(elapsedTicks);
        }

        protected virtual void FrayShoot(long elapsedTicks)
        {
            if (Active)
            {
                health -= healthBackup * FrayRate * elapsedTicks;
                if (health < 0)
                {
                    Active = false;
                }
            }
        }

        protected virtual void MoveShoot(Vector2D vector, long elapsedTicks)
        {
            // This code avoid Shoot to leave map, but it was removed after Blocks, Strollers and Trappers were allowed to move out of active map area
            //if (Bounds.X < -100 || Bounds.X > Cannon.Support.Screen.Map.Bounds.Width + 100 || Bounds.Y < -100 || Bounds.Y > Cannon.Support.Screen.Map.Bounds.Height + 100)
            //    if (Cannon.Support.Screen is PlayScreen PScreen)
            //        Dispose();
            var next = Bounds.Location;
            var distance = Speed * elapsedTicks;
            Bounds = new RectangleF(next.Move(distance, vector), Bounds.Size);
        }

        public virtual void Dispose()
        {
            if (Screen is PlayScreen PScreen)
                PScreen.RemoveShoot(this);
        }

        public virtual void Step(IEnumerable<object> data)
        {
            Move(movementVector);
        }

        public virtual bool VerifyCollision(ITouchable other)
        {
            if (other is Shoot shoot)
                if (shoot.Cannon.Support == Cannon.Support)
                    return false;
            if (other is Tank tank)
                if (tank == Cannon.Support)
                    return false;
            return CollisionBounds.IntersectsWith(other.CollisionBounds);
        }
    }
}