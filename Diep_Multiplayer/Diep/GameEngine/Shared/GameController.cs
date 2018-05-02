using Diep.GameConnection;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Diep.GameEngine.Shared
{
    public abstract class GameController : IGameObject
    {
        public const float PenWidth = 4f;
        public const int FrameRate = 120;
        public const int PenAlpha = 50;
        public const int TicksPerSecond = 10_000_000;
        public const int FrameTickRate = TicksPerSecond / FrameRate;

        public GameController(Control Control, MessageFilter MessageFilter, Boolean EnableStepping, Boolean EnableDrawing, Boolean Online)
        {
            this.Online = Online;
            this.Control = Control;
            this.MessageFilter = MessageFilter;
            this.EnableStepping = EnableStepping;
            this.EnableDrawing = EnableDrawing;
            // Initializing Properties
            InitializeMembers();
        }
        
        public DiepConnection Connection { get; protected set; }
        public Control Control { get; protected set; }
        public MessageFilter MessageFilter { get; protected set; }
        public bool EnableStepping { get; set; }
        public bool EnableDrawing { get; set; }
        public bool Online { get; }
        public GameScreen Screen { get; protected set; }

        public bool Running { get; protected set; }

        public virtual TimeSpan Elapsed { get; }

        public RectangleF Bounds
        {
            get { return Control?.Bounds ?? Rectangle.Empty; }
            set
            {
                if (Control is null)
                    return;
                Control.Bounds = Rectangle.Round(value);
            }
        }

        public IEnumerable<IDrawable> Drawables
        {
            get
            {
                return
                    new IDrawable[] { Screen }
                    .Cast<IDrawable>();
            }
        }

        public IEnumerable<GameScreen> Screens { get; protected set; }

        public long Id { get; set; }

        public abstract void Start(object sender, EventArgs e);

        public abstract void End(object sender, EventArgs e);

        public abstract void Step(IEnumerable<object> data);

        public abstract void SetGameScreen<T>() where T : GameScreen;

        public abstract void InitializeMembers();

        public void Draw(Graphics g)
        {
            if (!EnableDrawing)
                return;
            foreach (var drawable in Drawables)
            {
                drawable.Draw(g);
            }
        }
    }
}
