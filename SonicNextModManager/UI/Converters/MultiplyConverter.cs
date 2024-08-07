namespace SonicNextModManager.UI.Converters
{
    public class MultiplyConverter : IMultiValueConverter
    {
        public object Convert(object[] in_values, Type in_targetType, object in_param, CultureInfo in_culture)
        {
            double result = 1.0;

            for (int i = 0; i < in_values.Length; i++)
            {
                if (in_values[i] is double)
                    result *= (double)in_values[i];
            }

            return result;
        }

        public object[] ConvertBack(object in_value, Type[] in_targetTypes, object in_param, CultureInfo in_culture)
        {
            throw new NotImplementedException();
        }
    }
}
