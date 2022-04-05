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
        public string nickName { get; private set; }
        public MapBase Map { get; private set; } = null;

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
                       case CTPK_DISCONNECT:
                           OnDisconnectPrepare();
                           break;
                       default:
                           Log.Warning("Unknown Packet: {0}", protocolID);
                           break;
                   }
               });
        }

        private void OnDisconnectPrepare()
        {
            try
            {
                if(!Map.Equals(null))
                {
                    using( OutPacket o = new OutPacket( STPK_DISCONNECT ) )
                    {
                        o.Write( nickName );
                        SendToMap( o, true );
                    }

                    Map.RemoveClient( this );
                }
            }
            catch (Exception e)
            {
                Log.Exception( e );
            }
        }

        public void SendToMap(OutPacket o, bool IncludeMe = false)
        {
            if(!Map.Equals(null))
            {
                foreach(var i in Map.GetClients())
                {
                    i.Send( o );
                }
            }
        }

        private void OnMoveMap(InPacket iPacket)
        {
            int MapIDX = iPacket.ReadInt();
            OutPacket o = new OutPacket(STPK_MOVE_MAP);
            
            MapBase Map = MapBase.Maps[MapIDX];

            if(Map.MaxClient <= Map.NowClient)
            {
                o.Write(false); //입장 실패
            }
            else
            {
                o.Write(true);  //입장 성공
                var Clients = Map.GetClients();
                o.Write(Clients.Count);

                foreach (var i in Clients)
                {
                    o.Write(i.nickName);    //현재 맵에 있는 클라이언트들의 이름 전송.
                }
            }
            Send(o);
        }

        private void OnHello(InPacket iPacket)
        {
            string ID = iPacket.ReadString();
            string PWD = iPacket.ReadString();

            Log.Info("ID: {0} PWD: {1} GUID: {2}", ID, PWD, GUID);

            //int accountIDX = Database.Database.Select<int>("accountDB", "userID = '{0}' AND userPWD = '{1}'", ID, PWD);
            
            OutPacket o = new OutPacket(STPK_HELLO);

            //if (accountIDX == -1)
            //{
            //    o.Write(false);
            //    o.Write("Reason: ");
            //}
            //else
            {
                o.Write(true);
                o.Write(GUID);
            }

            Send(o);
        }
    }
}
