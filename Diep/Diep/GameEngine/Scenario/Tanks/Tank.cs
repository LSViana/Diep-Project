using Diep.GameEngine.Scenario.Cannons;
using Diep.GameEngine.Scenario.Screens;
using Diep.GameEngine.Scenario.Shields;
using Diep.GameEngine.Scenario.Shoots;
using Diep.GameEngine.Scenario.Skills_Manager;
using Diep.GameEngine.Shared;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Diep.GameEngine.Scenario.Tanks
{
    public class Tank : IGameObject, IMovable, ITouchable, IToolSupport
    {
        public const float PenWidth = GameController.PenWidth;
        public const float PenAlpha = GameController.PenAlpha;
        public const int MinLevel = 1;
        public const int MaxLevel = 45;
        public const float GrowthRate = .005f;
        public const float EvolutionRate = 1.25f;
        public const float MinSpeed = .00008f;
        public const float MaxSpeed = .00012f;
        public const float StandardSpeed = .00010f;
        public const float DeathTimeOut = .08f;
        public const float MinAcceleration = .0000005f;
        public const float MaxAcceleration = .00000075f;
        public const float MinHealthRegenRate = .0001f;
        public const float MaxHealthRegenRate = .0004f;
        public const float MinHealth = 300;
        public const float MaxHealth = 1000;
        public const float MinPower = 30;
        public const float MaxPower = 60;
        public const float MinStability = 8f;
        public const float MaxStability = 32f;
        public const float MinImpact = -1;
        public const float MaxImpact = 2;
        public const float StandardImpact = 1;
        public const int EvolutionSteps = 10;
        public const float ZoomMultiplier = .00001f;
        public const Int32 StandardWidth = 60;
        public const Int32 StandardHeight = 60;
        protected Single speed;
        protected float Deceleration = MinAcceleration / 3;
        protected DateTime lastSkillChangedTime;
        protected float HealthRegen;
        protected float angle;
        protected Int32 ShootSteps = 1;
        private Keys[] keyboardTriggers = new Keys[]
        {
            Keys.W, Keys.Up,
            Keys.A, Keys.Left,
            Keys.S, Keys.Down,
            Keys.D, Keys.Right
        };
        public float Acceleration { get; protected set; } = MinAcceleration;

        private Vector2D movementVector;
        private Single health;
        private Single healthBackup;
        protected RectangleF bounds;

        public Single MaximumHealth { get; set; }

        private int shootStep;

        public int ShootStep
        {
            get { return shootStep; }
            set { shootStep = value; }
        }


        public Single Health
        {
            get { return health; }
            set { health = value; healthBackup = value; }
        }

        public Single ZoomFactor { get; set; }

        public Vector2D MovementVector
        {
            get { return movementVector; }
            set { movementVector = value; }
        }

        public Boolean UserTank { get; set; }

        public Tank(GameScreen Screen, TeamColor TeamColor, Single Weight)
        {
            this.Screen = Screen;
            this.Weight = Weight;
            // Standard Property Values
            level = 1;
            KeyCooldown = DateTime.Now;
            lastSkillChangedTime = DateTime.Now;
            this.TeamColor = TeamColor;
            Power = MinPower;
            Active = true;
            movementVector.X = (Single)(Extensions.Random.NextDouble() * .15);
            movementVector.Y = (Single)(Extensions.Random.NextDouble() * .15);
            Health = MinHealth;
            MaximumHealth = Health;
            HealthRegen = MinHealthRegenRate;
            Stability = MinStability;
            Impact = StandardImpact;
            Speed = MaxSpeed;
            Opacity = 1;
        }

        public virtual void SetCannons()
        {
            // Creating Cannons
            Cannon cannon = new Cannon();
            // Defining Cannons
            var Cannons = new Cannon[]
            {
                cannon
            };
            // Positioning Cannons
            var cannonBounds = Bounds;
            cannon.TankPosition = new SizeF(.5f, .5f);
            cannon.Support = this;
            cannonBounds.Height *= .5f;
            cannonBounds.X += cannonBounds.Width / 2;
            cannon.Bounds = cannonBounds;
            this.Cannons = Cannons;
        }

        private Single score;

        public Single Score
        {
            get { return score; }
            set
            {
                score = value; if (score >= level * 100)
                {
                    // Setting Score and Level
                    if (level < 45)
                    {
                        score %= level * 100;
                        level++;
                        AvailableScore++;
                        Screen.ZoomFactor += -(level / 10000f);
                        // Resizing
                        RecalculateSize();
                    }
                }
            }
        }

        public Single LevelGrowth { get { return (Single)Math.Pow(GrowthRate, Level); } }

        public void RecalculateSize()
        {
            // Improve Tank and Cannon size with 2 percent at each new level
            var bounds = Bounds;
            bounds.Inflate(Bounds.Width * GrowthRate, Bounds.Height * GrowthRate);
            Bounds = bounds;
            // Improving Cannon Levels
            if (Cannons != null)
            {
                foreach (var cannon in Cannons)
                {
                    cannon.RecalculateSize();
                }
            }
        }

        public virtual Keys[] KeyboardTriggers
        {
            get
            {
                return keyboardTriggers;
            }
        }

        protected Int32 level;
        protected int lastCannonIndex;
        protected bool autoShoot;

        public Int32 Level
        {
            get { return level; }
            set { if (level < MinLevel) value = MinLevel; else if (level > MaxLevel) value = MaxLevel; level = value; }
        }

        public Int32 AvailableScore { get; set; }

        public RectangleF Bounds
        {
            get { return bounds; }
            set
            {
                bounds = value;
                // Temporarily Removed for Performance Issues
                //RepositionCannons(value);
            }
        }

        public virtual void RepositionCannons(RectangleF value)
        {
            if (Cannons != null)
            {
                foreach (var cannon in Cannons)
                {
                    cannon.Support = this;
                    cannon.Bounds = new RectangleF(new PointF(cannon.Bounds.X + value.X - bounds.X, cannon.Bounds.Y + value.Y - bounds.Y), cannon.Bounds.Size);
                }
            }
        }

        public GameScreen Screen { get; set; }

        public DateTime KeyCooldown { get; set; }

        public float Stability { get; set; }

        public virtual Cannon[] Cannons { get; set; }

        public virtual Shield[] Shields { get; set; }

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

        public float Weight { get; set; }

        public float Power { get; set; }

        public Boolean AutoShoot
        {
            set
            {
                autoShoot = value;
                if (Cannons is null)
                    return;
                foreach (var cannon in Cannons)
                {
                    cannon.AutoShoot = value;
                }
            }
            get { return autoShoot; }
        }

        public Single Speed
        {
            get { return speed; }
            set { if (value > MaxSpeed) value = MaxSpeed; else if (value < MinSpeed) value = MinSpeed; speed = value; }
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

        public RectangleF DrawingBounds
        {
            get
            {
                if (UserTank)
                {
                    var center = Screen.Bounds.GetCenter();
                    return new RectangleF(new PointF(center.X - Bounds.Width / 2, center.Y - Bounds.Height / 2), Bounds.Size);
                }
                else
                {
                    return Bounds;
                }
            }
        }

        public float Impact { get; set; }

        public TeamColor TeamColor { get; set; } = TeamColor.DeepSkyBlue;

        public void Draw(Graphics g)
        {
            if (Bounds.IntersectsWith(Screen.DrawArea))
            {
                PerformDraw(g);
            }
        }

        protected virtual void PerformDraw(Graphics g)
        {
            var db = DrawingBounds;
            // Getting Transform Matix
            var o = g.Transform;
            var t = g.Transform;
            // Drawing Before Rotate
            DrawUnderToolsUnrotated(g);
            // Getting Angle of Rotation
            float degreeAngle = Angle.ToDegree();
            // Applying Rotation
            t.RotateAt(degreeAngle, db.GetCenter());
            // Applying Transform to Graphics
            g.Transform = t;
            // Drawing Under Tools
            DrawUnderTools(g);
            // Drawing Tank Area
            DrawTank(g, db);
            // Drawing Over Tools
            DrawOverTools(g);
            // Derotating
            g.Transform = o;
            // Tests Purpose
            //g.DrawString($"{Score} - {Level}", new Font("Segoe UI", 8), new SolidBrush(Color.Black), Bounds.GetCenter());
            //g.DrawString($"{Health}", new Font("Segoe UI", 8), new SolidBrush(Color.Black), DrawingBounds.GetCenter());
            //g.FillEllipse(new SolidBrush(Color.Red), Bounds);
            if (health < 0) g.FillEllipse(new SolidBrush(Color.FromArgb(128, Color.Red)), DrawingBounds);
        }

        protected virtual void DrawUnderToolsUnrotated(Graphics g)
        {
            // Standard Implementation
        }

        protected virtual void DrawOverTools(Graphics g)
        {
            // Standard Implementation
        }

        private void DrawTank(Graphics g, RectangleF db)
        {
            byte alpha = GetOpacityByte();
            g.FillEllipse(Screen.GraphicsSupplier.GetSolidBrush(TeamColor, alpha), db);
            var penBounds = db;
            penBounds.Inflate(-GameController.PenWidth / 2, -GameController.PenWidth / 2);
            g.DrawEllipse(Screen.GraphicsSupplier.GetPen(alpha), penBounds);
        }

        private Byte GetOpacityByte()
        {
            return (byte)(Opacity * 255);
        }

        protected virtual void DrawUnderTools(Graphics g)
        {
            if (Cannons != null)
                foreach (var cannon in Cannons)
                {
                    cannon.Draw(g);
                }
        }

        public void Move(Vector2D vector)
        {
            // Treating about Transparency
            if (!Active)
            {
                if (Opacity > 0)
                {
                    Opacity -= DeathTimeOut;
                    var bounds = Bounds;
                    bounds.Inflate(DeathTimeOut * 5, DeathTimeOut * 5);
                    Bounds = bounds;
                }
                else
                {
                    if (Screen is PlayScreen PScreen)
                    {
                        PScreen.MovableShoots.Remove(this);
                        return;
                    }
                }
            }
            // Health Regen
            else
            {
                if (health < MaximumHealth)
                {
                    health += HealthRegen;
                    if (health > MaximumHealth)
                        health = MaximumHealth;
                }
            }
            // Limitating At Borders
            if (Bounds.X < -Screen.Map.MapOutLimit)
            {
                if (vector.X < 0)
                    vector.X = 0;
            }
            else if (Bounds.X > Screen.Map.Bounds.Width + Screen.Map.MapOutLimit - Bounds.Width)
            {
                if (vector.X > 0)
                    vector.X = 0;
            }
            if (Bounds.Y < -Screen.Map.MapOutLimit)
            {
                if (vector.Y < 0)
                    vector.Y = 0;
            }
            else if (Bounds.Y > Screen.Map.Bounds.Height + Screen.Map.MapOutLimit - Bounds.Height)
            {
                if (vector.Y > 0)
                    vector.Y = 0;
            }
            // Moving Tank
            long elapsed = Screen.Elapsed.Ticks;
            var distance = Speed * elapsed;
            PointF next = Bounds.Location;
            Bounds = new RectangleF(next.Move(distance, vector), Bounds.Size);
            // Fraying the Movement
            if (movementVector.X > 0)
                movementVector.X *= 1 - Deceleration * elapsed;
            else
                movementVector.X *= 1 - Deceleration * elapsed;
            if (movementVector.Y > 0)
                movementVector.Y *= 1 - Deceleration * elapsed;
            else
                movementVector.Y *= 1 - Deceleration * elapsed;
        }

        public void Step(IEnumerable<object> data)
        {
            // Verifying Auto Behaviours
            if(Cannons != null)
                foreach (var cannon in Cannons)
                {
                    cannon.Step(data);
                }
            if(Shields != null)
                foreach (var shield in Shields)
                {
                    shield.Step(data);
                }
            // Auto-Shoot
            Move(movementVector);
            // Verifying User Commands
            foreach (var item in data)
            {
                if (item is Point mouseLocation)
                {
                    Angle = GetAngle(mouseLocation);
                }
                // Zoom Factor
                if (Screen.MessageFilter.IsPressed(Keys.PageUp))
                {
                    Screen.ZoomFactor += ZoomMultiplier;
                }
                else if (Screen.MessageFilter.IsPressed(Keys.PageDown))
                {
                    Screen.ZoomFactor -= ZoomMultiplier;
                }
                // Keys always active
                if (KeyboardTriggers.Any(a => Screen.MessageFilter.IsPressed(a)))
                {
                    IncrementVector(KeyboardTriggers.Where(a => Screen.MessageFilter.IsPressed(a)));
                }
                // Keys with Cooldown active
                var now = DateTime.Now;
                if ((now - KeyCooldown).TotalMilliseconds <= 100)
                    continue;
                if (Screen.MessageFilter.IsPressed(Keys.LButton) || Screen.MessageFilter.IsPressed(Keys.Space))
                {
                    ExecuteShoot();
                }
                else if (Screen.MessageFilter.IsPressed(Keys.E))
                {
                    AutoShoot ^= true;
                }
                else if (SkillButtonPressed())
                {
                    if (AvailableScore > 0)
                    {
                        var skills = SkillButtonsPressed().ToArray();
                        foreach (var skill in skills)
                        {
                            ImproveSkill((SkillKey)((Int32)skill - (Int32)Keys.NumPad0));
                        }
                    }
                }
                KeyCooldown = now;
            }
        }

        protected virtual Single GetAngle(Point mouseLocation)
        {
            PointF boundsCenter = Bounds.GetCenter();
            if (UserTank && Screen is PlayScreen PScreen)
            {
                mouseLocation.Offset((int)PScreen.CameraPoint.X, (int)PScreen.CameraPoint.Y);
            }
            var angle = boundsCenter.GetAngle(mouseLocation);
            if (Single.IsNaN(angle))
                return 0;
            return angle;
        }

        protected virtual void ExecuteShoot()
        {
            // Asynchronous Shoot Mode
            //Cannons[lastCannonIndex].Shoot();
            //lastCannonIndex++;
            //if (lastCannonIndex >= Cannons.Length)
            //    lastCannonIndex = 0;
            // Synchronous Shoot Mode
            foreach (var cannon in Cannons)
                cannon.Shoot();
        }

        private bool SkillButtonPressed()
        {
            for (int i = (Int32)Keys.NumPad0; i <= (Int32)Keys.NumPad9; i++)
            {
                if (Screen.MessageFilter.IsPressed((Keys)i))
                    return true;
            }
            return false;
        }

        public IEnumerable<Keys> SkillButtonsPressed()
        {
            for (int i = (Int32)Keys.NumPad0; i <= (Int32)Keys.NumPad9; i++)
            {
                if (Screen.MessageFilter.IsPressed((Keys)i))
                    yield return (Keys)i;
            }
        }

        private void IncrementVector(IEnumerable<Keys> keys)
        {
            var elapsed = Screen.Elapsed.Ticks;
            foreach (var key in keys)
            {
                switch (key)
                {
                    case Keys.W:
                    case Keys.Up:
                        movementVector.Y -= Acceleration * elapsed;
                        break;
                    case Keys.A:
                    case Keys.Left:
                        movementVector.X -= Acceleration * elapsed;
                        break;
                    case Keys.S:
                    case Keys.Down:
                        movementVector.Y += Acceleration * elapsed;
                        break;
                    case Keys.D:
                    case Keys.Right:
                        movementVector.X += Acceleration * elapsed;
                        break;
                }
            }
            if (Math.Abs(movementVector.X) > 1)
                movementVector.X = movementVector.X > 0 ? 1 : -1;
            if (Math.Abs(movementVector.Y) > 1)
                movementVector.Y = movementVector.Y > 0 ? 1 : -1;
        }

        public void ImproveSkill(SkillKey identifier)
        {
            switch (identifier)
            {
                case SkillKey.BulletDamage:
                    foreach (var cannon in Cannons)
                    {
                        cannon.SkillsManager.Power += EvolutionRate / EvolutionSteps;
                    }
                    break;
                case SkillKey.BulletEndurance:
                    foreach (var cannon in Cannons)
                    {
                        cannon.SkillsManager.Endurance += EvolutionRate / EvolutionSteps;
                    }
                    break;
                case SkillKey.ShootSpeed:
                    foreach (var cannon in Cannons)
                    {
                        cannon.SkillsManager.ShootSpeed += EvolutionRate / EvolutionSteps;
                    }
                    break;
                case SkillKey.BulletReload:
                    foreach (var cannon in Cannons)
                    {
                        cannon.SkillsManager.ShootRate++;
                    }
                    break;
                case SkillKey.Speed:
                    Speed += (MaxSpeed - MinSpeed) / EvolutionSteps;
                    break;
                case SkillKey.MaximumHealth:
                    if (MaximumHealth < MaxHealth)
                    {
                        MaximumHealth += (MaxHealth - MinHealth) / EvolutionSteps;
                        if (MaximumHealth > MaxHealth)
                        {
                            MaximumHealth = MaxHealth;
                        }
                    }
                    break;
                case SkillKey.HealthRegen:
                    if (HealthRegen < MaxHealthRegenRate)
                    {
                        HealthRegen += (MaxHealthRegenRate - MinHealthRegenRate) / EvolutionSteps;
                        if (HealthRegen > MaxHealthRegenRate)
                            HealthRegen = MaxHealthRegenRate;
                    }
                    break;
                case SkillKey.BodyDamage:
                    if (Power < MaxPower)
                    {
                        Power += (MaxPower - MinPower) / EvolutionSteps;
                        if (Power > MaxPower)
                            Power = MaxPower;
                    }
                    break;
                case SkillKey.Stability:
                    if (Stability < MaxStability)
                    {
                        Stability += (MaxStability - MinStability) / EvolutionSteps;
                        if (Stability > MaxStability)
                            Stability = Stability;
                    }
                    break;
                case SkillKey.ShootImpact:
                    foreach (var cannon in Cannons)
                    {
                        cannon.SkillsManager.ShootImpact += (SkillsManager.MaxShootImpact - SkillsManager.MinShootImpact) / EvolutionSteps;
                    }
                    break;
                default:
                    break;
            }
            // This code should be changed to not spend Score when the Skills is already at maximum level
            // Uncomment this line at final version
            //AvailableScore--;
        }

        public bool VerifyCollision(ITouchable other)
        {
            if (other is Shoot shoot)
            {
                if (shoot.Cannon.Support == this)
                    return false;
            }
            return CollisionBounds.IntersectsWith(other.CollisionBounds);
        }

        public void Collide(ITouchable other)
        {
            Health -= other.Power;
            if (Health <= 0)
            {
                Screen.MovableShoots.Remove(this);
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
    }
}