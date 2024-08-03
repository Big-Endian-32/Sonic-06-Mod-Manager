using System.ComponentModel;

namespace SonicNextModManager
{
    public class Configuration : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private string _config = $"{App.GetAssemblyName()}.json";

        public bool Setup_Complete { get; set; } = false;

        public string? General_Language { get; set; } = "en-GB";

        public WindowState Manager_WindowState { get; set; } = WindowState.Normal;

        public bool General_Debug { get; set; } = false;

        public int Emulator_Backend { get; set; } = 0;

        public int Emulator_Width { get; set; } = 0;

        public int Emulator_Height { get; set; } = 0;

        public int Emulator_Language { get; set; } = 0;

        public string? Emulator_Arguments { get; set; } = "";

        public bool Emulator_Fullscreen { get; set; } = false;

        public bool Emulator_GammaCorrect { get; set; } = true;

        public bool Emulator_LaunchAfterInstallingContent { get; set; } = false;

        public string? Path_ModsDirectory { get; set; } = "";

        public string? Path_GameExecutable { get; set; } = "";

        public string? Path_EmulatorExecutable { get; set; } = "";

        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
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
    }
}
