using MITesDataCollection.Utils;
using System.Drawing;

namespace MITesDataCollection
{
    partial class WhereStoreDataForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.MainMenu mainMenu1;

        public void InitializeInterface()
        {

            this.Width = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width - Constants.SCREEN_WIDTH_MARGIN;
            this.Height = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height - Constants.SCREEN_HEIGHT_MARGIN;
            if ((this.Width > Constants.MAX_SCREEN_WIDTH) || (this.Height > Constants.MAX_SCREEN_HEIGHT))
            {
                this.Width = this.Width / 2;
                this.Height = this.Height / 2;
            }

            int widgetHeight = (this.ClientSize.Height - ((Constants.WHERE_FORM_WIDGETS_COUNT + 1) * Constants.WIDGET_SPACING)) / Constants.WHERE_FORM_WIDGETS_COUNT;
            int widgetWidth = (int)((this.ClientSize.Width - Constants.WIDGET_SPACING - Constants.SCREEN_LEFT_MARGIN- Constants.SCREEN_RIGHT_MARGIN- Constants.WIDGET_SPACING));

            widgetHeight =(int)(0.9 * widgetHeight);
            widgetWidth = (int) (0.9 * widgetWidth);

            //adjust top label size and location
            this.label1.Text = Constants.WHERE_FORM_LABEL1;
            this.label1.Width = widgetWidth;
            this.label1.Height = widgetHeight;
            this.label1.Location = new Point((int)Constants.SCREEN_LEFT_MARGIN, (int)Constants.SCREEN_TOP_MARGIN);
            this.label1.Font = GUI.CalculateBestFitFont(this.label1.Parent.CreateGraphics(), Constants.MIN_FONT,
                   Constants.MAX_FONT, this.label1.Size, Constants.WHERE_FORM_LABEL1, this.label1.Font, (float)0.9, (float)0.9);

            //adjust textbox
            this.textBox1.Width = widgetWidth;
            this.textBox1.Height = widgetHeight;
            this.textBox1.Location = new Point((int)Constants.SCREEN_LEFT_MARGIN, Constants.SCREEN_TOP_MARGIN+this.label1.Location.Y+this.label1.Height+Constants.WIDGET_SPACING);
            this.textBox1.Font = this.label1.Font;

            this.button1.Width = widgetWidth;
            this.button1.Height = widgetHeight;
            this.button1.Location = new Point((int)Constants.SCREEN_LEFT_MARGIN, Constants.SCREEN_TOP_MARGIN + this.textBox1.Location.Y + this.textBox1.Height + Constants.WIDGET_SPACING);
            this.button1.Font = GUI.CalculateBestFitFont(this.button1.Parent.CreateGraphics(), Constants.MIN_FONT,
                   Constants.MAX_FONT, this.button1.Size, Constants.WHERE_FORM_LABEL1, this.button1.Font, (float)0.9, (float)0.9);


            this.button2.Width = (int)(widgetWidth * Constants.WHERE_BUTTONS_WIDTH_PERCENTAGE);
            this.button3.Width = (int)(widgetWidth * Constants.WHERE_BUTTONS_WIDTH_PERCENTAGE);
            this.button2.Height = (int)(widgetHeight * Constants.WHERE_BUTTONS_HEIGHT_PERCENTAGE);
            this.button3.Height = (int)(widgetHeight * Constants.WHERE_BUTTONS_HEIGHT_PERCENTAGE);


            this.button3.Location = new Point((int)Constants.SCREEN_LEFT_MARGIN, (int)(this.button1.Location.Y + this.button1.Height + Constants.WIDGET_SPACING));
            this.button2.Location = new Point((int)(Constants.SCREEN_LEFT_MARGIN + this.button3.Width + Constants.WIDGET_SPACING),
                (int)(this.button1.Location.Y + this.button1.Height + Constants.WIDGET_SPACING));
            this.button2.Font = this.button3.Font = GUI.CalculateBestFitFont(this.button2.Parent.CreateGraphics(), Constants.MIN_FONT,
                Constants.MAX_FONT, this.button2.Size, "Next", this.button2.Font, (float)0.9, (float)0.9);
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
            this.components = new System.ComponentModel.Container();
            this.mainMenu1 = new System.Windows.Forms.MainMenu();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.Add(this.menuItem1);
            
            //.AddRange(new System.Windows.Forms.MenuItem[] {
            //this.menuItem1});
            // 
            // menuItem1
            // 
            //this.menuItem1.Index = 0;
            this.menuItem1.Text = "Quit";
            this.menuItem1.Click += new System.EventHandler(this.menuItem1_Click);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(16, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(199, 32);
            this.label1.TabIndex = 5;
            this.label1.Text = "Where do you want to store your data?";
            // 
            // textBox1
            // 
            this.textBox1.Enabled = false;
            this.textBox1.Location = new System.Drawing.Point(16, 70);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(199, 20);
            this.textBox1.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(16, 97);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(199, 20);
            this.button1.TabIndex = 2;
            this.button1.Text = "Choose a directory";
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Enabled = false;
            this.button2.Location = new System.Drawing.Point(143, 139);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(72, 20);
            this.button2.TabIndex = 3;
            this.button2.Text = "Next";
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(16, 139);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(72, 20);
            this.button3.TabIndex = 4;
            this.button3.Text = "Back";
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // WhereStoreDataForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label1);
            this.Menu = this.mainMenu1;
            this.Name = "WhereStoreDataForm";
            this.Text = "Collect Data...";
            this.ResumeLayout(false);
            //this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
    }
}