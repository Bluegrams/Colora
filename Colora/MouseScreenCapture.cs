using System;
using System.Windows.Threading;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Forms;

using Point = System.Windows.Point;
using Color = System.Windows.Media.Color;
using System.Drawing.Drawing2D;

namespace Colora
{
    class MouseScreenCapture
    {
        private DispatcherTimer timer;

        public event EventHandler CaptureTick;
        /// <summary>
        /// The real pixel size captured which will be zoomed to 100px.
        /// </summary>
        public int RealCaptureSize { get; set; }
        public Point MouseScreenPosition { get { return new Point(Control.MousePosition.X, Control.MousePosition.Y); } }
        public Bitmap CaptureBitmap { get; private set; }
        public BitmapImage CaptureBitmapImage
        {
            get
            {
                BitmapImage bmpImage = new BitmapImage();
                using (MemoryStream memory = new MemoryStream())
                {
                    CaptureBitmap.Save(memory, ImageFormat.Bmp);
                    memory.Position = 0;
                    bmpImage.BeginInit();
                    bmpImage.StreamSource = memory;
                    bmpImage.CacheOption = BitmapCacheOption.OnLoad;
                    bmpImage.EndInit();
                }
                return bmpImage;
            }
        }
        public Color PointerPixelColor { get; private set; }

        public MouseScreenCapture()
        {
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 50);
            timer.Tick += new EventHandler(timer_Tick);
            RealCaptureSize = 33;
        }

        public void StartCapturing()
        {
            timer.Start();
        }

        public void StopCapturing()
        {
            timer.Stop();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            int sz = RealCaptureSize;
            int shalf = (int)Math.Floor(RealCaptureSize / (double)2);
            Bitmap screen = new Bitmap(sz, sz);
            using (Graphics g = Graphics.FromImage(screen))
            {
                g.CopyFromScreen(Control.MousePosition.X - shalf, Control.MousePosition.Y - shalf, 0, 0, new System.Drawing.Size(sz, sz));
            }
            Bitmap map = new Bitmap(100, 100);
            using (Graphics g = Graphics.FromImage(map))
            {
                g.PixelOffsetMode = PixelOffsetMode.Half;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.DrawImage(screen, new Rectangle(0, 0, 100, 100), new Rectangle(0, 0, sz, sz), GraphicsUnit.Pixel);
            }
            CaptureBitmap = map;
            System.Drawing.Color drawing_Col = screen.GetPixel(sz / 2, sz / 2);
            PointerPixelColor = Color.FromRgb(drawing_Col.R, drawing_Col.G, drawing_Col.B);
            screen.Dispose();
            if (CaptureTick != null)
                CaptureTick(this, new EventArgs());
        }
    }
}
