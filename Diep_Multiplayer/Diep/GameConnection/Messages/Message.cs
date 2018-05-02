using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Diep.GameConnection.Messages
{
    public abstract class Message
    {
        public Message()
        {
            // Standard Constructor
        }

        public MessageType Type { get; protected set; }

        public abstract void Write(BinaryWriter bw);

        public abstract void Read(BinaryReader br);
    }
}
