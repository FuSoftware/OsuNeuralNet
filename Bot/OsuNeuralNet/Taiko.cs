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
    public partial class Taiko : Form
    {
        enum Inputs
        {
            INPUT_R = 0,
            INPUT_B,
            INPUT_NONE
        }

        private Inputs lastInput;
        private Keys lastKey;

        public Taiko()
        {
            InitializeComponent();
            StartThread();
            this.TopMost = true;
            lastInput = Inputs.INPUT_NONE;
        }

        public void StartThread()
        {
            pBG.Maximum = 255;
            pBB.Maximum = 255;
            pBR.Maximum = 255;

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

        public void GetPixelColor()
        {
            while (Thread.CurrentThread.IsAlive)
            {
                Thread.Sleep(1);


                //Fullscreen
                Color c = Win32.GetPixelColor(300, 358);
                Color cCheck = Win32.GetPixelColor(194, 476);
                Color cSpinner = Win32.GetPixelColor(500, 600);

                //800*600 Top Left
                //Color c = Win32.GetPixelColor(200, 265);
                //Color cCheck = Win32.GetPixelColor(147, 343);

                /*
                this.Invoke((MethodInvoker)delegate {
                    UpdatePanelColor(c); // runs on UI thread
                });
                */
                

                if (cCheck.R == 0 && cCheck.G == 0 && cCheck.B == 0)
                {
                    //Update the label
                    this.Invoke((MethodInvoker)delegate {
                        this.LabelRunning.Text = "Taiko Running";
                    });

                    if((c.R == 146 && c.G == 107 && c.B == 3) || (cSpinner.R > 250 && cSpinner.B < 25 && cSpinner.G < 50))
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
                    }
                    else if(c.R > 128 && c.R > c.B)
                    {
                        if (lastInput != Inputs.INPUT_R)
                        {
                            SendRed();
                        }
                    }
                    else if (c.B > 128 && c.B > c.R)
                    {
                        if(lastInput != Inputs.INPUT_B)
                        {
                            SendBlue();
                        }
                    }
                    else
                    {
                        lastInput = Inputs.INPUT_NONE;
                        //Thread.Sleep(10);
                        this.Invoke((MethodInvoker)delegate {
                            this.panelLastKey.BackColor = Color.Gray;
                        });
                    }
                }   
                else
                {
                    //Update the label
                    this.Invoke((MethodInvoker)delegate {
                        this.LabelRunning.Text = "Taiko not Running";
                    });
                }
            }
        }

        public void SendRed()
        {
            lastInput = Inputs.INPUT_R;
            //SendKeys.SendWait("D");

            if(lastKey == Keys.D)
            {
                PressKey(Keys.L);
            }
            else
            {
                PressKey(Keys.D);
            }

            this.Invoke((MethodInvoker)delegate {
                this.panelLastKey.BackColor = Color.IndianRed;
            });
        }

        public void SendBlue()
        {
            lastInput = Inputs.INPUT_B;
            //SendKeys.SendWait("S");

            if (lastKey == Keys.S)
            {
                PressKey(Keys.M);
            }
            else
            {
                PressKey(Keys.S);
            }

            this.Invoke((MethodInvoker)delegate {
                this.panelLastKey.BackColor = Color.LightBlue;
            });
        }

        public void PressKey(Keys key)
        {
            Win32.PressKey(key, false);
            Thread.Sleep(20);
            Win32.PressKey(key, true);
            lastKey = key;
        }

        public void SendNone()
        {
            lastInput = Inputs.INPUT_NONE;
            this.Invoke((MethodInvoker)delegate {
                this.panelLastKey.BackColor = Color.Gray;
            });
        }

        public void UpdatePanelColor(Color c)
        {
            this.MainPanel.BackColor = c;
            this.labelB.Text = "B " + c.B;
            this.labelG.Text = "G " + c.G;
            this.labelR.Text = "R " + c.R;
            this.pBG.Value = c.G;
            this.pBB.Value = c.B;
            this.pBR.Value = c.R;
        }
    }
}
