using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace gifer
{
	public class GifImage
	{
		private Image gif;
		private int framesCount;
		private int currentFrame = 0;

		public int Delay { get; set; }

		public GifImage(Image image)
		{
			gif = image;
			framesCount = gif.GetFrameCount(FrameDimension.Time);
			Delay = BitConverter.ToInt32(image.GetPropertyItem(20736).Value, 0) * 10;
		}

		public Bitmap Next()
		{
			if (currentFrame >= framesCount || currentFrame < 1) {
				currentFrame = 0;
			}
			gif.SelectActiveFrame(FrameDimension.Time, currentFrame++);
			return Image.FromHbitmap(new Bitmap(gif).GetHbitmap());
		}
	}
}
