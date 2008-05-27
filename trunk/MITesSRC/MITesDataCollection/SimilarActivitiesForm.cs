using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MITesFeatures.Utils.parser;

namespace MITesDataCollection
{
    public partial class SimilarActivitiesForm : Form
    {
        private AXML.Annotation aanotation;
        private string confusedActivity;
        public SimilarActivitiesForm(string activity, AXML.Annotation aanotation)
        {
            this.checkboxes = new System.Collections.ArrayList();
            this.aanotation = aanotation;
            this.confusedActivity = activity;
            InitializeComponent();
            InitializeInterface();            

        }

        //Here we need to parse the training file
        //collapse the similar activities in a file and separate them in another
        // retrain on the collapsed activities and the separated ones 2 different decision trees
        // store the trees in a hash table with the labels that trigger them
        // particularly having a root label plus the collapsed classes
        // eventually the collapsed format need to be stored somehow
        private void button2_Click(object sender, EventArgs e)
        {
           // ArffParser parser=
        }
    }
}