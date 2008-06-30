using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;


namespace MITesDataCollection
{
    public partial class MainForm : Form,ChainnedForm
    {
        private Form[] nextForms;
        private static int selectedForm;


        public static bool IsDisposed = false;

        public MainForm()
        {
            InitializeComponent();
            InitializeInterface();
            MainForm.selectedForm = -1;

            ActivityProtocolForm activityProtocolForm = new ActivityProtocolForm();
            SensorConfigurationForm sensorConfigurationForm = new SensorConfigurationForm();
            BuildModelFeatureForm buildModelFeatureForm = new BuildModelFeatureForm();            
            WhereStoreDataForm whereStoreDataForm = new WhereStoreDataForm();
        
            activityProtocolForm.PreviousForm = this;
            activityProtocolForm.NextForm = sensorConfigurationForm;
            sensorConfigurationForm.PreviousForm = activityProtocolForm;
            sensorConfigurationForm.NextForm = whereStoreDataForm;
            whereStoreDataForm.PreviousForm = sensorConfigurationForm;
            whereStoreDataForm.NextForm = null;
            
            Form[] nextForms = new Form[2];
            nextForms[0] = activityProtocolForm;
            nextForms[1] = buildModelFeatureForm;
            SetNextForms(nextForms);

            
           
        }

        public static int SelectedForm
        {
            get
            {
                return MainForm.selectedForm;
            }
        }

        public void SetNextForms(Form[] nextForms)
        {
            this.nextForms = new Form[nextForms.Length];
            for (int i = 0; (i < nextForms.Length); i++)
                this.nextForms[i] = nextForms[i];
        }

        public void Initialize()
        {
        }
        public void Cleanup()
        { 
           
            this.Close();

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


        private void button2_Click(object sender, EventArgs e)
        {
            this.nextForms[0].Visible = true;
            MainForm.selectedForm = Constants.MAIN_SELECTED_COLLECT_DATA;      
            //this.Visible = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.nextForms[1].Visible = true;
            MainForm.selectedForm = Constants.MAIN_SELECTED_ESTIMATE_ENERGY;
            //this.Visible = false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.nextForms[2].Visible = true;
            MainForm.selectedForm = Constants.MAIN_SELECTED_TROUBLESHOOT;  
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.nextForms[3].Visible = true;
            MainForm.selectedForm = Constants.MAIN_SELECTED_CALIBRATE;  
        }



    }
}