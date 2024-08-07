using SonicNextModManager.UI.Components;
using SonicNextModManager.UI.Dialogs;

namespace SonicNextModManager.UI
{
    /// <summary>
    /// Interaction logic for DebugWindow.xaml
    /// </summary>
    public partial class DebugWindow : ImmersiveWindow
    {
        public DebugWindow()
        {
            InitializeComponent();
        }

        private void NextMessageBox_Test_OnClick(object sender, RoutedEventArgs e)
        {
            if (sender == NextMessageBox_None)
            {
                NextMessageBox.Show
                (
                     "This is a message.",
                     "This is a caption.",
                     ENextMessageBoxButton.OK
                );
            }
            else if (sender == NextMessageBox_Error)
            {
                NextMessageBox.Show
                (
                     "This is an error message.",
                     "This is a caption.",
                     ENextMessageBoxButton.AbortRetryIgnore,
                     ENextMessageBoxIcon.Error
                );
            }
            else if (sender == NextMessageBox_Question)
            {
                NextMessageBox.Show
                (
                     "This is a question?",
                     "This is a caption.",
                     ENextMessageBoxButton.YesNo,
                     ENextMessageBoxIcon.Question
                );
            }
            else if (sender == NextMessageBox_Warning)
            {
                NextMessageBox.Show
                (
                     "This is a warning.",
                     "This is a caption.",
                     ENextMessageBoxButton.YesNoCancel,
                     ENextMessageBoxIcon.Warning
                );
            }
            else if (sender == NextMessageBox_Information)
            {
                NextMessageBox.Show
                (
                     "This is some information.",
                     "This is a caption.",
                     ENextMessageBoxButton.OK,
                     ENextMessageBoxIcon.Information
                );
            }
        }

        private void NextTaskDialog_Test_OnClick(object sender, RoutedEventArgs e)
        {
            var result = new NextTaskDialog();
            result.AddTask("Install mod from archive", "Extracts a mod from an archive to your mods directory.", "archive", () => NextMessageBox.Show("installing from archive"));
            result.AddTask("Install mod from folder", "Copies a mod from a folder to your mods directory.", "folder", () => NextMessageBox.Show("installing from folder"));
            result.AddTask("Create a mod", "Write information about your mod before creation.", "edit", () => NextMessageBox.Show("creating a mod"));
            result.Show("What would you like to do today?");
        }

        private void SideMenu_MouseEnter(object sender, MouseEventArgs e)
        {
            SideMenu.Width = 240;
        }

        private void ProgressDialog_Test_OnClick(object sender, RoutedEventArgs e)
        {
            new ProgressDialog().ShowDialog();
        }
    }
}
