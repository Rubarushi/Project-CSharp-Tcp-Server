namespace Common
{
	public class Protocol
	{
		public const int CTPK_HELLO = 0x0001;
		public const int STPK_HELLO = 0x2001;

		public const int CTPK_DISCONNECT = 0x0003;
		public const int STPK_DISCONNECT = 0x2003;

		public const int CTPK_MOVE_MAP = 0x0002;
		public const int STPK_MOVE_MAP = 0x2002;
		public const int STPK_OUT_MAP =  0x2015;
	}
}
