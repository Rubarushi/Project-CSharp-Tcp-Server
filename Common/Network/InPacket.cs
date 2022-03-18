using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Common.Network
{
    public class InPacket : IDisposable
    {
		private BinaryReader m_reader;
		private MemoryStream m_stream;
		public InPacket(byte[] data)
		{
			m_stream = new MemoryStream(data);
			m_reader = new BinaryReader(m_stream);
		}

		public int ReadInt()
		{
			return m_reader.ReadInt32();
		}

		public uint ReadUInt()
		{
			return m_reader.ReadUInt32();
		}

		public short ReadShort()
		{
			return m_reader.ReadInt16();
		}

		public ushort ReadUShort()
		{
			return m_reader.ReadUInt16();
		}

		public byte ReadByte()
		{
			return m_reader.ReadByte();
		}

		public bool ReadBool()
		{
			return m_reader.ReadBoolean();
		}

		public string ReadString()
		{
			int iLen = m_reader.ReadInt32();
			byte[] data = m_reader.ReadBytes(iLen);
			return Encoding.GetEncoding(949).GetString(data);
		}

		public void Dispose()
        {
			m_reader.Dispose();
			m_stream.Dispose();
        }
    }
}
