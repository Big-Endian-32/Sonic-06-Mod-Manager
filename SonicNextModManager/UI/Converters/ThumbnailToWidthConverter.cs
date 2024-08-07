namespace SonicNextModManager.UI.Converters
{
    [ValueConversion(typeof(string), typeof(int))]
    public class ThumbnailToWidthConverter : IValueConverter
    {
        public object Convert(object in_value, Type in_targetType, object in_param, CultureInfo in_culture)
        {
            return string.IsNullOrEmpty((string)in_value) ? 0 : 320;
        }

        public object ConvertBack(object in_value, Type in_targetType, object in_param, CultureInfo in_culture)
        {
            throw new NotImplementedException();
        }
    }
}
