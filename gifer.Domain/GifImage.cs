using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace gifer.Domain {
	public class GifImage : IDisposable {
		private Image _gif;
		private int _currentFrame = 0;

		public int Delay { get; set; }
        public int Frames { get; set; }

        public GifImage(Image image) {
			_gif = image;
            //PropertyItem item = current_image.GetPropertyItem(0x5100); // FrameDelay in libgdiplus
            //delay = (item.Value[0] + item.Value[1] * 256) * 10; // Time is in 1/100th of a second
            Frames = _gif.GetFrameCount(FrameDimension.Time);
			Delay = BitConverter.ToInt32(image.GetPropertyItem(20736).Value, 0) * 10;
            if (Delay == 0) {
                Delay = 100;
            }
		}

		public Bitmap Next() {
			if (_currentFrame >= Frames || _currentFrame < 1) {
				_currentFrame = 0;
			}
			_gif.SelectActiveFrame(FrameDimension.Time, _currentFrame++);
			return Image.FromHbitmap(new Bitmap(_gif).GetHbitmap());
		}

        public void Dispose() {
            _gif.Dispose();
        }
	}
}
