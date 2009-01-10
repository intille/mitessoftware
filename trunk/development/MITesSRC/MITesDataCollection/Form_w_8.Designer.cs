namespace WocketsInterface
{
    partial class Form_w_8
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
            this.w_8_label_1 = new System.Windows.Forms.Label();
            this.w_8_label_2 = new System.Windows.Forms.Label();
            this.w_8_button_OK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // w_8_label_1
            // 
            this.w_8_label_1.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.w_8_label_1.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.w_8_label_1.Location = new System.Drawing.Point(56, 32);
            this.w_8_label_1.Name = "w_8_label_1";
            this.w_8_label_1.Size = new System.Drawing.Size(100, 20);
            this.w_8_label_1.Text = "Ready!";
            this.w_8_label_1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // w_8_label_2
            // 
            this.w_8_label_2.Location = new System.Drawing.Point(21, 79);
            this.w_8_label_2.Name = "w_8_label_2";
            this.w_8_label_2.Size = new System.Drawing.Size(197, 40);
            this.w_8_label_2.Text = "You can start doing any of the activities.";
            // 
            // w_8_button_OK
            // 
            this.w_8_button_OK.Location = new System.Drawing.Point(83, 175);
            this.w_8_button_OK.Name = "w_8_button_OK";
            this.w_8_button_OK.Size = new System.Drawing.Size(72, 20);
            this.w_8_button_OK.TabIndex = 2;
            this.w_8_button_OK.Text = "OK";
            this.w_8_button_OK.Click += new System.EventHandler(this.w_8_button_OK_Click);
            // 
            // Form_w_8
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.Controls.Add(this.w_8_button_OK);
            this.Controls.Add(this.w_8_label_2);
            this.Controls.Add(this.w_8_label_1);
            this.Menu = this.mainMenu1;
            this.Name = "Form_w_8";
            this.Text = "Form_w_8";
            this.Closed += new System.EventHandler(this.Form_w_8_Closed);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label w_8_label_1;
        private System.Windows.Forms.Label w_8_label_2;
        private System.Windows.Forms.Button w_8_button_OK;
    }
}