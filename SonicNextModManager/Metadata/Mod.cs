using System.Collections.ObjectModel;

namespace SonicNextModManager
{
    public class Mod : Metadata
    {
        /// <summary>
        /// The version string for this mod.
        /// </summary>
        public string? Version { get; set; }

        /// <summary>
        /// A collection of patches required by this mod.
        /// </summary>
        public ObservableCollection<string> Patches { get; set; } = new();

        public Mod Parse(string file)
        {
            Mod metadata = JsonConvert.DeserializeObject<Mod>(File.ReadAllText(file));

            // Set metadata path.
            metadata.Path = file;

            // Get single thumbnail and use that as the path.
            if (DirectoryExtensions.Contains(System.IO.Path.GetDirectoryName(file), "thumbnail.*", out string thumbnail))
                metadata.Thumbnail = thumbnail;

            return metadata;
        }

        public void Write(Mod mod, string file)
            => File.WriteAllText(file, JsonConvert.SerializeObject(mod, Formatting.Indented));

        public void Write(string file)
            => Write(this, file);

        public void Write()
			=> Write(this, Path);

        public void Install()
        {
            // TODO
        }
    }

    [ValueConversion(typeof(string), typeof(int))]
    public class Thumbnail2WidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => string.IsNullOrEmpty((string)value) ? 0 : 320;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
