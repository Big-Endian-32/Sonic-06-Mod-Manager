namespace SonicNextModManager.Helpers
{
    public class TypeHelper
    {
        public static IEnumerable<Type> GetDerivedInterfaces<T>()
        {
            return Assembly.GetExecutingAssembly().GetTypes()
                .Where(x => typeof(T).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract);
        }
    }
}
