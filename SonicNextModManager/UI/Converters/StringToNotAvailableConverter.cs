namespace SonicNextModManager.UI.Converters
{
    public class StringToNotAvailableConverter : IValueConverter
    {
        public object Convert(object in_value, Type in_targetType, object in_param, CultureInfo in_culture)
        {
            return string.IsNullOrEmpty((string)in_value)
                ? LocaleService.Localise("Common_NotAvailable")
                : (string)in_value;
        }

        public object ConvertBack(object in_value, Type in_targetType, object in_param, CultureInfo in_culture)
        {
            throw new NotImplementedException();
        }
    }
}
