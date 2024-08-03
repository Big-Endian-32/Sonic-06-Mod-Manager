namespace SonicNextModManager.UI.Converters
{
    [ValueConversion(typeof(string), typeof(int))]
    public class ThumbnailToWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => string.IsNullOrEmpty((string)value) ? 0 : 320;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
