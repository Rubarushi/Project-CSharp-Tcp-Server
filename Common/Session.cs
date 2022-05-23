using Common.IO;
using System;
using System.Net.Sockets;
using System.Threading;

namespace Common.Network
{
	public abstract class Session : Protocol
	{
		public string GUID { get; protected set; }
		public string IP { get; protected set; } = "0.0.0.0:0";
		public Socket Socket { get; private set; }

		public abstract void OnDisconnect();

		public abstract void OnPacket( int protocolID, InPacket iPacket );

		private byte[] mBuffer = new byte[5000];
		private byte[] mSharedBuffer = new byte[5000];
		private int mCursor = 0;

		public int mDisconnected = 0;

		public void SetSocket( Socket socket )
		{
			Socket = socket;
			IP = Socket.RemoteEndPoint.ToString();
		}

		public void Disconnect()
		{
			if( Interlocked.CompareExchange( ref mDisconnected, 1, 0 ) == 0 )
			{
				OnDisconnect();

				try
				{
					Socket.Shutdown( SocketShutdown.Both );
					Socket.BeginDisconnect( true, new AsyncCallback( DisconnectCallBack ), Socket );
				}
				catch
				{

				}
			}
		}

		public void TryCatch( Action action )
		{
			try
			{
				action.Invoke();
			}
			catch( Exception ex )
			{
				Log.Error( ex.ToString() );
				Disconnect();
			}
		}

		private void DisconnectCallBack( IAsyncResult ar )
		{
			Socket client = (Socket)ar.AsyncState;
			client.EndDisconnect( ar );
		}

		public void BeginReceive()
		{
			if( mDisconnected != 0 || !Socket.Connected )
			{
				Disconnect();
				return;
			}
			try
			{
				Socket.BeginReceive( mSharedBuffer, 0, 5000, SocketFlags.None, new AsyncCallback( EndReceive ), Socket );
			}
			catch
			{
				Disconnect();
			}
		}

		public void Append( byte[] pBuffer, int pStart, int pLength )
		{
			try
			{
				if( mBuffer.Length - mCursor < pLength )
				{
					int newSize = mBuffer.Length * 2;
					while( newSize < mCursor + pLength )
					{
						newSize *= 2;
					}
					Array.Resize( ref mBuffer, newSize );
				}
				Buffer.BlockCopy( pBuffer, pStart, mBuffer, mCursor, pLength );
				mCursor += pLength;
			}
			catch
			{

			}
		}

		/// <summary>
		/// 패킷 규약
		/// ---------
		/// int ProtocolID
		/// int size
		/// ---------
		/// data 시작.
		/// </summary>
		private void EndReceive( IAsyncResult ar )
		{
			if( mDisconnected != 0 )
			{
				return;
			}

			try
			{
				int iLen = 0;
				try
				{
					iLen = Socket.EndReceive( ar );
				}
				catch
				{
					Disconnect();
					return;
				}

				if( iLen <= 0 )
				{
					Disconnect();
					return;
				}

				Append( mSharedBuffer, 0, iLen );

				while( true )
				{
					if( mCursor < 8 )
					{
						break;
					}

					int size = BitConverter.ToInt32(mBuffer, 4);

					if( mCursor < size )
					{
						break;
					}

					byte[] data = new byte[size];
					Buffer.BlockCopy( mBuffer, 0, data, 0, size );

					InPacket iPacket = new InPacket(data);

					int protocolID = iPacket.ReadInt();
					iPacket.ReadInt(); //사이즈 스킵(위에서 읽었음)

					mCursor -= size;

					if( mCursor > 0 )
					{
						Buffer.BlockCopy( mBuffer, size, mBuffer, 0, mCursor );
					}

					if( mDisconnected != 0 )
					{
						return;
					}
					//WorkerThread.AddQueue( () => { } ); //이거 안쓴게 성능이 더나옴.
					OnPacket( protocolID, iPacket );
				}

				BeginReceive();
			}
			catch( Exception e )
			{
				AsyncStream.Write( "Exception.txt", e.ToString() );

				Disconnect();
			}
		}

		public void Send( OutPacket pPacket )
		{
			try
			{
				byte[] data = pPacket.ToArray();

				Socket.Send( data );
			}
			catch( SocketException )
			{
				OnDisconnect();
			}
			catch( Exception e )
			{
				AsyncStream.Write( "Exception.txt", e.ToString() );
				Disconnect();
				return;
			}
		}
	}
}
