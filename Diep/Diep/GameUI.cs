using Diep.GameEngine;
using Diep.GameEngine.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Diep
{
    public partial class GameUI : Form
    {
        private MessageFilter MessageFilter;
        private Pen borderPen = new Pen(Color.Gray);

        public GameController Controller { get; private set; }

        public GameUI(MessageFilter MessageFilter)
        {
            //SetStyle(ControlStyles.UserPaint, true);
            //SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            InitializeComponent();
            this.MessageFilter = MessageFilter;

        }

        private void GameUI_Load(object sender, EventArgs e)
        {
            Controller = new MainController(this, MessageFilter, false, true);
            MessageFilter.KeyPress += MessageFilter_KeyPress;
            MessageFilter.KeyUp += MessageFilter_KeyUp;
            Controller.Start(this, EventArgs.Empty);
        }

        private void MessageFilter_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void MessageFilter_KeyPress(object sender, KeyEventArgs e)
        {

        }

        private void GameUI_FormClosing(object sender, FormClosingEventArgs e) => Controller.End(this, EventArgs.Empty);

        protected override void OnPaint(PaintEventArgs e)
        {
            // Draw Border
            e.Graphics.DrawRectangle(borderPen, 0, 0, Width - 1, Height - 1);
            //
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            //e.Graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
            Controller.Draw(e.Graphics);
        }
    }
}
