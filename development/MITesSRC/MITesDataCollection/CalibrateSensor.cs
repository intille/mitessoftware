using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using SXML;

namespace MITesDataCollection
{
    public partial class CalibrateSensor : Form, ChainnedForm
    {
        private int sensorIndex = 0;
        private Form nextForm;
        private Form previousForm;

        public CalibrateSensor()
        {
            InitializeComponent();


        }

        public void Initialize()
        {
            if (CalibrateSensors.Sensors.Sensors.Count == 0)
            {
               MessageBox.Show("You did not choose any sensors to calibrate...Exiting!");
               Application.Exit();
            }
            else            
               this.label3.Text = "Enter the information for Sensor" + ((SXML.Sensor)CalibrateSensors.Sensors.Sensors[sensorIndex]).ID;
            for (int i = 0; (i < Constants.MAX_CONTROLLERS); i++)
            {
                this.comboBox1.Items.Add(i.ToString());
            }
        }
        public Form PreviousForm
        {
            set
            {
                this.previousForm = value;
            }
        }

        public Form NextForm
        {
            set
            {
                this.nextForm = value;
            }

        }
    
        public void Cleanup()
        {
            ((ChainnedForm)this.previousForm).Cleanup();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if ((this.textBox1.Text.Length > 0) && (this.comboBox1.SelectedIndex >= 0))
            {
                ((Sensor)CalibrateSensors.Sensors.Sensors[sensorIndex]).Receiver = this.comboBox1.SelectedIndex.ToString();
                CalibrateSensors.Sensors.NumberSensors[this.comboBox1.SelectedIndex] = CalibrateSensors.Sensors.NumberSensors[this.comboBox1.SelectedIndex] + 1;
                if (CalibrateSensors.Sensors.TotalReceivers < (this.comboBox1.SelectedIndex + 1))
                    CalibrateSensors.Sensors.TotalReceivers = this.comboBox1.SelectedIndex + 1;
                ((Sensor)CalibrateSensors.Sensors.Sensors[sensorIndex]).Location = this.textBox1.Text;
                sensorIndex++;
                if (sensorIndex == CalibrateSensors.Sensors.Sensors.Count) //start calibration
                {
                    TextWriter tw = new StreamWriter("SensorData.xml");
                    tw.WriteLine(CalibrateSensors.Sensors.toXML());
                    tw.Close();
                    Cleanup();
                }
                else
                {
                    this.label3.Text = "Enter the information for Sensor" + ((SXML.Sensor)CalibrateSensors.Sensors.Sensors[sensorIndex]).ID;
                    this.textBox1.Text = "";
                    this.comboBox1.SelectedIndex = -1;
                }
            }
            else
                MessageBox.Show("Please enter a location and select a receiver.");

        }
    }
}