using System;
using System.Collections.Generic;
using System.Text;


namespace PhoneAccelerometers.HTC.DiamondTouch
{
    public class HTCDecoder
    {
        private PhoneAccelerometers.HTC.DiamondTouch.HTCGSensor diamondTouchSensor;
        

        private SensorData lastData;

        public HTCDecoder()
        {
            this.diamondTouchSensor =  new HTCGSensor();
            //this.diamondTouchSensor2 = new HTCGSensor();
            SensorData data = new SensorData();
            data.X = 0;
            data.Y = 0;
            data.Z = 0;
            this.lastData = data;
         
        }

        public SensorData GetSensorData()
        {

            HTCGSensorData gData = this.diamondTouchSensor.GetRawSensorData();
            SensorData data = new SensorData();
            data.X = ((int)((gData.TiltX+1000) ))/4;
            data.Y = ((int)((gData.TiltY+1000) ))/4;
            data.Z = ((int)((gData.TiltZ+1000) ))/4;


            if ((data.X > 0) && (data.Y > 0) && (data.Z > 0))
                this.lastData = data;
           
            return this.lastData;

        }

        public SensorData LastData
        {
            get
            {
                return this.lastData;
            }
        }
        public void Reset()
        {
          //  this.diamondTouchSensor = new HTCGSensor();
        }
    }
}
