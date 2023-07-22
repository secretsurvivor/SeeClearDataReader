using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Objects.Logging
{
    public struct LogMessage
	{
		public LogType Type;
		public string Message;
		public string BuiltMessage
		{
			get
			{
				string output;

				switch (Type)
				{
					case LogType.ERROR:
						output = "[ERROR]";
						break;
					case LogType.SQLITE:
						output = "[SQLITE]";
						break;
					case LogType.SERVER:
						output = "[SERVER]";
						break;
					case LogType.INFO:
					default:
						output = "[INFO]";
						break;
				}

				output += "[" + DateTime.Now.ToShortTimeString() + "]";

				output += " :: " + Message;

				return output;
			}
		}
	}
	public enum LogType
	{
		INFO,
		ERROR,
		SQLITE,
		SERVER
	}
}
