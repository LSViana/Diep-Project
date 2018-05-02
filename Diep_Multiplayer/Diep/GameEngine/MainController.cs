using Diep.GameConnection;
using Diep.GameConnection.Messages;
using Diep.GameEngine.Scenario.Blocks;
using Diep.GameEngine.Scenario.Maps;
using Diep.GameEngine.Scenario.Screens;
using Diep.GameEngine.Scenario.Shoots;
using Diep.GameEngine.Scenario.Tanks;
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
        //private const double messageInterval = 50;
        private const double messageInterval = 1000 / 30.0;
        private TimeSpan elapsed;
        private Stopwatch stopWatch;
        private DateTime lastInfoTime = DateTime.Now.AddMilliseconds(-messageInterval);

        public Thread Processing { get; protected set; }
        public Dictionary<long, Tank> Tanks { get; set; }
        public Dictionary<long, Shoot> Shoots { get; set; }
        public Dictionary<long, Shape> Shapes { get; set; }

        public override TimeSpan Elapsed
        {
            get { return elapsed; }
        }

        public MainController(Control Control, MessageFilter MessageFilter, Boolean EnableStepping, Boolean EnableDrawing, Boolean Online) : base(Control, MessageFilter, EnableStepping, EnableDrawing, Online)
        {
            // Simplest Controller
            Processing = new Thread(Run)
            {
                Name = "Controller"
            };
            //
            if (Online)
            {
                Connection = new DiepConnection();
                Connection.MessageReceived += Client_MessageReceived;
                Connection.Start();
                var connectMessage = new ConnectMessage()
                {
                    Id = -1,
                    Name = DiepConnection.GetUsername(),
                };
                Connection.Enqueue(connectMessage);
            }
            // Starting Controller Dictionaries
            Tanks = new Dictionary<long, Tank>();
            Shoots = new Dictionary<long, Shoot>();
            Shapes = new Dictionary<long, Shape>();
        }

        private void Client_MessageReceived(DiepConnection Connection, GameConnection.Messages.Message Message)
        {
            Control.Invoke((Action)delegate
           {
               switch (Message.Type)
               {
                   case MessageType.Message:
                       break;
                   case MessageType.ConnectMessage:
                       SetIdentification(Connection, Message as ConnectMessage);
                       break;
                   case MessageType.TankSpawnMessage:
                       if (Message is TankSpawnMessage tsm)
                           SpawnTank(tsm);
                       break;
                   case MessageType.TankMoveMessage:
                       MoveTank(Message as TankMoveMessage);
                       break;
                   case MessageType.ShootMessage:
                       ExecuteShoot(Message as ShootMessage);
                       break;
                   case MessageType.ShootSpawnMessage:
                       if (Message is ShootSpawnMessage ssm)
                           SpawnShoot(ssm);
                       break;
                   case MessageType.ShootMoveMessage:
                       MoveShoot(Message as ShootMoveMessage);
                       break;
                   default:
                       break;
               }
           });
        }

        private void MoveShoot(ShootMoveMessage shootMoveMessage)
        {
            var shoot = Shoots[shootMoveMessage.Id];
            var loc = new PointF(shootMoveMessage.X, shootMoveMessage.Y);
            shoot.Bounds = new RectangleF(loc, shoot.Bounds.Size);
            shoot.MovementVector = new Vector2D(shootMoveMessage.MvX, shootMoveMessage.Y);
            shoot.Angle = shootMoveMessage.Angle;
        }

        private void ExecuteShoot(ShootMessage shootMessage)
        {
            var tank = Screen.Tanks.First(a => a.Id == shootMessage.Id) as Tank;
            var cannon = tank.Cannons[shootMessage.CannonIndex];
            cannon.ReducingRecoil = true;
            cannon.PullSupport(cannon.GetShootSize(), Extensions.GetVectorFromAngle(tank.Angle));
        }

        private void MoveTank(TankMoveMessage movableMessage)
        {
            var tank = Tanks[movableMessage.Id];
            tank.Bounds = new RectangleF(new PointF(movableMessage.X, movableMessage.Y), tank.Bounds.Size);
            tank.Score = movableMessage.Score;
            tank.Angle = movableMessage.Angle;
            tank.MovementVector = new Vector2D(movableMessage.MvX, movableMessage.MvY);
        }

        private void SpawnShoot(ShootSpawnMessage spawnMessage)
        {
            if (spawnMessage.SupportType == ShootSupportType.Tank)
            {
                var tank = Screen.Tanks.Cast<IGameObject>().First(a => a.Id == spawnMessage.IdSupport) as Tank;
                var cannon = tank.Cannons[spawnMessage.CannonIndex];
                var shoot = cannon.GetReadyShoot();
                shoot.Id = spawnMessage.ShootServerId;
                Shoots[shoot.Id] = shoot;
                Screen.AddShoot(shoot);
            }
        }

        private void SpawnTank(TankSpawnMessage spawnMessage)
        {
            var tank = new Tank(Screen, spawnMessage.TeamColor, spawnMessage.Weight);
            tank.Id = spawnMessage.ServerTankId;
            var bounds = new RectangleF(spawnMessage.X, spawnMessage.Y, spawnMessage.Width, spawnMessage.Height);
            tank.Bounds = bounds;
            tank.InitializeTank();
            if (spawnMessage.Id == Connection.Id)
            {
                Screen.SetUserTank(tank);
                tank.InitializeTank();
            };
            Tanks[tank.Id] = tank;
            Screen.AddTank(tank);
        }

        private void SetIdentification(DiepConnection connection, ConnectMessage message)
        {
            connection.Id = message.Id;
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
                var mousePosition = MessageFilter.MousePosition;
                //mousePosition.Offset(-Control.Location.X, -Control.Location.Y);
                if (EnableStepping)
                    Step(
                        // Mouse Position
                        new object[] { mousePosition }
                        );
                // Redrawing
                if (EnableDrawing)
                    Control.Invalidate();
            }
        }

        public override void Step(IEnumerable<object> data)
        {
            if (!EnableStepping)
                return;
            Screen.Step(data);
            if (Screen.UserTank != null && Online)
            {
                var now = DateTime.Now;
                if (Connection != null && Connection.Id > 0 && (now - lastInfoTime).TotalMilliseconds > messageInterval)
                {
                    SendMessages(now);
                    lastInfoTime = now;
                }
            }
        }

        protected virtual void SendMessages(DateTime now)
        {
            // Send info about tank state to server
            var ut = Screen.UserTank;
            if (ut is null)
                return;
            var mv = ut.MovementVector;
            Connection.Enqueue(new TankMoveMessage()
            {
                Id = ut.Id,
                Angle = ut.Angle,
                X = ut.Bounds.X,
                Y = ut.Bounds.Y,
                MvX = mv.X,
                MvY = mv.Y,
                Score = ut.Score,
            });
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