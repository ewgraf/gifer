using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualBasic.FileIO;

namespace gifer {
	public partial class GiferForm : Form {
		private GifImage _gifImage;
		private string _currentImagePath;
		private List<string> _imagesInFolder;
		private static readonly NaturalSortingComparer NaturalSortingComparer = new NaturalSortingComparer();
		private MoveFormWithControlsHandler _handler;
		private bool _helpWindow = true;
		private InterpolationMode _interpolationMode;
		private Rectangle _bounds;
		private bool _zooming;

		public GiferForm() {
			this.DoubleBuffered = true;
			this.Initialize();
		}

		public GiferForm(string imagePath) : this() {
			this.groupBox1.Visible = false;
			this.labelDragAndDrop.Visible = false;
			_helpWindow = false;
			LoadImageAndFolder(imagePath);
		}

		private void Initialize() {
			_gifImage?.Dispose();
			_gifImage = null;
			// hack so that "this.ResumeLayout(false)" at "this.InitializeComponent()" won't throw "ArgumentOutOfRangeException"
			// as we do "form.MaximumSize = new Size(int.MaxValue, int.MaxValue);" to go out of screen bounds for zooming
			this.MaximumSize = Screen.PrimaryScreen.Bounds.Size;
			this.InitializeComponent();
			this.timer1.Stop();
			this.timerUpdateTaskbarIcon.Stop();
			this.pictureBox1.Image?.Dispose();
			this.pictureBox1.Image = null;
			this.FormBorderStyle = FormBorderStyle.None;
			this.AllowDrop = true;
			this.pictureBox1.MouseWheel += pictureBox1_MouseWheel;
			this.pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
			Screen currentScreen = Screen.FromControl(this);
			Point center = (Point)currentScreen.Bounds.Size.Divide(2);
			this.Location = Point.Subtract(center, this.Size.Divide(2));
			Control[] controls = this.Controls.ToArray().Concat(this.groupBox1.Controls.ToArray()).ToArray();
			_handler = new MoveFormWithControlsHandler(form: this, controls: controls);
			foreach (Control c in controls) {
				c.MouseClick += (s, e) => pictureBox1_MouseClick(s, e);
			}
		}

		private void Reinitialize() {
			this.timer1?.Stop();
			this.timerUpdateTaskbarIcon?.Stop();
			this.pictureBox1.Image?.Dispose();
			this.pictureBox1.Image = null;
			_currentImagePath = null;
			_imagesInFolder.Clear();
			this.MaximumSize = Screen.PrimaryScreen.Bounds.Size;
			this.Controls.Clear();
			this.Initialize();
		}

		private void GiferForm_Load(object sender, EventArgs e) => this.MaximumSize = new Size(int.MaxValue, int.MaxValue);

		public void LoadImageAndFolder(string imagePath, bool loadFolder = true) {
			if (string.IsNullOrEmpty(imagePath)) {
				return;
			}
			if (Gifer.KnownImageFormats.Any(imagePath.ToUpper().EndsWith)) {
				GifImage image;
				try {
					image = new GifImage(imagePath);
				} catch (Exception ex) {
					HandleFileStreamException(ex, imagePath);
					this.Reinitialize();
					return;
				}
				if (image == null) {
					throw new Exception($@"Something went terribly wrong - despite ""new FileStream({imagePath}, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.SequentialScan);"" has thrown no exception, yet stream == null - gifer couldn't have loaded file. May be some inner operations in new FileStream were intercepted/overriden?");
				}
				if (this.InvokeRequired) {
					this.Invoke(new MethodInvoker(() => {
						SetGifImage(image);
					}));
				} else {
					SetGifImage(image);
				}
				_currentImagePath = imagePath;
				if (loadFolder) {
					_imagesInFolder = Directory.GetFiles(Path.GetDirectoryName(_currentImagePath))
											   .Where(path => Gifer.KnownImageFormats.Any(path.ToUpper().EndsWith))
											   .OrderBy(p => p, NaturalSortingComparer)
											   .ToList();
				}
			} else {
				MessageBox.Show($"Unknown image extension at: '{imagePath}' '{Path.GetExtension(imagePath)}'");
			}
		}

