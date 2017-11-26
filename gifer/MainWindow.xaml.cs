using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using gifer.Domain;
using gifer.Utils;
using Microsoft.VisualBasic.FileIO;
using System.Security;

namespace giferWpf {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private readonly Configuration _config;
        private readonly DispatcherTimer _gifTimer;
        private readonly DispatcherTimer _resizeTimer;
        private readonly DispatcherTimer _iconTimer;
        private GifImage _gifImage;
        private List<string> _imagesInFolder;
        private string _currentImagePath;
        private WriteableBitmap _writableBitmap;
        private Stopwatch _drawindDelayStopwath;

        public MainWindow() {
            _config = ConfigurationManager.OpenExeConfiguration($@"{AppDomain.CurrentDomain.BaseDirectory}\gifer.exe").Setup();

            InitializeComponent();
            
            var horizontalMargin = SystemParameters.PrimaryScreenWidth  / 2 - this.pictureBox1.Width  / 2;
            var verticalMargin   = SystemParameters.PrimaryScreenHeight / 2 - this.pictureBox1.Height / 2;
            this.pictureBox1.Margin = new Thickness(horizontalMargin, verticalMargin, horizontalMargin, verticalMargin);
            this.canvas.Margin = new Thickness(0d, 0d, SystemParameters.PrimaryScreenWidth - this.canvas.Width, SystemParameters.PrimaryScreenHeight - this.canvas.Height);
            _drawindDelayStopwath = new Stopwatch();
            _gifTimer = new DispatcherTimer();
            _resizeTimer = new DispatcherTimer();
            _iconTimer = new DispatcherTimer();
            _gifTimer.Tick += (s, e) => {
                _drawindDelayStopwath.Restart();
                _gifTimer.Stop();
                _gifImage.DrawNext(ref _writableBitmap);
                _drawindDelayStopwath.Stop();
                int delay = _gifImage.CurrentFrameDelay - (int)_drawindDelayStopwath.ElapsedMilliseconds;
                _gifTimer.Interval = new TimeSpan(0, 0, 0, 0, milliseconds: delay > 0 ? delay : 0);
                _gifTimer.Start();
            };
            _resizeTimer.Tick += new EventHandler(resizeTimer_Tick);
            _iconTimer.Tick += (s, e) => this.Icon = _writableBitmap;
            _resizeTimer.Interval = new TimeSpan(0, 0, 0, 0, 1000 / 60);
            _iconTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);

            if (Application.Current.Properties["Args"] != null) {
                string filePath = Application.Current.Properties["Args"].ToString();
                SetImage(filePath);
            } else {
                SetDefaultImage();
            }

