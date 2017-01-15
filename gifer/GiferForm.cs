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
		private static List<string> ImagePathesInFolder;
		private static string CurrentImagePath;
		// https://en.wikipedia.org/wiki/Image_file_formats
		private static readonly string[] KnownImageExtensions = {
			"jpg", "jpeg", "jfif", "jp2",
			"tif", "tiff",
			"gif",
			"bmp", "dib",
			"png",
			"pbm", "pgm", "ppm", "pnm",
			"webp",
			"heif", "heic",
			"bpg",
			// ".svg", "svgz" 
		};

		// http://stackoverflow.com/questions/25382400/gif-animated-files-in-c-sharp-have-lower-framerates-than-they-should
		private const int timerAccuracy = 1;
		[System.Runtime.InteropServices.DllImport("winmm.dll")]
		private static extern int timeBeginPeriod(int msec);
		[System.Runtime.InteropServices.DllImport("winmm.dll")]
		public static extern int timeEndPeriod(int msec);

		public GiferForm(string imagePath)
		{
			InitializeComponent();

			this.FormBorderStyle = FormBorderStyle.None;
			this.AllowDrop = true;
			this.pictureBox1.MouseWheel += new MouseEventHandler(this.pictureBox1_MouseWheel);
			this.pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
			this.StartPosition = FormStartPosition.CenterScreen;
			this.TopMost = true;

			// moving picturebox insible invisible form
			// making transparent form fullscreen
			this.Size = Screen.PrimaryScreen.Bounds.Size;
			this.TransparencyKey = this.BackColor;

			if (!string.IsNullOrEmpty(imagePath)) {
				LoadImages(imagePath);
			} else {
				Bitmap image = new Bitmap(256, 256);
				using (Graphics g = Graphics.FromImage(image))
				{
					g.FillRectangle(Brushes.White, 0, 0, image.Width, image.Height);
				}
				SetImage(image);
			}
		}

		private void LoadImages(string imagePath)
		{
			CurrentImagePath = imagePath;
			ImagePathesInFolder = Directory.GetFiles(Path.GetDirectoryName(CurrentImagePath))
				.Where(filePath => KnownImageExtensions.Any(Path.GetExtension(filePath).EndsWith))
				.ToList();
			Image image = Image.FromFile(CurrentImagePath);
			SetImage(image);
		}

		private GifImage GifImage { get; set; }

		private void SetImage(Image image)
		{
			//pictureBox1.Image = CurrentFrame;
			if (image.Width > this.Size.Width || image.Height > this.Size.Height) {
				pictureBox1.Size = ResizeProportionaly(image.Size, this.Size);
			} else {
				pictureBox1.Size = image.Size;
			}

			// center image
			int x = (this.Width  / 2) - (pictureBox1.Width  / 2);
			int y = (this.Height / 2) - (pictureBox1.Height / 2);
			pictureBox1.Location = new Point(x, y);

			if (image.RawFormat.Equals(ImageFormat.Gif) && ImageAnimator.CanAnimate(image)) {
				GifImage = new GifImage(image);
				pictureBox1.Image = GifImage.Next();
				timer1.Interval = GifImage.AverageDelay;
				timer1.Start();
			} else {
				pictureBox1.Image = image;
			}
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

		private void Form1_Load(object sender, EventArgs e)
        {
            //pictureBox1.Image = Bitmap.FromFile(@"C:\1\1.gif");
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
			ImagePathesInFolder = ((string[])e.Data.GetData(DataFormats.FileDrop)).ToList();
			if(ImagePathesInFolder.Count == 1) {
				LoadImages(ImagePathesInFolder.First());
			} else {
				CurrentImagePath = ImagePathesInFolder.First();
				ImagePathesInFolder = ImagePathesInFolder.Where(filePath => KnownImageExtensions.Any(Path.GetExtension(filePath).EndsWith))
					.ToList();
				Image image = Image.FromFile(CurrentImagePath);
				SetImage(image);
			}			
		}

        private bool move = false;
        private int X = 0, Y = 0;

		// Global Variables
		private int _xPos;
		private int _yPos;
		private bool _dragging;

		private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
			//move = true;
			//X = e.X;
			//Y = e.Y;
			if (e.Button != MouseButtons.Left) {
				return;
			}			
			_dragging = true;
			_xPos = e.X;
			_yPos = e.Y;
		}

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
			//pictureBox1.Focus();
			//if (move)
			//{
			//    this.SetDesktopLocation(MousePosition.X - X, MousePosition.Y - Y);
			//}
			var c = sender as PictureBox;
			if (!_dragging || null == c) {
				return;
			}
			c.Top = e.Y + c.Top - _yPos;
			c.Left = e.X + c.Left - _xPos;
		}

		private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
		{
			//move = false;
			var c = sender as PictureBox;
			if (null == c) {
				return;
			}			
			_dragging = false;
		}

		private int Delta = 0;
        private int x1 = 0;

        public void resize()
        {
            try
            {
                resizing = true;
                /* Work. */
                x1 = Delta;

                /* Plan Be */
                //x1 = Delta;
                /*if (x1 > 0)
                {
                    while (x1 > 0)
                    {
                        x1 -= x1 / 2 > 0 ? x1 / 2 : 1;

                        pictureBox1.Invoke((MethodInvoker)delegate
                        {
                            pictureBox1.Size = new Size(pictureBox1.Size.Height + x1/2, pictureBox1.Size.Width + x1/2);
                        });
                        this.Invoke((MethodInvoker)delegate
                        {
                            this.Size = pictureBox1.Size;
                        });
                    }
                }
                else //x1 < 0
                {
                    while (x1 < 0)
                    {
                        x1 += x1 / 2 > 0 ? x1 / 2 : 1;

                        pictureBox1.Invoke((MethodInvoker)delegate
                        {
                            pictureBox1.Size = new Size(pictureBox1.Size.Height - x1/2, pictureBox1.Size.Width - x1/2);
                        });
                        this.Invoke((MethodInvoker)delegate
                        {
                            this.Size = pictureBox1.Size;
                        });
                    }
                }*/

                /* Plan Tse */
                /*if (x1>0)
                {
                    for (int i = x1; i > 0; i--)
                    {
                        pictureBox1.Invoke((MethodInvoker)delegate
                        {
                            pictureBox1.Size = new Size(pictureBox1.Size.Height + 1, pictureBox1.Size.Width + 1);
                        });
                        this.Invoke((MethodInvoker)delegate
                        {
                            this.Size = pictureBox1.Size;
                        });

                    }
                }
                else
                {
                    for (int i = x1; i < 0; i++)
                    {
                        pictureBox1.Invoke((MethodInvoker)delegate
                        {
                            pictureBox1.Size = new Size(pictureBox1.Size.Height - 1, pictureBox1.Size.Width - 1);
                        });
                        this.Invoke((MethodInvoker)delegate
                        {
                            this.Size = pictureBox1.Size;
                        });
                    }
                }*/

                //x1 = 1;
                int sdvig_left = 0, sdvif_up = 0;

                /* Plan De*/
                /*sdvig_left = Convert.ToInt32(pictureBox1.Size.Height * x1 / 1000);
                sdvif_up = Convert.ToInt32(pictureBox1.Size.Width * x1 / 1000);
                pictureBox1.Invoke((MethodInvoker)delegate
                {
                    pictureBox1.Size = new Size(pictureBox1.Size.Width + sdvif_up, pictureBox1.Size.Height + sdvig_left);
                    this.Size = pictureBox1.Size;
                    this.Location = new Point(this.Location.X - sdvig_left / 2, this.Location.Y - sdvif_up / 2);
                });*/
                while (x1 != 0) {
					sdvif_up   = Convert.ToInt32(pictureBox1.Size.Width  * x1 / 1000);
					sdvig_left = Convert.ToInt32(pictureBox1.Size.Height * x1 / 1000);

                    pictureBox1.Invoke((MethodInvoker) delegate {
                        pictureBox1.Size = new Size(pictureBox1.Size.Width + sdvif_up, pictureBox1.Size.Height + sdvig_left);
                    });

                    this.Invoke((MethodInvoker) delegate {
                        this.Size = pictureBox1.Size;
                        this.Location = new Point(this.Location.X - sdvig_left / 2, this.Location.Y - sdvif_up / 2);
                    });

                    x1 = x1 > 0 ? x1 - 1 : x1 + 1;

                    x1 /= 2;

                    //System.Threading.Thread.Sleep(15);
                }
                resizing = false;
                return;
            }
            catch (System.Threading.ThreadInterruptedException ex)
            {
                /* Clean up. */
                x1 = 0;
                resizing = false;
                return;
            }
        }
		
        public void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
			pictureBox1_Resize(sender, e);
		}

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            this.ActiveControl = null;
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
			if (delta > 0) {
				ZoomIn(ratio);
			} else {
				ZoomOut(ratio);
			}
			resizing = false;
		}

		//private double ZOOMFACTOR = 1.25;   // = 25% smaller or larger
        private int MINMAX = 5;             // 5 times bigger or smaller than the ctrl
		private bool resizing = false;

		/// <summary>
		/// Make the PictureBox dimensions larger to effect the Zoom.
		/// </summary>
		/// <remarks>Maximum 5 times bigger</remarks>
		private void ZoomIn(double ratio)
		{
			if ((pictureBox1.Width < (MINMAX * pictureBox1.Image?.Width)) && (pictureBox1.Height < (MINMAX * pictureBox1.Image?.Height))) {
				Size prevSize = pictureBox1.Size;
				Point prevLocation = pictureBox1.Location;
				pictureBox1.Width = Convert.ToInt32(pictureBox1.Width * ratio);
				pictureBox1.Height = Convert.ToInt32(pictureBox1.Height * ratio);
				pictureBox1.Location = new Point(prevLocation.X - (pictureBox1.Width - prevSize.Width) / 2, prevLocation.Y - (pictureBox1.Height - prevSize.Height) / 2);
			}
		}

		/// <summary>
		/// Make the PictureBox dimensions smaller to effect the Zoom.
		/// </summary>
		/// <remarks>Minimum 5 times smaller</remarks>
		private void ZoomOut(double ratio)
		{
			if ((pictureBox1.Width > (pictureBox1.Image?.Width / MINMAX)) && (pictureBox1.Height > (pictureBox1.Image?.Height / MINMAX))) {
				Size prevSize = pictureBox1.Size;
				Point prevLocation = pictureBox1.Location;
				pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
				pictureBox1.Width = Convert.ToInt32(pictureBox1.Width / ratio);
				pictureBox1.Height = Convert.ToInt32(pictureBox1.Height / ratio);
				pictureBox1.Location = new Point(prevLocation.X + (prevSize.Width - pictureBox1.Width) / 2, prevLocation.Y + (prevSize.Height - pictureBox1.Height) / 2);
			}
		}
		
		private void GiferForm_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Right) {
				CurrentImagePath = ImagePathesInFolder.Next(CurrentImagePath);
			} else if (e.KeyCode == Keys.Left) {
				CurrentImagePath = ImagePathesInFolder.Previous(CurrentImagePath);
			}
			SetImage(Image.FromFile(CurrentImagePath));
		}

		private void pictureBox1_Paint(object sender, PaintEventArgs e)
		{

		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			pictureBox1.Image = GifImage.Next();
		}

		private void pictureBox1_DragDrop(object sender, DragEventArgs e)
		{
		}

		private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) {
                Application.Exit();
            }
        }
    }
}