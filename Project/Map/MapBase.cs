using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server.Map
{
    public class MapBase
    {
        public int Index { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public int MaxClient { get; set; } = 10;
        public int NowClient => JoinedClients.Count;

        public BlockingCollection<Client> JoinedClients = new BlockingCollection<Client>();

        public static List<MapBase> Maps = new List<MapBase>();

        public void AddClient(Client client)
        {
            JoinedClients.Add(client);
        }
    }
}
