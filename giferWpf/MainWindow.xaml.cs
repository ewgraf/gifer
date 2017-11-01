using gifer.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;
using System.Configuration;
using System.Drawing;
using gifer.Utils;
using System.Drawing.Imaging;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Diagnostics;
using Microsoft.VisualBasic.FileIO;
using System.Windows.Input;
using System.Threading;

namespace giferWpf {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private readonly Configuration _config;
        private GifImage _gifImage;
        private string _currentImagePath;
        private List<string> _imagesInFolder;
        private readonly OpenWithListener _openWithListener;
        private readonly DispatcherTimer _gifTimer;
        private readonly DispatcherTimer _resizeTimer;

        public MainWindow(/*Configuration config*/) {
            //_config = config;
            _openWithListener = new OpenWithListener(Gifer.EndPoint);

            InitializeComponent();
            this.Left = 0;
            this.Top = 0;
            this.Width = SystemParameters.PrimaryScreenWidth;
            this.Height = SystemParameters.PrimaryScreenHeight;
            //this.pictureBox1.Margin = new Thickness(
            //    0d,
            //    0d,
            //    this.Width  - this.pictureBox1.Width,
            //    this.Height - this.pictureBox1.Height
            //);
            //pictureBox1 = new System.Windows.Controls.Image();
            //Thumb thumb1 = new Thumb();
            //thumb1.DragDelta += new DragDeltaEventHandler(Thumb_DragDelta);
            //ControlTemplate template1 = new ControlTemplate();
            //template1.Template = new TemplateContent();
            //var fec = new FrameworkElementFactory(typeof(System.Windows.Controls.Image));
            //template1.VisualTree = fec;
            //thumb1.Template = template1;

            //SweetCanvas.Children.Add(tmbDragThumb);
            //var q = this.canvas1.Children;
            double horizontalMargin = SystemParameters.PrimaryScreenWidth / 2 - this.pictureBox1.Width / 2;
            double verticalMargin = SystemParameters.PrimaryScreenHeight / 2 - this.pictureBox1.Height / 2;
            this.pictureBox1.Margin = new Thickness(horizontalMargin, verticalMargin, horizontalMargin, verticalMargin);
            this.canv.Width = 1;
            this.canv.Height = 1;
            this.canv.Margin = new Thickness(0d, 0d,
                SystemParameters.PrimaryScreenWidth - this.canv.Width,
                SystemParameters.PrimaryScreenHeight - this.canv.Height
            );
            //this.FormBorderStyle = FormBorderStyle.None;
            this.AllowDrop = true;
            // Form.BackgroungImage flickers when updated, therefore can not be used as a thind to draw a gif on 
            // we have to use PictureBox
            //this.pictureBox1.MouseWheel += new MouseEventHandler(this.pictureBox1_MouseWheel);
            //this.pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            //this.StartPosition = FormStartPosition.CenterScreen;

            SetDefaultImage();

            //int x = (this.Width / 2) - (pictureBox1.Width / 2);
            //int y = (this.Height / 2) - (pictureBox1.Height / 2);
            //pictureBox1.Location = new Point(x, y);
            //SetupStandalone(bool.Parse(_config.AppSettings.Settings["openInStandalone"].Value));
            _gifTimer = new DispatcherTimer();
            _gifTimer.Tick += new EventHandler(timer_Tick);
            _resizeTimer = new DispatcherTimer();
            _resizeTimer.Interval = new TimeSpan(0, 0, 0, 0, 1000 / 60);
            _resizeTimer.Tick += new EventHandler(resizeTimer_Tick);
            //_timer.Interval = new TimeSpan(0, 0, 1);
            //_timer.Start();
        }

        private void timer_Tick(object sender, EventArgs e) {
            this.pictureBox1.Source = this.BitmapToImageSource(_gifImage.Next());
        }

        //public MainWindow(Configuration config, string imagePath) : this(config) {
        //    //LoadImageAndFolder(imagePath);
        //}

        private void SetDefaultImage() {
            // Empty field
            var image = new Bitmap(256, 256);
            using (Graphics g = Graphics.FromImage(image)) {
                g.FillRectangle(System.Drawing.Brushes.LightGray, 0, 0, image.Width, image.Height);
                g.DrawString("[Drag GIF/Image Here]", new Font("Courier New", 9), System.Drawing.Brushes.Black, 47, 125);
            }
            SetImage(image);
        }

