using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using AXML;
using MITesDataCollection.Utils;

namespace MITesDataCollection
{
    public partial class ActivityProtocolForm : Form, ChainnedForm
    {
        private static Protocol selectedProtocol;
        private Protocols protocols;
        private Form nextForm;
        private Form previousForm;

        //stores the selected activity protocol
        public static Protocol SelectedProtocol
        {
            get
            {
                return selectedProtocol;
            }
        }

        public Form PreviousForm
        {
            set
            {
                this.previousForm = value;
            }
        }

        public Form NextForm
        {
            set
            {
                this.nextForm = value;
            }

        }

        public void Cleanup()
        {
            ((ChainnedForm)this.previousForm).Cleanup();
        }

        public ActivityProtocolForm()
        {
            InitializeComponent();
            InitializeInterface();
            
        }

        private void menuItem1_Click(object sender, EventArgs e)
        {
#if (PocketPC)
            Application.Exit();
#else
            Environment.Exit(0);
#endif
        }
        
#if (PocketPC)
#else
        private const int CP_NOCLOSE_BUTTON = 0x200;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
                return myCp;
            }
        } 
#endif

        private void menuItem2_Click(object sender, EventArgs e)
        {

        }

        private void ActivityProtocol_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {            
            this.previousForm.Visible = true;
            this.Visible = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {                   
            this.nextForm.Visible = true;
            this.Visible = false;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedProtocol = (AXML.Protocol) protocols.ActivityProtocols[this.listBox1.SelectedIndex];
            if (this.button1.Enabled == false)
                this.button1.Enabled = true;
        }
    }
}