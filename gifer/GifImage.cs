using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace gifer
{
	public class GifFrame
	{
		public Bitmap Bitmap { get; set; }
		public int Delay { get; set; }
	}

	public class GifImage
	{
		private Image gif;
		private Bitmap[] Frames { get; set; }
		private int framesCount;
		private int currentFrame = 0;

		public int AverageDelay { get; set; }

		public GifImage(Image image)
		{
			gif = image;
			framesCount = gif.GetFrameCount(FrameDimension.Time);
			Frames = new Bitmap[framesCount];

			int index = 0;
			var delays = new List<int>();
			for (int i = 0; i < framesCount; i++, index += 4) {
				gif.SelectActiveFrame(FrameDimension.Time, i);
				delays.Add(BitConverter.ToInt32(image.GetPropertyItem(20736).Value, index) * 10);
				Frames[i] = Image.FromHbitmap(new Bitmap(gif).GetHbitmap());
			}
			AverageDelay = (int)delays.Average();
		}

		public Bitmap Next()
		{
			if (currentFrame >= framesCount || currentFrame < 1) {
				currentFrame = 0;
			}
			return Frames[currentFrame++];
		}
	}
}
