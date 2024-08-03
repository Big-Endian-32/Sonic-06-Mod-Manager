namespace SonicNextModManager.Extensions
{
    public static class LuaExtensions
    {
        /// <summary>
        /// Pushes all public static methods to Lua globals from a class as a generic type.
        /// </summary>
        /// <typeparam name="T">Class to pull methods from.</typeparam>
        /// <param name="L">Lua interpreter.</param>
        public static void PushExposedFunctions<T>(this Script L)
        {
            // Get all methods that meet the binding flags from the class as a generic type.
            foreach (MethodInfo methodInfo in typeof(T).GetMethods(BindingFlags.Public | BindingFlags.Static))
            {
                // Create new callback function using method info.
                L.Globals[methodInfo.Name] = methodInfo;
            }
        }
    }
}
