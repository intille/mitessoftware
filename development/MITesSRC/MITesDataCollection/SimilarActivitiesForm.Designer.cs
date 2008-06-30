using System;

namespace MITesDataCollection
{
    partial class SimilarActivitiesForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.MainMenu mainMenu1;


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

        private void InitializeInterface()
        {
            string arffFile = TroubleshootModel.SelectedFile;  
            //parse the AXML
            AXML.Reader reader = new AXML.Reader(Constants.MASTER_DIRECTORY, TroubleshootModel.SelectedFolder);
            if (reader.validate() == false)
            {
                throw new Exception("Error Code 0: XML format error - activities.xml does not match activities.xsd!");
            }
            else
            {
                this.annotation = reader.parse();
                this.annotation.DataDirectory = TroubleshootModel.SelectedFolder;
            }

            int cindex=0;
            foreach (AXML.Label label in ((AXML.Category)this.annotation.Categories[0]).Labels)
            {
                System.Windows.Forms.CheckBox checkbox = new System.Windows.Forms.CheckBox();                    
                checkbox.Location = new System.Drawing.Point(18, 59 + 25 * cindex);                    
                checkbox.Name = "checkBox" + cindex;                
                checkbox.Size = new System.Drawing.Size(100, 20);                
                checkbox.TabIndex = cindex;                
                checkbox.Text = label.Name;                
                this.Controls.Add(checkbox);                
                this.checkboxes.Add(checkbox);                
                cindex++;
            }

            this.button1.Location = new System.Drawing.Point(this.button1.Location.X, 59 + 25 * cindex);
            this.button2.Location = new System.Drawing.Point(this.button2.Location.X, 59 + 25 * cindex);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.mainMenu1 = new System.Windows.Forms.MainMenu();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(18, 168);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(72, 20);
            this.button1.TabIndex = 0;
            this.button1.Text = "Back";
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(135, 168);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(72, 20);
            this.button2.TabIndex = 1;
            this.button2.Text = "Next";
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(18, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(189, 33);
            this.label1.Text = "What activities are confused?";
            // 
            // SimilarActivitiesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Menu = this.mainMenu1;
            this.Name = "SimilarActivitiesForm";
            this.Text = "SimilarActivitiesForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Collections.ArrayList checkboxes;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label1;
    }
}