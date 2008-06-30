using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace MITesDataCollection
{
    public partial class WhereStoreDataForm : Form, ChainnedForm, PathInterface
    {
        private static string selectedFolder;
        //private MITesDataCollectionForm mitesDataCollectionForm;
        private Form nextForm;
        private Form previousForm;

        public static string SelectedFolder
        {
            get
            {
                return WhereStoreDataForm.selectedFolder;
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

        public WhereStoreDataForm()
        {
            InitializeComponent();
            InitializeInterface();
            WhereStoreDataForm.selectedFolder = "";
        }

        public void UpdateSelectedPath(string path)
        {
            this.textBox1.Text = WhereStoreDataForm.selectedFolder = path;
            if (WhereStoreDataForm.selectedFolder.Equals(""))
                this.button2.Enabled = false;
            else
                this.button2.Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenNETCF.Windows.Forms.FolderBrowserDialog folderSelect = new OpenNETCF.Windows.Forms.FolderBrowserDialog(this);
            folderSelect.Show();                  
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            ((ChainnedForm)this.previousForm).Cleanup();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.previousForm.Visible = true;
            this.Visible = false;
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
        private void menuItem1_Click(object sender, EventArgs e)
        {
#if (PocketPC)
            Application.Exit();
#else
            Environment.Exit(0);
#endif
        }
    }
}