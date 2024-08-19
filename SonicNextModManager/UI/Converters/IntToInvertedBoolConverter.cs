namespace SonicNextModManager.UI.Converters
{
    [ValueConversion(typeof(int), typeof(bool))]
    public class IntToInvertedBoolConverter : IValueConverter
    {
        public object Convert(object in_value, Type in_targetType, object in_param, CultureInfo in_culture)
        {
            return System.Convert.ToInt64(in_value) == 0 ? true : false;
        }

        public object ConvertBack(object in_value, Type in_targetType, object in_param, CultureInfo in_culture)
        {
            throw new NotImplementedException();
        }
    }
}
