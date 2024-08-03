using SonicNextModManager.Helpers;
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

        private void OK_Click(object in_sender, RoutedEventArgs in_args)
            => Close();

        private void Language_SelectionChanged(object in_sender, SelectionChangedEventArgs in_args)
            => SonicNextModManager.Language.UpdateCultureResources();

        private void Path_ModsDirectory_Browse(object in_sender, EventArgs in_args)
            => PropertyHelper.SetStringWithNullCheck(s => App.Settings.Path_ModsDirectory = s, DirectoryQueries.QueryModsDirectory());

        private void Path_GameExecutable_Browse(object in_sender, EventArgs in_args)
            => PropertyHelper.SetStringWithNullCheck(s => App.Settings.Path_GameExecutable = s, FileQueries.QueryGameExecutable());

        private void Path_EmulatorExecutable_Browse(object in_sender, EventArgs in_args)
            => PropertyHelper.SetStringWithNullCheck(s => App.Settings.Path_EmulatorExecutable = s, FileQueries.QueryEmulatorExecutable());
    }
}
