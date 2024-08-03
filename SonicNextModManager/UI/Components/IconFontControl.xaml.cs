namespace SonicNextModManager.UI.Components
{
    /// <summary>
    /// Interaction logic for IconFontControl.xaml
    /// </summary>
    public partial class IconFontControl : UserControl
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register
        (
            nameof(Text),
            typeof(string),
            typeof(IconFontControl),
            new PropertyMetadata("block")
        );

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public IconFontControl()
        {
            InitializeComponent();
        }
    }
}
