using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MITesLogger_PC
{

    /// <summary>
    /// 
    /// </summary>
    public delegate void ShowMITesFormDelegate();

    /// <summary>
    /// 
    /// </summary>
    public partial class SubjectForm : Form
    {

        /// <summary>
        /// 
        /// </summary>
        public ShowMITesFormDelegate ShowMITesFormCallback;

        private string dataDirectory;
        /// <summary>
        /// 
        /// </summary>
        public SubjectForm(string dataDirectory)
        {
            InitializeComponent();
            this.dataDirectory = dataDirectory;
        }

        private void SubjectForm_Load(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
           
            SubjectXML.Subjects subjects = new SubjectXML.Subjects(this.dataDirectory );
            try
            {
                subjects.FirstSubject.ID = this.textBox1.Text;
                subjects.FirstSubject.Age = Convert.ToInt32(this.textBox2.Text);
                subjects.FirstSubject.Sex = this.textBox3.Text;
                subjects.FirstSubject.Weight = Convert.ToDouble(this.textBox4.Text);
                subjects.FirstSubject.Height = Convert.ToDouble(this.textBox5.Text);
                subjects.FirstSubject.FatMass = Convert.ToDouble(this.textBox6.Text);
                subjects.FirstSubject.BoneMass = Convert.ToDouble(this.textBox7.Text);
                subjects.FirstSubject.WaterMass = Convert.ToDouble(this.textBox8.Text);
                subjects.FirstSubject.Ethnicity = this.textBox9.Text;
                subjects.FirstSubject.Shoe = this.textBox10.Text;
                subjects.ToXMLFile();
                this.ShowMITesFormCallback();
                this.Close(); 
            }
            catch (Exception)
            {
                MessageBox.Show("Please enter an integer value for age and real values for weight, height, fat mass, bone mass and water mass.");
            }
                       
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SubjectXML.Subjects subjects = new SubjectXML.Subjects(this.dataDirectory);
            MessageBox.Show("An empty subject file will be saved.");
            subjects.ToXMLFile();
            this.ShowMITesFormCallback();
            this.Close();       
        }
    }
}