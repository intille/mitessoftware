using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace MITesDataCollection
{
    public partial class ProgressForm : Form
    {
        public ProgressForm()
        {
            //InitializeComponent();
            InitializeInterface();
        }

        public void UpdateProgressBar(string description)
        {
            this.AppendLog(description);
            this.Update();
            progressBar9.MarqueeUpdate(); 
        }

        public void UpdateProgressBar()
        {
            this.Update();
            progressBar9.MarqueeUpdate(); 
        }
        private void marqueeTimer_Tick(object sender, EventArgs e)
        {

            progressBar9.MarqueeUpdate();           
        }

 
    }
}