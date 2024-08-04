namespace SonicNextModManager.UI.Converters
{
    [ValueConversion(typeof(Enum), typeof(int))]
    public class EnumToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => System.Convert.ToInt32(value);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => Enum.ToObject(targetType, value);
    }
}