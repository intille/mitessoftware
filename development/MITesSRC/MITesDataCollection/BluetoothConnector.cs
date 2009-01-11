using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Bluetooth;
using HousenCS.MITes;
using System.IO;

namespace MITesDataCollection
{
    public class BluetoothConnector
    {

        //private Hashtable reconnectionThreadQuit = false;

        //private bool connectionLost = false;
        private int reconnection_attempts = 0;
        private int reconnection_timeout = 0;

        private HousenCS.MITes.MITesDecoder[] mitesDecoders;
        private Bluetooth.BluetoothController[] bluetoothControllers;
        private SXML.Receiver receiver;
        TextWriter log;
        public BluetoothConnector(SXML.Receiver receiver, Bluetooth.BluetoothController[] bluetoothControllers, HousenCS.MITes.MITesDecoder[] mitesDecoders)
        {
            this.receiver = receiver;
            this.bluetoothControllers = bluetoothControllers;
            this.mitesDecoders = mitesDecoders;

            // TODO At some point close this properly
            if (!Directory.Exists("\\Wockets"))
                Directory.CreateDirectory("\\Wockets");
            try
            {
                this.log = new StreamWriter("\\Wockets\\log-" + receiver.ID + ".txt", true);
            }
            catch (IOException e)
            {
                // If an exception is thrown, then make sure we don't crash. NIH demo only. 
                // TODO remove and fix more elegantly
               this.log = new StreamWriter("\\Wockets\\log-" + receiver.ID + "-" + Environment.TickCount + ".txt", true);
            }
        }

        //This method is executed as a seperate thread to manage the progress
        //form
        public void Reconnect()
        {
            reconnection_attempts = 0;
            //while (true)
            //{
#if (PocketPC)
            // Thread.Sleep(1000);
#else
                Thread.Sleep(20);
#endif

            /*while (reconnection_timeout > 0)
                {

                    reconnection_timeout--;
                    System.Threading.Thread.Sleep(1000);
                    this.log.WriteLine("Retrying in ... " + reconnection_timeout + " seconds");
                    this.log.Flush();
                    //SetErrorLabel("Retrying in ... " + reconnection_timeout + " seconds");

                }*/

            // bool btConnected = true;
            //foreach (Receiver receiver in this.sensors.Receivers)
            //{
            if ((receiver.Type == SXML.Constants.RECEIVER_BLUETOOTH) && (receiver.Restarting == true))
            {
                //SetErrorLabel("Reconnecting to wocket (" + reconnection_attempts + ") " + receiver.MAC);
                //progressMessage += "Initializing Bluetooth ...";
                this.bluetoothControllers[receiver.ID] = new BluetoothController();
                try
                {
                    this.log.WriteLine("Calling initialize BT controller");
                    this.log.Flush();
                    this.bluetoothControllers[receiver.ID].initialize(receiver.MacAddress, receiver.PassKey);
                    this.log.WriteLine("Success Initializing BT Controller");
                    this.log.Flush();
                    this.mitesDecoders[receiver.ID] = new MITesDecoder();

                }
                catch (Exception)
                {
                    reconnection_timeout = 10;
                    this.log.WriteLine("Failed to connect with BT retrying in 10 seconds");
                    this.log.Flush();
                }
                //btConnected = btConnected & receiver.Running;

                receiver.Restarted = true;
                //break;
            }
            //}

            //if (btConnected == true)
            //{
            // btRestablished = true;
            // reconnectionThreadQuit = true;
            //}
            // reconnection_attempts++;
            // }
            this.log.WriteLine("Exiting Bluetooth reconnector");
            this.log.Flush();

            //reconnectionThreadQuit = false;
            // }
        }
    }
}
