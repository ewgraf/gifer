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
using gifer.Domain;
using gifer.Utils;
using Microsoft.VisualBasic.FileIO;
using System.Runtime.InteropServices;

namespace gifer {
	public partial class GiferForm : Form {
        //private readonly Configuration _config;
        private GifImage _gifImage;
        private string _currentImagePath;
        private List<string> _imagesInFolder;
        private readonly OpenWithListener _openWithListener;

        public GiferForm(/*Configuration config*/) {
            //_config = config;
            _openWithListener = new OpenWithListener(Gifer.EndPoint);

            InitializeComponent();

            this.FormBorderStyle = FormBorderStyle.None;
            this.AllowDrop = true;
            // Form.BackgroungImage flickers when updated, therefore can not be used as a thind to draw a gif on 
            // we have to use PictureBox
            this.pictureBox1.MouseWheel += new MouseEventHandler(this.pictureBox1_MouseWheel);
            this.pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            this.StartPosition = FormStartPosition.CenterScreen;

            // moving picturebox insible invisible form
            // making transparent form fullscreen
            //this.Size = Screen.PrimaryScreen.Bounds.Size;
            //this.TransparencyKey = this.BackColor;
            //this.pictureBox1.BackColor = Color.Red;            

            //SetDefaultImage();
            //this.pictureBox1.BackColor = Color.LightGray;
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.groupBox1.BackColor = Color.Transparent;

            int x = (this.Width / 2) - (pictureBox1.Width / 2);
            int y = (this.Height / 2) - (pictureBox1.Height / 2);
            pictureBox1.Location = new Point(x, y);
            //SetupStandalone(bool.Parse(_config.AppSettings.Settings["openInStandalone"].Value));
            //this.OnPaintBackground
        }

        //protected override void OnPaintBackground(PaintEventArgs e) {
        //    var backgroundBrush = new SolidBrush(Color.Transparent);
        //    Graphics g = e.Graphics;
        //    g.FillRectangle(backgroundBrush, 0, 0, this.Width, this.Height);
        //}

        private void SetDefaultImage() {
            // Empty field
            var image = new Bitmap(256, 256);
            using (Graphics g = Graphics.FromImage(image)) {
                g.FillRectangle(Brushes.LightGray, 0, 0, image.Width, image.Height);
                g.DrawString("[Drag GIF/Image Here]", new Font("Courier New", 9), Brushes.Black, 47, 125);
                //
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

        public GiferForm(/*Configuration config, */string imagePath) : this(/*config*/) => LoadImageAndFolder(imagePath);

        private void LoadImageAndFolder(string imagePath) {
			if (string.IsNullOrEmpty(imagePath)) {
                return;
            }
            if (Gifer.KnownImageFormats.Any(imagePath.ToUpper().EndsWith)) {
				Bitmap image = (Bitmap)LoadImage(imagePath);
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
					.Where(path => Gifer.KnownImageFormats.Any(path.ToUpper().EndsWith))
					.ToList();
			} else {
				MessageBox.Show($"Unknown image extension at: '{imagePath}' '{Path.GetExtension(imagePath)}'");
			}
		}

		private Image LoadImage(string imagePath) {
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

		private void SetImage(Bitmap image) {
            //SuspendDrawing(this);
            timer1.Stop();
            timerUpdateTaskbarIcon.Stop();
            if (image.Width > Screen.PrimaryScreen.Bounds.Size.Width || image.Height > Screen.PrimaryScreen.Bounds.Size.Height) {
				pictureBox1.Size = ResizeProportionaly(image.Size, Screen.PrimaryScreen.Bounds.Size);
            } else {
				pictureBox1.Size = image.Size;
            }
            this.Size = pictureBox1.Size;
            Point center = (Point)Screen.PrimaryScreen.Bounds.Size.Divide(2);
            this.Location = Point.Subtract(center, this.Size.Divide(2));

            pictureBox1.Image?.Dispose();
            pictureBox1.Image = null;
            _gifImage?.Dispose();
            GC.Collect();
            if (image.RawFormat == ImageFormat.Gif && ImageAnimator.CanAnimate(image) 
                || image.RawFormat.Guid == new Guid("b96b3cb0-0728-11d3-9d7b-0000f81ef32e")) {
				_gifImage = new GifImage(image);
				//pictureBox1.Image = _gifImage.Next();
                timer1.Interval = _gifImage.CurrentFrameDelay;
                timer1.Start();
                timerUpdateTaskbarIcon.Start();
            } else { // if plain image
                pictureBox1.Image = image;
                this.Icon = Icon.FromHandle(((Bitmap)image).GetHicon());                
            }
            this.BringToFront();
            //ResumeDrawing(this);
		}

		public static Size ResizeProportionaly(Size size, Size fitSize) {
            double ratioX = (double)fitSize.Width  / (double)size.Width;
            double ratioY = (double)fitSize.Height / (double)size.Height;
            double ratio  = Math.Min(ratioX, ratioY);
            return size.Multiply(ratio);
		}
		
        private void Form1_DragEnter(object sender, DragEventArgs e) {
            e.Effect = DragDropEffects.All;
        }

        private void Form1_DragDrop(object sender, DragEventArgs e) {
	        groupBox1.Visible = false;
			string imagePath = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];
            LoadImageAndFolder(imagePath);
			this.Activate();
		}

		#region Moving

		private int _x;
		private int _y;
		private bool _moving;

		private void pictureBox1_MouseDown(object sender, MouseEventArgs e) {
			if (e.Button != MouseButtons.Left) {
				return;
			}
			_moving = true;
			_x = e.X;
			_y = e.Y;
		}

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e) {
			var c = sender as PictureBox;
			if (!_moving || c == null) {
				return;
			}

            //c.Top = e.Y + c.Top - _y;
            //c.Left = e.X + c.Left - _x;
            this.Top  = e.Y + this.Top  - _y;
            this.Left = e.X + this.Left - _x;
        }

		private void pictureBox1_MouseUp(object sender, MouseEventArgs e) {
			var c = sender as PictureBox;
			if (c == null) {
				return;
			}			
			_moving = false;
		}

        #endregion

        #region Resizing

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, Int32 wMsg, bool wParam, Int32 lParam);

