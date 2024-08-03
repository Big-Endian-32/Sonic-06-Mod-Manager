using SonicNextModManager.Helpers;
using SonicNextModManager.UI.Components;
using SonicNextModManager.UI.Dialogs;

namespace SonicNextModManager.UI
{
    /// <summary>
    /// Interaction logic for Editor.xaml
    /// </summary>
    public partial class Editor : ImmersiveWindow
    {
        protected internal Metadata? Metadata { get; set; } = new();

        protected internal string? Directory { get; set; }

        protected internal bool IsINI { get; private set; } = false;

        public Editor(Metadata? metadata = null)
        {
            InitializeComponent();

            // Set current metadata and enable edit mode.
            if (metadata != null)
            {
                Metadata = metadata;
                Directory = metadata.Path;

                // Set title to display the metadata name.
                Title = LocaleService.Localise("Editor_Editing", metadata.Title);

                // This metadata was converted.
                if (Path.GetExtension(metadata.Path) == ".ini")
                    IsINI = true;
            }

            DataContext = Metadata;
        }

        private void Thumbnail_Browse_Click(object sender, RoutedEventArgs e)
        {
            (Metadata as Mod).Thumbnail = FileQueries.BasicFileQuery
            (
                "Please select a thumbnail...",

                new Dictionary<string, string>
                {
                    { LocaleService.Localise("Filter_Supported"), "*.png; *.jpg; *.jpeg; *.jpe; *.jfif" },
                    { "PNG", "*.png" },
                    { "JPEG", "*.jpg; *.jpeg; *.jpe; *.jfif" }
                }
            );
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            if (Metadata is Mod mod)
            {
                string thumbnailDest = Path.Combine(Directory, $"thumbnail{Path.GetExtension(mod.Thumbnail)}");

                // Copy new thumbnail to the mod directory.
                if (!string.IsNullOrEmpty(mod.Thumbnail) && mod.Thumbnail != thumbnailDest)
                    File.Copy(mod.Thumbnail, thumbnailDest, true);

                // Write metadata as JSON - rename if necessary.
                mod.Write(IsINI ? StringHelper.ReplaceFilename(mod.Path, "mod.json") : mod.Path);

                // Back up original INI.
                if (IsINI)
                    File.Move(mod.Path, $"{mod.Path}.bak");
            }
            else if (Metadata is Patch patch)
            {
                patch.Write();
            }

            Close();
        }
    }
}
