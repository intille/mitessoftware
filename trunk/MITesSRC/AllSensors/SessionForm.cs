using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace MITesLogger_PC
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    delegate void SetTextCallback(string label, int control_id);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="activityProtocol"></param>
    /// <param name="sensorConfiguration"></param>
    /// <param name="dataDirectory"></param>
    public delegate void StartMITesReceiversDelegate(AXML.Protocol activityProtocol, SXML.Configuration sensorConfiguration, string dataDirectory);
    /// <summary>
    /// 
    /// </summary>
    public partial class SessionForm : Form
    {
        /// <summary>
        /// 
        /// </summary>
        public StartMITesReceiversDelegate StartMITesReceiversCallback;

        private SXML.Configurations sensorConfigurations;
        private AXML.Protocols activityProtocols;

        /// <summary>
        /// 
        /// </summary>
        public SessionForm()
        {
            
            InitializeComponent();
            InitializeInterface();

        }

 

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.textBox2.Text = ((AXML.Protocol)this.activityProtocols.ActivityProtocols[this.listBox1.SelectedIndex]).Description;
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.textBox3.Text = ((SXML.Configuration)this.sensorConfigurations.SensorConfigurations[this.listBox2.SelectedIndex]).Description;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderSelect = new System.Windows.Forms.FolderBrowserDialog();
            folderSelect.Description = "Select your data directory.";
            System.Windows.Forms.DialogResult result = folderSelect.ShowDialog();
            if (Directory.Exists(folderSelect.SelectedPath))
                this.textBox1.Text = folderSelect.SelectedPath;
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!(Directory.Exists(this.textBox1.Text)) || (this.listBox1.SelectedItems.Count == 0) ||
                (this.listBox2.SelectedItems.Count == 0))
            {
                MessageBox.Show("Please select an activity protocol, a sensor configuration and a directory where you would like to store your data.");
            }
            else //closed successfully
            {
                
                this.StartMITesReceiversCallback(((AXML.Protocol)this.activityProtocols.ActivityProtocols[this.listBox1.SelectedIndex]), ((SXML.Configuration)this.sensorConfigurations.SensorConfigurations[this.listBox2.SelectedIndex]), this.textBox1.Text);
                this.Close();
                //this.parent.InitializeInterface();
                
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="ee"></param>
        private void OnResize(object sender, EventArgs ee)
        {
            //to prevent resizing when minimizing
            if ((this.ClientSize.Width < Constants.MINIMUM_WINDOW_WIDTH) || (this.ClientSize.Height < Constants.MINIMUM_WINDOW_HEIGHT))
                return;
            string longest_description = this.activityProtocols.LongestLabel;
            if (this.sensorConfigurations.LongestDescription.Length > longest_description.Length)
                longest_description = this.sensorConfigurations.LongestDescription;


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
            this.listBox1.Size = new System.Drawing.Size(button_width, button_height * 5);
            this.listBox1.Font = this.label1.Font;

            this.textBox2.Location = new System.Drawing.Point(button_x + button_width + Constants.CONTROL_SPACING, button_y);
            this.textBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Bold);
            this.textBox2.Size = new System.Drawing.Size(button_width, button_height * 5);
            this.textBox2.Font = CalculateBestFitFont(this.textBox2.CreateGraphics(), Constants.MIN_FONT, Constants.MAX_FONT,
                this.textBox2.ClientSize, this.activityProtocols.LongestDescription, this.textBox2.Font, (float)0.9, (float)0.75);


            button_y += 5 * delta_y;
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
            this.textBox1.Size = new System.Drawing.Size(button_width, button_height);
            this.textBox1.Font = this.label1.Font;

            this.button1.Location = new System.Drawing.Point(button_x + button_width + Constants.CONTROL_SPACING, button_y);
            this.button1.Size = new System.Drawing.Size(button_width, button_height);
            this.button1.Font = this.label1.Font;

            button_y += delta_y;

            this.button2.Location = new System.Drawing.Point(button_x, button_y);
            this.button2.Size = new System.Drawing.Size(button_width, button_height);
            this.button2.Font = this.label1.Font;


            this.button3.Location = new System.Drawing.Point(button_x + button_width + Constants.CONTROL_SPACING, button_y);
            this.button3.Size = new System.Drawing.Size(button_width, button_height);
            this.button3.Font = this.label1.Font;

        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}