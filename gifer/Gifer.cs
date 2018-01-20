using System.Drawing.Imaging;
using System.Linq;
using gifer.Utils;

namespace giferWpf {
    public class Gifer {
        public const string MyVersion = "4f9a48e1-e8b8-464b-8e6b-d756fe817ba3";
        public static readonly string[] KnownImageFormats =
            new[] { ImageFormat.Bmp, ImageFormat.Gif, ImageFormat.Jpeg, ImageFormat.Png, ImageFormat.Tiff }
            .Select(ImageExtensions.GetFilenameExtension)
            .SelectMany(e => e.Replace("*.", string.Empty).Split(';'))
            .ToArray();
    }
}
