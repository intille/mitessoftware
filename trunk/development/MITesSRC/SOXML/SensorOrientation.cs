using System;
using System.Collections.Generic;
using System.Text;

namespace SOXML
{
    public class SensorOrientation
    {
        private int id;
        private string description;
        private string imageFile;
        private double x;
        private double y;
        private double z;
        private SensorAcceleration[] accelerations;

        public SensorOrientation()
        {
            this.accelerations = new SensorAcceleration[Constants.ACCELERATION_DIRECTIONS];
        }

        public SensorAcceleration[] Accelerations
        {
            get
            {
                return this.accelerations;
            }
        }
        public int ID
        {
            get
            {
                return this.id;
            }

            set
            {
                this.id = value;
            }
        }

        public string Description
        {
            get
            {
                return this.description;
            }

            set
            {
                this.description = value;
            }
        }

        public string ImageFile
        {
            get
            {
                return this.imageFile;
            }

            set
            {
                this.imageFile = value;
            }
        }
        public double X 
        {
            get
            {
                return this.x;
            }

            set
            {
                this.x = value;
            }
        }

        public double Y
        {
            get
            {
                return this.y;
            }
            set
            {
                this.y=value;
            }
        }

        public double Z
        {
            get
            {
                return this.z;
            }
            set
            {
                this.z = value;
            }
        }
    }
}
