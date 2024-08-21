using SonicNextModManager.Collections;
using SonicNextModManager.Logger.Interfaces;
using System.Threading.Tasks;

namespace SonicNextModManager.Logger.Handlers
{
    internal class FrontendLogger : ILogger
    {
        public static StackList<Log> Logs = new(100);

        public void Log(object in_message, ELogLevel in_logLevel, string in_caller)
        {
            WriteLine(string.IsNullOrEmpty(in_caller) ? in_message : $"[{in_caller}] {in_message}", in_logLevel);
        }

        public void Write(object in_message, ELogLevel in_logLevel)
        {
            try
            {
                App.Current.Dispatcher.Invoke
                (
                    () =>
                    {
                        var log = new Log(in_message.ToString(), in_logLevel);

                        for (int i = 0; i < Logs.Count; i++)
                        {
                            if (Logs[i].Equals(log))
                            {
                                Logs[i].RepeatCount++;
                                return;
                            }
                        }

                        Logs.Add(log);
                    }
                );
            }
            catch (TaskCanceledException)
            {
                // Ignored...
            }
        }

        public void WriteLine(object in_message, ELogLevel in_logLevel)
        {
            Write(in_message.ToString().Replace("\n", "\r\n") + "\r\n", in_logLevel);
        }
    }
}
