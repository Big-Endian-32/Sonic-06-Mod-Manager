using SonicNextModManager.Helpers;
using SonicNextModManager.Metadata;
using SonicNextModManager.UI.Components;
using SonicNextModManager.UI.Dialogs;

namespace SonicNextModManager.UI
{
    /// <summary>
    /// Interaction logic for EditorWindow.xaml
    /// </summary>
    public partial class EditorWindow : ImmersiveWindow
    {
        protected internal MetadataBase? Metadata { get; set; } = new();

        protected internal string? Directory { get; set; }

        protected internal bool IsINI { get; private set; } = false;

        public EditorWindow(MetadataBase? in_metadata = null)
        {
            InitializeComponent();

            // Set current metadata and enable edit mode.
            if (in_metadata != null)
            {
                Metadata = in_metadata;
                Directory = Path.GetDirectoryName(in_metadata.Location);

                // Set title to display the metadata name.
                Title = LocaleService.Localise("Editor_Editing", in_metadata.Title);

                // This metadata was converted.
                if (Path.GetExtension(in_metadata.Location) == ".ini")
                    IsINI = true;
            }

            DataContext = Metadata;
        }

        private void Thumbnail_Browse_Click(object in_sender, RoutedEventArgs in_args)
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

        private void OK_Click(object in_sender, RoutedEventArgs in_args)
        {
            if (Metadata is Mod mod)
            {
                string thumbnailDest = Path.Combine(Directory, $"thumbnail{Path.GetExtension(mod.Thumbnail)}");

                // Copy new thumbnail to the mod directory.
                if (!string.IsNullOrEmpty(mod.Thumbnail) && mod.Thumbnail != thumbnailDest)
                    File.Copy(mod.Thumbnail, thumbnailDest, true);

                // Write metadata as JSON - rename if necessary.
                mod.Write(IsINI ? StringHelper.ReplaceFilename(mod.Location, "mod.json") : mod.Location);

                // Back up original INI.
                if (IsINI)
                    File.Move(mod.Location, $"{mod.Location}.bak");
            }
            else if (Metadata is Patch patch)
            {
                patch.Write();
            }

            Close();
        }
    }
}
