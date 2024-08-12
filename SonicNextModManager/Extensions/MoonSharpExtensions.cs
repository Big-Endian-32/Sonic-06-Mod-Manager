using SonicNextModManager.Helpers;
using SonicNextModManager.Lua.Attributes;
using SonicNextModManager.Lua.Interfaces;
using SonicNextModManager.Lua.Syntax.Interfaces;

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
        /// Registers all classes derived from ILuaUserDataDescriptor with the interpreter.
        /// </summary>
        /// <param name="L">The script to push to.</param>
        public static void RegisterDescriptors(this Script L)
        {
            foreach (var type in TypeHelper.GetDerivedInterfaces<ILuaUserDataDescriptor>())
            {
                var instance = Activator.CreateInstance(type) as ILuaUserDataDescriptor;
                var register = typeof(ILuaUserDataDescriptor).GetMethod("Register");

                if (register == null)
                    continue;

                register.Invoke(instance, [L]);
            }
        }

        /// <summary>
        /// Registers all classes marked as Lua user data with the interpreter.
        /// </summary>
        public static void RegisterUserData(this Script L)
        {
            var types = Assembly.GetExecutingAssembly().GetTypes()
                .Where(x => x.GetCustomAttributes(typeof(LuaUserDataAttribute), false).Length > 0);

            foreach (var type in types)
                UserData.RegisterType(type);
        }

        /// <summary>
        /// Registers all enums marked as Lua enums with the interpreter.
        /// </summary>
        /// <param name="L">The script to push to.</param>
        public static void RegisterEnums(this Script L)
        {
            var types = Assembly.GetExecutingAssembly().GetTypes()
                .Where(x => x.GetCustomAttributes(typeof(LuaEnumAttribute), false).Length > 0);

            foreach (var type in types)
                L.RegisterEnum(type);
        }

        /// <summary>
        /// Registers an enum with the interpreter.
        /// </summary>
        /// <param name="L">The script to push to.</param>
        /// <param name="in_type">The enum type to register.</param>
        public static void RegisterEnum(this Script L, Type in_type, string in_name = "")
        {
            if (!in_type.IsEnum)
                throw new ArgumentException("The specified type is not an enum.");

            var name = string.IsNullOrEmpty(in_name)
                ? in_type.Name
                : in_name;

            var underlyingType = Enum.GetUnderlyingType(in_type);

            L.Globals[name] = DynValue.NewTable(L);

            foreach (var value in Enum.GetValues(in_type))
            {
                ((Table)L.Globals[name])[value.ToString()] =
                    DynValue.NewNumber(Convert.ToDouble(Convert.ChangeType(value, underlyingType)));
            }
        }

        /// <summary>
        /// Registers an enum with the interpreter.
        /// </summary>
        /// <typeparam name="T">The enum type to register.</typeparam>
        /// <param name="L">The script to push to.</param>
        public static void RegisterEnum<T>(this Script L, string in_name = "")
        {
            RegisterEnum(L, typeof(T), in_name);
        }

        /// <summary>
        /// Registers a type and its constructors with the interpreter.
        /// </summary>
        /// <typeparam name="T">The type to register.</typeparam>
        /// <param name="L">The script to push to.</param>
        public static void RegisterType<T>(this Script L, string in_name = "")
        {
            if (typeof(T).IsEnum)
            {
                L.RegisterEnum<T>(in_name);
                return;
            }

            UserData.RegisterType<T>();

            var ctors = typeof(T).GetConstructors(BindingFlags.Public | BindingFlags.Instance);

            foreach (var ctor in ctors)
            {
                var parameters = ctor.GetParameters();
                var parameterTypes = parameters.Select(x => x.ParameterType).ToArray();

                var ctorDelegate = (Func<object[]>)
                (
                    () =>
                    {
                        var args = parameters.Select((p, i) => L.Globals[$"a{i}"]).ToArray();

                        return [ctor.Invoke(args)];
                    }
                );

                L.Globals[string.IsNullOrEmpty(in_name) ? typeof(T).Name : in_name] = ctorDelegate;
            }
        }

        /// <summary>
        /// Parses a class from a Lua table.
        /// </summary>
        /// <param name="in_value">The Lua table to parse.</param>
        /// <param name="in_type">The type to extract.</param>
        public static object ParseClassFromDynValue(this DynValue in_value, Type in_type)
        {
            if (in_value.Type == DataType.UserData)
                return in_value.ToObject(in_type);

            if (in_value.Type != DataType.Table)
                throw new ArgumentException("The DynValue is not a Lua table.");

            var instance = Activator.CreateInstance(in_type);
            var members = in_type.GetAllMembers();

            foreach (var member in members)
            {
                if (in_value.Table.RawGet(member.Name) is DynValue out_value)
                {
                    if (member is PropertyInfo out_property)
                    {
                        out_property.SetValue(instance, out_value.TransformDynValueToCLRType(out_property.PropertyType));
                    }
                    else if (member is FieldInfo out_field)
                    {
                        out_field.SetValue(instance, out_value.TransformDynValueToCLRType(out_field.FieldType));
                    }
                }
            }

            return instance!;
        }

        /// <summary>
        /// Parses a class from a Lua table.
        /// </summary>
        /// <typeparam name="T">The type to extract.</typeparam>
        /// <param name="in_value">The Lua table to parse.</param>
        public static T ParseClassFromDynValue<T>(this DynValue in_value) where T : new()
        {
            return (T)in_value.ParseClassFromDynValue(typeof(T));
        }

        /// <summary>
        /// Transforms the value of a <see cref="DynValue"/> to a CLR type.
        /// </summary>
        /// <param name="in_value">The Lua type to parse.</param>
        /// <param name="in_type">The CLR type to cast to.</param>
        public static object TransformDynValueToCLRType(this DynValue in_value, Type in_type)
        {
            if (in_type.IsGenericType)
            {
                if (in_type.GetGenericTypeDefinition() == typeof(List<>))
                {
                    var itemType = in_type.GetGenericArguments()[0];

                    var listType = typeof(List<>).MakeGenericType(itemType);
                    var listInstance = Activator.CreateInstance(listType);

                    var addMethod = listType.GetMethod("Add");

                    foreach (var key in in_value.Table.Keys)
                    {
                        if (key.Type == DataType.Number)
                        {
                            var itemValue = in_value.Table.RawGet(key);

                            addMethod.Invoke(listInstance, [itemValue.TransformDynValueToCLRType(itemType)]);
                        }
                    }

                    return listInstance;
                }
            }
            else if (in_type == typeof(string))
            {
                return in_value.String;
            }
            else if
            (
                in_type == typeof(sbyte) || in_type == typeof(byte)   ||
                in_type == typeof(short) || in_type == typeof(ushort) ||
                in_type == typeof(int)   || in_type == typeof(uint)   ||
                in_type == typeof(long)  || in_type == typeof(ulong)  ||
                in_type == typeof(float) || in_type == typeof(double)
            )
            {
                return Convert.ChangeType(in_value.Number, in_type);
            }
            else if (in_type == typeof(bool))
            {
                return in_value.Boolean;
            }
            else if (in_type == typeof(object))
            {
                return in_value.ToObject();
            }
            else if (in_value.Type == DataType.Table && (in_type.IsClass || in_type.IsStruct()))
            {
                return in_value.ParseClassFromDynValue(in_type);
            }
            else if (in_type.IsEnum)
            {
                return Enum.ToObject(in_type, (int)in_value.Number);
            }
            else if (in_type.IsArray)
            {
                return TransformDynValueToCLRTypeArray(in_value, in_type);
            }

            throw new NotSupportedException($"Unsupported type: {in_type}");
        }

        /// <summary>
        /// Transforms the value of a <see cref="DynValue"/> to an array of CLR types.
        /// </summary>
        /// <param name="in_value">The Lua type to parse.</param>
        /// <param name="in_type">The CLR types to cast to.</param>
        public static object[] TransformDynValueToCLRTypeArray(this DynValue in_value, Type in_type)
        {
            if (in_value.Type != DataType.Table)
                throw new ArgumentException("The DynValue is not a Lua table.");

            if (in_type.IsArray)
                throw new ArgumentException("The target CLR type cannot be an array.");

            var arr = new object[in_value.Table.Length];

            for (int i = 0; i < arr.Length; i++)
                arr[i] = TransformDynValueToCLRType(in_value.Table.Get(i + 1), in_type);

            return arr;
        }

        /// <summary>
        /// Patches the input string with all types derived from <see cref="ICustomSyntax"/>.
        /// </summary>
        /// <param name="in_code">The code to patch.</param>
        public static string InstallCustomSyntax(this Script L, string in_code)
        {
            var code = in_code;

            foreach (var type in TypeHelper.GetDerivedInterfaces<ICustomSyntax>())
            {
                var instance = Activator.CreateInstance(type) as ICustomSyntax;
                var install = typeof(ICustomSyntax).GetMethod("Install");

                if (install == null)
                    continue;

                code = (string)install.Invoke(instance, [code])!;
            }

            return code;
        }

        /// <summary>
        /// Determines whether the <see cref="DynValue"/> is an array table.
        /// </summary>
        /// <param name="in_value">The value to check.</param>
        public static bool IsArray(this DynValue in_value)
        {
            return in_value.Type == DataType.Table && in_value.GetLength().CastToNumber() > 0;
        }
    }
}
