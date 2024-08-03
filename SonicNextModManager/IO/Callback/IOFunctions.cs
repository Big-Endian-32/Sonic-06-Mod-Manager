using Keystone;
using Marathon.Formats.Archive;
using Marathon.Formats.Script.Lua;
using Marathon.IO;
using SonicNextModManager.Helpers;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SonicNextModManager.IO.Callback
{
	[MoonSharpUserData]
    public class IOFunctions
    {
        private static Dictionary<string, U8Archive> _archives = new();

        /// <summary>
        /// Loads an archive from the input path.
        /// </summary>
        /// <param name="path">Path to archive.</param>
        public static void LoadArchive(string path)
        {
            if (_archives.ContainsKey(path))
                _archives[path].Dispose();

            _archives[path] = new U8Archive(path, ReadMode.IndexOnly);
        }

        /// <summary>
        /// Saves an archive from the input path.
        /// </summary>
        /// <param name="path">Path to archive.</param>
        public static void SaveArchive(string path)
        {
            _archives[path].Save();
            _archives.Remove(path);
        }

        /// <summary>
        /// Writes a file to the loaded archive.
        /// </summary>
        /// <param name="path">Archive to write data to.</param>
        /// <param name="internalPath">Path to write the file to inside the archive.</param>
        /// <param name="filePath">Path to the file to read data from.</param>
        public static void WriteFile(string path, string internalPath, string filePath)
        {
            _archives[path].Root.CreateDirectories(Path.GetDirectoryName(internalPath)).Add
            (
                new U8ArchiveFile(filePath)
                {
                    Name = Path.GetFileName(internalPath)
                }
            );
        }

        /// <summary>
        /// Encrypts the current game executable (required step for PlayStation executable patching).
        /// </summary>
        public static bool EncryptExecutable()
        {
            string? executable = App.Settings.Path_GameExecutable;

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

                    return true;
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

                    return true;
                }

                default:
                    return false;
            }
        }

        /// <summary>
        /// Decrypts the current game executable.
        /// </summary>
        public static bool DecryptExecutable()
        {
            string? executable = App.Settings.Path_GameExecutable;

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

                    return true;
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

                    return true;
                }

                default:
                    return false;
            }
        }

        /// <summary>
        /// Decompiles the input Lua script.
        /// </summary>
        public static bool DecompileLua(string path)
        {
            LuaBinary lub = new(path);
            lub.Decompile();

            return false;
        }

        /// <summary>
        /// Reads bytes from the specified address and returns a hexadecimal string.
        /// </summary>
        /// <param name="file">File to read from.</param>
        /// <param name="address">Address to read from.</param>
        /// <param name="count">Amount of bytes to read.</param>
        public static string ReadBytes(string file, int address, int count)
        {
#if !DEBUG
            try
            {
#endif
                // Open the file for reading.
                using (FileStream fileStream = File.OpenRead(Patcher.GetSymbol(file)))
                {
                    using (var reader = new BinaryReader(fileStream))
                    {
                        // Seek to requested address in the stream.
                        reader.BaseStream.Seek(address, SeekOrigin.Begin);

                        // Return as a hexadecimal string.
                        return MemoryHelper.ByteArrayToHexString(reader.ReadBytes(count));
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
        /// <param name="file">File to write to.</param>
        /// <param name="address">Address to write from.</param>
        /// <param name="hex">Hexadecimal string containing the bytes to be written.</param>
        public static bool WriteBytes(string file, int address, string hex)
        {
#if !DEBUG
            try
            {
#endif
                // Open the file for writing.
                using (FileStream fileStream = File.OpenWrite(Patcher.GetSymbol(file)))
                {
                    using (var writer = new BinaryWriter(fileStream))
                    {
                        // Seek to the requested address in the stream.
                        writer.BaseStream.Seek(address, SeekOrigin.Begin);

                        // Write the specified bytes to the address.
                        writer.Write(MemoryHelper.HexStringToByteArray(hex));

                        // Log written bytes to the console.
                        Console.WriteLine($"Written bytes to 0x{address:X8}: {hex}");
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
        /// <param name="file">File to write to.</param>
        /// <param name="address">Address to write from.</param>
        /// <param name="data">Byte array containing the bytes to be written.</param>
        private static bool WriteBytes(string file, int address, byte[] data)
            => WriteBytes(file, address, MemoryHelper.ByteArrayToHexString(data));

        /// <summary>
        /// Writes a no-operation opcode to the specified address and returns whether or not it succeeded.
        /// </summary>
        /// <param name="file">File to write to.</param>
        /// <param name="address">Address to write from.</param>
        /// <param name="count">Amount of NOPs to write.</param>
        public static bool WriteNop(string file, int address, int count = 1, Keystone.Architecture arch = Keystone.Architecture.PPC, Mode archMode = Mode.PPC64 | Mode.BIG_ENDIAN)
        {
            for (int i = 0; i < count; i++)
            {
                // Write a no operation opcode to the address in the requested file.
                WriteAsm(file, address, "nop;", arch, archMode);

                // Increment the address by four bytes.
                address += 4;
            }

            return true;
        }

        /// <summary>
        /// Writes a jump instruction to the specified address and returns whether or not it succeeded.
        /// </summary>
        /// <param name="file">File to write to.</param>
        /// <param name="source">Address to write from.</param>
        /// <param name="destination">Destination to jump to.</param>
        public static bool WriteJump(string file, int source, int destination, Keystone.Architecture arch = Keystone.Architecture.PPC, Mode archMode = Mode.PPC64 | Mode.BIG_ENDIAN)
            => WriteAsm(file, source, $"b {destination};", arch, archMode);

        /// <summary>
        /// Writes a UTF8-encoded string to the specified address and returns the input string.
        /// </summary>
        /// <param name="file">File to write to.</param>
        /// <param name="address">Address to write from.</param>
        /// <param name="text">Text to write.</param>
        public static string WriteString(string file, int address, string text)
        {
            // Write the specified UTF8 string to the address in the requested file.
            WriteBytes(file, address, Encoding.UTF8.GetBytes(text));

            return text;
        }

        /// <summary>
        /// Writes compiled assembly bytecode to the specified address and returns whether or not it succeeded.
        /// </summary>
        /// <param name="file">File to write to.</param>
        /// <param name="address">Address to write from.</param>
        /// <param name="assembly">Assembly to compile.</param>
        /// <param name="arch">Compiler architecture.</param>
        /// <param name="archMode">Compiler architecture mode.</param>
        public static bool WriteAsm(string file, int address, string assembly, Keystone.Architecture arch = Keystone.Architecture.PPC, Mode archMode = Mode.PPC64 | Mode.BIG_ENDIAN)
        {
#if !DEBUG
            try
            {
#endif
                using
                (
                    var keystone = new Engine(arch, archMode)
                    {
                        ThrowOnError = true
                    }
                )
                {
                    // Write assembled bytecode to the address in the requested file.
                    WriteBytes(file, address, keystone.Assemble(assembly, (ulong)address).Buffer);

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
