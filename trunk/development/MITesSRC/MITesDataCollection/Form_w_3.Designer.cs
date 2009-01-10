namespace WocketsInterface
{
    partial class Form_w_3
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
            this.w_4_label1 = new System.Windows.Forms.Label();
            this.w_4_button_Next = new System.Windows.Forms.Button();
            this.w_4_label_act = new System.Windows.Forms.Label();
            this.w_4_button_Back = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // w_4_label1
            // 
            this.w_4_label1.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.w_4_label1.Location = new System.Drawing.Point(15, 22);
            this.w_4_label1.Name = "w_4_label1";
            this.w_4_label1.Size = new System.Drawing.Size(204, 68);
            this.w_4_label1.Text = "Great! \r\nplease press Next\r\nto demostrate";
            this.w_4_label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // w_4_button_Next
            // 
            this.w_4_button_Next.Location = new System.Drawing.Point(147, 202);
            this.w_4_button_Next.Name = "w_4_button_Next";
            this.w_4_button_Next.Size = new System.Drawing.Size(72, 20);
            this.w_4_button_Next.TabIndex = 1;
            this.w_4_button_Next.Text = "Next";
            this.w_4_button_Next.Click += new System.EventHandler(this.w_4_button_Next_Click);
            // 
            // w_4_label_act
            // 
            this.w_4_label_act.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.w_4_label_act.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.w_4_label_act.Location = new System.Drawing.Point(15, 90);
            this.w_4_label_act.Name = "w_4_label_act";
            this.w_4_label_act.Size = new System.Drawing.Size(204, 24);
            this.w_4_label_act.Text = "activity";
            this.w_4_label_act.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // w_4_button_Back
            // 
            this.w_4_button_Back.Location = new System.Drawing.Point(15, 202);
            this.w_4_button_Back.Name = "w_4_button_Back";
            this.w_4_button_Back.Size = new System.Drawing.Size(72, 20);
            this.w_4_button_Back.TabIndex = 3;
            this.w_4_button_Back.Text = "Back";
            this.w_4_button_Back.Click += new System.EventHandler(this.w_4_button_Back_Click);
            // 
            // Form_w_3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.Controls.Add(this.w_4_button_Back);
            this.Controls.Add(this.w_4_label_act);
            this.Controls.Add(this.w_4_button_Next);
            this.Controls.Add(this.w_4_label1);
            this.Menu = this.mainMenu1;
            this.Name = "Form_w_3";
            this.Text = "Wockets";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label w_4_label1;
        private System.Windows.Forms.Button w_4_button_Next;
        private System.Windows.Forms.Label w_4_label_act;
        private System.Windows.Forms.Button w_4_button_Back;
    }
}