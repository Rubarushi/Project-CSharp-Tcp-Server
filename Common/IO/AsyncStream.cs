using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;

namespace Common.IO
{

	internal class StreamSet
	{
		public string FileName = "";
		public string Contents = "";
		public DateTime WriteTime = default;
	}

	public static class AsyncStream
	{
		private static readonly BlockingCollection<StreamSet> StreamSets = new BlockingCollection<StreamSet>();
		public static void Start()
		{
			Log.Debug( "AsyncStream Start..." );
			Task.Factory.StartNew( () =>
			{
				foreach( StreamSet i in StreamSets.GetConsumingEnumerable() )
				{
					using( StreamWriter w = new StreamWriter( i.FileName, true ) )
					{
						w.WriteLine( i.Contents );
						w.Flush();
					}
				}
			}, TaskCreationOptions.LongRunning );
		}

		public static void Write( string filename, string contents, params object[] args ) => StreamSets.Add( new StreamSet
		{
			FileName = filename,
			Contents = string.Format( contents, args ),
			WriteTime = DateTime.Now
		} );
	}
}
