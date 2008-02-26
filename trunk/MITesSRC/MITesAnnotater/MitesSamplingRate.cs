using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using HousenCS.MITes;
using SXML;

namespace MitesAnnotater
{
    public partial class MitesSamplingRate : Form
    {
        private MITesDataFilterer aMITesDataFilterer;
        private SensorAnnotation sensorConfiguration;


        public MitesSamplingRate()
        {
            InitializeComponent();
            InitializeInterface();
        }

        public MitesSamplingRate(MITesDataFilterer aMITesDataFilterer,SensorAnnotation sensorConfiguration)
        {
            InitializeComponent();            
            this.aMITesDataFilterer = aMITesDataFilterer;
            this.sensorConfiguration = sensorConfiguration;
            InitializeInterface();

        }

        private void MitesSamplingRate_Load(object sender, EventArgs e)
        {

        }
        public void updateInterface()
        {
            // The loop goes through each sensor specified in sensor.xml and calculates received samples/expected samples * 100%
            foreach (SXML.Sensor sensor in this.sensorConfiguration.Sensors)
            {
                int sensor_id = Convert.ToInt32(sensor.ID);
                double rate = ((double)this.aMITesDataFilterer.MitesPerformanceTracker[sensor_id].PreviousCounter / (double)this.aMITesDataFilterer.MitesPerformanceTracker[sensor_id].GoodRate) * 100;
                if (rate > 100)
                    rate = 100;
                else if (rate < 0)
                    rate = 0;
                this.labels[sensor_id].Text = rate.ToString("00.00") + "%";
            }
        }
    }
}