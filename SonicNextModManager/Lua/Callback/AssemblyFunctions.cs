using Keystone;
using SonicNextModManager.Lua.Attributes;

namespace SonicNextModManager.Lua.Callback
{
    public class AssemblyFunctions
    {
        /// <summary>
        /// Assembles PowerPC assembly into bytecode.
        /// </summary>
        /// <param name="in_code">The code to assemble.</param>
        /// <param name="in_addr">The hint address for context.</param>
        /// <returns>A byte array containing PowerPC bytecode.</returns>
        [LuaCallback]
        public static byte[] Assemble(string in_code, uint in_addr = 0)
        {
#if !DEBUG
            try
            {
#endif
                using (var keystone = new Engine(Architecture.PPC, Mode.PPC64 | Mode.BIG_ENDIAN) { ThrowOnError = true })
                    return keystone.Assemble(in_code, in_addr).Buffer;
#if !DEBUG
            }
            catch (KeystoneException ex)
            {
                Console.WriteLine(ex);
                return [];
            }
#endif
        }

        /// <summary>
        /// Writes PowerPC assembly to the specified address.
        /// </summary>
        /// <param name="in_addr">The address to write to.</param>
        /// <param name="in_code">The code to assemble.</param>
        /// <returns><c>true</c> if the operation succeeded; otherwise, <c>false</c>.</returns>
        [LuaCallback]
        public static bool WriteAsm(uint in_addr, string in_code)
        {
            using (var keystone = new Engine(Architecture.PPC, Mode.PPC64 | Mode.BIG_ENDIAN) { ThrowOnError = true })
                return MemoryFunctions.WriteBytes("Executable", in_addr, Assemble(in_code, in_addr));
        }

        /// <summary>
        /// Writes a NOP instruction to the specified address.
        /// </summary>
        /// <param name="in_addr">The address to write to.</param>
        /// <param name="in_count">The number of NOP instructions to write.</param>
        /// <returns><c>true</c> if the operation succeeded; otherwise, <c>false</c>.</returns>
        [LuaCallback]
        public static bool WriteNop(uint in_addr, int in_count = 1)
        {
            for (int i = 0; i < in_count; i++)
            {
                MemoryFunctions.WriteBytes("Executable", in_addr, [0x60, 0x00, 0x00, 0x00]);
                in_addr += 4;
            }

            return true;
        }

        /// <summary>
        /// Writes a branch instruction to the specified address.
        /// </summary>
        /// <param name="in_source">The address to write a branch instruction to.</param>
        /// <param name="in_destination">The address to branch to.</param>
        /// <returns><c>true</c> if the operation succeeded; otherwise, <c>false</c>.</returns>
        [LuaCallback]
        public static bool WriteJump(uint in_source, int in_destination)
            => WriteAsm(in_source, $"b {in_destination};");
    }
}
