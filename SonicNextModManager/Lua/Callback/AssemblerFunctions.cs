using SonicNextModManager.Lua.Attributes;
using SonicNextModManager.Patches;

namespace SonicNextModManager.Lua.Callback
{
    public class AssemblerFunctions
    {
        /// <summary>
        /// Assembles PowerPC assembly into bytecode.
        /// </summary>
        /// <param name="in_code">The code to assemble.</param>
        /// <param name="in_addr">The hint address for context.</param>
        /// <returns>A byte array containing PowerPC bytecode.</returns>
        [LuaCallback]
        public static byte[] Assemble(Script L, string in_code, uint in_addr = 0)
        {
            return AssemblerService.Assemble(in_code, in_addr);
        }

        /// <summary>
        /// Writes PowerPC assembly to the specified address.
        /// </summary>
        /// <param name="in_addr">The address to write to.</param>
        /// <param name="in_code">The code to assemble.</param>
        /// <returns><c>true</c> if the operation succeeded; otherwise, <c>false</c>.</returns>
        [LuaCallback]
        public static bool WriteAsm(Script L, uint in_addr, string in_code)
        {
            var gameExecutable = App.Settings.Path_GameExecutable;

            if (string.IsNullOrEmpty(gameExecutable))
                return false;

            using var fs = File.OpenWrite(gameExecutable);

            return new MemoryService(fs).WriteBytes(in_addr, Assemble(L, in_code, in_addr));
        }

        /// <summary>
        /// Creates a mid-asm hook.
        /// </summary>
        /// <param name="in_addr">The address to hook.</param>
        /// <param name="in_code">The code to assemble.</param>
        [LuaCallback]
        public static void WriteAsmHook(Script L, uint in_addr, string in_code)
        {
            Bootstrapper.Hooks.Add(new(in_addr, Assemble(L, in_code, in_addr)));
        }

        /// <summary>
        /// Writes a NOP instruction to the specified address.
        /// </summary>
        /// <param name="in_addr">The address to write to.</param>
        /// <param name="in_count">The number of NOP instructions to write.</param>
        /// <returns><c>true</c> if the operation succeeded; otherwise, <c>false</c>.</returns>
        [LuaCallback]
        public static bool WriteNop(Script L, uint in_addr, int in_count = 1)
        {
            var gameExecutable = App.Settings.Path_GameExecutable;

            if (string.IsNullOrEmpty(gameExecutable))
                return false;

            using (var fs = File.OpenWrite(gameExecutable))
            {
                var ms = new MemoryService(fs);

                for (int i = 0; i < in_count; i++)
                {
                    ms.WriteBytes(in_addr, [0x60, 0x00, 0x00, 0x00]);
                    in_addr += 4;
                }
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
        public static bool WriteJump(Script L, uint in_source, int in_destination)
        {
            // TODO: support far jumps.
            return WriteAsm(L, in_source, $"b {in_destination};");
        }
    }
}
