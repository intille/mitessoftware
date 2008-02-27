using System;
using System.Windows.Forms;
using System.Drawing;

namespace MITesLogger_PC
{
    partial class SessionForm
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

        private void InitializeInterface()
        {
            //Initializations
            SimpleLogger.Logger logger = new SimpleLogger.Logger(Form1.NEEDED_FILES_PATH+"NeededFiles\\log", SimpleLogger.Logger.Priority.FATAL);
            string executableDirectory = System.Windows.Forms.Application.StartupPath;

            //Check the Sensor Configurations master file                     
            try
            {
                SXML.ConfigurationReader creader = new SXML.ConfigurationReader();
                if (creader.validate() == false)
                {
                    MessageBox.Show("Error: The format of " + executableDirectory + "\\" + SXML.ConfigurationReader.DEFAULT_XML_FILE +
                        " is incorrect. Please correct the file and re-run the program.");
                    logger.Error("The format of " + executableDirectory + "\\" + SXML.ConfigurationReader.DEFAULT_XML_FILE +
                        " is incorrect. Please correct the file and re-run the program.");
                    logger.Dispose();
                    Environment.Exit(0);
                }
                this.sensorConfigurations = creader.parse();
                

            }
            catch (Exception e)
            {
                MessageBox.Show("Fatal Error: Exception occurred when reading " + executableDirectory +
                    "\\" + SXML.ConfigurationReader.DEFAULT_XML_FILE + ". For more details check the log most recent log file at" +
                    executableDirectory + "\\" + SimpleLogger.Logger.LOGGER_DIRECTORY);
                logger.Fatal("Exception occurred when reading " + executableDirectory +
                    "\\" + SXML.ConfigurationReader.DEFAULT_XML_FILE + ". " + e.ToString());
                logger.Dispose();
                Environment.Exit(0);
            }

            //Check the activity protocols master file

            try
            {
                AXML.ProtocolsReader preader = new AXML.ProtocolsReader();
                if (preader.validate() == false)
                {
                    MessageBox.Show("Error: The format of " + executableDirectory + "\\" + AXML.ProtocolsReader.DEFAULT_XML_FILE +
                        " is incorrect. Please correct the file and re-run the program.");
                    logger.Error("The format of " + executableDirectory + "\\" + AXML.ProtocolsReader.DEFAULT_XML_FILE +
                        " is incorrect. Please correct the file and re-run the program.");
                    logger.Dispose();
                    Environment.Exit(0);
                }
                this.activityProtocols = preader.parse();

            }
            catch (Exception e)
            {
                MessageBox.Show("Fatal Error: Exception occurred when reading " + executableDirectory +
                    "\\" + AXML.ProtocolsReader.DEFAULT_XML_FILE + ". For more details check the log most recent log file at" +
                    executableDirectory +"\\"+SimpleLogger.Logger.LOGGER_DIRECTORY);
                logger.Fatal("Exception occurred when reading " + executableDirectory +
                    "\\" + AXML.ProtocolsReader.DEFAULT_XML_FILE + ". " + e.ToString());
                logger.Dispose();
                Environment.Exit(0);
            }

            for (int i=0;(i<this.activityProtocols.ActivityProtocols.Count);i++)
            {
                this.listBox1.Items.Add(((AXML.Protocol)this.activityProtocols.ActivityProtocols[i]).Name);
            }

            for (int i = 0; (i < this.sensorConfigurations.SensorConfigurations.Count); i++)
            {
                this.listBox2.Items.Add(((SXML.Configuration)this.sensorConfigurations.SensorConfigurations[i]).Name);
            }

            //logger has to be disposed
            logger.Dispose();


            //resize the interface components

            string longest_description = this.activityProtocols.LongestLabel;
            if (this.sensorConfigurations.LongestDescription.Length > longest_description.Length)
                longest_description = this.sensorConfigurations.LongestDescription;

            this.Width = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width - Constants.SCREEN_WIDTH_MARGIN;
            this.Height = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height - Constants.SCREEN_HEIGHT_MARGIN;


            int button_width = (this.Size.Width - Constants.LEFT_MARGIN - Constants.RIGHT_MARGIN - 20) / 2; 
            int button_height = (this.ClientSize.Height - Constants.TOP_MARGIN - Constants.BOTTOM_MARGIN - (15 * Constants.CONTROL_SPACING)) / 15;
            int button_x = Constants.LEFT_MARGIN;
            int button_y = Constants.TOP_MARGIN;
            int delta_y = button_height + Constants.CONTROL_SPACING;

            this.label1.Location = new System.Drawing.Point(button_x, button_y);
            this.label1.Size = new System.Drawing.Size(button_width, button_height);
            this.label1.Font = CalculateBestFitFont(this.label1.CreateGraphics(), Constants.MIN_FONT, Constants.MAX_FONT,
                this.label1.Size, "Activity Protocols", this.label1.Font, (float)0.9, (float)0.75);

            button_y += delta_y;
            this.listBox1.Location = new System.Drawing.Point(button_x, button_y);
            this.listBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Bold);
            this.listBox1.Size = new System.Drawing.Size(button_width, button_height*5);
            this.listBox1.Font = this.label1.Font;

