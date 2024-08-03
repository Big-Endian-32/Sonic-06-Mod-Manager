using System.Windows.Forms;

namespace SonicNextModManager.UI.Dialogs
{
    public class DirectoryQueries
    {
        public static string BasicDirectoryQuery(string in_title)
        {
            FolderBrowserDialog fbd = new()
            {
                Description = in_title,
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
