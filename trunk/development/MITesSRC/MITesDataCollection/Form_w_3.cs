using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WocketsInterface
{
    public partial class Form_w_3 : Form
    {
        public Form_w_3()
        {
            InitializeComponent();
        }

        private void w_4_button_Next_Click(object sender, EventArgs e)
        {
            Program.MainFormManager.CurrentForm = new Form_w_4();
        }

        private void w_4_button_Back_Click(object sender, EventArgs e)
        {
            Program.MainFormManager.CurrentForm = new Form_w_2();
        }


    }
}