using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Common
{
	public static class WorkerThread
	{
		public static Thread Worker;

		private static List<Action> WorkList = new List<Action>();

		private static object _Lock = new object();

		private static ManualResetEvent Pause = new ManualResetEvent(false);

		public static void StartWork()
		{
			Worker = new Thread( new ThreadStart( QueueWork ) );

			Worker.Start();
		}

		private static bool Stopped = false;

		public static void StopThread()
		{
			lock( _Lock )
			{
				foreach( Action i in WorkList )
				{
					i.Invoke();
				}
				Pause.Set();
				Stopped = true;
			}
		}

		public static void AddQueue( Action action )
		{
			if( null == Worker )
			{
				action.Invoke();
			}
			else
			{
				lock( _Lock )
				{
					WorkList.Add( action );
					Pause.Set();
				}
			}
		}

		private static void QueueWork()
		{
			while( Pause.WaitOne() )
			{
				if( Stopped )
				{
					break;
				}

				lock( _Lock )
				{
					Action work = WorkList.FirstOrDefault();

					if( WorkList.Count != 0 && work != null )
					{
						lock( work )
						{
							WorkList.Remove( work );
							work.Invoke();
						}
					}

					if( WorkList.Count == 0 )
					{
						Pause.Reset();
					}
				}
			}
		}
	}
}