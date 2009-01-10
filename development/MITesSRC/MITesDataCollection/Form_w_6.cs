using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WocketsInterface
{
    public partial class Form_w_6 : Form
    {
        public Form_w_6()
        {
            InitializeComponent();
        }

        private void w_6_button_other_Click(object sender, EventArgs e)
        {
            Program.MainFormManager.CurrentForm = new Form_w_2();
        }

        private void w_6_button_done_Click(object sender, EventArgs e)
        {
            Program.MainFormManager.CurrentForm = new Form_w_7();
        }

    }
}