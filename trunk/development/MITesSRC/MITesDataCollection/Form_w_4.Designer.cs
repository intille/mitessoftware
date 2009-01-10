namespace WocketsInterface
{
    partial class Form_w_4
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.mainMenu1 = new System.Windows.Forms.MainMenu();
            this.w_4_label_1 = new System.Windows.Forms.Label();
            this.w_4_label_act = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.w_4_button_start = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // w_4_label_1
            // 
            this.w_4_label_1.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.w_4_label_1.Location = new System.Drawing.Point(18, 39);
            this.w_4_label_1.Name = "w_4_label_1";
            this.w_4_label_1.Size = new System.Drawing.Size(55, 20);
            this.w_4_label_1.Text = "Start";
            // 
            // w_4_label_act
            // 
            this.w_4_label_act.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.w_4_label_act.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.w_4_label_act.Location = new System.Drawing.Point(67, 39);
            this.w_4_label_act.Name = "w_4_label_act";
            this.w_4_label_act.Size = new System.Drawing.Size(141, 20);
            this.w_4_label_act.Text = "Activity";
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.label3.Location = new System.Drawing.Point(18, 59);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(190, 46);
            this.label3.Text = "Until you hear a beap indicating to stop.";
            // 
            // w_4_button_start
            // 
            this.w_4_button_start.Location = new System.Drawing.Point(75, 169);
            this.w_4_button_start.Name = "w_4_button_start";
            this.w_4_button_start.Size = new System.Drawing.Size(72, 20);
            this.w_4_button_start.TabIndex = 3;
            this.w_4_button_start.Text = "Start";
            this.w_4_button_start.Click += new System.EventHandler(this.w_4_button_start_Click);
            // 
            // Form_w_4
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.Controls.Add(this.w_4_button_start);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.w_4_label_act);
            this.Controls.Add(this.w_4_label_1);
            this.Menu = this.mainMenu1;
            this.Name = "Form_w_4";
            this.Text = "Wockets";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label w_4_label_1;
        private System.Windows.Forms.Label w_4_label_act;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button w_4_button_start;
    }
}