        //private void SetupStandalone(bool start) {
        //    if (start && !_openWithListener.Running) {
        //        Task.Run(() => _openWithListener.Start(filePath => {
        //            try {
        //                LoadImageAndFolder(filePath);
        //            } catch (Exception ex) {
        //                MessageBox.Show(ex.ToString());
        //                Application.Exit();
        //            }
        //        }));
        //    } else if (!start && _openWithListener.Running) {
        //        _openWithListener.Stop();
        //    }
        //}

        private void LoadImageAndFolder(string imagePath) {
            if (string.IsNullOrEmpty(imagePath)) {
                return;
            }
            if (Gifer.KnownImageFormats.Any(imagePath.ToUpper().EndsWith)) {
                System.Drawing.Image image = LoadImage(imagePath);
                if (image == null) {
                    MessageBox.Show($"Can not load image: '{imagePath}'");
                }
                //if (this.InvokeRequired) {
                //    this.Invoke(new MethodInvoker(() => {
                //        SetImage(image);
                //    }));
                //} else {
                    SetImage(image);
                //}
                _currentImagePath = imagePath;
                this.Title = _currentImagePath;
                _imagesInFolder = Directory.GetFiles(System.IO.Path.GetDirectoryName(_currentImagePath))
                    .Where(path => Gifer.KnownImageFormats.Any(path.ToUpper().EndsWith))
                    .ToList();
            } else {
                MessageBox.Show($"Unknown image extension at: '{imagePath}' '{System.IO.Path.GetExtension(imagePath)}'");
            }
        }

        private System.Drawing.Image LoadImage(string imagePath) {
            try {
                switch (System.IO.Path.GetExtension(imagePath)) {
                    default:
                        return System.Drawing.Image.FromFile(imagePath);
                }
            } catch (Exception ex) {
                MessageBox.Show(ex.ToString());
                return null;
            }
        }

        private void SetImage(System.Drawing.Image image) {
            _gifTimer?.Stop();
            //timerUpdateTaskbarIcon.Stop();
            var center = new System.Windows.Point(
                this.pictureBox1.Margin.Left + this.pictureBox1.Width  / 2,
                this.pictureBox1.Margin.Top  + this.pictureBox1.Height / 2
            );
            if (image.Width > SystemParameters.PrimaryScreenWidth || image.Height > SystemParameters.PrimaryScreenHeight) {
                var size = ResizeProportionaly(image.Size,
                    new System.Drawing.Size((int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight)
                );
                this.pictureBox1.Width  = size.Width;
                this.pictureBox1.Height = size.Height;
            } else {
                this.pictureBox1.Width  = image.Width;
                this.pictureBox1.Height = image.Height;
            }
            //this.Size = pictureBox1.Size;
            double horizontalMargin = center.X - this.pictureBox1.Width  / 2;
            double verticalMargin   = center.Y - this.pictureBox1.Height / 2;
            this.pictureBox1.Margin = new Thickness(horizontalMargin, verticalMargin, horizontalMargin, verticalMargin);
            //System.Drawing.Point center =
            //    new[] { new System.Drawing.Size(
            //        (int)SystemParameters.PrimaryScreenWidth,
            //        (int)SystemParameters.PrimaryScreenHeight)
            //    .Divide(2) }
            //    .Select(s => new System.Drawing.Point(s.Width, s.Height))
            //    .Single();
            //this.pictureBox1.Margin = new Thickness(center.X - this.pictureBox1.Width / 2, center.Y - this.pictureBox1.Height / 2, 0, 0);
            //pictureBox1.Image?.Dispose();
            //pictureBox1.Image = null;
            _gifImage?.Dispose();
            GC.Collect();
            if (image.RawFormat == ImageFormat.Gif && ImageAnimator.CanAnimate(image)
                || image.RawFormat.Guid == new Guid("b96b3cb0-0728-11d3-9d7b-0000f81ef32e")) {
                _gifImage = new GifImage(image);
                pictureBox1.Source = BitmapToImageSource(_gifImage.Next());
                _gifTimer.Interval = new TimeSpan(0, 0, 0, 0, milliseconds: _gifImage.Delay);
                _gifTimer.Start();
                //timerUpdateTaskbarIcon.Start();
            } else { // if plain image
                pictureBox1.Source = BitmapToImageSource((Bitmap)image);
            }
            this.Icon = BitmapToImageSource((Bitmap)image);
        }

        BitmapImage BitmapToImageSource(Bitmap bitmap) {
            using (var stream = new MemoryStream()) {
                bitmap.Save(stream, ImageFormat.Bmp);
                stream.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = stream;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        public static System.Drawing.Size ResizeProportionaly(System.Drawing.Size size, System.Drawing.Size fitSize) {
            double ratioX = (double)fitSize.Width / (double)size.Width;
            double ratioY = (double)fitSize.Height / (double)size.Height;
            double ratio = Math.Min(ratioX, ratioY);
            return size.Multiply(ratio);
        }        

        private void pictureBox1_Drop(object sender, DragEventArgs e) {
            string imagePath = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];
            LoadImageAndFolder(imagePath);
            this.Activate();
        }

        #region Moving

        private double _x;
        private double _y;
        private bool _moving;
        private System.Windows.Point _position;

        private void pictureBox1_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.RightButton == MouseButtonState.Pressed) {
                this.Close();
            } else if (e.LeftButton != MouseButtonState.Pressed) {
                return;
            }
            _moving = true;
            this.pictureBox1.CaptureMouse();
            _position = e.GetPosition(this.pictureBox1);
            //_x = position.X;
            //_y = position.Y;
        }

