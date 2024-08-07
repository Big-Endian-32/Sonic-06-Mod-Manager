namespace SonicNextModManager.UI.Converters
{
    [ValueConversion(typeof(Enum), typeof(int))]
    public class EnumToIntConverter : IValueConverter
    {
        public object Convert(object in_value, Type in_targetType, object in_param, CultureInfo in_culture)
        {
            return System.Convert.ToInt32(in_value);
        }

        public object ConvertBack(object in_value, Type in_targetType, object in_param, CultureInfo in_culture)
        {
            return Enum.ToObject(in_targetType, in_value);
        }
    }
}