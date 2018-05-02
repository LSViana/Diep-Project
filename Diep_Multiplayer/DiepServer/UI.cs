using Diep;
using Diep.GameConnection;
using Diep.GameConnection.Messages;
using Diep.GameEngine.Scenario.Tanks;
using Diep.GameEngine.Shared;
using DiepServer.GameConnection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DiepServer
{
    public partial class UI : Form
    {
        public Action<Action<Object>, Object> InvokerMainThread;

        public GameUI GUI { get; private set; }

        private long CurrentShootId;
        private long CurrentTankId;
        private int CurrentConnectionId;

        public UI()
        {
            InitializeComponent();
        }

        public List<ServerDiepConnection> Connections { get; private set; }
        public TcpListener TcpListener { get; private set; }
        public Thread AcceptConnections { get; private set; }
        public bool Running { get; set; }

        private void UI_Load(object sender, EventArgs e)
        {
            StartServer();
            InvokerMainThread = InvokeOnMainThread;
            var messageFilter = new MessageFilter();
            Application.AddMessageFilter(messageFilter);
            GUI = new ServerGameUI(this, messageFilter, true)
            {
                TopLevel = false,
                ControlBox = false,
                Bounds = new Rectangle(0, 0, panelGameUI.Width, panelGameUI.Height),
                Dock = DockStyle.Fill
            };
            panelGameUI.Controls.Add(GUI);
            // Decomment these lines when at the final version
            GUI.Show();
            //GUI.Controller.EnableDrawing = false;
        }

        private void InvokeOnMainThread(Action<object> action, object obj)
        {
            Invoke((Action)delegate
            {
                action?.Invoke(obj);
            });
        }

        private void StartServer()
        {
            // Properties to Manage Game
            Connections = new List<ServerDiepConnection>();
            // Start Listening
            TcpListener = new TcpListener(IPAddress.Loopback, DiepConnection.GameServerPort);
            TcpListener.Start();
            labelServerIP.Text = TcpListener.Server.LocalEndPoint.ToString();
            AcceptConnections = new Thread(AcceptConnection)
            {
                Name = $"AcceptConnection"
            };
            Running = true;
            AcceptConnections.Start();
        }

        private void AcceptConnection()
        {
            while (Running)
            {
                var freshConnection = new ServerDiepConnection(TcpListener.AcceptTcpClient());
                freshConnection.Start();
                freshConnection.MessageReceived += FreshConnection_MessageReceived;
            }
        }

        private void FreshConnection_MessageReceived(DiepConnection Connection, Diep.GameConnection.Messages.Message Message)
        {
            if (Connection is ServerDiepConnection sdc)
                switch (Message.Type)
                {
                    case MessageType.Message:
                        break;
                    case MessageType.ConnectMessage:
                        AuthenticateClient(sdc, Message as ConnectMessage);
                        SpawnTank(sdc);
                        SendOtherTanks(Connection);
                        break;
                    case MessageType.TankMoveMessage:
                        Broadcast(Connection, Message);
                        break;
                    case MessageType.TankSpawnMessage:
                        Broadcast(null, Message);
                        break;
                    case MessageType.ShootMessage:
                        ExecuteShoot(Connection as ServerDiepConnection, Message as ShootMessage);
                        break;
                    default:
                        break;
                }
        }

        private void ExecuteShoot(ServerDiepConnection connection, ShootMessage shootMessage)
        {
            Broadcast(null, shootMessage);
            //
            var spawnMessage = new ShootSpawnMessage()
            {
                Id = ++CurrentShootId,
                IdSupport = connection.Tank.Id,
                SupportType = shootMessage.SupportType,
                ShootServerId = CurrentShootId,
                CannonIndex = shootMessage.CannonIndex
            };
            //
            Broadcast(null, spawnMessage);
        }

        private void SendOtherTanks(DiepConnection Connection)
        {
            foreach (var connection in Connections.ToArray().Where(a => a.Id != Connection.Id && a.Tank != null))
            {
                var tank = connection.Tank;
                var bounds = tank.Bounds;
                var spawnMessage = new TankSpawnMessage()
                {
                    Id = connection.Id,
                    ServerTankId = tank.Id,
                    Name = connection.Tank.Name,
                    Height = bounds.Height,
                    Width = bounds.Width,
                    TeamColor = tank.TeamColor,
                    Weight = tank.Weight,
                    X = bounds.X,
                    Y = bounds.Y
                };
                Connection.Enqueue(spawnMessage);
            }
        }

        private void SpawnTank(ServerDiepConnection connection)
        {
            var tank = new Tank(GUI.Controller.Screen, TeamColor.DeepSkyBlue, Tank.StandardWeight)
            {
                Name = $"{connection.Name}@{connection.Id}",
                Id = ++CurrentTankId
            };
            tank.InitializeTank();
            var bounds = tank.Bounds;
            bounds.X = (float)(Extensions.Random.NextDouble() * 300);
            bounds.Y = (float)(Extensions.Random.NextDouble() * 300);
            tank.Bounds = bounds;
            // This line is removed because the Spawn is now performed through the Spawn Message
            ///GUI.Controller.Screen.Tanks.Add(tank);
            var spawnMessage = new TankSpawnMessage()
            {
                Id = connection.Id,
                ServerTankId = tank.Id,
                Name = tank.Name,
                X = bounds.X,
                Y = bounds.Y,
                Width = bounds.Width,
                Height = bounds.Height,
                TeamColor = tank.TeamColor,
                Weight = tank.Weight,
            };
            connection.Tank = tank;
            // @null to broadcast it to every user connected at server
            Broadcast(null, spawnMessage);
        }

        public void Broadcast(DiepConnection Connection, Diep.GameConnection.Messages.Message message)
        {
            foreach (var connection in Connections.ToArray().Where(a => Connection == null || a.Id != Connection.Id))
            {
                connection.Enqueue(message);
            }
        }

        private void AuthenticateClient(ServerDiepConnection connection, ConnectMessage message)
        {
            message.Id = ++CurrentConnectionId;
            connection.Id = message.Id;
            connection.Name = message.Name;
            connection.Enqueue(message);
            Connections.Add(connection);
            InvokerMainThread.Invoke((amount) => { labelUserAmount.Text = amount.ToString(); }, Connections.Count);
        }
    }
}
