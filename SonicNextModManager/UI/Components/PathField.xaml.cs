namespace SonicNextModManager.UI.Components
{
    /// <summary>
    /// Interaction logic for PathField.xaml
    /// </summary>
    public partial class PathField : UserControl
    {
        public static readonly DependencyProperty CaptionProperty = DependencyProperty.Register
        (
            nameof(Caption),
            typeof(string),
            typeof(PathField),
            new PropertyMetadata("Placeholder")
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
            typeof(PathField),
            new PropertyMetadata("A long placeholder description.")
        );

        public string Description
        {
            get => (string)GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }

        public static readonly DependencyProperty PropertyProperty = DependencyProperty.Register
        (
            nameof(Property),
            typeof(string),
            typeof(PathField),
            new PropertyMetadata()
        );

        public string Property
        {
            get => (string)GetValue(PropertyProperty);
            set => SetValue(PropertyProperty, value);
        }

        public event EventHandler? Browse;

        public PathField()
        {
            InitializeComponent();
        }

        private void Browse_Click(object in_sender, RoutedEventArgs in_args)
            => Browse?.Invoke(in_sender, in_args);
    }
}
