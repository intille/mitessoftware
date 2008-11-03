using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;
using System.Net.Sockets;
using HousenCS.MITes;

namespace BTMITesEmulator
{
    public partial class Form1 : Form
    {
        private SerialPort BluetoothConnection;
        private Byte[] mitesData = { 0x44, 0x44 };
        private string MITES_SAMPLE_DIRECTORY = "..\\NeededFiles\\SamplePLFormat\\";
        private string INCOMING_PORT = "COM17";
        Thread t;
        private bool realistic = false;
        private int gapmillseconds = 2;
        private bool startTransmission = false;


        
        public Form1()
        {
            InitializeComponent();
            BluetoothConnection = new SerialPort();
            this.BluetoothConnection.PortName = INCOMING_PORT;
            BluetoothConnection.WriteTimeout = 1000;
            BluetoothConnection.StopBits = StopBits.One;
            BluetoothConnection.BaudRate = 57600;
            BluetoothConnection.Parity = Parity.None;

            //BluetoothConnection.
            BluetoothConnection.Open();

            t = new Thread(new ThreadStart(MITesSenderThread));
            t.Start();
        }

        string sps = "";
        string bps = "";
        private void MITesSenderThread()
        {
            MITesDecoder aMITesDecoder = new MITesDecoder();
            MITesLoggerReader aMITesLoggerReader = new MITesLoggerReader(aMITesDecoder, MITES_SAMPLE_DIRECTORY);
            MITesData data = new MITesData();
            double UnixTime = ((TimeSpan)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)))).TotalMilliseconds;
            //double difference=0;
            double previousTime=0;
            Random random = new Random();
            int bytePerSecond = 0;
            int samplesPerSecond=0;
            DateTime previousDateTime=DateTime.Now;
            bool workingBT=false;


            while (true)
            {
                //Thread.Sleep(1000);
                if (BluetoothConnection.IsOpen)
                {
                    try
                    {
       

                        if (startTransmission)
                        {
                            bool isData = aMITesLoggerReader.GetSensorData(10);
                            int channel = 0, x = 0, y = 0, z = 0;
                            double unixtimestamp = 0.0;
                            double lasttime = 0.0;
                            byte[] rawSample = aMITesDecoder.GetSomeMITesData()[0].rawBytes;

                            do
                            {
                                //decode the frame

                                if (realistic == false)
                                {
                                    //Thread.Sleep(gapmillseconds);
                                    if (rawSample[0] == 4)
                                        rawSample[0] = 8;
                                    else
                                        rawSample[0] = 4;

                                    rawSample[1] = (byte)random.Next(256);
                                    rawSample[2] = (byte)random.Next(256);
                                    rawSample[3] = (byte)random.Next(256);
                                    
                                    //BluetoothConnection.Write(rawSample, 0, 5);
                                    samplesPerSecond++;
                                    if ((samplesPerSecond % 5)==0)                 
                                        BluetoothConnection.Write(mitesData, 0, 2);
                                    else
                                        BluetoothConnection.Write(rawSample, 0, 5);
                                    workingBT = true;
                                }
                                else if ((int)previousTime != 0)
                                {
                                    channel = aMITesDecoder.GetSomeMITesData()[0].channel;
                                    x = aMITesDecoder.GetSomeMITesData()[0].x;
                                    y = aMITesDecoder.GetSomeMITesData()[0].y;
                                    z = aMITesDecoder.GetSomeMITesData()[0].z;
                                    unixtimestamp = aMITesDecoder.GetSomeMITesData()[0].unixTimeStamp;
                                    Thread.Sleep((int)previousTime);
                                    previousTime = unixtimestamp;

                                    if (channel != 83)
                                        BluetoothConnection.Write(aMITesDecoder.GetSomeMITesData()[0].rawBytes, 0, 5);
                                }

                                double seconds = ((TimeSpan)(DateTime.Now - previousDateTime)).TotalSeconds;
                                if (seconds > 5)
                                {
                                    sps = ((int)(samplesPerSecond / 5)).ToString();
                                    bps = ((int)(samplesPerSecond * 5)).ToString();
                                    samplesPerSecond = 0;
                                    previousDateTime = DateTime.Now;
                                }

                            } while ((realistic == false));//|| (isData = aMITesLoggerReader.GetSensorData(10)));
                            BluetoothConnection.Close();
                            Application.Exit();
                        }
                        else
                        {
                            //2 sync bytes
                            BluetoothConnection.Write(mitesData, 0, 2);
                            //Thread.Sleep(5);
                        }
                            
                      

                        /*else
                        {
                            int startTime = (int)((TimeSpan)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)))).TotalSeconds;                             
                            do
                            {
                                Thread.Sleep(gapmillseconds);
                                
                                //decode the frame
                                if (data.channel == 4)
                                    data.channel = 8;
                                else
                                    data.channel = 4;
                                data.x = (short)random.Next(1000);
                                data.y = (short)random.Next(1000);
                                data.z = (short)random.Next(1000);
                                data.unixTimeStamp = ((TimeSpan)(DateTime.UtcNow.Subtract(new DateTime(1970, 1,1)))).TotalMilliseconds;
                                data.ResetRawVals();
                                if (channel != 83)
                                    BluetoothConnection.Write(aMITesDecoder.GetSomeMITesData()[0].rawBytes, 0, 5);
                                int currentTime = (int) ((TimeSpan)(DateTime.UtcNow.Subtract(new DateTime(1970, 1,1)))).TotalSeconds;                                
                            } while (currentTime<(startTime+totalSeconds));

                        }*/

                        
                    }
                    catch (Exception e)
                    {
                        samplesPerSecond = 0;
                        if (workingBT == true) //only reset if BT was already working
                        {
                            BluetoothConnection.Close();
                            BluetoothConnection = new SerialPort();
                            this.BluetoothConnection.PortName = INCOMING_PORT;
                            BluetoothConnection.WriteTimeout = 1000;
                            BluetoothConnection.StopBits = StopBits.One;
                            BluetoothConnection.BaudRate = 57600;
                            BluetoothConnection.Parity = Parity.None;
                            //BluetoothConnection.
                            BluetoothConnection.Open();
                            workingBT = false;
                        }
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            startTransmission = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            this.label3.Text = sps;
            this.label4.Text = bps;
        }

    }
}