using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Bornander.UI.ProgressBar.Test
{
    public partial class TestForm : Form
    {
        public TestForm()
        {
            InitializeComponent();
        }

        private void trackBar_Scroll(object sender, EventArgs e)
        {
            progressBar1.Value = trackBar.Value;
            progressBar2.Value = trackBar.Value;
            progressBar3.Value = trackBar.Value;
            progressBar4.Value = trackBar.Value;
            progressBar5.Value = trackBar.Value;
        }


        private void marqueeTimer_Tick(object sender, EventArgs e)
        {
            progressBar6.Value = (progressBar6.Value + 1) % progressBar6.Maximum;
            progressBar7.MarqueeUpdate();
            progressBar8.MarqueeUpdate();
            progressBar9.MarqueeUpdate();
            progressBar10.MarqueeUpdate();
            progressBar11.MarqueeUpdate();
        }

    }
}