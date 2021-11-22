﻿global using Newtonsoft.Json;
global using System;
global using System.Collections.Generic;
global using System.Globalization;
global using System.IO;
global using System.Linq;
global using System.Text;
global using System.Windows;
global using System.Windows.Controls;
global using System.Windows.Data;
global using System.Windows.Input;

using System.Reflection;
using MoonSharp.Interpreter;
using Sprint;

namespace SonicNextModManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IConfiguration Settings { get; } = new ConfigurationBuilder<IConfiguration>().UseIniFile($"{GetAssemblyName()}.ini").Build();

        public static Languages SupportedCultures { get; set; }

        public static Language CurrentCulture { get; set; }

        public static Dictionary<string, string> Directories { get; } = new()
        {
            { "Patches", Path.Combine(Environment.CurrentDirectory, "Patches") },
            { "Profiles", Path.Combine(Environment.CurrentDirectory, "Profiles") }
        };

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
        /// Creates the exception handler to provide a friendly interface for errors.
        /// </summary>
        private void CreateExceptionHandler()
        {
#if !DEBUG
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                new ErrorHandler((Exception)e.ExceptionObject, e.IsTerminating).ShowDialog();
            };
#endif
        }

        public static string GetAssemblyName()
            => Assembly.GetEntryAssembly().GetName().Name;

        /// <summary>
        /// Returns the current assembly version.
        /// </summary>
        public static string GetAssemblyVersion()
            => Assembly.GetEntryAssembly().GetName().Version.ToString();

        protected override void OnStartup(StartupEventArgs e)
        {
            // Set up exception handler.
            CreateExceptionHandler();

            // Set up directory tree.
            CreateDefaultDirectoryTree();

            // Set up language settings.
            Language.LoadCultureResources();

            // Start with Manager.xaml if the step-by-step guide has been completed already.
            if (Settings.Setup_Complete)
                StartupUri = new Uri("pack://application:,,,/SonicNextModManager;component/UI/Manager.xaml");

            base.OnStartup(e);
        }
    }
}
