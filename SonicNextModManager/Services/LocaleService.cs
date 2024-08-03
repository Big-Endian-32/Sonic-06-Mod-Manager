namespace SonicNextModManager.Services
{
    public class LocaleService
    {
        /// <summary>
        /// Returns a localised string from the input resource name.
        /// </summary>
        /// <param name="key">Resource name.</param>
        public static string Localise(string key)
        {
            var resource = Application.Current.TryFindResource(key);

            if (resource is string str)
                return str.Replace("\\n", "\n");

            return key;
        }

        /// <summary>
        /// Returns a formatted localised string from the input resources.
        /// </summary>
        /// <param name="key">Resource name.</param>
        /// <param name="args">Formatting instructions.</param>
        public static string Localise(string key, params object[] args)
            => string.Format(Localise(key), args);
    }
}
