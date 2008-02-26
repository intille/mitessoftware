using System.Drawing;
using System;

namespace MITesLogger_PC
{
    partial class MainForm
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
            
            this.Width = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width - Constants.SCREEN_WIDTH_MARGIN;
            this.Height = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height - Constants.SCREEN_HEIGHT_MARGIN;

            int button_width = this.ClientSize.Width - Constants.LEFT_MARGIN - Constants.RIGHT_MARGIN;
            int button_height = (this.ClientSize.Height - Constants.TOP_MARGIN - Constants.BOTTOM_MARGIN - (NUMBER_BUTTONS * Constants.CONTROL_SPACING)) / NUMBER_BUTTONS;
            int button_x = Constants.LEFT_MARGIN;
            int button_y = Constants.TOP_MARGIN;
            int delta_y = button_height + Constants.CONTROL_SPACING;            

            this.button1.Location = new System.Drawing.Point(button_x, button_y);            
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Bold);
            this.button1.Size = new System.Drawing.Size(button_width, button_height);
            this.button1.Font = CalculateBestFitFont(this.button1.CreateGraphics(), Constants.MIN_FONT, Constants.MAX_FONT,
                this.button1.ClientSize, LONGEST_LABEL, this.button1.Font, (float)0.9, (float)0.75);
            
            button_y += delta_y;
            this.button2.Location = new System.Drawing.Point(button_x, button_y);
            this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Bold);
            this.button2.Size = new System.Drawing.Size(button_width, button_height);
            this.button2.Font = CalculateBestFitFont(this.button2.CreateGraphics(), Constants.MIN_FONT, Constants.MAX_FONT,
                this.button2.ClientSize, LONGEST_LABEL, this.button2.Font, (float)0.9, (float)0.75);

            button_y += delta_y;
            this.button3.Location = new System.Drawing.Point(button_x, button_y);
            this.button3.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Bold);
            this.button3.Size = new System.Drawing.Size(button_width, button_height);
            this.button3.Font = CalculateBestFitFont(this.button3.CreateGraphics(), Constants.MIN_FONT, Constants.MAX_FONT,
                this.button3.ClientSize, LONGEST_LABEL, this.button3.Font, (float)0.9, (float)0.75);

            this.Resize += new EventHandler(OnResize);
            this.button2.Enabled = false;
            this.button3.Enabled = false;
            
        }


        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(93, 41);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(110, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Collect Data";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.Location = new System.Drawing.Point(93, 98);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(110, 23);
            this.button2.TabIndex = 1;
            this.button2.Text = "Visualize Data";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button3.Location = new System.Drawing.Point(93, 161);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(110, 23);
            this.button3.TabIndex = 2;
            this.button3.Text = "Demo";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 269);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Name = "MainForm";
            this.Text = "MITes Software Tools";
            this.ResumeLayout(false);

        }

        #endregion


        private const int NUMBER_BUTTONS = 3;
        private const string LONGEST_LABEL = "Visualize Data";
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
    }
}