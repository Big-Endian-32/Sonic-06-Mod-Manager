using SonicNextModManager.Extensions;
using SonicNextModManager.Metadata;

namespace SonicNextModManager.Lua
{
    public static class Patcher
    {
        private static Dictionary<string, string> _symbols { get; } = [];

        /// <summary>
        /// Initialises the input Lua interpreter with the default exposed functions.
        /// </summary>
        /// <param name="L">Lua interpreter to initialise.</param>
        public static Script Initialise(this Script L)
        {
            L.RegisterCallbacks();
            L.RegisterDescriptors();
            L.RegisterUserData();

            // Initialise patch symbols.
            AddSymbol("Executable", App.Settings.Path_GameExecutable!);
            AddSymbol("Platform", App.GetCurrentPlatform() == EPlatform.Xbox ? "xenon" : "ps3");
            AddSymbol("Root", App.Settings.GetGameDirectory()!);

            return L;
        }

        /// <summary>
        /// Adds or replaces a new symbol to the dictionary.
        /// <para>Symbols are used to create shortcut phrases for what would otherwise be long strings.</para>
        /// </summary>
        /// <param name="in_name">The name of the symbol to add or replace.</param>
        /// <param name="in_value">The value of the symbol.</param>
        /// <returns>The value of the symbol.</returns>
        public static string AddSymbol(string in_name, string in_value)
        {
            if (_symbols.ContainsKey(in_name))
            {
                _symbols[in_name] = in_value;
                return in_value;
            }

            _symbols.Add(in_name, in_value);

            return in_value;
        }

        /// <summary>
        /// Gets the value of a symbol.
        /// </summary>
        /// <param name="in_symbol">The name of the symbol to get.</param>
        /// <returns>If it exists, the value of the specified symbol; otherwise, the symbol name.</returns>
        public static string GetSymbol(string in_name)
        {
            if (_symbols.ContainsKey(in_name))
                return _symbols[in_name];

            return in_name;
        }
    }
}
