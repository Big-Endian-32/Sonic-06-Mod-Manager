namespace SonicNextModManager.UI.Components
{
    /// <summary>
    /// Interaction logic for ComboBoxField.xaml
    /// </summary>
    public partial class HorizontalFieldLabel : UserControl
    {
        public static readonly DependencyProperty CaptionProperty = DependencyProperty.Register
        (
            nameof(Caption),
            typeof(string),
            typeof(HorizontalFieldLabel),
            new PropertyMetadata("")
        );

        public string Caption
        {
            get => (string)GetValue(CaptionProperty);
            set => SetValue(CaptionProperty, value);
        }

        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register
        (
            nameof(Description),
            typeof(string),
            typeof(HorizontalFieldLabel),
            new PropertyMetadata("")
        );

        public string Description
        {
            get => (string)GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }

        public HorizontalFieldLabel()
        {
            InitializeComponent();
        }
    }
}
