using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OsuNeuralNet
{
    class VideoProcessing
    {
        public class ThreadInfo
        {
            public int Index { get; set; }
        }

        public Color[] Colors;
        int[][] pixels;

        bool Run = false;
        bool isLocked = false;

        public struct RGB
        {
            public int R, G, B;
            public RGB(Color c)
            {
                this.R = c.R;
                this.G = c.G;
                this.B = c.B;
            }

            public RGB(int R, int G, int B)
            {
                this.R = R;
                this.G = G;
                this.B = B;
            }
        }

        public VideoProcessing()
        {
            
        }

        public void Start(int[][] pixels)
        {
            if (pixels == null)
                throw new ArgumentException("Empty pixel array", "pixels");

            this.pixels = pixels;
            Run = true;

            Colors = new Color[pixels.Length];

            for(int i=0;i<Colors.Length;i++)
            {
                Thread t = new Thread(Process);
                ThreadInfo ti = new ThreadInfo();
                ti.Index = i;
                t.Start(ti);
            }          
        }

        public void Process(object a)
        {
            ThreadInfo t = (ThreadInfo)a;
            int index = t.Index;
            while (Run)
            {
                if (!isLocked)
                {
                    IntPtr hdc = Win32.GetDC(IntPtr.Zero);
                    Colors[index] = Win32.GetPixelColor(this.pixels[index][0], this.pixels[index][1]);
                    Win32.ReleaseDC(IntPtr.Zero, hdc);
                }
                Thread.Sleep(1);
            }
        }

        public void Stop()
        {
            Run = false;
        }

        public void Lock()
        {
            isLocked = true;
        }

        public void UnLock()
        {
            isLocked = false;
        }

        public List<RGB> GetPixels()
        {
            Lock();
            List<RGB> c = new List<RGB>();
            for (int i=0;i< Colors.Length;i++)
            {
                c.Add(new RGB(Colors[i]));
            }
            UnLock();

            return c;
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
