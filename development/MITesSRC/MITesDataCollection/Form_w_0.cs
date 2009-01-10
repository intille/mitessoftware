using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;


namespace WocketsInterface
{
    public partial class Form_w_0 : Form
    {
        public Form_w_0()
        {
            InitializeComponent();
        }

        private void w_0_button_start_Click(object sender, EventArgs e)
        {
            Program.MainFormManager.CurrentForm = new Form_w_1();
           
        }

    }
}