using System;
using Common.Network;
using System.Threading;
using System.Net.Sockets;

namespace Common
{
    public abstract class Session : Protocol
    {
        public abstract void OnPacket(int protocolID, InPacket iPacket);
        public abstract void OnDisconnect();

        public Socket Socket { get; protected set; }
        public string GUID { get; protected set; }
        public string IP { get; protected set; } = "0.0.0.0:0";

        public byte[] mSharedBuffer = new byte[5000];
        public byte[] mBuffer = new byte[5000];

        public int mOffset = 0;
        public void SetSocket(Socket socket)
        {
            Socket = socket;
            IP = Socket.RemoteEndPoint.ToString();
        }

        public Session()
        {
            GUID = new Guid().ToString();
        }

        public void BeginReceive()
        {
            if(mDisconnected != 0 || !Socket.Connected)
            {
                Disconnect();
                return;
            }

            //Socket.BeginReceive(mBuffer, mOffset, 5000 + mOffset, SocketFlags.None, new AsyncCallback(EndReceive), Socket);
            Socket.BeginReceive(mSharedBuffer, 0, 5000, SocketFlags.None, new AsyncCallback(EndReceive), Socket);

            //TryCatch(() =>
            //{
            //});
        }

        public void TryCatch(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                Log.Exception(ex, "GUID: {0}", GUID);
                Disconnect();
            }
        }

        private volatile int mDisconnected = 0;

        private void Disconnect()
        {
            if(mDisconnected == 0)
            {
                Interlocked.Increment(ref mDisconnected);   //For Thread Safe

                OnDisconnect();
                Socket.Shutdown(SocketShutdown.Both);
                Socket.Close();
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
        private void EndReceive(IAsyncResult ar)
        {
            if (mDisconnected != 0) return;
            int iLen = 0;
            try
            {
                iLen = Socket.EndReceive(ar);
            }
            catch (SocketException)
            {
                Disconnect();
                return;
            }

            if (iLen < 0)
            {
                Disconnect();
                return;
            }

            if (iLen == 0)
            {
                BeginReceive();
                return;
            }

            Append(mSharedBuffer, 0, iLen);

            while(true)
            {
                if (mCursor < 7)
                {
                    break;
                }

                BitConverter.ToInt32(mBuffer, 0);
                int size = BitConverter.ToInt32(mBuffer, 4);

                if (size > iLen)     //받은 데이터의 길이보다 패킷의 길이가 더 길면 뭔가 이상한거임
                {
                    //mOffset = iLen; //이전에 받은 데이터들에 이어서 받기 위해
                    BeginReceive();
                    return;
                }

                byte[] data = new byte[iLen];
                Buffer.BlockCopy(mBuffer, 0, data, 0, size);

                InPacket iPacket = new InPacket(data);

                mCursor -= size;

                int iProtocol = iPacket.ReadInt();
                iPacket.ReadInt();//size pass
                WorkerThread.AddQueue(() => OnPacket(iProtocol, iPacket));  //리시브랑 작업처리 스레드를 나눔

            }
            BeginReceive();
        }

        public void Append(byte[] pBuffer) => Append(pBuffer, 0, pBuffer.Length);
        
        private int mCursor = 0;

        public void Append(byte[] pBuffer, int pStart, int pLength)
        {
            try
            {
                if (mBuffer.Length - mCursor < pLength)
                {
                    int newSize = mBuffer.Length * 2;
                    while (newSize < mCursor + pLength) newSize *= 2;
                    Array.Resize(ref mBuffer, newSize);
                }
                Buffer.BlockCopy(pBuffer, pStart, mBuffer, mCursor, pLength);
                mCursor += pLength;
            }
            catch
            {

            }
        }
        public void Send(OutPacket oPacket)
        {
            if (mDisconnected != 0) return;

            byte[] data = oPacket.ToArray();
            Socket.Send(data);
            oPacket.Dispose();
        }
    }
}
