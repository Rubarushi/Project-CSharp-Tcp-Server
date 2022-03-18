using Common;
using Common.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class Client : Session
    {
        public Client(Socket Socket) : base()
        {
            SetSocket(Socket);
        }

        public void Hello()
        {
            Log.Info("Send Hello");
            OutPacket o = new OutPacket(CTPK_HELLO);
            o.Write("ID");
            o.Write("PW");

            Send(o);
        }

        public void MoveMap(int MapIDX)
        {
            OutPacket o = new OutPacket(CTPK_MOVE_MAP);
            o.Write(MapIDX);
            Send(o);
        }

        public override void OnDisconnect()
        {

        }

        public override void OnPacket(int protocolID, InPacket iPacket)
        {

        }
    }
}
