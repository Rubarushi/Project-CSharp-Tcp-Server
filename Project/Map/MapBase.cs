using System.Collections.Generic;

namespace Server.Map
{
	public class MapBase
	{
		public int Index { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }

		public int MaxClient { get; set; } = 1500;
		public int NowClient => JoinedClients.Count;

		public List<Client> JoinedClients = new List<Client>();

		public static List<MapBase> Maps = new List<MapBase>();

		public object _lock = new object();
		public void AddClient( Client client )
		{
			lock( _lock )
			{
				JoinedClients.Add( client );
			}
		}

		public void RemoveClient( Client c )
		{
			lock( _lock )
			{
				if( JoinedClients.Contains( c ) )
				{
					JoinedClients.Remove( c );
				}
			}
		}
		public List<Client> GetClients() => JoinedClients;
	}
}
