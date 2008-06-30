using System;
using System.Collections;

namespace MITesDataCollection
{
    partial class MobilityForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.MainMenu mainMenu1;


        private AXML.Annotation annotation;
        private SXML.SensorAnnotation sensors;
        private int activityIndex=0;
        private static int[][] mobilityMatrix;
        //private int[][] mobilityIndex;
        private ArrayList sensorLabels;
        private ArrayList sensorButtons;

        private void InitializeInterface()
        {
            string activityName = (string)SimilarActivitiesForm.ConfusingActivities[this.activityIndex];
            this.label1.Text = "During " + activityName + ", how mobile are the following areas of your body?";

            this.sensorLabels = new ArrayList();
            this.sensorButtons = new ArrayList();

            //Initialize Mobility Matrix
            MobilityForm.mobilityMatrix= new int[SimilarActivitiesForm.ConfusingActivities.Count][];
           // this.mobilityIndex = new int[SimilarActivitiesForm.ConfusingActivities.Count][];
            SXML.Reader sreader = new SXML.Reader(Constants.MASTER_DIRECTORY, TroubleshootModel.SelectedFolder);
            if (sreader.validate() == false)
            {
                throw new Exception("Error Code 0: XML format error - sensors.xml does not match sensors.xsd!");
            }
            else
            {
                this.sensors = sreader.parse(Constants.MAX_CONTROLLERS);
            }

            for (int i = 0; (i < MobilityForm.mobilityMatrix.Length); i++)
            {
                MobilityForm.mobilityMatrix[i] = new int[this.sensors.Sensors.Count];
                //this.mobilityIndex[i] = new int[this.sensors.Sensors.Count];
                for (int j = 0; (j < this.sensors.Sensors.Count); j++)
                {
                    MobilityForm.mobilityMatrix[i][j] = 0;
                    //this.mobilityIndex[i][j] = 0;
                }
            }

            int index = 0;
            foreach (SXML.Sensor sensor in this.sensors.Sensors)
            {

                System.Windows.Forms.Label label = new System.Windows.Forms.Label();
                label.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
                label.Location = new System.Drawing.Point(16, 97+ (30 * index));
                label.Name = Convert.ToString(index);
                label.Size = new System.Drawing.Size(56, 19);
                label.Text = sensor.Location;
                this.Controls.Add(label);
                this.sensorLabels.Add(label);

                System.Windows.Forms.Button button = new System.Windows.Forms.Button();
                button.Location = new System.Drawing.Point(78, 87 + (30 * index));
                button.Name = Convert.ToString(index);
                button.Size = new System.Drawing.Size(120, 30);
                button.TabIndex = index;
                button.Text = Constants.MOBILITY_FORM_NOT_MOBILE_LABEL;
                button.Click += new System.EventHandler(this.button_Click);
                this.Controls.Add(button);
                this.sensorButtons.Add(button);
                index++;
            }

            this.button2.Location = new System.Drawing.Point(78, 87 + (30 * index));
            this.button3.Location = new System.Drawing.Point(16, 87 + (30 * index));
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.mainMenu1 = new System.Windows.Forms.MainMenu();
            this.label1 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(16, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(209, 55);
            this.label1.Text = "During Walking, how mobile are the following areas of your body?";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(153, 165);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(72, 20);
            this.button2.TabIndex = 3;
            this.button2.Text = "Next";
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(16, 165);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(72, 20);
            this.button3.TabIndex = 4;
            this.button3.Text = "Back";
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // MobilityForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label1);
            this.Menu = this.mainMenu1;
            this.Name = "MobilityForm";
            this.Text = "MobilityForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
    }
}