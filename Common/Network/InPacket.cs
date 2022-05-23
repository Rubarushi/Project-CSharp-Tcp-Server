using System;
using System.IO;
using System.Text;

namespace Common.Network
{
	public class InPacket : IDisposable
	{
		private BinaryReader m_reader;
		private MemoryStream m_stream;
		public InPacket( byte[] data )
		{
			m_stream = new MemoryStream( data );
			m_reader = new BinaryReader( m_stream );
		}

		public int ReadInt() => m_reader.ReadInt32();

		public uint ReadUInt() => m_reader.ReadUInt32();

		public short ReadShort() => m_reader.ReadInt16();

		public ushort ReadUShort() => m_reader.ReadUInt16();

		public byte ReadByte() => m_reader.ReadByte();

		public bool ReadBool() => m_reader.ReadBoolean();

		public string ReadString()
		{
			int iLen = m_reader.ReadInt32();
			byte[] data = m_reader.ReadBytes(iLen);
			return Encoding.GetEncoding( 949 ).GetString( data );
		}

		public void Dispose()
		{
			m_reader.Dispose();
			m_stream.Dispose();
		}

		public byte[] ToArray() => m_stream.ToArray();

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			foreach( byte b in ToArray() )
			{
				sb.AppendFormat( "{0:X2} ", b );
			}

			return sb.ToString();
		}
	}
}
