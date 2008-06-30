using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace SXML
{
    public class SensorAnnotation
    {
        private string dataset;
        private ArrayList sensors;
        //private int num_sensors1;
        //private int num_sensors2;
        private int[] num_sensors;
        private int totalReceivers;
        private bool isHR;
        private int firstAccelerometerID;
        private int maximumSensorID;
        private Hashtable sensorsIndex;

        public SensorAnnotation(int maxReceivers)
        {
            this.sensors = new ArrayList();
            this.sensorsIndex = new Hashtable();
            //this.num_sensors1 = 0;
            //this.num_sensors2 = 0;
            this.isHR = false;
            this.num_sensors = new int[maxReceivers];
            this.totalReceivers=0;
            this.firstAccelerometerID = -1;
            this.maximumSensorID = -1;
        }

        public Hashtable SensorsIndex
        {
            get
            {
                return this.sensorsIndex;
            }
        }
        public int MaximumSensorID
        {
            get
            {
                return this.maximumSensorID;
            }
            set
            {
                this.maximumSensorID = value;
            }
        }
        public int FirstAccelerometer
        {
            get
            {
                return this.firstAccelerometerID;
            }
            set
            {
                this.firstAccelerometerID = value;
            }
        }
        public bool IsHR
        {
            get
            {
                return this.isHR;
            }
            set
            {
                this.isHR = value;
            }
        }

        public string Dataset{
            get{
                return this.dataset;
            }
            set
            {
                this.dataset = value;
            }
        }

        public ArrayList Sensors
        {
            get
            {
                return this.sensors;
            }
        }

        public int TotalReceivers
        {
            get
            {
                return this.totalReceivers;
            }
            set
            {
                this.totalReceivers = value;
            }
        }

        public int[] NumberSensors
        {
            get
            {
                return this.num_sensors;
            }
        }

        public int GetNumberSensors(int receiver_id)
        {
            if ((receiver_id < this.num_sensors.Length) && (receiver_id >= 0))
                return this.num_sensors[receiver_id];
            else
                return -1;

        }

        public string toXML()
        {
            string xml = "<"+Constants.SENSORDATA_ELEMENT+" "+
                Constants.DATASET_ATTRIBUTE+"=\""+this.dataset+"\" xmlns=\"urn:mites-schema\">\n";
            foreach (Sensor sensor in this.sensors)            
                xml += sensor.toXML();
            xml += "</" + Constants.SENSORDATA_ELEMENT + ">\n";

            return xml;
        }
     
    }
}
