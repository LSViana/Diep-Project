using Diep.GameConnection.Messages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Diep.GameConnection
{
    public static class MessageTranslator
    {
        private static Dictionary<string, Type> types;

        static MessageTranslator()
        {
            var baseMessageType = typeof(Message);
            var assemblyTypes = Assembly.GetExecutingAssembly().GetTypes().Where(a => a == baseMessageType || a.IsSubclassOf(baseMessageType));
            types = new Dictionary<string, Type>();
            var values = Enum.GetValues(typeof(MessageType));
            for (int i = 0; i < values.Length; i++)
            {
                var typeName = ((MessageType)i).ToString();
                types[typeName] = assemblyTypes.First(a => a.Name == typeName);
            }
        }

        public static byte[] EncodeMessage(Message message)
        {
            var ms = new MemoryStream();
            var bw = new BinaryWriter(ms);
            bw.Write((int)message.Type);
            message.Write(bw);
            bw.Flush();
            return ms.ToArray();
        }

        public static Message DecodeMessage(byte[] bytes)
        {
            var type = (MessageType)BitConverter.ToInt32(bytes, 0);
            var msg = Activator.CreateInstance(types[type.ToString()]) as Message;
            var ms = new MemoryStream(bytes, sizeof(int), bytes.Length - sizeof(int));
            var br = new BinaryReader(ms);
            msg.Read(br);
            return msg;
        }
    }
}