            if (_config.GetShowHelpAtStartup()) {
                ShowHelp(_config);
            }
        }

        private void SetImage(string imagePath) {
            if (!Gifer.KnownImageFormats.Any(imagePath.ToUpper().EndsWith)) {
                MessageBox.Show($"Unknown image extension at: '{imagePath}' '{Path.GetExtension(imagePath)}'");
                this.Close();
            }

            _gifTimer?.Stop();
            _iconTimer?.Stop();
            _writableBitmap = null;
            this.pictureBox1.Source = null;
            _gifImage?.Dispose();
            GC.Collect();

            try {
                _gifImage = new GifImage(imagePath);
            } catch (Exception ex) {
                HandleFileStreamException(ex);
                this.Close();
            }
            if (_gifImage == null) {
                throw new Exception($@"Something went terribly wrong - despite ""new FileStream({imagePath}, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.SequentialScan);"" has thrown no exception, yet stream == null - gifer couldn't have loaded file. May be some inner operations in new FileStream were intercepted/overriden?");
            }

            _currentImagePath = imagePath;
            _imagesInFolder = Directory.GetFiles(Path.GetDirectoryName(_currentImagePath))
                .Where(path => Gifer.KnownImageFormats.Any(path.ToUpper().EndsWith))
                .ToList();
            
            _writableBitmap = _gifImage.GetWritableBitmap();

            this.Title = _currentImagePath;
            this.pictureBox1.Margin = ResizeImageMargin(this.pictureBox1, _writableBitmap.PixelWidth, _writableBitmap.PixelHeight);
            this.pictureBox1.Source = _writableBitmap;

            if (_gifImage.IsGif) {
                _gifTimer.Interval = new TimeSpan(0, 0, 0, 0, milliseconds: _gifImage.CurrentFrameDelay);
                _gifTimer.Start();
                _iconTimer.Start();
            } else {
                this.pictureBox1.Source.Freeze();
            }
        }

        private void HandleFileStreamException(Exception exception) {
            string title = $"Failed opening file";
            string parameters = $@"{Environment.NewLine}{Environment.NewLine}path: [{_currentImagePath}],{Environment.NewLine}{Environment.NewLine}exception: [{exception}]";

            if (exception is ArgumentNullException) {
                MessageBox.Show($"Current file's path is null. {parameters}", title);
            } else if (exception is ArgumentOutOfRangeException) {
                MessageBox.Show($"Mode, which open file in, contains an invalid value. {parameters}", title);
            } else if (exception is ArgumentException) {
                MessageBox.Show(@"Path is an empty string (""), contains only white space, or contains one or more invalid characters. " +
                               $@"-or- path refers to a non-file device, such as ""con: "", ""com1: "", ""lpt1:"", etc. in an NTFS environment. {parameters}", title);
            } else if (exception is NotSupportedException) {
                MessageBox.Show($@"Path refers to a non-file device, such as ""con: "", ""com1: "", ""lpt1: "", etc. in a non-NTFS environment. {parameters}", title);
            } else if (exception is SecurityException) {
                MessageBox.Show($"Current user does not have the required permission to open file. {parameters}", title);
            } else if (exception is FileNotFoundException) {
                MessageBox.Show("The file, specified by path, cannot be found, such as when mode is FileMode.Truncate or FileMode.Open, and the file does not exist. " +
                               $"The file must already exist in these modes. {parameters}", title);
            } else if (exception is DirectoryNotFoundException) {
                MessageBox.Show($"The specified path is invalid, such as being on an unmapped drive. {parameters}", title);
            } else if (exception is PathTooLongException) {
                MessageBox.Show("The specified path, file name, or both exceed the system-defined maximum length. " +
                               $"For example, on Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters. {parameters}", title);
            } else if (exception is IOException) {
                MessageBox.Show("An I/O error, such as specifying FileMode.CreateNew when the file specified by path already exists, occurred. " +
                               $"-or- The stream has been closed. {parameters}", title);
            } else if (exception is UnauthorizedAccessException) {
                MessageBox.Show($"Current user does not have the required permission to open file. {parameters}", title);
            } else {
                MessageBox.Show($"Unpredicted exception occured. {parameters}", title);
            }
        }

        private void ShowHelp(Configuration config) {
            bool showHelp = config.GetShowHelpAtStartup();
            var helpForm = new HelpWindow(showHelp);
            helpForm.ShowDialog();
            config.SetShowHelpAtStartup(helpForm.ShowHelpAtStartup);
        }
        
        private void SetDefaultImage() {
            var image = new Bitmap(256, 256);
            using (Graphics g = Graphics.FromImage(image)) {
                g.FillRectangle(Brushes.LightGray, 0, 0, image.Width, image.Height);
                g.DrawString("[Drag GIF/Image Here]", new Font("Courier New", 9), Brushes.Black, 47, 125);
            }
            this.pictureBox1.Source = image.ToBitmapSource();
        }

        #region Events

        private void pictureBox1_Drop(object sender, DragEventArgs e) {
            string imagePath = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];
            SetImage(imagePath);
            this.Activate();
        }

        #region Moving

        private bool _moving = false;
        private System.Windows.Point _position;

        private void pictureBox1_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton != MouseButtonState.Pressed) {
                return;
            }
            _moving = true;
            _position = e.GetPosition(this.pictureBox1);
            this.pictureBox1.CaptureMouse();
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e) {
            var s = sender as System.Windows.Controls.Image;
            if (!_moving || s == null) {
                return;
            }
            System.Windows.Point position = e.GetPosition(this);
            s.SetValue(Canvas.LeftProperty, e.GetPosition(canvas).X - s.Margin.Left - _position.X);
            s.SetValue(Canvas.TopProperty,  e.GetPosition(canvas).Y - s.Margin.Top  - _position.Y);
        }

        private void pictureBox1_MouseUp(object sender, MouseButtonEventArgs e) {
            var s = sender as System.Windows.Controls.Image;
            if (s == null) {
                return;
            }
            _moving = false;
        }

        #endregion

        #region Resizing

        private bool _resizing = false;
        private Thickness _newMargin;
        private Thickness _deltaMargin;
        private double _deltaWidth;
        private double _deltaHeigth;
        private System.Windows.Size _newSize;
        private uint _resizeIterations;

        private void resizeTimer_Tick(object sender, EventArgs e) {
            if (_resizeIterations-->0) {
                Application.Current.Dispatcher.Invoke(
                    DispatcherPriority.Background,
                    new ThreadStart(delegate {
                        pictureBox1.Margin = new Thickness(
                            pictureBox1.Margin.Left + _deltaMargin.Left,
                            pictureBox1.Margin.Top + _deltaMargin.Top,
                            pictureBox1.Margin.Right + _deltaMargin.Right,
                            pictureBox1.Margin.Bottom + _deltaMargin.Bottom
                        );
                        pictureBox1.Width  += _deltaWidth;
                        pictureBox1.Height += _deltaHeigth;
                }));
            } else {
                _resizeTimer.Stop();
            }            
        }

        private void pictureBox1_MouseWheel(object sender, MouseWheelEventArgs e) {
            this.pictureBox1.CaptureMouse();
            pictureBox1_SizeChanged(sender, e);
        }

        private void pictureBox1_SizeChanged(object sender, EventArgs e) {
            var args = e as MouseWheelEventArgs;
            if (args == null) { // if resize is caused not by mouse wheel, but by 'pictureBox1.Size = ' or '+='.
                return;
            }

            if (_resizing) {
                return;
            }

            int delta = args.Delta;
            if (delta == 0) {
                return;
            }

            double ratio = 1.35;
            
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) {
                ratio = 1.05;
            } else if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)) {
                ratio = 2.0;
            }

            _resizing = true;
            Zoom(Math.Sign(delta) * ratio, args);
            _resizing = false;
        }

        private void Zoom(double ratio, MouseWheelEventArgs e) {
            Thickness margin = pictureBox1.Margin;
            var size = new System.Windows.Size(
                SystemParameters.PrimaryScreenWidth - (pictureBox1.Margin.Left + pictureBox1.Margin.Right),
                SystemParameters.PrimaryScreenHeight - (pictureBox1.Margin.Top + pictureBox1.Margin.Bottom)
            );
            double enlargementRatio = AnimationHelper.GetEnlargementValue(ratio);
            double delta = enlargementRatio;
            _newSize = new System.Windows.Size(
                size.Width * enlargementRatio,
                size.Height * enlargementRatio
            );

            _deltaWidth = ((_newSize.Width - pictureBox1.Width)) / 60;
            _deltaHeigth = ((_newSize.Height - pictureBox1.Height)) / 60;

            float animationAcceleration = 10.0f;

            _deltaWidth *= animationAcceleration;
            _deltaHeigth *= animationAcceleration;

            _newMargin = new Thickness(
                margin.Left - (pictureBox1.Width - _newSize.Width) / 2,
                margin.Top - (pictureBox1.Height - _newSize.Height) / 2,
                margin.Right - (pictureBox1.Width - _newSize.Width) / 2,
                margin.Bottom - (pictureBox1.Height - _newSize.Height) / 2
            );
            //System.Windows.Point center = new System.Windows.Point(_newMargin.Left + _newSize.Width / 2, _newMargin.Top + _newSize.Height / 2);
            //System.Windows.Point position = e.GetPosition(this.pictureBox1);
            //_newMargin = new Thickness(
            //    _newMargin.Left   - Math.Sign(ratio) * (pictureBox1.Width / 2 - position.X) / 2,
            //    _newMargin.Top    - Math.Sign(ratio) * (pictureBox1.Height / 2 - position.Y) / 2,
            //    _newMargin.Right  + Math.Sign(ratio) * (pictureBox1.Width / 2 - position.X) / 2,
            //    _newMargin.Bottom + Math.Sign(ratio) * (pictureBox1.Height / 2 - position.Y) / 2
            //);
            _deltaMargin = new Thickness(
                ((pictureBox1.Margin.Left - _newMargin.Left) / 60) * animationAcceleration,
                ((pictureBox1.Margin.Top - _newMargin.Top) / 60) * animationAcceleration,
                ((pictureBox1.Margin.Right - _newMargin.Right) / 60) * animationAcceleration,
                ((pictureBox1.Margin.Bottom - _newMargin.Bottom) / 60) * animationAcceleration
            );
            _resizeIterations = (uint)((_newSize.Width - pictureBox1.Width) / _deltaWidth);
            _resizeTimer.Start();
        }

        #endregion
        
        private void Window_KeyDown(object sender, KeyEventArgs e) {
            switch (e.Key) {
                case Key.Left:
                case Key.Right:
                    if (_currentImagePath == null) {
                        return;
                    }
                    _gifTimer?.Stop();
                    _resizeTimer?.Stop();
                    _iconTimer?.Stop();
                    if (e.Key == Key.Left) {
                        _currentImagePath = _imagesInFolder.Previous(_currentImagePath);
                    } else if (e.Key == Key.Right) {
                        _currentImagePath = _imagesInFolder.Next(_currentImagePath);
                    }
                    SetImage(_currentImagePath);
                    break;
                case Key.H:
                    ShowHelp(_config);
                    break;
                case Key.Delete:
                    if (_currentImagePath == null) {
                        return;
                    }
                    string imageToDeletePath = _currentImagePath;
                    _currentImagePath = _imagesInFolder.Next(_currentImagePath);
                    SetImage(_currentImagePath);
                    _imagesInFolder.Remove(imageToDeletePath);
                    if (imageToDeletePath == _currentImagePath) {
                        _currentImagePath = null;
                        SetDefaultImage();
                    }
                    FileSystem.DeleteFile(imageToDeletePath, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                    break;
                case Key.P:
                    if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) {
                        if (_currentImagePath == null) {
                            return;
                        }
                        var p = new Process();
                        p.StartInfo.FileName = _currentImagePath;
                        p.StartInfo.Verb = "Print";
                        p.Start();
                    }
                    break;
                case Key.Escape:
                    Application.Current.Shutdown();
                    break;
                default:
                    break;
            }
        }

        private void pictureBox1_MouseRightButtonUp(object s, EventArgs e) {
            this.Close();
        }

        #endregion

        private Thickness ResizeImageMargin(System.Windows.Controls.Image image, double imageWidth, double imageHeight) {
            var center = new System.Windows.Point(
                    image.Margin.Left + image.Width / 2,
                    image.Margin.Top + image.Height / 2
                );
            if (imageWidth > SystemParameters.PrimaryScreenWidth || imageHeight > SystemParameters.PrimaryScreenHeight) {
                var size = ResizeProportionaly(
                    new System.Windows.Size(
                        imageWidth,
                        imageHeight),
                    new System.Windows.Size(
                        SystemParameters.PrimaryScreenWidth,
                        SystemParameters.PrimaryScreenHeight)
                );
                image.Width = size.Width;
                image.Height = size.Height;
            } else {
                image.Width = imageWidth;
                image.Height = imageHeight;
            }
            double horizontalMargin = center.X - image.Width / 2;
            double verticalMargin = center.Y - image.Height / 2;
            return new Thickness(horizontalMargin, verticalMargin, horizontalMargin, verticalMargin);
        }

        public static System.Windows.Size ResizeProportionaly(System.Windows.Size size, System.Windows.Size toFitSize) {
            double ratioX = toFitSize.Width / size.Width;
            double ratioY = toFitSize.Height / size.Height;
            double ratio = Math.Min(ratioX, ratioY);
            return size.Multiply(ratio);
        }
    }
}
