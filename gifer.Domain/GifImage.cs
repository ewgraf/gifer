using gifer.Utils;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace gifer.Domain {
    public class GifImage : IDisposable {
        // http://www.onicos.com/staff/iz/formats/gif.html
        private static readonly byte[] GifHeader    = new byte[] { 71, 73, 70             }; // GIF
        private static readonly byte[] Gif87aHeader = new byte[] { 71, 73, 70, 56, 55, 97 }; // GIF87a
        private static readonly byte[] Gif89aHeader = new byte[] { 71, 73, 70, 56, 57, 97 }; // GIF89a

        private readonly Bitmap _gif;
        private readonly MemoryStream _stream;
        private readonly byte[] _property;
        private readonly Rectangle _rectangle;
        private int _currentFrame = 0;
        object share = new object();

        public int CurrentFrameDelay { get; set; }
        public int Frames { get; set; }
        public bool IsGif { get; private set; }
        public GifType Type { get; private set; }

        [Obsolete("Лучше используйте GifImage(byte[] bytes)")]
        public GifImage(Bitmap image) {
            _gif = image;
            _rectangle = new Rectangle(0, 0, _gif.Width, _gif.Height);
            //PropertyItem item = current_image.GetPropertyItem(0x5100); // FrameDelay in libgdiplus
            //delay = (item.Value[0] + item.Value[1] * 256) * 10; // Time is in 1/100th of a second
            Frames = _gif.GetFrameCount(FrameDimension.Time);
            CurrentFrameDelay = BitConverter.ToInt32(image.GetPropertyItem(20736).Value, 0) * 10;
            if (CurrentFrameDelay == 0) {
                CurrentFrameDelay = 100;
            }
        }

        public GifImage(byte[] bytes) {
            _stream = new MemoryStream(bytes);
            _gif = new Bitmap(_stream);

            _rectangle = new Rectangle(0, 0, _gif.Width, _gif.Height);
            byte[] signature = bytes.Take(6).ToArray();

            if (signature.SequenceEqual(Gif89aHeader)) {
                Type = GifType.GIF89a;
                IsGif = true;
            } else if (signature.SequenceEqual(Gif87aHeader)) {
                Type = GifType.GIF87a;
                IsGif = true;
            } else if (signature.Take(3).SequenceEqual(GifHeader)) {
                Type = GifType.GIFUnknown;
                IsGif = true;
            } else {
                Type = GifType.UnknownOrPlainImage;
                IsGif = false;
            }
            //Debug.WriteLine($"Type: {Type}");

            if (IsGif) {
                Frames = _gif.GetFrameCount(FrameDimension.Time);
                CurrentFrameDelay = BitConverter.ToInt32(_gif.GetPropertyItem(20736).Value, 0) * 10;
                if (CurrentFrameDelay == 0) {
                    CurrentFrameDelay = 100;
                }
            }
        }

        public WriteableBitmap GetWritableBitmap() => CreateImageSource(_stream);

        public void DrawNext(ref WriteableBitmap bitmap) {
            if (_currentFrame >= Frames || _currentFrame < 1) {
                _currentFrame = 0;
            }
            _gif.SelectActiveFrame(FrameDimension.Time, _currentFrame);
            // int32 is 4bytes -> shift is 4
            CurrentFrameDelay = BitConverter.ToInt32(_gif.GetPropertyItem(20736).Value, 4 * _currentFrame) * 10;
            //Debug.WriteLine(CurrentFrameDelay);
            if (CurrentFrameDelay == 0) {
                CurrentFrameDelay = 100;
            }
            if (CurrentFrameDelay < 0) {
                throw new InvalidOperationException($"CurrentFrameDelay {CurrentFrameDelay} <= 0. Is int32 4 bytes length?");
            }
            _currentFrame++;
            BitmapData frameData = _gif.LockBits(_rectangle, ImageLockMode.ReadOnly, _gif.PixelFormat);
            bitmap.Lock();
            NativeMethods.CopyMemory(bitmap.BackBuffer, frameData.Scan0, Math.Abs(frameData.Stride * _gif.Height));
            bitmap.AddDirtyRect(new Int32Rect(0, 0, _rectangle.Width, _rectangle.Height));
            bitmap.Unlock();
            _gif.UnlockBits(frameData);
        }

        public Bitmap Copy() => (Bitmap)_gif.Clone();

        private WriteableBitmap CreateImageSource(MemoryStream stream) {
            stream.Position = 0;
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.CacheOption = BitmapCacheOption.OnLoad;
            bi.StreamSource = stream;
            bi.EndInit();
            bi.Freeze();

            BitmapSource prgbaSource = new FormatConvertedBitmap(bi, PixelFormats.Pbgra32, null, 0);
            WriteableBitmap bmp = new WriteableBitmap(prgbaSource);
            int w = bmp.PixelWidth;
            int h = bmp.PixelHeight;
            int[] pixelData = new int[w * h];
            //int widthInBytes = 4 * w;
            int widthInBytes = bmp.PixelWidth * (bmp.Format.BitsPerPixel / 8); //equals 4*w
            bmp.CopyPixels(pixelData, widthInBytes, 0);

            bmp.WritePixels(new Int32Rect(0, 0, w, h), pixelData, widthInBytes, 0);
            bi = null;

            return bmp;
        }

        public void Dispose() {
            _gif?.Dispose();
            _stream?.Dispose();
            _stream?.Close();
        }
	}
}
