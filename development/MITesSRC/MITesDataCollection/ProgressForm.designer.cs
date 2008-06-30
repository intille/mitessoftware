using MITesDataCollection.Utils;
namespace MITesDataCollection
{
    partial class ProgressForm
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

        private void InitializeInterface()
        {
            this.Width = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width - Constants.SCREEN_WIDTH_MARGIN;
            this.Height = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height - Constants.SCREEN_HEIGHT_MARGIN;
            if ((this.Width > Constants.MAX_SCREEN_WIDTH) || (this.Height > Constants.MAX_SCREEN_HEIGHT))
            {
                this.Width = this.Width / 2;
                this.Height = this.Height / 2;
            }

            int widgetWidth = this.Width - Constants.WIDGET_SPACING -Constants.WIDGET_SPACING;
            int widgetHeight = ((this.Height - Constants.WIDGET_SPACING - Constants.WIDGET_SPACING)/5);

            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProgressForm));
            this.label1 = new System.Windows.Forms.Label();
            this.marqueeTimer = new System.Windows.Forms.Timer();
            this.progressBar9 = new Bornander.UI.BornanderProgressBar.BornanderProgressBar();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(22, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.Text = "Loading...";
            // 
            // marqueeTimer
            // 
            this.marqueeTimer.Enabled = true;
            this.marqueeTimer.Interval = 20;
            this.marqueeTimer.Tick += new System.EventHandler(this.marqueeTimer_Tick);
            // 
            // progressBar9
            // 
  
            this.progressBar9.BackgroundDrawMethod = Bornander.UI.BornanderProgressBar.BornanderProgressBar.DrawMethod.Stretch;
            this.progressBar9.BackgroundLeadingSize = 12;
#if (PocketPC)
            this.progressBar9.BackgroundPicture = ((System.Drawing.Image)(resources.GetObject("progressBar9.BackgroundPicture")));
#else
            this.progressBar9.BackgroundPicture = Resources.vista_background;
#endif 
            this.progressBar9.BackgroundTrailingSize = 12;
            this.progressBar9.ForegroundDrawMethod = Bornander.UI.BornanderProgressBar.BornanderProgressBar.DrawMethod.Stretch;
            this.progressBar9.ForegroundLeadingSize = 12;
#if (PocketPC)
            this.progressBar9.ForegroundPicture = ((System.Drawing.Image)(resources.GetObject("progressBar9.ForegroundPicture")));
#else
            this.progressBar9.ForegroundPicture = Resources.vista_foreground;
#endif 
            this.progressBar9.ForegroundTrailingSize = 12;
            this.progressBar9.Location = new System.Drawing.Point(9, 45);
            this.progressBar9.Marquee = Bornander.UI.BornanderProgressBar.BornanderProgressBar.MarqueeStyle.Wave;
            this.progressBar9.MarqueeWidth = 40;
            this.progressBar9.Maximum = 100;
            this.progressBar9.Minimum = 0;
            this.progressBar9.Name = "progressBar9";
            this.progressBar9.OverlayDrawMethod = Bornander.UI.BornanderProgressBar.BornanderProgressBar.DrawMethod.Stretch;
            this.progressBar9.OverlayLeadingSize = 12;
#if (PocketPC)
            this.progressBar9.OverlayPicture = ((System.Drawing.Image)(resources.GetObject("progressBar9.OverlayPicture")));
#else
            this.progressBar9.OverlayPicture= Resources.vista_overlay;
#endif 
            this.progressBar9.OverlayTrailingSize = 12;
            this.progressBar9.Size = new System.Drawing.Size(widgetWidth, 21);
            this.progressBar9.Type = Bornander.UI.BornanderProgressBar.BornanderProgressBar.BarType.Marquee;
            this.progressBar9.Value = 0;
            // 
            // ProgressForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.Controls.Add(this.progressBar9);
            this.Controls.Add(this.label1);
            this.Name = "ProgressForm";
            this.Text = "Please Wait ...";
            this.ResumeLayout(false);

           
            this.progressBar9.Location = new System.Drawing.Point(Constants.WIDGET_SPACING, Constants.WIDGET_SPACING+(int)(this.Height/5));
            
            this.label1.Text = "Please wait ...";
            this.label1.Size = new System.Drawing.Size(this.Width - Constants.SCREEN_LEFT_MARGIN - Constants.SCREEN_RIGHT_MARGIN, (int)(this.Height / 5));
            this.label1.Location = new System.Drawing.Point(Constants.WIDGET_SPACING, Constants.WIDGET_SPACING);
            this.label1.Font = GUI.CalculateBestFitFont(this.label1.Parent.CreateGraphics(), Constants.MIN_FONT,
                   Constants.MAX_FONT, this.label1.Size, this.label1.Text, this.label1.Font, (float)0.9, (float)0.9);

            this.textbox1 = new System.Windows.Forms.TextBox();
            this.textbox1.Multiline = true;
            this.textbox1.Location = new System.Drawing.Point(this.progressBar9.Location.X, this.progressBar9.Location.Y + this.progressBar9.Height + Constants.WIDGET_SPACING);
            this.textbox1.Size = new System.Drawing.Size(widgetWidth, ((int)widgetHeight*3));
            this.textbox1.Text = "Loading MITes Software...\r\n";
            this.textbox1.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
        
            this.Controls.Add(this.textbox1);
            this.Width = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width - Constants.SCREEN_WIDTH_MARGIN;
            this.Height = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height - Constants.SCREEN_HEIGHT_MARGIN;
            if ((this.Width > Constants.MAX_SCREEN_WIDTH) || (this.Height > Constants.MAX_SCREEN_HEIGHT))
            {
                this.Width = this.Width / 2;
                this.Height = this.Height / 2;
            }
     
        }

        public void AppendLog(string message)
        {
            this.textbox1.Text = message;
            this.textbox1.SelectionStart = this.textbox1.Text.Length;
            this.textbox1.ScrollToCaret();
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.marqueeTimer = new System.Windows.Forms.Timer();
            this.progressBar9 = new Bornander.UI.BornanderProgressBar.BornanderProgressBar();      
            this.SuspendLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox textbox1;
        private System.Windows.Forms.Label label1;
        private Bornander.UI.BornanderProgressBar.BornanderProgressBar progressBar9;
        private System.Windows.Forms.Timer marqueeTimer;

    }
}

