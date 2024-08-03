using SonicNextModManager.UI.Components;

namespace SonicNextModManager.UI
{
    /// <summary>
    /// Interaction logic for ErrorHandler.xaml
    /// </summary>
    public partial class ErrorHandler : ImmersiveWindow
    {
        private Exception Exception { get; set; }

        private bool Fatal { get; set; }

        public ErrorHandler(Exception ex)
        {
            InitializeComponent();

            Exception = ex;
            Error.Text = BuildExceptionLog();
        }

        /// <summary>
        /// Builds the exception log for the RichTextBox control.
        /// </summary>
        /// <param name="markdown">Enables markdown for a better preview with services that use it.</param>
        private string BuildExceptionLog(bool markdown = false)
        {
            StringBuilder exception = new();

            if (markdown)
                exception.AppendLine("```");

            exception.AppendLine("Sonic '06 Mod Manager " + $"({App.GetAssemblyVersion()})");

            if (!string.IsNullOrEmpty(Exception.GetType().Name))
                exception.AppendLine($"\nType: {Exception.GetType().Name}");

            if (!string.IsNullOrEmpty(Exception.Message))
                exception.AppendLine($"Message: {Exception.Message}");

            if (!string.IsNullOrEmpty(Exception.Source))
                exception.AppendLine($"Source: {Exception.Source}");

            if (Exception.TargetSite != null)
                exception.AppendLine($"Function: {Exception.TargetSite}");

            if (!string.IsNullOrEmpty(Exception.StackTrace))
                exception.AppendLine($"\nStack Trace: \n{Exception.StackTrace}");

            if (Exception.InnerException != null)
                exception.AppendLine($"\nInner Exception: \n{Exception.InnerException}");

            if (markdown)
                exception.AppendLine("```");

            return exception.ToString();
        }

        private void Copy_Click(object sender, RoutedEventArgs e)
            => Clipboard.SetText(BuildExceptionLog(true));

        private void Report_Click(object sender, RoutedEventArgs e)
        {
            GitHub.CreateNewIssue
            (
                (string.IsNullOrEmpty(Exception.Source) ? string.Empty : $"[{Exception.Source}] ") +
                (string.IsNullOrEmpty(Exception.Message) ? string.Empty : $"'{Exception.Message}'"),

                BuildExceptionLog(true),

                new List<string>() { "bug", Fatal ? "fatal" : "unhandled" }
            );
        }

        private void Ignore_Click(object sender, RoutedEventArgs e)
            => Close();
    }
}
