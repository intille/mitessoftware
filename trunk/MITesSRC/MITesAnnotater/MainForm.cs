using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

using AXML;
using SXML;
//using ATones;
using HousenCS.MITes;

namespace MitesAnnotater
{
    public partial class MainForm : Form, ControlCreator
    {

        Annotation annotation;
        SensorAnnotation sensors;
        AnnotatedRecord currentRecord;
        MITesDataFilterer aMITesDataFilterer;
        MitesSamplingRate mitesSamplingRates;

        private const int MINIMUM_WINDOW_HEIGHT = 30;
        private const int MINIMUM_WINDOW_WIDTH = 30;

        public MainForm(string dataDirectory, string sourceDirectory)
        {
            
            InitializeComponent();
            
            AXML.Reader reader = new AXML.Reader(sourceDirectory,this);
            if (reader.validate() == false)
            {
                throw new Exception("Error Code 0: XML format error - activities.xml does not match activities.xsd!");               
            }
            else
            {
                this.annotation = reader.parse();
                this.annotation.DataDirectory = dataDirectory;


                SXML.Reader sreader = new SXML.Reader(sourceDirectory);
                if (sreader.validate() == false)
                {
                    throw new Exception("Error Code 0: XML format error - sensors.xml does not match sensors.xsd!");                  
                }
                else
                {
                    this.sensors = sreader.parse();
                    InitializeTimers();
                    InitializeSound();
                    InitializeInterface();
                }
            }
            
        }

        // This method stores a reference to the data filterer for the MITes receiver to
        // individual sensor performance metrics
        public void SetDataFilterer(MITesDataFilterer aMITesDataFilterer)
        {
            this.aMITesDataFilterer = aMITesDataFilterer;
        }

        // This returns the sensor objects in sensors.xml
        public SensorAnnotation SensorConfiguration
        {
            get
            {
                return this.sensors;
            }
        }
        private void roundButton1_Click(object sender, EventArgs e)
        {

            Button button = (Button)sender;
            

            //button state is now start
            if (button.BackColor == System.Drawing.Color.Green)
            {
                this.startSound.Play();
                //Generator generator = new Generator();
                //generator.InitializeSound(this.Handle.ToInt32());
                //generator.CreateBuffer();

                this.startStopButton.Text = "Stop";
                this.startStopButton.BackColor = System.Drawing.Color.Red;
                this.overallTimer.reset();
                this.overallTimer.start();
                this.goodTimer.reset();
                this.goodTimer.start();
                


                //store the current state of the categories
                this.currentRecord = new AnnotatedRecord();
                this.currentRecord.StartDate = DateTime.Now.ToString("MM-dd-yyyy");
                this.currentRecord.StartHour = DateTime.Now.Hour;
                this.currentRecord.StartMinute = DateTime.Now.Minute;
                this.currentRecord.StartSecond = DateTime.Now.Second;
                TimeSpan ts = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
                this.currentRecord.StartUnix = ts.TotalSeconds;

                //check all buttons values, store them and disable them
                foreach (Button category_button in categoryButtons)
                {
                    int button_id=Convert.ToInt32(category_button.Name);
                    Category category=(Category)this.annotation.Categories[button_id];
                    string current_label=((AXML.Label)category.Labels[(int)this.buttonIndex[button_id]]).Name;
                    this.currentRecord.Labels.Add(new AXML.Label(current_label,category.Name));
                    category_button.Enabled = false;
                }

                
                

            }
                //button state is now stop

            else if (button.BackColor == System.Drawing.Color.Red)
            {
                this.stopSound.Play();
                this.startStopButton.Text="Start";
                this.startStopButton.BackColor = System.Drawing.Color.Green;
                this.overallTimer.reset();
                this.goodTimer.reset();

                //store the current state of the categories
                this.currentRecord.EndDate = DateTime.Now.ToString("MM-dd-yyyy");
                this.currentRecord.EndHour = DateTime.Now.Hour;
                this.currentRecord.EndMinute = DateTime.Now.Minute;
                this.currentRecord.EndSecond = DateTime.Now.Second;
                TimeSpan ts = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
                this.currentRecord.EndUnix = ts.TotalSeconds;
                this.annotation.Data.Add(this.currentRecord);

                //each time an activity is stopped, rewrite the file on disk, need to backup file to avoid corruption
                this.annotation.ToXMLFile();
                this.annotation.ToCSVFile();

                foreach (Button category_button in categoryButtons)
                {
                    category_button.Enabled = true;
                }
                
            }
        }

