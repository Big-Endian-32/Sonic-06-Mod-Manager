using SonicNextModManager.UI.Components;
using SonicNextModManager.UI.Dialogs;

namespace SonicNextModManager.UI
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : ImmersiveWindow
    {
        public Settings()
        {
            InitializeComponent();

            // Create credits list.
            foreach (var credits in SonicNextModManager.Credits.Parse())
                Credits.Children.Add(new CreditsPane(credits));
        }

        private void OK_Click(object sender, RoutedEventArgs e)
            => Close();

        private void Language_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => SonicNextModManager.Language.UpdateCultureResources();

        private void Path_ModsDirectory_Browse(object sender, EventArgs e)
            => PropertyExtensions.SetStringWithNullCheck(s => App.Settings.Path_ModsDirectory = s, DirectoryQueries.QueryModsDirectory());

        private void Path_GameExecutable_Browse(object sender, EventArgs e)
            => PropertyExtensions.SetStringWithNullCheck(s => App.Settings.Path_GameExecutable = s, FileQueries.QueryGameExecutable());

        private void Path_EmulatorExecutable_Browse(object sender, EventArgs e)
            => PropertyExtensions.SetStringWithNullCheck(s => App.Settings.Path_EmulatorExecutable = s, FileQueries.QueryEmulatorExecutable());
    }
}
