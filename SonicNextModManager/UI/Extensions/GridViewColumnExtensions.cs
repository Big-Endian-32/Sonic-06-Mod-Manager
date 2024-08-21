using System.ComponentModel;

namespace SonicNextModManager.UI.Extensions
{
    public static class GridViewColumnExtensions
    {
        public static readonly DependencyProperty MinWidthProperty = DependencyProperty.RegisterAttached
        (
            "MinWidth",
            typeof(double),
            typeof(GridViewColumnExtensions),
            new UIPropertyMetadata(double.NaN, OnMinWidthChanged)
        );

        public static double GetMinWidth(DependencyObject in_obj)
        {
            return (double)in_obj.GetValue(MinWidthProperty);
        }

        public static void SetMinWidth(DependencyObject in_obj, double in_value)
        {
            in_obj.SetValue(MinWidthProperty, in_value);
        }

        private static void OnMinWidthChanged(DependencyObject in_obj, DependencyPropertyChangedEventArgs in_args)
        {
            if (in_obj is GridViewColumn out_column && in_args.NewValue is double out_minWidth)
            {
                DependencyPropertyDescriptor.FromProperty(GridViewColumn.WidthProperty, typeof(GridViewColumn))
                    .AddValueChanged(out_column, (s, e) => ClampMinWidth(out_column, out_minWidth));
            }
        }

        private static void ClampMinWidth(GridViewColumn in_column, double in_minWidth)
        {
            if (in_column.Width < in_minWidth)
                in_column.Width = in_minWidth;
        }
    }
}
