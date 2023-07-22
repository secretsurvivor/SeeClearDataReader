using Microsoft.Data.Sqlite;
using Shared.Objects.Logging;

namespace Shared
{
	public class Logger
	{
		private static ILogController internalController = new DefaultFileLogController();
		private static Queue<LogMessage> _messages_to_write = new();

		private static Task WriteToFile;

		private static void CreateTask()
		{
			WriteToFile = new Task(() => {
				if (_messages_to_write.Count > 0)
				{
					using(ILogWriter stream = internalController.GetOutputStream())
					{
						while (_messages_to_write.Count > 0)
						{
							LogMessage LogMsg = _messages_to_write.Dequeue();

							if (LogMsg.Message != "")
								stream.Write(LogMsg);

							stream.Flush();
						}
					}
				}
			});
		}

		static Logger()
		{
			internalController.PreSetup();

			CreateTask();
		}

		public static void SetLoggingController(ILogController controller)
		{
			internalController = controller;

			internalController.PreSetup();
		}

		private static void AddLog(LogType type, string str)
		{
			LogMessage LogMsg = new()
			{
				Type = type,
				Message = str
			};

			_messages_to_write.Enqueue(LogMsg);

			switch (WriteToFile.Status)
			{
				case TaskStatus.Created:
					WriteToFile.Start();
					break;
				case TaskStatus.RanToCompletion:
					CreateTask();
					WriteToFile.Start();
					break;
			}
		}

		public static void Print(string str)
		{
			AddLog(LogType.INFO, str);
		}

		public static void Error(string str)
		{
			AddLog(LogType.ERROR, str);
		}

		public static void ServerInfo(string str)
		{
			AddLog(LogType.SERVER, str);
		}

		public static void SqlDump(SqliteDataReader reader)
		{
			string columnNames = "";

			for (int i = 0; i < reader.VisibleFieldCount; i++)
			{
				if (i == 0)
					columnNames += "" + reader.GetName(i);
				else
					columnNames += " | " + reader.GetName(i);
			}

			AddLog(LogType.SQLITE, columnNames);

			while (reader.Read())
			{
				string rowData = "";

				for (int i = 0; i < reader.VisibleFieldCount; i++)
				{
					if (reader.IsDBNull(i))
					{
						if (i == 0)
							rowData += "NULL";
						else
							rowData += " | NULL";
					}
					else
					{
						if (i == 0)
							rowData += "" + reader.GetString(i);
						else
							rowData += " | " + reader.GetString(i);
					}
				}

				AddLog(LogType.SQLITE, rowData);
			}
		}

		#region Print Overloads
		public static void Print(string header, string str)
		{
			Print(header + ": " + str);
		}

		public static void Print(string header, object obj)
		{
			Print(header, obj.ToString() ?? "");
		}
		#endregion

		#region Error Overloads
		public static void Error(string className, string functionName, string msg)
		{
			Error(className + " - " + functionName + " - " + msg);
		}

		public static void Error(string className, string functionName, Exception ex)
		{
			Error(className, functionName, ex.Message + ex.InnerException ?? "" + System.Environment.NewLine + ex.StackTrace);
		}
		#endregion
	}
}