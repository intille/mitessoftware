using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SXML;
using MITesDataCollection.Utils;

namespace MITesDataCollection
{
    public partial class SensorConfigurationForm : Form, ChainnedForm
    {
        //private ClassificationForm classificationForm;
        //private RealtimeMITesSignalForm mitesSignalForm;
       // private WhereStoreDataForm whereStoreDataForm;
        private Configurations configurations;
        private static Configuration selectedConfiguration;
       
        private Form nextForm;
        private Form previousForm;

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

        public static Configuration SelectedSensors
        {
            get
            {
                return selectedConfiguration;
            }
        }

        public SensorConfigurationForm()
        {
            InitializeComponent();
            InitializeInterface();
        }

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

        private void button1_Click(object sender, EventArgs e)
        {           
            this.nextForm.Visible=true;
            this.Visible = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {            
            this.previousForm.Visible = true;
            this.Visible = false;
        }

        private void menuItem1_Click(object sender, EventArgs e)
        {
#if (PocketPC)
            Application.Exit();
#else
            Environment.Exit(0);
#endif
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedConfiguration = (Configuration)configurations.SensorConfigurations[this.listBox1.SelectedIndex];
            if (this.button1.Enabled==false)
                this.button1.Enabled = true;
        }
    }
}