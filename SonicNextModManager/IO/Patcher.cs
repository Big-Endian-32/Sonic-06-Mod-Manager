using SonicNextModManager.Extensions;
using SonicNextModManager.IO.Callback;

namespace SonicNextModManager.IO
{
    public static class Patcher
    {
        private static Dictionary<string, string> Symbols = new Dictionary<string, string>();

        /// <summary>
        /// Initialises the input Lua interpreter with the default exposed functions.
        /// </summary>
        /// <param name="L">Lua interpreter to initialise.</param>
        public static Script Initialise(this Script L)
        {
            // Set up callback functions.
            L.PushExposedFunctions<UtilityFunctions>();
            L.PushExposedFunctions<IOFunctions>();

            return L;
        }

        /// <summary>
        /// Adds a new symbol to the dictionary and returns the actual value.
        /// <para>Symbols are used to create shortcut phrases for what would otherwise be long strings.</para>
        /// </summary>
        /// <param name="symbol">Symbol name.</param>
        /// <param name="actualValue">Value of this symbol.</param>
        public static string AddSymbol(string symbol, string actualValue)
        {
            Symbols.Add(symbol, actualValue);

            return actualValue;
        }

        /// <summary>
        /// Returns the value of a symbol.
        /// <para>If the symbol does not exist, it'll return the input string.</para>
        /// </summary>
        /// <param name="symbol">Symbol name.</param>
        public static string GetSymbol(string symbol)
        {
            if (Symbols.ContainsKey(symbol))
                return Symbols[symbol];

            return symbol;
        }
    }
}
