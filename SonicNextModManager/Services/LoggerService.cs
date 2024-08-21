using SonicNextModManager.Logger;
using SonicNextModManager.Logger.Handlers;
using SonicNextModManager.Logger.Interfaces;
using System.Runtime.CompilerServices;

namespace SonicNextModManager.Services
{
    internal static class LoggerService
    {
        private static List<ILogger> _handlers = [new ConsoleLogger(), new FrontendLogger()];

        public static void Add(ILogger in_logger)
        {
            _handlers.Add(in_logger);
        }

        public static bool Remove(ILogger in_logger)
        {
            return _handlers.Remove(in_logger);
        }

        public static void Log(object in_message, ELogLevel in_logLevel, [CallerMemberName] string in_caller = null)
        {
            foreach (var logger in _handlers)
                logger.Log(in_message, in_logLevel, in_caller);
        }

        public static void Log(object in_message, [CallerMemberName] string in_caller = null)
        {
            Log(in_message, ELogLevel.None, in_caller);
        }

        public static void Log(object in_message)
        {
            Log(in_message, string.Empty);
        }

        public static void Utility(object in_message, [CallerMemberName] string in_caller = null)
        {
            Log(in_message, ELogLevel.Utility, in_caller);
        }

        public static void Utility(object in_message)
        {
            Utility(in_message, string.Empty);
        }

        public static void Warning(object in_message, [CallerMemberName] string in_caller = null)
        {
            Log(in_message, ELogLevel.Warning, in_caller);
        }

        public static void Warning(object in_message)
        {
            Warning(in_message, string.Empty);
        }

        public static void Error(object in_message, [CallerMemberName] string in_caller = null)
        {
            Log(in_message, ELogLevel.Error, in_caller);
        }

        public static void Error(object in_message)
        {
            Error(in_message, string.Empty);
        }

        public static void Write(object in_str, ELogLevel in_logLevel)
        {
            foreach (var logger in _handlers)
                logger.Write(in_str, in_logLevel);
        }

        public static void Write(object in_str)
        {
            Write(in_str, ELogLevel.None);
        }

        public static void WriteLine(object in_str, ELogLevel in_logLevel)
        {
            foreach (var logger in _handlers)
                logger.WriteLine(in_str, in_logLevel);
        }

        public static void WriteLine(object in_str)
        {
            WriteLine(in_str, ELogLevel.None);
        }
    }
}