        private void pictureBox1_MouseMove(object sender, System.Windows.Input.MouseEventArgs e) {
            var s = sender as System.Windows.Controls.Image;
            if (!_moving || s == null) {
                return;
            }

            //this.pictureBox1.MouseMove -= pictureBox1_MouseMove;
            /*
            //c.Top = e.Y + c.Top - _y;
            //c.Left = e.X + c.Left - _x;
            var position = e.GetPosition(this);
            var margin = this.pictureBox1.Margin;
            double xDelta = position.X - _position.X;
            double yDelta = position.Y - _position.Y;
            this.pictureBox1.Margin = new Thickness(
                margin.Left + xDelta,
                margin.Top + yDelta,
                margin.Right - xDelta,
                margin.Bottom - yDelta
            );
            Debug.WriteLine($"MouseMove xDelta: {xDelta} yDelta: {yDelta}");
            this.pictureBox1.MouseMove += pictureBox1_MouseMove;
            //this.Left = position.X + this.Left - _position.Y;
            //this.Top = position.Y + this.Top - _position.X;*/


            //var upperlimit = 0 + (s.Height / 2);
            //var lowerlimit = SystemParameters.PrimaryScreenHeight + canv.ActualHeight - (s.Height / 2);

            //var leftlimit = 0 + (s.Width / 2);
            //var rightlimit = SystemParameters.PrimaryScreenWidth + canv.ActualWidth - (s.Width / 2);




            //if ((absmouseXpos > leftlimit && absmouseXpos < rightlimit) && (absmouseYpos > upperlimit && absmouseYpos < lowerlimit)) {
            //    s.SetValue(Canvas.LeftProperty, e.GetPosition(canv).X - (s.Width / 2));
            //    s.SetValue(Canvas.TopProperty, e.GetPosition(canv).Y - (s.Height / 2));
            //}
            var position = e.GetPosition(this);
            //var absmouseXpos = e.GetPosition(this).X;
            //var absmouseYpos = e.GetPosition(this).Y;
            s.SetValue(Canvas.LeftProperty, e.GetPosition(canv).X - /*(s.Width / 2)*/  s.Margin.Left - _position.X/*+ e.GetPosition(s).X*//*- position.X*/);
            s.SetValue(Canvas.TopProperty,  e.GetPosition(canv).Y -/* (s.Height / 2)*/ s.Margin.Top - _position.Y /*+ e.GetPosition(s).Y*//*- position.Y*/);
            Debug.WriteLine($"{e.GetPosition(canv).X} {e.GetPosition(canv).Y} {position.X} {position.Y}");
        }

