using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WocketsInterface
{
    public partial class Form_w_8 : Form
    {
        public Form_w_8()
        {
            InitializeComponent();
        }

        private void w_8_button_OK_Click(object sender, EventArgs e)
        {
            Program.MainFormManager.CurrentForm.Close();
        }

        private void Form_w_8_Closed(object sender, EventArgs e)
        {
            Application.Exit();
        }


    }
}