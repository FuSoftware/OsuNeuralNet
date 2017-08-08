using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace OsuNeuralNet
{
    public partial class ManiaBot : Form
    {
        int _RunningThreads = 0;
        bool[] isKeyPressed;
        bool[] isKeyPressing;
        long[] lastPress;
        Stopwatch timer;

        bool _running = false;

        public ManiaBot()
        {
            InitializeComponent();
            StartThread();
            this.TopMost = true;
        }

        public void StartThread()
        {

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

        public bool isManiaRunning()
        {
            //Color c = Win32.GetPixelColor(250, 250);
            Color c = Win32.GetPixelColor(595, 1024);

            //Console.WriteLine(c.R + " " + c.G + " " + c.B);

            return (c.R > 250 && c.G > 250 && c.B > 250);
        }

        public void ProcessPixel(int x, int y, int r, int g, int b, int key, int delay)
        {
            int t = 0;
            while(Thread.CurrentThread.IsAlive)
            {
                if(t < 5)
                    Thread.Sleep(5 - t);

                if (_running)
                {
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    Color c = Win32.GetPixelColor(x, y);
                    

                    //Key
                    if(c.R < 250 && c.G < 250 && c.B < 250)
                        SendKeyToggle(key, (c.R > r || c.G > g || c.B > b), delay);
                    
                    
                    Invoke((MethodInvoker)delegate
                    {
                        switch (key)
                        {
                            case 0:
                                panelD.BackColor = c;
                                labelD.Text = sw.ElapsedMilliseconds + "ms";
                                break;
                            case 1:
                                panelF.BackColor = c;
                                labelF.Text = sw.ElapsedMilliseconds + "ms";
                                break;
                            case 2:
                                panelJ.BackColor = c;
                                labelJ.Text = sw.ElapsedMilliseconds + "ms";
                                break;
                            case 3:
                                panelK.BackColor = c;
                                labelK.Text = sw.ElapsedMilliseconds + "ms";
                                break;
                        }
                    });

                    sw.Stop();

                    t = (int)sw.ElapsedMilliseconds;
                }
                else
                {
                    Thread.Sleep(100);
                }
            }

            Console.WriteLine("Thread for key " + key + " crashed");
        }

        public void GetPixelColor()
        {
            int l = 10;
            int d = 725; //ms delay

            isKeyPressed = new bool[4] { false, false, false, false };
            isKeyPressing = new bool[4] { false, false, false, false };
            lastPress = new long[4];

            timer = new Stopwatch();
            timer.Start();

            // Threads 1600*900
            /*
            Thread t1 = new Thread(new ThreadStart((MethodInvoker)delegate { ProcessPixel(280, 50, l, l, l, 0, d); }));
            Thread t2 = new Thread(new ThreadStart((MethodInvoker)delegate { ProcessPixel(335, 50, l, 0, l, 1, d); }));
            Thread t3 = new Thread(new ThreadStart((MethodInvoker)delegate { ProcessPixel(390, 50, l, 0, l, 2, d); }));
            Thread t4 = new Thread(new ThreadStart((MethodInvoker)delegate { ProcessPixel(445, 50, l, l, l, 3, d); }));
            */

            Thread t1 = new Thread(new ThreadStart((MethodInvoker)delegate { ProcessPixel(320, 50, l, l, l, 0, d); }));
            Thread t2 = new Thread(new ThreadStart((MethodInvoker)delegate { ProcessPixel(390, 50, l, l, l, 1, d); }));
            Thread t3 = new Thread(new ThreadStart((MethodInvoker)delegate { ProcessPixel(460, 50, l, l, l, 2, d); }));
            Thread t4 = new Thread(new ThreadStart((MethodInvoker)delegate { ProcessPixel(530, 50, l, l, l, 3, d); }));

            t1.Start();
            t2.Start();
            t3.Start();
            t4.Start();

            // UI
            while (Thread.CurrentThread.IsAlive)
            {
                Thread.Sleep(100);
                _running = isManiaRunning();
                if (_running)
                {
                    this.Invoke((MethodInvoker)delegate {
                        if (this.labelRunning.Text != "Mania Running")
                            this.labelRunning.Text = "Mania Running";
                    });
                }
                else
                {
                    this.Invoke((MethodInvoker)delegate {
                        if(this.labelRunning.Text != "Mania Idle")
                            this.labelRunning.Text = "Mania Idle";
                    });
                }
            }
        }

        public void ToggleKey(Keys key, bool isPressed)
        {
            
            Win32.PressKey(key, !isPressed);

            Color c;
            if(isPressed)
            {
                c = Color.Green;
            }
            else
            {
                c = Color.Red;
            }

            /*
            Invoke((MethodInvoker)delegate
            {
                switch (key)
                {
                    case Keys.D:
                        panelDKey.BackColor = c;
                        break;
                    case Keys.F:
                        panelFKey.BackColor = c;
                        break;
                    case Keys.J:
                        panelJKey.BackColor = c;
                        break;
                    case Keys.K:
                        panelKKey.BackColor = c;
                        break;
                }
            });
            */
        }

        private void SendKeyToggle(int line, bool isPressed, int delay = 0)
        {
            Keys k = Keys.D;

            switch (line)
            {
                case 0:
                    k = Keys.D;
                    break;
                case 1:
                    k = Keys.F;
                    break;
                case 2:
                    k = Keys.J;
                    break;
                case 3:
                    k = Keys.K;
                    break;
            }


            if(isKeyPressed[line] != isPressed)
            {
                Thread t = new Thread(new ThreadStart(
                    (MethodInvoker)delegate
                    {
                        isKeyPressed[line] = isPressed;
                        Thread.Sleep(delay);

                        if (timer.ElapsedMilliseconds - lastPress[line] < 20)
                            Thread.Sleep(60 - (int)(timer.ElapsedMilliseconds - lastPress[line]));

                        ToggleKey(k, isPressed);
                        lastPress[line] = timer.ElapsedMilliseconds;
                    }));
                t.Start();
                
            }
        }
    }
}