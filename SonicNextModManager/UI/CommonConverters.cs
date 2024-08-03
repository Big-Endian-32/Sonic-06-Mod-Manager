using SonicNextModManager.Services;

namespace SonicNextModManager
{
    [ValueConversion(typeof(bool), typeof(string))]
    public class Boolean2YesNoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => (bool)value ? LocaleService.Localise("Common_Yes") : LocaleService.Localise("Common_No");

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => (string)value == LocaleService.Localise("Common_Yes") ? true : false;
    }

    [ValueConversion(typeof(int), typeof(bool))]
    public class Int2InvertedBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => (int)value == 0 ? true : false;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class String2NotAvailableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => string.IsNullOrEmpty((string)value) ? LocaleService.Localise("Common_NotAvailable") : (string)value;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class MultiplyConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double result = 1.0;

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] is double)
                    result *= (double)values[i];
            }

            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    [ValueConversion(typeof(Enum), typeof(int))]
    public class Enum2IntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => (int)value;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
