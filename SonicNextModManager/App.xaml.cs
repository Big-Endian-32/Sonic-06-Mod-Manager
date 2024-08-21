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
using SonicNextModManager.Metadata;

namespace SonicNextModManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static Configuration Settings { get; } = new Configuration().Import();

        public static Languages? SupportedCultures { get; set; }

        public static Language? CurrentCulture { get; set; }

        public static Dictionary<string, string> Directories { get; } = new()
        {
            { "Patches", Path.Combine(Environment.CurrentDirectory, "Patches") },
            { "Profiles", Path.Combine(Environment.CurrentDirectory, "Profiles") },
            { "Resources", Path.Combine(Environment.CurrentDirectory, "Resources") }
        };

        public static Dictionary<string, string> Configurations { get; } = new()
        {
            { "Content", Path.Combine(Settings.Path_ModsDirectory, "content.json") }
        };

        public static Dictionary<string, string> Modules { get; } = new()
        {
            { "xextool", Path.Combine(Directories["Resources"], "Libraries", "Xbox", "xextool.exe") },
            { "scetool", Path.Combine(Directories["Resources"], "Libraries", "PlayStation", "scetool.exe") },
            { "make_fself", Path.Combine(Directories["Resources"], "Libraries", "PlayStation", "make_fself.exe") }
        };

        /// <summary>
        /// Gets the assembly informational version from the entry assembly. 
        /// </summary>
        public static string? GetInformationalVersion()
        {
            var version = Assembly.GetEntryAssembly()!.GetCustomAttribute<AssemblyInformationalVersionAttribute>()!.InformationalVersion;
            var split   = version.Split('+', StringSplitOptions.RemoveEmptyEntries);

            if (split.Length <= 0 || split.Length == 1)
                return version;

            return $"{split[0]}-{split[1][..7]}";
        }

        /// <summary>
        /// Gets the current assembly name.
        /// </summary>
        public static string? GetAssemblyName()
        {
            return Assembly.GetEntryAssembly()!.GetName()!.Name;
        }

        /// <summary>
        /// Gets the current assembly version.
        /// </summary>
        public static string? GetAssemblyVersion()
        {
            return Assembly.GetEntryAssembly()!.GetName()!.Version!.ToString();
        }

        /// <summary>
        /// Gets the current platform from the game executable path.
        /// </summary>
        /// <returns></returns>
        public static EPlatform GetCurrentPlatform()
        {
            return StringHelper.GetPlatformFromFilePath(Settings.Path_GameExecutable);
        }

        /// <summary>
        /// Gets the current platform from the game executable path
        /// and returns it as the game's internal platform-specific directory name.
        /// </summary>
        public static string GetCurrentPlatformString()
        {
            return GetCurrentPlatform() == EPlatform.Xbox
                ? "xenon"
                : "ps3";
        }

        /// <summary>
        /// Creates the exception handler to provide a friendly interface for errors.
        /// </summary>
        private static void CreateExceptionHandler()
        {
#if !DEBUG
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                new UI.ExceptionWindow((Exception)e.ExceptionObject).ShowDialog();
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

        protected override void OnStartup(StartupEventArgs in_args)
        {
            // Set up exception handler.
            CreateExceptionHandler();

            // Set up directory tree.
            CreateDefaultDirectoryTree();

            // Verify if the required modules exist and throw if not.
            if (!VerifyModuleIntegrity())
                throw new FileNotFoundException(LocaleService.Localise("Exception_MissingModules"));

            // Set up language settings.
            Language.LoadCultureResources();

            // Start with MainWindow.xaml if the step-by-step guide has been completed already.
            if (Settings.Setup_IsComplete)
                StartupUri = new Uri("pack://application:,,,/SonicNextModManager;component/UI/MainWindow.xaml");

            base.OnStartup(in_args);
        }
    }
}
