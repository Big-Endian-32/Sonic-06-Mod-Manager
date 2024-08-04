using HandyControl.Expression.Shapes;
using Marathon.Exceptions;
using Marathon.Formats.Archive;
using Marathon.Formats.Script.Lua;
using SonicNextModManager.Helpers;
using SonicNextModManager.Metadata;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SonicNextModManager.Lua.Callback
{
    [MoonSharpUserData]
    public class IOFunctions
    {
        /// <summary>
        /// Encrypts the game executable.
        /// </summary>
        public static void EncryptExecutable()
        {
            var executablePath = App.Settings.Path_GameExecutable!;

            IOHelper.Backup(executablePath);

            switch (App.CurrentPlatform)
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
        public static void DecryptExecutable()
        {
            string executablePath = App.Settings.Path_GameExecutable!;

            IOHelper.Backup(executablePath);

            switch (App.CurrentPlatform)
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
        /// Decompiles the specified Lua script.
        /// </summary>
        /// <param name="in_path">The path to the Lua script to decompile.</param>
        /// <returns><c>true</c> if the Lua script was decompiled successfully; otherwise, <c>false</c>.</returns>
        public static bool DecompileLua(string in_path)
        {
            LuaBinary lub = new();

            try
            {
                lub.Load(in_path);
                lub.Decompile();
            }
            catch (InvalidSignatureException)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Writes a file to the specified location.
        /// </summary>
        /// <param name="in_destination">The path to write to.</param>
        /// <param name="in_source">The path to the file on the local disk to write to the archive.</param>
        public static void WriteFile(string in_destination, string in_source)
        {
            var source = Path.Combine(UtilityFunctions.GetSymbol("Work"), in_source);

            if (!File.Exists(source))
            {
                source = in_source;

                if (!File.Exists(source))
                    return;
            }

            if (in_destination.Contains(".arc/"))
            {
                var arcIndex = in_destination.IndexOf(".arc/") + 5;
                var arcPath  = in_destination[..(arcIndex - 1)];
                var filePath = in_destination[arcIndex..];

                var arc = Database.LoadArchive(arcPath);

                if (arc == null)
                    return;

                arc.Root.CreateDirectories(Path.GetDirectoryName(filePath)).Add
                (
                    new U8ArchiveFile(source)
                    {
                        Name = Path.GetFileName(in_destination)
                    }
                );
            }
            else
            {
                File.Copy(source, in_destination);
            }
        }
    }
}
