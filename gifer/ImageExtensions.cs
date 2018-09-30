using System.Drawing.Imaging;
using System.Linq;

namespace gifer {
    public static class ImageExtensions {
        public static string GetFilenameExtension(ImageFormat format) {
            // ImageFormat.Jpeg -> "*.jpg;*.jpeg;*.jpe;*.jfif"
            return ImageCodecInfo.GetImageEncoders().FirstOrDefault(x => x.FormatID == format.Guid)?.FilenameExtension ?? string.Empty;
        }
    }
}
