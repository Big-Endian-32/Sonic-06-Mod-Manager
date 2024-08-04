namespace SonicNextModManager.UI.Converters
{
    [ValueConversion(typeof(InstallState), typeof(Visibility))]
    public class InstallStateToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is InstallState out_state)
            {
                // Treat InstallState.Uninstalling the same as InstallState.Installing for this context.
                if ((out_state == InstallState.Installing || out_state == InstallState.Uninstalling) &&
                    (value.Equals(InstallState.Installing) || value.Equals(InstallState.Uninstalling)))
                {
                    return Visibility.Visible;
                }
            }

            return value.Equals(parameter) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
