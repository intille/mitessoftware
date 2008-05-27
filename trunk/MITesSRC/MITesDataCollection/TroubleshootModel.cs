using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
//using OpenNETCF.Windows.Forms;

namespace MITesDataCollection
{
    public partial class TroubleshootModel : Form
    {
        private string selectedFolder;
        private TroubleshootActivitiesForm troubleshootActivitiesForm;

        public TroubleshootModel()
        {
            InitializeComponent();
            this.selectedFolder = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //OpenNETCF.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new OpenNETCF.Windows.Forms.FolderBrowserDialog(this);
            //folderBrowserDialog.ShowDialog();
            //this.textBox1.Text = this.selectedFolder = folderBrowserDialog.SelectedPath;

            //if (this.selectedFolder.Equals(""))
            //    this.button2.Enabled = false;
            //else
            //    this.button2.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (troubleshootActivitiesForm == null)
                troubleshootActivitiesForm = new TroubleshootActivitiesForm(this.selectedFolder);
            troubleshootActivitiesForm.Show();
        }

        private void menuItem1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}