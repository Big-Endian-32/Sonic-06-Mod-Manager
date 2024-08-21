using SonicNextModManager.Metadata;
using System.Diagnostics;

namespace SonicNextModManager.Patches
{
    public class Executable
    {
        /// <summary>
        /// Decrypts the game executable.
        /// </summary>
        public static void Decrypt(string? in_path = null)
        {
            in_path ??= App.Settings.Path_GameExecutable!;

            Database.Backup(in_path);

            switch (App.GetCurrentPlatform())
            {
                case EPlatform.Xbox:
                {
                    Process.Start
                    (
                        new ProcessStartInfo
                        {
                            Arguments = $"-e u -c b \"{in_path}\"",
                            FileName = App.Modules["xextool"],
                            WindowStyle = ProcessWindowStyle.Hidden
                        }
                    )
                    !.WaitForExit();

                    break;
                }

                case EPlatform.PlayStation:
                {
                    Process.Start
                    (
                        new ProcessStartInfo
                        {
                            Arguments = $"-d \"{in_path}\" \"{in_path}\"",
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
        /// Encrypts the game executable.
        /// </summary>
        public static void Encrypt(string? in_path = null)
        {
            in_path ??= App.Settings.Path_GameExecutable!;

            Database.Backup(in_path);

            switch (App.GetCurrentPlatform())
            {
                case EPlatform.Xbox:
                {
                    Process.Start
                    (
                        new ProcessStartInfo
                        {
                            Arguments = $"-e e \"{in_path}\"",
                            FileName = App.Modules["xextool"],
                            WindowStyle = ProcessWindowStyle.Hidden
                        }
                    )
                    !.WaitForExit();

                    break;
                }

                case EPlatform.PlayStation:
                {
                    string encryptedName = $"{in_path}_ENC";

                    Process.Start
                    (
                        new ProcessStartInfo
                        {
                            Arguments = $"\"{in_path}\" \"{encryptedName}\"",
                            FileName = App.Modules["make_fself"],
                            WindowStyle = ProcessWindowStyle.Hidden
                        }
                    )
                    !.WaitForExit();

                    // Overwrite decrypted executable with the encrypted result.
                    if (File.Exists(encryptedName))
                        File.Move(encryptedName, in_path, true);

                    break;
                }
            }
        }
    }
}
