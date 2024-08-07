﻿namespace SonicNextModManager.UI.Converters
{
    [ValueConversion(typeof(EInstallState), typeof(bool))]
    public class InstallStateToBoolConverter : IValueConverter
    {
        public object Convert(object in_value, Type in_targetType, object in_param, CultureInfo in_culture)
        {
            if (in_param is EInstallState out_state)
            {
                // Treat InstallState.Uninstalling the same as InstallState.Installing for this context.
                if ((out_state == EInstallState.Installing || out_state == EInstallState.Uninstalling) &&
                    (in_value.Equals(EInstallState.Installing) || in_value.Equals(EInstallState.Uninstalling)))
                {
                    return true;
                }
            }

            return in_value.Equals(in_param ?? EInstallState.Idle) ? true : false;
        }

        public object ConvertBack(object in_value, Type in_targetType, object in_param, CultureInfo in_culture)
        {
            throw new NotImplementedException();
        }
    }
}
