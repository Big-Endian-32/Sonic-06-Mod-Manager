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
        /// <param name="in_bmp">Bitmap to convert.</param>
        public static BitmapSource GdiBitmapToBitmapSource(Bitmap in_bmp)
        {
            if (in_bmp == null)
                throw new ArgumentNullException("bitmap");

            var rect = new Rectangle(0, 0, in_bmp.Width, in_bmp.Height);

            var bitmapData = in_bmp.LockBits
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
                    in_bmp.Width,
                    in_bmp.Height,
                    in_bmp.HorizontalResolution,
                    in_bmp.VerticalResolution,
                    PixelFormats.Bgra32,
                    null,
                    bitmapData.Scan0,
                    size,
                    bitmapData.Stride
                );
            }
            finally
            {
                in_bmp.UnlockBits(bitmapData);
            }
        }
    }
}
