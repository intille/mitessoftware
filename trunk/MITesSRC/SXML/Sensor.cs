using System;
using System.Collections.Generic;
using System.Text;

namespace SXML
{
    public class Sensor
    {
        private string sensor_class;
        private string type;
        private string id;
        private string sensor_object;
        private string location;
        private string description;
        private string receiver;
        private string display_type;
        private string display_x;
        private string display_y;
        private string color_on;
        private string color_off;
        private double xmean;
        private double xstd;
        private double ymean;
        private double ystd;
        private double zmean;
        private double zstd;


        public Sensor()
        {
        }

        public string SensorClass
        {
            get
            {
                return this.sensor_class;
            }
            set
            {
                this.sensor_class = value;
            }
        }

        public string Type
        {
            get
            {
                return this.type;
            }

            set
            {
                this.type = value;
            }
        }

        public string ID
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

        public string SensorObject
        {
            get
            {
                return this.sensor_object;
            }

            set
            {
                this.sensor_object = value;
            }
        }

        public string Receiver
        {
            get
            {
                return this.receiver;
            }
            set
            {
                this.receiver = value;
            }
        }

        public string Location
        {
            get
            {
                return this.location;
            }
            set
            {
                this.location = value;
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

        public string DisplayType
        {
            get
            {
                return this.display_type;
            }
            set
            {
                this.display_type = value;
            }
        }

        public string DisplayX
        {
            get
            {
                return this.display_x;
            }
            set
            {
                this.display_x = value;
            }
        }

        public string DisplayY
        {
            get
            {
                return this.display_y;
            }
            set
            {
                this.display_y = value;
            }
        }

        public string ColorOn
        {

            get
            {
                return this.color_on;
            }
            set
            {
                this.color_on = value;
            }
        }

        public string ColorOff
        {
            get
            {
                return this.color_off;
            }
            set
            {
                this.color_off = value;
            }
        }

        public double XMean
        {
            get
            {
                return this.xmean;
            }
            set
            {
                this.xmean = value;
            }
        }

        public double YMean
        {
            get
            {
                return this.ymean;
            }
            set
            {
                this.ymean = value;
            }
        }

        public double ZMean
        {
            get
            {
                return this.zmean;
            }
            set
            {
                this.zmean = value;
            }
        }


        public double XStd
        {
            get
            {
                return this.xstd;
            }
            set
            {
                this.xstd = value;
            }
        }

        public double YStd
        {
            get
            {
                return this.ystd;
            }
            set
            {
                this.ystd = value;
            }
        }

        public double ZStd
        {
            get
            {
                return this.zstd;
            }
            set
            {
                this.zstd = value;
            }
        }
    }
}
