using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static OsuNeuralNet.VideoProcessing;

namespace OsuNeuralNet
{
    class TaikoBot
    {
        class Latency
        {
            public Latency(long Scan, long Process, long Total)
            {
                this.Scan = Scan;
                this.Process = Process;
                this.Total = Total;
            }

            public long Scan { get; set; }
            public long Process { get; set; }
            public long Total { get; set; }
        }
        List<Latency> LatencyList = new List<Latency>();
        List<long> NoteList = new List<long>();
        long lastTime = 0;
        bool UpdatePanels = false;

        VideoProcessing v;

        Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();

        enum Inputs
        {
            INPUT_R = 0,
            INPUT_B,
            INPUT_NONE
        }

        private Inputs lastInput;
        private Keys lastKey;

        /* Events */
        public event EventHandler TaikoStateUpdated;
        public virtual void OnTaikoStateUpdated(TaikoStateUpdatedEventArgs e)
        {
            TaikoStateUpdated?.Invoke(this, e);
        }
        public class TaikoStateUpdatedEventArgs : EventArgs
        {
            public string State { get; set; }
        }

        public event EventHandler PanelColorUpdated;
        public virtual void OnPanelColorUpdated(PanelColorUpdatedEventArgs e)
        {
            PanelColorUpdated?.Invoke(this, e);
        }
        public class PanelColorUpdatedEventArgs : EventArgs
        {
            public Color c { get; set; }
        }

        public event EventHandler PanelLastKeyUpdated;
        public virtual void OnPanelLastKeyUpdated(PanelLastKeyUpdatedEventArgs e)
        {
            PanelLastKeyUpdated?.Invoke(this, e);
        }
        public class PanelLastKeyUpdatedEventArgs : EventArgs
        {
            public Color c { get; set; }
        }

        /* Code */
        public TaikoBot()
        {
            v = new VideoProcessing();
            StartThread();
            lastInput = Inputs.INPUT_NONE;
        }

