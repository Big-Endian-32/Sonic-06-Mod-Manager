using SonicNextModManager.Helpers;
using SonicNextModManager.UI.Components;
using System.Threading;
using System.Threading.Tasks;

namespace SonicNextModManager.UI.Dialogs
{
    /// <summary>
    /// Interaction logic for ProgressDialog.xaml
    /// </summary>
    public partial class ProgressDialog : ImmersiveWindow
    {
        private CancellationTokenSource _cts = new();

        public static readonly DependencyProperty CaptionProperty = DependencyProperty.Register
        (
            nameof(Caption),
            typeof(string),
            typeof(ProgressDialog),
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
            typeof(ProgressDialog),
            new PropertyMetadata("Placeholder")
        );

        public string Description
        {
            get => (string)GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }

        public static readonly DependencyProperty ProgressProperty = DependencyProperty.Register
        (
            nameof(Progress),
            typeof(double),
            typeof(ProgressDialog),
            new PropertyMetadata(0.0)
        );

        public double Progress
        {
            get => (double)GetValue(ProgressProperty);
            set => SetValue(ProgressProperty, value);
        }

        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register
        (
            nameof(Minimum),
            typeof(double),
            typeof(ProgressDialog),
            new PropertyMetadata(0.0)
        );

        public double Minimum
        {
            get => (double)GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }

        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register
        (
            nameof(Maximum),
            typeof(double),
            typeof(ProgressDialog),
            new PropertyMetadata(100.0)
        );

        public double Maximum
        {
            get => (double)GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }

        public Action<CancellationToken> Callback { get; set; }

        public ProgressDialog()
        {
            // Set owner to last active window calling this class.
            Owner = WindowHelper.GetActiveWindow();

            InitializeComponent();

            Loaded += ProgressDialog_Loaded;

            DataContext = this;
        }

        public ProgressDialog(string in_desc, string in_caption = "Sonic '06 Mod Manager") : this()
        {
            Description = LocaleService.Localise(in_desc);
            Caption = LocaleService.Localise(in_caption);
        }

        private void ProgressDialog_Loaded(object in_sender, RoutedEventArgs in_args)
        {
            if (Owner == null)
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }

            if (Callback != null)
            {
                Task.Run
                (
                    () =>
                    {
                        Callback(_cts.Token);
                        Dispatcher.Invoke(() => Close());
                    }
                );
            }
        }

        public void SetCaption(string in_caption)
        {
            Dispatcher.Invoke(() => Caption = in_caption);
        }

        public void SetDescription(string in_desc)
        {
            Dispatcher.Invoke(() => Description = in_desc);
        }

        public void SetProgress(double in_value)
        {
            Dispatcher.Invoke(() => Progress = in_value);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            _cts.Cancel();
        }
    }
}
