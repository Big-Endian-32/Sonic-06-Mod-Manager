using SonicNextModManager.Helpers;
using SonicNextModManager.UI.Dialogs;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace SonicNextModManager
{
    public class Database : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// A collection of mods with their relevant information.
        /// </summary>
        public ObservableCollection<Mod> Mods { get; set; } = new();

        /// <summary>
        /// A collection of patches with their relevant information.
        /// </summary>
        public ObservableCollection<Patch> Patches { get; set; } = new();

        /// <summary>
        /// Active content data for JSON serialisation.
        /// </summary>
        public class ActiveContentData
        {
            /// <summary>
            /// A collection of relative paths to active mods.
            /// </summary>
            public List<string> Mods { get; set; } = new();

            /// <summary>
            /// A collection of relative paths to active patches.
            /// </summary>
            public List<string> Patches { get; set; } = new();
        }

        /// <summary>
        /// Instance of active content data.
        /// </summary>
        public ActiveContentData ActiveContent { get; set; } = new();

        /// <summary>
        /// The current content data being installed.
        /// </summary>
        public Metadata? CurrentContentInQueue { get; set; }

        /// <summary>
        /// Location of the content database.
        /// </summary>
        private string Location { get; } = Directory.Exists(App.Settings.Path_ModsDirectory)
                                               ? Path.Combine(App.Settings.Path_ModsDirectory, "content.json")
                                               : "content.json";

        public Database(bool loadActiveContent = true)
        {
            // Initialise content data upon construction.
            Mods = InitMods();
            Patches = InitPatches();

            // Load database and set up checked content.
            if (loadActiveContent)
                Load();
        }

        /// <summary>
        /// Load all mods from the mods directory.
        /// </summary>
        public ObservableCollection<Mod> InitMods()
        {
            if (!Directory.Exists(App.Settings.Path_ModsDirectory))
                goto Finish;

            // Clear mods list before writing to it.
            Mods.Clear();

            // Get the count of mods using the old INI system.
            var inis = Directory.GetFiles(App.Settings.Path_ModsDirectory, "mod.ini", SearchOption.AllDirectories);

            // Check if we've found any mods using the old INI system.
            if (inis.Length > 0)
            {
                // Ask the user if they want to migrate the old mods to the new system.
                var result = NextMessageBox.Show(LocaleService.Localise("Message_MigrateMods_Body", inis.Length),
                    LocaleService.Localise("Message_MigrateMods_Title"),
                    NextMessageBoxButton.YesNo,
                    NextMessageBoxIcon.Warning);

                // If the user agrees, then convert all the old mods to the new system.
                if (result == NextDialogResult.Yes)
                {
                    var progressDlg = new ProgressDialog("Common_PleaseWait", "Message_MigrateMods_Title")
                    {
                        Maximum = inis.Length
                    };

                    progressDlg.Callback = () =>
                    {
                        for (int i = 0; i < inis.Length; i++)
                        {
                            var ini = inis[i];

                            progressDlg.SetDescription(Path.GetFileName(Path.GetDirectoryName(ini)));
                            progressDlg.SetProgress(i);

                            SiS.MetadataConverter.Convert(ini);
                        }
                    };

                    progressDlg.ShowDialog();
                }
            }

            // Parse all mods from the mods directory.
            foreach (string mod in Directory.EnumerateFiles(App.Settings.Path_ModsDirectory, "mod.json", SearchOption.AllDirectories))
                Mods.Add(new Mod().Parse(mod));

        Finish:
            return Mods;
        }

        /// <summary>
        /// Load all patches from the patches directory.
        /// </summary>
        public ObservableCollection<Patch> InitPatches()
        {
            if (!Directory.Exists(App.Directories["Patches"]))
                goto Finish;

            // Clear patches list before writing to it.
            Patches.Clear();

            // Parse all patches from the patches directory.
            foreach (string patch in Directory.EnumerateFiles(App.Directories["Patches"], "patch.json", SearchOption.AllDirectories))
                Patches.Add(new Patch().Parse(patch));

        Finish:
            return Patches;
        }

        /// <summary>
        /// Returns the last index of a checked mod.
        /// </summary>
        /// <typeparam name="T">Content type.</typeparam>
        /// <param name="collection">Collection of content.</param>
        public static int IndexOfLastChecked<T>(ObservableCollection<T> collection) where T : Metadata
        {
            // Compute last index of installing or installed content.
            for (int i = collection.Count - 1; i > 0; i--)
            {
                if (collection[i] is T { Enabled: true })
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Returns the last index of the installing or installed content.
        /// </summary>
        /// <typeparam name="T">Content type.</typeparam>
        /// <param name="collection">Collection of content.</param>
        public static int IndexOfLastInstall<T>(ObservableCollection<T> collection) where T : Metadata
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
        public void Delete(Metadata metadata)
        {
            if (metadata is Mod)
            {
                Mods.Remove((Mod)metadata);

                // Delete mod directory recursively.
                Directory.Delete(Path.GetDirectoryName(metadata.Path), true);
            }
            else if (metadata is Patch)
            {
                Patches.Remove((Patch)metadata);

                // Delete patch.
                File.Delete(metadata.Path);
            }
        }

        /// <summary>
        /// Load and restore all enabled content from the database.
        /// </summary>
        public void Load()
        {
            if
            (
                !File.Exists(Location) ||
                !Directory.Exists(App.Settings.Path_ModsDirectory) ||
                !Directory.Exists(App.Directories["Patches"])
            )
            {
                return;
            }

            // Deserialise JSON to content.
            ActiveContent = JsonConvert.DeserializeObject<ActiveContentData>(File.ReadAllText(Location));

            foreach (var modRelativePath in ActiveContent.Mods.AsEnumerable().Reverse())
            {
                Mod mod = Mods.SingleOrDefault
                (
                    // Combine with mods directory with the relative path to get the full path.
                    mod => mod.Path == Path.Combine(App.Settings.Path_ModsDirectory, modRelativePath.ToString())
                );

                SetMetadataEnabledState(mod, Mods);
            }

            foreach (var patchRelativePath in ActiveContent.Patches.AsEnumerable().Reverse())
            {
                Patch patch = Patches.SingleOrDefault
                (
                    // Combine with patches directory with the relative path to get the full path.
                    patch => patch.Path == Path.Combine(App.Directories["Patches"], patchRelativePath.ToString())
                );

                SetMetadataEnabledState(patch, Patches);
            }

            void SetMetadataEnabledState<T>(T metadata, ObservableCollection<T> collection) where T : Metadata
            {
                if (metadata != null)
                {
                    // Set enabled state.
                    metadata.Enabled = true;

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
            ActiveContent.Mods.Clear();
            ActiveContent.Patches.Clear();

            foreach (var mod in Mods)
            {
                if (mod.Enabled)
                {
                    ActiveContent.Mods.Add
                    (
                        StringHelper.OmitSourceDirectory(mod.Path, App.Settings.Path_ModsDirectory)
                    );
                }
            }

            foreach (var patch in Patches)
            {
                if (patch.Enabled)
                {
                    ActiveContent.Patches.Add
                    (
                        StringHelper.OmitSourceDirectory(patch.Path, App.Directories["Patches"])
                    );
                }
            }

            File.WriteAllText(Location, JsonConvert.SerializeObject(ActiveContent, Formatting.Indented));
        }
    }
}
