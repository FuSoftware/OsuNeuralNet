using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OsuNeuralNet
{
    public partial class Performance : Form
    {
        VideoProcessing vp;

        public Performance()
        {
            InitializeComponent();
            this.TopMost = true;

            // Déclaration du thread
            Thread myThread;

            myThread = new Thread(new ThreadStart(RunPerformance));

            // Lancement du thread
            myThread.Start();
        }

        private void RunPerformance()
        {
            /*
            vp = new VideoProcessing();
            vp.StartTaikoGeneration(); 
            
            while (Thread.CurrentThread.IsAlive)
            {
                PerformanceCopyFromScreen();
                Thread.Sleep(1000);
            }
            */
            
        }

        private void PerformangeGetPixelAt()
        {
            int x = (int)numericUpDownX.Value;
            int y = (int)numericUpDownY.Value;
            int w = (int)numericUpDownW.Value;
            int h = (int)numericUpDownH.Value;

            Stopwatch watch = new Stopwatch();

            watch.Start();
            Win32.getColors(x, y, h, w);

            LabelTime.Text = "Processed " + w * h + " pixels in " + watch.ElapsedMilliseconds + " ms";
            watch.Stop();
        }
    }
}
