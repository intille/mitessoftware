namespace MITesDataCollection
{
    partial class MITesDataCollectionForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.MainMenu mainMenu1;

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
            this.components = new System.ComponentModel.Container();
            this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
            this.readDataTimer = new System.Windows.Forms.Timer(this.components);
            this.qualityTimer = new System.Windows.Forms.Timer(this.components);
            this.HRTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // readDataTimer
            // 
            this.readDataTimer.Interval = 10;
            this.readDataTimer.Enabled = false;
            this.readDataTimer.Tick += new System.EventHandler(this.readDataTimer_Tick);
            // 
            // qualityTimer
            // 
            this.qualityTimer.Interval = 1000;
            this.qualityTimer.Enabled = false;
            this.qualityTimer.Tick += new System.EventHandler(this.qualityTimer_Tick);
            // 
            // HRTimer
            // 
            this.HRTimer.Interval = 30000;
            this.HRTimer.Enabled = false;
            this.HRTimer.Tick += new System.EventHandler(this.HRTimer_Tick);
            // 
            // MITesDataCollectionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.KeyPreview = true;
            this.Menu = this.mainMenu1;
            this.Name = "MITesDataCollectionForm";
            this.Text = "Collect Data...";
            this.ResumeLayout(false);

        }


        #endregion


        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem menuItem2;
        private System.Windows.Forms.MenuItem menuItem3;
        private System.Windows.Forms.MenuItem menuItem4;
        private System.Windows.Forms.MenuItem menuItem5;
        private System.Windows.Forms.MenuItem menuItem6;
        private System.Windows.Forms.MenuItem menuItem7;
        private System.Windows.Forms.MenuItem menuItem8;
        private System.Windows.Forms.MenuItem menuItem9;
        private System.Windows.Forms.MenuItem menuItem10;
        private System.Windows.Forms.MenuItem menuItem11;
        private System.Windows.Forms.MenuItem menuItem12;
        private System.Windows.Forms.MenuItem menuItem13;
        private System.Windows.Forms.MenuItem menuItem14;
        private System.Windows.Forms.MenuItem menuItem15;
        private System.Windows.Forms.MenuItem menuItem16;
        private System.Windows.Forms.MenuItem menuItem17;
        private System.Windows.Forms.MenuItem menuItem18;
        private System.Windows.Forms.MenuItem menuItem19;
        private System.Windows.Forms.MenuItem menuItem20;
        private System.Windows.Forms.Timer readDataTimer;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button resetButton;
        private System.Windows.Forms.Button startStopButton;
        private System.Windows.Forms.Button oxyconButton;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.MenuItem menuItem21;
        private System.Windows.Forms.MenuItem menuItem22;
        private System.Windows.Forms.Panel panel1, panel2, panel3, panel4;

        #region PC and PocketPC Specific Widgets
#if (PocketPC)
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
#else
        private System.Windows.Forms.Form form1;
        private System.Windows.Forms.Form form2;
        private System.Windows.Forms.Form form3;
        private System.Windows.Forms.Form form4;
        private System.Windows.Forms.Timer qualityTimer;
        private System.Windows.Forms.Timer HRTimer;
#endif 
        #endregion PocketPC Widgets

    }
}