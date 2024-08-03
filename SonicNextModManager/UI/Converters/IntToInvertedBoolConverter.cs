namespace SonicNextModManager.UI.Converters
{
    [ValueConversion(typeof(int), typeof(bool))]
    public class IntToInvertedBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => (int)value == 0 ? true : false;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
