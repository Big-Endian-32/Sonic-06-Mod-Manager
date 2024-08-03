using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace SonicNextModManager.UI.Components
{
    public class ImmersiveWindow : Window
    {
        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        public static readonly DependencyProperty CloseButtonProperty = DependencyProperty.Register
        (
            nameof(CloseButton),
            typeof(bool),
            typeof(ImmersiveWindow),
            new PropertyMetadata(true)
        );

        public bool CloseButton
        {
            get => (bool)GetValue(CloseButtonProperty);
            set => SetValue(CloseButtonProperty, value);
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            // Initialise immersive dark mode for every window.
            ImmersiveDarkMode.Initialise(new WindowInteropHelper(this).Handle, true);

            if (!CloseButton)
            {
                // Disable the close button on the title bar.
                var hWND = new WindowInteropHelper(this).Handle;
                SetWindowLong(hWND, GWL_STYLE, GetWindowLong(hWND, GWL_STYLE) & ~WS_SYSMENU);
            }

            base.OnSourceInitialized(e);
        }
    }
}
