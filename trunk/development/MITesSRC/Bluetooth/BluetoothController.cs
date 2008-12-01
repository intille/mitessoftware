using System;
using System.Collections.Generic;
using System.Text;

namespace Bluetooth
{

    public class BluetoothController
    {
        public static int MAXBYTESBUFFER = 4096;

        /// <summary>
        /// The buffer that actually stores raw serial data acquired from the Bluetooth controller
        /// </summary>
        private byte[] bluetoothBytesBuffer;
        private BluetoothStream bs;

        public byte[] BluetoothBytesBuffer
        {
            get { return this.bluetoothBytesBuffer; }
        }
        public BluetoothController()
        {
            this.bluetoothBytesBuffer = new byte[MAXBYTESBUFFER];
        }

        public void initialize(byte[] addr, string pin)
        {
            this.bs = BluetoothStream.OpenConnection(addr, pin);
        }

        public int read()
        {
            return this.bs.Read(this.bluetoothBytesBuffer, 0, MAXBYTESBUFFER);
        }

        public int read(byte[] dataBuffer)
        {
            return this.bs.Read(dataBuffer, 0, MAXBYTESBUFFER);
        }

        public void cleanup()
        {
            this.bs.Close();
        }
    }
}
