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
    public partial class BuildModelFeatureForm : Form, ChainnedForm
    {
        //private FileBrowserDialog fileBrowserDialog1;
       // private FileListDialog fileListDialog1;
        private MITesDataCollectionForm classificationForm;

        public BuildModelFeatureForm()
        {
            InitializeComponent();

        }

        public void Cleanup()
        {
        }
        private void button1_Click(object sender, EventArgs e)
        {
            //this.fileBrowserDialog1 = new FileBrowserDialog();

            //this.fileBrowserDialog1.ShowNewFolderButton = false;

            //this.fileBrowserDialog1.ShowDialog();
            //this.textBox1.Text = this.fileBrowserDialog1.SelectedFile;


        }

        private void menuItem1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Visible = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
           // if (classificationForm == null)
           //     classificationForm = new MITesDataCollectionForm(this.fileBrowserDialog1.SelectedPath, this.fileBrowserDialog1.SelectedFile);
           // classificationForm.Show();
        }
    }
}