        private void roundButton2_Click(object sender, EventArgs e)
        {
            this.resetSound.Play();
            this.startStopButton.Text = "Start";
            this.startStopButton.BackColor = System.Drawing.Color.Green;
            //this.overallTimer.stop();
            this.overallTimer.reset();
            this.goodTimer.reset();            
          
            foreach (Button category_button in categoryButtons)
            {
                int button_id = Convert.ToInt32(category_button.Name);
                Category category = (Category)this.annotation.Categories[button_id];
                this.buttonIndex[button_id] = 0;
                category_button.Text = category.Name + " : " + ((AXML.Label)category.Labels[0]).Name;
                category_button.Enabled = true;
            }
        }

        

        private void button_Click(object sender, EventArgs e)
        {            
            Button button = (Button)sender;
            int button_id = Convert.ToInt32(button.Name);
            Category category=(Category)this.annotation.Categories[button_id];
            int nextIndex = ((int)this.buttonIndex[button_id] + 1)% category.Labels.Count;
            //this.clickSound.Play();
            button.Text = category.Name + " : " + ((AXML.Label)category.Labels[nextIndex]).Name;
            //((AXML.Label)category.Labels[nextIndex]).PlayTone();
            this.buttonIndex[button_id] = nextIndex;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            if (File.Exists(this.annotation.DataDirectory+"\\"+this.annotation.OutputFile)){
                System.Diagnostics.Process.Start("iexplore.exe", this.annotation.DataDirectory + "\\" + this.annotation.OutputFile);
            }
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            if (File.Exists(this.annotation.DataDirectory+"\\"+this.annotation.OutputCSVFile))
            {
                //System.Diagnostics.Process.Start("excel.exe", this.annotation.DataDirectory + "\\" + this.annotation.OutputCSVFile);
            }
        }

        // This method is used to update the interface of the MITes Sampling form with new performance
        // metrics. This is called every second from the MITes receiver polling loop
        public void updateMitesSampling()
        {
            if ((this.mitesSamplingRates != null) && !this.mitesSamplingRates.IsDisposed)
            {
                this.mitesSamplingRates.updateInterface();
            }
        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if ((this.mitesSamplingRates == null) || this.mitesSamplingRates.IsDisposed)
            {
                this.mitesSamplingRates = new MitesSamplingRate(this.aMITesDataFilterer, this.sensors);
                this.mitesSamplingRates.Show();                
            }
            
        }

        private void OnResize(object sender, EventArgs ee)
        {
            //to prevent resizing when minimizing
            if ((this.ClientSize.Width < MINIMUM_WINDOW_WIDTH) || (this.ClientSize.Height< MINIMUM_WINDOW_HEIGHT))
                return;
            int button_width = this.ClientSize.Width - LEFT_MARGIN - RIGHT_MARGIN;
            int button_height = (this.ClientSize.Height - TOP_MARGIN - BOTTOM_MARGIN - (this.annotation.Categories.Count * CONTROL_SPACING)) / (this.annotation.Categories.Count + 1);
            int button_x = LEFT_MARGIN;
            int button_y = TOP_MARGIN;

            int delta_y = button_height + CONTROL_SPACING;
            int button_id = 0;


            foreach (System.Windows.Forms.Button button in categoryButtons)
            {
                button.Location = new System.Drawing.Point(button_x, button_y + button_id * delta_y);
                button.Font = CalculateBestFitFont(button.CreateGraphics(), MIN_FONT, MAX_FONT,
                button.ClientSize, this.longest_label, button.Font, (float)0.9, (float)0.75);
                button.Size = new System.Drawing.Size(button_width, button_height);        
                button_id++;
            }

            //adjust round buttons start/stop -reset
            button_width = (this.Size.Width - LEFT_MARGIN - RIGHT_MARGIN - 20) / 2;
            this.startStopButton.Size = new System.Drawing.Size(button_width, button_height);
            this.resetButton.Size = new System.Drawing.Size(button_width, button_height);
            this.startStopButton.Location = new System.Drawing.Point(LEFT_MARGIN, button_y + button_id * delta_y);
            this.resetButton.Location = new System.Drawing.Point(this.startStopButton.Location.X + this.startStopButton.Size.Width + CONTROL_SPACING, button_y + button_id * delta_y);
            this.startStopButton.Font = CalculateBestFitFont(this.startStopButton.CreateGraphics(), MIN_FONT, MAX_FONT,
            this.startStopButton.ClientSize, "Start", this.startStopButton.Font, (float)0.9, (float)0.75);
            this.resetButton.Font = CalculateBestFitFont(this.resetButton.CreateGraphics(), MIN_FONT, MAX_FONT,
            this.resetButton.ClientSize, "Reset", this.resetButton.Font, (float)0.9, (float)0.75);

        }
    }
}