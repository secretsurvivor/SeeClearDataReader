using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Objects.Logging
{
    public interface ILogController
    {
        public ILogWriter GetOutputStream();
        public void PreSetup();
    }

    public interface ILogWriter : IDisposable
    {
        public void Write(LogMessage message);
        public void Flush();
    }

    public class DefaultFileLogController : ILogController // Default Internal Logging Controller
    {
        public string Path { get; set; } = Directory.GetCurrentDirectory() + "\\Streaming Logs\\";

        public ILogWriter GetOutputStream()
        {
            return new FileLogWriter(Path + string.Format("Log-{0}.txt", DateTime.Now.ToShortDateString().Replace("/", "-")));
        }

        public void PreSetup()
        {
            Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\Streaming Logs");
        }
    }

    public class FileLogWriter : ILogWriter   // Default Log Writer
    {
        StreamWriter writer;

        public FileLogWriter(string path)
        {
            writer = File.AppendText(path);
        }

        public void Dispose()
        {
            ((IDisposable)writer).Dispose();
        }

        public void Write(LogMessage message)
        {
            writer.WriteLine(message.BuiltMessage);
        }

        public void Flush()
        {
            writer.Flush();
        }
    }
}
