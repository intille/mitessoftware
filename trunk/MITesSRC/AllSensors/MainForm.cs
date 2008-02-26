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
    public partial class MainForm : Form
    {
        private SessionForm sessionForm;
        private Form1 receiversForm;
        private SubjectForm subjectForm;

        private string mainPath;
        private int distID1, distID2, distID3, distID4, accelID1, accelID2, accelID3, accelID4, accelID5, accelID6, lightID1, tvThreshold, currentID, uvID, tempID, objectID;
        /// <summary>
        /// 
        /// </summary>
        public MainForm(string mainPath, int distID1, int distID2, int distID3, int distID4, int accelID1, int accelID2, int accelID3, int accelID4, int accelID5, int accelID6, int lightID1, int tvThreshold, int currentID, int uvID, int tempID, int objectID)
        {

            this.mainPath = mainPath;
            this.distID1 = distID1;
            this.distID2 = distID2;
            this.distID3 = distID3;
            this.distID4 = distID4;
            this.accelID1 = accelID1;
            this.accelID2 = accelID2;
            this.accelID3 = accelID3;
            this.accelID4 = accelID4;
            this.accelID5 = accelID5;
            this.accelID6 = accelID6;
            this.lightID1 = lightID1;
            this.tvThreshold = tvThreshold;
            this.currentID = currentID;
            this.uvID = uvID;
            this.tempID = tempID;
            this.objectID = objectID;

            InitializeComponent();
            InitializeInterface();
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if ((this.sessionForm == null) || this.sessionForm.IsDisposed)
            {
                this.sessionForm = new SessionForm();                
                this.sessionForm.StartMITesReceiversCallback = new StartMITesReceiversDelegate(this.InitializeMITesForm);                
                this.sessionForm.Show();
            }
        }

        private void InitializeMITesForm(AXML.Protocol activityProtocol, SXML.Configuration sensorConfiguration, string dataDirectory)
        {
            //Disable data collection button
            this.sessionForm.Close();
            this.Visible = false;            
            this.button1.Enabled = false;
            this.receiversForm = new Form1(this.mainPath, this.distID1, this.distID2, this.distID3, this.distID4, this.accelID1, this.accelID2, this.accelID3, this.accelID4, this.accelID5, this.accelID6, this.lightID1, this.tvThreshold, this.currentID, this.uvID, this.tempID, this.objectID);
            this.receiversForm.InitializeInterface(activityProtocol.FileName, sensorConfiguration.FileName, dataDirectory);
            this.subjectForm = new SubjectForm(dataDirectory);
            this.subjectForm.ShowMITesFormCallback = new ShowMITesFormDelegate(this.OpenMITesForm);
            this.subjectForm.Show();                        
            //Initialize MITes
            

        }

        /// <summary>
        /// 
        /// </summary>
        private void OpenMITesForm()
        {            
            this.Location = new Point(0, 0);
            this.Visible = true;
            this.receiversForm.ShowForms();
            
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="ee"></param>
        private void OnResize(object sender, EventArgs ee)
        {
            //to prevent resizing when minimizing
            if ((this.ClientSize.Width < Constants.MINIMUM_WINDOW_WIDTH) || (this.ClientSize.Height < Constants.MINIMUM_WINDOW_HEIGHT))
                return;
            int button_width = this.ClientSize.Width - Constants.LEFT_MARGIN - Constants.RIGHT_MARGIN;
            int button_height = (this.ClientSize.Height - Constants.TOP_MARGIN - Constants.BOTTOM_MARGIN - (NUMBER_BUTTONS * Constants.CONTROL_SPACING)) / NUMBER_BUTTONS;
            int button_x = Constants.LEFT_MARGIN;
            int button_y = Constants.TOP_MARGIN;

            int delta_y = button_height + Constants.CONTROL_SPACING;

            this.button1.Location = new System.Drawing.Point(button_x, button_y);
            this.button1.Size = new System.Drawing.Size(button_width, button_height);
            this.button1.Font = CalculateBestFitFont(this.button1.CreateGraphics(), Constants.MIN_FONT, Constants.MAX_FONT,
                this.button1.ClientSize, LONGEST_LABEL, this.button1.Font, (float)0.9, (float)0.75);

            button_y += delta_y;
            this.button2.Location = new System.Drawing.Point(button_x, button_y);
            this.button2.Size = new System.Drawing.Size(button_width, button_height);
            this.button2.Font = CalculateBestFitFont(this.button2.CreateGraphics(), Constants.MIN_FONT, Constants.MAX_FONT,
                this.button2.ClientSize, LONGEST_LABEL, this.button2.Font, (float)0.9, (float)0.75);

            button_y += delta_y;
            this.button3.Location = new System.Drawing.Point(button_x, button_y);
            this.button3.Size = new System.Drawing.Size(button_width, button_height);
            this.button3.Font = CalculateBestFitFont(this.button3.CreateGraphics(), Constants.MIN_FONT, Constants.MAX_FONT,
                this.button3.ClientSize, LONGEST_LABEL, this.button3.Font, (float)0.9, (float)0.75);


        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length != 5)
            {
                Console.WriteLine("Invalid command line argument. [filePath] [dist sensor ID1] [dist sensor ID2] [dist sensor ID3] [dist sensor ID4] [accel ID1] [accel ID2] [accel ID3]");
            }

            int id1 = Int32.Parse(args[1]);
            int id2 = Int32.Parse(args[2]);
            int id3 = Int32.Parse(args[3]);
            int id4 = Int32.Parse(args[4]);
            int aid1 = Int32.Parse(args[5]);
            int aid2 = Int32.Parse(args[6]);
            int aid3 = Int32.Parse(args[7]);
            int aid4 = Int32.Parse(args[8]);
            int aid5 = Int32.Parse(args[9]);
            int aid6 = Int32.Parse(args[10]);
            int lid1 = Int32.Parse(args[11]);
            int tvthresh = Int32.Parse(args[12]);
            int currentID = Int32.Parse(args[13]);
            int uvID = Int32.Parse(args[14]);
            int tempID = Int32.Parse(args[15]);
            int objectID = Int32.Parse(args[16]);
            //Form1 form1=new Form1(args[0], id1, id2, id3, id4, aid1, aid2, aid3, aid4, aid5, aid6, lid1, tvthresh, currentID, uvID, tempID, objectID);
            //SessionForm sessionForm = new SessionForm(form1);

            Application.Run(new MainForm(args[0], id1, id2, id3, id4, aid1, aid2, aid3, aid4, aid5, aid6, lid1, tvthresh, currentID, uvID, tempID, objectID));
            // Application.Run(new Form1(args[0], id1, id2, id3, id4, aid1, aid2, aid3, aid4, aid5, aid6, lid1, tvthresh, currentID, uvID, tempID, objectID));
            //Application.Exit (); 
            System.Environment.Exit(0);

        }
    }
}