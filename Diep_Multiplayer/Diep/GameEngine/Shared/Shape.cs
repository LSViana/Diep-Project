using Diep.GameEngine.Scenario.Cannons;
using Diep.GameEngine.Scenario.MovableBlocks;
using Diep.GameEngine.Scenario.Screens;
using Diep.GameEngine.Scenario.Shoots;
using Diep.GameEngine.Shared;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diep.GameEngine.Scenario.Blocks
{
    public abstract class Shape : IGameObject, IMovable, ITouchable, IToolSupport
    {
        public const Single PenWidth = GameController.PenWidth;
        public const Single PenAlpha = GameController.PenAlpha;
        public const float MinStability = 1.5f;
        public const float MaxStability = 2.5f;
        public const float MinImpact = -1;
        public const float MaxImpact = 2;
        public const float StandardImpact = 1;
        public const float DeathTimeOut = .000001f;
        public const float FrayRate = .00001f;
        protected Vector2D movementVector;
        protected float angle;
        protected Boolean colliding;

        public Shape(GameScreen Screen, TeamColor TeamColor)
        {
            this.Screen = Screen;
            // Standard Property Values
            this.TeamColor = TeamColor;
            Active = true;
            Opacity = 1;
            Stability = MinStability;
            Impact = StandardImpact;
        }

        public virtual PointF BoundsCenter { get { return Bounds.GetCenter(); } }
        public Boolean CanLeaveMap { get; set; }
        public virtual RectangleF Bounds { get; set; }
        public float Angle
        {
            get
            {
                return angle;
            }
            set
            {
                angle = value;
                MovementVector = Extensions.GetVectorFromAngle(value);
            }
        }

        public float Power { get; set; }

        public Single Speed { get; set; }

        public GameScreen Screen { get; set; }

        public Single Rotation { get; set; }

        public Single Spin { get; set; }

        public float Health { get; set; }

        public virtual RectangleF CollisionBounds
        {
            get
            {
                return new RectangleF(Bounds.X - Bounds.Width / 5, Bounds.Y - Bounds.Height / 5, Bounds.Width * 1.4f, Bounds.Height * 1.4f);
            }
        }

        public float Stability { get; set; }

        public bool Active { get; set; }

        public float Opacity { get; set; }

        public Vector2D MovementVector { get { return movementVector; } set { movementVector = value; } }

        public RectangleF DrawingBounds
        {
            get
            {
                var loc = Bounds.Location;
                var cp = Screen.CameraPoint;
                return new RectangleF(new PointF(loc.X - cp.X, loc.Y - cp.Y), Bounds.Size);
            }
        }

        public float Impact { get; set; }

        public TeamColor TeamColor { get; set; }
        public Cannon[] Cannons { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public long Id { get; set; }

        public virtual void Collide(ITouchable other)
        {
            if (!(other is Shape))
            {
                // Damage to this
                Health -= other.Power * Screen.Elapsed.Ticks;
                if (Health <= 0)
                {
                    Active = false;
                    //Screen.MovableShapes.Remove(this);
                }
            }
            if (Active)
            {
                // Movement to opposite side
                var otherCenter = other.CollisionBounds.GetCenter();
                var collisionAngle = otherCenter.GetAngle(Bounds.GetCenter());
                if (Math.Abs(movementVector.Y) < 1)
                    movementVector.Y += (Single)(Math.Sin(collisionAngle) / Stability) * ((other is Tracker || other is Stroller) ? other.Impact * 10 : other.Impact);
                if (Math.Abs(movementVector.X) < 1)
                    movementVector.X += (Single)(Math.Cos(collisionAngle) / Stability) * ((other is Tracker || other is Stroller) ? other.Impact * 10 : other.Impact);
            }
        }

        public virtual void Draw(Graphics g)
        {
            // Tests Purpose
            //g.DrawLine(new Pen(Color.Red), Bounds.Location, new PointF(Screen.Bounds.Width / 2, Screen.Bounds.Height / 2));
            //g.FillRectangle(new SolidBrush(Color.Red), CollisionBounds);
            //g.DrawString(movementVector.X.ToString(), new Font("Segoe UI", 8), new SolidBrush(Color.Black), new RectangleF(Bounds.X - 10, Bounds.Y - 10, Bounds.Size.Width, Bounds.Size.Height));
        }

        public virtual void Move(Vector2D vector)
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
                }
                else
                {
                    if (Screen is PlayScreen PScreen)
                    {
                        PScreen.RemoveShape(this);
                        return;
                    }
                }
            }
            // Limitating At Borders
            if (!CanLeaveMap)
            {
                if (Bounds.X < -Screen.Map.MapOutLimit)
                {
                    if (vector.X < 0)
                    {
                        movementVector.X *= -1;
                        vector.X *= -1;
                    }
                }
                else if (Bounds.X > Screen.Map.MapOutLimit + Screen.Map.Bounds.Width - Bounds.Width)
                {
                    if (vector.X > 0)
                    {
                        movementVector.X *= -1;
                        vector.X *= -1;
                    }
                }
                if (Bounds.Y < -Screen.Map.MapOutLimit)
                {
                    if (vector.Y < 0)
                    {
                        movementVector.Y *= -1;
                        vector.Y *= -1;
                    }
                }
                else if (Bounds.Y > Screen.Map.MapOutLimit + Screen.Map.Bounds.Height - Bounds.Height)
                {
                    if (vector.Y > 0)
                    {
                        movementVector.Y *= -1;
                        vector.Y *= -1;
                    }
                }
            }
            // Fraying Movement
            if (Math.Abs(movementVector.X) > 1)
            {
                movementVector.X *= 1 - FrayRate;
            }
            if (Math.Abs(movementVector.Y) > 1)
            {
                movementVector.Y *= 1 - FrayRate;
            }
            // Moving Block
            var next = Bounds.Location;
            var distance = Speed * Screen.Elapsed.Ticks;
            Bounds = new RectangleF(next.Move(distance, vector), Bounds.Size);
            // Rotating
            Spin += Rotation;
        }

        public virtual Byte GetOpacityByte()
        {
            return (byte)(Opacity * Byte.MaxValue);
        }

        public virtual void Step(IEnumerable<object> data)
        {
            Move(MovementVector);
        }

        public abstract bool VerifyCollision(ITouchable other);
    }
}
