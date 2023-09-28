public static class Logger
{
    public static LogLevel logLevel = LogLevel.Error;

    public static void SetLogLevel(LogLevel level)
    {
        logLevel = level;
    }

    public static void Debug(object msg)
    {
        if( logLevel == LogLevel.Debug)
        {
            Console.WriteLine("DEBUG: " + msg);
        }
    }

    public static void Warning(object msg)
    {
        if( logLevel >= LogLevel.Warning)
        {
            Console.WriteLine("WARN: " + msg);
        }
    }

    public static void ERROR(object msg)
    {
            Console.WriteLine("ERROR: " + msg);
    }

    public enum LogLevel
    {
        Error,
        Warning,
        Debug,
    }
}