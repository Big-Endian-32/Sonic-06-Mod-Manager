namespace SonicNextModManager.Extensions
{
    public static class MoonSharpExtensions
    {
        /// <summary>
        /// Pushes all public static methods to Lua globals from a class.
        /// </summary>
        /// <typeparam name="T">The type of class to push from.</typeparam>
        /// <param name="in_script">The script to push to.</param>
        public static void PushExposedFunctions<T>(this Script in_script)
        {
            foreach (MethodInfo methodInfo in typeof(T).GetMethods(BindingFlags.Public | BindingFlags.Static))
                in_script.Globals[methodInfo.Name] = methodInfo;
        }
    }
}
