using SonicNextModManager.Logger;
using System.Windows.Media;

namespace SonicNextModManager.UI.Converters
{
    public class LogLevelToBackgroundBrushConverter : IValueConverter
    {
        public object Convert(object in_value, Type in_targetType, object in_param, CultureInfo in_culture)
        {
            if (in_value is ELogLevel out_logLevel)
            {
                return out_logLevel switch
                {
                    ELogLevel.Utility => new SolidColorBrush(Color.FromRgb(0, 48, 0)),
                    ELogLevel.Warning => new SolidColorBrush(Color.FromRgb(48, 32, 0)),
                    ELogLevel.Error   => new SolidColorBrush(Color.FromRgb(56, 0, 0)),
                    _                 => new SolidColorBrush(Color.FromRgb(32, 32, 32))
                };
            }

            throw new NotSupportedException();
        }

        public object ConvertBack(object in_value, Type in_targetType, object in_param, CultureInfo in_culture)
        {
            throw new NotImplementedException();
        }
    }
}
