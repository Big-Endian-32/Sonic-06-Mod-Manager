using SonicNextModManager.Logger.Interfaces;
using System.Diagnostics;

namespace SonicNextModManager.Logger.Handlers
{
    internal class ConsoleLogger : ILogger
    {
        public void Log(object in_message, ELogLevel in_logLevel, string in_caller)
        {
            WriteLine(string.IsNullOrEmpty(in_caller) ? in_message : $"[{in_caller}] {in_message}", in_logLevel);
        }

        public void Write(object in_message, ELogLevel in_logLevel)
        {
            var oldColour = Console.ForegroundColor;

            switch (in_logLevel)
            {
                case ELogLevel.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;

                case ELogLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;

                case ELogLevel.Utility:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
            }

            Console.Write(in_message);
            Debug.Write(in_message);

            Console.ForegroundColor = oldColour;
        }

        public void WriteLine(object in_message, ELogLevel in_logLevel)
        {
            Write(in_message.ToString().Replace("\n", "\r\n") + "\r\n", in_logLevel);
        }
    }
}
