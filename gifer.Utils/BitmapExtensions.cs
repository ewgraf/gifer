using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace gifer.Utils {
    public static class ImageExtensions {
        public static string GetFilenameExtension(ImageFormat format) {
            // ImageFormat.Jpeg -> "*.JPG;*.JPEG;*.JPE;*.JFIF"
            return ImageCodecInfo.GetImageEncoders()
                .FirstOrDefault(x => x.FormatID == format.Guid)?.FilenameExtension ?? string.Empty;
        }

        public static BitmapSource ToBitmapSource(this Bitmap image) {
            IntPtr hBitmap = image.GetHbitmap();
            return Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }

        public static WriteableBitmap ToWritableBitmap(this Bitmap image) {
            BitmapSource source = image.ToBitmapSource();
            source.Freeze();
            WriteableBitmap bitmap = new WriteableBitmap(source);
            source = null;
            return bitmap;
        }
    }
}
