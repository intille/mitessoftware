using System.Drawing;

namespace MITesLogger_PC
{
    partial class ProgressForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="minFontSize"></param>
        /// <param name="maxFontSize"></param>
        /// <param name="layoutSize"></param>
        /// <param name="s"></param>
        /// <param name="f"></param>
        /// <param name="scale_width"></param>
        /// <param name="scale_height"></param>
        /// <returns></returns>
        public Font CalculateBestFitFont(Graphics g, float minFontSize, float maxFontSize, Size layoutSize, string s, Font f, float scale_width, float scale_height)
        {
            if (maxFontSize == minFontSize)
                f = new Font(f.FontFamily, minFontSize, f.Style);

            SizeF extent = g.MeasureString(s, f);

            if (maxFontSize <= minFontSize)
                return f;

            float hRatio = (layoutSize.Height * scale_height) / extent.Height;
            float wRatio = (layoutSize.Width * scale_width) / extent.Width;
            float ratio = (hRatio < wRatio) ? hRatio : wRatio;

            float newSize = f.Size * ratio;

            if (newSize < minFontSize)
                newSize = minFontSize;
            else if (newSize > maxFontSize)
                newSize = maxFontSize;

            f = new Font(f.FontFamily, newSize, f.Style);
            extent = g.MeasureString(s, f);

            return f;
        }

        /// <summary>
        /// 
        /// </summary>
        public void InitializeInterface()
        {
            //Initialize form size based on screen size

            this.Width = ((int)((System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width - Constants.SCREEN_WIDTH_MARGIN)/2));
            this.Height = ((int)((System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height - Constants.SCREEN_HEIGHT_MARGIN)/2));
            this.Location= new System.Drawing.Point( (int)(this.Width/4),(int)(this.Height/4));

            int button_width = this.ClientSize.Width - Constants.LEFT_MARGIN - Constants.RIGHT_MARGIN;
            int button_height = (this.ClientSize.Height - Constants.TOP_MARGIN - Constants.BOTTOM_MARGIN - (3 * Constants.CONTROL_SPACING)) / 3;
            int button_x = Constants.LEFT_MARGIN;
            int button_y = Constants.TOP_MARGIN;
            int delta_y = button_height + Constants.CONTROL_SPACING;


            this.progressBar1.Size= new System.Drawing.Size(button_width, (int)(2*button_height));
            this.progressBar1.Location = new System.Drawing.Point(button_x, button_y);
            this.progressBar1.Minimum = 1;
            this.progressBar1.Maximum = 100;
            button_y += Constants.CONTROL_SPACING;

            this.label1.Location = new System.Drawing.Point(button_x, button_y);
            this.label1.Size = new System.Drawing.Size(button_width, button_height);
            this.label1.Font = CalculateBestFitFont(this.label1.CreateGraphics(), Constants.MIN_FONT, Constants.MAX_FONT,
                this.label1.Size, "Loading MITes Configuration Files ...", this.label1.Font, (float)0.9, (float)0.75);

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="description"></param>
        /// <param name="step"></param>
        public void PerformStep(string description, int step)
        {
            this.progressBar1.Step = step;
            this.progressBar1.PerformStep();
            this.label1.Text = description;
        }
        /// <summary>
        /// 
        /// </summary>
        public void ResetBar()
        {
            this.progressBar1.Value = 0;
        }
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
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(30, 41);
            this.progressBar1.Maximum = 50;
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(520, 44);
            this.progressBar1.Step = 1;
            this.progressBar1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(30, 92);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "label1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(30, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(114, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Loading... Please Wait";
            // 
            // ProgressForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(568, 134);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.progressBar1);
            this.Name = "ProgressForm";
            this.Text = "Loading MITes Data Collection...";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}