            this.textBox2.Location = new System.Drawing.Point(button_x + button_width + Constants.CONTROL_SPACING, button_y);
            this.textBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Bold);
            this.textBox2.Size = new System.Drawing.Size(button_width, button_height * 5);
            this.textBox2.Font = CalculateBestFitFont(this.textBox2.CreateGraphics(), Constants.MIN_FONT, Constants.MAX_FONT,
                this.textBox2.ClientSize,this.activityProtocols.LongestDescription, this.textBox2.Font, (float)0.9, (float)0.75);


            button_y += 5*delta_y;
            this.label2.Location = new System.Drawing.Point(button_x, button_y);
            this.label2.Size = new System.Drawing.Size(button_width, button_height);
            this.label2.Font = this.label1.Font;

            button_y += delta_y;
            this.listBox2.Location = new System.Drawing.Point(button_x, button_y);
            this.listBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Bold);
            this.listBox2.Size = new System.Drawing.Size(button_width, button_height * 5);
            this.listBox2.Font = this.label1.Font;

            this.textBox3.Location = new System.Drawing.Point(button_x + button_width + Constants.CONTROL_SPACING, button_y);
            this.textBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Bold);
            this.textBox3.Size = new System.Drawing.Size(button_width, button_height * 5);
            this.textBox3.Font = this.textBox2.Font;
            
            button_y += 5 * delta_y;
            this.label3.Location = new System.Drawing.Point(button_x, button_y);
            this.label3.Size = new System.Drawing.Size(button_width, button_height);
            this.label3.Font = this.label1.Font;
            button_y += delta_y;

            this.textBox1.Location = new System.Drawing.Point(button_x, button_y);
            this.textBox1.Size = new System.Drawing.Size(button_width+((int)(0.5*button_width)), button_height);
            this.textBox1.Font = this.label1.Font;

            this.button1.Location = new System.Drawing.Point(button_x + button_width + Constants.CONTROL_SPACING + ((int)(0.5 * button_width)), button_y);
            this.button1.Size = new System.Drawing.Size(((int)(button_width*0.5)), button_height);
            this.button1.Font = this.label1.Font;

            button_y += delta_y;

            this.button2.Location = new System.Drawing.Point(button_x, button_y);
            this.button2.Size = new System.Drawing.Size(button_width, button_height);
            this.button2.Font = this.label1.Font;


            this.button3.Location = new System.Drawing.Point(button_x + button_width + Constants.CONTROL_SPACING, button_y);
            this.button3.Size = new System.Drawing.Size(button_width, button_height);
            this.button3.Font = this.label1.Font;

            this.Resize += new EventHandler(OnResize);
           

        }

  

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.listBox2 = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(12, 27);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(89, 82);
            this.listBox1.TabIndex = 0;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // listBox2
            // 
            this.listBox2.FormattingEnabled = true;
            this.listBox2.Location = new System.Drawing.Point(12, 128);
            this.listBox2.Name = "listBox2";
            this.listBox2.Size = new System.Drawing.Size(89, 95);
            this.listBox2.TabIndex = 1;
            this.listBox2.SelectedIndexChanged += new System.EventHandler(this.listBox2_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(106, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Activity Protocols";
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 112);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Sensor Sets";
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(15, 230);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Data Directory";
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.Color.White;
            this.textBox1.Location = new System.Drawing.Point(18, 247);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(193, 20);
            this.textBox1.TabIndex = 5;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(213, 246);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(51, 23);
            this.button1.TabIndex = 6;
            this.button1.Text = "Browse";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.Green;
            this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.Location = new System.Drawing.Point(43, 273);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 7;
            this.button2.Text = "Start";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.Yellow;
            this.button3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button3.Location = new System.Drawing.Point(136, 273);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 8;
            this.button3.Text = "Cancel";
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // textBox2
            // 
            this.textBox2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox2.Location = new System.Drawing.Point(142, 27);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.Size = new System.Drawing.Size(122, 82);
            this.textBox2.TabIndex = 9;
            this.textBox2.Text = "Choose an activity protocol.";
            // 
            // textBox3
            // 
            this.textBox3.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox3.Location = new System.Drawing.Point(142, 128);
            this.textBox3.Multiline = true;
            this.textBox3.Name = "textBox3";
            this.textBox3.ReadOnly = true;
            this.textBox3.Size = new System.Drawing.Size(122, 95);
            this.textBox3.TabIndex = 10;
            this.textBox3.Text = "Choose a sensor set.";
            // 
            // SessionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(270, 302);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listBox2);
            this.Controls.Add(this.listBox1);
            this.Name = "SessionForm";
            this.Text = "Data Collection Setup";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.ListBox listBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private TextBox textBox2;
        private TextBox textBox3;
    }
}