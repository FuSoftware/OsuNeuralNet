using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OsuNeuralNet
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new TaikoBotForm());
            


            //GraphicsBenchmark();
            //TestLockBits();
        }

        public static void GraphicsBenchmark()
        {
            int l = 100;
            Stopwatch sw = new Stopwatch();

            sw.Start();
            IntPtr hdc = Win32.GetDC(IntPtr.Zero);
            for (int i = 0; i < l; i++)
            {
                Win32.GetPixelColor(hdc, 500, 500);
            }
            sw.Stop();
            Console.WriteLine("GetPixelColor : ");
            Console.WriteLine("{0} operation in {1} ms = {2} ms/op", l, sw.ElapsedMilliseconds, sw.ElapsedMilliseconds / l);

            sw.Restart();
            for (int i = 0; i < l; i++)
            {
                Win32.GetPixelColorShort(hdc, 500, 500);
            }
            Win32.ReleaseDC(IntPtr.Zero, hdc);
            sw.Stop();
            Console.WriteLine("GetPixelColorShort : ");
            Console.WriteLine("{0} operation in {1} ms = {2} ms/op", l, sw.ElapsedMilliseconds, sw.ElapsedMilliseconds / l);

            sw.Restart();
            Bitmap b = new Bitmap(1,1, PixelFormat.Format32bppArgb);
            for (int i = 0; i < l; i++)
            {
                using (Graphics gfx = Graphics.FromImage(b))
                {
                    gfx.CopyFromScreen(500, 500, 0, 0, new Size(1, 1), CopyPixelOperation.SourceCopy);
                }

                Color c = b.GetPixel(0, 0);
            }
            sw.Stop();
            Console.WriteLine("GFX : ");
            Console.WriteLine("{0} operation in {1} ms = {2} ms/op", l, sw.ElapsedMilliseconds, sw.ElapsedMilliseconds / l);

            sw.Restart();
            for (int i = 0; i < l; i++)
            {
                Color c = Win32.GetColorAt(500, 500);
            }
            sw.Stop();
            Console.WriteLine("BitBlt : ");
            Console.WriteLine("{0} operation in {1} ms = {2} ms/op", l, sw.ElapsedMilliseconds, sw.ElapsedMilliseconds / l);



            Console.ReadLine();
        }

        public static void TestLockBits()
        {
            Bitmap b = new Bitmap(1920, 1080);
            using (Graphics gfx = Graphics.FromImage(b))
            {
                gfx.CopyFromScreen(0, 0, 0, 0, new Size(1920, 1080), CopyPixelOperation.SourceCopy);
            }

            BitmapData d = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

            Color c = PixelScanner.GetPixel(d, 500, 500);
            

            b.UnlockBits(d);

            Console.Read();
        }
    }

    
}
