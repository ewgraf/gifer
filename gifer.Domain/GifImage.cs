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
        private static readonly byte[] GifHeader     = new byte[] { 71, 73, 70 }; // GIF
        private static readonly byte[] Gif87aVersion = new byte[] { 56, 55, 97 }; // 87a
        private static readonly byte[] Gif89aVersion = new byte[] { 56, 57, 97 }; // 89a

        private readonly Bitmap _gif;
        private readonly FileStream _stream;
        private readonly byte[] _property;
        private readonly Rectangle _rectangle;
        private readonly object share = new object();
        private readonly string _imagePath;
        private int _currentFrame = 0;

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
            //_stream = new MemoryStream(bytes);
            _gif = new Bitmap(_stream);

            _rectangle = new Rectangle(0, 0, _gif.Width, _gif.Height);
            byte[] signature = bytes.Take(6).ToArray();

            //if (signature.SequenceEqual(Gif89aHeader)) {
            //    Type = GifType.GIF89a;
            //    IsGif = true;
            //} else if (signature.SequenceEqual(Gif87aHeader)) {
            //    Type = GifType.GIF87a;
            //    IsGif = true;
            //} else if (signature.Take(3).SequenceEqual(GifHeader)) {
            //    Type = GifType.GIFUnknown;
            //    IsGif = true;
            //} else {
            //    Type = GifType.UnknownOrPlainImage;
            //    IsGif = false;
            //}
            Type = GifType.GIF89a;
            IsGif = true;

            if (IsGif) {
                Frames = _gif.GetFrameCount(FrameDimension.Time);
                CurrentFrameDelay = BitConverter.ToInt32(_gif.GetPropertyItem(20736).Value, 0) * 10;
                if (CurrentFrameDelay == 0) {
                    CurrentFrameDelay = 100;
                }
            }
        }

        public GifImage(string imagePath) {
            //FileAttributes attributes = File.GetAttributes(_currentImagePath);
            _imagePath = imagePath;
            _stream = OpenReadFileStream(_imagePath);
            //byte[] imageBytes = new byte[_stream.Length];
            //_stream.Read(imageBytes, 0, (int)_stream.Length);
            //_stream.Close();
            //_stream.Dispose();
            // http://www.matthewflickinger.com/lab/whatsinagif/bits_and_bytes.asp
            byte[] signature = new byte[3];
            byte[] version = new byte[3];
            _stream.Read(signature, 0, 3);
            _stream.Read(version, 0, 3);
            //string s = Encoding.ASCII.GetString(signature);
            //string v = Encoding.ASCII.GetString(version);
            _stream.Seek(0, SeekOrigin.Begin);

            if (signature.SequenceEqual(GifHeader)) {
                // Important! Bitmap(Stream s) captures stream, and all manipulations with Stream should be before "new Bitmap(stream)"
                _gif = new Bitmap(_stream);
                _rectangle = new Rectangle(0, 0, _gif.Width, _gif.Height);
                Frames = _gif.GetFrameCount(FrameDimension.Time);
                CurrentFrameDelay = BitConverter.ToInt32(_gif.GetPropertyItem(20736).Value, 0) * 10;
                IsGif = true;
                //byte[] logicalScreenDescriptor = new byte[7];
                //_stream.Read(logicalScreenDescriptor, 0, 7);
                //// TO DO: update, when screen resolutions are >65535px
                //ushort canvasWidth = (ushort)BitConverter.ToInt16(logicalScreenDescriptor, 0);
                //ushort canvasHeigth = (ushort)BitConverter.ToInt16(logicalScreenDescriptor, 2);
                if (version.SequenceEqual(Gif89aVersion)) {
                    Type = GifType.GIF89a;
                } else if (signature.SequenceEqual(Gif87aVersion)) {
                    Type = GifType.GIF87a;
                } else {
                    Type = GifType.GIFUnknown;
                }
            } else {
                Type = GifType.UnknownOrPlainImage;
                IsGif = false;
            }
        }

        private FileStream OpenReadFileStream(string path) => new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.SequentialScan);

        public WriteableBitmap GetWritableBitmap() => CreateImageSource(_stream);

        public void DrawNext(ref WriteableBitmap bitmap) {
            if (_currentFrame >= Frames || _currentFrame < 1) {
                _currentFrame = 0;
            }
            _gif.SelectActiveFrame(FrameDimension.Time, _currentFrame);
            // int32 is 4bytes -> shift is 4
            CurrentFrameDelay = BitConverter.ToInt32(_gif.GetPropertyItem(20736).Value, 4 * _currentFrame) * 10;
            Debug.WriteLine($"CurrentFrameDelay: {CurrentFrameDelay}");
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

        private WriteableBitmap CreateImageSource(Stream stream) {
            stream.Position = 0;
            
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.None;
            bitmap.StreamSource = stream;
            bitmap.EndInit();
            BitmapSource prgbaSource = new FormatConvertedBitmap(bitmap, PixelFormats.Pbgra32, null, 0);
            var writeableBitmap = new WriteableBitmap(prgbaSource);
            _currentFrame++;
            return writeableBitmap;
        }

        public void Dispose() {
            _gif?.Dispose();
            _stream.Close();
            _stream.Dispose();
        }
	}
}
