using System.Drawing.Imaging;
using System.Linq;
using System.Net;
using gifer.Utils;

namespace giferWpf {
    public class Gifer {
        private const int Port = 42357;
        public static IPEndPoint EndPoint = new IPEndPoint(IPAddress.Loopback, Port);
        public static readonly string[] KnownImageFormats =
            new[] { ImageFormat.Bmp, ImageFormat.Gif, ImageFormat.Jpeg, ImageFormat.Png, ImageFormat.Tiff }
            .Select(ImageExtensions.GetFilenameExtension)
            .SelectMany(e => e.Replace("*.", string.Empty).Split(';'))
            .ToArray();
    }
}
