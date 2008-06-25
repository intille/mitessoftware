using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MITesDataCollection
{
    public partial class TroubleshootActivitiesForm : Form
    {
        private string dataDirectory;
        private string selectedActivity;
        private SimilarActivitiesForm similarActivitiesForm;

        public TroubleshootActivitiesForm(string dataDirectory)
        {
            this.dataDirectory = dataDirectory;           
            InitializeComponent();
            InitializeInterface();

            
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedActivity = ((AXML.Label)((AXML.Category)this.aannotation.Categories[0]).Labels[this.listBox1.SelectedIndex]).Name;
            if (this.button2.Enabled == false)
                this.button2.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (similarActivitiesForm == null)
                similarActivitiesForm = new SimilarActivitiesForm(this.selectedActivity,this.aannotation);
            similarActivitiesForm.Show();
        }

    }
}