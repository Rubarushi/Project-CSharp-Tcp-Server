using Common;
using Common.Network;
using System;
using System.Net.Sockets;
using System.Timers;

namespace Client
{
	public class Client : Session
	{
		public Client( Socket Socket ) : base()
		{
			SetSocket( Socket );
			GUID = Guid.NewGuid().ToString();
		}

		public void Hello()
		{
			Console.WriteLine( "Send Hello" );
			OutPacket o = new OutPacket(CTPK_HELLO);
			o.Write( "ID" );
			o.Write( "PW" );
			o.Write( GUID );
			Send( o );
		}

		public void MoveMap( int MapIDX )
		{
			sendTime = DateTime.Now;
			OutPacket o = new OutPacket(CTPK_MOVE_MAP);
			o.Write( MapIDX );
			Send( o );
		}

		public override void OnDisconnect()
		{
			timer.Stop();
			timer.Dispose();
		}

		public override void OnPacket( int protocolID, InPacket iPacket )
		{
			try
			{
				switch( protocolID )
				{
					case STPK_HELLO:
					{
						OnHello( iPacket );
					}
					break;
					case STPK_MOVE_MAP:
					{
						OnMoveMap( iPacket );
					}
					break;
					case STPK_DISCONNECT:
					{
						PreDisconnect();
					}
					break;
					case STPK_OUT_MAP:
					{
						OnOutMap( iPacket );
					}
					break;
				}
			}
			catch( Exception e )
			{
				Log.Exception( e );
				Disconnect();
			}
		}

		private void OnOutMap( InPacket iPacket )
		{
			bool isMe = iPacket.ReadBool();
			if( !isMe )
			{
				for( int i = 0; i < iPacket.ReadInt(); i++ )
				{
					string n = iPacket.ReadString();
					Console.WriteLine( n + "님이 맵에서 나가셨습니다." );
				}
			}
		}

		private void OnHello( InPacket iPacket )
		{
			bool bSuccess = iPacket.ReadBool();
			Console.WriteLine( "현재 클라이언트의 GUID: {0}", GUID );
		}

		private void PreDisconnect()
		{
			Console.WriteLine( "서버와 연결이 끊겼습니다.(예정된 작업) {0}", GUID );
			timer.Stop();
			timer.Dispose();
			Disconnect();
		}

		private object _lock = new object();
		private DateTime sendTime = DateTime.Now;

		private void OnMoveMap( InPacket iPacket )
		{
			bool isMe = iPacket.ReadBool();
			bool bSuccess = iPacket.ReadBool();

			if( isMe )
			{
				if( bSuccess == false )
				{
					Console.WriteLine( "맵 진입에 실패하였습니다." );
					return;
				}
				else
				{
					Console.WriteLine( "맵 진입에 성공하였습니다. 걸린 시간: {0}", DateTime.Now - sendTime );
				}

				lock( _lock )
				{
					int cnt = iPacket.ReadInt();
					for( int i = 0; i < cnt; i++ )
					{
						string tGUID = iPacket.ReadString();
						//Console.WriteLine( "[{1}]현재 맵에 있는 클라이언트들: {0}", tGUID, i + 1 );
					}
				}
			}
		}

		private Timer timer = new Timer(1000);// Times[random.Next(0, 3)]);
		private static int[] Times = new int[]{5000, 1000, 10000, 13000};
		internal void DoJob()
		{
			timer.Elapsed += Timer_Elapsed;
			timer.Start();
		}

		public static Random random = new Random();

		private void Timer_Elapsed( object sender, ElapsedEventArgs e ) => MoveMap( random.Next( 0, 100 ) );
	}
}
