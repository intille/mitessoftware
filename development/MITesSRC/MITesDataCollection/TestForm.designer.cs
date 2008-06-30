namespace Bornander.UI.ProgressBar.Test
{
    partial class TestForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TestForm));
            this.trackBar = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.progressBar5 = new Bornander.UI.ProgressBar.ProgressBar();
            this.progressBar4 = new Bornander.UI.ProgressBar.ProgressBar();
            this.progressBar3 = new Bornander.UI.ProgressBar.ProgressBar();
            this.progressBar2 = new Bornander.UI.ProgressBar.ProgressBar();
            this.progressBar6 = new System.Windows.Forms.ProgressBar();
            this.progressBar7 = new Bornander.UI.ProgressBar.ProgressBar();
            this.progressBar8 = new Bornander.UI.ProgressBar.ProgressBar();
            this.progressBar9 = new Bornander.UI.ProgressBar.ProgressBar();
            this.progressBar10 = new Bornander.UI.ProgressBar.ProgressBar();
            this.progressBar11 = new Bornander.UI.ProgressBar.ProgressBar();
            this.marqueeTimer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.trackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // trackBar
            // 
            this.trackBar.Location = new System.Drawing.Point(404, 30);
            this.trackBar.Maximum = 100;
            this.trackBar.Name = "trackBar";
            this.trackBar.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBar.Size = new System.Drawing.Size(45, 250);
            this.trackBar.TabIndex = 0;
            this.trackBar.TickFrequency = 5;
            this.trackBar.Scroll += new System.EventHandler(this.trackBar_Scroll);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(98, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Winforms standard:";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(16, 30);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(188, 23);
            this.progressBar1.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(86, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Custom XP style:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 107);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(131, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Custom Mac style (I think):";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 148);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(95, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Custom Vista style:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(16, 219);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(89, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Custom text style:";
            // 
            // progressBar5
            // 
            this.progressBar5.BackgroundDrawMethod = Bornander.UI.ProgressBar.ProgressBar.DrawMethod.Tile;
            this.progressBar5.BackgroundLeadingSize = 0;
            this.progressBar5.BackgroundPicture = ((System.Drawing.Image)(resources.GetObject("progressBar5.BackgroundPicture")));
            this.progressBar5.BackgroundTrailingSize = 0;
            this.progressBar5.ForegroundDrawMethod = Bornander.UI.ProgressBar.ProgressBar.DrawMethod.Tile;
            this.progressBar5.ForegroundLeadingSize = 0;
            this.progressBar5.ForegroundPicture = ((System.Drawing.Image)(resources.GetObject("progressBar5.ForegroundPicture")));
            this.progressBar5.ForegroundTrailingSize = 0;
            this.progressBar5.Location = new System.Drawing.Point(16, 238);
            this.progressBar5.Marquee = Bornander.UI.ProgressBar.ProgressBar.MarqueeStyle.TileWrap;
            this.progressBar5.MarqueeWidth = 10;
            this.progressBar5.Maximum = 100;
            this.progressBar5.Minimum = 0;
            this.progressBar5.Name = "progressBar5";
            this.progressBar5.OverlayDrawMethod = Bornander.UI.ProgressBar.ProgressBar.DrawMethod.Stretch;
            this.progressBar5.OverlayLeadingSize = 0;
            this.progressBar5.OverlayPicture = null;
            this.progressBar5.OverlayTrailingSize = 0;
            this.progressBar5.Size = new System.Drawing.Size(188, 42);
            this.progressBar5.TabIndex = 10;
            this.progressBar5.Type = Bornander.UI.ProgressBar.ProgressBar.BarType.Progress;
            this.progressBar5.Value = 0;
            // 
            // progressBar4
            // 
            this.progressBar4.BackgroundDrawMethod = Bornander.UI.ProgressBar.ProgressBar.DrawMethod.Stretch;
            this.progressBar4.BackgroundLeadingSize = 12;
            this.progressBar4.BackgroundPicture = global::Bornander.UI.ProgressBar.Test.Properties.Resources.vista_background;
            this.progressBar4.BackgroundTrailingSize = 12;
            this.progressBar4.ForegroundDrawMethod = Bornander.UI.ProgressBar.ProgressBar.DrawMethod.Stretch;
            this.progressBar4.ForegroundLeadingSize = 12;
            this.progressBar4.ForegroundPicture = global::Bornander.UI.ProgressBar.Test.Properties.Resources.vista_foreground;
            this.progressBar4.ForegroundTrailingSize = 12;
            this.progressBar4.Location = new System.Drawing.Point(16, 165);
            this.progressBar4.Marquee = Bornander.UI.ProgressBar.ProgressBar.MarqueeStyle.TileWrap;
            this.progressBar4.MarqueeWidth = 10;
            this.progressBar4.Maximum = 100;
            this.progressBar4.Minimum = 0;
            this.progressBar4.Name = "progressBar4";
            this.progressBar4.OverlayDrawMethod = Bornander.UI.ProgressBar.ProgressBar.DrawMethod.Stretch;
            this.progressBar4.OverlayLeadingSize = 12;
            this.progressBar4.OverlayPicture = global::Bornander.UI.ProgressBar.Test.Properties.Resources.vista_overlay;
            this.progressBar4.OverlayTrailingSize = 12;
            this.progressBar4.Size = new System.Drawing.Size(188, 24);
            this.progressBar4.TabIndex = 8;
            this.progressBar4.Type = Bornander.UI.ProgressBar.ProgressBar.BarType.Progress;
            this.progressBar4.Value = 0;
            // 
            // progressBar3
            // 
            this.progressBar3.BackgroundDrawMethod = Bornander.UI.ProgressBar.ProgressBar.DrawMethod.Tile;
            this.progressBar3.BackgroundLeadingSize = 0;
            this.progressBar3.BackgroundPicture = global::Bornander.UI.ProgressBar.Test.Properties.Resources.mac_background;
            this.progressBar3.BackgroundTrailingSize = 0;
            this.progressBar3.ForegroundDrawMethod = Bornander.UI.ProgressBar.ProgressBar.DrawMethod.Tile;
            this.progressBar3.ForegroundLeadingSize = 0;
            this.progressBar3.ForegroundPicture = global::Bornander.UI.ProgressBar.Test.Properties.Resources.mac_foreground;
            this.progressBar3.ForegroundTrailingSize = 0;
            this.progressBar3.Location = new System.Drawing.Point(16, 124);
            this.progressBar3.Marquee = Bornander.UI.ProgressBar.ProgressBar.MarqueeStyle.TileWrap;
            this.progressBar3.MarqueeWidth = 10;
            this.progressBar3.Maximum = 100;
            this.progressBar3.Minimum = 0;
            this.progressBar3.Name = "progressBar3";
            this.progressBar3.OverlayDrawMethod = Bornander.UI.ProgressBar.ProgressBar.DrawMethod.Stretch;
            this.progressBar3.OverlayLeadingSize = 4;
            this.progressBar3.OverlayPicture = global::Bornander.UI.ProgressBar.Test.Properties.Resources.mac_overlay;
            this.progressBar3.OverlayTrailingSize = 4;
            this.progressBar3.Size = new System.Drawing.Size(188, 17);
            this.progressBar3.TabIndex = 6;
            this.progressBar3.Type = Bornander.UI.ProgressBar.ProgressBar.BarType.Progress;
            this.progressBar3.Value = 0;
            // 
            // progressBar2
            // 
            this.progressBar2.BackgroundDrawMethod = Bornander.UI.ProgressBar.ProgressBar.DrawMethod.Stretch;
            this.progressBar2.BackgroundLeadingSize = 5;
            this.progressBar2.BackgroundPicture = ((System.Drawing.Image)(resources.GetObject("progressBar2.BackgroundPicture")));
            this.progressBar2.BackgroundTrailingSize = 5;
            this.progressBar2.ForegroundDrawMethod = Bornander.UI.ProgressBar.ProgressBar.DrawMethod.Tile;
            this.progressBar2.ForegroundLeadingSize = 0;
            this.progressBar2.ForegroundPicture = global::Bornander.UI.ProgressBar.Test.Properties.Resources.xp_foreground;
            this.progressBar2.ForegroundTrailingSize = 0;
            this.progressBar2.Location = new System.Drawing.Point(16, 77);
            this.progressBar2.Marquee = Bornander.UI.ProgressBar.ProgressBar.MarqueeStyle.TileWrap;
            this.progressBar2.MarqueeWidth = 10;
            this.progressBar2.Maximum = 100;
            this.progressBar2.Minimum = 0;
            this.progressBar2.Name = "progressBar2";
            this.progressBar2.OverlayDrawMethod = Bornander.UI.ProgressBar.ProgressBar.DrawMethod.Stretch;
            this.progressBar2.OverlayLeadingSize = 5;
            this.progressBar2.OverlayPicture = ((System.Drawing.Image)(resources.GetObject("progressBar2.OverlayPicture")));
            this.progressBar2.OverlayTrailingSize = 5;
            this.progressBar2.Size = new System.Drawing.Size(188, 27);
            this.progressBar2.TabIndex = 4;
            this.progressBar2.Type = Bornander.UI.ProgressBar.ProgressBar.BarType.Progress;
            this.progressBar2.Value = 0;
            // 
            // progressBar6
            // 
            this.progressBar6.Location = new System.Drawing.Point(210, 30);
            this.progressBar6.Name = "progressBar6";
            this.progressBar6.Size = new System.Drawing.Size(188, 23);
            this.progressBar6.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar6.TabIndex = 11;
            this.progressBar6.Value = 50;
            // 
            // progressBar7
            // 
            this.progressBar7.BackgroundDrawMethod = Bornander.UI.ProgressBar.ProgressBar.DrawMethod.Stretch;
            this.progressBar7.BackgroundLeadingSize = 5;
            this.progressBar7.BackgroundPicture = ((System.Drawing.Image)(resources.GetObject("progressBar7.BackgroundPicture")));
            this.progressBar7.BackgroundTrailingSize = 5;
            this.progressBar7.ForegroundDrawMethod = Bornander.UI.ProgressBar.ProgressBar.DrawMethod.Tile;
            this.progressBar7.ForegroundLeadingSize = 0;
            this.progressBar7.ForegroundPicture = global::Bornander.UI.ProgressBar.Test.Properties.Resources.xp_foreground;
            this.progressBar7.ForegroundTrailingSize = 0;
            this.progressBar7.Location = new System.Drawing.Point(210, 77);
            this.progressBar7.Marquee = Bornander.UI.ProgressBar.ProgressBar.MarqueeStyle.TileWrap;
            this.progressBar7.MarqueeWidth = 10;
            this.progressBar7.Maximum = 100;
            this.progressBar7.Minimum = 0;
            this.progressBar7.Name = "progressBar7";
            this.progressBar7.OverlayDrawMethod = Bornander.UI.ProgressBar.ProgressBar.DrawMethod.Stretch;
            this.progressBar7.OverlayLeadingSize = 5;
            this.progressBar7.OverlayPicture = ((System.Drawing.Image)(resources.GetObject("progressBar7.OverlayPicture")));
            this.progressBar7.OverlayTrailingSize = 5;
            this.progressBar7.Size = new System.Drawing.Size(188, 27);
            this.progressBar7.TabIndex = 12;
            this.progressBar7.Type = Bornander.UI.ProgressBar.ProgressBar.BarType.Marquee;
            this.progressBar7.Value = 0;
            // 
            // progressBar8
            // 
            this.progressBar8.BackgroundDrawMethod = Bornander.UI.ProgressBar.ProgressBar.DrawMethod.Tile;
            this.progressBar8.BackgroundLeadingSize = 0;
            this.progressBar8.BackgroundPicture = global::Bornander.UI.ProgressBar.Test.Properties.Resources.mac_background;
            this.progressBar8.BackgroundTrailingSize = 0;
            this.progressBar8.ForegroundDrawMethod = Bornander.UI.ProgressBar.ProgressBar.DrawMethod.Tile;
            this.progressBar8.ForegroundLeadingSize = 0;
            this.progressBar8.ForegroundPicture = global::Bornander.UI.ProgressBar.Test.Properties.Resources.mac_foreground;
            this.progressBar8.ForegroundTrailingSize = 0;
            this.progressBar8.Location = new System.Drawing.Point(210, 124);
            this.progressBar8.Marquee = Bornander.UI.ProgressBar.ProgressBar.MarqueeStyle.TileWrap;
            this.progressBar8.MarqueeWidth = 10;
            this.progressBar8.Maximum = 100;
            this.progressBar8.Minimum = 0;
            this.progressBar8.Name = "progressBar8";
            this.progressBar8.OverlayDrawMethod = Bornander.UI.ProgressBar.ProgressBar.DrawMethod.Stretch;
            this.progressBar8.OverlayLeadingSize = 4;
            this.progressBar8.OverlayPicture = global::Bornander.UI.ProgressBar.Test.Properties.Resources.mac_overlay;
            this.progressBar8.OverlayTrailingSize = 4;
            this.progressBar8.Size = new System.Drawing.Size(188, 17);
            this.progressBar8.TabIndex = 13;
            this.progressBar8.Type = Bornander.UI.ProgressBar.ProgressBar.BarType.Marquee;
            this.progressBar8.Value = 0;
            // 
            // progressBar9
            // 
            this.progressBar9.BackgroundDrawMethod = Bornander.UI.ProgressBar.ProgressBar.DrawMethod.Stretch;
            this.progressBar9.BackgroundLeadingSize = 12;
            this.progressBar9.BackgroundPicture = global::Bornander.UI.ProgressBar.Test.Properties.Resources.vista_background;
            this.progressBar9.BackgroundTrailingSize = 12;
            this.progressBar9.ForegroundDrawMethod = Bornander.UI.ProgressBar.ProgressBar.DrawMethod.Stretch;
            this.progressBar9.ForegroundLeadingSize = 14;
            this.progressBar9.ForegroundPicture = global::Bornander.UI.ProgressBar.Test.Properties.Resources.vista_foreground;
            this.progressBar9.ForegroundTrailingSize = 14;
            this.progressBar9.Location = new System.Drawing.Point(210, 165);
            this.progressBar9.Marquee = Bornander.UI.ProgressBar.ProgressBar.MarqueeStyle.Wave;
            this.progressBar9.MarqueeWidth = 30;
            this.progressBar9.Maximum = 100;
            this.progressBar9.Minimum = 0;
            this.progressBar9.Name = "progressBar9";
            this.progressBar9.OverlayDrawMethod = Bornander.UI.ProgressBar.ProgressBar.DrawMethod.Stretch;
            this.progressBar9.OverlayLeadingSize = 12;
            this.progressBar9.OverlayPicture = global::Bornander.UI.ProgressBar.Test.Properties.Resources.vista_overlay;
            this.progressBar9.OverlayTrailingSize = 12;
            this.progressBar9.Size = new System.Drawing.Size(188, 24);
            this.progressBar9.TabIndex = 14;
            this.progressBar9.Type = Bornander.UI.ProgressBar.ProgressBar.BarType.Marquee;
            this.progressBar9.Value = 0;
            // 
            // progressBar10
            // 
            this.progressBar10.BackgroundDrawMethod = Bornander.UI.ProgressBar.ProgressBar.DrawMethod.Tile;
            this.progressBar10.BackgroundLeadingSize = 0;
            this.progressBar10.BackgroundPicture = ((System.Drawing.Image)(resources.GetObject("progressBar10.BackgroundPicture")));
            this.progressBar10.BackgroundTrailingSize = 0;
            this.progressBar10.ForegroundDrawMethod = Bornander.UI.ProgressBar.ProgressBar.DrawMethod.Tile;
            this.progressBar10.ForegroundLeadingSize = 0;
            this.progressBar10.ForegroundPicture = ((System.Drawing.Image)(resources.GetObject("progressBar10.ForegroundPicture")));
            this.progressBar10.ForegroundTrailingSize = 0;
            this.progressBar10.Location = new System.Drawing.Point(210, 238);
            this.progressBar10.Marquee = Bornander.UI.ProgressBar.ProgressBar.MarqueeStyle.TileWrap;
            this.progressBar10.MarqueeWidth = 10;
            this.progressBar10.Maximum = 100;
            this.progressBar10.Minimum = 0;
            this.progressBar10.Name = "progressBar10";
            this.progressBar10.OverlayDrawMethod = Bornander.UI.ProgressBar.ProgressBar.DrawMethod.Stretch;
            this.progressBar10.OverlayLeadingSize = 0;
            this.progressBar10.OverlayPicture = null;
            this.progressBar10.OverlayTrailingSize = 0;
            this.progressBar10.Size = new System.Drawing.Size(188, 42);
            this.progressBar10.TabIndex = 15;
            this.progressBar10.Type = Bornander.UI.ProgressBar.ProgressBar.BarType.Marquee;
            this.progressBar10.Value = 0;
            // 
            // progressBar11
            // 
            this.progressBar11.BackgroundDrawMethod = Bornander.UI.ProgressBar.ProgressBar.DrawMethod.Stretch;
            this.progressBar11.BackgroundLeadingSize = 12;
            this.progressBar11.BackgroundPicture = global::Bornander.UI.ProgressBar.Test.Properties.Resources.vista_background;
            this.progressBar11.BackgroundTrailingSize = 12;
            this.progressBar11.ForegroundDrawMethod = Bornander.UI.ProgressBar.ProgressBar.DrawMethod.Tile;
            this.progressBar11.ForegroundLeadingSize = 20;
            this.progressBar11.ForegroundPicture = global::Bornander.UI.ProgressBar.Test.Properties.Resources.vista_foreground;
            this.progressBar11.ForegroundTrailingSize = 20;
            this.progressBar11.Location = new System.Drawing.Point(210, 195);
            this.progressBar11.Marquee = Bornander.UI.ProgressBar.ProgressBar.MarqueeStyle.BlockWrap;
            this.progressBar11.MarqueeWidth = 10;
            this.progressBar11.Maximum = 100;
            this.progressBar11.Minimum = 0;
            this.progressBar11.Name = "progressBar11";
            this.progressBar11.OverlayDrawMethod = Bornander.UI.ProgressBar.ProgressBar.DrawMethod.Stretch;
            this.progressBar11.OverlayLeadingSize = 12;
            this.progressBar11.OverlayPicture = global::Bornander.UI.ProgressBar.Test.Properties.Resources.vista_overlay;
            this.progressBar11.OverlayTrailingSize = 12;
            this.progressBar11.Size = new System.Drawing.Size(188, 24);
            this.progressBar11.TabIndex = 15;
            this.progressBar11.Type = Bornander.UI.ProgressBar.ProgressBar.BarType.Marquee;
            this.progressBar11.Value = 0;
            // 
            // marqueeTimer
            // 
            this.marqueeTimer.Enabled = true;
            this.marqueeTimer.Interval = 20;
            this.marqueeTimer.Tick += new System.EventHandler(this.marqueeTimer_Tick);
            // 
            // TestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(456, 298);
            this.Controls.Add(this.progressBar11);
            this.Controls.Add(this.progressBar10);
            this.Controls.Add(this.progressBar9);
            this.Controls.Add(this.progressBar8);
            this.Controls.Add(this.progressBar7);
            this.Controls.Add(this.progressBar6);
            this.Controls.Add(this.progressBar5);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.progressBar4);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.progressBar3);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.progressBar2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.trackBar);
            this.Name = "TestForm";
            this.Text = "Custom ScrollBar Test";
            ((System.ComponentModel.ISupportInitialize)(this.trackBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TrackBar trackBar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label label2;
        private ProgressBar progressBar2;
        private System.Windows.Forms.Label label3;
        private ProgressBar progressBar3;
        private System.Windows.Forms.Label label4;
        private ProgressBar progressBar4;
        private System.Windows.Forms.Label label5;
        private ProgressBar progressBar5;
        private System.Windows.Forms.ProgressBar progressBar6;
        private ProgressBar progressBar7;
        private ProgressBar progressBar8;
        private ProgressBar progressBar9;
        private ProgressBar progressBar10;
        private ProgressBar progressBar11;
        private System.Windows.Forms.Timer marqueeTimer;

    }
}

