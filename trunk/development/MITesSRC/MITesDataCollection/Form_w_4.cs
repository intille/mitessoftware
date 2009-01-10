using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WocketsInterface
{
    public partial class Form_w_4 : Form
    {
        public Form_w_4()
        {
            InitializeComponent();
        }

        private void w_4_button_start_Click(object sender, EventArgs e)
        {
            Program.MainFormManager.CurrentForm = new Form_w_5();
        }
    }
}