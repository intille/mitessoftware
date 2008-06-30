using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace MITesDataCollection
{
    public partial class OrientationForm : Form, ChainnedForm
    {
        private Form nextForm;
        private Form previousForm;

        public static int[][] OrientationMatrix
        {
            get
            {
                return orientationMatrix;
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

        public OrientationForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int countOrientations=((ArrayList)this.body_parts_orientations_names[this.bodyPartCurrentIndex]).Count;
            if (this.orientationCurrentIndex==countOrientations-1)
                this.orientationCurrentIndex=0;
            else
                this.orientationCurrentIndex++;
            if (OrientationForm.orientationMatrix[this.activityCurrentIndex][this.bodyPartCurrentIndex]==this.orientationCurrentIndex)
                this.panel1.BackColor = System.Drawing.Color.Green;
            else
                this.panel1.BackColor = System.Drawing.Color.White;
            this.pictureBox1.Image = (System.Drawing.Image)((ArrayList)this.body_parts_orientations[this.bodyPartCurrentIndex])[this.orientationCurrentIndex];
            this.label2.Text = (string)((ArrayList)this.body_parts_orientations_names[this.bodyPartCurrentIndex])[this.orientationCurrentIndex];
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
             int countOrientations=((ArrayList)this.body_parts_orientations_names[this.bodyPartCurrentIndex]).Count;
            if (this.orientationCurrentIndex==0)
                this.orientationCurrentIndex = countOrientations - 1;
            else
                this.orientationCurrentIndex--;
            if (OrientationForm.orientationMatrix[this.activityCurrentIndex][this.bodyPartCurrentIndex]==this.orientationCurrentIndex)
                this.panel1.BackColor = System.Drawing.Color.Green;
            else
                this.panel1.BackColor = System.Drawing.Color.White;
            this.pictureBox1.Image = (System.Drawing.Image)((ArrayList)this.body_parts_orientations[this.bodyPartCurrentIndex])[this.orientationCurrentIndex];
            this.label2.Text = (string)((ArrayList)this.body_parts_orientations_names[this.bodyPartCurrentIndex])[this.orientationCurrentIndex];
            
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
      
            if ((OrientationForm.orientationMatrix[this.activityCurrentIndex][this.bodyPartCurrentIndex] ==-1) &&
                (this.panel1.BackColor == System.Drawing.Color.White))
            {
                this.panel1.BackColor = System.Drawing.Color.Green;
                OrientationForm.orientationMatrix[this.activityCurrentIndex][this.bodyPartCurrentIndex] = this.orientationCurrentIndex;
            }
            else if ((OrientationForm.orientationMatrix[this.activityCurrentIndex][this.bodyPartCurrentIndex] >= 0) &&
                (this.panel1.BackColor == System.Drawing.Color.Green))
            {
                this.panel1.BackColor = System.Drawing.Color.White;
                OrientationForm.orientationMatrix[this.activityCurrentIndex][this.bodyPartCurrentIndex] = -1;
            }
            else
            {
                MessageBox.Show("Please deselect your choices and try again");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (OrientationForm.orientationMatrix[this.activityCurrentIndex][this.bodyPartCurrentIndex] >= 0)
            {

                if (this.bodyPartCurrentIndex == this.sensors.Sensors.Count - 1)
                {

                    if (this.activityCurrentIndex == SimilarActivitiesForm.ConfusingActivities.Count - 1)
                        Cleanup();
                    else
                        this.activityCurrentIndex++;

                    this.bodyPartCurrentIndex = 0;
                }
                else if (this.bodyPartCurrentIndex == this.body_parts.Count - 1)
                    this.bodyPartCurrentIndex = 0;
                else
                    this.bodyPartCurrentIndex++;

                if (OrientationForm.orientationMatrix[this.activityCurrentIndex][this.bodyPartCurrentIndex] == 0)
                    this.panel1.BackColor = System.Drawing.Color.Green;
                else
                    this.panel1.BackColor = System.Drawing.Color.White;
                this.orientationCurrentIndex = 0;
                this.pictureBox1.Image = (System.Drawing.Image)((ArrayList)this.body_parts_orientations[this.bodyPartCurrentIndex])[0];
                this.label2.Text = (string)((ArrayList)this.body_parts_orientations_names[this.bodyPartCurrentIndex])[0];
                this.label1.Text = "During " + (string)SimilarActivitiesForm.ConfusingActivities[activityCurrentIndex] + ", how does your " + (string)this.body_parts[this.bodyPartCurrentIndex] + " look like?";
            }
            else
            {
                MessageBox.Show("Please choose an orientation.");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Visible = false;
        }
    }
}