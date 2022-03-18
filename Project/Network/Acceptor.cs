using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server.Network
{
    public class Acceptor
    {
        private TcpListener Listener;
        public SynchronizedCollection<Client> m_vClient = new SynchronizedCollection<Client>();
        private int port = 0;

        public Acceptor(int port)
        {
            this.port = port;
            Listener = new TcpListener(System.Net.IPAddress.Any, port);
        }

        public void StartWork()
        {
            Log.Info("New Acceptor Ready on {0}", port);
            Listener.Start();
            Listener.BeginAcceptSocket(new AsyncCallback(OnAccept), null);
        }

        private void OnAccept(IAsyncResult ar)
        {
            if (Server.m_bStopped)
            {
                Listener.Stop();
                return;
            }

            Socket Socket = Listener.EndAcceptSocket(ar);

            Log.Info("New Client Joined {0}", Socket.RemoteEndPoint.ToString());

            Client client = new Client(Socket);
            client.BeginReceive();

            m_vClient.Add(client);

            Listener.BeginAcceptSocket(new AsyncCallback(OnAccept), null);
        }
    }
}
