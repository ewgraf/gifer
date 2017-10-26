using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace gifer
{
	public partial class GiferForm : Form
	{
        private static readonly string[] KnownImageFormats = 
            new [] { ImageFormat.Bmp, ImageFormat.Gif, ImageFormat.Jpeg, ImageFormat.Png, ImageFormat.Tiff }
            .Select(GetFilenameExtension)
            .SelectMany(e => e.Replace("*.", string.Empty).Split(';'))
            .ToArray();
        private readonly Configuration _config;

        private GifImage _gifImage;
        private string _currentImagePath;
        private List<string> _imagesInFolder;
        private readonly OpenWithListener _openWithListener;

        public GiferForm(Configuration config) {
            _config = config;
            _openWithListener = new OpenWithListener();

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

            SetDefaultImage();

            int x = (this.Width / 2) - (pictureBox1.Width / 2);
            int y = (this.Height / 2) - (pictureBox1.Height / 2);
            pictureBox1.Location = new Point(x, y);
            SetupStandalone(bool.Parse(_config.AppSettings.Settings["openInStandalone"].Value));
        }

        private void SetDefaultImage() {
            // Empty field
            var image = new Bitmap(256, 256);
            using (Graphics g = Graphics.FromImage(image)) {
                g.FillRectangle(Brushes.LightGray, 0, 0, image.Width, image.Height);
                g.DrawString("[Drag GIF/Image Here]", new Font("Courier New", 9), Brushes.Black, 47, 125);
            }
            SetImage(image);
        }

        private void SetupStandalone(bool start) {
            if (start && !_openWithListener.Running) {
                Task.Run(() => _openWithListener.Start(filePath => {
                    try {
                        LoadImageAndFolder(filePath);
                    } catch (Exception ex) {
                        MessageBox.Show(ex.ToString());
                        Application.Exit();
                    }
                }));
            } else if (!start && _openWithListener.Running) {
                _openWithListener.Stop();
            }
        }

        public GiferForm(Configuration config, string imagePath) : this(config) => LoadImageAndFolder(imagePath);

        private void LoadImageAndFolder(string imagePath) {
			if (string.IsNullOrEmpty(imagePath)) {
                return;
            }
            if (KnownImageFormats.Any(imagePath.ToUpper().EndsWith)) {
				Image image = LoadImage(imagePath);
				if (image == null) {
					MessageBox.Show($"Can not load image: '{imagePath}'");
				}
                if (this.InvokeRequired) {
                    this.Invoke(new MethodInvoker(() => {
                        SetImage(image);
                    }));
                } else {
                    SetImage(image);
                }
				_currentImagePath = imagePath;
				_imagesInFolder = Directory.GetFiles(Path.GetDirectoryName(_currentImagePath))
					.Where(path => KnownImageFormats.Any(path.ToUpper().EndsWith))
					.ToList();
			} else {
				MessageBox.Show($"Unknown image extension at: '{imagePath}' '{Path.GetExtension(imagePath)}'");
			}
		}

		private Image LoadImage(string imagePath)
		{
			try {
				switch (Path.GetExtension(imagePath)) {
					default:
						return Image.FromFile(imagePath);
				}
			} catch (Exception ex) {
				MessageBox.Show(ex.ToString());
				return null;
			}
		}

		private void SetImage(Image image)
        {
            timer1.Stop();  
            Point center = Point.Add(pictureBox1.Location, pictureBox1.Size.Divide(2));
            if (image.Width > this.Size.Width || image.Height > this.Size.Height) {
				pictureBox1.Size = ResizeProportionaly(image.Size, this.Size);
			} else {
				pictureBox1.Size = image.Size;
			}
            pictureBox1.Location = Point.Subtract(center, pictureBox1.Size.Divide(2));

            pictureBox1.Image?.Dispose();
            pictureBox1.Image = null;
            _gifImage?.Dispose();
            GC.Collect();
            if (image.RawFormat == ImageFormat.Gif && ImageAnimator.CanAnimate(image) 
                || image.RawFormat.Guid == new Guid("b96b3cb0-0728-11d3-9d7b-0000f81ef32e"))
			{
				_gifImage = new GifImage(image);
				pictureBox1.Image = _gifImage.Next();
                timer1.Interval = _gifImage.Delay;
                timer1.Start();
			} else { // if plain image
                pictureBox1.Image = image;
			}
		}

		public static string GetFilenameExtension(ImageFormat format)
		{
			return ImageCodecInfo.GetImageEncoders()
                .FirstOrDefault(x => x.FormatID == format.Guid)
                ?.FilenameExtension
                ?? string.Empty; // ImageFormat.Jpeg -> "*.JPG;*.JPEG;*.JPE;*.JFIF"
		}

		public static Size ResizeProportionaly(Size size, Size fitSize)
		{
            double ratioX = (double)fitSize.Width  / (double)size.Width;
            double ratioY = (double)fitSize.Height / (double)size.Height;
            double ratio  = Math.Min(ratioX, ratioY);
            return size.Multiply(ratio);
		}
		
        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string imagePath = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];
            LoadImageAndFolder(imagePath);
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

		private bool _resizing = false;

        public void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
			pictureBox1_Resize(sender, e);
		}
		
		private void pictureBox1_Resize(object sender, EventArgs e)
		{
			var args = e as MouseEventArgs;
			if(args == null) { // if resize is caused not by mouse wheel, but by 'pictureBox1.Size = ' or '+='.
				return;
            }

            if (_resizing) {
                //_resizing = false;
                return;
            }

            int delta = args.Delta;
			if(delta == 0) {
				return;
			}

			double ratio = 1.35;
			if (ModifierKeys.HasFlag(Keys.Control)) {
				ratio = 1.05;
			} else if (ModifierKeys.HasFlag(Keys.Shift)) {
                ratio = 2.0;
            }

			_resizing = true;
			Zoom(Math.Sign(delta)*ratio);
			_resizing = false;
		}

        // Gaussiana [0.01, 0.05, 0.1, 0.15, 0.2, 0.25, 0.3, 0.35, 0.4, 0.35, 0.3, 0.25, 0.2, 0.15, 0.1, 0.05, 0.01] / 3 => Sum = ~1
        //private static double[] Gaussiana = new[] { 0.003, 0.016, 0.03, 0.05, 0.06, 0.083, 0.1, 0.116,  0.13,  0.116, 0.1, 0.083, 0.06, 0.05, 0.03, 0.016, 0.003 };
        //private static int[] Gaussiana = new[] { 64, 32, 16, 8, 4, 2, 4, 8, 16, 32, 64 };
        //private float Gauss(float x) {            
        //    return exp(-(x - mu) ^ 2 / (2 * sigma ^ 2)) / sqrt(2 * pi * sigma ^ 2)
        //}

        private void Zoom(double ratio)
        {
            Size size = pictureBox1.Size;
			Point location = pictureBox1.Location;
            double enlargementRatio = Animation.GetEnlargementValue(ratio);
            var newSize = new Size {
                Width  = Convert.ToInt32(pictureBox1.Width  * enlargementRatio),
                Height = Convert.ToInt32(pictureBox1.Height * enlargementRatio)
            };
            Size widening = newSize - size;
            var newLocation = Point.Add(location, widening.Divide(-2));
            //   const
            // ---------- -> steps : so that when 'widening' rizes, 'steps' reduses, to resize larger window faster
            //  widening
            //
            // Ex: let widening 64 -> be steps 64, diff 128 -> steps 32
            //    c
            // -------- -> 64 steps of resizing => c = 64*64 = 4096
            //    64
            // so let it be 4096. pretty round, huh
            int steps = 4096 / (Math.Abs(widening.Width + widening.Height) / 2);
            Debug.WriteLine($"Steps: {steps}");
            widening = widening.Divide(steps).RoundToPowerOf2();
            Size shift = widening.Divide(2);
            while (_resizing && !ModifierKeys.HasFlag(Keys.Alt) && (pictureBox1.Size - newSize).AbsMore(widening)) {
                pictureBox1.Size += widening;
                pictureBox1.Location -= shift;
                Application.DoEvents();
            }            
            pictureBox1.Size = newSize;
            pictureBox1.Location = newLocation;
        }

        #endregion

        private void GiferForm_KeyDown(object sender, KeyEventArgs e)
		{
            if (e.KeyCode == Keys.Right || e.KeyCode == Keys.Left) {
                if (e.KeyCode == Keys.Right) {
                    _currentImagePath = _imagesInFolder.Next(_currentImagePath);
                } else if (e.KeyCode == Keys.Left) {
                    _currentImagePath = _imagesInFolder.Previous(_currentImagePath);
                }
                SetImage(Image.FromFile(_currentImagePath));
            } else if (e.KeyCode == Keys.H) {
                ShowHelp(_config);
            } else if (e.KeyCode == Keys.Delete) {
                if (_currentImagePath == null) {
                    return;
                }
                string imageToDeletePath = _currentImagePath;
                _currentImagePath = _imagesInFolder.Next(_currentImagePath);
                LoadImageAndFolder(_currentImagePath);
                _imagesInFolder.Remove(imageToDeletePath);
                if (imageToDeletePath == _currentImagePath) {
                    _currentImagePath = null;
                    SetDefaultImage();
                }
                FileSystem.DeleteFile(imageToDeletePath, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
            } else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.P ) {
                if (_currentImagePath == null) {
                    return;
                }
                var p = new Process();
                p.StartInfo.FileName = _currentImagePath;
                p.StartInfo.Verb = "Print";
                p.Start();
            } else if (e.KeyCode == Keys.Escape) {
                Application.Exit();
            }
        }

        private void ShowHelp(Configuration config)
        {
            bool showHelp;
            bool standalone;
            bool.TryParse(config.AppSettings.Settings["showHelpAtStartup"].Value, out showHelp);
            bool.TryParse(config.AppSettings.Settings["openInStandalone"].Value, out standalone);
            var helpForm = new HelpForm(showHelp, standalone);
            helpForm.StartPosition = FormStartPosition.CenterScreen;
            helpForm.ShowDialog();
            if (config.AppSettings.Settings["showHelpAtStartup"].Value != helpForm.ShowHelpAtStartup.ToString()) {
                config.AppSettings.Settings.Remove("showHelpAtStartup");
                config.AppSettings.Settings.Add("showHelpAtStartup", helpForm.ShowHelpAtStartup.ToString());
                config.Save(ConfigurationSaveMode.Minimal);
            }
            if (config.AppSettings.Settings["openInStandalone"].Value != helpForm.OpenInStandalone.ToString()) {
                config.AppSettings.Settings.Remove("openInStandalone");
                config.AppSettings.Settings.Add("openInStandalone", helpForm.OpenInStandalone.ToString());
                config.Save(ConfigurationSaveMode.Minimal);
                SetupStandalone(helpForm.OpenInStandalone);
            }
        }
		
		private void timer1_Tick(object sender, EventArgs e)
		{
			pictureBox1.Image = _gifImage.Next();
		}

		private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) {
                Application.Exit();
            }
        }

        private void GiferForm_Load(object sender, EventArgs e)
        {
            bool showHelp;
            bool.TryParse(_config.AppSettings.Settings["showHelpAtStartup"].Value, out showHelp);
            if (showHelp) {
                ShowHelp(_config);
            }
        }
    }
}
