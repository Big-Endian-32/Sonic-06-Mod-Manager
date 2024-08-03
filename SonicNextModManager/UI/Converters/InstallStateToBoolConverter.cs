namespace SonicNextModManager.UI.Converters
{
    [ValueConversion(typeof(InstallState), typeof(bool))]
    public class InstallStateToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value.Equals(InstallState.Idle) ? false : true;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
