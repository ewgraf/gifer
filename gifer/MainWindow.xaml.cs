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
using System.Windows.Media.Imaging;
using System.Windows.Interop;
using System.Windows.Media;

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
            _gifImage.DrawNext(ref _writableBitmap);
            //pictureBox1.Source = _writableBitmap;
        }

        private void iconTimer_Tick(object sender, EventArgs e) {
            this.Icon = _writableBitmap;
        }

        private WriteableBitmap CreateImageSource(Stream stream) {
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.CacheOption = BitmapCacheOption.OnLoad;
            bi.StreamSource = stream;
            bi.EndInit();
            bi.Freeze();

            BitmapSource prgbaSource = new FormatConvertedBitmap(bi, PixelFormats.Pbgra32, null, 0);
            WriteableBitmap bmp = new WriteableBitmap(prgbaSource);
            int w = bmp.PixelWidth;
            int h = bmp.PixelHeight;
            int[] pixelData = new int[w * h];
            //int widthInBytes = 4 * w;
            int widthInBytes = bmp.PixelWidth * (bmp.Format.BitsPerPixel / 8); //equals 4*w
            bmp.CopyPixels(pixelData, widthInBytes, 0);

            bmp.WritePixels(new Int32Rect(0, 0, w, h), pixelData, widthInBytes, 0);
            bi = null;

            return bmp;
        }

        //private void SetImage(Bitmap image, bool updateTaskbarIcon = true) {
        //    //pictureBox1.Source = null;
        //    //UpdateLayout();
        //    //GC.Collect();
            
        //    BitmapSource source = Imaging.CreateBitmapSourceFromHBitmap(
        //        image.GetHbitmap(), 
        //        IntPtr.Zero, 
        //        Int32Rect.Empty, 
        //        BitmapSizeOptions.FromEmptyOptions());
        //    source.Freeze();
        //    pictureBox1.Source = null;
        //    GC.Collect();
        //    pictureBox1.Source = source;
        //    //UpdateLayout();
        //    //GC.Collect();
        //    return;
        //    _gifTimer?.Stop();
        //    _iconTimer?.Stop();
        //    var center = new System.Windows.Point(
        //        this.pictureBox1.Margin.Left + this.pictureBox1.Width  / 2,
        //        this.pictureBox1.Margin.Top  + this.pictureBox1.Height / 2
        //    );
        //    if (image.Width > SystemParameters.PrimaryScreenWidth || image.Height > SystemParameters.PrimaryScreenHeight) {
        //        var size = ResizeProportionaly(image.Size, new System.Drawing.Size((int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight));
        //        this.pictureBox1.Width  = size.Width;
        //        this.pictureBox1.Height = size.Height;
        //    } else {
        //        this.pictureBox1.Width  = image.Width;
        //        this.pictureBox1.Height = image.Height;
        //    }
        //    double horizontalMargin = center.X - this.pictureBox1.Width  / 2;
        //    double verticalMargin   = center.Y - this.pictureBox1.Height / 2;
        //    this.pictureBox1.Margin = new Thickness(horizontalMargin, verticalMargin, horizontalMargin, verticalMargin);
        //    _gifImage?.Dispose();
        //    _gifImage = null;
        //    this.pictureBox1.Source?.Freeze();
        //    this.pictureBox1.Source = null;
        //    UpdateLayout();
        //    GC.Collect();
        //    if (image.RawFormat == ImageFormat.Gif && ImageAnimator.CanAnimate(image) || image.RawFormat.Guid == new Guid("b96b3cb0-0728-11d3-9d7b-0000f81ef32e")) {
        //        _gifImage = new GifImage((Bitmap)image.Clone());

        //        _writableBitmap = null;
        //        Bitmap frame = _gifImage.Copy();
        //        _writableBitmap = frame.ToWritableBitmap();
        //        frame.Dispose();
        //        frame = null;

        //        this.pictureBox1.Source = _writableBitmap;
        //        _gifTimer.Interval = new TimeSpan(0, 0, 0, 0, milliseconds: _gifImage.Delay);
        //        _gifTimer.Start();
        //        if (updateTaskbarIcon) {
        //            _iconTimer.Start();
        //        }
        //    } else { // if plain image
        //        pictureBox1.Source = image.ToBitmapSource();
        //        pictureBox1.Source.Freeze();
        //        if (updateTaskbarIcon) {
        //            this.Icon = null;
        //            this.Icon = pictureBox1.Source;
        //            this.Icon.Freeze();
        //        }
        //    }
        //    GC.Collect();
        //}

        private static readonly byte[] GifHeader = new byte[] { 71, 73, 70, 56, 57, 97 };

        private void SetImage(Stream stream, bool updateTaskbarIcon = true) {
            _gifTimer?.Stop();
            _iconTimer?.Stop();
            var bytes = new byte[6];
            stream.Read(bytes, 0, 6);
            stream.Seek(0, SeekOrigin.Begin);
            _writableBitmap = CreateImageSource(stream);
            //stream.Seek(0, SeekOrigin.Begin);
            stream.Close();
            var center = new System.Windows.Point(
                this.pictureBox1.Margin.Left + this.pictureBox1.Width / 2,
                this.pictureBox1.Margin.Top + this.pictureBox1.Height / 2
            );
            if (_writableBitmap.Width > SystemParameters.PrimaryScreenWidth || _writableBitmap.Height > SystemParameters.PrimaryScreenHeight) {
                var size = ResizeProportionaly(new System.Drawing.Size((int)_writableBitmap.Width, (int)_writableBitmap.Height), new System.Drawing.Size((int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight));
                this.pictureBox1.Width = size.Width;
                this.pictureBox1.Height = size.Height;
            } else {
                this.pictureBox1.Width = _writableBitmap.Width;
                this.pictureBox1.Height = _writableBitmap.Height;
            }
            double horizontalMargin = center.X - this.pictureBox1.Width / 2;
            double verticalMargin = center.Y - this.pictureBox1.Height / 2;
            this.pictureBox1.Margin = new Thickness(horizontalMargin, verticalMargin, horizontalMargin, verticalMargin);
            //_gifImage?.Dispose();
            //_gifImage = null;
            this.pictureBox1.Source?.Freeze();
            this.pictureBox1.Source = null;
            //UpdateLayout();
            //GC.Collect();
            if (bytes.SequenceEqual(GifHeader)) {
                _gifImage = new GifImage((Bitmap)Bitmap.FromFile(_currentImagePath));

                //_writableBitmap = null;
                //_writableBitmap = image;
                this.pictureBox1.Source = _writableBitmap;
                _gifTimer.Interval = new TimeSpan(0, 0, 0, 0, milliseconds: _gifImage.Delay);
                _gifTimer.Start();
                if (updateTaskbarIcon) {
                    _iconTimer.Start();
                }
            } else { // if plain image
                this.pictureBox1.Source = _writableBitmap;
                //GC.Collect();
                //pictureBox1.Source = image.ToBitmapSource();
                //pictureBox1.Source.Freeze();
                //if (updateTaskbarIcon) {
                //    this.Icon = null;
                //    this.Icon = pictureBox1.Source;
                //    this.Icon.Freeze();
                //}
            }
            GC.Collect();
        }

        private void SetDefaultImage() {
            var image = new Bitmap(256, 256);
            using (Graphics g = Graphics.FromImage(image)) {
                g.FillRectangle(System.Drawing.Brushes.LightGray, 0, 0, image.Width, image.Height);
                g.DrawString("[Drag GIF/Image Here]", new Font("Courier New", 9), System.Drawing.Brushes.Black, 47, 125);
            }
            this.pictureBox1.Source = image.ToBitmapSource();
            //SetImage(image, updateTaskbarIcon: false);
        }

        private void LoadImageAndFolder(string imagePath) {
            if (string.IsNullOrEmpty(imagePath)) {
                return;
            }

            if (Gifer.KnownImageFormats.Any(imagePath.ToUpper().EndsWith)) {
                //Bitmap image = LoadImage(imagePath);
                //if (image == null) {
                //    MessageBox.Show($"Can not load image: '{imagePath}'");
                //}
                _currentImagePath = imagePath;
                var stream = new FileStream(imagePath, FileMode.Open);
                SetImage(stream);
                stream.Close();
                //image.Dispose();
                this.Title = _currentImagePath;
                _imagesInFolder = Directory.GetFiles(Path.GetDirectoryName(_currentImagePath))
                    .Where(path => Gifer.KnownImageFormats.Any(path.ToUpper().EndsWith))
                    .ToList();
            } else {
                MessageBox.Show($"Unknown image extension at: '{imagePath}' '{Path.GetExtension(imagePath)}'");
            }
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
            System.Windows.Point position = e.GetPosition(this);
            s.SetValue(Canvas.LeftProperty, e.GetPosition(canvas).X - s.Margin.Left - _position.X);
            s.SetValue(Canvas.TopProperty,  e.GetPosition(canvas).Y - s.Margin.Top  - _position.Y);
            Debug.WriteLine($"pictureBox1.Margin.Left: {pictureBox1.Margin.Left} pictureBox1.Margin.Top: {pictureBox1.Margin.Top}");
            Debug.WriteLine($"canvas.Margin.Left: {canvas.Margin.Left} canvas.Margin.Top: {canvas.Margin.Top}");
            Debug.WriteLine($"s.GetValue(Canvas.LeftProperty): {s.GetValue(Canvas.LeftProperty)} s.GetValue(Canvas.TopProperty): {s.GetValue(Canvas.TopProperty)}");
        }

        private void pictureBox1_MouseUp(object sender, MouseButtonEventArgs e) {
            var s = sender as System.Windows.Controls.Image;
            if (s == null) {
                return;
            }
            _moving = false;
        }

        //private void Thumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e) {
        //    var thumb = e.Source as UIElement;
        //    Canvas.SetLeft(thumb, Canvas.GetLeft(thumb) + e.HorizontalChange);
        //    Canvas.SetTop(thumb, Canvas.GetTop(thumb) + e.VerticalChange);
        //}

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
            double enlargementRatio = Animation.GetEnlargementValue(ratio);
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
                    if (e.Key == Key.Left) {
                        _currentImagePath = _imagesInFolder.Previous(_currentImagePath);
                    } else if (e.Key == Key.Right) {
                        _currentImagePath = _imagesInFolder.Next(_currentImagePath);
                    }
                    LoadImageAndFolder(_currentImagePath);
                    //Process.Start(Application.ResourceAssembly.Location, _currentImagePath);
                    //Application.Current.Shutdown();
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
