using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Windows.Media;
using Microsoft.VisualBasic.FileIO;
using gifer;
using gifer.Domain;
using gifer.Utils;
using gifer.Languages;

namespace giferWpf {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private static readonly NaturalSortingComparer NaturalSortingComparer = new NaturalSortingComparer();
        private readonly DispatcherTimer _gifTimer = new DispatcherTimer();
        private readonly DispatcherTimer _resizeTimer = new DispatcherTimer();
        private readonly DispatcherTimer _iconTimer = new DispatcherTimer();
        private GifImage _gifImage;
        private List<string> _imagesInFolder;
        private string _currentImagePath;
        private WriteableBitmap _writableBitmap;
        private Stopwatch _drawindDelayStopwath = new Stopwatch();
        private BitmapScalingMode _scalingMode;
        private Language _language;

        public MainWindow() {
            _scalingMode = ConfigHelper.FindScalingMode() ?? BitmapScalingMode.NearestNeighbor;
            _language = ConfigHelper.FindLanguage() ?? gifer.Utils.Language.RU;

            InitializeComponent();
            OnScalingModeChanged(_scalingMode);
            OnLanguageChanged(_language);
            
            // center pictureBox
            var horizontalMargin = SystemParameters.VirtualScreenWidth  / 2 - this.pictureBox1.Width  / 2;
            var verticalMargin   = SystemParameters.VirtualScreenHeight / 2 - this.pictureBox1.Height / 2;
            this.pictureBox1.Margin = new Thickness(horizontalMargin, verticalMargin, horizontalMargin, verticalMargin);
            
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

            if (ConfigHelper.GetShowHelpAtStartup()) {
                ShowHelp();
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
                .OrderBy(p => p, NaturalSortingComparer)
                .ToList();
            
            _writableBitmap = _gifImage.GetWritableBitmap();

            this.Title = _currentImagePath;
            this.pictureBox1.Margin = ResizeImageMargin(this.pictureBox1, _writableBitmap.PixelWidth, _writableBitmap.PixelHeight);
            this.pictureBox1.Source = _writableBitmap;

            if (_gifImage.IsGif && _gifImage.Frames > 1) {
                if(_gifImage.CurrentFrameDelay == 0) {
                    //MessageBox.Show("Gif has 0 ms frame delay, that is strange");
                }
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

        private void ShowHelp() {
            bool showHelp = ConfigHelper.GetShowHelpAtStartup();
            var helpForm = new HelpWindow(showHelp, _language);
            helpForm.ShowDialog();
            ConfigHelper.SetShowHelpAtStartup(helpForm.ShowHelpAtStartup);
        }

        private void ShowSettings() {
            var settings = new SettingsWindow(
                m => this.OnScalingModeChanged(m),
                l => this.OnLanguageChanged(l),
                _scalingMode,
                _language
            );
            settings.ShowDialog();
        }

        private void OnScalingModeChanged(BitmapScalingMode scalingMode) {
            _scalingMode = scalingMode;
            RenderOptions.SetBitmapScalingMode(pictureBox1, scalingMode);
            ConfigHelper.SetScalingMode(_scalingMode);
        }

        private void OnLanguageChanged(Language language) {
            _language = language;
            ConfigHelper.SetLanguage(_language);
            if(_currentImagePath == null) { // still default image is set
                SetDefaultImage();
            }
        }
        
        private void SetDefaultImage() {
            var image = new Bitmap(256, 256);
            using (Graphics g = Graphics.FromImage(image)) {
                var rect = new Rectangle(0, 0, image.Width, image.Height);
                g.FillRectangle(System.Drawing.Brushes.LightGray, rect);
                var format = new StringFormat {
                    LineAlignment = StringAlignment.Center,
                    Alignment = StringAlignment.Center
                };                
                string message = LanguageDictionary.GetString(_language, "DefaultMessage");
                g.DrawString(message, new Font("Courier New", 9), System.Drawing.Brushes.Black, rect, format);
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
            var pictureBox = sender as System.Windows.Controls.Image; // moving whole pictureBox
#if DEBUG
            var p = e.GetPosition(this.pictureBox1);
            Debug.WriteLine($"position {p.X} {p.Y} == {this.pictureBox1.Width / p.X} {this.pictureBox1.Height / p.Y}");
#endif
            if (!_moving || pictureBox == null) {
                return;
            }
            System.Windows.Point position = e.GetPosition(this);
            pictureBox.SetValue(Canvas.LeftProperty, e.GetPosition(canvas).X - pictureBox.Margin.Left - _position.X);
            pictureBox.SetValue(Canvas.TopProperty,  e.GetPosition(canvas).Y - pictureBox.Margin.Top  - _position.Y);
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
        private int _resizeIterations;

        private void resizeTimer_Tick(object sender, EventArgs e) {
            if (_resizeIterations-->0) {
                Application.Current.Dispatcher.Invoke(
                    DispatcherPriority.Background,
                    new ThreadStart(delegate {
                        pictureBox1.Margin = new Thickness(
                            pictureBox1.Margin.Left   - _deltaMargin.Left,
                            pictureBox1.Margin.Top    - _deltaMargin.Top,
                            pictureBox1.Margin.Right  - _deltaMargin.Right,
                            pictureBox1.Margin.Bottom - _deltaMargin.Bottom
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
            Thickness originalMargin = pictureBox1.Margin;
            var originalSize = new System.Windows.Size(
                SystemParameters.VirtualScreenWidth  - (originalMargin.Left + originalMargin.Right ),
                SystemParameters.VirtualScreenHeight - (originalMargin.Top  + originalMargin.Bottom)
            );
            // [-inf, +inf] -> [0, 1]
            // 1.35 -> 1.35, -1.35 -> 0.74 (|1/ratio|)
            double enlargementRatio = AnimationHelper.GetEnlargementValue(ratio); 
            _newSize = new System.Windows.Size(
                originalSize.Width  * enlargementRatio,
                originalSize.Height * enlargementRatio
            );

            int fps = 60;
            _deltaWidth  = ((_newSize.Width  - originalSize.Width )) / fps;
            _deltaHeigth = ((_newSize.Height - originalSize.Height)) / fps;

            // tuning
            float animationAcceleration = 11.1f;
            _deltaWidth  *= animationAcceleration;
            _deltaHeigth *= animationAcceleration;

            var originalCursorPosition = e.GetPosition(this.pictureBox1);
            double xRatio = originalCursorPosition.X / originalSize.Width;
            double yRatio = originalCursorPosition.Y / originalSize.Height;
            var newCursorPosition = new System.Windows.Point(_newSize.Width * xRatio, _newSize.Height * yRatio);
#if DEBUG            
            Debug.WriteLine($"cursorPositionOnPictureBox {cursorPositionOnPictureBox.X} {cursorPositionOnPictureBox.Y}");            
            Debug.WriteLine($"newCursorPosition {newCursorPosition.X} {newCursorPosition.Y}");
#endif
            double widthDifference  = (_newSize.Width   - originalSize.Width ) / 2;
            double heightDifference = (_newSize.Height  - originalSize.Height) / 2;
            double horizontalShift  = (widthDifference  - (newCursorPosition.X - originalCursorPosition.X));
            double verticalShift    = (heightDifference - (newCursorPosition.Y - originalCursorPosition.Y));

            _newMargin = new Thickness(
                originalMargin.Left   - widthDifference  + horizontalShift,
                originalMargin.Top    - heightDifference + verticalShift,
                originalMargin.Right  - widthDifference  - horizontalShift,
                originalMargin.Bottom - heightDifference - verticalShift
            );
            _deltaMargin = new Thickness(
                ((originalMargin.Left   - _newMargin.Left  ) / fps) * animationAcceleration,
                ((originalMargin.Top    - _newMargin.Top   ) / fps) * animationAcceleration,
                ((originalMargin.Right  - _newMargin.Right ) / fps) * animationAcceleration,
                ((originalMargin.Bottom - _newMargin.Bottom) / fps) * animationAcceleration
            );
            _resizeIterations = (int)((_newSize.Width - originalSize.Width) / _deltaWidth);
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
                case Key.F1:
                case Key.H:
                    ShowHelp();
                    break;
                case Key.S:
                    ShowSettings();
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

        private void pictureBox1_MouseRightButtonUp(object s, MouseButtonEventArgs e) {
            // костыль, да, но при текущей реализации - изображение рисуется на прозрачном окне вов есь экран - надо проверять что кликнули таки на картинку
            var clickPoint = e.GetPosition(this.pictureBox1);
            if (clickPoint.X >= 0 && clickPoint.Y >= 0) { // right-clicked out of image
                this.Close();
            }
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
