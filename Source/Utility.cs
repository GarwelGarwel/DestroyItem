namespace DestroyItem
{
    enum LogLevel { Message = 0, Warning, Error };

    class Utility
    {
        internal static void Log(string message, LogLevel logLevel = LogLevel.Message)
        {
            message = $"[DestroyItem] {message}";
            switch (logLevel)
            {
                case LogLevel.Message:
                    //if (Settings.DebugLogging)
                        Verse.Log.Message(message);
                    break;

                case LogLevel.Warning:
                    Verse.Log.Warning(message);
                    break;

                case LogLevel.Error:
                    Verse.Log.Error(message);
                    break;
            }
        }
    }
}
