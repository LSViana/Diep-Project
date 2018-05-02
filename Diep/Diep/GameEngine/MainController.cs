using Diep.GameEngine.Scenario.Maps;
using Diep.GameEngine.Scenario.Screens;
using Diep.GameEngine.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Diep.GameEngine
{
    public class MainController : GameController
    {
        private TimeSpan elapsed;
        private Stopwatch stopWatch;

        public Thread Processing { get; protected set; }

        public Graphics Graphics { get; }

        public override TimeSpan Elapsed
        {
            get { return elapsed; }
        }

        public MainController(Control Control, MessageFilter MessageFilter, Boolean EnableStepping, Boolean EnableDrawing) : base(Control, MessageFilter, EnableStepping, EnableDrawing)
        {
            // Simplest Controller
            Processing = new Thread(Run);
            Graphics = Control.CreateGraphics();
        }

        public override void End(object sender, EventArgs e)
        {
            Processing.Abort();
        }

        public override void Start(object sender, EventArgs e)
        {
            Running = true;
            Processing.Start();
        }

        private void Run()
        {
            stopWatch = Stopwatch.StartNew();
            while (Running)
            {
                // Set the Elapsed time
                elapsed = new TimeSpan(stopWatch.ElapsedTicks);
                stopWatch.Restart();
                // Run
                var mousePosition = Control.MousePosition;
                mousePosition.Offset(-Control.Location.X, -Control.Location.Y);
                if (EnableStepping)
                    Step(
                        // Mouse Position
                        new object[] { mousePosition }
                        );
                // Redrawing
                if(EnableDrawing)
                    Control.Invalidate();
            }
        }

        public override void Step(IEnumerable<object> data)
        {
            if (!EnableStepping)
                return;
            Screen.Step(data);
        }

        public override void InitializeMembers()
        {
            // Defining Variables
            var startScreen = new StartScreen(this);
            var playScreen = new PlayScreen(this);
            // Defining Properties
            Screens = new GameScreen[]
            {
                startScreen,
                playScreen
            };
            // Initializing Screens
            foreach (var screen in Screens)
            {
                screen.InitializeMembers();
            }
            //
            SetGameScreen<PlayScreen>();
        }

        public override void SetGameScreen<T>()
        {
            var screen = Screens.First(a => a is T);
            Screen = screen;
            Screen.PositionMembers();
        }
    }
}