        public void ExportLatency()
        {
            string csv = "Total,Process,Scan\n";

            for (int i = 0; i < LatencyList.Count; i++)
            {
                csv += LatencyList[i].Total + "," + LatencyList[i].Process + "," + LatencyList[i].Scan + "\n";
            }

            string file = Path.GetTempPath() + "taikobot_" + (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds + "_" + LatencyList.Count + ".csv";

            LatencyList.Clear();

            File.Delete(file);
            File.WriteAllText(file, csv);

            excel.Workbooks.Add(file);
            excel.Visible = true;
        }

        public void StartThread()
        {
            /*
            int[][] pixels = new int[2][];
            pixels[0] = new int[2] { 390,412 };
            pixels[1] = new int[2] { 1880,574 };
            v.Start(pixels);
            */

            // Déclaration du thread
            Thread myThread;

            // Instanciation du thread, on spécifie dans le 
            // délégué ThreadStart le nom de la méthode qui
            // sera exécutée lorsque l'on appele la méthode
            // Start() de notre thread.
            myThread = new Thread(new ThreadStart(GetPixelColor));

            // Lancement du thread
            myThread.Start();
        }

        public long NoteFrequency()
        {
            if (NoteList.Count > 0)
            {
                long res = (long)NoteList.Average();

                if (NoteList.Count > 50)
                {
                    NoteList.RemoveRange(0, NoteList.Count - 50);
                }

                return res;
            }
            return 0;
        }

        public void CountNote(long time)
        {
            
            if(NoteList.Count > 0 )
            {
                NoteList.Add(time - lastTime);
            }
            else
            {
                NoteList.Add(0);
            }

            lastTime = time;
        }

        public void GetPixelColor()
        {
            Stopwatch swScan = new Stopwatch();
            Stopwatch swProcess = new Stopwatch();
            Stopwatch swLoop = new Stopwatch();

            swLoop.Start();

            while (Thread.CurrentThread.IsAlive)
            {
                //LatencyList.Add(new Latency(swScan.ElapsedMilliseconds, swProcess.ElapsedMilliseconds, swLoop.ElapsedMilliseconds));
                //Thread.Sleep(1);

                //1280*1024
                /*
                Color c = Win32.GetPixelColor(-390, -412);
                Color cCheck = Win32.GetPixelColor(1880, 574);
                Color cSpinner = Color.White;//Win32.GetPixelColor(500, 600);
                */


                //1600*900
                /*
                Color c = Win32.GetPixelColor(300, 358);
                Color cCheck = Win32.GetPixelColor(194, 476);
                Color cSpinner = Win32.GetPixelColor(500, 600);
                */


                //1920*1080
                
                IntPtr hdc = Win32.GetDC(IntPtr.Zero);
                Color c = Win32.GetPixelColor(hdc, 390, 412);
                Color cCheck = Win32.GetPixelColor(hdc, 1880, 574);
                Color cSpinner = Color.White;//Win32.GetPixelColor(500, 600);
                Win32.ReleaseDC(IntPtr.Zero, hdc);
                

                //800*600 Top Left
                //Color c = Win32.GetPixelColor(200, 265);
                //Color cCheck = Win32.GetPixelColor(147, 343);
                /*
                List<RGB> Pixels = v.GetPixels();
                RGB c;
                RGB cCheck;
                */


                if (UpdatePanels)
                {
                    PanelColorUpdatedEventArgs PanelColorArgs = new PanelColorUpdatedEventArgs();
                    PanelColorArgs.c = Color.FromArgb(0, c.R, c.G, c.B);
                    OnPanelColorUpdated(PanelColorArgs);
                }

                if (cCheck.R == 0 && cCheck.G == 0 && cCheck.B == 0)
                {
                    //Update the label
                    TaikoStateUpdatedEventArgs TaikoState = new TaikoStateUpdatedEventArgs();
                    TaikoState.State = "Taiko Running";
                    OnTaikoStateUpdated(TaikoState);

                    if ((c.R == 146 && c.G == 107 && c.B == 3))
                    {
                        if (lastInput == Inputs.INPUT_R)
                        {
                            SendBlue();
                        }
                        else if (lastInput == Inputs.INPUT_B)
                        {
                            SendRed();
                        }
                        else
                        {
                            SendRed();
                            SendBlue();
                        }
                        CountNote(swLoop.ElapsedMilliseconds);
                    }
                    else if (c.R > 120 && c.R > c.B)
                    {
                        if (lastInput != Inputs.INPUT_R)
                        {
                            SendRed();
                        }
                        CountNote(swLoop.ElapsedMilliseconds);
                    }
                    else if (c.B > 120 && c.B > c.R)
                    {
                        if (lastInput != Inputs.INPUT_B)
                        {
                            SendBlue();
                        }
                        CountNote(swLoop.ElapsedMilliseconds);
                    }
                    else
                    {
                        lastInput = Inputs.INPUT_NONE;
                    }
                }
                else
                {

                    //Update the label
                    TaikoStateUpdatedEventArgs TaikoState = new TaikoStateUpdatedEventArgs();
                    TaikoState.State = "Taiko Not Running";
                    OnTaikoStateUpdated(TaikoState);
                }

                Thread.Sleep(1);
            }
        }

        public void SendRed()
        {
            lastInput = Inputs.INPUT_R;

            if (lastKey == Keys.D)
            {
                PressKey(Keys.L);
            }
            else
            {
                PressKey(Keys.D);
            }

            if (UpdatePanels)
                UpdateKeyColor(Color.IndianRed);
        }

        public void SendBlue()
        {
            lastInput = Inputs.INPUT_B;

            if (lastKey == Keys.S)
            {
                PressKey(Keys.M);
            }
            else
            {
                PressKey(Keys.S);
            }

            if (UpdatePanels)
                UpdateKeyColor(Color.LightBlue);
        }

        public void PressKey(Keys key)
        {
            Win32.PressKey(key, false);
            lastKey = key;

            Thread t = new Thread(new ThreadStart((MethodInvoker)delegate
            {
                Thread.Sleep(5);
                Win32.PressKey(key, true);
            }));
            t.Start();

        }

        public void SendNone()
        {
            if(UpdatePanels)
                UpdateKeyColor(Color.Gray);
        }

        public void UpdateKeyColor(Color c)
        {
            PanelLastKeyUpdatedEventArgs PanelColorArgs = new PanelLastKeyUpdatedEventArgs();
            PanelColorArgs.c = c;
            OnPanelLastKeyUpdated(PanelColorArgs);
        }

    }
}
