using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SonicNextModManager.Helpers
{
    public class ImageHelper
    {
        /// <summary>
        /// Creates a <see cref="BitmapSource"/> from a <see cref="Bitmap"/>.
        /// <para><see href="https://stackoverflow.com/a/26261562">Learn more...</see></para>
        /// </summary>
        /// <param name="bitmap">Bitmap to convert.</param>
        public static BitmapSource GdiBitmapToBitmapSource(Bitmap bitmap)
        {
            if (bitmap == null)
                throw new ArgumentNullException("bitmap");

            var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

            var bitmapData = bitmap.LockBits
            (
                rect,
                ImageLockMode.ReadWrite,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb
            );

            try
            {
                var size = (rect.Width * rect.Height) * 4;

                return BitmapSource.Create
                (
                    bitmap.Width,
                    bitmap.Height,
                    bitmap.HorizontalResolution,
                    bitmap.VerticalResolution,
                    PixelFormats.Bgra32,
                    null,
                    bitmapData.Scan0,
                    size,
                    bitmapData.Stride
                );
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }
        }
    }
}
