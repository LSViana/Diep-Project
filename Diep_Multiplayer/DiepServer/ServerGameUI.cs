using Diep;
using Diep.GameEngine.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DiepServer
{
    public partial class ServerGameUI : GameUI
    {

        public ServerGameUI(UI UI, MessageFilter MessageFilter, bool Online = false) : base(MessageFilter, Online)
        {
            InitializeComponent();
            this.UI = UI;
        }

        public UI UI { get; }

        public override void GameUI_Load(object sender, EventArgs e)
        {
            Controller = new ServerMainController(this, MessageFilter, true, false, Online);
            Controller.Start(this, EventArgs.Empty);
        }
    }
}
