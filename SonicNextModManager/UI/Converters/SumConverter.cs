namespace SonicNextModManager.UI.Converters
{
    public class SumConverter : IValueConverter
    {
        public object Convert(object in_value, Type in_targetType, object in_param, CultureInfo in_culture)
        {
            var s1 = System.Convert.ToDecimal(in_value);
            var s2 = System.Convert.ToDecimal(in_param);

            return s1 + s2;
        }

        public object ConvertBack(object in_value, Type in_targetType, object in_param, CultureInfo in_culture)
        {
            throw new NotImplementedException();
        }
    }
}
