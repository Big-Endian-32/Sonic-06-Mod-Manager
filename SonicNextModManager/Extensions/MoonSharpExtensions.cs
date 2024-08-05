using SonicNextModManager.Lua.Attributes;
using SonicNextModManager.Lua.Interfaces;

namespace SonicNextModManager.Extensions
{
    public static class MoonSharpExtensions
    {
        /// <summary>
        /// Registers all methods marked as Lua callback functions with the interpreter.
        /// </summary>
        /// <param name="L">The script to push to.</param>
        public static void RegisterCallbacks(this Script L)
        {
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                foreach (var methodInfo in type.GetMethods())
                {
                    if (methodInfo.GetCustomAttributes(typeof(LuaCallbackAttribute), false).Any())
                        L.Globals[methodInfo.Name] = methodInfo;
                }
            }
        }

        /// <summary>
        /// Registers all classes marked as Lua user data with the interpreter.
        /// </summary>
        /// <param name="L">The script to push to.</param>
        public static void RegisterDescriptors(this Script L)
        {
            var @interface = typeof(ILuaUserDataDescriptor);

            var types = Assembly.GetExecutingAssembly().GetTypes()
                .Where(x => @interface.IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract);

            foreach (var type in types)
            {
                var instance = Activator.CreateInstance(type) as ILuaUserDataDescriptor;
                var register = @interface.GetMethod("Register");

                if (register == null)
                    continue;

                register.Invoke(instance, null);
            }
        }

        /// <summary>
        /// Registers all classes marked as Lua user data with the interpreter.
        /// </summary>
        /// <param name="L">The script to push to.</param>
        public static void RegisterUserData(this Script L)
        {
            var types = Assembly.GetExecutingAssembly().GetTypes()
                .Where(x => x.GetCustomAttributes(typeof(LuaUserDataAttribute), false).Length > 0);

            foreach (var type in types)
                UserData.RegisterType(type);
        }
    }
}
