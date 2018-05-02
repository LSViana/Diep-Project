using Diep.GameEngine.Controls;
using Diep.GameEngine.Scenario.Blocks;
using Diep.GameEngine.Scenario.Maps;
using Diep.GameEngine.Scenario.Shoots;
using Diep.GameEngine.Scenario.Tanks;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diep.GameEngine.Shared
{
    public abstract class GameScreen : IGameObject
    {
        protected RectangleF screenBounds;
        protected Single zoomFactor;

        public GameScreen(GameController Controller)
        {
            this.Controller = Controller;
            // Standard Property Values
            Shoots = new List<IMovable>();
            Shapes = new List<IMovable>();
            Tanks = new List<IMovable>();
            ZoomFactor = 1f;
        }

        public GraphicsSupplier GraphicsSupplier { get; set; }

        public PointF CameraPoint { get; set; }

        public abstract List<Control> Controls { get; protected set; }

        public abstract Map Map { get; protected set; }

        public RectangleF ScreenBounds
        {
            get { return screenBounds; }
            set { screenBounds = value; }
        }

        public Single ZoomFactor
        {
            get { return zoomFactor; }
            set { zoomFactor = value; CalculateScreenSize(); }
        }

        public MessageFilter MessageFilter { get { return Controller.MessageFilter; } }

        public abstract void SetUserTank(Tank tank);

        public RectangleF DrawArea
        {
            get
            {
                var zoomCamera = CameraPoint;
                zoomCamera.X += screenBounds.X;
                zoomCamera.Y += screenBounds.Y;
                return new RectangleF(zoomCamera, screenBounds.Size);
            }
        }

        public virtual TimeSpan Elapsed
        {
            get { return Controller.Elapsed; }
        }

        protected virtual IEnumerable<IGameObject> GameObjects
        {
            get
            {
                return Drawables
                    .Cast<IGameObject>();
            }
        }

        protected virtual IEnumerable<ITouchable> Touchables
        {
            get
            {
                return
                    Tanks.OfType<ITouchable>()
                    .Union(Shoots.OfType<ITouchable>().ToArray())
                    .Union(Shapes.OfType<ITouchable>().ToArray())
                    .Cast<ITouchable>();
            }
        }

        protected virtual IEnumerable<IDrawable> Drawables
        {
            get
            {
                return
                    new IDrawable[] { Map }
                    .Union(Shoots.ToArray())
                    .Union(Shapes.ToArray())
                    .Union(Tanks.ToArray())
                    .Union(Controls)
                    .Cast<IDrawable>();
            }
        }

        public RectangleF Bounds
        {
            get { return Controller.Bounds; }
            set { Controller.Bounds = value; }
        }

        public GameController Controller { get; protected set; }

        public Tank UserTank { get; protected set; }

        public List<IMovable> Tanks { get; set; }

        public List<IMovable> Shapes { get; protected set; }

        public List<IMovable> Shoots { get; protected set; }

        public long Id { get; set; }

        public virtual void Draw(Graphics g)
        {
            // Standard Implementation
            foreach (var drawable in Drawables)
            {
                drawable?.Draw(g);
            }
        }

        public virtual void AddShoot(Shoot shoot)
        {
            Shoots.Add(shoot);
        }

        public virtual void RemoveShoot(Shoot shoot)
        {
            Shoots.Remove(shoot);
        }

        public virtual void AddTank(Tank tank)
        {
            Tanks.Add(tank);
        }
        
        public virtual void RemoveTank(Tank tank)
        {
            //Tanks.Remove(tank);
        }

        public virtual void AddShape(Shape shape)
        {
            Shapes.Add(shape);
        }

        public virtual void RemoveShape(Shape shape)
        {
            Shapes.Remove(shape);
        }

        public virtual void Step(IEnumerable<object> data)
        {
            // Standard Implementation
            foreach (var gameObject in GameObjects)
            {
                gameObject?.Step(data);
            }
        }

        protected void CalculateScreenSize()
        {
            //var deviceBounds = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
            var controllerControl = Controller.Control;
            var deviceBounds = controllerControl.Bounds;
            // Zoom-Factor Screen Bounds Calculation
            var width = deviceBounds.Width / zoomFactor;
            var height = deviceBounds.Height / zoomFactor;
            var x = (deviceBounds.Width - width) / 2;
            var y = (deviceBounds.Height - height) / 2;
            screenBounds = new RectangleF(x, y, width, height);
            // Standard Zoom-Factor
            //screenBounds = deviceBounds;
        }


        public abstract void InitializeMembers();

        public abstract void PositionMembers();
    }
}