		private void HandleFileStreamException(Exception exception, string filePath) {
#if DEBUG
			throw exception;
#endif
			string title = $"Failed opening file";
			string parameters = $@"{Environment.NewLine}{Environment.NewLine}path: [{_currentImagePath}],{Environment.NewLine}{Environment.NewLine}exception: [{exception}]";

			var sb = new StringBuilder();
			sb.AppendLine($"{title} '{filePath}'");

			if (exception is ArgumentNullException) {
				sb.AppendLine($"Current file's path is null.");
			} else if (exception is ArgumentOutOfRangeException) {
				sb.AppendLine($"Mode, which open file in, contains an invalid value.");
			} else if (exception is ArgumentException) {
				sb.AppendLine(@"Path is an empty string (""), contains only white space, or contains one or more invalid characters. " +
							   $@"-or- path refers to a non-file device, such as ""con: "", ""com1: "", ""lpt1:"", etc. in an NTFS environment.");
			} else if (exception is NotSupportedException) {
				sb.AppendLine($@"Path refers to a non-file device, such as ""con: "", ""com1: "", ""lpt1: "", etc. in a non-NTFS environment.");
			} else if (exception is SecurityException) {
				sb.AppendLine($"Current user does not have the required permission to open file.");
			} else if (exception is FileNotFoundException) {
				sb.AppendLine("The file, specified by path, cannot be found, such as when mode is FileMode.Truncate or FileMode.Open, and the file does not exist. " +
							   $"The file must already exist in these modes.");
			} else if (exception is DirectoryNotFoundException) {
				sb.AppendLine($"The specified path is invalid, such as being on an unmapped drive.");
			} else if (exception is PathTooLongException) {
				sb.AppendLine("The specified path, file name, or both exceed the system-defined maximum length. " +
							   $"For example, on Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.");
			} else if (exception is IOException) {
				sb.AppendLine("An I/O error, such as specifying FileMode.CreateNew when the file specified by path already exists, occurred. " +
							   $"-or- The stream has been closed.");
			} else if (exception is UnauthorizedAccessException) {
				sb.AppendLine($"Current user does not have the required permission to open file.");
			} else {
				sb.AppendLine($"Unpredicted exception occured.");
			}
			MessageBox.Show(sb.ToString(), title);

		}

		private void SetGifImage(GifImage gifImage) {
			timer1.Stop();
			timerUpdateTaskbarIcon.Stop();
			_gifImage?.Dispose();
			_gifImage = gifImage;
			Size newSize;
			Screen currentScreen = Screen.FromControl(this);
			if (gifImage.Width > currentScreen.Bounds.Width || gifImage.Height > currentScreen.Bounds.Height) {
				newSize = gifImage.Size.ResizeProportionaly(currentScreen.Bounds.Size);
			} else {
				newSize = gifImage.Size;
			}
			Point center = (Point)currentScreen.Bounds.Size.Divide(2);
			Point newLocation = Point.Subtract(center, newSize.Divide(2));
			if (_gifImage.IsGif) {
				timer1.Interval = _gifImage.CurrentFrameDelayMilliseconds;
				timer1.Start();
				timerUpdateTaskbarIcon.Start();
				// timer1 OnTick sets pictureBox1's Image
			} else { // if plain image
				this.Invalidate();
				this.Icon = Icon.FromHandle(gifImage.Image.GetHicon());
			}
			this.Location = newLocation;
			this.Size = newSize;
			this.BringToFront();
		}

		#region DragDrop

		private void Form1_DragEnter(object sender, DragEventArgs e) => e.Effect = DragDropEffects.All;

		private void Form1_DragDrop(object sender, DragEventArgs e) {
			this.groupBox1.Visible = false;
			this.labelDragAndDrop.Visible = false;
			_helpWindow = false;
			string imagePath = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];
			LoadImageAndFolder(imagePath);
			this.Activate();
		}

		private void groupBox1_DragDrop(object s, DragEventArgs e) => this.Form1_DragDrop(s, e);

		#endregion

		#region Resizing

		public void pictureBox1_MouseWheel(object sender, MouseEventArgs e) {
			if (_helpWindow) {
				return;
			}

			var sw = Stopwatch.StartNew();
			this.pictureBox1_Resize(sender, e);
			sw.Stop();
			Debug.WriteLine($"Zoomed done in {sw.Elapsed.ToString()} sec.");
		}

