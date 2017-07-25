using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace ContactManager.Utils
{
    class FileAppender
    {
        string _logFile;
        StreamWriter _writer;

        public void WriteMessage(string message)
        {
            if (_writer == null)
            {
                try
                {
                    var folder = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                    _logFile = Path.Combine(folder, "message.log");
                    if (!File.Exists(_logFile))
                        File.Create(_logFile);
                    _writer = _writer ?? new StreamWriter(_logFile, true, Encoding.UTF8);
                }
                catch (Exception ex)
                {
                    if (_writer != null)
                    {
                        _writer.Flush();
                        _writer.Close();
                        _writer.Dispose();
                        _writer = null;
                    }
                    throw new Exception(string.Format("Error openning the log addin {0}!", _logFile), ex);
                }
            }

            _writer.WriteLine(message);
            _writer.Flush();
        }
    }
}
