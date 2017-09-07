using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace gifer
{
	public partial class GiferForm : Form
	{
		private static List<string> imagesInFolder;
		private static string CurrentImagePath;

		string[] knownImageExtensions;

		private GifImage GifImage { get; set; }

		private static readonly ImageFormat[] KnownImageFormats = {
			ImageFormat.Bmp,
			ImageFormat.Gif,
			ImageFormat.Jpeg,
			ImageFormat.Png,
			ImageFormat.Tiff,
		};

		public GiferForm(string imagePath)
		{
			InitializeComponent();

			this.FormBorderStyle = FormBorderStyle.None;
			this.AllowDrop = true;
			this.pictureBox1.MouseWheel += new MouseEventHandler(this.pictureBox1_MouseWheel);
			this.pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
			this.StartPosition = FormStartPosition.CenterScreen;

			// moving picturebox insible invisible form
			// making transparent form fullscreen
			this.Size = Screen.PrimaryScreen.Bounds.Size;
			this.TransparencyKey = this.BackColor;
			this.pictureBox1.BackColor = Color.Transparent;

			knownImageExtensions = KnownImageFormats.Select(GetFilenameExtension)
				.SelectMany(e => e.Replace("*.", string.Empty).Split(';'))
				.ToArray();

			// Empty field
			Image image = new Bitmap(256, 256);
			using (Graphics g = Graphics.FromImage(image)) {
				g.FillRectangle(Brushes.LightGray, 0, 0, image.Width, image.Height);
			}
			SetImage(image);

			LoadImageAndFolder(imagePath);
		}

		private void LoadImageAndFolder(string imagePath)
		{
			if (!string.IsNullOrEmpty(imagePath)) {
				if (knownImageExtensions.Any(imagePath.ToUpper().EndsWith)) {
					Image image = LoadImage(imagePath);
					if (image == null) {
						MessageBox.Show($"Can not load image: '{imagePath}'");
					}
					SetImage(image);
					CurrentImagePath = imagePath;
					imagesInFolder = Directory.GetFiles(Path.GetDirectoryName(CurrentImagePath))
						.Where(path => knownImageExtensions.Any(path.ToUpper().EndsWith))
						.ToList();
				} else {
					MessageBox.Show($"Unknown image extension at: '{imagePath}' '{Path.GetExtension(imagePath)}'");
				}
			}
		}

		private Image LoadImage(string imagePath)
		{
			try
			{
				switch (Path.GetExtension(imagePath))
				{
					default:
						return Image.FromFile(imagePath);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
				return null;
			}
		}

		private void SetImage(Image image)
		{
			timer1.Stop();

			if (image.Width > this.Size.Width || image.Height > this.Size.Height)
			{
				pictureBox1.Size = ResizeProportionaly(image.Size, this.Size);
			}
			else
			{
				pictureBox1.Size = image.Size;
			}

			// center image
			int x = (this.Width / 2) - (pictureBox1.Width / 2);
			int y = (this.Height / 2) - (pictureBox1.Height / 2);
			pictureBox1.Location = new Point(x, y);

			if (image.RawFormat == ImageFormat.Gif && ImageAnimator.CanAnimate(image))
			{
				GifImage = new GifImage(image);
				pictureBox1.Image = GifImage.Next();
				timer1.Interval = GifImage.Delay;
				timer1.Start();
			}
			else
			{
				pictureBox1.Image = image;
			}
		}

		public static string GetFilenameExtension(ImageFormat format)
		{
			return ImageCodecInfo.GetImageEncoders().FirstOrDefault(x => x.FormatID == format.Guid)?.FilenameExtension ?? string.Empty; // ImageFormat.Jpeg -> "*.JPG;*.JPEG;*.JPE;*.JFIF"
		}

		public static Size ResizeProportionaly(Size size, Size fitSize)
		{
			double ratioX = (double)fitSize.Width  / size.Width;
			double ratioY = (double)fitSize.Height / size.Height;
			double ratio  = Math.Min(ratioX, ratioY);

			int newWidth  = (int)(size.Width  * ratio);
			int newHeight = (int)(size.Height * ratio);
			
			return new Size(newWidth, newHeight);
		}
		
        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
			LoadImageAndFolder(((string[])e.Data.GetData(DataFormats.FileDrop))[0]);
			this.Activate();
		}

		#region Moving

		private int _x;
		private int _y;
		private bool _moving;

		private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
			if (e.Button != MouseButtons.Left) {
				return;
			}
			_moving = true;
			_x = e.X;
			_y = e.Y;
		}

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
			var c = sender as PictureBox;
			if (!_moving || c == null) {
				return;
			}
			c.Top = e.Y + c.Top - _y;
			c.Left = e.X + c.Left - _x;
		}

		private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
		{
			var c = sender as PictureBox;
			if (c == null) {
				return;
			}			
			_moving = false;
		}

		#endregion

		#region Resizing

		private const int MINMAX = 5;
		private bool resizing = false;

		public void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
			pictureBox1_Resize(sender, e);
		}
		
		private void pictureBox1_Resize(object sender, EventArgs e)
		{
			if (resizing) {
				return;
			}

			var args = e as MouseEventArgs;
			if(args == null) {
				return;
			}

			int delta = args.Delta;
			if(delta == 0) {
				return;
			}

			double ratio = 1.25;
			if(ModifierKeys == Keys.Control) {
				ratio = 1.05;
			}

			resizing = true;
			Zoom(Math.Sign(delta)*ratio);
			resizing = false;
		}

		private void Zoom(double ratio)
		{
			Size prevSize = pictureBox1.Size;
			Point prevLocation = pictureBox1.Location;
			if (ratio > 0) {
				if ((pictureBox1.Width  < (pictureBox1.Image.Width  * MINMAX)) && 
					(pictureBox1.Height < (pictureBox1.Image.Height * MINMAX))) {
					var width  = Convert.ToInt32(pictureBox1.Width  * Math.Abs(ratio));
					var heigth = Convert.ToInt32(pictureBox1.Height * Math.Abs(ratio));
                    var size = new Point(prevLocation.X + (prevSize.Width  - pictureBox1.Width ) / 2,
                                         prevLocation.Y + (prevSize.Height - pictureBox1.Height) / 2);
                    int step = ((width - pictureBox1.Width) + (heigth - pictureBox1.Height)) / 2;
                    step /= 30;
                    var sizeStep = new Size(-1, -1);
                    while (pictureBox1.Width < width || pictureBox1.Height < heigth/* || pictureBox1.Location.X < size.X || pictureBox1.Location.Y < size.Y*/) {
                        pictureBox1.Location = Point.Add(pictureBox1.Location, sizeStep);
                        pictureBox1.Width+=2;
                        pictureBox1.Height+=2;
                        Application.DoEvents();
                    }

                    pictureBox1.Width = width;
                    pictureBox1.Height = heigth;
                    //pictureBox1.Location = size;
                }
			} else {
				if ((pictureBox1.Width  > (pictureBox1.Image.Width  / MINMAX)) && 
					(pictureBox1.Height > (pictureBox1.Image.Height / MINMAX))) {
					pictureBox1.Width  = Convert.ToInt32(pictureBox1.Width  / Math.Abs(ratio));
					pictureBox1.Height = Convert.ToInt32(pictureBox1.Height / Math.Abs(ratio));
				}
			}
			//pictureBox1.Location = new Point(prevLocation.X + (prevSize.Width - pictureBox1.Width) / 2, prevLocation.Y + (prevSize.Height - pictureBox1.Height) / 2);
		}

		#endregion

		private void GiferForm_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Right) {
				CurrentImagePath = imagesInFolder.Next(CurrentImagePath);
			} else if (e.KeyCode == Keys.Left) {
				CurrentImagePath = imagesInFolder.Previous(CurrentImagePath);
			}
			SetImage(Image.FromFile(CurrentImagePath));
		}
		
		private void timer1_Tick(object sender, EventArgs e)
		{
			pictureBox1.Image = GifImage.Next();
		}

		private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) {
                Application.Exit();
            }
        }
    }
}
