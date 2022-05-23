using Common;
using Common.Network;
using Server.Map;
using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Server
{
	public class Client : Session
	{

		public Client( Socket Socket ) : base()
		{
			SetSocket( Socket );
		}

		public bool OnDisconnectCalled = false;

		public override void OnDisconnect()
		{
			if( OnDisconnectCalled == false )
			{
				lock( Server.Acceptor._lock )
				{
					Server.Acceptor.m_vClient.Remove( this );
				}
				Console.WriteLine( "Disconnected: {0}", IP );
				OnDisconnectCalled = true;
			}
		}

		public override void OnPacket( int protocolID, InPacket iPacket )
		{
			Console.WriteLine( "[{2}][{0}]{1}", protocolID, iPacket.ToString(), GUID );
			TryCatch( () =>
				{
					switch( protocolID )
					{
						case CTPK_HELLO:
							OnHello( iPacket );
							break;
						case CTPK_MOVE_MAP:
							OnMoveMap( iPacket );
							break;
						case CTPK_DISCONNECT:
							OnDisconnectPrepare();
							break;
						default:
							Log.Warning( "Unknown Packet: {0}", protocolID );
							break;
					}
				} );
		}

		private void OnDisconnectPrepare()
		{
			try
			{
				if( !CurMap.Equals( null ) )
				{
					using( OutPacket o = new OutPacket( STPK_MOVE_MAP ) )
					{
						o.Write( false );
						o.Write( GUID );
						SendToMap( o, true, CurMap );
					}

					CurMap.RemoveClient( this );
				}
			}
			catch( Exception e )
			{
				Log.Exception( e );
			}
		}

		public void SendToMap( OutPacket o, bool IncludeMe, MapBase Map )
		{
			List<Client> cs = Map.GetClients();
			for( int i = 0; i < cs.Count; i++ )
			{
				Client target = cs[i];
				if( target != null )
				{
					{
						target.Send( o );
					}
				}
			}
		}

		private MapBase CurMap = null;
		private void OnMoveMap( InPacket iPacket )
		{
			int MapIDX = iPacket.ReadInt();
			OutPacket oa = new OutPacket(STPK_MOVE_MAP);

			MapBase Map = MapBase.Maps[MapIDX];

			if( Map.MaxClient <= Map.NowClient )
			{
				oa.Write( true );    //나인가
				oa.Write( false );   //입장실패
			}
			else
			{
				if( CurMap != null )
				{
					using( OutPacket o = new OutPacket( STPK_OUT_MAP ) )
					{
						o.Write( GUID );
						lock( Map._lock )
						{
							SendToMap( o, false, CurMap );
						}
					}
					CurMap.RemoveClient( this );
				}

				CurMap = Map;
				Map.AddClient( this );

				oa.Write( true );  //입장 성공
				oa.Write( true );
				lock( Map._lock )
				{
					List<Client> Clients = Map.GetClients();
					oa.Write( Clients.Count );
					for( int i = 0; i < Clients.Count; i++ )
					{
						Client target = Clients[i];
						if( target != null )
						{
							oa.Write( target.GUID );    //현재 맵에 있는 클라이언트들의 이름 전송.
						}
					}
				}
			}
			Send( oa );
		}

		private void OnHello( InPacket iPacket )
		{
			string ID = iPacket.ReadString();
			string PWD = iPacket.ReadString();
			GUID = iPacket.ReadString();
			Console.WriteLine( "ID: {0} PWD: {1} GUID: {2}", ID, PWD, GUID );

			OutPacket o = new OutPacket(STPK_HELLO);
			o.Write( true );

			Send( o );
		}
	}
}