        private const int WM_SETREDRAW = 11;

        public static void SuspendDrawing(Control parent) {
            SendMessage(parent.Handle, WM_SETREDRAW, false, 0);
        }

        public static void ResumeDrawing(Control parent) {
            SendMessage(parent.Handle, WM_SETREDRAW, true, 0);
            parent.Refresh();
        }

        private bool _resizing = false;

        public void pictureBox1_MouseWheel(object sender, MouseEventArgs e) {
			pictureBox1_Resize(sender, e);
		}
		
		private void pictureBox1_Resize(object sender, EventArgs e) {
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
			Zoom(Math.Sign(delta)*ratio, this, this.pictureBox1);
			_resizing = false;
		}

        // Gaussiana [0.01, 0.05, 0.1, 0.15, 0.2, 0.25, 0.3, 0.35, 0.4, 0.35, 0.3, 0.25, 0.2, 0.15, 0.1, 0.05, 0.01] / 3 => Sum = ~1
        //private static double[] Gaussiana = new[] { 0.003, 0.016, 0.03, 0.05, 0.06, 0.083, 0.1, 0.116,  0.13,  0.116, 0.1, 0.083, 0.06, 0.05, 0.03, 0.016, 0.003 };
        //private static int[] Gaussiana = new[] { 64, 32, 16, 8, 4, 2, 4, 8, 16, 32, 64 };
        //private float Gauss(float x) {            
        //    return exp(-(x - mu) ^ 2 / (2 * sigma ^ 2)) / sqrt(2 * pi * sigma ^ 2)
        //}

