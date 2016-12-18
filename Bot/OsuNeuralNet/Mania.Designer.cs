namespace OsuNeuralNet
{
    partial class Mania
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.LabelDelay = new System.Windows.Forms.Label();
            this.LabelRunning = new System.Windows.Forms.Label();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.LabelDelay);
            this.flowLayoutPanel1.Controls.Add(this.LabelRunning);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(284, 262);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // LabelDelay
            // 
            this.LabelDelay.AutoSize = true;
            this.LabelDelay.Location = new System.Drawing.Point(3, 0);
            this.LabelDelay.Name = "LabelDelay";
            this.LabelDelay.Size = new System.Drawing.Size(34, 13);
            this.LabelDelay.TabIndex = 0;
            this.LabelDelay.Text = "Delay";
            // 
            // LabelRunning
            // 
            this.LabelRunning.AutoSize = true;
            this.LabelRunning.Location = new System.Drawing.Point(43, 0);
            this.LabelRunning.Name = "LabelRunning";
            this.LabelRunning.Size = new System.Drawing.Size(73, 13);
            this.LabelRunning.TabIndex = 1;
            this.LabelRunning.Text = "LabelRunning";
            // 
            // Mania
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Name = "Mania";
            this.Text = "Mania";
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label LabelDelay;
        private System.Windows.Forms.Label LabelRunning;
    }
}