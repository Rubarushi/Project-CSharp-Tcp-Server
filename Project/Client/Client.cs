using Common;
using Common.Network;
using Server.Map;
using System.Net.Sockets;

namespace Server
{
    public class Client : Session
    {
        public string nickName { get; private set; }

        public Client(Socket Socket) : base()
        {
            SetSocket(Socket);
        }

        public override void OnDisconnect()
        {
            Log.Info("Disconnected: {0}", IP);
        }

        public override void OnPacket(int protocolID, InPacket iPacket)
        {
            TryCatch(() =>
               {
                   switch (protocolID)
                   {
                       case CTPK_HELLO:
                           OnHello(iPacket);
                           break;
                       case CTPK_MOVE_MAP:
                           OnMoveMap(iPacket);
                           break;
                       default:
                           Log.Warning("Unknown Packet: {0}", protocolID);
                           break;
                   }
               });
        }

        private void OnMoveMap(InPacket iPacket)
        {
            int MapIDX = iPacket.ReadInt();
            OutPacket o = new OutPacket(STPK_MOVE_MAP);
            
            MapBase Map = MapBase.Maps[MapIDX];

            if(Map.MaxClient <= Map.NowClient)
            {
                o.Write(false);
            }
            else
            {
                o.Write(true);
                o.Write(Map.JoinedClients.Count);

                foreach (var i in Map.JoinedClients)
                {
                    o.Write(i.nickName);
                }
            }
            Send(o);
        }

        private void OnHello(InPacket iPacket)
        {
            string ID = iPacket.ReadString();
            string PWD = iPacket.ReadString();

            Log.Info("ID: {0} PWD: {0}", ID, PWD);
            OutPacket o = new OutPacket(STPK_HELLO);
            o.Write(true);
            o.Write(GUID);
            Send(o);
        }
    }
}
