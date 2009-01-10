using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WocketsInterface
{
    public partial class Form_w_5 : Form
    {
        public Form_w_5()
        {
            InitializeComponent();
        }

        private void w_5_button_start_over_Click(object sender, EventArgs e)
        {
            Program.MainFormManager.CurrentForm = new Form_w_2();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Program.MainFormManager.CurrentForm = new Form_w_6();
        }
    }
}