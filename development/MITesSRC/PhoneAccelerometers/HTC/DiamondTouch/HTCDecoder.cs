using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using HousenCS.MITes;

namespace PhoneAccelerometers.HTC.DiamondTouch
{
    public class HTCDecoder 
    {
        private PhoneAccelerometers.HTC.DiamondTouch.HTCGSensor diamondTouchSensor;        
        //private SensorData lastData;
       
        private int samplingRate;
        private double activityCount;
        private double prevX;
        private double prevY;
        private double prevZ;
        private double averageX;
        private double averageY;
        private double averageZ;
        private int lastSamplingRate = 0;


        #region Builtin Accelerometer Polling Thread
#if (PocketPC)

 
        
        int writeIndex = 0;
        int readIndex = 0;
        private bool htcQuitting = false;
        private double lastTime=0;
        public GenericAccelerometerData PollBuiltInSensors()
        {
            double currentTime = UnixTime.GetUnixTime();
            if ((currentTime - lastTime) > 1000)
            {
                this.lastSamplingRate = this.samplingRate;
                this.activityCount = this.averageX + this.averageY + this.averageZ / 3;
                this.activityCount /= this.samplingRate;
                this.averageX = 0;
                this.averageY = 0;
                this.averageZ = 0;
                this.samplingRate = 0;
                lastTime = currentTime;
            }
            else
                this.samplingRate = this.samplingRate + 1;
            return GetSensorData();
        }

#endif
        #endregion Builtin Accelerometer Polling Thread



        public bool isQuitting
        {
            get
            {
                return this.htcQuitting;
            }
            set
            {
                this.htcQuitting = value;
            }
           
        }
        public int ReadIndex
        {
            get
            {
                return this.readIndex;
            }

            set
            {
                this.readIndex = value;
            }
        }

        public int WriteIndex
        {
            get
            {
                return this.writeIndex;
            }

            set
            {
                this.writeIndex = value;
            }
        }

        public HTCDecoder()
        {
            this.diamondTouchSensor =  new HTCGSensor();
            //this.diamondTouchSensor2 = new HTCGSensor();
            //SensorData data = new SensorData();
            //data.X = 0;
            //data.Y = 0;
            //data.Z = 0;
            this.samplingRate = 0;
            this.activityCount = 0;
            this.prevX = 0;
            this.prevY = 0;
            this.prevZ = 0;
            this.averageX = 0;
            this.averageY = 0;
            this.averageZ = 0;
            this.lastTime = UnixTime.GetUnixTime();
         
        }

        public int SamplingRate
        {
            get
            {
                return this.lastSamplingRate;
            }
        }

        public double ActivityCount
        {
            get
            {
                return this.activityCount;
            }
        }


        public GenericAccelerometerData GetSensorData()
        {
            GenericAccelerometerData data = new GenericAccelerometerData(Constants.BUILT_IN_ACCELEROMETER_CHANNEL_ID);
            HTCGSensorData gData = this.diamondTouchSensor.GetRawSensorData();
            data.Type = Constants.DIAMOND_TOUCH_ACCELEROMETER;
            data.MaximumSamplingRate = Constants.DIAMOND_TOUCH_MAX_SAMPLING_RATE;          

            averageX = averageX + Math.Abs(prevX - gData.TiltX);
            averageX = averageY + Math.Abs(prevY - gData.TiltY);
            averageX = averageZ + Math.Abs(prevZ - gData.TiltZ);
            prevX = gData.TiltX;
            prevY = gData.TiltY;
            prevZ = gData.TiltZ;

            data.X = (int)(gData.TiltX+1500);
            data.Y = (int)(gData.TiltY+1500);
            data.Z = (int)(gData.TiltZ+1500);
                     
            return data;

        }

        public void Reset()
        {
          //  this.diamondTouchSensor = new HTCGSensor();
        }
    }
}
