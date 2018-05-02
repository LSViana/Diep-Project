using Diep.GameEngine.Controls;
using Diep.GameEngine.Scenario.Blocks;
using Diep.GameEngine.Scenario.Cannons;
using Diep.GameEngine.Scenario.Maps;
using Diep.GameEngine.Scenario.MovableBlocks;
using Diep.GameEngine.Scenario.Shoots;
using Diep.GameEngine.Scenario.Tanks;
using Diep.GameEngine.Shared;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diep.GameEngine.Scenario.Screens
{
    public class PlayScreen : GameScreen
    {
        public const Int32 MapWidth = 3840;
        public const Int32 MapHeight = 2160;
        public readonly Int32 ScreenWidth;
        public readonly Int32 ScreenHeight;
        public Size ClientScreen;

        public PlayScreen(GameController Controller) : base(Controller)
        {
            // Standard Property Values
            lastCollisionVerifyTime = DateTime.Now;
            ClientScreen = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Size;
            var screenBounds = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
            //
            ScreenWidth = screenBounds.Width;
            ScreenHeight = screenBounds.Height;
            GraphicsSupplier = new GraphicsSupplier();
        }

        public override List<Control> Controls { get; protected set; }

        private Map map;

        public override Map Map { get; protected set; }
        
        public bool PermanentAdded { get; private set; }

        private Tank userTank;
        private DateTime lastCollisionVerifyTime;

        public override void InitializeMembers()
        {
            // Initializing Properties
            Controls = new List<Control>();
            // Defining Controls
            map = new Map(this, Color.Gainsboro, Color.LightGray, 40);
            Map = map;
            // Defining User Tank
            SetUserTank(new Tanks.SpreadShot(this, TeamColor.DeepSkyBlue, .5f)
            {
                UserTank = true
            });
        }

        public override void SetUserTank(Tank tank)
        {
            userTank = tank;
            UserTank = userTank;
            var tankSize = new SizeF(Tank.StandardWidth, Tank.StandardHeight);
            var tankLocation = new PointF(1f * Bounds.Width / 2 - tankSize.Width / 2, 1f * Bounds.Height / 2 - tankSize.Height / 2);
            UserTank.Bounds = new RectangleF(tankLocation, tankSize);
            UserTank.Speed = Tank.MinSpeed;
            UserTank.RecalculateSize();
        }

        public override void Draw(Graphics g)
        {
            //// Calculating Camera Render Point
            CameraPoint = new PointF(
                -(ClientScreen.Width / 2 - UserTank.Bounds.X - UserTank.Bounds.Width / 2),
                -(ClientScreen.Height / 2 - UserTank.Bounds.Y - UserTank.Bounds.Height / 2)
                );
            var m = new Matrix();
            m.Scale(ZoomFactor, ZoomFactor, MatrixOrder.Append);
            m.Translate(ScreenWidth * (1 - ZoomFactor) / 2, ScreenHeight * (1 - ZoomFactor) / 2, MatrixOrder.Append);
            // Tests Purpose
            //g.ScaleTransform(.8f, .8f);
            g.Transform = m;
            g.FillRectangle(new SolidBrush(Color.DimGray), ScreenBounds);
            // Drawing
            base.Draw(g);
        }

        public void FeedMap(int TargetAmount)
        {
            bool permanentAdded = false;
            for (int i = 0; i < TargetAmount; i++)
            {
                // Initializing Block
                Block block;
                var r = Extensions.Random.Next(5);
                //var r = 0;
                if (r == 0)
                    block = new Square(this, TeamColor.Mustard);
                else if (r == 1)
                    block = new Circle(this, TeamColor.LightGreen);
                else if (r == 2)
                    block = new Triangle(this, TeamColor.IndianRed);
                else if (r == 3)
                    block = new Tracker(this, TeamColor.HotPink);
                else if (r == 4 && !PermanentAdded)
                {
                    permanentAdded = true;
                    block = new Stroller(this, TeamColor.White);
                }
                else
                    block = new Square(this, TeamColor.Mustard);
                // Randomizing Position and Angle
                var blockSize = new SizeF((Int32)(Map.SquareSize * 1.2f), (Int32)(Map.SquareSize * 1.2f));
                Single xPos = -1, yPos = -1;
                while (xPos < 0 || xPos > Map.Bounds.Width - blockSize.Width)
                {
                    xPos = (Single)(Extensions.Random.NextDouble() * Map.Bounds.Width);
                }
                while (yPos < 0 || yPos > Map.Bounds.Height - blockSize.Height)
                {
                    yPos = (Single)(Extensions.Random.NextDouble() * Map.Bounds.Height);
                }
                var angle = (Single)(Extensions.Random.NextDouble() * Math.PI * 2);
                var spin = (Single)(Extensions.Random.NextDouble() * Math.PI * 2);
                // Defining Properties
                block.Angle = angle;
                block.Spin = spin;
                if (block is Tracker || block is Stroller)
                {
                    var triangleSize = (Single)(blockSize.Width * (Extensions.Random.NextDouble() + .5));
                    block.Bounds = new RectangleF(xPos, yPos, (Single)(Math.Sqrt(Math.Pow(triangleSize, 2) - Math.Pow(triangleSize / 2, 2))), triangleSize);
                    block.Speed = (Single)(Extensions.Random.NextDouble() * .00003) + .00003f;
                }
                else
                {
                    if (block is Triangle)
                    {
                        block.Bounds = new RectangleF(xPos, yPos, blockSize.Width, (Single)(Math.Sqrt(Math.Pow(blockSize.Width, 2) - Math.Pow(blockSize.Width / 2, 2))));
                    }
                    else
                    {
                        block.Bounds = new RectangleF(xPos, yPos, blockSize.Width, blockSize.Height);
                    }
                    block.Speed = (Single)(Extensions.Random.NextDouble() * .00001f) + .00001f;
                    block.Rotation = (Single)(Extensions.Random.NextDouble() * .0005) + .0005f;
                }
                if (Extensions.Random.Next(2) == 0)
                    block.Rotation *= -1;
                //
                MovableShapes.Add(block);
            }
            // Avoiding post-spawn of permanent ships
            PermanentAdded = permanentAdded;
        }

        public override void PositionMembers()
        {
            // Maps
            Map.Bounds = new RectangleF(PointF.Empty, new SizeF(MapWidth, MapHeight));
            UserTank.SetCannons();
            UserTank.KeyCooldown = DateTime.Now.AddSeconds(1);
        }

        public override void Step(IEnumerable<object> data)
        {
            base.Step(data);
            //
            if (MovableShapes.Count(a => !(a is Stroller || a is Tracker)) == 0)
                FeedMap(Extensions.Random.Next(10) + 5);
            //
            CalculateCollisions();
        }

        private void CalculateCollisions()
        {
            var now = DateTime.Now;
            if ((now - lastCollisionVerifyTime).TotalMilliseconds <= 33.33f) // About to 30 times per second
                return;
            foreach (var movable in Touchables)
            {
                if (!movable.Active)
                    continue;
                foreach (var other in Touchables.Where(a => a != movable))
                {
                    if (!other.Active)
                        continue;
                    if (movable.VerifyCollision(other) && other.VerifyCollision(movable))
                    {
                        if (movable is Shoot shoot)
                        {
                            Single score = 0;
                            // Calculating Score obtained for Shoot
                            if (other.Health > shoot.Power * 2)
                                score = shoot.Power * 2;
                            else
                                score = other.Health * 2;
                            // Adding 
                            if(shoot.Cannon.Support is Tank tank)
                            {
                                tank.Score += score;
                            }
                            else if(shoot.Cannon.Support is Shoots.Destroyer destroyer)
                            {
                                if(destroyer.Cannon.Support is Tank destroyerTank)
                                {
                                    destroyerTank.Score += score;
                                }
                            }
                        }
                        other.Collide(movable);
                        movable.Collide(other);
                    }
                }
            }
            lastCollisionVerifyTime = DateTime.Now;
        }
    }
}