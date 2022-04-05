using System.Collections.Generic;

namespace Server.Map
{
    public class MapBase
    {
        public int Index { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public int MaxClient { get; set; } = 10;
        public int NowClient => JoinedClients.Count;

        public SynchronizedCollection<Client> JoinedClients = new SynchronizedCollection<Client>();

        public static List<MapBase> Maps = new List<MapBase>();

        public void AddClient(Client client)
        {
            JoinedClients.Add(client);
        }

        public void RemoveClient(Client c)
        {
            if (JoinedClients.Contains(c))
            {
                JoinedClients.Remove(c);
            }
        }
        public SynchronizedCollection<Client> GetClients()
        {
            return JoinedClients;
        }
    }
}
