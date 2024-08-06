using GongSolutions.Wpf.DragDrop;
using SonicNextModManager.Metadata;
using SonicNextModManager.Metadata.Events;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;

namespace SonicNextModManager.UI.ViewModel
{
    /// <summary>
    /// View model for Manager.xaml
    /// </summary>
    public class MainWindowViewModel : INotifyPropertyChanged, IDropTarget
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// The current content installation state.
        /// </summary>
        public InstallState State { get; set; } = InstallState.Idle;

        /// <summary>
        /// The current content database containing mods and patches.
        /// </summary>
        public Database Database { get; set; } = new();

        /// <summary>
        /// Determines whether or not the sidebar is open.
        /// </summary>
        public bool IsSidebarOpen { get; set; } = false;

        /// <summary>
        /// The progress value for the Install button.
        /// </summary>
        public double InstallProgress { get; set; }

        /// <summary>
        /// The progress value for the Uninstall button.
        /// </summary>
        public double UninstallProgress { get; set; }

        public MainWindowViewModel()
        {
            Database.ContentProcessedEvent += Database_ContentProcessedEvent;
        }

        private void Database_ContentProcessedEvent(object in_sender, ContentProcessedEventArgs in_args)
        {
            var progress = (double)in_args.Index / (double)in_args.Total * 100.0;
#if DEBUG
            Debug.WriteLine($"{State}: {progress}% ({in_args.Index} / {in_args.Total})");
#endif
            if (State == InstallState.Installing)
            {
                InstallProgress = progress;
                UninstallProgress = 0;
            }
            else if (State == InstallState.Uninstalling)
            {
                InstallProgress = 0;
                UninstallProgress = progress;
            }
            else
            {
                ResetProgress();
            }

            if (progress < 100)
                return;

            ResetProgress();
        }

        public void ResetProgress()
        {
            InstallProgress = UninstallProgress = 0;
        }

        /// <summary>
        /// Updates the database by reloading everything.
        /// </summary>
        public void InvokeDatabaseContentUpdate()
            => Database = new();

        /// <summary>
        /// Updates the database by reparsing it.
        /// </summary>
        public void InvokeDatabaseActiveContentUpdate()
            => Database.Load();

        public void DragOver(IDropInfo in_dropInfo)
        {
            if (in_dropInfo.Data is MetadataBase && in_dropInfo.TargetItem is MetadataBase)
            {
                in_dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                in_dropInfo.Effects = DragDropEffects.Move;
            }
        }

        public void Drop(IDropInfo in_dropInfo)
        {
            MetadataBase? sourceItem = in_dropInfo.Data as MetadataBase;
            MetadataBase? targetItem = in_dropInfo.TargetItem as MetadataBase;

            if (sourceItem is Mod)
            {
                // Set up item for the mods database.
                InsertQueuedItem(Database.Mods);
            }
            else if (sourceItem is Patch)
            {
                // Set up item for the patches database.
                InsertQueuedItem(Database.Patches);
            }

            void InsertQueuedItem<T>(ObservableCollection<T> in_collection) where T : MetadataBase
            {
                int forbiddenIndex = Database.IndexOfLastInstall(in_collection);
                int dropIndex = in_collection.IndexOf((T)targetItem);

                // Remove current item.
                in_collection.Remove((T)sourceItem);

                if (dropIndex < forbiddenIndex)
                {
                    // Insert current item after the last index of installing or installed content.
                    in_collection.Insert(forbiddenIndex + 1, (T)sourceItem);
                }
                else
                {
                    // Insert current item at dropped location.
                    in_collection.Insert(dropIndex, (T)sourceItem);
                }
            }
        }
    }
}
