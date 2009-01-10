namespace WocketsInterface
{
    partial class Form_w_0
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
            this.w_0_label_welcome = new System.Windows.Forms.Label();
            this.w_0_button_start = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // w_0_label_welcome
            // 
            this.w_0_label_welcome.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.w_0_label_welcome.Location = new System.Drawing.Point(31, 66);
            this.w_0_label_welcome.Name = "w_0_label_welcome";
            this.w_0_label_welcome.Size = new System.Drawing.Size(181, 49);
            this.w_0_label_welcome.Text = "Welcome to Wockets\r\nDemo";
            this.w_0_label_welcome.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // w_0_button_start
            // 
            this.w_0_button_start.Location = new System.Drawing.Point(86, 168);
            this.w_0_button_start.Name = "w_0_button_start";
            this.w_0_button_start.Size = new System.Drawing.Size(72, 20);
            this.w_0_button_start.TabIndex = 1;
            this.w_0_button_start.Text = "start";
            this.w_0_button_start.Click += new System.EventHandler(this.w_0_button_start_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.Controls.Add(this.w_0_button_start);
            this.Controls.Add(this.w_0_label_welcome);
            this.Menu = this.mainMenu1;
            this.Name = "Form1";
            this.Text = "Wockets";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label w_0_label_welcome;
        private System.Windows.Forms.Button w_0_button_start;
    }
}

