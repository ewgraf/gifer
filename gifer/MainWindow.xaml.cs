using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using gifer.Domain;
using gifer.Utils;
using Microsoft.VisualBasic.FileIO;

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

        public MainWindow() {
            _config = ConfigurationManager.OpenExeConfiguration($@"{AppDomain.CurrentDomain.BaseDirectory}\gifer.exe").Setup();

            InitializeComponent();
            
            var horizontalMargin = SystemParameters.PrimaryScreenWidth  / 2 - this.pictureBox1.Width  / 2;
            var verticalMargin   = SystemParameters.PrimaryScreenHeight / 2 - this.pictureBox1.Height / 2;
            this.pictureBox1.Margin = new Thickness(horizontalMargin, verticalMargin, horizontalMargin, verticalMargin);
            this.canvas.Margin = new Thickness(0d, 0d, SystemParameters.PrimaryScreenWidth - this.canvas.Width, SystemParameters.PrimaryScreenHeight - this.canvas.Height);
            _gifTimer = new DispatcherTimer();
            _gifTimer.Tick += new EventHandler(timer_Tick);
            _resizeTimer = new DispatcherTimer();
            _resizeTimer.Tick += new EventHandler(resizeTimer_Tick);
            _resizeTimer.Interval = new TimeSpan(0, 0, 0, 0, 1000 / 60);
            _iconTimer = new DispatcherTimer();
            _iconTimer.Tick += new EventHandler(iconTimer_Tick);
            _iconTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);

            if (Application.Current.Properties["Args"] != null) {
                string filePath = Application.Current.Properties["Args"].ToString();
                LoadImageAndFolder(filePath);
            } else {
                SetDefaultImage();
            }

            if (_config.GetShowHelpAtStartup()) {
                ShowHelp(_config);
            }
        }

        private void timer_Tick(object sender, EventArgs e) {
            this.pictureBox1.Source = _gifImage.Next().ToBitmapSource();
        }

        private void iconTimer_Tick(object sender, EventArgs e) {
            this.Icon = pictureBox1.Source;
        }

        private void SetImage(Bitmap image, bool updateTaskbarIcon = true) {
            _gifTimer?.Stop();
            _iconTimer?.Stop();
            var center = new System.Windows.Point(
                this.pictureBox1.Margin.Left + this.pictureBox1.Width  / 2,
                this.pictureBox1.Margin.Top  + this.pictureBox1.Height / 2
            );
            if (image.Width > SystemParameters.PrimaryScreenWidth || image.Height > SystemParameters.PrimaryScreenHeight) {
                var size = ResizeProportionaly(image.Size, new System.Drawing.Size((int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight));
                this.pictureBox1.Width  = size.Width;
                this.pictureBox1.Height = size.Height;
            } else {
                this.pictureBox1.Width  = image.Width;
                this.pictureBox1.Height = image.Height;
            }
            double horizontalMargin = center.X - this.pictureBox1.Width  / 2;
            double verticalMargin   = center.Y - this.pictureBox1.Height / 2;
            this.pictureBox1.Margin = new Thickness(horizontalMargin, verticalMargin, horizontalMargin, verticalMargin);
            _gifImage?.Dispose();
            GC.Collect();
            if (image.RawFormat == ImageFormat.Gif && ImageAnimator.CanAnimate(image) || image.RawFormat.Guid == new Guid("b96b3cb0-0728-11d3-9d7b-0000f81ef32e")) {
                _gifImage = new GifImage(image);
                pictureBox1.Source = _gifImage.Next().ToBitmapSource();
                _gifTimer.Interval = new TimeSpan(0, 0, 0, 0, milliseconds: _gifImage.Delay);
                _gifTimer.Start();
                _iconTimer.Start();
            } else { // if plain image
                if (updateTaskbarIcon) {
                    this.Icon = image.ToBitmapSource();
                }
                pictureBox1.Source = image.ToBitmapSource();
            }
        }

        private void SetDefaultImage() {
            var image = new Bitmap(256, 256);
            using (Graphics g = Graphics.FromImage(image)) {
                g.FillRectangle(Brushes.LightGray, 0, 0, image.Width, image.Height);
                g.DrawString("[Drag GIF/Image Here]", new Font("Courier New", 9), Brushes.Black, 47, 125);
            }
            SetImage(image, updateTaskbarIcon: false);
        }

        private Bitmap LoadImage(string imagePath) {
            try {
                switch (Path.GetExtension(imagePath)) {
                    default:
                        return (Bitmap)Bitmap.FromFile(imagePath);
                }
            } catch (Exception ex) {
                MessageBox.Show(ex.ToString());
                return null;
            }
        }

        private void LoadImageAndFolder(string imagePath) {
            if (string.IsNullOrEmpty(imagePath)) {
                return;
            }
            if (Gifer.KnownImageFormats.Any(imagePath.ToUpper().EndsWith)) {
                Bitmap image = LoadImage(imagePath);
                if (image == null) {
                    MessageBox.Show($"Can not load image: '{imagePath}'");
                }
                SetImage(image);
                _currentImagePath = imagePath;
                this.Title = _currentImagePath;
                _imagesInFolder = Directory.GetFiles(Path.GetDirectoryName(_currentImagePath))
                    .Where(path => Gifer.KnownImageFormats.Any(path.ToUpper().EndsWith))
                    .ToList();
            } else {
                MessageBox.Show($"Unknown image extension at: '{imagePath}' '{Path.GetExtension(imagePath)}'");
            }
        }

        public static System.Drawing.Size ResizeProportionaly(System.Drawing.Size size, System.Drawing.Size fitSize) {
            double ratioX = (double)fitSize.Width / (double)size.Width;
            double ratioY = (double)fitSize.Height / (double)size.Height;
            double ratio = Math.Min(ratioX, ratioY);
            return size.Multiply(ratio);
        }

        private void ShowHelp(Configuration config) {
            bool showHelp = config.GetShowHelpAtStartup();
            var helpForm = new HelpWindow(showHelp);
            helpForm.ShowDialog();
            config.SetShowHelpAtStartup(helpForm.ShowHelpAtStartup);
        }

        #region Events

        private void pictureBox1_Drop(object sender, DragEventArgs e) {
            string imagePath = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];
            LoadImageAndFolder(imagePath);
            this.Activate();
        }

        #region Moving

        private bool _moving;
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
            var position = e.GetPosition(this);
            s.SetValue(Canvas.LeftProperty, e.GetPosition(canvas).X - s.Margin.Left - _position.X);
            s.SetValue(Canvas.TopProperty,  e.GetPosition(canvas).Y - s.Margin.Top  - _position.Y);
            Debug.WriteLine($"{e.GetPosition(canvas).X} {e.GetPosition(canvas).Y} {position.X} {position.Y}");
        }

        private void pictureBox1_MouseUp(object sender, MouseButtonEventArgs e) {
            var s = sender as System.Windows.Controls.Image;
            if (s == null) {
                return;
            }
            _moving = false;
        }

        private void Thumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e) {
            var thumb = e.Source as UIElement;
            Canvas.SetLeft(thumb, Canvas.GetLeft(thumb) + e.HorizontalChange);
            Canvas.SetTop(thumb, Canvas.GetTop(thumb) + e.VerticalChange);
        }

        #endregion

        #region Resizing

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

        private bool _resizing = false;

        private void pictureBox1_MouseWheel(object sender, MouseWheelEventArgs e) {
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
            Zoom(Math.Sign(delta) * ratio);
            _resizing = false;
        }

        private void Zoom(double ratio) {
            Thickness margin = pictureBox1.Margin;
            var size = new System.Windows.Size(
                SystemParameters.PrimaryScreenWidth  - (pictureBox1.Margin.Left + pictureBox1.Margin.Right ),
                SystemParameters.PrimaryScreenHeight - (pictureBox1.Margin.Top  + pictureBox1.Margin.Bottom)
            );
            double enlargementRatio = Animation.GetEnlargementValue(ratio);
            double delta = enlargementRatio;
            _newSize = new System.Windows.Size(
                size.Width  * enlargementRatio,
                size.Height * enlargementRatio
            );

            _deltaWidth  = ((_newSize.Width  - pictureBox1.Width )) / 60;
            _deltaHeigth = ((_newSize.Height - pictureBox1.Height)) / 60;

            float animationAcceleration = 10.0f;

            _deltaWidth  *= animationAcceleration;
            _deltaHeigth *= animationAcceleration;

            _newMargin = new Thickness(
                margin.Left   - (pictureBox1.Width  - _newSize.Width ) / 2,
                margin.Top    - (pictureBox1.Height - _newSize.Height) / 2,
                margin.Right  - (pictureBox1.Width  - _newSize.Width ) / 2,
                margin.Bottom - (pictureBox1.Height - _newSize.Height) / 2
            );
            _deltaMargin = new Thickness(
                ((pictureBox1.Margin.Left   - _newMargin.Left  ) / 60) * animationAcceleration,
                ((pictureBox1.Margin.Top    - _newMargin.Top   ) / 60) * animationAcceleration,
                ((pictureBox1.Margin.Right  - _newMargin.Right ) / 60) * animationAcceleration,
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
                    if (e.Key == Key.Left) {
                        _currentImagePath = _imagesInFolder.Previous(_currentImagePath);
                    } else if (e.Key == Key.Right) {
                        _currentImagePath = _imagesInFolder.Next(_currentImagePath);
                    }
                    LoadImageAndFolder(_currentImagePath);
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
                    LoadImageAndFolder(_currentImagePath);
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

        private void pictureBox1_MouseRightButtonUp(object s, EventArgs e) => this.Close();

        #endregion
    }
}
