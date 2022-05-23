using Common;
using Common.IO;
using Common.Network;
using Server.Network;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server
{
	public class Server
	{
		public static bool m_bStopped = false;
		public static Acceptor Acceptor;

		private static void Main( string[] args )
		{
			WorkerThread.StartWork();
			Log.StartLogger();
			AsyncStream.Start();

			Acceptor = new Acceptor( 14009 );
			Acceptor.StartWork();

			for( int i = 0; i < 100; i++ )
			{
				Map.MapBase.Maps.Add( new Map.MapBase() );
			}


			Task Commander = Task.Factory.StartNew(() =>
			{
				while (m_bStopped == false)
				{
					string cmd = Console.ReadLine();
					string[] cmds = cmd.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

					if (cmds.Length < 1 || !cmds[0].StartsWith("/"))
					{
						continue;
					}

					cmds[0] = cmds[0].Replace("/", "");

					switch (cmds[0])
					{
						case "stop":
						{
							m_bStopped = true;
							lock(Acceptor._lock)
							{
								for(int i = 0; i < Acceptor.m_vClient.Count; i++ )
								{
									OutPacket o = new OutPacket(0x2003);
									Acceptor.m_vClient[i].Send(o);
									//Acceptor.m_vClient[i].Disconnect();		//클라이언트가 능동적으로 끊게
								}
							}
							return;
						}
						case "sessions":
						{
							MessageBox.Show(string.Format("{0} Connecting", Acceptor.m_vClient.Count));
						}
						break;
						case "list":
						{
							StringBuilder sb = new StringBuilder();
							int ItemCountPerLine = 0;

							System.Collections.Generic.List<Client>.Enumerator it = Acceptor.m_vClient.GetEnumerator();
							while(it.MoveNext())
							{
								if(ItemCountPerLine == 5)
								{
									sb.Append("\r\n");
									ItemCountPerLine = 0;
								}
								ItemCountPerLine++;
								sb.Append(string.Format("{0,30}", it.Current.GUID));
								sb.Append(" | ");
							}
							MessageBox.Show(sb.ToString());
						}
						break;
						default:
							Log.Warning("Unknown Command: {0}", cmd);
							break;
					}
				}
			});

			Commander.Wait();
			Console.WriteLine( "Server Closed" );
		}
	}
}
