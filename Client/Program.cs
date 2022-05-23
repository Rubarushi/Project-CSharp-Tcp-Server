using Common;
using Common.IO;
using System;
using System.Net.Sockets;
using System.Threading;

namespace Client
{
	public class Program
	{
		public static string ServerIP = "127.0.0.1";
		public static int ServerPort = 14009;

		private static void Main( string[] args )
		{
			Log.StartLogger();
			AsyncStream.Start();
			WorkerThread.StartWork();

			for( int i = 0; i < 300; i++ )
			{
				WorkerThread.AddQueue( () =>
				{
					Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
					socket.Connect( ServerIP, ServerPort );
					Console.WriteLine( "Success Connect to server" );
					Client client = new Client(socket);
					client.BeginReceive();
					client.Hello();
					client.DoJob();
				} );
			}
		}
	}
}
