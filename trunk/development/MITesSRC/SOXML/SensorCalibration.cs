using System;
using System.Collections.Generic;
using System.Text;

namespace SOXML
{
    public class SensorCalibration
    {
        private int id;
        SensorOrientation[] orientations;

        public SensorCalibration()
        {
            this.orientations = new SensorOrientation[Constants.SENSOR_ORIENTATIONS];
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

        public SensorOrientation[] Orientations
        {
            get
            {
                return this.orientations;
            }
        }
    }
}
