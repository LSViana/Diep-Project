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
        private Pen borderPen = new Pen(Color.Gray);

        public MessageFilter MessageFilter { get; protected set; }
        public GameController Controller { get; protected set; }

        public bool Online { get; }

        public GameUI(MessageFilter MessageFilter, bool Online = false)
        {
            //SetStyle(ControlStyles.UserPaint, true);
            //SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            InitializeComponent();
            this.MessageFilter = MessageFilter;
            this.Online = Online;
        }

        public virtual void GameUI_Load(object sender, EventArgs e)
        {
            Controller = new MainController(this, MessageFilter, true, true, Online);
            Controller.Start(this, EventArgs.Empty);
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
