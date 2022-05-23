using Common;
using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Server.Network
{
	public class Acceptor
	{
		private TcpListener Listener;
		public object _lock = new object();
		public List<Client> m_vClient = new List<Client>();
		private int port = 0;

		public Acceptor( int port )
		{
			this.port = port;
			Listener = new TcpListener( System.Net.IPAddress.Any, port );
		}

		public void StartWork()
		{
			Console.WriteLine( "New Acceptor Ready on {0}", port );
			Listener.Start();
			Listener.BeginAcceptSocket( new AsyncCallback( OnAccept ), null );
		}

		private void OnAccept( IAsyncResult ar )
		{
			if( Server.m_bStopped )
			{
				Listener.Stop();
				return;
			}

			Socket Socket = Listener.EndAcceptSocket(ar);

			Console.WriteLine( "New Client Joined {0}", Socket.RemoteEndPoint.ToString() );

			Client client = new Client(Socket);
			client.BeginReceive();

			lock( Server.Acceptor._lock )
			{
				m_vClient.Add( client );
			}

			Listener.BeginAcceptSocket( new AsyncCallback( OnAccept ), null );
		}
	}
}
