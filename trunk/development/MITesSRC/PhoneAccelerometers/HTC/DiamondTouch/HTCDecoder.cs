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
        private SensorData lastData;
        private int samplingRate;
        private double activityCount;
        private double prevX;
        private double prevY;
        private double prevZ;
        private double averageX;
        private double averageY;
        private double averageZ;
        private int lastSamplingRate = 0;
        public static int MaximumSamplingRate = 25;



        #region Builtin Accelerometer Polling Thread
#if (PocketPC)
        private int[] bufferX;
        private int[] bufferY;
        private int[] bufferZ;
        int writeIndex = 0;
        int readIndex = 0;
        private bool htcQuitting = false;
        public void PollBuiltInSensors()
        {
            bufferX = new int[24];
            bufferY = new int[24];
            bufferZ = new int[24];
            writeIndex = 0;
            double lastTime = UnixTime.GetUnixTime();
            while (true)
            {
                Thread.Sleep(40);
                if (this.htcQuitting == true)
                    break;
                else
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

                    GetSensorData();
                    bufferX[writeIndex] = this.LastData.X;
                    bufferY[writeIndex] = this.LastData.Y;
                    bufferZ[writeIndex] = this.LastData.Z;
                    writeIndex = (writeIndex + 1) % 24;
                    // this.htcDecoder.Reset();

                }
            }
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
        public int[] BufferX
        {
            get
            {
                return this.bufferX;
            }
        }
        public int[] BufferY
        {
            get
            {
                return this.bufferY;
            }
        }
        public int[] BufferZ
        {
            get
            {
                return this.bufferZ;
            }
        }
        public HTCDecoder()
        {
            this.diamondTouchSensor =  new HTCGSensor();
            //this.diamondTouchSensor2 = new HTCGSensor();
            SensorData data = new SensorData();
            data.X = 0;
            data.Y = 0;
            data.Z = 0;
            this.lastData = data;
            this.samplingRate = 0;
            this.activityCount = 0;
            this.prevX = 0;
            this.prevY = 0;
            this.prevZ = 0;
            this.averageX = 0;
            this.averageY = 0;
            this.averageZ = 0;
         
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


        public SensorData GetSensorData()
        {

            HTCGSensorData gData = this.diamondTouchSensor.GetRawSensorData();
            SensorData data = new SensorData();

            averageX = averageX + Math.Abs(prevX - gData.TiltX);
            averageX = averageY + Math.Abs(prevY - gData.TiltY);
            averageX = averageZ + Math.Abs(prevZ - gData.TiltZ);
            prevX = gData.TiltX;
            prevY = gData.TiltY;
            prevZ = gData.TiltZ;

            data.X = ((int)((gData.TiltX+1000) ))/4;
            data.Y = ((int)((gData.TiltY+1000) ))/4;
            data.Z = ((int)((gData.TiltZ+1000) ))/4;


            if ((data.X > 0) && (data.Y > 0) && (data.Z > 0))
            {
                this.lastData = data;
                this.samplingRate++;
            }
           
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