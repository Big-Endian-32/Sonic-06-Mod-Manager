using SonicNextModManager.Lua.Attributes;

namespace SonicNextModManager.Lua
{
    [LuaEnum]
    public enum EReadMode
    {
        /// <summary>
        /// Streams changes directly to the loaded file.
        /// </summary>
        Stream,

        /// <summary>
        /// Copies the file to a new buffer and writes the changes upon saving manually.
        /// </summary>
        Copy
    }
}
