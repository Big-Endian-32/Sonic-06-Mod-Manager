using SonicNextModManager.Helpers;

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

        public CreditsPane(Credits? in_credits)
        {
            InitializeComponent();

            if (in_credits == null)
                return;

            Credits = in_credits;
            Category.Header = LocaleService.Localise(Credits.Category);
        }

        private void ListViewItem_MouseDoubleClick(object in_sender, MouseButtonEventArgs in_args)
        {
            string url = ((Credits.Contributor)((ListViewItem)in_sender).Content).URL;

            // Direct to user webpage (if available) when double-clicked.
            if (!string.IsNullOrEmpty(url))
                ProcessHelper.StartWithDefaultProgram(url);
        }
    }
}
