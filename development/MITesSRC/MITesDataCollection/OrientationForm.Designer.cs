using System;
using System.Windows.Forms;
using System.Collections;
using BodyXML;
namespace MITesDataCollection
{
    partial class OrientationForm
    {
        /// <summary>
        /// Required designer variable
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.MainMenu mainMenu1;
        //private System.Windows.Forms.ImageList imageList1,imageList2,imageList3;
        private ArrayList body_parts;
        private ArrayList body_parts_orientations;
        private ArrayList body_parts_orientations_names;
        
        private static int[][] orientationMatrix;
        //private int[][] orientationIndex;
        private SXML.SensorAnnotation sensors;
        private BodyXML.BodyParts bodyParts;
        
        private int bodyPartCurrentIndex;
        private int orientationCurrentIndex;
        private int activityCurrentIndex;

        private void InitializeInterface()
        {
            
            orientationMatrix = new int[SimilarActivitiesForm.ConfusingActivities.Count][];
            //orientationIndex = new int[SimilarActivitiesForm.ConfusingActivities.Count][];
            SXML.Reader sreader = new SXML.Reader(Constants.MASTER_DIRECTORY, TroubleshootModel.SelectedFolder);
            if (sreader.validate() == false)
            {
                throw new Exception("Error Code 0: XML format error - sensors.xml does not match sensors.xsd!");
            }
            else
            {
                this.sensors = sreader.parse(Constants.MAX_CONTROLLERS);
            }

            for (int i = 0; (i < OrientationForm.orientationMatrix.Length); i++)
            {
                OrientationForm.orientationMatrix[i] = new int[this.sensors.Sensors.Count];
            //    this.orientationIndex[i] = new int[this.sensors.Sensors.Count];
                for (int j = 0; (j < this.sensors.Sensors.Count); j++)
                {
                    OrientationForm.orientationMatrix[i][j] = -1;
          //          this.orientationIndex[i][j] = 0;
                }
            }


            BodyXML.Parser parser = new BodyXML.Parser(Constants.MASTER_DIRECTORY);
            this.bodyParts = parser.parse();
            this.body_parts=new ArrayList();
            this.body_parts_orientations= new ArrayList();
            this.body_parts_orientations_names= new ArrayList();
            int maxImageWidth = 0;
            int maxImageHeight = 0;
            foreach (BodyPart bodyPart in this.bodyParts.Bodyparts)
            {
                this.body_parts.Add(bodyPart.Label);
                ArrayList orientations= new ArrayList();
                ArrayList names=new ArrayList();
                foreach (BodyXML.Orientation orientation in bodyPart.Orientations){      
                    System.Drawing.Image orientationImage=(System.Drawing.Image) new System.Drawing.Bitmap(Constants.NEEDED_FILES_PATH + "images\\orientations\\"+orientation.Imagefile);
                    if (orientationImage.Width > maxImageWidth)
                        maxImageWidth = orientationImage.Width;
                    if (orientationImage.Height > maxImageHeight)
                        maxImageHeight = orientationImage.Height;

                    orientations.Add(orientationImage);
                    names.Add(orientation.Label);
                }
                this.body_parts_orientations.Add(orientations);
                this.body_parts_orientations_names.Add(names);
            }

            this.bodyPartCurrentIndex = 0;
            this.orientationCurrentIndex = 0;
            this.activityCurrentIndex = 0;
            //this.pictureBox1.Width = maxImageWidth+Constants.WIDGET_SPACING*2;
            //this.pictureBox1.Height = maxImageHeight + Constants.WIDGET_SPACING * 2;
            //this.panel1.Width = this.pictureBox1.Width + Constants.WIDGET_SPACING * 2;
            //this.panel1.Height = this.pictureBox1.Height + Constants.WIDGET_SPACING * 2;
            this.panel1.BackColor = System.Drawing.Color.White;
            //this.button1.Location = new System.Drawing.Point(this.panel1.Location.X, this.panel1.Location.Y + this.panel1.Location.Y);
            //this.button2.Location = new System.Drawing.Point(this.panel1.Location.X, this.panel1.Location.Y + this.button1.Location.Y+this.button1.Width+Constants.WIDGET_SPACING);
            this.pictureBox1.Image = (System.Drawing.Image)((ArrayList)this.body_parts_orientations[this.bodyPartCurrentIndex])[this.orientationCurrentIndex];
            this.label2.Text = (string)((ArrayList)this.body_parts_orientations_names[this.bodyPartCurrentIndex])[this.orientationCurrentIndex];
            this.label1.Text = "During " + (string)SimilarActivitiesForm.ConfusingActivities[activityCurrentIndex] + ", how does your "+(string)this.body_parts[this.bodyPartCurrentIndex]+" look like?";

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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(20, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(184, 42);
            this.label1.Text = "During walking, how does your arm look like?";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(55, 64);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(86, 99);
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular);
            this.button1.Location = new System.Drawing.Point(174, 114);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(30, 20);
            this.button1.TabIndex = 2;
            this.button1.Text = ">>";
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(3, 114);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(36, 20);
            this.button2.TabIndex = 3;
            this.button2.Text = "<<";
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(106, 194);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(72, 20);
            this.button3.TabIndex = 4;
            this.button3.Text = "Next";
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(3, 194);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(72, 20);
            this.button4.TabIndex = 5;
            this.button4.Text = "Back";
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(162, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 50);
            this.label2.Text = "label2";
            // 
            // panel1
            // 
            this.panel1.Location = new System.Drawing.Point(44, 51);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(112, 128);
            // 
            // OrientationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.Menu = this.mainMenu1;
            this.Name = "OrientationForm";
            this.Text = "OrientationForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private PictureBox pictureBox1;
        private Button button1;
        private Button button2;
        private Button button3;
        private Button button4;
        private Label label2;
        private Panel panel1;
    }
}