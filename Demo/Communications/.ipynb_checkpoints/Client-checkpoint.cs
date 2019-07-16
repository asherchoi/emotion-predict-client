using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;

namespace Demo.Communications
{
    public class Client
    {
        private Socket sock;
        private IPEndPoint hostEndPoint;
        

        public Client(string IP, int PORT)
        {

            var addresses = Dns.GetHostAddresses(IP);
            hostEndPoint = new IPEndPoint(addresses[0], PORT);
            this.sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.sock.Connect(hostEndPoint);
        }

        public string ReceiveByte()
        {
            byte[] buffer = new byte[1024];
            string data = null;
            int bufferRec = this.sock.Receive(buffer);
            data += Encoding.Default.GetString(buffer, 0, bufferRec);
            return data;
        }

        public void SendByte(byte[] len, byte[] pixels)
        {
            byte[] data = Combine(len, pixels);
            this.sock.Send(data);
        }

        public byte[] Combine(byte[] first, byte[] second)
        {
            byte[] ret = new byte[first.Length + second.Length];
            Buffer.BlockCopy(first, 0, ret, 0, first.Length);
            Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);
            return ret;
        }

    }
   
}
