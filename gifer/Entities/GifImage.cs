using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace gifer {
	public class GifImage : IDisposable {
		// http://www.onicos.com/staff/iz/formats/gif.html
		private static readonly byte[] GifHeader	 = new byte[] { 71, 73, 70 }; // GIF
		private static readonly byte[] Gif87aVersion = new byte[] { 56, 55, 97 }; // 87a
		private static readonly byte[] Gif89aVersion = new byte[] { 56, 57, 97 }; // 89a

		private readonly Bitmap _image;
		private readonly FileStream _stream;
		private readonly object share = new object();
		private readonly string _imagePath;
		private int _currentFrame = -1;
		private Rectangle _rectangle;

		public int CurrentFrameDelayMilliseconds { get; set; }
		public int Frames { get; set; }
		public bool IsGif { get; private set; }
		public GifType Type { get; private set; }
		public int Width { get => _image.Width; }
		public int Height { get => _image.Height; }
		public Size Size { get => _image.Size; }
		public Bitmap Image { get => _image; }
		public string Path { get => _imagePath; }

		[Obsolete("Лучше используйте GifImage(byte[] bytes)")]
		public GifImage(Bitmap image) {
			_image = image;
			_rectangle = new Rectangle(0, 0, _image.Width, _image.Height);
			//PropertyItem item = current_image.GetPropertyItem(0x5100); // FrameDelay in libgdiplus
			//delay = (item.Value[0] + item.Value[1] * 256) * 10; // Time is in 1/100th of a second
			Frames = _image.GetFrameCount(FrameDimension.Time);
			CurrentFrameDelayMilliseconds = BitConverter.ToInt32(image.GetPropertyItem(20736).Value, 0) * 10;
			if (CurrentFrameDelayMilliseconds == 0) {
				CurrentFrameDelayMilliseconds = 100;
			}
		}

		[Obsolete]
		public GifImage(byte[] bytes) {
			//_stream = new MemoryStream(bytes);
			_image = new Bitmap(_stream);

			_rectangle = new Rectangle(0, 0, _image.Width, _image.Height);
			byte[] signature = bytes.Take(6).ToArray();

			//if (signature.SequenceEqual(Gif89aHeader)) {
			//  Type = GifType.GIF89a;
			//  IsGif = true;
			//} else if (signature.SequenceEqual(Gif87aHeader)) {
			//  Type = GifType.GIF87a;
			//  IsGif = true;
			//} else if (signature.Take(3).SequenceEqual(GifHeader)) {
			//  Type = GifType.GIFUnknown;
			//  IsGif = true;
			//} else {
			//  Type = GifType.UnknownOrPlainImage;
			//  IsGif = false;
			//}
			Type = GifType.GIF89a;
			IsGif = true;

			if (IsGif) {
				Frames = _image.GetFrameCount(FrameDimension.Time);
				CurrentFrameDelayMilliseconds = BitConverter.ToInt32(_image.GetPropertyItem(20736).Value, 0) * 10;
				if (CurrentFrameDelayMilliseconds == 0) {
					CurrentFrameDelayMilliseconds = 100;
				}
			}
		}

		public GifImage(string imagePath) {
			//FileAttributes attributes = File.GetAttributes(_currentImagePath);
			_imagePath = imagePath;
			_stream = OpenReadFileStream(_imagePath);
			//byte[] imageBytes = new byte[_stream.Length];
			//_stream.Read(imageBytes, 0, (int)_stream.Length);
			//_stream.Close();
			//_stream.Dispose();
			// http://www.matthewflickinger.com/lab/whatsinagif/bits_and_bytes.asp
			byte[] signature = new byte[3];
			byte[] version = new byte[3];
			_stream.Read(signature, 0, 3);
			_stream.Read(version, 0, 3);
			//string s = Encoding.ASCII.GetString(signature);
			//string v = Encoding.ASCII.GetString(version);
			_stream.Seek(0, SeekOrigin.Begin);
			// Important! Bitmap(Stream s) captures stream, and all manipulations with Stream should be before "new Bitmap(stream)"
			_image = new Bitmap(_stream);
			_rectangle = new Rectangle(0, 0, _image.Width, _image.Height);
			// image.RawFormat == ImageFormat.Gif && ImageAnimator.CanAnimate(image) || image.RawFormat.Guid == new Guid("b96b3cb0-0728-11d3-9d7b-0000f81ef32e")
			if (signature.SequenceEqual(GifHeader)) {
				Frames = _image.GetFrameCount(FrameDimension.Time);
				CurrentFrameDelayMilliseconds = BitConverter.ToInt32(_image.GetPropertyItem(20736).Value, 0) * 10;
				if (CurrentFrameDelayMilliseconds == 0) {
					CurrentFrameDelayMilliseconds = 100;
				}
				IsGif = true;
				//byte[] logicalScreenDescriptor = new byte[7];
				//_stream.Read(logicalScreenDescriptor, 0, 7);
				//// TO DO: update, when screen resolutions are >65535px
				//ushort canvasWidth = (ushort)BitConverter.ToInt16(logicalScreenDescriptor, 0);
				//ushort canvasHeigth = (ushort)BitConverter.ToInt16(logicalScreenDescriptor, 2);
				if (version.SequenceEqual(Gif89aVersion)) {
					Type = GifType.GIF89a;
				} else if (signature.SequenceEqual(Gif87aVersion)) {
					Type = GifType.GIF87a;
				} else {
					Type = GifType.GIFUnknown;
				}
			} else {
				Type = GifType.UnknownOrPlainImage;
				IsGif = false;
			}
		}

		private FileStream OpenReadFileStream(string path) => new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.SequentialScan);

		public Bitmap Copy() => (Bitmap)_image.Clone();

		public Bitmap Curr() {
			_image.SelectActiveFrame(FrameDimension.Time, _currentFrame);
			// int32 is 4bytes -> shift is 4
			CurrentFrameDelayMilliseconds = BitConverter.ToInt32(_image.GetPropertyItem(20736).Value, 4 * _currentFrame) * 10;
			Debug.WriteLine($"CurrentFrameDelay: {CurrentFrameDelayMilliseconds}");
			if (CurrentFrameDelayMilliseconds == 0) {
				CurrentFrameDelayMilliseconds = 100;
			}
			if (CurrentFrameDelayMilliseconds < 0) {
				throw new InvalidOperationException($"CurrentFrameDelay {CurrentFrameDelayMilliseconds} <= 0. Is int32 4 bytes length?");
			}

			return System.Drawing.Image.FromHbitmap(new Bitmap(_image).GetHbitmap());
		}

		public void Next() {
			if (_currentFrame >= Frames - 1 || _currentFrame < 0) {
				_currentFrame = -1;
			}
			_currentFrame++;
		}

		public void Prev() {
			if (_currentFrame >= Frames || _currentFrame < 1) {
				_currentFrame = Frames;
			}
			_currentFrame--;
		}

		public void Dispose() {
			_image?.Dispose();
			_stream?.Close();
			_stream?.Dispose();
		}
	}
}
