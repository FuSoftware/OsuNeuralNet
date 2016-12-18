using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OsuNeuralNet
{
    class VideoProcessing
    {
        public static Bitmap getBitmap(int x, int y, int w, int h)
        {
            //Create a new bitmap.
            var bmpScreenshot = new Bitmap(w,
                                           h,
                                           PixelFormat.Format32bppArgb);

            // Create a graphics object from the bitmap.
            var gfxScreenshot = Graphics.FromImage(bmpScreenshot);

            // Take the screenshot from the upper left corner to the right bottom corner.
            gfxScreenshot.CopyFromScreen(x,
                                            y,
                                            0,
                                            0,
                                            Screen.PrimaryScreen.Bounds.Size,
                                            CopyPixelOperation.SourceCopy);
            return bmpScreenshot;
        }

        public static List<Color> getColors(Bitmap bmp)
        {
            List<Color> c = new List<Color>();

            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    c.Add(bmp.GetPixel(i, j));
                }
            }

            return c;
        }

        public static List<Color> getColors(int x, int y, int w, int h)
        {
            return getColors( getBitmap(x, y, w, h));
        }

        public static Bitmap ResizeBitmap(int width, int height, Bitmap image)
        {
            // uncomment for higher quality output
            //graph.InterpolationMode = InterpolationMode.High;
            //graph.CompositingQuality = CompositingQuality.HighQuality;
            //graph.SmoothingMode = SmoothingMode.AntiAlias;

            float scale = Math.Min(width / image.Width, height / image.Height);
            var brush = new SolidBrush(Color.Black);

            var bmp = new Bitmap((int)width, (int)height);
            var graph = Graphics.FromImage(bmp);

            var scaleWidth = (int)(image.Width * scale);
            var scaleHeight = (int)(image.Height * scale);

            graph.FillRectangle(brush, new RectangleF(0, 0, width, height));
            graph.DrawImage(image, new Rectangle(((int)width - scaleWidth) / 2, ((int)height - scaleHeight) / 2, scaleWidth, scaleHeight));

            return bmp;
        }

        public static List<double> GetInputsFromColor(List<Color> colors)
        {
            List<double> d = new List<double>();

            foreach(Color c in colors)
            {
                d.Add((double)c.R / 255.0);
                d.Add((double)c.G / 255.0);
                d.Add((double)c.B / 255.0);
            }

            return d;
        }
    }
}
