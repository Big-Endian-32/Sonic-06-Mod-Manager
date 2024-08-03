namespace SonicNextModManager.Helpers
{
    public class WindowHelper
    {
        /// <summary>
        /// Returns the last active window in the current application.
        /// </summary>
        public static Window GetActiveWindow() => Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
    }
}
