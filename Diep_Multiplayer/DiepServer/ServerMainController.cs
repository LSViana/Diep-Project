using Diep.GameConnection.Messages;
using Diep.GameEngine;
using Diep.GameEngine.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DiepServer
{
    public class ServerMainController : MainController
    {
        private DateTime lastShootUpdateTime;
        private int shootUpdateInterval = 1000;

        public ServerMainController(ServerGameUI ServerGameUI, MessageFilter MessageFilter, bool EnableStepping, bool EnableDrawing, bool Online) : base(ServerGameUI, MessageFilter, EnableStepping, EnableDrawing, Online)
        {
            this.ServerGameUI = ServerGameUI;
            //
            lastShootUpdateTime = DateTime.Now.AddMilliseconds(-shootUpdateInterval);
        }

        public ServerGameUI ServerGameUI { get; }

        public override void Step(IEnumerable<object> data)
        {
            base.Step(data);
        }

        protected override void SendMessages(DateTime now)
        {
            base.SendMessages(now);
            //
            UpdateShoots(now);
        }

        private void UpdateShoots(DateTime now)
        {
            if ((now - lastShootUpdateTime).TotalMilliseconds > shootUpdateInterval)
            {
                var shoots = Screen.Shoots.ToArray();
                //
                foreach (var shoot in shoots)
                {
                    var sb = shoot.Bounds;
                    var mv = shoot.MovementVector;
                    var movableMessage = new ShootMoveMessage()
                    {
                        Id = shoot.Id,
                        Angle = shoot.Angle,
                        X = sb.X,
                        Y = sb.Y,
                        MvX = mv.X,
                        MvY = mv.Y,
                    };
                    //
                    ServerGameUI.UI.Broadcast(null, movableMessage);
                }
                lastShootUpdateTime = now;
            }
        }
    }
}
