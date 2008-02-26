using System.Collections;
using AXML;
using System.Drawing;
using System.Media;
using System;


namespace MitesAnnotater
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

        
        public Font CalculateBestFitFont(Graphics g, float minFontSize, float maxFontSize, Size layoutSize, string s, Font f, float scale_width,float scale_height)
        {
            if (maxFontSize == minFontSize)
                f = new Font(f.FontFamily, minFontSize, f.Style);

            SizeF extent = g.MeasureString(s, f);

            if (maxFontSize <= minFontSize)
                return f;

            float hRatio = (layoutSize.Height*scale_height) / extent.Height;
            float wRatio = (layoutSize.Width*scale_width) / extent.Width;
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


        private void InitializeSound()
        {
            this.startSound = new SoundPlayer(START_SOUND_FILE);
            this.stopSound = new SoundPlayer(STOP_SOUND_FILE);
            this.resetSound = new SoundPlayer(RESET_SOUND_FILE);
            this.clickSound = new SoundPlayer(CLICK_SOUND_FILE);
        }

        string longest_label = "";
        private void InitializeInterface()
        {

            
            //Initialize form size based on screen size

            this.Width=System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width - SCREEN_WIDTH_MARGIN;
            this.Height = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height - SCREEN_HEIGHT_MARGIN;

            //Initialize Buttons

            this.categoryButtons = new ArrayList();
            this.buttonIndex = new ArrayList();
            int button_width = this.ClientSize.Width - LEFT_MARGIN -RIGHT_MARGIN;
            int button_height = (this.ClientSize.Height- TOP_MARGIN - BOTTOM_MARGIN - (this.annotation.Categories.Count * CONTROL_SPACING)) / (this.annotation.Categories.Count+1);
            int button_x = LEFT_MARGIN;
            int button_y = TOP_MARGIN;

            int delta_y = button_height + CONTROL_SPACING;
            int button_id = 0;
           

            foreach (Category category in this.annotation.Categories)
            {
                System.Windows.Forms.Button button = new System.Windows.Forms.Button();
                button.Location = new System.Drawing.Point(button_x, button_y+ button_id*delta_y);
                button.Name = button_id.ToString();
                button.Font=new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular);
                button.Size = new System.Drawing.Size(button_width, button_height);
                button.TabIndex = button_id;
                button.Text = category.Name+" : "+((Label)category.Labels[0]).Name;
                button.UseVisualStyleBackColor = true;
                button.Click += new System.EventHandler(this.button_Click);
                this.categoryButtons.Add(button);
                this.Controls.Add(button);
                //check the longest label for this button
                foreach (Label label in category.Labels)
                {
                    string newlabel=category.Name + " : " + label.Name;

                    if (newlabel.Length > longest_label.Length)
                        longest_label = newlabel;
                }
                this.buttonIndex.Add(0);
                button_id++;
            }

            foreach (System.Windows.Forms.Button button in categoryButtons)
            {
                button.Font = CalculateBestFitFont(button.CreateGraphics(), MIN_FONT, MAX_FONT,
                button.ClientSize, longest_label, button.Font, (float) 0.9, (float) 0.75);
            }

            //adjust round buttons start/stop -reset
            button_width = (this.Size.Width - LEFT_MARGIN - RIGHT_MARGIN-20)/2;
            this.startStopButton.Size = new System.Drawing.Size(button_width, button_height);
            this.resetButton.Size = new System.Drawing.Size(button_width, button_height);
            this.startStopButton.Location = new System.Drawing.Point(LEFT_MARGIN, button_y + button_id * delta_y);
            this.resetButton.Location = new System.Drawing.Point(this.startStopButton.Location.X+this.startStopButton.Size.Width + CONTROL_SPACING, button_y + button_id * delta_y);
            this.startStopButton.Font = CalculateBestFitFont(this.startStopButton.CreateGraphics(), MIN_FONT, MAX_FONT,
            this.startStopButton.ClientSize, "Start", this.startStopButton.Font, (float)0.9, (float)0.75);
            this.resetButton.Font = CalculateBestFitFont(this.resetButton.CreateGraphics(), MIN_FONT, MAX_FONT,
            this.resetButton.ClientSize, "Reset", this.resetButton.Font, (float)0.9, (float)0.75);

            //Assume a strong signal at the start
            this.pictureBox1.Image = System.Drawing.Image.FromFile(WEAK_SIGNAL_FILE);

            this.Resize += new EventHandler(OnResize);


        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label4 = new System.Windows.Forms.Label();
            this.startStopButton = new System.Windows.Forms.Button();
            this.resetButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.Green;
            this.label1.Location = new System.Drawing.Point(3, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "0:00:00";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.label2.Location = new System.Drawing.Point(53, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(13, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "/";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.label3.Location = new System.Drawing.Point(67, 3);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "0:00:00";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = System.Drawing.Image.FromFile(WEAK_SIGNAL_FILE); 
            this.pictureBox1.Location = new System.Drawing.Point(205, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(26, 20);
            this.pictureBox1.TabIndex = 5;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.label4.ForeColor = System.Drawing.Color.Red;
            this.label4.Location = new System.Drawing.Point(177, 5);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(25, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "HR";
            // 
            // roundButton1
            // 


            this.startStopButton.BackColor = System.Drawing.Color.Green;            
            this.startStopButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.startStopButton.Location = new System.Drawing.Point(26, 218);
            this.startStopButton.Name = "startStopButton";            
            this.startStopButton.Size = new System.Drawing.Size(78, 57);
            this.startStopButton.TabIndex = 10;
            this.startStopButton.Text = "Start";            
            this.startStopButton.Click += new System.EventHandler(this.roundButton1_Click);
            // 
            // roundButton2
            // 
            this.resetButton.BackColor = System.Drawing.Color.Yellow;
            this.resetButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.resetButton.Location = new System.Drawing.Point(124, 218);
            this.resetButton.Name = "resetButton";            
            this.resetButton.Size = new System.Drawing.Size(78, 57);
            this.resetButton.TabIndex = 10;
            this.resetButton.Text = "Reset";            
            this.resetButton.Click += new System.EventHandler(this.roundButton2_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(232, 280);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.startStopButton);
            this.Controls.Add(this.resetButton);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "MainForm";
            this.Text = "MITes Annotator";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }


        #endregion

        public void SetText(string label,int control_id)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.label1.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { label,control_id });
            }
            else
            {
                if (control_id == GOOD_TIMER)
                    this.label1.Text = label;
                else if (control_id == OVERALL_TIMER)
                {
                    this.label3.Text = label;

                }
            }
        }


        public void SetSignalSign(bool isGood)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.pictureBox1.InvokeRequired)
            {
                SetSignalCallback d = new SetSignalCallback(SetSignalSign);
                this.Invoke(d, new object[] { isGood });
            }
            else
            {
               //set the sign + control the good/bad timer
                if (isGood)
                {
                    if (this.startStopButton.BackColor==System.Drawing.Color.Red)
                        this.goodTimer.start();
                    this.pictureBox1.Image = System.Drawing.Image.FromFile(STRONG_SIGNAL_FILE);
                }
                else
                {
                    if (this.startStopButton.BackColor == System.Drawing.Color.Red)
                        this.goodTimer.stop();
                    this.pictureBox1.Image = System.Drawing.Image.FromFile(WEAK_SIGNAL_FILE);
                }

                //check for heart rate
                double rate = ((double)this.aMITesDataFilterer.MitesPerformanceTracker[0].PreviousCounter / (double)this.aMITesDataFilterer.MitesPerformanceTracker[0].GoodRate) * 100;
                //Testing - remove random component
                
                //int randvar = new Random().Next(3);
                if ( (rate > 30))
                {
                    this.label4.ForeColor = System.Drawing.Color.Green;
                }
                else
                {
                    this.label4.ForeColor = System.Drawing.Color.Red;
                }
            }
        }

        public void InitializeTimers()
        {
            this.goodTimer = new ATimer(this, GOOD_TIMER);
            this.overallTimer = new ATimer(this, OVERALL_TIMER);

        }

        //Constants for dynamic layouts
        public const int GOOD_TIMER = 1;
        public const int OVERALL_TIMER = 2;
        public const int TOP_MARGIN = 26;
        public const int LEFT_MARGIN = 10;
        public const int RIGHT_MARGIN = 10;
        public const int BOTTOM_MARGIN = 10;
        public const int CONTROL_SPACING = 4;
        public const int MIN_FONT = 6;
        public const int MAX_FONT =64;

        public const int SCREEN_WIDTH_MARGIN = 10;
        public const int SCREEN_HEIGHT_MARGIN = 10;
        
        
        //Constant files for sound
        private const string CLICK_SOUND_FILE = "..\\..\\NeededFiles\\sounds\\click.wav";
        private const string RESET_SOUND_FILE = "..\\..\\NeededFiles\\sounds\\reset.wav";
        private const string START_SOUND_FILE = "..\\..\\NeededFiles\\sounds\\start.wav";
        private const string STOP_SOUND_FILE = "..\\..\\NeededFiles\\sounds\\stop.wav";

        private const string STRONG_SIGNAL_FILE = "..\\..\\NeededFiles\\images\\strong.gif";
        private const string WEAK_SIGNAL_FILE = "..\\..\\NeededFiles\\images\\weak.gif";


        private SoundPlayer startSound, resetSound, stopSound, clickSound;
        private ATimer goodTimer, overallTimer;
        private ArrayList categoryButtons;
        private ArrayList buttonIndex;

        delegate void SetTextCallback(string label, int control_id);
        delegate void SetSignalCallback(bool isGood);

 


        private System.Windows.Forms.Button startStopButton;
        private System.Windows.Forms.Button resetButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label4;
    }
}

