namespace LocalStreamingService
{
	public class StreamingConst
	{
		public static readonly int URLTokenExpirationTime;

		public static readonly string ServiceName;

		static StreamingConst()
		{
			URLTokenExpirationTime = 10;
			ServiceName = "Connor's Netflix";
		}
	}
}
