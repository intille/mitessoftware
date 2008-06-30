using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenNETCF.Windows.Forms;

namespace MITesDataCollection
{
    public partial class BuildModelFeatureForm : Form, ChainnedForm,PathInterface
    {
        private FileBrowserDialog fileBrowserDialog1;
       // private FileListDialog fileListDialog1;



        private static string selectedFolder;
        private static string selectedFile;
        private Form nextForm;
        private Form previousForm;

        public static string SelectedFolder
        {
            get
            {
                return BuildModelFeatureForm.selectedFolder;
            }
        }


        public static string SelectedFile
        {
            get
            {
                return BuildModelFeatureForm.selectedFile;
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

        public void Initialize()
        {
        }

        public void Cleanup()
        {
            ((ChainnedForm)this.previousForm).Cleanup();
        }


        public BuildModelFeatureForm()
        {
            InitializeComponent();
            this.button2.Enabled = false;

        }


        private void button1_Click(object sender, EventArgs e)
        {
            this.fileBrowserDialog1 = new FileBrowserDialog(this);
            this.fileBrowserDialog1.ShowNewFolderButton = false;
            this.fileBrowserDialog1.Show();
            //this.textBox1.Text = this.fileBrowserDialog1.SelectedFile;
            //this.Show();


        }

        private void menuItem1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        public void UpdateSelectedPath(string path)
        {
            this.textBox1.Text = BuildModelFeatureForm.selectedFile = path;         
            BuildModelFeatureForm.selectedFolder = this.fileBrowserDialog1.SelectedPath;
            if (BuildModelFeatureForm.selectedFolder.Equals(""))
                this.button2.Enabled = false;
            else
                this.button2.Enabled = true;
            this.fileBrowserDialog1.Cleanup();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Visible = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ((ChainnedForm)this.previousForm).Cleanup();
           //if (classificationForm == null)
           //    classificationForm = new MITesDataCollectionForm(BuildModelFeatureForm.SelectedFolder, BuildModelFeatureForm.selectedFile);
           //classificationForm.Show();
        }
    }
}