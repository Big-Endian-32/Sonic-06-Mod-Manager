using Microsoft.Win32;
using SonicNextModManager.Services;

namespace SonicNextModManager.UI.Dialogs
{
    public class FileQueries
    {
        public static string BasicFileQuery(string title, Dictionary<string, string> filters)
        {
            OpenFileDialog ofd = new()
            {
                Filter = new FilterBuilder(filters).Result,
                Title = title
            };

            if (ofd.ShowDialog() == true)
                return ofd.FileName;

            return string.Empty;
        }

        public static string QueryGameExecutable()
        {
            return BasicFileQuery
            (
                LocaleService.Localise("Query_GameExecutable"),

                new Dictionary<string, string>
                {
                    { LocaleService.Localise("Filter_Supported"), "*.xex; *.bin" },
                    { LocaleService.Localise("Filter_Xbox"), "*.xex" },
                    { LocaleService.Localise("Filter_PlayStation"), "*.bin" }
                }
            );
        }

        public static string QueryEmulatorExecutable()
        {
            return BasicFileQuery
            (
                LocaleService.Localise("Query_EmulatorExecutable"),

                new Dictionary<string, string>
                {
                    { LocaleService.Localise("Filter_Supported"), "*.exe" }
                }
            );
        }
    }
}
