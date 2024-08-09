namespace SonicNextModManager.Extensions
{
    public static class TypeExtensions
    {
        public static bool IsStruct(this Type in_type)
        {
            return in_type.IsValueType && !in_type.IsPrimitive && !in_type.IsEnum;
        }

        public static MemberInfo[] GetAllMembers(this Type in_type)
        {
            var properties = in_type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var fields = in_type.GetFields(BindingFlags.Public | BindingFlags.Instance);

            var result = new MemberInfo[properties.Length + fields.Length];

            properties.CopyTo(result, 0);
            fields.CopyTo(result, properties.Length);

            return result;
        }
    }
}
