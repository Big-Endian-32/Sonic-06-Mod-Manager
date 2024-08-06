using Marathon.Exceptions;
using Marathon.Formats.Archive;
using Marathon.Formats.Script.Lua;
using Marathon.Helpers;
using SonicNextModManager.Helpers;
using SonicNextModManager.Lua.Attributes;
using SonicNextModManager.Metadata;
using System.Diagnostics;

namespace SonicNextModManager.Lua.Callback
{
    public class IOFunctions
    {
        /// <summary>
        /// Encrypts the game executable.
        /// </summary>
        [LuaCallback]
        public static void EncryptExecutable()
        {
            var executablePath = App.Settings.Path_GameExecutable!;

            IOHelper.Backup(executablePath);

            switch (App.GetCurrentPlatform())
            {
                case Platform.Xbox:
                {
                    Process.Start
                    (
                        new ProcessStartInfo
                        {
                            Arguments = $"-e e \"{executablePath}\"",
                            FileName = App.Modules["xextool"],
                            WindowStyle = ProcessWindowStyle.Hidden
                        }
                    )
                    !.WaitForExit();

                    break;
                }

                case Platform.PlayStation:
                {
                    string encryptedName = $"{executablePath}_ENC";

                    Process.Start
                    (
                        new ProcessStartInfo
                        {
                            Arguments = $"\"{executablePath}\" \"{encryptedName}\"",
                            FileName = App.Modules["make_fself"],
                            WindowStyle = ProcessWindowStyle.Hidden
                        }
                    )
                    !.WaitForExit();

                    // Overwrite decrypted executable with the encrypted result.
                    if (File.Exists(encryptedName))
                        File.Move(encryptedName, executablePath, true);

                    break;
                }
            }
        }

        /// <summary>
        /// Decrypts the game executable.
        /// </summary>
        [LuaCallback]
        public static void DecryptExecutable()
        {
            string executablePath = App.Settings.Path_GameExecutable!;

            IOHelper.Backup(executablePath);

            switch (App.GetCurrentPlatform())
            {
                case Platform.Xbox:
                {
                    Process.Start
                    (
                        new ProcessStartInfo
                        {
                            Arguments = $"-e u -c b \"{executablePath}\"",
                            FileName = App.Modules["xextool"],
                            WindowStyle = ProcessWindowStyle.Hidden
                        }
                    )
                    !.WaitForExit();

                    break;
                }

                case Platform.PlayStation:
                {
                    Process.Start
                    (
                        new ProcessStartInfo
                        {
                            Arguments = $"-d \"{executablePath}\" \"{executablePath}\"",
                            FileName = App.Modules["scetool"],
                            WindowStyle = ProcessWindowStyle.Hidden
                        }
                    )
                    !.WaitForExit();

                    break;
                }
            }
        }

        /// <summary>
        /// Writes a file to the specified location.
        /// </summary>
        /// <param name="in_destination">The path to write to.</param>
        /// <param name="in_source">The path to the file on the local disk to write to the archive.</param>
        [LuaCallback]
        public static void WriteFile(string in_destination, string in_source)
        {
            var source = Path.Combine(UtilityFunctions.GetSymbol("Work"), in_source);

            if (!File.Exists(source))
            {
                source = in_source;

                if (!File.Exists(source))
                    return;
            }

            if (Helpers.ArchiveHelper.IsInternalArchivePath(in_destination))
            {
                var dir = Helpers.ArchiveHelper.GetArchiveDirectory(Path.GetDirectoryName(in_destination)!);

                if (dir == null)
                    return;

                dir.Add(new U8ArchiveFile(source) { Name = Path.GetFileName(in_destination) });
            }
            else
            {
                File.Copy(source, in_destination);
            }
        }
    }
}
