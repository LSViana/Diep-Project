using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diep.GameConnection.Messages
{
    public enum MessageType
    {
        Message,
        ConnectMessage,
        TankSpawnMessage,
        TankMoveMessage,
        ShootMessage,
        ShootSpawnMessage,
        ShootMoveMessage,
    }
}
