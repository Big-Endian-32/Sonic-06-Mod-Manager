using SonicNextModManager.Logger;
using SonicNextModManager.Logger.Handlers;

namespace SonicNextModManager.UI.Pages
{
    /// <summary>
    /// Interaction logic for DebugPage.xaml
    /// </summary>
    public partial class DebugPage : UserControl
    {
        public DebugPage()
        {
            InitializeComponent();

            DebugLog.ItemsSource = FrontendLogger.Logs;
        }

        private void DebugLog_Copy_Click(object in_sender, RoutedEventArgs in_args)
        {
            if (((MenuItem)in_sender).DataContext is not Log out_log)
                return;

            Clipboard.SetText(out_log.Message);
        }
    }
}
