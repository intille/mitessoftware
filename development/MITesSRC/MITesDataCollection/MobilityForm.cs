using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MITesDataCollection
{
    public partial class MobilityForm : Form, ChainnedForm
    {
        private Form nextForm;
        private Form previousForm;


        public static int[][] MobilityMatrix
        {
            get
            {
                return mobilityMatrix;
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
            InitializeInterface();
        }
        public void Cleanup()
        {
            ((ChainnedForm)this.previousForm).Cleanup();
        }

        public MobilityForm()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            int button_id = Convert.ToInt32(button.Name);
            int currentChoice = MobilityForm.mobilityMatrix[this.activityIndex][button_id];
            if (currentChoice == 0){
                ((Button)this.sensorButtons[button_id]).Text = Constants.MOBILITY_FORM_SLIGHTLY_MOBILE_LABEL;
                 MobilityForm.mobilityMatrix[this.activityIndex][button_id] =  1;
            }
            else if (currentChoice ==1){
                ((Button)this.sensorButtons[button_id]).Text = Constants.MOBILITY_FORM_MOBILE_LABEL;
                MobilityForm.mobilityMatrix[this.activityIndex][button_id]=2;
            }
            else if (currentChoice == 2){
                ((Button)this.sensorButtons[button_id]).Text = Constants.MOBILITY_FORM_VERY_MOBILE_LABEL;
                MobilityForm.mobilityMatrix[this.activityIndex][button_id]=3;
            }
            else if (currentChoice == 3){
                ((Button)this.sensorButtons[button_id]).Text = Constants.MOBILITY_FORM_NOT_MOBILE_LABEL;
                MobilityForm.mobilityMatrix[this.activityIndex][button_id]=0;
            }
                   
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.activityIndex == SimilarActivitiesForm.ConfusingActivities.Count - 1)
            {
                ((ChainnedForm)this.nextForm).Initialize();
                this.nextForm.Visible = true;
                this.Visible = false;
            }
            else
            {

                this.activityIndex++;
                string activityName = (string)SimilarActivitiesForm.ConfusingActivities[this.activityIndex];
                this.label1.Text = "During " + activityName + ", how mobile are the following areas of your body?";

                //load the current form choices

                foreach (Button button in this.sensorButtons)
                {
                    int button_id = Convert.ToInt32(button.Name);
                    int currentChoice = MobilityForm.mobilityMatrix[this.activityIndex][button_id];
                    if (currentChoice == 0)                   
                        ((Button)this.sensorButtons[button_id]).Text = Constants.MOBILITY_FORM_NOT_MOBILE_LABEL;                   
                    else if (currentChoice == 1)
                        ((Button)this.sensorButtons[button_id]).Text = Constants.MOBILITY_FORM_SLIGHTLY_MOBILE_LABEL;
                    else if (currentChoice == 2)
                        ((Button)this.sensorButtons[button_id]).Text = Constants.MOBILITY_FORM_MOBILE_LABEL;
                    else if (currentChoice == 3)
                        ((Button)this.sensorButtons[button_id]).Text = Constants.MOBILITY_FORM_VERY_MOBILE_LABEL;
                }
              
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (this.activityIndex > 0)
                this.activityIndex--;

            string activityName = (string)SimilarActivitiesForm.ConfusingActivities[this.activityIndex];
            this.label1.Text = "During " + activityName + ", how mobile are the following areas of your body?";

            //load the current form choices

            foreach (Button button in this.sensorButtons)
            {
                int button_id = Convert.ToInt32(button.Name);
                int currentChoice = MobilityForm.mobilityMatrix[this.activityIndex][button_id];
                if (currentChoice == 0)
                    ((Button)this.sensorButtons[button_id]).Text = Constants.MOBILITY_FORM_NOT_MOBILE_LABEL;
                else if (currentChoice == 1)
                    ((Button)this.sensorButtons[button_id]).Text = Constants.MOBILITY_FORM_SLIGHTLY_MOBILE_LABEL;
                else if (currentChoice == 2)
                    ((Button)this.sensorButtons[button_id]).Text = Constants.MOBILITY_FORM_MOBILE_LABEL;
                else if (currentChoice == 3)
                    ((Button)this.sensorButtons[button_id]).Text = Constants.MOBILITY_FORM_VERY_MOBILE_LABEL;
            }
        }
    }
}