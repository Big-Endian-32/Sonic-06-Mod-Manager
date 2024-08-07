namespace SonicNextModManager.UI.Converters
{
    [ValueConversion(typeof(DateTime), typeof(string))]
    public class DateTimeConverter : IValueConverter
    {
        public object Convert(object in_value, Type in_targetType, object in_param, CultureInfo in_culture)
        {
            if (in_value is string out_str)
            {
                if (DateTime.TryParse(out_str, out var out_result))
                    return out_result;
            }

            return in_value;
        }

        public object ConvertBack(object in_value, Type in_targetType, object in_param, CultureInfo in_culture)
        {
            if (in_value is DateTime out_dt)
                return out_dt.ToString(LocaleService.Localise("Common_DateFormat"));

            return in_value;
        }
    }
}
