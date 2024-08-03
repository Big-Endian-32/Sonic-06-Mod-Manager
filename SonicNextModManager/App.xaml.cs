global using MoonSharp.Interpreter;
global using Newtonsoft.Json;
global using Newtonsoft.Json.Converters;
global using SonicNextModManager.Services;
global using System;
global using System.Collections.Generic;
global using System.Globalization;
global using System.IO;
global using System.Linq;
global using System.Reflection;
global using System.Text;
global using System.Windows;
global using System.Windows.Controls;
global using System.Windows.Data;
global using System.Windows.Input;

using SonicNextModManager.Helpers;

namespace SonicNextModManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static Configuration Settings { get; } = new Configuration().Import();

        public static Languages SupportedCultures { get; set; }

        public static Language CurrentCulture { get; set; }

        public static Platform CurrentPlatform { get; } = StringHelper.GetPlatformFromFilePath(Settings.Path_GameExecutable);

        public static string CurrentInformationalVersion { get; private set; }

        public static Dictionary<string, string> Directories { get; } = new()
        {
            { "Patches", Path.Combine(Environment.CurrentDirectory, "Patches") },
            { "Profiles", Path.Combine(Environment.CurrentDirectory, "Profiles") },
            { "Resources", Path.Combine(Environment.CurrentDirectory, "Resources") }
        };

        public static Dictionary<string, string> Configurations { get; } = new()
        {
            { "Content", Path.Combine(Settings.Path_ModsDirectory, "content.json") },
            { "Data", Path.Combine(Settings.Path_ModsDirectory, "data.json") }
        };

        public static Dictionary<string, string> Modules { get; } = new()
        {
            { "xextool", Path.Combine(Directories["Resources"], "Libraries", "Xbox", "xextool.exe") },
            { "scetool", Path.Combine(Directories["Resources"], "Libraries", "PlayStation", "scetool.exe") },
            { "make_fself", Path.Combine(Directories["Resources"], "Libraries", "PlayStation", "make_fself.exe") }
        };

        public static Dictionary<string, Marathon.Formats.Archive.U8Archive> Archives = new();

        /// <summary>
        /// Returns the assembly informational version from the entry assembly. 
        /// </summary>
        public static string GetInformationalVersion()
            => Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;

        /// <summary>
        /// Returns the current assembly name.
        /// </summary>
        public static string GetAssemblyName()
            => Assembly.GetEntryAssembly().GetName().Name;

        /// <summary>
        /// Returns the current assembly version.
        /// </summary>
        public static string GetAssemblyVersion()
            => Assembly.GetEntryAssembly().GetName().Version.ToString();

        /// <summary>
        /// Creates the exception handler to provide a friendly interface for errors.
        /// </summary>
        private void CreateExceptionHandler()
        {
#if !DEBUG
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                new ErrorHandler((Exception)e.ExceptionObject).ShowDialog();
            };
#endif
        }

        /// <summary>
        /// Creates the default directory tree.
        /// </summary>
        private static void CreateDefaultDirectoryTree()
        {
            // Create default directory tree from the working directory.
            foreach (var directory in Directories)
                Directory.CreateDirectory(directory.Value);

            string modsDirectory = Settings.Path_ModsDirectory;
            {
                // Create default mods directory if the current one is null or doesn't exist.
                if (string.IsNullOrEmpty(modsDirectory) || !Directory.Exists(modsDirectory))
                {
                    Directory.CreateDirectory
                    (
                        Settings.Path_ModsDirectory = Path.Combine(Environment.CurrentDirectory, "Mods")
                    );
                }
            }
        }

        /// <summary>
        /// Verifies if the required modules exist.
        /// </summary>
        private static bool VerifyModuleIntegrity()
        {
            // If the resources directory doesn't exist, return false.
            if (!Directory.Exists(Directories["Resources"]))
                return false;

            foreach (var module in Modules)
            {
                // If a module doesn't exist, return false.
                if (!File.Exists(module.Value))
                    return false;
            }

            return true;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            // Set current informational version for binding.
            CurrentInformationalVersion = GetInformationalVersion();

            // Set up exception handler.
            CreateExceptionHandler();

            // Set up directory tree.
            CreateDefaultDirectoryTree();

            // Verify if the required modules exist and throw if not.
            if (!VerifyModuleIntegrity())
                throw new FileNotFoundException(LocaleService.Localise("Exception_MissingModules"));

            // Set up language settings.
            Language.LoadCultureResources();

            // Start with Manager.xaml if the step-by-step guide has been completed already.
            if (Settings.Setup_Complete)
                StartupUri = new Uri("pack://application:,,,/SonicNextModManager;component/UI/Manager.xaml");

            base.OnStartup(e);
        }
    }
}
