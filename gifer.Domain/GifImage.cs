using gifer.Utils;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace gifer.Domain {
    public class GifImage : IDisposable {
        private readonly Bitmap _gif;
        private readonly Rectangle _rectangle;
        private int _currentFrame = 0;
        object share = new object();

		public int Delay { get; set; }
        public int Frames { get; set; }

        public GifImage(Bitmap image) {
            _gif = image;
            _rectangle = new Rectangle(0, 0, _gif.Width, _gif.Height);
            //PropertyItem item = current_image.GetPropertyItem(0x5100); // FrameDelay in libgdiplus
            //delay = (item.Value[0] + item.Value[1] * 256) * 10; // Time is in 1/100th of a second
            Frames = _gif.GetFrameCount(FrameDimension.Time);
            Delay = BitConverter.ToInt32(image.GetPropertyItem(20736).Value, 0) * 10;
            if (Delay == 0) {
                Delay = 100;
            }
		}

		public void DrawNext(ref WriteableBitmap bitmap) {
			if (_currentFrame >= Frames || _currentFrame < 1) {
				_currentFrame = 0;
			}
            lock (share) {
			    _gif.SelectActiveFrame(FrameDimension.Time, _currentFrame++);
                BitmapData frameData = _gif.LockBits(_rectangle, ImageLockMode.ReadOnly, _gif.PixelFormat);
                bitmap.Lock();
                NativeMethods.CopyMemory(bitmap.BackBuffer, frameData.Scan0, Math.Abs(frameData.Stride * _gif.Height));
                bitmap.AddDirtyRect(new Int32Rect(0, 0, _rectangle.Width, _rectangle.Height));
                bitmap.Unlock();
                _gif.UnlockBits(frameData);
            }
        }

        public Bitmap Copy() => (Bitmap)_gif.Clone();

        public void Dispose() {
            _gif.Dispose();
        }
	}
}
