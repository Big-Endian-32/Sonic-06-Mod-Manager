using System.ComponentModel;

namespace SonicNextModManager.Logger
{
    internal class Log(string in_message, ELogLevel in_logLevel) : INotifyPropertyChanged
    {
        public string Message { get; set; } = in_message;

        public ELogLevel LogLevel { get; set; } = in_logLevel;

        public ulong RepeatCount { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public override bool Equals(object? in_obj)
        {
            if (in_obj!.GetType().Equals(typeof(Log)))
            {
                var log = (Log)in_obj;

                return Message  == log.Message &&
                       LogLevel == log.LogLevel;
            }

            return false;
        }
    }
}
