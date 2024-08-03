using Keystone;
using Marathon.Exceptions;
using Marathon.Formats.Archive;
using Marathon.Formats.Script.Lua;
using Marathon.IO;
using SonicNextModManager.Helpers;
using SonicNextModManager.Metadata;
using System.Diagnostics;

namespace SonicNextModManager.Lua.Callback
{
    [MoonSharpUserData]
    public class IOFunctions
    {
        /// <summary>
        /// Loads an archive from the input path.
        /// </summary>
        /// <param name="in_path">Path to archive.</param>
        public static void LoadArchive(string in_path)
        {
            if (Database.Archives.ContainsKey(in_path))
                Database.Archives[in_path].Dispose();

            Database.Archives[in_path] = new U8Archive(in_path, ReadMode.IndexOnly);
        }

        /// <summary>
        /// Saves an archive from the input path.
        /// </summary>
        /// <param name="in_path">Path to archive.</param>
        public static void SaveArchive(string in_path)
        {
            Database.Archives[in_path].Save();
            Database.Archives.Remove(in_path);
        }

        /// <summary>
        /// Writes a file to the loaded archive.
        /// </summary>
        /// <param name="in_path">Archive to write data to.</param>
        /// <param name="in_internalPath">Path to write the file to inside the archive.</param>
        /// <param name="in_filePath">Path to the file to read data from.</param>
        public static void WriteFile(string in_path, string in_internalPath, string in_filePath)
        {
            Database.Archives[in_path].Root.CreateDirectories(Path.GetDirectoryName(in_internalPath)).Add
            (
                new U8ArchiveFile(in_filePath)
                {
                    Name = Path.GetFileName(in_internalPath)
                }
            );
        }

        /// <summary>
        /// Encrypts the current game executable (required step for PlayStation executable patching).
        /// </summary>
        public static void EncryptExecutable()
        {
            string? executable = App.Settings.Path_GameExecutable;

            IOHelper.Backup(executable);

            switch (App.CurrentPlatform)
            {
                case Platform.Xbox:
                {
                    Process.Start
                    (
                        new ProcessStartInfo
                        {
                            Arguments = $"-e e \"{executable}\"",
                            FileName = App.Modules["xextool"],
                            WindowStyle = ProcessWindowStyle.Hidden
                        }
                    )
                    .WaitForExit();

                    break;
                }

                case Platform.PlayStation:
                {
                    string encrypt = $"{executable}_ENC";

                    Process.Start
                    (
                        new ProcessStartInfo
                        {
                            Arguments = $"\"{executable}\" \"{encrypt}\"",
                            FileName = App.Modules["make_fself"],
                            WindowStyle = ProcessWindowStyle.Hidden
                        }
                    )
                    .WaitForExit();

                    // Overwrite decrypted executable with the encrypted result.
                    if (File.Exists(encrypt))
                        File.Move(encrypt, executable, true);

                    break;
                }
            }
        }

        /// <summary>
        /// Decrypts the current game executable.
        /// </summary>
        public static void DecryptExecutable()
        {
            string? executable = App.Settings.Path_GameExecutable;

            IOHelper.Backup(executable);

            switch (App.CurrentPlatform)
            {
                case Platform.Xbox:
                {
                    Process.Start
                    (
                        new ProcessStartInfo
                        {
                            Arguments = $"-e u -c b \"{executable}\"",
                            FileName = App.Modules["xextool"],
                            WindowStyle = ProcessWindowStyle.Hidden
                        }
                    )
                    .WaitForExit();

                    break;
                }

                case Platform.PlayStation:
                {
                    Process.Start
                    (
                        new ProcessStartInfo
                        {
                            Arguments = $"-d \"{executable}\" \"{executable}\"",
                            FileName = App.Modules["scetool"],
                            WindowStyle = ProcessWindowStyle.Hidden
                        }
                    )
                    .WaitForExit();

                    break;
                }
            }
        }

