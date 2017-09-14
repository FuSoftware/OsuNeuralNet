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
        bool UpdatePanels = true;

        PixelScanner scanner = new PixelScanner(1920,1080);

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

        //Pixels
        Point pScan = new Point(390,412);
        Point pScanBis = new Point(380, 412);


        Point pSpinner = new Point(0,0);
        Point pCheck = new Point(1880, 574);

        Point pSlow0 = new Point(925, 1058);
        Point pSlow1 = new Point(900, 1058);

        Point pFast0 = new Point(1000, 1058);
        Point pFast1 = new Point(1025, 1058);

        /* Code */
        public TaikoBot()
        {
            //Init Video Processing
            this.scanner.AddCoordinates(pScan);
            this.scanner.AddCoordinates(pScanBis);
            this.scanner.AddCoordinates(pCheck);


            this.scanner.Start();

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
            Thread myThread;
            myThread = new Thread(new ThreadStart(GetPixelColor));
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
                
                Color c = scanner.GetPixel(0);
                Color cb = scanner.GetPixel(1);
                Color cCheck = scanner.GetPixel(2);
                Color cSpinner = Color.White;

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
                    TaikoState.State = "Taiko Running (" + Math.Round(scanner.GetAverageLatency(),2) + "ms)";
                    OnTaikoStateUpdated(TaikoState);

                    if (isSpinner(c))
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
                    else if (isRed(c) || isRed(cb))
                    {
                        if (lastInput != Inputs.INPUT_R)
                        {
                            SendRed();
                        }
                        CountNote(swLoop.ElapsedMilliseconds);
                    }
                    else if (isBlue(c) || isBlue(cb))
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

        public bool isBlue(Color c)
        {
            return (c.B > 120 && c.B > c.R);
        }

        public bool isRed(Color c)
        {
            return (c.R > 120 && c.R > c.B);
        }

        public bool isSpinner(Color c)
        {
            return (c.R == 146 && c.G == 107 && c.B == 3);
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