		private void pictureBox1_Resize(object sender, EventArgs e) {
			var args = e as MouseEventArgs;
			if (args == null) { // if resize is caused not by mouse wheel, but by 'pictureBox1.Size = ' or '+='.
				return;
			}

			if (_zooming) {
				return;
			}

			int delta = args.Delta;
			if (delta == 0) {
				return;
			}

			float ratio = 1.35f;
			if (ModifierKeys.HasFlag(Keys.Control)) {
				ratio = 1.05f;
			} else if (ModifierKeys.HasFlag(Keys.Shift)) {
				ratio = 2.0f;
			}

			_bounds = Zoom(this, Math.Sign(delta) * ratio);
			_zooming = true;
			this.pictureBox1.Invalidate();
			//ZoomSmooth(Math.Sign(delta) * ratio, this, this.pictureBox1);
		}

		private Rectangle Zoom(Form form, float ratio) {
			float enlargementRatio = AnimationHelper.GetEnlargementValue(ratio);
			var newSize = new SizeF {
				Width = form.Size.Width * enlargementRatio,
				Height = form.Size.Height * enlargementRatio
			};
			SizeF widening = newSize - form.Size;
			var newLocation = PointF.Add(form.Location, widening.Divide(-2));
			form.MaximumSize = new Size(int.MaxValue, int.MaxValue);
			Point cursorLocationOnImage = this.pictureBox1.PointToClient(Cursor.Position);
			float xRatio = cursorLocationOnImage.X / (float)form.Size.Width;
			float yRatio = cursorLocationOnImage.Y / (float)form.Size.Height;
			var newCursorPosition = new PointF(newSize.Width * xRatio, newSize.Height * yRatio);
			float widthDifference = (newSize.Width - form.Size.Width) / 2;
			float heightDifference = (newSize.Height - form.Size.Height) / 2;
			float shiftX = (widthDifference - (newCursorPosition.X - cursorLocationOnImage.X));
			float shiftY = (heightDifference - (newCursorPosition.Y - cursorLocationOnImage.Y));
			var newBounds = new Rectangle((int)Math.Round(newLocation.X + shiftX),
										  (int)Math.Round(newLocation.Y + shiftY),
										  (int)Math.Round(newSize.Width),
										  (int)Math.Round(newSize.Height));
			return newBounds;
		}

		private void ZoomSmooth(float ratio, Form form, PictureBox pictureBox) {
			var size = form.Size;
			float enlargementRatio = AnimationHelper.GetEnlargementValue(ratio);
			var newSize = new SizeF {
				Width = form.Size.Width * enlargementRatio,
				Height = form.Size.Height * enlargementRatio
			};
			SizeF widening = newSize - size;
			var newLocation = PointF.Add(form.Location, widening.Divide(-2));
			form.MaximumSize = new Size(int.MaxValue, int.MaxValue);
			Point cursorLocationOnImage = this.pictureBox1.PointToClient(Cursor.Position);
			float xRatio = cursorLocationOnImage.X / (float)form.Size.Width;
			float yRatio = cursorLocationOnImage.Y / (float)form.Size.Height;
			var newCursorPosition = new PointF(newSize.Width * xRatio, newSize.Height * yRatio);
			float widthDifference = (newSize.Width - form.Size.Width) / 2;
			float heightDifference = (newSize.Height - form.Size.Height) / 2;
			float shiftX = (widthDifference - (newCursorPosition.X - cursorLocationOnImage.X));
			float shiftY = (heightDifference - (newCursorPosition.Y - cursorLocationOnImage.Y));
			GDIHelper.MoveWindow(this.Handle,
					   (int)Math.Round(newLocation.X + shiftX),
					   (int)Math.Round(newLocation.Y + shiftY),
					   (int)Math.Round(newSize.Width),
					   (int)Math.Round(newSize.Height),
					   repaint: false);
			return;
			//Size size = screen.Bounds.Size;
			//Point location = form.Location;
			//float enlargementRatio = AnimationHelper.GetEnlargementValue(ratio);
			//var newSize = new Size {
			//	Width = Convert.ToInt32(size.Width * enlargementRatio),
			//	Height = Convert.ToInt32(size.Height * enlargementRatio)
			//};
			//Size widening = newSize - size;
			//var newLocation = Point.Add(location, widening.Divide(-2));
			////   const
			//// ---------- -> steps : so that when 'widening' rizes, 'steps' reduses, to resize larger window faster
			////  widening
			////
			//// Ex: let widening 64 -> be steps 64, diff 128 -> steps 32
			////	c
			//// -------- -> 64 steps of resizing => c = 64*64 = 4096
			////	64
			//// so let it be 4096. pretty round, huh
			//int steps = 4096 / (Math.Abs(widening.Width + widening.Height) / 2);
			//Debug.WriteLine($"Steps: {steps}");
			//widening = widening.Divide(steps).RoundToPowerOf2();
			//Size shift = widening.Divide(2);
			////parent.Size = newSize;
			////parent.Location = newLocation;
			//while (_resizing && !ModifierKeys.HasFlag(Keys.Alt) && (pictureBox.Size - newSize).AbsMore(widening)) {
			//	pictureBox.Size += widening;
			//	//Application.DoEvents();
			//	//Application.DoEvents();
			//	//Application.DoEvents();
			//	if (this.Size.Width < Screen.PrimaryScreen.Bounds.Width &&
			//		this.Size.Height < Screen.PrimaryScreen.Bounds.Height) {
			//		form.Size += widening;
			//		form.Location -= shift;
			//	} else {
			//		pictureBox.Location -= shift;
			//	}
			//	//parent.Size += widening;
			//	//Application.DoEvents();
			//	//Application.DoEvents();
			//	//parent.Location -= shift;
			//	Application.DoEvents();
			//}
			//if (this.Size.Width < Screen.PrimaryScreen.Bounds.Width &&
			//	this.Size.Height < Screen.PrimaryScreen.Bounds.Height) {
			//	form.Size += widening;
			//	form.Location -= shift;
			//} else {
			//	pictureBox.Size = newSize;
			//	pictureBox.Location = newLocation;
			//}
		}

