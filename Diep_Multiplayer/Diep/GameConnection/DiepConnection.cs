using Diep.GameConnection.Messages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Diep.GameConnection
{
    public class DiepConnection
    {
        public const string GameServer = "127.0.0.1";
        public const int GameServerPort = 6666;

        private static int connectionAmount = 0;
        private MemoryStream ms;

        static DiepConnection()
        {
            Formatter = new BinaryFormatter();
        }

        public delegate void MessageReceivedDelegate(DiepConnection Connection, Message Message);

        public event MessageReceivedDelegate MessageReceived;

        public DiepConnection()
        {
            Processing = new Thread(Run)
            {
                Name = $"Connection{++connectionAmount}"
            };
            ms = new MemoryStream();
            SendBuffers = new Queue<byte[]>();
            // Standard ServerIP and ServerPort values
            this.Server = GameServer;
            this.Port = GameServerPort;
            this.Id = -1;
        }

        public DiepConnection(string Server, int Port) : this()
        {
            this.Server = Server;
            this.Port = Port;
        }

        public DiepConnection(TcpClient TcpClient) : this()
        {
            this.TcpClient = TcpClient;
            Stream = TcpClient.GetStream();
        }

        public static String GetUsername()
        {
            return Dns.GetHostName();
        }

        public int Id { get; set; }
        public String Name { get; set; }
        public string Server { get; }
        public int Port { get; }
        public Thread Processing { get; }
        public Queue<byte[]> SendBuffers { get; private set; }
        public bool Running { get; private set; }
        public TcpClient TcpClient { get; private set; }
        public NetworkStream Stream { get; private set; }
        public static BinaryFormatter Formatter { get; private set; }

        public void Start()
        {
            if (TcpClient is null)
            {
                TcpClient = new TcpClient(Server, Port);
                Stream = TcpClient.GetStream();
            }
            //
            Running = true;
            Processing.Start();
        }

        public void Run()
        {
            while (Running)
            {
                //Thread.Sleep(5);
                if (SendBuffers.Count > 0)
                {
                    Send();
                }
                if (Stream.DataAvailable)
                {
                    Read();
                }
            }
        }

        private void Read()
        {
            var length = new byte[sizeof(int)];
            var recovered = Stream.Read(length, 0, length.Length);
            if(recovered == length.Length)
            {
                var dataLength = BitConverter.ToInt32(length, 0);
                var buffer = new byte[dataLength];
                recovered = Stream.Read(buffer, 0, buffer.Length);
                if(recovered == buffer.Length)
                {
                    var message = MessageTranslator.DecodeMessage(buffer);
                    MessageReceived?.Invoke(this, message);
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        private void Send()
        {
            while (SendBuffers.Count > 0)
            {
                var sb = SendBuffers.Dequeue();
                var length = BitConverter.GetBytes(sb.Length);
                var buffer = new byte[sb.Length + length.Length];
                length.CopyTo(buffer, 0);
                sb.CopyTo(buffer, length.Length);
                Stream.Write(buffer, 0, buffer.Length);
            }
        }

        public void Enqueue(Message message)
        {
            // Direct Sending
            var messageBytes = MessageTranslator.EncodeMessage(message);
            var bytes = new byte[sizeof(int) + messageBytes.Length];
            var length = BitConverter.GetBytes(messageBytes.Length);
            length.CopyTo(bytes, 0);
            messageBytes.CopyTo(bytes, 4);
            Stream.Write(bytes, 0, bytes.Length);
            // Queue
            //SendBuffers.Enqueue(MessageTranslator.EncodeMessage(message));
        }
    }
}
