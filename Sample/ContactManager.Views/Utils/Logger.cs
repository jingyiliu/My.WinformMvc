
namespace ContactManager.Utils
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class Logger
    {
        static readonly FileAppender _log = new FileAppender();

        public static void Log(string message)
        {
            _log.WriteMessage(message);
        }
    }
}
