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
        private int num_sensors1;
        private int num_sensors2;
        private bool isHR;

        public SensorAnnotation(){
            this.sensors = new ArrayList();
            this.num_sensors1 = 0;
            this.num_sensors2 = 0;
            this.isHR = false;
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

        public int SensorCount1
        {
            get
            {
                return this.num_sensors1;
            }
            set
            {
                this.num_sensors1 = value;
            }
        }

        public int SensorCount2
        {
            get
            {
                return this.num_sensors2;
            }
            set
            {
                this.num_sensors2 = value;
            }
        }
     
    }
}
