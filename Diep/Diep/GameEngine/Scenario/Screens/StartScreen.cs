using Diep.GameEngine.Controls;
using Diep.GameEngine.Scenario.Maps;
using Diep.GameEngine.Scenario.Tanks;
using Diep.GameEngine.Shared;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diep.GameEngine.Scenario.Screens
{
    public class StartScreen : GameScreen
    {
        private TextBox nameTextBox;
        private Label translucentBox;
        private FlatButton startButton;
        private Map map;
        private Label logoLabel;

        public StartScreen(GameController Controller) : base(Controller)
        {
            // Simplest Constructor
        }

        protected override IEnumerable<IDrawable> Drawables
        {
            get
            {
                return
                    new IDrawable[] { Map }
                    .Union(Controls)
                    .Cast<IDrawable>();
            }
        }

        public override List<Control> Controls { get; protected set; }

        public override Map Map { get; protected set; }

        public override void InitializeMembers()
        {
            // Initializing Properties
            Controls = new List<Control>();
            // Defining Variables
            map = new Map(this, Color.Gainsboro, Color.LightGray, 30)
            {
                Bounds = Bounds
            };
            nameTextBox = new TextBox(this, "", new Font("Segoe UI", Bounds.Width / 40, FontStyle.Bold), Color.White, Color.DimGray, true)
            {
                ReadOnly = false,
                MaxLength = 8
            };
            nameTextBox.Format.LineAlignment = StringAlignment.Center;
            nameTextBox.Format.Alignment = StringAlignment.Center;
            translucentBox = new Label(this, "", nameTextBox.Font, Color.FromArgb(50, Color.Gray), Color.Empty);
            startButton = new FlatButton(this, "Start", new Font("Segoe UI", Bounds.Width / 40), Color.DimGray, Color.White, false)
            {
                ReadOnly = true,
                MouseDownOnEnter = true
            };
            startButton.Format.LineAlignment = StringAlignment.Center;
            startButton.Format.Alignment = StringAlignment.Center;
            startButton.MouseDown += StartButton_MouseDown;
            logoLabel = new Label(this, "DIEP", new Font("Segoe UI", Bounds.Width / 25, FontStyle.Bold), Color.Transparent, Color.DimGray)
            {
                ReadOnly = true
            };
            logoLabel.Format.LineAlignment = StringAlignment.Center;
            logoLabel.Format.Alignment = StringAlignment.Center;
            // Defining Properties
            Controls.AddRange(new Control[] { nameTextBox, translucentBox, startButton });
            Map = map;
        }

        private void StartButton_MouseDown(object sender, EventArgs e)
        {
            Controller.SetGameScreen<PlayScreen>();
        }

        public override void PositionMembers()
        {
            var height10 = Bounds.Height * .1f;
            var height25 = Bounds.Height * .25f;
            var width25 = Bounds.Width * .25f;
            var leftCentered25 = 1f * Bounds.Width / 2 - width25 / 2;
            var topCentered10 = 1f * Bounds.Height / 2 - height10 / 2;
            var overHeight10 = topCentered10 - height25;
            var underHeight10 = topCentered10 + (Int32)(height10 * 1.2f);
            // Positioning Controls
            translucentBox.Bounds = Bounds;
            nameTextBox.Bounds = new RectangleF(leftCentered25, topCentered10, width25, height10);
            startButton.Bounds = new RectangleF(leftCentered25, underHeight10, width25, height10);
            logoLabel.Bounds = new RectangleF(leftCentered25, overHeight10, width25, height10);
        }

        public override void SetUserTank(Tank tank)
        {
            throw new NotImplementedException();
        }
    }
}
