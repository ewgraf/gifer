using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace gifer.Utils {

    public static class ImageExtensions {
        public static string GetFilenameExtension(ImageFormat format) {
            return ImageCodecInfo.GetImageEncoders()
                .FirstOrDefault(x => x.FormatID == format.Guid)
                ?.FilenameExtension
                ?? string.Empty; // ImageFormat.Jpeg -> "*.JPG;*.JPEG;*.JPE;*.JFIF"
        }
    }

    public static class SizeExtensions {
        public static Size Multiply(this Size size, double by) => new Size((int)(size.Width * by), (int)(size.Height * by));

        public static Size Divide(this Size size, float by) {
            var sizef = new SizeF(size.Width / by, size.Height / by);
            if (Math.Abs(sizef.Width ) < 1) {
                sizef.Width  = Math.Sign(sizef.Width );
            }
            if (Math.Abs(sizef.Height) < 1) {
                sizef.Height = Math.Sign(sizef.Height);
            }
            // Round    0.1 -> 0, 0.9 -> 1
            // Ceiling  0.1 -> 1, 0.9 -> 1
            // Truncate 0.1 -> 0, 0.9 -> 0
            return Size.Round(sizef);
        }

        public static bool AbsMore(this Size size1, Size size2) {
            return Math.Abs(size1.Width) > Math.Abs(size2.Width) && Math.Abs(size1.Height) > Math.Abs(size2.Height);
        }

        public static Size RoundToPowerOf2(this Size size) {
            if (size.Width % 2 != 0) {
                size.Width += Math.Sign(size.Width);
            }
            if (size.Height % 2 != 0) {
                size.Height += Math.Sign(size.Height);
            }
            return size;
        }
    }

    public static class ListExtensions {
		public static T Next<T>(this List<T> list, T current) {
			int index = list.IndexOf(current);
			if (index < list.Count - 1) {
				return list.ElementAt(index + 1);
			} else {
				return list.ElementAt(0);
			}
		}

		public static T Previous<T>(this List<T> list, T current) {
			int index = list.IndexOf(current);
			if (index >= 1) {
				return list.ElementAt(index - 1);
			} else {
				return list.ElementAt(list.Count - 1);
			}
		}
	}
}
