using Shared;
using System.Net;
using System.Net.Sockets;

namespace LocalStreamingService.Objects
{
	public class ServerPort
	{
		private static bool enabled = false;
		public static int Port { get; set; } = 26555;
		private static IPAddress address = IPAddress.Broadcast;
		private static TcpListener listener = null;

		static ServerPort()
		{
			

			if (enabled)
			{
				listener = new TcpListener(address, Port);

				listener.Start();

				try
				{
					while (true)
					{
						Logger.ServerInfo("Waiting for Client");

						TcpClient client = listener.AcceptTcpClient();

						Logger.ServerInfo("Client Connected");


					}
				}
				catch (Exception ex)
				{
					Logger.Error("ServerPort", "Static", ex);
				}
				finally
				{
					listener.Stop();
				}
			}
		}

	}
}
