namespace SonicNextModManager.UI.Converters
{
    public class StringToNotAvailableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => string.IsNullOrEmpty((string)value) ? LocaleService.Localise("Common_NotAvailable") : (string)value;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
