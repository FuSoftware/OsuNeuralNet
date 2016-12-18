using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OsuNeuralNet
{
    public partial class Mania : Form
    {
        private long offset = 0; //ms between seeing the note and pressing
        Stopwatch swDelay;

        public Mania()
        {
            InitializeComponent();
            this.TopMost = true;

            Initialize();
        }

        public void Initialize()
        {
            //Y 900P
            int y = 356;

            //4K 900P
            int[] keys = new int[4] { 280, 340, 395, 455 };
            Color[] c = new Color[4];

            for(int i=0;i<4;i++)
            {
                c[i] = Win32.GetPixelColor(keys[i], y);
            }


            Thread threadDelay;
            threadDelay = new Thread(new ThreadStart(InitializeOffset));
            threadDelay.Start();
        }

        public void InitializeOffset()
        {
            swDelay = new Stopwatch();
            swDelay.Start();

            while(Thread.CurrentThread.IsAlive)
            {
                //900P
                Color c = Win32.GetPixelColor(366, 356);
                Color cCheck = Win32.GetPixelColor(500,620);

                if (cCheck.R == 255 && cCheck.G == 255 && cCheck.B == 255)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        this.LabelRunning.Text = "Mania Running";
                    });

                    if (c.R >250  && c.G > 250 && c.B > 250)
                    {
                        if (!swDelay.IsRunning)
                        {
                            swDelay.Start();
                            this.Invoke((MethodInvoker)delegate
                            {
                                this.LabelDelay.Text = "Calculating delay";
                            });
                        }
                        else
                        {
                            offset = swDelay.ElapsedMilliseconds - offset;
                            this.Invoke((MethodInvoker)delegate
                            {
                                this.LabelDelay.Text = "Delay " + offset + " ms";
                            });
                        }

                    }
                }
                else
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        this.LabelRunning.Text = "Mania not running (" + cCheck.R + "," + cCheck.G + "," + cCheck.B + ")";
                    });
                }
            }
        }
    }
}
