using Marathon.Formats.Archive;
using Marathon.IO;
using SonicNextModManager.Helpers;
using SonicNextModManager.Interop;
using SonicNextModManager.Metadata.Events;
using SonicNextModManager.UI.Dialogs;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace SonicNextModManager.Metadata
{
    public class Database : INotifyPropertyChanged
    {
        private string _location { get; } = App.Configurations["Content"];

        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// A collection of mods with their relevant information.
        /// </summary>
        public ObservableCollection<Mod>? Mods { get; set; } = [];

        /// <summary>
        /// A collection of patches with their relevant information.
        /// </summary>
        public ObservableCollection<Patch>? Patches { get; set; } = [];

        /// <summary>
        /// Active content data for JSON serialisation.
        /// </summary>
        public class ActiveContentData
        {
            /// <summary>
            /// A collection of relative paths to active mods.
            /// </summary>
            public List<string>? Mods { get; set; } = [];

            /// <summary>
            /// A collection of relative paths to active patches.
            /// </summary>
            public List<string>? Patches { get; set; } = [];

            /// <summary>
            /// Gets the total amount of mods and patches.
            /// </summary>
            public int GetTotalContent()
            {
                return Mods!.Count + Patches!.Count;
            }
        }

        /// <summary>
        /// Instance of active content data.
        /// </summary>
        public ActiveContentData? ActiveContent { get; set; } = new();

        /// <summary>
        /// The current content data being installed.
        /// </summary>
        public MetadataBase? CurrentContentInQueue { get; set; }

        /// <summary>
        /// A collection of archives currently in use.
        /// </summary>
        public static Dictionary<string, U8Archive>? Archives { get; set; } = [];

        public event ContentProcessedEventHandler? ContentProcessedEvent;

        public Database(bool in_isInitActiveContent = true)
        {
            Mods    = InitMods();
            Patches = InitPatches();

            // Load database and set up checked content.
            if (in_isInitActiveContent)
                Load();
        }

        /// <summary>
        /// Load all mods from the mods directory.
        /// </summary>
        public ObservableCollection<Mod> InitMods()
        {
            if (!Directory.Exists(App.Settings.Path_ModsDirectory))
                goto Finish;

            Mods!.Clear();

            // Get mods using the old metadata format.
            var inis = Directory.GetFiles(App.Settings.Path_ModsDirectory, "mod.ini", SearchOption.AllDirectories);

            if (inis.Length > 0)
            {
                var result = NextMessageBox.Show
                (
                    LocaleService.Localise("Message_MigrateMods_Body", inis.Length),
                    LocaleService.Localise("Message_MigrateMods_Title"),
                    NextMessageBoxButton.YesNo,
                    NextMessageBoxIcon.Warning
                );

                if (result == NextDialogResult.Yes)
                {
                    var progressDlg = new ProgressDialog("Common_PleaseWait", "Message_MigrateMods_Title")
                    {
                        Maximum = inis.Length
                    };

                    progressDlg.Callback = (t) =>
                    {
                        for (int i = 0; i < inis.Length; i++)
                        {
                            if (t.IsCancellationRequested)
                                break;

                            var ini = inis[i];

                            progressDlg.SetDescription(Path.GetFileName(Path.GetDirectoryName(ini))!);
                            progressDlg.SetProgress(i);

                            ModConverter.Convert(ini);
                        }
                    };

                    progressDlg.ShowDialog();
                }
            }

            foreach (string mod in Directory.EnumerateFiles(App.Settings.Path_ModsDirectory, "mod.json", SearchOption.AllDirectories))
                Mods!.Add(Mod.Parse(mod));

        Finish:
            return Mods!;
        }

        /// <summary>
        /// Load all patches from the patches directory.
        /// </summary>
        public ObservableCollection<Patch> InitPatches()
        {
            if (!Directory.Exists(App.Directories["Patches"]))
                goto Finish;

            Patches!.Clear();

            foreach (string patch in Directory.EnumerateFiles(App.Directories["Patches"], "patch.lua", SearchOption.AllDirectories))
                Patches.Add(Patch.Parse(patch));

        Finish:
            return Patches!;
        }

        /// <summary>
        /// Installs all content asynchronously.
        /// </summary>
        public async Task Install(CancellationToken? in_cancellationToken = null, Action? in_onCancelCallback = null)
        {
            var contentCount = ActiveContent!.GetTotalContent();

            for (int i = 0; i < Mods!.Count; i++)
            {
                if (in_cancellationToken?.IsCancellationRequested == true)
                    break;

                var mod = Mods[i];

                if (mod.IsEnabled)
                {
                    mod.Install();

                    ContentProcessedEvent?.Invoke(this,
                        new ContentProcessedEventArgs(mod.Title!, i, contentCount));
                }
            }

            for (int i = 0; i < Patches!.Count; i++)
            {
                if (in_cancellationToken?.IsCancellationRequested == true)
                    break;

                var patch = Patches[i];

                if (patch.IsEnabled)
                {
                    patch.Install();

                    ContentProcessedEvent?.Invoke(this,
                        new ContentProcessedEventArgs(patch.Title!, Mods.Count + i, contentCount));
                }
            }

            if (in_cancellationToken?.IsCancellationRequested == true)
            {
                in_onCancelCallback?.Invoke();
            }
            else
            {
                WriteArchives();

#if DEBUG
                Debug.WriteLine("Install complete.");
#endif
            }
        }

        /// <summary>
        /// Uninstalls all content asynchronously.
        /// </summary>
        public void Uninstall()
        {
            var gameDirectory = App.Settings.GetGameDirectory();

            if (string.IsNullOrEmpty(gameDirectory))
                return;

            // Restore backed up files.
            foreach (var file in Directory.GetFiles(gameDirectory, "*.06mm_backup", SearchOption.AllDirectories))
            {
                if (!File.Exists(file))
                    continue;

                File.Move(file, file.Replace(".06mm_backup", ""), true);
            }

            for (int i = 0; i < Mods!.Count; i++)
            {
                var mod = Mods[i];

                mod.Uninstall();

                ContentProcessedEvent?.Invoke(this,
                    new ContentProcessedEventArgs(mod.Title!, i, Mods.Count));
            }

            DisposeArchives();

#if DEBUG
            Debug.WriteLine("Uninstall complete.");
#endif
        }

        /// <summary>
        /// Returns the last index of a checked mod.
        /// </summary>
        /// <typeparam name="T">Content type.</typeparam>
        /// <param name="collection">Collection of content.</param>
        public static int IndexOfLastChecked<T>(ObservableCollection<T> collection) where T : MetadataBase
        {
            // Compute last index of installing or installed content.
            for (int i = collection.Count - 1; i > 0; i--)
            {
                if (collection[i] is T { IsEnabled: true })
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Returns the last index of the installing or installed content.
        /// </summary>
        /// <typeparam name="T">Content type.</typeparam>
        /// <param name="collection">Collection of content.</param>
        public static int IndexOfLastInstall<T>(ObservableCollection<T> collection) where T : MetadataBase
        {
            // Compute last index of installing or installed content.
            for (int i = collection.Count - 1; i > 0; i--)
            {
                if (collection[i] is T { State: InstallState.Installing } || collection[i] is T { State: InstallState.Installed })
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Deletes the content's data pertaining to metadata.
        /// </summary>
        /// <param name="metadata">Metadata to delete.</param>
        public void Delete(MetadataBase metadata)
        {
            if (string.IsNullOrEmpty(metadata.Location))
                return;

            if (metadata is Mod out_mod)
            {
                Mods!.Remove(out_mod);

                if (string.IsNullOrEmpty(out_mod.Location))
                    return;

                Directory.Delete(Path.GetDirectoryName(out_mod.Location)!, true);
            }
            else if (metadata is Patch out_patch)
            {
                Patches!.Remove(out_patch);

                if (string.IsNullOrEmpty(out_patch.Location))
                    return;

                File.Delete(out_patch.Location);
            }
        }

        /// <summary>
        /// Load and restore all enabled content from the database.
        /// </summary>
        public void Load()
        {
            if (!File.Exists(_location) ||
                !Directory.Exists(App.Settings.Path_ModsDirectory) ||
                !Directory.Exists(App.Directories["Patches"]))
            {
                return;
            }

            ActiveContent = JsonConvert.DeserializeObject<ActiveContentData>(File.ReadAllText(_location))!;

            if (ActiveContent == null)
                return;

            foreach (var modRelativePath in ActiveContent!.Mods!.AsEnumerable().Reverse())
            {
                var mod = Mods!.SingleOrDefault
                (
                    // Combine with mods directory with the relative path to get the full path.
                    mod => mod.Location == Path.Combine(App.Settings.Path_ModsDirectory, modRelativePath.ToString())
                );

                if (mod == null)
                    continue;

                SetMetadataEnabledState(mod!, Mods!);
            }

            foreach (var patchRelativePath in ActiveContent!.Patches!.AsEnumerable().Reverse())
            {
                var patch = Patches!.SingleOrDefault
                (
                    // Combine with patches directory with the relative path to get the full path.
                    patch => patch.Location == Path.Combine(App.Directories["Patches"], patchRelativePath.ToString())
                );

                if (patch == null)
                    continue;

                SetMetadataEnabledState(patch!, Patches!);
            }

            static void SetMetadataEnabledState<T>(T metadata, ObservableCollection<T> collection) where T : MetadataBase
            {
                if (metadata != null)
                {
                    metadata.IsEnabled = true;

                    // Insert to the beginning of the collection.
                    collection.Remove(metadata);
                    collection.Insert(0, metadata);
                }
            }
        }

        /// <summary>
        /// Save all enabled content to the database.
        /// </summary>
        public void Save()
        {
            // Clear active content lists.
            ActiveContent?.Mods!.Clear();
            ActiveContent?.Patches!.Clear();

            foreach (var mod in Mods!)
            {
                if (mod.IsEnabled)
                    ActiveContent?.Mods!.Add(Path.GetRelativePath(App.Settings.Path_ModsDirectory!, mod.Location!));
            }

            foreach (var patch in Patches!)
            {
                if (patch.IsEnabled)
                    ActiveContent?.Patches!.Add(Path.GetRelativePath(App.Directories["Patches"], patch.Location!));
            }

            File.WriteAllText(_location, JsonConvert.SerializeObject(ActiveContent, Formatting.Indented));
        }

        /// <summary>
        /// Loads an archive from the specified location.
        /// </summary>
        /// <param name="in_path">The path to the archive to load.</param>
        public static U8Archive? LoadArchive(string in_path)
        {
            var gameDirectory = App.Settings.GetGameDirectory();

            if (string.IsNullOrEmpty(gameDirectory))
                return null;

            // Replace Unix path separators from Lua.
            if (!in_path.StartsWith(gameDirectory))
                in_path = Path.Combine(gameDirectory, in_path.Replace('/', '\\'));

            if (!Archives!.ContainsKey(in_path))
                Archives.Add(in_path, new(in_path, ReadMode.IndexOnly));

            return Archives[in_path];
        }

        /// <summary>
        /// Writes all archives resident in memory.
        /// </summary>
        public static void WriteArchives()
        {
            foreach (var arc in Archives!)
            {
                IOHelper.Backup(arc.Key);

                arc.Value.Save(arc.Key);
                arc.Value.Dispose();
            }

            Archives!.Clear();
        }

        /// <summary>
        /// Disposes of all archives resident in memory.
        /// </summary>
        public static void DisposeArchives()
        {
            foreach (var arc in Archives!)
                arc.Value.Dispose();

            Archives!.Clear();
        }
    }
}
