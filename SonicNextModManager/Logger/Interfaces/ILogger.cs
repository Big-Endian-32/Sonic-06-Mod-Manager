using System.Runtime.CompilerServices;

namespace SonicNextModManager.Logger.Interfaces
{
    internal interface ILogger
    {
        public void Log(object in_message, ELogLevel in_logLevel, [CallerMemberName] string in_caller = null);
        public void Write(object in_message, ELogLevel in_logLevel);
        public void WriteLine(object in_message, ELogLevel in_logLevel);
    }
}
