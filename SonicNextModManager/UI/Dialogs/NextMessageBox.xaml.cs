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

        private void NextMessageBox_Loaded(object sender, RoutedEventArgs e)
        {
            if (Owner != null)
                return;

            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        /// <summary>
        /// Displays the message box dialog with the specified parameters.
        /// </summary>
        /// <param name="message">Message to display.</param>
        /// <param name="caption">Title bar caption to use.</param>
        /// <param name="buttons">Win32 buttons to display.</param>
        /// <param name="icon">Win32 icon to display.</param>
        public static NextDialogResult Show
        (
            string message,
            string caption = "",
            NextMessageBoxButton buttons = NextMessageBoxButton.OK,
            NextMessageBoxIcon icon = NextMessageBoxIcon.None
        )
        {
            var msg = new NextMessageBox();

            msg.Message = message;
            msg.Caption = caption;

            // Add generic Win32 buttons.
            msg.SetButtons(buttons);

            // Set dialog icon.
            msg.SetIcon(icon);

            // Open message box.
            msg.ShowDialog();

            return msg.Result;
        }

        /// <summary>
        /// Adds a button to the message box.
        /// </summary>
        /// <param name="caption">Text shown on the button.</param>
        /// <param name="action">Actions performed once the button is clicked.</param>
        public void AddButton(string caption, Action action)
            => DialogButtons.AddButton(caption, action);

        /// <summary>
        /// Removes a button from the message box matching the input caption.
        /// </summary>
        /// <param name="caption">Caption to remove.</param>
        public void RemoveButton(string caption)
            => DialogButtons.RemoveButton(caption);

        /// <summary>
        /// Adds the generic Win32 default buttons using <see cref="NextMessageBoxButton"/>.
        /// </summary>
        /// <param name="buttons">Tasks to use.</param>
        public void SetButtons(NextMessageBoxButton buttons)
        {
            switch (buttons)
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

            void SetButtonResultAndClose(string localisedResource, NextDialogResult result)
            {
                AddButton
                (
                    LocaleService.Localise(localisedResource), () =>
                    {
                        Result = result;
                        Close();
                    }
                );
            }
        }

        /// <summary>
        /// Sets the icon used by the message using <see cref="NextMessageBoxIcon"/>.
        /// </summary>
        /// <param name="icon">Icon to display.</param>
        public void SetIcon(NextMessageBoxIcon icon)
        {
            // Set default width in case this changes whilst the dialog is open.
            IconColumn.Width = new GridLength(72);

            switch (icon)
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
