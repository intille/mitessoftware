using System;
using System.Collections.Generic;
using System.Text;

namespace SXML
{
    public class Receiver
    {
        private int id;
        private string mac;
        private string passkey;
        private string type;
        private byte[] macAddress;
        private string decoder;
        private bool running;
        private bool restarted;
        private bool restarting;

        public Receiver()
        {
            this.macAddress = new byte[Constants.MAC_SIZE];
            this.running = false;
            this.restarted = false;
            this.restarting = false;
        }

        public bool Running
        {
            get
            {
                return this.running;
            }
            set
            {
                this.running = value;
            }
        }


        public bool Restarting
        {
            get
            {
                return this.restarting;
            }
            set
            {
                this.restarting = value;
            }
        }

        public bool Restarted
        {
            get
            {
                return this.restarted;
            }
            set
            {
                this.restarted = value;
            }
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
        public string Decoder
        {
            get
            {
                return this.decoder;
            }

            set
            {
                this.decoder = value;
            }
        }

        public string MAC
        {
            get
            {
                return this.mac;
            }

            set
            {
                this.mac = value;
            }
        }

        public byte[] MacAddress
        {
            get
            {
                return this.macAddress;
            }
        }

        public string PassKey
        {
            get
            {
                return this.passkey;
            }

            set
            {
                this.passkey = value;
            }
        }

        public string Type
        {
            get
            {
                return this.type;
            }

            set
            {
                this.type = value;
            }
        }

        public bool isBluetooth()
        {
            if (this.type == Constants.RECEIVER_BLUETOOTH)
                return true;
            else
                return false;
        }

        public bool isUSB()
        {
            if (this.type == Constants.RECEIVER_USB)
                return true;
            else
                return false;
        }
    }
}