		// Gaussiana [0.01, 0.05, 0.1, 0.15, 0.2, 0.25, 0.3, 0.35, 0.4, 0.35, 0.3, 0.25, 0.2, 0.15, 0.1, 0.05, 0.01] / 3 => Sum = ~1
		//private static double[] Gaussiana = new[] { 0.003, 0.016, 0.03, 0.05, 0.06, 0.083, 0.1, 0.116,  0.13,  0.116, 0.1, 0.083, 0.06, 0.05, 0.03, 0.016, 0.003 };
		//private static int[] Gaussiana = new[] { 64, 32, 16, 8, 4, 2, 4, 8, 16, 32, 64 };
		//private float Gauss(float x) {			
		//  return exp(-(x - mu) ^ 2 / (2 * sigma ^ 2)) / sqrt(2 * pi * sigma ^ 2)
		//}

		#endregion

		private void GiferForm_KeyDown(object sender, KeyEventArgs e) {
			if (_currentImagePath != null && (e.KeyCode == Keys.Right || e.KeyCode == Keys.Left)) {
				if (e.KeyCode == Keys.Right) {
					_currentImagePath = _imagesInFolder.Next(_currentImagePath);
				} else if (e.KeyCode == Keys.Left) {
					_currentImagePath = _imagesInFolder.Previous(_currentImagePath);
				}
				LoadImageAndFolder(_currentImagePath, loadFolder: false);
			} else if (e.KeyCode == Keys.H) {
				_helpWindow = true;
				this.Reinitialize();
			} else if (e.KeyCode == Keys.Delete) {
				if (_currentImagePath == null) {
					return;
				}
				this.timer1.Stop();
				this.timerUpdateTaskbarIcon.Stop();
				string imageToDeletePath = _currentImagePath;
				_currentImagePath = _imagesInFolder.Next(_currentImagePath);
				_imagesInFolder.Remove(imageToDeletePath);
				if (_currentImagePath == null || !_imagesInFolder.Any()) {
					this.Reinitialize();
				} else {
					LoadImageAndFolder(_currentImagePath, loadFolder: false);
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
			} else if (e.KeyCode == Keys.D1) {
				PaintWith(InterpolationMode.NearestNeighbor);
			} else if (e.KeyCode == Keys.D2) {
				PaintWith(InterpolationMode.Bilinear);
			} else if (e.KeyCode == Keys.D3) {
				PaintWith(InterpolationMode.HighQualityBilinear);
			} else if (e.KeyCode == Keys.D4) {
				PaintWith(InterpolationMode.HighQualityBicubic);
			}
		}

		private void timer1_Tick(object sender, EventArgs e) {
			_gifImage.Next();
			this.pictureBox1.Image = _gifImage.Curr();
			this.timer1.Interval = _gifImage.CurrentFrameDelayMilliseconds;
		}

		private void timerUpdateTaskbarIcon_Tick(object sender, EventArgs e) {
			if (this.pictureBox1.Image != null) {
				UpdateThumbnail(this.pictureBox1.Image);
			}
		}

		private void UpdateThumbnail(Image image) => this.Icon = Icon.FromHandle(image.Pad().GetHicon());

		private void pictureBox1_MouseClick(object sender, MouseEventArgs e) {
			if (e.Button == MouseButtons.Right) {
				Application.Exit();
			}
		}

		private void GiferForm_Activated(object sender, EventArgs e) => this.TopMost = true;

		private void GiferForm_Deactivate(object sender, EventArgs e) => this.TopMost = false;

		private void PaintWith(InterpolationMode interpolationMode) {
			_interpolationMode = interpolationMode;
			this.pictureBox1.Invalidate();
		}

		private void pictureBox1_Paint(object s, PaintEventArgs e) {
			var sw = Stopwatch.StartNew();
			if (_gifImage?.Image != null) {
				if (_zooming) {
					GDIHelper.MoveWindow(this.Handle, _bounds.X, _bounds.Y, _bounds.Width, _bounds.Height, repaint: false);
					_zooming = false;
				}
				var srcdst = GetSrcDsrAreas(this, _gifImage);
				e.Graphics.InterpolationMode = _interpolationMode;
				e.Graphics.CompositingMode = CompositingMode.SourceCopy;
				e.Graphics.CompositingQuality = CompositingQuality.HighSpeed;
				e.Graphics.SmoothingMode = SmoothingMode.HighSpeed;
				e.Graphics.DrawImage(
					_gifImage.Image,
					srcdst.Item2, // destination rectangle
					srcdst.Item1, // source rectangle
					GraphicsUnit.Pixel);
			}
			base.OnPaint(e);
			sw.Stop();
			Debug.WriteLine($"OnPaint done in {sw.Elapsed.ToString()} sec.");
		}

		private Tuple<RectangleF, RectangleF> GetSrcDsrAreas(Form form, GifImage gifImage) {
			var screenBounds = Screen.FromControl(form).Bounds;
			var srcArea = new RectangleF(0, 0, gifImage.Width, gifImage.Height);
			var dstArea = new RectangleF(0, 0, form.Width, form.Height);
			// only visible parts, for ultra fast drawing
			//bool outOfAllBounds = false;
			//if (form.Location.X < 0 &&
			//  form.Location.Y < 0 &&
			//  form.Location.X + form.Width > screenBounds.Width &&
			//  form.Location.Y + form.Height > screenBounds.Height) {
			//  outOfAllBounds = true;
			//}
			//if (outOfAllBounds) {
			//  float ratio = (float)form.Width / gifImage.Width;
			//  srcArea = new RectangleF(-form.Location.X / ratio, -form.Location.Y / ratio, screenBounds.Width / ratio, screenBounds.Height / ratio);
			//  dstArea = new RectangleF(-form.Location.X, -form.Location.Y, screenBounds.Width, screenBounds.Height);
			//}
			return Tuple.Create(srcArea, dstArea);
		}	 

		private void GiferForm_KeyPress(object sender, KeyPressEventArgs e) {
			if (e.KeyChar == ' ') {
				if (this.timer1.Enabled) {
					this.timer1.Stop();
					this.timerUpdateTaskbarIcon.Stop();
				} else {
					this.timer1.Start();
					this.timerUpdateTaskbarIcon.Start();
				}
			} else if (e.KeyChar == ',' || e.KeyChar == 'б' || e.KeyChar == 'Б' || e.KeyChar == '<') {
				this.timer1.Stop();
				this.timerUpdateTaskbarIcon.Stop();
				_gifImage.Prev();
				this.pictureBox1.Image = _gifImage.Curr();
				UpdateThumbnail(this.pictureBox1.Image);
			} else if (e.KeyChar == '.' || e.KeyChar == 'ю' || e.KeyChar == 'Ю' || e.KeyChar == '>') {
				this.timer1.Stop();
				this.timerUpdateTaskbarIcon.Stop();
				_gifImage.Next();
				this.pictureBox1.Image = _gifImage.Curr();
				UpdateThumbnail(this.pictureBox1.Image);
			}
		}
	}
}