        private void pictureBox1_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            var s = sender as System.Windows.Controls.Image;
            if (s == null) {
                return;
            }
            _moving = false;
        }

        private void pictureBox1_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e) {
           // var s = sender as System.Windows.Controls.Image;
           // if (s == null) {
           //     return;
           // }
           //_moving = false;
        }

        #endregion

        private void Thumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e) {
            UIElement thumb = e.Source as UIElement;

            Canvas.SetLeft(thumb, Canvas.GetLeft(thumb) + e.HorizontalChange);
            Canvas.SetTop(thumb, Canvas.GetTop(thumb) + e.VerticalChange);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e) {
            switch (e.Key) {
                case Key.Left:
                case Key.Right:
                    if (e.Key == Key.Left) {
                        _currentImagePath = _imagesInFolder.Previous(_currentImagePath);
                    } else if (e.Key == Key.Right) {
                        _currentImagePath = _imagesInFolder.Next(_currentImagePath);
                    }
                    LoadImageAndFolder(_currentImagePath);
                    break;
                case Key.H:
                    //ShowHelp(_config);
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

        #region Resizing

        private Thickness _newMargin;
        private Thickness _deltaMargin;
        private System.Drawing.Size _deltaSize;
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
               // pictureBox1.Margin = _newMargin;
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
                //_resizing = false;
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
            //if (pictureBox1.Width >= SystemParameters.PrimaryScreenWidth &&
            //    pictureBox1.Height >= SystemParameters.PrimaryScreenHeight) {
            //    size = pictureBox1.Margin;
            //} else {
            //    size = form.Size;
            //}

            //System.Windows.Point location;
            //if (pictureBox1.Width >= SystemParameters.PrimaryScreenWidth &&
            //    pictureBox1.Height >= SystemParameters.PrimaryScreenHeight) {
            //    location = pictureBox.Location;
            //} else {
            //    location = form.Location;
            //}

            double enlargementRatio = Animation.GetEnlargementValue(ratio);
            //var newSize = new System.Windows.Size {
            //    Width = Convert.ToInt32(size.Width * enlargementRatio),
            //    Height = Convert.ToInt32(size.Height * enlargementRatio)
            //};
            //System.Drawing.Size widening = newSize - size;
            double delta = enlargementRatio/* / 4d*/;
            //pictureBox1.Width *= enlargementRatio;
            //pictureBox1.Height *= enlargementRatio;
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
                margin.Left - (pictureBox1.Width - _newSize.Width) / 2,
                margin.Top - (pictureBox1.Height - _newSize.Height) / 2,
                margin.Right - (pictureBox1.Width - _newSize.Width) / 2,
                margin.Bottom - (pictureBox1.Height - _newSize.Height) / 2
            );
            _deltaMargin = new Thickness(
                ((pictureBox1.Margin.Left - _newMargin.Left) / 60) * animationAcceleration,
                ((pictureBox1.Margin.Top - _newMargin.Top) / 60) * animationAcceleration,
                ((pictureBox1.Margin.Right - _newMargin.Right) / 60) * animationAcceleration,
                ((pictureBox1.Margin.Bottom - _newMargin.Bottom) / 60) * animationAcceleration
            );
            _resizeIterations = (uint)((_newSize.Width - pictureBox1.Width) / _deltaWidth);
            _resizeTimer.Start();
            //var newLocation = Point.Add(location, widening.Divide(-2));
            //   const
            // ---------- -> steps : so that when 'widening' rizes, 'steps' reduses, to resize larger window faster
            //  widening
            //
            // Ex: let widening 64 -> be steps 64, diff 128 -> steps 32
            //    c
            // -------- -> 64 steps of resizing => c = 64*64 = 4096
            //    64
            // so let it be 4096. pretty round, huh
            //int steps = 4096 / (Math.Abs(widening.Width + widening.Height) / 2);
            //int steps = 64;
            //Debug.WriteLine($"Steps: {steps}");
            //widening = widening.Divide(steps).RoundToPowerOf2();
            //Size shift = widening.Divide(2);
            //parent.Size = newSize;
            //parent.Location = newLocation;
            //int sign = Math.Sign(enlargementRatio);

            //if (this.Size.Width < Screen.PrimaryScreen.Bounds.Width &&
            //    this.Size.Height < Screen.PrimaryScreen.Bounds.Height) {
            //    form.Size += widening;
            //    form.Location -= shift;
            //} else {
            //    pictureBox.Size = newSize;
            //    pictureBox.Location = newLocation;
            //}
        }

        #endregion

    }
}

