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
    public partial class TroubleshootModel : Form, ChainnedForm, PathInterface
    {
        private static string selectedFolder;
        private static string selectedFile;
        private FileBrowserDialog fileBrowserDialog1;
        //private TroubleshootActivitiesForm troubleshootActivitiesForm;

        private Form nextForm;
        private Form previousForm;

        public static string SelectedFolder
        {
            get
            {
                return TroubleshootModel.selectedFolder;
            }
        }

        public static string SelectedFile
        {
            get
            {
                return TroubleshootModel.selectedFile;
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

        public void UpdateSelectedPath(string path)
        {
            this.textBox1.Text = TroubleshootModel.selectedFile = path;
            TroubleshootModel.selectedFolder = this.fileBrowserDialog1.SelectedPath;
            if (TroubleshootModel.selectedFolder.Equals(""))
                this.button2.Enabled = false;
            else
                this.button2.Enabled = true;
            this.fileBrowserDialog1.Cleanup();
        }

        public TroubleshootModel()
        {
            InitializeComponent();
            selectedFolder = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.fileBrowserDialog1 = new FileBrowserDialog(this);
            this.fileBrowserDialog1.ShowNewFolderButton = false;
            this.fileBrowserDialog1.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ((ChainnedForm)this.nextForm).Initialize();
            this.nextForm.Visible = true;
            this.Visible = false;

         //   if (troubleshootActivitiesForm == null)
           //     troubleshootActivitiesForm = new TroubleshootActivitiesForm(this.selectedFolder);
           // troubleshootActivitiesForm.Show();
        }

        private void menuItem1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Visible = false;
        }
    }
}