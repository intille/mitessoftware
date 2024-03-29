using System;
using System.Collections.Generic;
using System.Text;

namespace PhoneAccelerometers
{
    public class SensorData
    {
        private int channel_id;
        private int sub_id;
        private int x, y, z;
        private int timestamp;
        private double unixtimestamp;
        private int maximumSamplingRate;

        public SensorData()
        {
            this.x = 0;
            this.y = 0;
            this.z = 0;
            this.timestamp = 0;
            this.unixtimestamp = 0;
            this.sub_id = 0;
        }

        public int[] toArray()
        {
            int[] data = new int[3];
            data[0] = this.x;
            data[1] = this.y;
            data[2] = this.z;
            return data;
        }


        public int ChannelID
        {
            get
            {
                return this.channel_id;
            }
            set
            {
                this.channel_id = value;
            }
        }

        public int SubID
        {
            get
            {
                return this.sub_id;
            }

            set
            {
                this.sub_id = value;
            }
        }
        public int Timestamp
        {
            get
            {
                return this.timestamp;
            }
            set
            {
                this.timestamp = value;
            }
        }

        public double Unixtimestamp
        {
            get
            {
                return this.unixtimestamp;
            }

            set
            {
                this.unixtimestamp = value;
            }
        }


        public int X
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

        public int Y
        {
            get
            {
                return this.y;
            }
            set
            {
                this.y = value;
            }
        }

        public int Z
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
