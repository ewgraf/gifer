using System.Drawing.Imaging;
using System.Linq;
using gifer.Utils;

namespace giferWpf {
    public class Gifer {
        public const string MyVersion = "0cc0d445-8ccf-4a97-bd30-84381e60a0ee";
        public static readonly string[] KnownImageFormats =
            new[] { ImageFormat.Bmp, ImageFormat.Gif, ImageFormat.Jpeg, ImageFormat.Png, ImageFormat.Tiff }
            .Select(ImageExtensions.GetFilenameExtension)
            .SelectMany(e => e.Replace("*.", string.Empty).Split(';'))
            .ToArray();
    }
}
