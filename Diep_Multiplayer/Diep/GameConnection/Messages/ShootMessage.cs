using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diep.GameConnection.Messages
{
    public class ShootMessage : Message
    {
        public ShootMessage()
        {
            Type = MessageType.ShootMessage;
        }

        public int Id { get; set; }
        public byte CannonIndex { get; set; }
        public ShootSupportType SupportType { get; set; }
        public float Angle { get; set; }

        public override void Read(BinaryReader br)
        {
            Id = br.ReadInt32();
            CannonIndex = br.ReadByte();
            SupportType = (ShootSupportType)br.ReadInt32();
            Angle = br.ReadSingle();
        }

        public override void Write(BinaryWriter bw)
        {
            bw.Write(Id);
            bw.Write(CannonIndex);
            bw.Write((int)SupportType);
            bw.Write(Angle);
        }
    }
}
