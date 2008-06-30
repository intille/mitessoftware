using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using MITesFeatures.Utils.parser;

namespace MITesDataCollection
{
    public partial class SimilarActivitiesForm : Form, ChainnedForm
    {
        private AXML.Annotation annotation;
        private Form nextForm;
        private Form previousForm;
        private static ArrayList confusingActivities = new System.Collections.ArrayList();

        public static ArrayList ConfusingActivities
        {
            get
            {
                return confusingActivities;
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

        public SimilarActivitiesForm()//string activity, AXML.Annotation aanotation)
        {
            this.checkboxes = new System.Collections.ArrayList();         
            InitializeComponent();                       
        }



        //Here we need to parse the training file   
        //collapse the similar activities in a file and separate them in another
        // retrain on the collapsed activities and the separated ones 2 different decision trees
        // store the trees in a hash table with the labels that trigger them
        // particularly having a root label plus the collapsed classes
        // eventually the collapsed format need to be stored somehow
        private void button2_Click(object sender, EventArgs e)
        {
           //Based on the selection of the activities generate the following files
           //Major Group File, HDT File and subtree files
            foreach (CheckBox c in this.checkboxes)            
                if (c.Checked == true)                
                    confusingActivities.Add(c.Text);

            ((ChainnedForm)this.nextForm).Initialize();
            this.nextForm.Visible = true;
            this.Visible = false; 
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Visible = false;
        }
    }
}