        /// <summary>
        /// Decompiles the input Lua script.
        /// </summary>
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
        /// Reads bytes from the specified address and returns a hexadecimal string.
        /// </summary>
        /// <param name="in_file">File to read from.</param>
        /// <param name="in_addr">Address to read from.</param>
        /// <param name="in_count">Amount of bytes to read.</param>
        public static string ReadBytes(string in_file, int in_addr, int in_count)
        {
#if !DEBUG
            try
            {
#endif
                // Open the file for reading.
                using (FileStream fileStream = File.OpenRead(Patcher.GetSymbol(in_file)))
                {
                    using (var reader = new BinaryReader(fileStream))
                    {
                        // Seek to requested address in the stream.
                        reader.BaseStream.Seek(in_addr, SeekOrigin.Begin);

                        // Return as a hexadecimal string.
                        return MemoryHelper.ByteArrayToHexString(reader.ReadBytes(in_count));
                    }
                }
#if !DEBUG
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                return string.Empty;
            }
#endif
        }

        /// <summary>
        /// Writes bytes to the specified address and returns whether or not it succeeded.
        /// </summary>
        /// <param name="in_file">File to write to.</param>
        /// <param name="in_addr">Address to write from.</param>
        /// <param name="in_hexStr">Hexadecimal string containing the bytes to be written.</param>
        public static bool WriteBytes(string in_file, int in_addr, string in_hexStr)
        {
#if !DEBUG
            try
            {
#endif
                IOHelper.Backup(in_file);

                // Open the file for writing.
                // TODO: accept paths to archive files.
                using (FileStream fileStream = File.OpenWrite(Patcher.GetSymbol(in_file)))
                {
                    using (var writer = new BinaryWriter(fileStream))
                    {
                        // Seek to the requested address in the stream.
                        writer.BaseStream.Seek(in_addr, SeekOrigin.Begin);

                        // Write the specified bytes to the address.
                        writer.Write(MemoryHelper.HexStringToByteArray(in_hexStr));

                        // Log written bytes to the console.
                        Console.WriteLine($"Written bytes to 0x{in_addr:X8}: {in_hexStr}");
                    }
                }
#if !DEBUG
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                return false;
            }
#endif

            return true;
        }

        /// <summary>
        /// Writes bytes to the specified address and returns whether or not it succeeded.
        /// </summary>
        /// <param name="in_file">File to write to.</param>
        /// <param name="in_addr">Address to write from.</param>
        /// <param name="in_data">Byte array containing the bytes to be written.</param>
        private static bool WriteBytes(string in_file, int in_addr, byte[] in_data)
            => WriteBytes(in_file, in_addr, MemoryHelper.ByteArrayToHexString(in_data));

        /// <summary>
        /// Writes a no-operation opcode to the specified address and returns whether or not it succeeded.
        /// </summary>
        /// <param name="in_addr">Address to write from.</param>
        /// <param name="in_count">Amount of NOPs to write.</param>
        public static bool WriteNop(int in_addr, int in_count = 1)
        {
            for (int i = 0; i < in_count; i++)
            {
                WriteBytes("Executable", in_addr, [0x60, 0x00, 0x00, 0x00]);
                in_addr += 4;
            }

            return true;
        }

        /// <summary>
        /// Writes a jump instruction to the specified address and returns whether or not it succeeded.
        /// </summary>
        /// <param name="in_source">Address to write from.</param>
        /// <param name="in_destination">Destination to jump to.</param>
        public static bool WriteJump(int in_source, int in_destination)
            => WriteAsm(in_source, $"b {in_destination};");

        /// <summary>
        /// Writes a UTF8-encoded string to the specified address and returns the input string.
        /// </summary>
        /// <param name="in_file">File to write to.</param>
        /// <param name="in_addr">Address to write from.</param>
        /// <param name="in_str">Text to write.</param>
        public static string WriteString(string in_file, int in_addr, string in_str)
        {
            // Write the specified UTF8 string to the address in the requested file.
            WriteBytes(in_file, in_addr, Encoding.UTF8.GetBytes(in_str));

            return in_str;
        }

        /// <summary>
        /// Writes compiled assembly bytecode to the specified address and returns whether or not it succeeded.
        /// </summary>
        /// <param name="in_addr">Address to write from.</param>
        /// <param name="in_code">Assembly to compile.</param>
        public static bool WriteAsm(int in_addr, string in_code)
        {
#if !DEBUG
            try
            {
#endif
                using (var keystone = new Engine(Architecture.PPC, Mode.PPC64 | Mode.BIG_ENDIAN) { ThrowOnError = true })
                {
                    WriteBytes("Executable", in_addr, keystone.Assemble(in_code, (ulong)in_addr).Buffer);
                    return true;
                }
#if !DEBUG
            }
            catch (KeystoneException ex)
            {
                Console.WriteLine(ex);
                return false;
            }
#endif
        }
    }
}
