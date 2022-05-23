using System;
using System.IO;
using System.Text;

namespace Common.Network
{
	public class OutPacket : IDisposable
	{
		private MemoryStream m_stream;
		private BinaryWriter m_writer;
		private int size => (int)m_writer.BaseStream.Length + 8;
		private int protocol = 0;

		public OutPacket( int protocol )
		{
			m_stream = new MemoryStream( 64 );
			m_writer = new BinaryWriter( m_stream );
			this.protocol = protocol;
		}

		public OutPacket Write( short n )
		{
			m_writer.Write( n );
			return this;
		}

		public OutPacket Write( int n )
		{
			m_writer.Write( n );
			return this;
		}

		public OutPacket Write( long n )
		{
			m_writer.Write( n );
			return this;
		}

		public OutPacket Write( string str )
		{
			byte[] data = Encoding.GetEncoding(949).GetBytes(str);
			m_writer.Write( data.Length );
			m_writer.Write( data );
			return this;
		}

		public OutPacket Write( bool n )
		{
			m_writer.Write( n );
			return this;
		}

		public OutPacket Write( byte n )
		{
			m_writer.Write( n );
			return this;
		}

		public byte[] ToArray()
		{
			byte[] data = new byte[m_stream.Length + 4 + 4];
			Buffer.BlockCopy( BitConverter.GetBytes( protocol ), 0, data, 0, 4 );
			Buffer.BlockCopy( BitConverter.GetBytes( size ), 0, data, 4, 4 );
			Buffer.BlockCopy( m_stream.ToArray(), 0, data, 8, (int)m_stream.Length );

			return data;
		}

		public void Dispose()
		{
			m_writer.Dispose();
			m_stream.Dispose();
		}
	}
}
