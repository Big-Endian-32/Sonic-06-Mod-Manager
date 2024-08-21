using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using SonicNextModManager.Emulation;
using SonicNextModManager.Helpers;
using SonicNextModManager.Metadata;
using SonicNextModManager.UI.Components;
using SonicNextModManager.UI.Dialogs;
using SonicNextModManager.UI.ViewModel;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace SonicNextModManager.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ImmersiveWindow
    {
        private CancellationTokenSource _cts = new();

        private MainWindowViewModel _viewModel { get; set; } = new();

        /// <summary>
        /// Property storage for <see cref="InfoDisplayMargin"/>.
        /// </summary>
        internal static readonly DependencyProperty InfoDisplayMarginProperty = DependencyProperty.Register
        (
            nameof(InfoDisplayMargin),
            typeof(Thickness),
            typeof(MainWindow),
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

        public MainWindow()
        {
            InitializeComponent();
            RefreshUI();

            // Set title to include the assembly informational version.
            Title = $"Sonic '06 Mod Manager (Version {App.GetInformationalVersion()})";

            // Set data context to new view model.
            DataContext = _viewModel;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

#if DEBUG
            if (e.Key == Key.F1)
            {
                new DebugWindow().ShowDialog();
            }
#endif
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
            /* Set visibility of the emulator launcher depending on if there's an emulator specified.
               There's no point in displaying this option if the user is installing for real hardware. */
            EmulatorLauncher.Visibility = string.IsNullOrEmpty(App.Settings.Path_EmulatorExecutable)
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
                        item.IsEnabled ^= true;

                    // Save updated content list.
                    _viewModel.Database.Save();

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
            _viewModel.Database.Save();

            if (_viewModel.State == EInstallState.Installing)
            {
                var metadata = ((CheckBox)in_sender).DataContext as MetadataBase;

                if (metadata is Mod)
                {
                    // Set up item for the mods database.
                    InsertQueuedItem(_viewModel.Database.Mods);
                }
                else if (metadata is Patch)
                {
                    // Set up item for the patches database.
                    InsertQueuedItem(_viewModel.Database.Patches);
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

            if (!App.Settings.General_IsAllowMultipleInfoDisplays)
            {
                if (selectedItem is Mod)
                {
                    // Close all info displays in the mods list.
                    CloseAllInfoDisplays(_viewModel.Database.Mods);
                }
                else if (selectedItem is Patch)
                {
                    // Close all info displays in the patches list.
                    CloseAllInfoDisplays(_viewModel.Database.Patches);
                }
            }

            void CloseAllInfoDisplays<T>(ObservableCollection<T> collection) where T : MetadataBase
            {
                // Close all info displays.
                foreach (T item in collection)
                {
                    // Don't close the current info display - we invert it later.
                    if (item == selectedItem && selectedItem.IsInfoDisplay)
                        continue;

                    item.IsInfoDisplay = false;
                }
            }

            // Invert info display visibility (description required).
            if (!string.IsNullOrEmpty(selectedItem.Description))
                selectedItem.IsInfoDisplay ^= true;
        }

        private void SetInstallState(EInstallState in_state)
        {
            _viewModel.State = in_state;

            Dispatcher.Invoke
            (
                () =>
                {
                    switch (in_state)
                    {
                        case EInstallState.Idle:
                            Install.IsEnabled = Uninstall.IsEnabled = EmulatorLauncher.IsEnabled = true;
                            Install.Content = LocaleService.Localise("Main_Install");
                            break;

                        case EInstallState.Installing:
                            Install.IsEnabled = true;
                            Uninstall.IsEnabled = EmulatorLauncher.IsEnabled = false;
                            Install.Content = LocaleService.Localise("Common_Cancel");
                            break;

                        case EInstallState.Uninstalling:
                            Install.IsEnabled = Uninstall.IsEnabled = EmulatorLauncher.IsEnabled = false;
                            Install.Content = LocaleService.Localise("Main_Install");
                            break;

                        case EInstallState.Cancelling:
                            Install.IsEnabled = Uninstall.IsEnabled = EmulatorLauncher.IsEnabled = false;
                            Install.Content = LocaleService.Localise("Common_Cancelling");
                            break;
                    }
                }
            );
        }

        private async void Install_Click(object in_sender, RoutedEventArgs in_args)
        {
            var isCancelled = false;

            if (_viewModel.State == EInstallState.Installing)
            {
                SetInstallState(EInstallState.Cancelling);

                _cts.Cancel();

                isCancelled = true;
            }
            else
            {
                _viewModel.InvokeDatabaseContentUpdate();

                SetInstallState(EInstallState.Uninstalling);

                // Uninstall content before installing to prevent stacking.
                await Task.Run(_viewModel.Database.Uninstall);

                SetInstallState(EInstallState.Installing);
            }

            try
            {
                void OnCancel()
                {
                    _viewModel.Database.Uninstall();

                    SetInstallState(EInstallState.Idle);
                }

                await Task.Run(() => _viewModel.Database.Install(_cts.Token, OnCancel), _cts.Token);
            }
            catch (TaskCanceledException)
            {
                _cts = new();

#if DEBUG
                Debug.WriteLine("Installation cancelled...");
#endif
            }
#if !DEBUG
            catch (Exception out_ex)
            {
                NextMessageBox.Show
                (
                    LocaleService.Localise("Exception_InstallError", out_ex),
                    LocaleService.Localise("Exception_InstallFailed"),
                    in_icon: ENextMessageBoxIcon.Error
                );

                SetInstallState(EInstallState.Uninstalling);

                await Task.Run(_viewModel.Database.Uninstall);
            }
#endif

            /* Reset mod states back to idle to
               restore the checkboxes post-install. */
            foreach (var mod in _viewModel!.Database!.Mods!)
                mod.State = EInstallState.Idle;

            if (!isCancelled)
                SetInstallState(EInstallState.Idle);

            _viewModel.ResetProgress();

            if (App.Settings.Emulator_IsLaunchAfterInstallingContent)
                Process.Start(EmulatorFactory.GetStartInfo());
        }

        private async void Uninstall_Click(object in_sender, RoutedEventArgs in_args)
        {
            SetInstallState(EInstallState.Uninstalling);

            await Task.Run(_viewModel.Database.Uninstall);

            SetInstallState(EInstallState.Idle);

            _viewModel.ResetProgress();
        }

        private void Emulator_Launcher_Click(object in_sender, RoutedEventArgs in_args)
        {
            var emuPath = App.Settings.Path_EmulatorExecutable;

            if (string.IsNullOrEmpty(emuPath) || !File.Exists(emuPath))
            {
                var result = NextMessageBox.Show
                (
                    LocaleService.Localise("Message_NoEmulator_Body"),
                    LocaleService.Localise("Message_NoEmulator_Title"),
                    ENextMessageBoxButton.YesNo,
                    ENextMessageBoxIcon.Question
                );

                if (result == ENextDialogResult.No)
                    return;

                App.Settings.Path_EmulatorExecutable = FileQueries.QueryEmulatorExecutable();

                Emulator_Launcher_Click(in_sender, in_args);

                return;
            }

            Process.Start(EmulatorFactory.GetStartInfo());
        }

        /// <summary>
        /// Performs a hard refresh of the content database.
        /// </summary>
        private void Refresh_Click(object in_sender, RoutedEventArgs in_args)
        {
            _viewModel.InvokeDatabaseContentUpdate();
        }

        /// <summary>
        /// Opens the mod editor to create a new mod.
        /// </summary>
        private void Add_Click(object in_sender, RoutedEventArgs in_args)
        {
            new EditorWindow { Owner = this }.ShowDialog();
        }

        /// <summary>
        /// Opens the containing directory of the content.
        /// </summary>
        private void Common_OpenFolder_Click(object in_sender, RoutedEventArgs in_args)
        {
            if (((MenuItem)in_sender).DataContext is MetadataBase out_metadata)
            {
                if (out_metadata is Mod out_mod)
                {
                    ProcessHelper.StartWithDefaultProgram(Path.GetDirectoryName(out_mod.Location));
                }
                else if (out_metadata is Patch out_patch)
                {
                    ProcessHelper.StartWithDefaultProgram("explorer", $"/select \"{out_patch.Location}\"");
                }
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
                    new EditorWindow(metadata) { Owner = this }.ShowDialog();
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
                    ENextMessageBoxButton.YesNo,
                    ENextMessageBoxIcon.Question
                );

                // Delete selected content.
                if (result == ENextDialogResult.Yes)
                    _viewModel.Database.Delete(metadata);
            }
        }

        /// <summary>
        /// Opens the side bar.
        /// </summary>
        private void OpenSidebar()
        {
            // Always deselect the menu button.
            SidebarMenu.IsSelected = false;

            if (!_viewModel.IsSidebarOpen)
            {
                (Sidebar.Resources["SidebarOpening"] as Storyboard).Begin();
                _viewModel.IsSidebarOpen = true;
            }
        }

        /// <summary>
        /// Closes the side bar.
        /// </summary>
        private void CloseSidebar()
        {
            // Always deselect the menu button.
            SidebarMenu.IsSelected = false;

            if (_viewModel.IsSidebarOpen)
            {
                (Sidebar.Resources["SidebarClosing"] as Storyboard).Begin();
                _viewModel.IsSidebarOpen = false;
            }
        }

        /// <summary>
        /// Opens or closes the side bar depending on the current open state.
        /// </summary>
        private void SidebarMenu_Click()
        {
            if (_viewModel.IsSidebarOpen)
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

            var index = Convert.ToInt32(in_index);

            // Deselect side menu items for inactive menus.
            if (index >= 4)
            {
                MainSideMenu.Items.Do(x => ((SideMenuItem)x).IsSelected = false);
            }
            else
            {
                MiscSideMenu.Items.Do(x => ((SideMenuItem)x).IsSelected = false);
            }

            MainTabControl.SelectedIndex = index;
        }
    }
}
