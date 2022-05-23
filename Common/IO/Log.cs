using Common.IO;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Common
{
	public class ConsoleContents
	{
		public LogLevel LogLevel { get; set; } = LogLevel.INFO;
		public string Message = string.Empty;
		public bool WriteLine = true;
		public DateTime WriteTime = DateTime.Now;
	}

	[Flags]
	public enum LogLevel : int
	{
		EMERGENCY,
		ALERT,
		CRITICAL,
		ERROR,
		WARNING,
		NOTICE,
		INFO,
		DEBUG,
	}

	public static class Log
	{
		private static BlockingCollection<ConsoleContents> Collections = new BlockingCollection<ConsoleContents>();

		public static void StartLogger()
		{
			string threadName = "";
			string message = "";
			ConsoleColor color = ConsoleColor.White;

			string prefix = "";
			Task.Factory.StartNew( () =>
			{
				foreach( ConsoleContents CC in Collections.GetConsumingEnumerable() )
				{
					if( Thread.CurrentThread.Name == null )
					{
						threadName = "Server thread";
					}
					else
					{
						threadName = Thread.CurrentThread.Name;
					}

					switch( CC.LogLevel )
					{
						case LogLevel.CRITICAL:
							color = ConsoleColor.Red;
							prefix = "CRITICAL";
							break;
						case LogLevel.ALERT:
							color = ConsoleColor.DarkRed;
							prefix = "ALERT";
							break;
						case LogLevel.ERROR:
							color = ConsoleColor.DarkRed;
							prefix = "ERROR";
							break;
						case LogLevel.EMERGENCY:
							prefix = "EMERGENCY";
							color = ConsoleColor.DarkRed;
							break;
						case LogLevel.WARNING:
							prefix = "WARNING";
							color = ConsoleColor.Yellow;
							break;
						case LogLevel.NOTICE:
							prefix = "NOTICE";
							color = ConsoleColor.Cyan;
							break;
						case LogLevel.INFO:
							prefix = "INFO";
							color = ConsoleColor.White;
							break;
						default:
							prefix = "INFO";
							color = ConsoleColor.White;
							break;
					}
					message = CC.Message;
					Console.ForegroundColor = ConsoleColor.Cyan;
					Console.Write( CC.WriteTime.ToString( "[HH:mm:ss] " ) );
					Console.ForegroundColor = color;
					Console.Write( $"[{threadName}/{prefix}]: " );
					Console.ForegroundColor = ConsoleColor.Gray;
					Console.WriteLine( message );
				}
			}, TaskCreationOptions.LongRunning );
		}

		public static void Exception( Exception ex, string description = null, params object[] args )
		{
			ConsoleContents CC = new ConsoleContents
			{
				LogLevel = LogLevel.ALERT,
				WriteLine = true,
				WriteTime = DateTime.Now,
			};

			if( description != null )
			{
				CC.Message = ex.ToString() + string.Format( description, args );
			}
			else
			{
				CC.Message = ex.ToString();
			}
			AsyncStream.Write( "Exceptions.txt", CC.Message );
		}

		public static void Info( string format, params object[] args )
		{
			ConsoleContents CC = new ConsoleContents
			{
				LogLevel = LogLevel.INFO,
				Message = string.Format(format, args),
				WriteLine = true,
				WriteTime = DateTime.Now,
			};
			Collections.Add( CC );
		}

		public static void Warning( string format, params object[] args )
		{
			ConsoleContents CC = new ConsoleContents
			{
				LogLevel = LogLevel.WARNING,
				Message = string.Format(format, args),
				WriteLine = true,
				WriteTime = DateTime.Now,
			};
			Collections.Add( CC );
		}

		public static void Error( string format, params object[] args )
		{
			ConsoleContents CC = new ConsoleContents
			{
				LogLevel = LogLevel.ERROR,
				Message = string.Format(format, args),
				WriteLine = true,
				WriteTime = DateTime.Now,
			};
			Collections.Add( CC );
		}

		public static void Debug( string format, params object[] args )
		{
			ConsoleContents CC = new ConsoleContents
			{
				LogLevel = LogLevel.DEBUG,
				Message = string.Format(format, args),
				WriteLine = true,
				WriteTime = DateTime.Now,
			};
			Collections.Add( CC );
		}
	}
}
