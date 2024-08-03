using System.Collections.ObjectModel;
using System.Collections.Specialized;
using SonicNextModManager.Helpers;
using SonicNextModManager.UI.Components;

namespace SonicNextModManager.UI.Dialogs
{
    /// <summary>
    /// Interaction logic for NextTaskDialog.xaml
    /// </summary>
    public partial class NextTaskDialog : ImmersiveWindow
    {
        public static readonly DependencyProperty CaptionProperty = DependencyProperty.Register
        (
            nameof(Caption),
            typeof(string),
            typeof(NextTaskDialog),
            new PropertyMetadata("Placeholder")
        );

        public string Caption
        {
            get => (string)GetValue(CaptionProperty);
            set => SetValue(CaptionProperty, value);
        }

        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register
        (
            nameof(Message),
            typeof(string),
            typeof(NextTaskDialog),
            new PropertyMetadata("Placeholder")
        );

        public string Message
        {
            get => (string)GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }

        public static readonly DependencyProperty CancelButtonProperty = DependencyProperty.Register
        (
            nameof(CancelButton),
            typeof(bool),
            typeof(NextTaskDialog),
            new PropertyMetadata(true)
        );

        public bool CancelButton
        {
            get => (bool)GetValue(CancelButtonProperty);
            set => SetValue(CancelButtonProperty, value);
        }

        internal ObservableCollection<TaskButton> Tasks { get; private set; } = new();

        /// <summary>
        /// Refreshes the tasks if the observable collection was changed.
        /// </summary>
        private void Tasks_CollectionChanged(object? in_sender, NotifyCollectionChangedEventArgs in_args)
        {
            // Clear all message box buttons.
            DialogTasks.Children.Clear();

            // Add all buttons to the panel.
            foreach (var button in Tasks)
                DialogTasks.Children.Add(button);
        }

        public NextTaskDialog()
        {
            // Set owner to last active window calling this class.
            Owner = WindowHelper.GetActiveWindow();

            InitializeComponent();

            // Subscribe to events.
            Tasks.CollectionChanged += Tasks_CollectionChanged;

            DataContext = this;
        }

        /// <summary>
        /// Displays the message box dialog with the specified parameters.
        /// </summary>
        /// <param name="in_message">Message to display.</param>
        /// <param name="in_caption">Title bar caption to use.</param>
        public void Show(string in_message = "", string in_caption = "")
        {
            Message = in_message;
            Caption = in_caption;

            // Add cancel button if requested.
            if (CancelButton)
                AddTask(LocaleService.Localise("Common_Cancel"), "", "close", () => Close());

            // Open message box.
            ShowDialog();
        }

        /// <summary>
        /// Adds a task to the dialog box.
        /// </summary>
        /// <param name="in_caption">Task title shown on the button.</param>
        /// <param name="in_desc">Task description shown on the button.</param>
        /// <param name="in_icon">Icon shown on the button.</param>
        /// <param name="in_callback">Actions performed once the button is clicked.</param>
        public TaskButton AddTask(string in_caption, string in_desc, string in_icon, Action in_callback)
        {
            TaskButton task = Tasks.SingleOrDefault(x => x.Caption == in_caption);

            // Remove task if it already exists.
            if (task != null)
                Tasks.Remove(task);

            // Create a new task.
            task = new()
            {
                Caption = in_caption,
                Description = in_desc,
                IconName = in_icon
            };

            // Set task click event.
            task.RootButton.Click += (s, e) => in_callback?.Invoke();

            Tasks.Add(task);

            return task;
        }

        /// <summary>
        /// Removes a task from the dialog box matching the input caption.
        /// </summary>
        /// <param name="in_caption">Caption to remove.</param>
        public void RemoveTask(string in_caption)
        {
            TaskButton task = Tasks.SingleOrDefault(x => x.Caption == in_caption);

            if (task != null)
            {
                // Remove button based on caption - generally you wouldn't have identical tasks.
                Tasks.Remove(task);
            }
        }
    }
}
