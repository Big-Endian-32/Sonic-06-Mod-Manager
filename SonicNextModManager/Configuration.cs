using System.ComponentModel;

namespace SonicNextModManager
{
    public class Configuration : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private string _config = $"{App.GetAssemblyName()}.json";

        public bool Setup_Complete { get; set; }

        public string? General_Language { get; set; } = "en-GB";

        public WindowState MainWindow_WindowState { get; set; } = WindowState.Normal;

        public bool General_Debug { get; set; }

        public int Emulator_Backend { get; set; }

        public int Emulator_Width { get; set; }

        public int Emulator_Height { get; set; }

        public int Emulator_Language { get; set; }

        public string? Emulator_Arguments { get; set; }

        public bool Emulator_Fullscreen { get; set; }

        public bool Emulator_GammaCorrect { get; set; } = true;

        public bool Emulator_LaunchAfterInstallingContent { get; set; }

        public string? Path_ModsDirectory { get; set; }

        public string? Path_GameExecutable { get; set; }

        public string? Path_EmulatorExecutable { get; set; }

        public void OnPropertyChanged(PropertyChangedEventArgs in_args)
        {
            PropertyChanged?.Invoke(this, in_args);
            Export();
        }

        public Configuration Export()
        {
            // Export current config.
            File.WriteAllText(_config, JsonConvert.SerializeObject(this, Formatting.Indented));

            // Return current config.
            return this;
        }

        public Configuration Import()
        {
            // Return deserialised object if the config exists.
            if (File.Exists(_config))
                return JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(_config));

            // Export from current config and return.
            return Export();
        }

        /// <summary>
        /// Gets the path to the current game directory.
        /// </summary>
        public string? GetGameDirectory()
        {
            return Path.GetDirectoryName(Path_GameExecutable);
        }
    }
}
