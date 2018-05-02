using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diep.GameConnection.Messages
{
    public class TankMoveMessage : Message
    {
        public TankMoveMessage()
        {
            Type = MessageType.TankMoveMessage;
        }

        public long Id { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float MvX { get; set; }
        public float MvY { get; set; }
        public float Score { get; set; }
        public float Angle { get; set; }

        public override void Read(BinaryReader br)
        {
            Id = br.ReadInt64();
            X = br.ReadSingle();
            Y = br.ReadSingle();
            MvX = br.ReadSingle();
            MvY = br.ReadSingle();
            Score = br.ReadSingle();
            Angle = br.ReadSingle();
        }

        public override void Write(BinaryWriter bw)
        {
            bw.Write(Id);
            bw.Write(X);
            bw.Write(Y);
            bw.Write(MvX);
            bw.Write(MvY);
            bw.Write(Score);
            bw.Write(Angle);
        }
    }
}
