using Diep.GameEngine.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diep.GameConnection.Messages
{
    public class TankSpawnMessage : Message
    {
        public TankSpawnMessage()
        {
            Type = MessageType.TankSpawnMessage;
        }

        public TeamColor TeamColor { get; set; }
        public int Id { get; set; }
        public long ServerTankId { get; set; }
        public Single X { get; set; }
        public Single Y { get; set; }
        public Single Width { get; set; }
        public Single Height { get; set; }
        public Single Weight { get; set; }
        public String Name { get; set; }

        public override void Read(BinaryReader br)
        {
            Id = br.ReadInt32();
            ServerTankId = br.ReadInt64();
            X = br.ReadSingle();
            Y = br.ReadSingle();
            Width = br.ReadSingle();
            Height = br.ReadSingle();
            TeamColor = (TeamColor)br.ReadInt32();
            Weight = br.ReadSingle();
            Name = br.ReadString();
        }

        public override void Write(BinaryWriter bw)
        {
            bw.Write(Id);
            bw.Write(ServerTankId);
            bw.Write(X);
            bw.Write(Y);
            bw.Write(Width);
            bw.Write(Height);
            bw.Write((int)TeamColor);
            bw.Write(Weight);
            bw.Write(Name);
        }
    }
}
