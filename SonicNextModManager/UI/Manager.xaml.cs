using SonicNextModManager.UI.ViewModel;
using System.Collections.ObjectModel;
using System.Windows.Media.Animation;
using SonicNextModManager.UI.Components;
using SonicNextModManager.UI.Dialogs;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using SonicNextModManager.Metadata;
using SonicNextModManager.Helpers;

namespace SonicNextModManager.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Manager : ImmersiveWindow
    {
        ManagerViewModel ViewModel { get; set; } = new();

        /// <summary>
        /// Property storage for <see cref="InfoDisplayMargin"/>.
        /// </summary>
        internal static readonly DependencyProperty InfoDisplayMarginProperty = DependencyProperty.Register
        (
            nameof(InfoDisplayMargin),
            typeof(Thickness),
            typeof(Manager),
            new PropertyMetadata(new Thickness(-40, 15, -930, 0))
        );

        /// <summary>
        /// The margin used for the info display - the right margin is used as the width.
        /// </summary>
        internal Thickness InfoDisplayMargin
        {
            get => (Thickness)GetValue(InfoDisplayMarginProperty);
            set => SetValue(InfoDisplayMarginProperty, value);
        }

        public Manager()
        {
            InitializeComponent();
            RefreshUI();

            // Set data context to new view model.
            DataContext = ViewModel;
        }

        protected override void OnClosed(EventArgs in_args)
        {
            base.OnClosed(in_args);

            Environment.Exit(0);
        }

        /// <summary>
        /// Sets the new width of the info display based on the current window size.
        /// </summary>
        private void SizeChanged(object in_sender, SizeChangedEventArgs in_args)
            => InfoDisplayMargin = new Thickness(-40, 15, (in_args.NewSize.Width - 76) * -1, 0);

        private void RefreshUI()
        {
            // Set title to include the assembly informational version.
            Title = $"Sonic '06 Mod Manager (Version {App.GetInformationalVersion()})";

            /* Set visibility of the emulator launcher depending on if there's an emulator specified.
               There's no point in displaying this option if the user is installing for real hardware. */
            Emulator_Launcher.Visibility = string.IsNullOrEmpty(App.Settings.Path_EmulatorExecutable)
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        /// <summary>
        /// Various key down events for list views.
        /// </summary>
        /// <param name="in_sender">List view calling this function.</param>
        /// <param name="in_args">Key event handler.</param>
        private void InvokeListViewKeyDown(ListView in_sender, KeyEventArgs in_args)
        {
            switch (in_args.Key)
            {
                // Set content enabled state upon space key down.
                case Key.Space:
                {
                    // Flip enabled state for each selected item.
                    foreach (MetadataBase item in in_sender.SelectedItems)
                        item.Enabled ^= true;

                    // Save updated content list.
                    ViewModel.Database.Save();

                    break;
                }
            }
        }

        /// <summary>
        /// Processes key down events for the mods list.
        /// </summary>
        private void ModsList_KeyDown(object in_sender, KeyEventArgs in_args)
            => InvokeListViewKeyDown(ModsList, in_args);

        /// <summary>
        /// Processes key down events for the patches list.
        /// </summary>
        private void PatchesList_KeyDown(object in_sender, KeyEventArgs in_args)
            => InvokeListViewKeyDown(PatchesList, in_args);

        /// <summary>
        /// Saves the database and shifts newly checked items to the bottom of the queue if currently installing.
        /// </summary>
        private void Content_CheckBox_CheckedChanged(object in_sender, RoutedEventArgs in_args)
        {
            // Save content database.
            ViewModel.Database.Save();

            if (ViewModel.State == InstallState.Installing)
            {
                var metadata = ((CheckBox)in_sender).DataContext as MetadataBase;

                if (metadata is Mod)
                {
                    // Set up item for the mods database.
                    InsertQueuedItem(ViewModel.Database.Mods);
                }
                else if (metadata is Patch)
                {
                    // Set up item for the patches database.
                    InsertQueuedItem(ViewModel.Database.Patches);
                }

                void InsertQueuedItem<T>(ObservableCollection<T> collection) where T : MetadataBase
                {
                    // Remove current item.
                    collection.Remove((T)metadata);

                    // Insert current item to the bottom of the queue.
                    collection.Insert(Database.IndexOfLastChecked<T>(collection) + 1, (T)metadata);
                }
            }
        }

        /// <summary>
        /// Opens or closes the info display for the selected content.
        /// </summary>
        private void ModsList_MouseDoubleClick(object in_sender, MouseButtonEventArgs in_args)
        {
            var selectedItem = ((ListViewItem)in_sender).Content as MetadataBase;

            // Do not handle for check boxes or null items.
            if (in_args.OriginalSource is System.Windows.Shapes.Rectangle || selectedItem == null)
                return;

            // Do not handle for other mouse clicks.
            if (in_args.ChangedButton != MouseButton.Left)
                return;

            if (selectedItem is Mod)
            {
                // Close all info displays in the mods list.
                CloseAllInfoDisplays(ViewModel.Database.Mods);
            }
            else if (selectedItem is Patch)
            {
                // Close all info displays in the patches list.
                CloseAllInfoDisplays(ViewModel.Database.Patches);
            }

            void CloseAllInfoDisplays<T>(ObservableCollection<T> collection) where T : MetadataBase
            {
                // Close all info displays.
                foreach (T item in collection)
                {
                    // Don't close the current info display - we invert it later.
                    if (item == selectedItem && selectedItem.InfoDisplay)
                        continue;

                    item.InfoDisplay = false;
                }
            }

            // Invert info display visibility (description required).
            if (!string.IsNullOrEmpty(selectedItem.Description))
                selectedItem.InfoDisplay ^= true;
        }

        private async Task InstallContent()
        {
            // TODO: change install button to cancel button with progress bar.
            if (ViewModel.State == InstallState.Installing)
                return;

            ViewModel.State = InstallState.Installing;
            Uninstall.IsEnabled = false;

            await Task.Run(ViewModel.Database.Install);

            Uninstall.IsEnabled = true;
            ViewModel.State = InstallState.Idle;
        }

        private async Task UninstallContent()
        {
            ViewModel.State = InstallState.Uninstalling;
            Install.IsEnabled = false;

            await Task.Run(ViewModel.Database.Uninstall);

            Install.IsEnabled = true;
            ViewModel.State = InstallState.Idle;
        }

        private async void Install_Click(object in_sender, RoutedEventArgs in_args)
        {
            await Dispatcher.Invoke(UninstallContent);
            await Dispatcher.Invoke(InstallContent);
        }

        private async void Uninstall_Click(object in_sender, RoutedEventArgs in_args)
        {
            await Dispatcher.Invoke(UninstallContent);
        }

        /// <summary>
        /// Performs a hard refresh of the content database.
        /// </summary>
        private void Refresh_Click(object in_sender, RoutedEventArgs in_args)
            => ViewModel.InvokeDatabaseContentUpdate();

        /// <summary>
        /// Opens the mod editor to create a new mod.
        /// </summary>
        private void Add_Click(object in_sender, RoutedEventArgs in_args)
        {
            new Editor { Owner = this }.ShowDialog();
        }

        /// <summary>
        /// Opens the containing directory of the content.
        /// </summary>
        private void Common_OpenFolder_Click(object in_sender, RoutedEventArgs in_args)
        {
            var metadata = ((MenuItem)in_sender).DataContext as MetadataBase;

            if (metadata != null)
            {
                // Open path in Windows Explorer.
                ProcessHelper.StartWithDefaultProgram
                (
                    // Use containing directory if mod, otherwise launch Windows Explorer.
                    metadata is Mod ? Path.GetDirectoryName(metadata.Location) : "explorer",

                    // Use no arguments if mod, otherwise select the patch with Windows Explorer.
                    metadata is Mod ? string.Empty : $"/select, \"{metadata.Location}\""
                );
            }
        }

        /// <summary>
        /// Opens the Editor window to create or edit content.
        /// </summary>
        private void Content_Create_Click(object in_sender, RoutedEventArgs in_args)
        {
            var metadata = ((MenuItem)in_sender).DataContext as MetadataBase;

            if (metadata != null)
            {
                if (metadata is Mod)
                {
                    // Pass current metadata through to the editor.
                    new Editor(metadata) { Owner = this }.ShowDialog();
                }
                else if (metadata is Patch)
                {
                    // Opens the current metadata in the default text viewer.
                    ProcessHelper.StartWithDefaultProgram(metadata.Location);
                }
            }
        }

        /// <summary>
        /// Deletes the selected content.
        /// </summary>
        private void Content_Delete_Click(object in_sender, RoutedEventArgs in_args)
        {
            var metadata = ((MenuItem)in_sender).DataContext as MetadataBase;

            if (metadata != null)
            {
                var result = NextMessageBox.Show
                (
                    LocaleService.Localise("Message_DeleteContent_Body", metadata.Title),
                    LocaleService.Localise("Message_DeleteContent_Title"),
                    NextMessageBoxButton.YesNo,
                    NextMessageBoxIcon.Question
                );

                // Delete selected content.
                if (result == NextDialogResult.Yes)
                    ViewModel.Database.Delete(metadata);
            }
        }

        /// <summary>
        /// Opens the side bar.
        /// </summary>
        private void OpenSidebar()
        {
            // Always deselect the menu button.
            SidebarMenu.IsSelected = false;

            if (!ViewModel.IsSidebarOpen)
            {
                (Sidebar.Resources["SidebarOpening"] as Storyboard).Begin();
                ViewModel.IsSidebarOpen = true;
            }
        }

        /// <summary>
        /// Closes the side bar.
        /// </summary>
        private void CloseSidebar()
        {
            // Always deselect the menu button.
            SidebarMenu.IsSelected = false;

            if (ViewModel.IsSidebarOpen)
            {
                (Sidebar.Resources["SidebarClosing"] as Storyboard).Begin();
                ViewModel.IsSidebarOpen = false;
            }
        }

        /// <summary>
        /// Opens or closes the side bar depending on the current open state.
        /// </summary>
        private void SidebarMenu_Click()
        {
            if (ViewModel.IsSidebarOpen)
            {
                CloseSidebar();
            }
            else
            {
                OpenSidebar();
            }
        }

        /// <summary>
        /// Executes <see cref="SidebarMenu_Click"/> for the menu button in the side bar.
        /// </summary>
        public RelayCommand Command_SidebarMenu_Click => new(SidebarMenu_Click);

        /// <summary>
        /// Closes the side bar upon mouse leaving the control.
        /// </summary>
        private void Sidebar_MouseLeave(object in_sender, MouseEventArgs in_args)
            => CloseSidebar();

        /// <summary>
        /// <see cref="SetSidebarTabIndex"/> command for side bar bindings.
        /// </summary>
        public RelayCommand<string> Command_SetSidebarTabIndex => new(SetSidebarTabIndex);

        /// <summary>
        /// Tab index setter for the side bar.
        /// </summary>
        /// <param name="in_index">Index parameter.</param>
        private void SetSidebarTabIndex(string in_index)
        {
            CloseSidebar();
            MainTabControl.SelectedIndex = Convert.ToInt32(in_index);
        }

        /// <summary>
        /// <see cref="SetSidebarTabIndex"/> command for side bar bindings.
        /// </summary>
        public RelayCommand Command_OpenSettings => new(OpenSettings);

        /// <summary>
        /// Opens the Settings window.
        /// </summary>
        private void OpenSettings()
        {
            CloseSidebar();
            Sidebar_Settings.IsSelected = false;

            new Settings { Owner = this }.ShowDialog();
            RefreshUI();
        }
    }
}
