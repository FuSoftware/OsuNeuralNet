using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OsuNeuralNet
{
    class PixelScanner
    {

        Thread t;

        List<Point> Coordinates = new List<Point>();
        Color[] Pixels;

        Queue<int> Latency = new Queue<int>();
        bool reading = false;

        bool run = false;

        int Width { get; set; }
        int Height { get; set; }

        public PixelScanner(int w, int h)
        {
            this.Width = w;
            this.Height = h;

            for(int i = 0; i < 10; i++)
            {
                Latency.Enqueue(0);
            }
        }

        public void AddCoordinates(Point p)
        {
            this.Coordinates.Add(p);
            this.Pixels = new Color[this.Coordinates.Count];
        }

        public void Start()
        {
            run = true;
            for(int i=0; i<Coordinates.Count; i++)
            {
                //t = new Thread(new ParameterizedThreadStart(QueryPixel));
                //t.Start(i);

                t = new Thread(QueryBitmap);
                t.Start();
            }
        }

        public void Stop()
        {
            run = false;
        }

        public void QueryBitmap()
        {
            Stopwatch sw = new Stopwatch();
            Bitmap b = new Bitmap(this.Width, this.Height);
            while (run)
            {
                sw.Restart();

                using (Graphics gfx = Graphics.FromImage(b))
                {
                    gfx.CopyFromScreen(0, 0, 0, 0, new Size(this.Width, this.Height), CopyPixelOperation.SourceCopy);
                }

                for (int i = 0; i < this.Coordinates.Count; i++)
                {
                    this.Pixels[i] = b.GetPixel(Coordinates[i].X, Coordinates[i].Y);
                }

                sw.Stop();
                Addlatency(sw.ElapsedMilliseconds);
            }
        }

        public void QueryPixel(object index)
        {
            IntPtr hdc = Win32.GetDC(IntPtr.Zero);

            Stopwatch sw = new Stopwatch();
            
            while (run)
            {
                sw.Restart();
                int j = (int)index;
                this.Pixels[j] = Win32.GetPixelColor(hdc, Coordinates[j].X, Coordinates[j].Y);
                sw.Stop();
                Addlatency(sw.ElapsedMilliseconds);
            }
            Win32.ReleaseDC(IntPtr.Zero, hdc);

        }

        private void Addlatency(long ms)
        {
            if (!reading)
            {
                this.Latency.Dequeue();
                this.Latency.Enqueue((int)ms);
            }
        }

        public double GetAverageLatency()
        {
            this.reading = true;
            double avg = this.Latency.Average();
            this.reading = false;
            return avg;
        }

        public Color GetPixel(int index)
        {
            Color a = this.Pixels[index];
            Color c = Color.FromArgb(0, a.R, a.G, a.B);
            return c;
        }

        public static Color GetPixel(BitmapData d, int x, int y)
        {
            int bytesForPixel = 3;

            Color c = Color.Black;

            unsafe
            {
                int x0 = x * bytesForPixel;
                int x1 = x0 + 1;
                int x2 = x1 + 1;

                byte* row = (byte*)d.Scan0 + (y * d.Stride);

                // Get color components (watch out for order!)
                byte pixelB = row[x0];
                byte pixelG = row[x1];
                byte pixelR = row[x2];

                c = Color.FromArgb(0, pixelR, pixelG, pixelB);
            }

            return c;
        }
    }
}
