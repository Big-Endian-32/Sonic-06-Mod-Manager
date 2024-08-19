using SonicNextModManager.Helpers;
using SonicNextModManager.Metadata;
using SonicNextModManager.UI.Components;
using SonicNextModManager.UI.Dialogs;

namespace SonicNextModManager.UI.Pages
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : UserControl
    {
        public SettingsPage()
        {
            InitializeComponent();

            UpdateXeniaFrontendVisibility();

            // Create credits list.
            foreach (var credits in SonicNextModManager.Credits.Parse())
                Credits.Children.Add(new CreditsPane(credits));
        }

        private void Language_SelectionChanged(object in_sender, SelectionChangedEventArgs in_args)
        {
            SonicNextModManager.Language.UpdateCultureResources();
        }

        private void Path_ModsDirectory_Browse(object in_sender, EventArgs in_args)
        {
            PropertyHelper.SetStringWithNullCheck(s => App.Settings.Path_ModsDirectory = s, DirectoryQueries.QueryModsDirectory());
        }

        private void Path_GameExecutable_Browse(object in_sender, EventArgs in_args)
        {
            PropertyHelper.SetStringWithNullCheck(s => App.Settings.Path_GameExecutable = s, FileQueries.QueryGameExecutable());

            UpdateXeniaFrontendVisibility();
        }

        private void Path_EmulatorExecutable_Browse(object in_sender, EventArgs in_args)
        {
            PropertyHelper.SetStringWithNullCheck(s => App.Settings.Path_EmulatorExecutable = s, FileQueries.QueryEmulatorExecutable());
        }

        private void UpdateXeniaFrontendVisibility()
        {
            var isXbox = App.GetCurrentPlatform() == EPlatform.Xbox;

            XeniaFrontend.Visibility = isXbox
                ? Visibility.Visible
                : Visibility.Collapsed;

            RPCS3Frontend.Visibility = isXbox
                ? Visibility.Collapsed
                : Visibility.Visible;
        }
    }
}
