using System.Media;
using SonicNextModManager.Helpers;
using SonicNextModManager.UI.Components;

namespace SonicNextModManager.UI.Dialogs
{
    /// <summary>
    /// Interaction logic for NextMessageBox.xaml
    /// </summary>
    public partial class NextMessageBox : ImmersiveWindow
    {
        public static readonly DependencyProperty CaptionProperty = DependencyProperty.Register
        (
            nameof(Caption),
            typeof(string),
            typeof(NextMessageBox),
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
            typeof(NextMessageBox),
            new PropertyMetadata("Placeholder")
        );

        public string Message
        {
            get => (string)GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }

        public static readonly DependencyProperty ButtonWidthProperty = DependencyProperty.Register
        (
            nameof(ButtonWidth),
            typeof(int),
            typeof(NextMessageBox),
            new PropertyMetadata(120)
        );

        public int ButtonWidth
        {
            get => (int)GetValue(ButtonWidthProperty);
            set => SetValue(ButtonWidthProperty, value);
        }

        public NextDialogResult Result { get; private set; }

        public NextMessageBox()
        {
            // Set owner to last active window calling this class.
            Owner = WindowHelper.GetActiveWindow();

            InitializeComponent();

            Loaded += NextMessageBox_Loaded;

            DataContext = this;
        }

        private void NextMessageBox_Loaded(object in_sender, RoutedEventArgs in_args)
        {
            if (Owner != null)
                return;

            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        /// <summary>
        /// Displays the message box dialog with the specified parameters.
        /// </summary>
        /// <param name="in_message">Message to display.</param>
        /// <param name="in_caption">Title bar caption to use.</param>
        /// <param name="in_buttons">Win32 buttons to display.</param>
        /// <param name="in_icon">Win32 icon to display.</param>
        public static NextDialogResult Show
        (
            string in_message,
            string in_caption = "",
            NextMessageBoxButton in_buttons = NextMessageBoxButton.OK,
            NextMessageBoxIcon in_icon = NextMessageBoxIcon.None
        )
        {
            var msg = new NextMessageBox();

            msg.Message = in_message;
            msg.Caption = in_caption;

            // Add generic Win32 buttons.
            msg.SetButtons(in_buttons);

            // Set dialog icon.
            msg.SetIcon(in_icon);

            // Open message box.
            msg.ShowDialog();

            return msg.Result;
        }

        /// <summary>
        /// Adds a button to the message box.
        /// </summary>
        /// <param name="in_caption">Text shown on the button.</param>
        /// <param name="in_callback">Actions performed once the button is clicked.</param>
        public void AddButton(string in_caption, Action in_callback)
            => DialogButtons.AddButton(in_caption, in_callback);

        /// <summary>
        /// Removes a button from the message box matching the input caption.
        /// </summary>
        /// <param name="in_caption">Caption to remove.</param>
        public void RemoveButton(string in_caption)
            => DialogButtons.RemoveButton(in_caption);

        /// <summary>
        /// Adds the generic Win32 default buttons using <see cref="NextMessageBoxButton"/>.
        /// </summary>
        /// <param name="in_buttons">Tasks to use.</param>
        public void SetButtons(NextMessageBoxButton in_buttons)
        {
            switch (in_buttons)
            {
                case NextMessageBoxButton.OK:
                    SetButtonResultAndClose("Common_OK", NextDialogResult.OK);
                    break;

                case NextMessageBoxButton.OKCancel:
                    SetButtonResultAndClose("Common_Cancel", NextDialogResult.Cancel);
                    SetButtonResultAndClose("Common_OK", NextDialogResult.OK);
                    break;

                case NextMessageBoxButton.AbortRetryIgnore:
                    SetButtonResultAndClose("Common_Ignore", NextDialogResult.Ignore);
                    SetButtonResultAndClose("Common_Retry", NextDialogResult.Retry);
                    SetButtonResultAndClose("Common_Abort", NextDialogResult.Abort);
                    break;

                case NextMessageBoxButton.YesNoCancel:
                    SetButtonResultAndClose("Common_Cancel", NextDialogResult.Cancel);
                    SetButtonResultAndClose("Common_No", NextDialogResult.No);
                    SetButtonResultAndClose("Common_Yes", NextDialogResult.Yes);
                    break;

                case NextMessageBoxButton.YesNo:
                    SetButtonResultAndClose("Common_No", NextDialogResult.No);
                    SetButtonResultAndClose("Common_Yes", NextDialogResult.Yes);
                    break;

                case NextMessageBoxButton.RetryCancel:
                    SetButtonResultAndClose("Common_Cancel", NextDialogResult.Cancel);
                    SetButtonResultAndClose("Common_Retry", NextDialogResult.Retry);
                    break;
            }

            void SetButtonResultAndClose(string in_str, NextDialogResult in_result)
            {
                AddButton
                (
                    LocaleService.Localise(in_str), () =>
                    {
                        Result = in_result;
                        Close();
                    }
                );
            }
        }

        /// <summary>
        /// Sets the icon used by the message using <see cref="NextMessageBoxIcon"/>.
        /// </summary>
        /// <param name="in_icon">Icon to display.</param>
        public void SetIcon(NextMessageBoxIcon in_icon)
        {
            // Set default width in case this changes whilst the dialog is open.
            IconColumn.Width = new GridLength(72);

            switch (in_icon)
            {
                case NextMessageBoxIcon.None:
                    IconColumn.Width = new GridLength(0);
                    break;

                case NextMessageBoxIcon.Error:
                    SystemSounds.Hand.Play();
                    MessageIcon.Source = ImageHelper.GdiBitmapToBitmapSource(Properties.Resources.Error);
                    break;

                case NextMessageBoxIcon.Question:
                    SystemSounds.Question.Play();
                    MessageIcon.Source = ImageHelper.GdiBitmapToBitmapSource(Properties.Resources.Question);
                    break;

                case NextMessageBoxIcon.Warning:
                    SystemSounds.Asterisk.Play();
                    MessageIcon.Source = ImageHelper.GdiBitmapToBitmapSource(Properties.Resources.Warning);
                    break;

                case NextMessageBoxIcon.Information:
                    SystemSounds.Asterisk.Play();
                    MessageIcon.Source = ImageHelper.GdiBitmapToBitmapSource(Properties.Resources.Information);
                    break;
            }
        }
    }
}
