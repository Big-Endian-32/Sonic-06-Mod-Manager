using GongSolutions.Wpf.DragDrop;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SonicNextModManager.UI.ViewModel
{
    /// <summary>
    /// View model for Manager.xaml
    /// </summary>
    public class ManagerViewModel : INotifyPropertyChanged, IDropTarget
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
        /// Updates the database by reloading everything.
        /// </summary>
        public void InvokeDatabaseContentUpdate()
            => Database = new Database();

        /// <summary>
        /// Updates the database by reparsing it.
        /// </summary>
        public void InvokeDatabaseActiveContentUpdate()
            => Database.Load();

        public void DragOver(IDropInfo dropInfo)
        {
            if (dropInfo.Data is Metadata && dropInfo.TargetItem is Metadata)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                dropInfo.Effects = DragDropEffects.Move;
            }
        }

        public void Drop(IDropInfo dropInfo)
        {
            Metadata? sourceItem = dropInfo.Data as Metadata;
            Metadata? targetItem = dropInfo.TargetItem as Metadata;

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

            void InsertQueuedItem<T>(ObservableCollection<T> collection) where T : Metadata
            {
                int forbiddenIndex = Database.IndexOfLastInstall<T>(collection);
                int dropIndex = collection.IndexOf((T)targetItem);

                // Remove current item.
                collection.Remove((T)sourceItem);

                if (dropIndex < forbiddenIndex)
                {
                    // Insert current item after the last index of installing or installed content.
                    collection.Insert(forbiddenIndex + 1, (T)sourceItem);
                }
                else
                {
                    // Insert current item at dropped location.
                    collection.Insert(dropIndex, (T)sourceItem);
                }
            }
        }
    }
}
