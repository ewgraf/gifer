using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace gifer
{
	public class GifImage
	{
		private Image _gif;
		private int _currentFrame = 0;

		public int Delay { get; set; }
        public int Frames { get; set; }

        public GifImage(Image image)
		{
			_gif = image;
            Frames = _gif.GetFrameCount(FrameDimension.Time);
			Delay = BitConverter.ToInt32(image.GetPropertyItem(20736).Value, 0) * 10;
            if (Delay == 0) {
                Delay = 100;
            }
		}

		public Bitmap Next()
		{
			if (_currentFrame >= Frames || _currentFrame < 1) {
				_currentFrame = 0;
			}
			_gif.SelectActiveFrame(FrameDimension.Time, _currentFrame++);
			return Image.FromHbitmap(new Bitmap(_gif).GetHbitmap());
		}
	}
}
