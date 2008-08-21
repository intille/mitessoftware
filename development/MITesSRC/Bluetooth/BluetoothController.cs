using System;
using System.Collections.Generic;
using System.Text;


// 32.feet.NET references
using InTheHand.Net;
using InTheHand.Net.Sockets;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Ports;
using System.IO.Ports;
using System.Runtime.InteropServices;
using System.Net.Sockets;

namespace Bluetooth
{

    public class BluetoothController
    {

        // Bluetooth Parameters
        private static InTheHand.Net.BluetoothAddress blt_address;
        private static BluetoothClient blt;
        private static BluetoothEndPoint blt_endPoint;
        private static BluetoothSerialPort btPort=null;
        //WIDCOMM stuff
        private static bool usingWidcomm = false;
        private static byte[] widcomm_btAddr;

        public BluetoothController()
        {
            if (BluetoothRadio.PrimaryRadio == null)
                usingWidcomm = true;

            if (!usingWidcomm)
                BluetoothRadio.PrimaryRadio.Mode = RadioMode.Connectable;
            else
                if (!initializeWidcommBluetooth())
                    throw new Exception("Couldn't start the WIDCOMM stack");
        }

        /// <summary>
        /// Prepares the bluetooth layer serial port to be opened by
        /// the application. This function does not actually open the 
        /// port for reading, the caller must do that by opening a 
        /// standard serial port with the name returned. 
        /// </summary>
        /// <param name="addr">The MAC address of the remote bluetooth device. 
        /// It <b>MUST</b> be in most-significant-byte first
        /// order (i.e. the bluetooth address 00:f1:ad:34:3d:f3 would be
        /// { 0x00, 0xf1, ...} and NOT {0xf3, 0x3d, ...})</param>
        /// <param name="pin">An optional pin for the bluetooth device</param>
        /// <returns>the name of the port to be opened
        /// i.e. "COM7", "BCG3", etc.</returns>
        public string initialize(byte[] addr, string pin)
        {
            if (!usingWidcomm)
            {
                BluetoothRadio.PrimaryRadio.Mode = RadioMode.Connectable;
                byte[] reverseAddr = new byte[addr.Length];
                for (int ii = 0; ii < addr.Length; ii++)
                {
                    reverseAddr[reverseAddr.Length - 1 - ii] = addr[ii];
                }
                blt_address = new BluetoothAddress(reverseAddr);

                BluetoothSecurity.PairRequest(blt_address, pin);
                if (pin != null)
                    BluetoothSecurity.SetPin(blt_address, pin);
                blt_endPoint = new BluetoothEndPoint((BluetoothAddress)blt_address, BluetoothService.SerialPort);
                btPort=BluetoothSerialPort.CreateClient(blt_endPoint);
                return btPort.PortName;
 

            }
            else
            {
                IntPtr stringPtr = prepareCOMportWidcomm(addr);
                if (stringPtr != IntPtr.Zero)
                    return "";
                else
                    throw new Exception("Got a null pointer from the WIDCOMM code");
            }

        }

        public void close()
        {
            if (btPort != null)
            {
                btPort.Close();
                BluetoothSecurity.RemoveDevice(blt_address);
            }
            if (usingWidcomm)
                destroyWidcommBluetooth();
        }

        [DllImport("WidcommWrapper.dll", CharSet = CharSet.Auto, EntryPoint = "?prepareCOMport@@YAPA_WQAE@Z")]
        private static extern IntPtr prepareCOMportWidcomm(byte[] addr);

        [DllImport("WidcommWrapper.dll", CharSet = CharSet.Auto, EntryPoint = "?instantiateBluetoothClient@@YAHXZ")]
        private static extern bool initializeWidcommBluetooth();

        [DllImport("WidcommWrapper.dll", CharSet = CharSet.Auto, EntryPoint = "?destroyBluetoothClient@@YAXXZ")]
        private static extern void destroyWidcommBluetooth();
    }
}
