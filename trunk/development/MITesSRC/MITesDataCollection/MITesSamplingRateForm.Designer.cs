using System;

namespace MITesDataCollection
{
    partial class MITesSamplingRateForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label[] labels = new System.Windows.Forms.Label[180];

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

        public void updateInterface()
        {
            // The loop goes through each sensor specified in sensor.xml and calculates received samples/expected samples * 100%
            foreach (SXML.Sensor sensor in this.sensorConfiguration.Sensors)
            {
                int sensor_id = Convert.ToInt32(sensor.ID);
                double rate = ((double)HousenCS.MITes.MITesDataFilterer.MITesPerformanceTracker[sensor_id].PreviousCounter / (double)HousenCS.MITes.MITesDataFilterer.MITesPerformanceTracker[sensor_id].GoodRate) * 100;
                if (rate > 100)
                    rate = 100;
                else if (rate < 0)
                    rate = 0;
                this.labels[sensor_id].Text = rate.ToString("00.00") + "%";
            }
        }

        private void InitializeInterface()
        {
            int counter = 0;


            foreach (SXML.Sensor sensor in this.sensorConfiguration.Sensors)
            {
                int sensor_id = Convert.ToInt32(sensor.ID);
                double rate = ((double)HousenCS.MITes.MITesDataFilterer.MITesPerformanceTracker[sensor_id].PreviousCounter / (double)HousenCS.MITes.MITesDataFilterer.MITesPerformanceTracker[sensor_id].GoodRate) * 100;
                if (rate > 100)
                    rate = 100;
                if (rate < 0)
                    rate = 0;

                System.Windows.Forms.Label label = new System.Windows.Forms.Label();
                //label.AutoSize = true;
                label.Location = new System.Drawing.Point(25, 37 + (counter * 30));
                label.Name = sensor.ID;
                label.Size = new System.Drawing.Size(35, 13);
                label.Text = "Sensor " + sensor.ID;
                Controls.Add(label);

                System.Windows.Forms.Label label2 = new System.Windows.Forms.Label();
                //label2.AutoSize = true;
                label2.Location = new System.Drawing.Point(81, 37 + (counter * 30));
                label2.Name = "expectedsampling" + sensor.ID;
                label2.Size = new System.Drawing.Size(35, 13);
                label2.Text = rate.ToString("00.00") + "%";
                Controls.Add(label2);
                this.labels[sensor_id] = label2;

                counter++;
            }
        }



        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            //this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);            
            this.label1.Text = "Sensor ID";
            // 
            // label2
            // 
            //this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.label2.Location = new System.Drawing.Point(81, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(139, 13);
            this.label2.Text = "% of Expected Samples";
            // 
            // MitesSamplingRate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(232, 413);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "MitesSamplingRate";
            this.Text = "Sampling Rates";
            this.Load += new System.EventHandler(this.MitesSamplingRate_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}