using SonicNextModManager.Emulation;
using System.ComponentModel;

namespace SonicNextModManager
{
    public class Configuration : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private string _config = $"{App.GetAssemblyName()}.json";

        public bool Setup_IsComplete { get; set; } = false;

        public string? General_Language { get; set; } = "en-GB";

        public bool General_IsAllowMultipleInfoDisplays { get; set; } = false;

        public WindowState MainWindow_WindowState { get; set; } = WindowState.Normal;

        public EXeniaBackend Emulator_Xenia_Backend { get; set; } = EXeniaBackend.D3D12;

        public int Emulator_Xenia_Width { get; set; } = 0;

        public int Emulator_Xenia_Height { get; set; } = 0;

        public EXeniaLanguage Emulator_Xenia_Language { get; set; } = EXeniaLanguage.English;

        public string? Emulator_Arguments { get; set; } = "";

        public bool Emulator_Xenia_IsFullscreen { get; set; } = false;

        public bool Emulator_Xenia_IsGammaCorrection { get; set; } = true;

        public bool Emulator_IsLaunchAfterInstallingContent { get; set; } = false;

        public string? Path_ModsDirectory { get; set; } = "";

        public string? Path_GameExecutable { get; set; } = "";

        public string? Path_EmulatorExecutable { get; set; } = "";

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
