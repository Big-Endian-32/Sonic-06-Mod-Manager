namespace SonicNextModManager.Helpers
{
    public class TypeHelper
    {
        public static IEnumerable<Type> GetDerivedInterfaces<T>()
        {
            return Assembly.GetExecutingAssembly().GetTypes()
                .Where(x => typeof(T).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract);
        }

        public static bool TryParse(string in_typeName, out Type? out_type)
        {
            if (Enum.TryParse(typeof(TypeCode), in_typeName, true, out var out_typeCode))
            {
                out_type = Type.GetType($"System.{out_typeCode}")!;

                if (out_type == null)
                    return false;

                return true;
            }

            switch (in_typeName)
            {
                case "bool":    out_type = typeof(bool);    break;
                case "char":    out_type = typeof(char);    break;
                case "sbyte":   out_type = typeof(sbyte);   break;
                case "byte":    out_type = typeof(byte);    break;
                case "short":   out_type = typeof(short);   break;
                case "ushort":  out_type = typeof(ushort);  break;
                case "int":     out_type = typeof(int);     break;
                case "uint":    out_type = typeof(uint);    break;
                case "long":    out_type = typeof(long);    break;
                case "ulong":   out_type = typeof(ulong);   break;
                case "float":   out_type = typeof(float);   break;
                case "double":  out_type = typeof(double);  break;
                case "decimal": out_type = typeof(decimal); break;

                default:
                    out_type = null;
                    return false;
            }

            return true;
        }
    }
}