        [DllImport("User32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern bool MoveWindow(IntPtr hWnd, int x, int y, int w, int h, bool Repaint);

        private void Zoom(double ratio, Form form, PictureBox pictureBox) {
            Size size = form.Size;
            Point location;
            bool toEnlarge = true;
            if (ratio > 0 && (form.Width >= Screen.PrimaryScreen.Bounds.Width
                              || form.Height >= Screen.PrimaryScreen.Bounds.Height)) {
                toEnlarge = false;
            }
            location = form.Location;
            double enlargementRatio = AnimationHelper.GetEnlargementValue(ratio);
            var newSize = toEnlarge ? new Size {
                                        Width = Convert.ToInt32(size.Width * enlargementRatio),
                                        Height = Convert.ToInt32(size.Height * enlargementRatio)
                                    }
                                    : size;
            if (newSize.Width > Screen.PrimaryScreen.Bounds.Width && newSize.Height > Screen.PrimaryScreen.Bounds.Height) {
                // consider landscape monitors
                float proportion = (float)newSize.Height/ newSize.Width;
                SizeF newSizeF = new SizeF(newSize.Width, newSize.Height);
                do {
                    newSizeF.Width--;
                    newSizeF.Height -= proportion;
                } while (newSizeF.Width > Screen.PrimaryScreen.Bounds.Width || newSizeF.Height > Screen.PrimaryScreen.Bounds.Height);
                newSize = new Size((int)Math.Round(newSizeF.Width), (int)Math.Round(newSizeF.Height));
                ;
            }
            if (newSize.Width > Screen.PrimaryScreen.Bounds.Width) {
                float cropRatio = (float)Screen.PrimaryScreen.Bounds.Width / newSize.Width;
                newSize.Width = Screen.PrimaryScreen.Bounds.Width;
                double height = newSize.Height * cropRatio;
                newSize.Height = (int)Math.Floor(height);
            } else if (newSize.Height > Screen.PrimaryScreen.Bounds.Height) {
                float cropRatio = (float)Screen.PrimaryScreen.Bounds.Height / newSize.Height;
                newSize.Height = Screen.PrimaryScreen.Bounds.Height;
                double width = newSize.Width * cropRatio;
                newSize.Width = (int)Math.Floor(width);
            }
            Size widening = newSize - size;
            var newLocation = Point.Add(location, widening.Divide(-2));
            //form.Hide();
            //SuspendDrawing(this);
            pictureBox.Size = newSize;
            form.Size = newSize;
            form.Location = newLocation;
            //ResumeDrawing(this);
            //form.Show();
        }

        private void ZoomSmooth(double ratio, Form form, PictureBox pictureBox) {
            Size size;
            if (pictureBox.Width >= Screen.PrimaryScreen.Bounds.Width &&
                pictureBox.Height >= Screen.PrimaryScreen.Bounds.Height) {
                size = pictureBox.Size;
            } else {
                size = form.Size;
            }

            Point location;
            if (pictureBox.Width >= Screen.PrimaryScreen.Bounds.Width &&
                pictureBox.Height >= Screen.PrimaryScreen.Bounds.Height) {
                location = pictureBox.Location;
            } else {
                location = form.Location;
            }
            
            double enlargementRatio = AnimationHelper.GetEnlargementValue(ratio);
            var newSize = new Size {
                Width  = Convert.ToInt32(size.Width  * enlargementRatio),
                Height = Convert.ToInt32(size.Height * enlargementRatio)
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
            //parent.Size = newSize;
            //parent.Location = newLocation;
            while (_resizing && !ModifierKeys.HasFlag(Keys.Alt) && (pictureBox.Size - newSize).AbsMore(widening)) {
                pictureBox.Size += widening;
                //Application.DoEvents();
                //Application.DoEvents();
                //Application.DoEvents();
                if (this.Size.Width < Screen.PrimaryScreen.Bounds.Width &&
                    this.Size.Height < Screen.PrimaryScreen.Bounds.Height) {
                    form.Size += widening;
                    form.Location -= shift;
                } else {
                    pictureBox.Location -= shift;
                }
                //parent.Size += widening;
                //Application.DoEvents();
                //Application.DoEvents();
                //parent.Location -= shift;
                Application.DoEvents();
            }
            if (this.Size.Width < Screen.PrimaryScreen.Bounds.Width &&
                this.Size.Height < Screen.PrimaryScreen.Bounds.Height) {
                form.Size += widening;
                form.Location -= shift;
            } else {
                pictureBox.Size = newSize;
                pictureBox.Location = newLocation;
            }
        }

        #endregion

        private void GiferForm_KeyDown(object sender, KeyEventArgs e) {
            if (_currentImagePath != null && (e.KeyCode == Keys.Right || e.KeyCode == Keys.Left)) {
                if (e.KeyCode == Keys.Right) {
                    _currentImagePath = _imagesInFolder.Next(_currentImagePath);
                } else if (e.KeyCode == Keys.Left) {
                    _currentImagePath = _imagesInFolder.Previous(_currentImagePath);
                }
                SetImage((Bitmap)Bitmap.FromFile(_currentImagePath));
            //} else if (e.KeyCode == Keys.H) {
            //    ShowHelp(_config);
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
            } else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.P) {
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

        private void ShowHelp(Configuration config) {
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

		private void timer1_Tick(object sender, EventArgs e) {
            pictureBox1.Image = _gifImage.Next();
        }

        private void timerUpdateTaskbarIcon_Tick(object sender, EventArgs e) {
            if (pictureBox1.Image != null) {
                this.Icon = Icon.FromHandle((PadImage(pictureBox1.Image)).GetHicon());
            }            
        }

        public static Bitmap PadImage(Image image) {
            int largestDimension = Math.Max(image.Height, image.Width);
            var squareSize = new Size(largestDimension, largestDimension);
            Bitmap squareImage = new Bitmap(squareSize.Width, squareSize.Height);
            using (Graphics graphics = Graphics.FromImage(squareImage)) {
                //graphics.FillRectangle(Brushes.White, 0, 0, squareSize.Width, squareSize.Height);
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Low;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                graphics.DrawImage(image, (squareSize.Width / 2) - (image.Width / 2), (squareSize.Height / 2) - (image.Height / 2), image.Width, image.Height);
            }
            return squareImage;
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Right) {
                Application.Exit();
            }
        }

        private void GiferForm_Load(object sender, EventArgs e) {
            //bool showHelp;
            //bool.TryParse(_config.AppSettings.Settings["showHelpAtStartup"].Value, out showHelp);
            //if (showHelp) {
            //    ShowHelp(_config);
            //}
        }

        private void GiferForm_Activated(object sender, EventArgs e) {
            this.TopMost = true;
            Debug.WriteLine("this.TopMost: " + this.TopMost);
        }

        private void GiferForm_Deactivate(object sender, EventArgs e) {
            this.TopMost = false;
            Debug.WriteLine("this.TopMost: " + this.TopMost);
        }

		private void groupBox1_DragDrop(object s, DragEventArgs e) => this.Form1_DragDrop(s, e);
	}
}
