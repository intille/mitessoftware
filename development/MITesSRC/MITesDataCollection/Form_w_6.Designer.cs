namespace WocketsInterface
{
    partial class Form_w_6
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
            this.w_6_label_1 = new System.Windows.Forms.Label();
            this.w_6_button_other = new System.Windows.Forms.Button();
            this.w_6_button_done = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // w_6_label_1
            // 
            this.w_6_label_1.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.w_6_label_1.Location = new System.Drawing.Point(68, 37);
            this.w_6_label_1.Name = "w_6_label_1";
            this.w_6_label_1.Size = new System.Drawing.Size(100, 20);
            this.w_6_label_1.Text = "Thanks!";
            this.w_6_label_1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // w_6_button_other
            // 
            this.w_6_button_other.Location = new System.Drawing.Point(75, 117);
            this.w_6_button_other.Name = "w_6_button_other";
            this.w_6_button_other.Size = new System.Drawing.Size(86, 20);
            this.w_6_button_other.TabIndex = 1;
            this.w_6_button_other.Text = "Train other";
            this.w_6_button_other.Click += new System.EventHandler(this.w_6_button_other_Click);
            // 
            // w_6_button_done
            // 
            this.w_6_button_done.Location = new System.Drawing.Point(75, 174);
            this.w_6_button_done.Name = "w_6_button_done";
            this.w_6_button_done.Size = new System.Drawing.Size(86, 20);
            this.w_6_button_done.TabIndex = 2;
            this.w_6_button_done.Text = "Done";
            this.w_6_button_done.Click += new System.EventHandler(this.w_6_button_done_Click);
            // 
            // Form_w_6
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.Controls.Add(this.w_6_button_done);
            this.Controls.Add(this.w_6_button_other);
            this.Controls.Add(this.w_6_label_1);
            this.Menu = this.mainMenu1;
            this.Name = "Form_w_6";
            this.Text = "Form_w_6";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label w_6_label_1;
        private System.Windows.Forms.Button w_6_button_other;
        private System.Windows.Forms.Button w_6_button_done;
    }
}