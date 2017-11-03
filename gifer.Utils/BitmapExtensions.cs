using System;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace gifer.Utils {
    public static class BitmapExtensions {
        public static BitmapSource ToBitmapSource(this Bitmap image) {
            IntPtr hBitmap = image.GetHbitmap();
            return Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }

        public static WriteableBitmap ToWritableBitmap(this Bitmap image) {
            return new WriteableBitmap(image.ToBitmapSource());
        }
    }
}
