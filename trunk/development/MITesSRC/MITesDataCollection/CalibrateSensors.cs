using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using SXML;

namespace MITesDataCollection
{
    public partial class CalibrateSensors : Form, ChainnedForm
    {
        private Form nextForm;
        private Form previousForm;
        //private ArrayList channels;
        private static SensorAnnotation sensors;

        public CalibrateSensors()
        {
            InitializeComponent();
            //this.channels = new ArrayList();
            sensors = new SensorAnnotation(Constants.MAX_CONTROLLERS);

        }

        public static SensorAnnotation Sensors
        {
            get
            {
                return sensors;
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
        public void Initialize()
        {
           // InitializeInterface();
        }
        public void Cleanup()
        {
            ((ChainnedForm)this.previousForm).Cleanup();
        }

        private void CalibrateSensors_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.checkBox1.Checked == true)
            {
                SXML.Sensor sensor = new Sensor();
                sensor.ID = Constants.ACCEL_ID1.ToString();
                if (Convert.ToInt32(sensor.ID) > sensors.MaximumSensorID)
                    sensors.MaximumSensorID = Convert.ToInt32(sensor.ID);
                if (sensors.FirstAccelerometer == -1)
                    sensors.FirstAccelerometer = Convert.ToInt32(sensor.ID);
                sensors.Sensors.Add(sensor);
                sensors.SensorsIndex[sensor.ID] = sensors.Sensors.IndexOf(sensor);
            }
            if (this.checkBox2.Checked == true)
            {
                SXML.Sensor sensor = new Sensor();
                sensor.ID = Constants.ACCEL_ID2.ToString();
                if (Convert.ToInt32(sensor.ID) > sensors.MaximumSensorID)
                    sensors.MaximumSensorID = Convert.ToInt32(sensor.ID);
                if (sensors.FirstAccelerometer == -1)
                    sensors.FirstAccelerometer = Convert.ToInt32(sensor.ID);
                sensors.Sensors.Add(sensor);
                sensors.SensorsIndex[sensor.ID] = sensors.Sensors.IndexOf(sensor);
            }
            if (this.checkBox3.Checked == true)
            {
                SXML.Sensor sensor = new Sensor();
                sensor.ID = Constants.ACCEL_ID3.ToString();
                if (Convert.ToInt32(sensor.ID) > sensors.MaximumSensorID)
                    sensors.MaximumSensorID = Convert.ToInt32(sensor.ID);
                if (sensors.FirstAccelerometer == -1)
                    sensors.FirstAccelerometer = Convert.ToInt32(sensor.ID);
                sensors.Sensors.Add(sensor);
                sensors.SensorsIndex[sensor.ID] = sensors.Sensors.IndexOf(sensor);
            }
            if (this.checkBox4.Checked == true)
            {
                SXML.Sensor sensor = new Sensor();
                sensor.ID = Constants.ACCEL_ID4.ToString();
                if (Convert.ToInt32(sensor.ID) > sensors.MaximumSensorID)
                    sensors.MaximumSensorID = Convert.ToInt32(sensor.ID);
                if (sensors.FirstAccelerometer == -1)
                    sensors.FirstAccelerometer = Convert.ToInt32(sensor.ID);
                sensors.Sensors.Add(sensor);
                sensors.SensorsIndex[sensor.ID] = sensors.Sensors.IndexOf(sensor);
            }
            if (this.checkBox5.Checked == true)
            {
                SXML.Sensor sensor = new Sensor();
                sensor.ID = Constants.ACCEL_ID5.ToString();
                if (Convert.ToInt32(sensor.ID) > sensors.MaximumSensorID)
                    sensors.MaximumSensorID = Convert.ToInt32(sensor.ID);
                sensors.Sensors.Add(sensor);
                sensors.SensorsIndex[sensor.ID] = sensors.Sensors.IndexOf(sensor);
            }
            if (this.checkBox6.Checked == true)
            {
                SXML.Sensor sensor = new Sensor();
                sensor.ID = Constants.ACCEL_ID6.ToString();
                if (Convert.ToInt32(sensor.ID) > sensors.MaximumSensorID)
                    sensors.MaximumSensorID = Convert.ToInt32(sensor.ID);
                if (sensors.FirstAccelerometer == -1)
                    sensors.FirstAccelerometer = Convert.ToInt32(sensor.ID);
                sensors.Sensors.Add(sensor);
                sensors.SensorsIndex[sensor.ID] = sensors.Sensors.IndexOf(sensor);
            }
            if (this.checkBox7.Checked == true)
            {
                SXML.Sensor sensor = new Sensor();
                sensor.ID = Constants.ACCEL_ID7.ToString();
                if (Convert.ToInt32(sensor.ID) > sensors.MaximumSensorID)
                    sensors.MaximumSensorID = Convert.ToInt32(sensor.ID);
                if (sensors.FirstAccelerometer == -1)
                    sensors.FirstAccelerometer = Convert.ToInt32(sensor.ID);
                sensors.Sensors.Add(sensor);
                sensors.SensorsIndex[sensor.ID] = sensors.Sensors.IndexOf(sensor);
            }

            ((ChainnedForm)this.nextForm).Initialize();
            this.nextForm.Visible = true;
            this.Visible = false; 
            //CalibrateSensor calibrateSensor = new CalibrateSensor();
            //calibrateSensor.Show();
            //this.Visible = false;
        }
    }
}