using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diep.GameConnection.Messages
{
    public class ConnectMessage : Message
    {
        public ConnectMessage()
        {
            Type = MessageType.ConnectMessage;
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public override void Read(BinaryReader br)
        {
            Id = br.ReadInt32();
            Name = br.ReadString();
        }

        public override void Write(BinaryWriter bw)
        {
            bw.Write(Id);
            bw.Write(Name);
        }
    }
}
