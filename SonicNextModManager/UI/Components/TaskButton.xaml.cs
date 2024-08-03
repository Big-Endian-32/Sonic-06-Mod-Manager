namespace SonicNextModManager.UI.Components
{
    /// <summary>
    /// Interaction logic for TaskButton.xaml
    /// </summary>
    public partial class TaskButton : UserControl
    {
        public static readonly DependencyProperty CaptionProperty = DependencyProperty.Register
        (
            nameof(Caption),
            typeof(string),
            typeof(TaskButton),
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
             typeof(TaskButton),
             new PropertyMetadata("")
        );

        public string Description
        {
            get => (string)GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }

        public static readonly DependencyProperty IconNameProperty = DependencyProperty.Register
        (
            nameof(IconName),
            typeof(string),
            typeof(TaskButton),
            new PropertyMetadata("")
        );

        public string IconName
        {
            get => (string)GetValue(IconNameProperty);
            set => SetValue(IconNameProperty, value);
        }

        public TaskButton()
        {
            InitializeComponent();
        }

        public TaskButton(string caption, string description = "", string iconName = "")
        {
            InitializeComponent();

            Caption = caption;
            Description = description;
            IconName = iconName;
        }
    }
}
