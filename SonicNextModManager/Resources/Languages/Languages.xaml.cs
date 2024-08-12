using HandyControl.Tools;

namespace SonicNextModManager
{
    public class Languages : List<Language>
    {
        public static int Count;
    }

    public class Language
    {
        public string FileName { get; set; }

        public string Name { get; set; }

        public int Lines { get; set; }

        public override string ToString() => Lines != Languages.Count
            ? $"{Name} ({(float)Lines / Languages.Count * 100:N0}%)"
            : Name;

        public static void Load(string culture)
        {
            ResourceDictionary langDict = new()
            {
                Source = new Uri($"Resources/Languages/{culture}.xaml", UriKind.Relative)
            };

            while (Application.Current.Resources.MergedDictionaries.Count > 5)
            {
                Application.Current.Resources.MergedDictionaries.RemoveAt(5);
            }

            try
            {
                // Set HandyControl language.
                ConfigHelper.Instance.SetLang(culture);
            }
            catch
            {
                // Fall back on English (United Kingdom) if the selected culture is invalid.
                ConfigHelper.Instance.SetLang("en-GB");
            }

            // No need to load the fallback language on top.
            if (culture == "en-GB")
                return;

            Application.Current.Resources.MergedDictionaries.Add(langDict);
        }

        /// <summary>
        /// Loads the requested culture resources from user settings.
        /// </summary>
        public static void LoadCultureResources()
        {
            // Find Languages resource from Languages.xaml.
            object? resource = Application.Current.TryFindResource("Languages");

            // Initialise supported cultures list.
            if (resource is Languages langs)
                App.SupportedCultures = langs;

            // Load language setting as current language.
            App.CurrentCulture = GetClosestCulture(App.Settings.General_Language);

            // Set current language.
            if (App.CurrentCulture != null)
                Load(App.CurrentCulture.FileName);

            // Register additional code pages.
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        /// <summary>
        /// Loads the updated culture and updates the user setting.
        /// </summary>
        public static void UpdateCultureResources()
            => Load(App.Settings.General_Language = App.CurrentCulture.FileName);

        /// <summary>
        /// Returns the closest supported culture.
        /// </summary>
        /// <param name="culture">Culture to find.</param>
        public static Language GetClosestCulture(string culture)
        {
            // Check if the culture exists.
            var cultureEntry = App.SupportedCultures.FirstOrDefault(t => t.FileName == culture);

            // Return first culture.
            if (cultureEntry != null)
                return cultureEntry;

            // Find another language based off culture.
            string language = culture.Split('-')[0];
            cultureEntry = App.SupportedCultures.FirstOrDefault(t => t.FileName.Split('-')[0] == language);
            cultureEntry ??= App.SupportedCultures.First();

            return cultureEntry;
        }
    }
}
