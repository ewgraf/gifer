using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gifer {
	public class EnlargedImageAndRectangle {
		public Bitmap ImagePart { get; set; }
		public Point ImageLocation { get; set; } // it's form location on screen if PictureBox covers all form
		public Size ImageSize { get; set; }
	}

	public class ImageOnScreen {
		private readonly Size _screenSize;
		private Size _imageContainerSize { get; set; }
		public Bitmap Image { get; set; }
		

		public ImageOnScreen(Size screenSize, Size formSize) {
			_screenSize = screenSize;
		}

		public EnlargedImageAndRectangle Enlarge(double ratio) {
			Size newSize = _imageContainerSize.Multiply(by: ratio);
			bool imageContainerOutOfScreenXBounds = false;
			bool imageContainerOutOfScreenYBounds = false;
			if (newSize.Width > _screenSize.Width) {
				imageContainerOutOfScreenXBounds = true;
			}
			if (newSize.Height > _screenSize.Height) {
				imageContainerOutOfScreenYBounds = true;
			}

			if (imageContainerOutOfScreenYBounds) {
				newSize = new Size(newSize.Width, _screenSize.Height);
			}
			return null;
		}
	}
}
