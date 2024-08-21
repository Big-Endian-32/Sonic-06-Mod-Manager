using SonicNextModManager.Logger;
using System.Windows.Media;

namespace SonicNextModManager.UI.Converters
{
    public class LogLevelToForegroundBrushConverter : IValueConverter
    {
        public object Convert(object in_value, Type in_targetType, object in_param, CultureInfo in_culture)
        {
            if (in_value is ELogLevel out_logLevel)
            {
                return out_logLevel switch
                {
                    ELogLevel.Utility => Brushes.LimeGreen,
                    ELogLevel.Warning => Brushes.Yellow,
                    ELogLevel.Error   => Brushes.Red,
                    _                 => Brushes.White
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
