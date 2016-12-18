using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OsuNeuralNet
{
    public partial class Performance : Form
    {
        public Performance()
        {
            InitializeComponent();
        }

        private void Calculate_Click(object sender, EventArgs e)
        {
            PerformanceCopyFromScreen();
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

        private void PerformanceCopyFromScreen()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            Bitmap bmp = VideoProcessing.getBitmap(208, 264, 1600 - 208, 470 - 264);
            bmp = new Bitmap(bmp, new Size(200,100));

            List<Color> colors = VideoProcessing.getColors(bmp);
            List<double> inputs = VideoProcessing.GetInputsFromColor(colors);

            watch.Stop();
            LabelTime.Text = "Processed " + inputs.Count + " inputs from " + colors.Count + " colors in " + watch.ElapsedMilliseconds + " ms";
            PanelSubscreen.BackgroundImage = bmp;
        }
    }
}
