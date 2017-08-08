using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Office.Interop.Excel;

namespace OsuNeuralNet
{
    public partial class TaikoBotForm : Form
    {
        TaikoBot Bot;
        public TaikoBotForm()
        {
            InitializeComponent();
            this.TopMost = true;

            pBG.Maximum = 255;
            pBB.Maximum = 255;
            pBR.Maximum = 255;

            Bot = new TaikoBot();

            Bot.PanelColorUpdated += UpdatePanelColor;
            Bot.PanelLastKeyUpdated += UpdateLastKey;
            Bot.TaikoStateUpdated += UpdateTaikoState;
        }

        public void UpdatePanelColor(object sender, EventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                UpdatePanelColor(((TaikoBot.PanelColorUpdatedEventArgs)e).c);
            });
        }

        public void UpdateLastKey(object sender, EventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                panelLastKey.BackColor = ((TaikoBot.PanelLastKeyUpdatedEventArgs)e).c;
            });
        }

        public void UpdateTaikoState(object sender, EventArgs e)
        {
            if(this.IsHandleCreated && LabelRunning.Text != ((TaikoBot.TaikoStateUpdatedEventArgs)e).State)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    LabelRunning.Text = ((TaikoBot.TaikoStateUpdatedEventArgs)e).State;
                });
            }
            
        }


        private void button1_Click(object sender, EventArgs e)
        {
            Bot.ExportLatency();
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
