using MITesDataCollection.Utils;
using System.Drawing;
using SXML;

namespace MITesDataCollection
{
    partial class SensorConfigurationForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.MainMenu mainMenu1;

        private void InitializeInterface()
        {
            this.Width = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width - Constants.SCREEN_WIDTH_MARGIN;
            this.Height = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height - Constants.SCREEN_HEIGHT_MARGIN;
            if ((this.Width > Constants.MAX_SCREEN_WIDTH) || (this.Height > Constants.MAX_SCREEN_HEIGHT))
            {
                this.Width = this.Width / 2;
                this.Height = this.Height / 2;
            }

            double drawableWidth = this.Width - Constants.SCREEN_LEFT_MARGIN - Constants.SCREEN_RIGHT_MARGIN;
            double drawableHeight = this.Height - Constants.SCREEN_TOP_MARGIN - Constants.SCREEN_BOTTOM_MARGIN - 4 * Constants.WIDGET_SPACING;

            this.button1.Enabled = false;
    

            //adjust top label size and location
            this.label1.Text = Constants.SENSOR_FORM_LABEL1;
            this.label1.Width = (int)drawableWidth;
            this.label1.Height = (int)(drawableHeight * Constants.SENSOR_LABEL_PERCENTAGE);
            this.label1.Location = new Point((int)Constants.SCREEN_LEFT_MARGIN, (int)Constants.SCREEN_TOP_MARGIN);
            this.label1.Font = GUI.CalculateBestFitFont(this.label1.Parent.CreateGraphics(), Constants.MIN_FONT,
                   Constants.MAX_FONT, this.label1.Size, Constants.SENSOR_FORM_LABEL1, this.label1.Font, (float)0.9, (float)0.9);

            //Load the activity protocols from the master directory
            ConfigurationReader creader = new ConfigurationReader(Constants.MASTER_DIRECTORY);
            string longest_label = "";
            if (creader.validate())
            {
                configurations = creader.parse();
                foreach (Configuration configuration in configurations.SensorConfigurations)
                {
                    this.listBox1.Items.Add(configuration.Name);
                    if (longest_label.Length < configuration.Name.Length)
                        longest_label = configuration.Name;
                }

                //Listbox dynamic placement
                this.listBox1.Width = (int)(drawableWidth * Constants.SENSOR_LIST_WIDTH_PERCENTAGE);
                this.listBox1.Height = (int)(drawableHeight * Constants.SENSOR_LIST_HEIGHT_PERCENTAGE);
                Font newFont = GUI.CalculateBestFitFont(this.listBox1.Parent.CreateGraphics(), Constants.MIN_FONT,
                   Constants.MAX_FONT, this.listBox1.Size, longest_label, this.listBox1.Font, (float)0.9, (float)0.9);
                this.listBox1.Font = new Font(Constants.FONT_FAMILY, newFont.Size, this.Font.Style);
                this.listBox1.Location = new Point((int)Constants.SCREEN_LEFT_MARGIN, (int)this.label1.Location.Y + this.label1.Height + Constants.WIDGET_SPACING);

                // buttons

                this.button1.Width = (int)(drawableWidth * Constants.ACTIVITY_BUTTONS_WIDTH_PERCENTAGE);
                this.button2.Width = (int)(drawableWidth * Constants.ACTIVITY_BUTTONS_WIDTH_PERCENTAGE);
                this.button1.Height = (int)(drawableHeight * Constants.ACTIVITY_BUTTONS_HEIGHT_PERCENTAGE);
                this.button2.Height = (int)(drawableHeight * Constants.ACTIVITY_BUTTONS_HEIGHT_PERCENTAGE);

                this.button2.Location = new Point((int)Constants.SCREEN_LEFT_MARGIN, (int)(this.listBox1.Location.Y + this.listBox1.Height + Constants.WIDGET_SPACING));
                this.button1.Location = new Point((int)(Constants.SCREEN_LEFT_MARGIN + this.button1.Width + Constants.WIDGET_SPACING),
                    (int)(this.listBox1.Location.Y + this.listBox1.Height + Constants.WIDGET_SPACING));
            }

            this.button2.Font = this.button1.Font = GUI.CalculateBestFitFont(this.button1.Parent.CreateGraphics(), Constants.MIN_FONT,
                    Constants.MAX_FONT, this.button1.Size, "Next", this.button1.Font, (float)0.9, (float)0.9);

        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected void Dispose(bool disposing)
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
            this.mainMenu1 = new System.Windows.Forms.MainMenu();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.Add(this.menuItem1);
            
            //.AddRange(new System.Windows.Forms.MenuItem[] {
            //this.menuItem1});
            // 
            // menuItem1
            // 
            //this.menuItem1.Index = 0;
            this.menuItem1.Text = "Quit";
            this.menuItem1.Click += new System.EventHandler(this.menuItem1_Click);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(3, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(205, 20);
            this.label1.TabIndex = 4;
            this.label1.Text = "Choose a sensor configuration:";
            // 
            // listBox1
            // 
            this.listBox1.Location = new System.Drawing.Point(64, 33);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(100, 95);
            this.listBox1.TabIndex = 1;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(136, 153);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(72, 20);
            this.button1.TabIndex = 2;
            this.button1.Text = "Next";
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(17, 153);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(72, 20);
            this.button2.TabIndex = 3;
            this.button2.Text = "Back";
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // SensorConfigurationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.label1);
            this.Menu = this.mainMenu1;
            this.Name = "SensorConfigurationForm";
            this.Text = "Collect Data ...";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.MenuItem menuItem1;
    }
}