using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class Program
    {
        public static string ServerIP = "127.0.0.1";
        public static int ServerPort = 14009;
        static void Main(string[] args)
        {
            WorkerThread.StartWork();

            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(ServerIP, ServerPort);
            Log.Info("Success Connect to server");
            Client client = new Client(socket);
            client.BeginReceive();
            client.Hello();
        }
    }
}
