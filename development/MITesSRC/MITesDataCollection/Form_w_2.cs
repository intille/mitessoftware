using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WocketsInterface
{
    public partial class Form_w_2 : Form
    {
        public Form_w_2()
        {
            InitializeComponent();
        }


        private void gotonextForm()
        {
            Program.MainFormManager.CurrentForm = new Form_w_3();
        }


        private void w_2_button_act_1_Click(object sender, EventArgs e)
        {
            gotonextForm();
        }

        
        private void w_2_button_act_2_Click(object sender, EventArgs e)
        {
            gotonextForm();
        }

        private void w_2_button_act_3_Click(object sender, EventArgs e)
        {
            gotonextForm();
        }

        private void w_2_button_act_4_Click(object sender, EventArgs e)
        {
            gotonextForm();
        }

        private void w_2_button_act_5_Click(object sender, EventArgs e)
        {
            gotonextForm();
        }

        private void w_2_button_act_6_Click(object sender, EventArgs e)
        {
            gotonextForm();
        }

        private void w_2_button_act_7_Click(object sender, EventArgs e)
        {
            gotonextForm();
        }

        private void w_2_button_act_other_Click(object sender, EventArgs e)
        {
            // Check with Fadh what he wants to do here
            gotonextForm();
        } 


    }
}