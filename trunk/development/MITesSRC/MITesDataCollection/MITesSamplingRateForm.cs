using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using HousenCS.MITes;
using SXML;

namespace MITesDataCollection
{
    public partial class MITesSamplingRateForm : Form
    {
        private MITesDataFilterer aMITesDataFilterer;
        private SensorAnnotation sensorConfiguration;


        public  MITesSamplingRateForm()
        {
            InitializeComponent();
            InitializeInterface();
        }

        public MITesSamplingRateForm(MITesDataFilterer aMITesDataFilterer, SensorAnnotation sensorConfiguration)
        {
            InitializeComponent();            
            this.aMITesDataFilterer = aMITesDataFilterer;
            this.sensorConfiguration = sensorConfiguration;
            InitializeInterface();

        }

        private void MitesSamplingRate_Load(object sender, EventArgs e)
        {

        }

    }
}