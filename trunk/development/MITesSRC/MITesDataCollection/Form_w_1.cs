using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WocketsInterface
{
    public partial class Form_w_1 : Form
    {
        public static double w_1_weight;
        private static string weight;

        public Form_w_1()
        {
            InitializeComponent();
        }

        private void w_1_button_Clear_Click(object sender, EventArgs e)
        {
            w_1_label_Weight.Text = "";
        }

        private void w_1_button_n1_Click(object sender, EventArgs e)
        {
            w_1_label_Weight.Text = w_1_label_Weight.Text + "1";
        }

        private void w_1_button_n2_Click(object sender, EventArgs e)
        {
            w_1_label_Weight.Text = w_1_label_Weight.Text + "2";
        }

        private void w_1_button_n3_Click(object sender, EventArgs e)
        {
            w_1_label_Weight.Text = w_1_label_Weight.Text + "3";
        }

        private void w_1_button_n4_Click(object sender, EventArgs e)
        {
            w_1_label_Weight.Text = w_1_label_Weight.Text + "4";
        }

        private void w_1_button_n5_Click(object sender, EventArgs e)
        {
            w_1_label_Weight.Text = w_1_label_Weight.Text + "5";
        }

        private void w_1_button_n6_Click(object sender, EventArgs e)
        {
            w_1_label_Weight.Text = w_1_label_Weight.Text + "6";
        }

        private void w_1_button_n7_Click(object sender, EventArgs e)
        {
            w_1_label_Weight.Text = w_1_label_Weight.Text + "7";
        }

        private void w_1_button_n8_Click(object sender, EventArgs e)
        {
            w_1_label_Weight.Text = w_1_label_Weight.Text + "8";
        }

        private void w_1_button_n9_Click(object sender, EventArgs e)
        {
            w_1_label_Weight.Text = w_1_label_Weight.Text + "9";
        }

        private void w_1_button_n0_Click(object sender, EventArgs e)
        {
            w_1_label_Weight.Text = w_1_label_Weight.Text + "0";
        }

        private void w_1_button_np_Click(object sender, EventArgs e)
        {
            w_1_label_Weight.Text = w_1_label_Weight.Text + ".";
        }

        private void w_1_button_next_Click(object sender, EventArgs e)
        {
            if (w_1_label_Weight.Text.CompareTo("") == 0)
            { w_1_weight = 0; }
            else
            {
                weight = w_1_label_Weight.Text;
                w_1_weight = System.Convert.ToDouble(weight); 
            }


            Program.MainFormManager.CurrentForm = new Form_w_2();
           
        }

     

    }
}