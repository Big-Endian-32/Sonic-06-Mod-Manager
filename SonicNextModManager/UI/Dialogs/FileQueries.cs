using Microsoft.Win32;

namespace SonicNextModManager.UI.Dialogs
{
    public class FileQueries
    {
        public static string BasicFileQuery(string title, Dictionary<string, string> in_filters)
        {
            OpenFileDialog ofd = new()
            {
                Filter = new FilterBuilder(in_filters).Result,
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
