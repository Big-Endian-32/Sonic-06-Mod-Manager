using System.Windows.Forms;
using SonicNextModManager.Services;

namespace SonicNextModManager.UI.Dialogs
{
    public class DirectoryQueries
    {
        public static string BasicDirectoryQuery(string title)
        {
            FolderBrowserDialog fbd = new()
            {
                Description = title,
                UseDescriptionForTitle = true,
            };

            if (fbd.ShowDialog() == DialogResult.OK)
                return fbd.SelectedPath;

            return string.Empty;
        }

        public static string QueryModsDirectory()
            => BasicDirectoryQuery(LocaleService.Localise("Query_ModsDirectory"));
    }
}
