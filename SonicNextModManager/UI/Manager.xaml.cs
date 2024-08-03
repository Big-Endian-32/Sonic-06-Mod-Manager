using SonicNextModManager.UI.ViewModel;
using System.Collections.ObjectModel;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using SonicNextModManager.UI.Components;
using SonicNextModManager.UI.Dialogs;
using CommunityToolkit.Mvvm.Input;

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

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Environment.Exit(0);
        }

        /// <summary>
        /// Sets the new width of the info display based on the current window size.
        /// </summary>
        private void SizeChanged(object sender, SizeChangedEventArgs e)
            => InfoDisplayMargin = new Thickness(-40, 15, (e.NewSize.Width - 76) * -1, 0);

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
        /// <param name="sender">List view calling this function.</param>
        /// <param name="e">Key event handler.</param>
        private void InvokeListViewKeyDown(ListView sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                // Set content enabled state upon space key down.
                case Key.Space:
                {
                    // Flip enabled state for each selected item.
                    foreach (Metadata item in sender.SelectedItems)
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
        private void ModsList_KeyDown(object sender, KeyEventArgs e)
            => InvokeListViewKeyDown(ModsList, e);

        /// <summary>
        /// Processes key down events for the patches list.
        /// </summary>
        private void PatchesList_KeyDown(object sender, KeyEventArgs e)
            => InvokeListViewKeyDown(PatchesList, e);

        /// <summary>
        /// Saves the database and shifts newly checked items to the bottom of the queue if currently installing.
        /// </summary>
        private void Content_CheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            // Save content database.
            ViewModel.Database.Save();

            if (ViewModel.State == InstallState.Installing)
            {
                Metadata? metadata = ((CheckBox)sender).DataContext as Metadata;

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

                void InsertQueuedItem<T>(ObservableCollection<T> collection) where T : Metadata
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
        private void ModsList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Metadata? selectedItem = ((ListViewItem)sender).Content as Metadata;

            // Do not handle for check boxes or null items.
            if (e.OriginalSource is Rectangle || selectedItem == null)
                return;

            // Do not handle for other mouse clicks.
            if (e.ChangedButton != MouseButton.Left)
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

            void CloseAllInfoDisplays<T>(ObservableCollection<T> collection) where T : Metadata
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

        /// <summary>
        /// TODO: https://github.com/hyperbx/SonicNextModManager/projects/3#card-72800882
        /// </summary>
        private void Install_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.State = InstallState.Installing;
            
            foreach (var mod in ViewModel.Database.Mods)
                mod.Install();

            foreach (var patch in ViewModel.Database.Patches)
                patch.Install();

            ViewModel.State = InstallState.Idle;
        }

        /// <summary>
        /// TODO: https://github.com/hyperbx/SonicNextModManager/projects/3#card-73379659
        /// </summary>
        private void Uninstall_Click(object sender, RoutedEventArgs e)
            => throw new NotImplementedException();

        /// <summary>
        /// Performs a hard refresh of the content database.
        /// </summary>
        private void Refresh_Click(object sender, RoutedEventArgs e)
            => ViewModel.InvokeDatabaseContentUpdate();

        /// <summary>
        /// Opens the mod editor to create a new mod.
        /// </summary>
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            new Editor
            {
                Owner = this
            }
            .ShowDialog();
        }

        /// <summary>
        /// Opens the containing directory of the content.
        /// </summary>
        private void Common_OpenFolder_Click(object sender, RoutedEventArgs e)
        {
            Metadata? metadata = ((MenuItem)sender).DataContext as Metadata;

            if (metadata != null)
            {
                // Open path in Windows Explorer.
                ProcessExtensions.StartWithDefaultProgram
                (
                    // Use containing directory if mod, otherwise launch Windows Explorer.
                    metadata is Mod ? System.IO.Path.GetDirectoryName(metadata.Path) : "explorer",

                    // Use no arguments if mod, otherwise select the patch with Windows Explorer.
                    metadata is Mod ? string.Empty : $"/select, \"{metadata.Path}\""
                );
            }
        }

        /// <summary>
        /// Opens the Editor window to create or edit content.
        /// </summary>
        private void Content_Create_Click(object sender, RoutedEventArgs e)
        {
            var metadata = ((MenuItem)sender).DataContext as Metadata;

            if (metadata != null)
            {
                if (metadata is Mod)
                {
                    // Pass current metadata through to the editor.
                    new Editor(metadata)
                    {
                        Owner = this
                    }
                    .ShowDialog();
                }
                else if (metadata is Patch)
                {
                    // Opens the current metadata in the default text viewer.
                    ProcessExtensions.StartWithDefaultProgram(metadata.Path);
                }
            }
        }

        /// <summary>
        /// Deletes the selected content.
        /// </summary>
        private void Content_Delete_Click(object sender, RoutedEventArgs e)
        {
            Metadata? metadata = ((MenuItem)sender).DataContext as Metadata;

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
        private void Sidebar_MouseLeave(object sender, MouseEventArgs e)
            => CloseSidebar();

        /// <summary>
        /// <see cref="SetSidebarTabIndex"/> command for side bar bindings.
        /// </summary>
        public RelayCommand<string> Command_SetSidebarTabIndex => new(SetSidebarTabIndex);

        /// <summary>
        /// Tab index setter for the side bar.
        /// </summary>
        /// <param name="index">Index parameter.</param>
        private void SetSidebarTabIndex(string index)
        {
            CloseSidebar();
            MainTabControl.SelectedIndex = Convert.ToInt32(index);
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

            new Settings
            {
                Owner = this
            }
            .ShowDialog();

            RefreshUI();
        }
    }
}
