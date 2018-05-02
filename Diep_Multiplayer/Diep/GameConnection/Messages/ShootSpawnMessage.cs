using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diep.GameConnection.Messages
{
    public class ShootSpawnMessage : Message
    {
        public ShootSpawnMessage()
        {
            Type = MessageType.ShootSpawnMessage;
        }

        public long Id { get; set; }
        public long ShootServerId { get; set; }
        public long IdSupport { get; set; }
        public byte CannonIndex { get; set; }
        public ShootSupportType SupportType { get; set; }

        public override void Read(BinaryReader br)
        {
            Id = br.ReadInt64();
            ShootServerId = br.ReadInt64();
            IdSupport = br.ReadInt64();
            SupportType = (ShootSupportType)br.ReadInt32();
            CannonIndex = br.ReadByte();
        }

        public override void Write(BinaryWriter bw)
        {
            bw.Write(Id);
            bw.Write(ShootServerId);
            bw.Write(IdSupport);
            bw.Write((int)SupportType);
            bw.Write(CannonIndex);
        }
    }
}
