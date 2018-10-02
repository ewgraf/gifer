using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;

namespace gifer {
    public static class ImageExtensions {
        public static string GetFilenameExtension(ImageFormat format) {
            // ImageFormat.Jpeg -> "*.jpg;*.jpeg;*.jpe;*.jfif"
            return ImageCodecInfo.GetImageEncoders().FirstOrDefault(x => x.FormatID == format.Guid)?.FilenameExtension ?? string.Empty;
        }

		public static Bitmap Pad(this Image image) {
			int largestDimension = Math.Max(image.Height, image.Width);
			var squareSize = new Size(largestDimension, largestDimension);
			Bitmap squareImage = new Bitmap(squareSize.Width, squareSize.Height);
			using (Graphics graphics = Graphics.FromImage(squareImage)) {
				graphics.CompositingQuality = CompositingQuality.HighSpeed;
				graphics.InterpolationMode = InterpolationMode.Low;
				graphics.SmoothingMode = SmoothingMode.None;
				graphics.DrawImage(image, (squareSize.Width / 2) - (image.Width / 2), (squareSize.Height / 2) - (image.Height / 2), image.Width, image.Height);
			}
			return squareImage;
		}
	}
}
