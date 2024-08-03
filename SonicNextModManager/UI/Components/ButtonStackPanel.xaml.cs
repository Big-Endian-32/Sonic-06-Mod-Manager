using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace SonicNextModManager.UI.Components
{
    /// <summary>
    /// Interaction logic for ButtonStackPanel.xaml
    /// </summary>
    public partial class ButtonStackPanel : UserControl
    {

        public static readonly DependencyProperty ButtonWidthProperty = DependencyProperty.Register
        (
            nameof(ButtonWidth),
            typeof(int),
            typeof(ButtonStackPanel),
            new PropertyMetadata(120)
        );

        public int ButtonWidth
        {
            get => (int)GetValue(ButtonWidthProperty);
            set => SetValue(ButtonWidthProperty, value);
        }

        internal ObservableCollection<Button> Buttons { get; private set; } = new();

        /// <summary>
        /// Refreshes the buttons if the observable collection was changed.
        /// </summary>
        private void Buttons_CollectionChanged(object? in_sender, NotifyCollectionChangedEventArgs in_args)
        {
            // Clear all message box buttons.
            DialogButtons.Children.Clear();

            // Add all buttons to the panel.
            foreach (var button in Buttons)
                DialogButtons.Children.Add(button);
        }

        public ButtonStackPanel()
        {
            InitializeComponent();

            // Subscribe to events.
            Buttons.CollectionChanged += Buttons_CollectionChanged;

            DataContext = this;
        }

        /// <summary>
        /// Adds a button to the message box.
        /// </summary>
        /// <param name="in_caption">Text shown on the button.</param>
        /// <param name="in_callback">Actions performed once the button is clicked.</param>
        public Button AddButton(string in_caption, Action in_callback)
        {
            Button button = Buttons.SingleOrDefault(x => x.Content == in_caption);

            // Remove button if it already exists.
            if (button != null)
                Buttons.Remove(button);

            // Create a new button.
            button = new()
            {
                Content = in_caption
            };

            // Set button click event.
            button.Click += (s, e) => in_callback?.Invoke();

            Buttons.Add(button);

            return button;
        }

        /// <summary>
        /// Removes a button from the message box matching the input caption.
        /// </summary>
        /// <param name="in_caption">Caption to remove.</param>
        public void RemoveButton(string in_caption)
        {
            Button button = Buttons.SingleOrDefault(x => x.Content == in_caption);

            if (button != null)
            {
                // Remove button based on caption - generally you wouldn't have identical buttons.
                Buttons.Remove(button);
            }
        }

        /// <summary>
        /// Returns a button from the message box matching the input caption.
        /// </summary>
        /// <param name="in_caption">Caption to find.</param>
        public Button GetButton(string in_caption)
        {
            Button button = Buttons.SingleOrDefault(x => x.Content == in_caption);

            if (button != null)
            {
                // Return button based on caption - generally you wouldn't have identical buttons.
                return button;
            }

            return null;
        }

        /// <summary>
        /// Returns a button from the input index.
        /// </summary>
        /// <param name="in_index">Index to return.</param>
        public Button GetButton(int in_index)
        {
            if (in_index > Buttons.Count)
                throw new IndexOutOfRangeException();

            return Buttons[in_index];
        }
    }
}
