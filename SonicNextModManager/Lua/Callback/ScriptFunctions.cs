using Marathon.Exceptions;
using Marathon.Formats.Script.Lua;
using SonicNextModManager.Lua.Attributes;

namespace SonicNextModManager.Lua.Callback
{
    internal class ScriptFunctions
    {
        /// <summary>
        /// Decompiles the specified Lua script.
        /// </summary>
        /// <param name="in_path">The path to the Lua script to decompile.</param>
        /// <returns><c>true</c> if the Lua script was decompiled successfully; otherwise, <c>false</c>.</returns>
        [LuaCallback]
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
    }
}
