namespace SonicNextModManager.UI.Converters
{
    [ValueConversion(typeof(bool), typeof(string))]
    public class BoolToYesNoConverter : IValueConverter
    {
        public object Convert(object in_value, Type in_targetType, object in_param, CultureInfo in_culture)
            => (bool)in_value ? LocaleService.Localise("Common_Yes") : LocaleService.Localise("Common_No");

        public object ConvertBack(object in_value, Type in_targetType, object in_param, CultureInfo in_culture)
            => (string)in_value == LocaleService.Localise("Common_Yes") ? true : false;
    }
}
