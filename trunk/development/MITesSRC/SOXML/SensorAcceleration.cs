using System;
using System.Collections.Generic;
using System.Text;

namespace SOXML
{
    public class SensorAcceleration
    {
        private string direction;
        private double minX;
        private double minY;
        private double minZ;
        private double maxX;
        private double maxY;
        private double maxZ;

        public SensorAcceleration()
        {
        }

        public string Direction
        {
            get
            {
                return this.direction;
            }

            set
            {
                this.direction = value;
            }
        }

        public double MinX
        {
            get
            {
                return this.minX;
            }

            set
            {
                this.minX = value;
            }
        }

        public double MinY
        {
            get
            {
                return this.minY;
            }

            set
            {
                this.minY = value;
            }
        }

        public double MinZ
        {
            get
            {
                return this.minZ;
            }

            set
            {
                this.minZ = value;
            }
        }


        public double MaxX
        {
            get
            {
                return this.maxX;
            }

            set
            {
                this.maxX = value;
            }
        }

        public double MaxY
        {
            get
            {
                return this.maxY;
            }

            set
            {
                this.maxY = value;
            }
        }

        public double MaxZ
        {
            get
            {
                return this.maxZ;
            }

            set
            {
                this.maxZ = value;
            }
        }
    }
}
