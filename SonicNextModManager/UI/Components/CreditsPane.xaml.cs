namespace SonicNextModManager.UI.Components
{
    /// <summary>
    /// Interaction logic for CreditsPane.xaml
    /// </summary>
    public partial class CreditsPane : UserControl
    {
        public static readonly DependencyProperty CreditsProperty = DependencyProperty.Register
        (
            nameof(Credits),
            typeof(Credits),
            typeof(CreditsPane),
            new PropertyMetadata()
        );

        public Credits Credits
        {
            get => (Credits)GetValue(CreditsProperty);
            set => SetValue(CreditsProperty, value);
        }

        public CreditsPane(Credits? credits)
        {
            InitializeComponent();

            if (credits == null)
                return;

            Credits = credits;
            Category.Header = LocaleService.Localise(Credits.Category);
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string url = ((Credits.Contributor)((ListViewItem)sender).Content).URL;

            // Direct to user webpage (if available) when double-clicked.
            if (!string.IsNullOrEmpty(url))
                ProcessExtensions.StartWithDefaultProgram(url);
        }
    }
}
