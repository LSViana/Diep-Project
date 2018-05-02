using Diep.GameConnection;
using Diep.GameEngine.Scenario.Tanks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DiepServer.GameConnection
{
    public class ServerDiepConnection : DiepConnection
    {
        public ServerDiepConnection(TcpClient TcpClient) : base(TcpClient)
        {
            this.Keys = new Dictionary<Keys, bool>();
            InitializeKeys();
        }

        private void InitializeKeys()
        {
            var values = Enum.GetValues(typeof(Keys));
            for (int i = 0; i < values.Length; i++)
            {
                Keys[(Keys)i] = false;
            }
        }

        public Dictionary<Keys, bool> Keys { get; }

        public Tank Tank { get; set; }
    }
}
