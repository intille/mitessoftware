using MITesDataCollection.Utils;
using System.Drawing;

namespace MITesDataCollection
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            //Initialize form size based on screen size
           
            this.Width = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width - Constants.SCREEN_WIDTH_MARGIN;
            this.Height = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height - Constants.SCREEN_HEIGHT_MARGIN;
            if ((this.Width > Constants.MAX_SCREEN_WIDTH) || (this.Height > Constants.MAX_SCREEN_HEIGHT))
            {
                this.Width = this.Width / 2;
                this.Height = this.Height / 2;
            }

            int widgetHeight = (this.ClientSize.Height - ((Constants.MAIN_FORM_WIDGETS_COUNT+1)*Constants.WIDGET_SPACING)) / Constants.MAIN_FORM_WIDGETS_COUNT;
            int widgetWidth=this.ClientSize.Width - Constants.WIDGET_SPACING - Constants.WIDGET_SPACING;


            Size oldSize = new Size(this.button2.Width, this.button2.Height);
            this.button2.Width = this.button3.Width = this.button4.Width =  widgetWidth;
            this.button2.Height = this.button3.Height = this.button4.Height = widgetHeight;
            this.button2.Location = new Point(Constants.WIDGET_SPACING, Constants.WIDGET_SPACING);
            this.button3.Location = new Point(Constants.WIDGET_SPACING, Constants.WIDGET_SPACING + this.button2.Location.Y + this.button2.Height);
            this.button4.Location = new Point(Constants.WIDGET_SPACING, Constants.WIDGET_SPACING + this.button3.Location.Y + this.button3.Height);
            this.button2.Font = this.button3.Font = this.button4.Font =
                GUI.CalculateBestFitFont(this.button2.Parent.CreateGraphics(), Constants.MIN_FONT,
                   Constants.MAX_FONT, this.button2.Size, Constants.MAIN_FORM_BUTTON1, this.button2.Font, (float)0.9, (float)0.9);

            this.button2.Text = Constants.MAIN_FORM_BUTTON1;
            this.button3.Text = Constants.MAIN_FORM_BUTTON2;
            this.button4.Text = Constants.MAIN_FORM_BUTTON3;

            this.button4.Enabled = false;
            this.button3.Enabled = false;
                  
        }



        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem1});
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 0;
            this.menuItem1.Text = "Quit";
            this.menuItem1.Click += new System.EventHandler(this.menuItem1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(26, 56);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(195, 30);
            this.button2.TabIndex = 8;
            this.button2.Text = "Collect MITes Data";
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(26, 92);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(195, 25);
            this.button3.TabIndex = 9;
            this.button3.Text = "Estimate Energy Expenditure";
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(26, 128);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(195, 26);
            this.button4.TabIndex = 10;
            this.button4.Text = "Correct Software";
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(257, 264);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Menu = this.mainMenu1;
            this.Name = "MainForm";
            this.Text = "MITes Software";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.MainMenu mainMenu1;
        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;

    }
}

