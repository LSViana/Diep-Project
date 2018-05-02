using Diep.GameEngine.Controls;
using Diep.GameEngine.Scenario.Maps;
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
            MovableShoots = new List<IMovable>();
            MovableShapes = new List<IMovable>();
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
                return new ITouchable[] { UserTank }
                    .Union(MovableShoots.OfType<ITouchable>().ToArray())
                    .Union(MovableShapes.OfType<ITouchable>().ToArray())
                    .Cast<ITouchable>();
            }
        }

        protected virtual IEnumerable<IDrawable> Drawables
        {
            get
            {
                return
                    new IDrawable[] { Map }
                    .Union(MovableShoots.ToArray())
                    .Union(MovableShapes.ToArray())
                    .Union(new IDrawable[] { UserTank })
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

        public List<IMovable> MovableShapes { get; protected set; }

        public List<IMovable> MovableShoots { get; protected set; }

        public virtual void Draw(Graphics g)
        {
            // Standard Implementation
            foreach (var drawable in Drawables)
            {
                drawable?.Draw(g);
            }
        }

        public virtual void Step(IEnumerable<object> data)
        {
            // Standard Implementation
            foreach (var gameObject in GameObjects)
            {
                gameObject.Step(data);
            }
        }

        private void CalculateScreenSize()
        {
            var deviceBounds = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
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
