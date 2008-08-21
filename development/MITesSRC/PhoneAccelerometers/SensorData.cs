using System;
using System.Collections.Generic;
using System.Text;

namespace PhoneAccelerometers
{
    public class SensorData
    {
        private int x, y, z;

        public SensorData()
        {
            this.x = 0;
            this.y = 0;
            this.z = 0;
        }

        public int[] toArray()
        {
            int[] data = new int[3];
            data[0] = this.x;
            data[1] = this.y;
            data[2] = this.z;
            return data;
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
