using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using HousenCS.MITes;
using HousenCS.Net;
using SocketServer;


namespace MITesLogger_PC
{
	/// <summary>
	/// This application displays many different types of MITes data in one window and saves out log files. 
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		private static readonly bool DEBUG = true;
		private int stopHour = -1; 
		private bool isStartedReceiver = false;

        private int currentSecond;
        private double currentHRSecond;
        private int currentSamplingRate;
       // private int currentHeartRate;
        private string mainPath;
        private int configuredSensorCount;
        private int expectedSamplingRate;


        //private bool isBeepOnStep = false;



		// Used for steps
		private SocketTransmitter aSocketTransmitter = new SocketTransmitter(2134);

		private double[,] returnVals = new double[3,4];
        private MITesReceiverController mrc = null;
        private MITesReceiverController mrc2 = null;
        private const int BYTES_BUFFER_SIZE = 4000; //2048 
		private byte[] someBytes = new byte[BYTES_BUFFER_SIZE];
		private bool isResized = false;
		private bool isNeedRedraw = false;
		private int xDim = 240;
		private int yDim = 320;
		private int maxPlots = 7; // Changed from 6
		private bool isPlotting = true;
		private Bitmap backBuffer = null;

		//private bool isPlaySound = true;

        //private bool isComputingSteps = false;

		private static int NUM_TEXT_BOX_ROWS = 7;
		private static int NUM_TEXT_BOX_COLS = 2;

		//private MITesSoundPlayer aMITesSoundPlayer;

		//private DemoForm aDemoForm;

		#region Definitions of all key MITes related C# objects
		private MITesActivityCounter aMITesActivityCounter1;
		private MITesActivityCounter aMITesActivityCounter2;
		private MITesActivityCounter aMITesActivityCounter3;
		private MITesActivityCounter aMITesActivityCounter4;
		private MITesActivityCounter aMITesActivityCounter5;
		private MITesActivityCounter aMITesActivityCounter6;
		private MITesDecoder aMITesDecoder;
        private MITesDecoder aMITesDecoder2;

		private MITesScalablePlotter aMITesPlotter;
		private MITesHRAnalyzer aMITesHRAnalyzer;
//	private MITesStepsAnalyzerNew aMITesStepsAnalyzer;
		private MITesRangeAnalyzer aMITesRangeAnalyzer1;
		private MITesRangeAnalyzer aMITesRangeAnalyzer2;
		private MITesRangeAnalyzer aMITesRangeAnalyzer3;
		private MITesRangeAnalyzer aMITesRangeAnalyzer4;
		private MITesLightAnalyzer aMITesLightAnalyzer;
		private MITesTempAnalyzer aMITesTempAnalyzer;
		private MITesObjectAnalyzer aMITesObjectAnalyzer;

		private MITesDemoObjectAnalyzer aMITesDemoObjectAnalyzer;
		
		private MITesCurrentAnalyzer aMITesCurrentAnalyzer;
		//private MITesRFIDAnalyzer aMITesRFIDAnalyzer;
		private MITesUVAnalyzer aMITesUVAnalyzer;
		private MITesDataFilterer aMITesDataFilterer;
		private MITesLoggerNew aMITesLogger; 
		private MITesSensorCalibrator aMITesSensorCalibrator;
		private MITesActivityLogger aMITesActivityLogger; 
		//private MITesLoggerReaderNew aMITesLoggerReader;

		#endregion

        MitesAnnotater.MainForm annotationForm;

		#region GUI objects and global variables

		private Pen aPen = new Pen(Color.Wheat);
		private SolidBrush aBrush = new SolidBrush(Color.White);
		private SolidBrush blueBrush = new SolidBrush(Color.LightBlue);
		private int gapDistance = 4;

		private System.Windows.Forms.Timer readDataTimer;
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem menuItem4;
		private System.Windows.Forms.MenuItem menuItem5;
		private System.Windows.Forms.MenuItem menuItem6;
		private System.Windows.Forms.MenuItem menuItem7;
		private System.Windows.Forms.MenuItem menuItem8;
		private System.Windows.Forms.TextBox textBoxHR;
		private System.Windows.Forms.TextBox textBoxSteps;
		private System.Windows.Forms.TextBox textBoxDist;
		private System.Windows.Forms.TextBox textBoxUV;
		private System.Windows.Forms.TextBox textBoxRate;
		private System.Windows.Forms.TextBox textBoxLight;
		private System.Windows.Forms.TextBox textBoxAC1;
		private System.Windows.Forms.TextBox textBoxAC2;
		private System.Windows.Forms.TextBox textBoxAC3;
		private System.Windows.Forms.TextBox textBoxAC4;
		private System.Windows.Forms.TextBox textBoxAC5;
		private System.Windows.Forms.TextBox textBoxAC6;
		private System.Windows.Forms.TextBox textBoxCurrent;
		private System.Windows.Forms.TextBox textBoxRFID;
		private System.Windows.Forms.TextBox textBoxTemp;
		private System.Windows.Forms.TextBox textBoxObject;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.MenuItem menuItem10;
		private System.Windows.Forms.MenuItem menuItem11;
		private System.Windows.Forms.MenuItem menuItem12;
		private System.Windows.Forms.Label labelPlottingStatus;
		private System.Windows.Forms.MenuItem menuItem13;
		private System.Windows.Forms.MenuItem menuItem14;
		private System.Windows.Forms.MenuItem menuItem15;
		private System.Windows.Forms.MenuItem menuItem16;
		private System.Windows.Forms.MenuItem menuItem17;
		private System.Windows.Forms.MenuItem menuItem18;
		private System.Windows.Forms.MenuItem menuItem19;
		private System.Windows.Forms.MenuItem menuItem20;
        private MenuItem menuItem21;
		private System.Windows.Forms.MenuItem menuItem9; 

		#endregion

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.menuItem4 = new System.Windows.Forms.MenuItem();
            this.menuItem5 = new System.Windows.Forms.MenuItem();
            this.menuItem7 = new System.Windows.Forms.MenuItem();
            this.menuItem6 = new System.Windows.Forms.MenuItem();
            this.menuItem8 = new System.Windows.Forms.MenuItem();
            this.menuItem9 = new System.Windows.Forms.MenuItem();
            this.menuItem10 = new System.Windows.Forms.MenuItem();
            this.menuItem11 = new System.Windows.Forms.MenuItem();
            this.menuItem12 = new System.Windows.Forms.MenuItem();
            this.menuItem13 = new System.Windows.Forms.MenuItem();
            this.menuItem14 = new System.Windows.Forms.MenuItem();
            this.menuItem15 = new System.Windows.Forms.MenuItem();
            this.menuItem16 = new System.Windows.Forms.MenuItem();
            this.menuItem17 = new System.Windows.Forms.MenuItem();
            this.menuItem18 = new System.Windows.Forms.MenuItem();
            this.menuItem19 = new System.Windows.Forms.MenuItem();
            this.menuItem20 = new System.Windows.Forms.MenuItem();
            this.readDataTimer = new System.Windows.Forms.Timer(this.components);
            this.textBoxAC1 = new System.Windows.Forms.TextBox();
            this.textBoxAC2 = new System.Windows.Forms.TextBox();
            this.textBoxAC3 = new System.Windows.Forms.TextBox();
            this.textBoxAC4 = new System.Windows.Forms.TextBox();
            this.textBoxAC5 = new System.Windows.Forms.TextBox();
            this.textBoxAC6 = new System.Windows.Forms.TextBox();
            this.textBoxHR = new System.Windows.Forms.TextBox();
            this.textBoxSteps = new System.Windows.Forms.TextBox();
            this.textBoxDist = new System.Windows.Forms.TextBox();
            this.textBoxUV = new System.Windows.Forms.TextBox();
            this.textBoxLight = new System.Windows.Forms.TextBox();
            this.textBoxRate = new System.Windows.Forms.TextBox();
            this.textBoxRFID = new System.Windows.Forms.TextBox();
            this.textBoxTemp = new System.Windows.Forms.TextBox();
            this.textBoxObject = new System.Windows.Forms.TextBox();
            this.textBoxCurrent = new System.Windows.Forms.TextBox();
            this.labelPlottingStatus = new System.Windows.Forms.Label();
            this.menuItem21 = new System.Windows.Forms.MenuItem();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem1,
            this.menuItem2});
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 0;
            this.menuItem1.Text = "Quit";
            this.menuItem1.Click += new System.EventHandler(this.menuItem1_Click);
            // 
            // menuItem2
            // 
            this.menuItem2.Index = 1;
            this.menuItem2.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem3,
            this.menuItem4,
            this.menuItem5,
            this.menuItem9,
            this.menuItem10,
            this.menuItem13,
            this.menuItem16,
            this.menuItem21});
            this.menuItem2.Text = "Options";
            // 
            // menuItem3
            // 
            this.menuItem3.Index = 0;
            this.menuItem3.Text = "Set channels1";
            this.menuItem3.Click += new System.EventHandler(this.menuItem3_Click);
            // 
            // menuItem4
            // 
            this.menuItem4.Index = 1;
            this.menuItem4.Text = "Check data";
            this.menuItem4.Click += new System.EventHandler(this.menuItem4_Click);
            // 
            // menuItem5
            // 
            this.menuItem5.Index = 2;
            this.menuItem5.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem7,
            this.menuItem6,
            this.menuItem8});
            this.menuItem5.Text = "Sync";
            // 
            // menuItem7
            // 
            this.menuItem7.Index = 0;
            this.menuItem7.Text = "Misc";
            this.menuItem7.Click += new System.EventHandler(this.menuItem7_Click);
            // 
            // menuItem6
            // 
            this.menuItem6.Index = 1;
            this.menuItem6.Text = "Start code";
            this.menuItem6.Click += new System.EventHandler(this.menuItem6_Click);
            // 
            // menuItem8
            // 
            this.menuItem8.Index = 2;
            this.menuItem8.Text = "End code";
            this.menuItem8.Click += new System.EventHandler(this.menuItem8_Click);
            // 
            // menuItem9
            // 
            this.menuItem9.Index = 3;
            this.menuItem9.Text = "Files";
            // 
            // menuItem10
            // 
            this.menuItem10.Index = 4;
            this.menuItem10.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem11,
            this.menuItem12});
            this.menuItem10.Text = "Plotting";
            // 
            // menuItem11
            // 
            this.menuItem11.Index = 0;
            this.menuItem11.Text = "Set on";
            this.menuItem11.Click += new System.EventHandler(this.menuItem11_Click);
            // 
            // menuItem12
            // 
            this.menuItem12.Index = 1;
            this.menuItem12.Text = "Set off";
            this.menuItem12.Click += new System.EventHandler(this.menuItem12_Click);
            // 
            // menuItem13
            // 
            this.menuItem13.Index = 5;
            this.menuItem13.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem14,
            this.menuItem15});
            this.menuItem13.Text = "Sound";
            // 
            // menuItem14
            // 
            this.menuItem14.Index = 0;
            this.menuItem14.Text = "Turn off";
            this.menuItem14.Click += new System.EventHandler(this.menuItem14_Click);
            // 
            // menuItem15
            // 
            this.menuItem15.Index = 1;
            this.menuItem15.Text = "Turn on";
            this.menuItem15.Click += new System.EventHandler(this.menuItem15_Click);
            // 
            // menuItem16
            // 
            this.menuItem16.Index = 6;
            this.menuItem16.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem17,
            this.menuItem18,
            this.menuItem19,
            this.menuItem20});
            this.menuItem16.Text = "Steps";
            // 
            // menuItem17
            // 
            this.menuItem17.Index = 0;
            this.menuItem17.Text = "Beep on";
            this.menuItem17.Click += new System.EventHandler(this.menuItem17_Click);
            // 
            // menuItem18
            // 
            this.menuItem18.Index = 1;
            this.menuItem18.Text = "Beep off";
            this.menuItem18.Click += new System.EventHandler(this.menuItem18_Click);
            // 
            // menuItem19
            // 
            this.menuItem19.Index = 2;
            this.menuItem19.Text = "Compute on";
            this.menuItem19.Click += new System.EventHandler(this.menuItem19_Click_1);
            // 
            // menuItem20
            // 
            this.menuItem20.Index = 3;
            this.menuItem20.Text = "Compute off";
            this.menuItem20.Click += new System.EventHandler(this.menuItem20_Click);
            // 
            // readDataTimer
            // 
            this.readDataTimer.Enabled = true;
            this.readDataTimer.Interval = 10;
            this.readDataTimer.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // textBoxAC1
            // 
            this.textBoxAC1.Font = new System.Drawing.Font("Nina", 9F);
            this.textBoxAC1.Location = new System.Drawing.Point(96, 152);
            this.textBoxAC1.Name = "textBoxAC1";
            this.textBoxAC1.ReadOnly = true;
            this.textBoxAC1.Size = new System.Drawing.Size(80, 22);
            this.textBoxAC1.TabIndex = 0;
            this.textBoxAC1.TabStop = false;
            this.textBoxAC1.Text = "AC: ";
            // 
            // textBoxAC2
            // 
            this.textBoxAC2.Font = new System.Drawing.Font("Nina", 9F);
            this.textBoxAC2.Location = new System.Drawing.Point(0, 152);
            this.textBoxAC2.Name = "textBoxAC2";
            this.textBoxAC2.ReadOnly = true;
            this.textBoxAC2.Size = new System.Drawing.Size(80, 22);
            this.textBoxAC2.TabIndex = 5;
            this.textBoxAC2.TabStop = false;
            this.textBoxAC2.Text = "AC: ";
            // 
            // textBoxAC3
            // 
            this.textBoxAC3.Font = new System.Drawing.Font("Nina", 9F);
            this.textBoxAC3.Location = new System.Drawing.Point(0, 128);
            this.textBoxAC3.Name = "textBoxAC3";
            this.textBoxAC3.ReadOnly = true;
            this.textBoxAC3.Size = new System.Drawing.Size(88, 22);
            this.textBoxAC3.TabIndex = 2;
            this.textBoxAC3.TabStop = false;
            this.textBoxAC3.Text = "AC: ";
            // 
            // textBoxAC4
            // 
            this.textBoxAC4.Font = new System.Drawing.Font("Nina", 9F);
            this.textBoxAC4.Location = new System.Drawing.Point(0, 128);
            this.textBoxAC4.Name = "textBoxAC4";
            this.textBoxAC4.ReadOnly = true;
            this.textBoxAC4.Size = new System.Drawing.Size(88, 22);
            this.textBoxAC4.TabIndex = 2;
            this.textBoxAC4.TabStop = false;
            this.textBoxAC4.Text = "AC: ";
            // 
            // textBoxAC5
            // 
            this.textBoxAC5.Font = new System.Drawing.Font("Nina", 9F);
            this.textBoxAC5.Location = new System.Drawing.Point(0, 128);
            this.textBoxAC5.Name = "textBoxAC5";
            this.textBoxAC5.ReadOnly = true;
            this.textBoxAC5.Size = new System.Drawing.Size(88, 22);
            this.textBoxAC5.TabIndex = 2;
            this.textBoxAC5.TabStop = false;
            this.textBoxAC5.Text = "AC: ";
            // 
            // textBoxAC6
            // 
            this.textBoxAC6.Font = new System.Drawing.Font("Nina", 9F);
            this.textBoxAC6.Location = new System.Drawing.Point(0, 128);
            this.textBoxAC6.Name = "textBoxAC6";
            this.textBoxAC6.ReadOnly = true;
            this.textBoxAC6.Size = new System.Drawing.Size(88, 22);
            this.textBoxAC6.TabIndex = 2;
            this.textBoxAC6.TabStop = false;
            this.textBoxAC6.Text = "AC: ";
            // 
            // textBoxHR
            // 
            this.textBoxHR.Font = new System.Drawing.Font("Nina", 9F);
            this.textBoxHR.Location = new System.Drawing.Point(96, 104);
            this.textBoxHR.Name = "textBoxHR";
            this.textBoxHR.ReadOnly = true;
            this.textBoxHR.Size = new System.Drawing.Size(80, 22);
            this.textBoxHR.TabIndex = 4;
            this.textBoxHR.TabStop = false;
            this.textBoxHR.Text = "No HR";
            // 
            // textBoxSteps
            // 
            this.textBoxSteps.Font = new System.Drawing.Font("Nina", 9F);
            this.textBoxSteps.Location = new System.Drawing.Point(96, 104);
            this.textBoxSteps.Name = "textBoxSteps";
            this.textBoxSteps.ReadOnly = true;
            this.textBoxSteps.Size = new System.Drawing.Size(80, 22);
            this.textBoxSteps.TabIndex = 4;
            this.textBoxSteps.TabStop = false;
            this.textBoxSteps.Text = "No Steps";
            // 
            // textBoxDist
            // 
            this.textBoxDist.Font = new System.Drawing.Font("Nina", 9F);
            this.textBoxDist.Location = new System.Drawing.Point(96, 128);
            this.textBoxDist.Name = "textBoxDist";
            this.textBoxDist.ReadOnly = true;
            this.textBoxDist.Size = new System.Drawing.Size(80, 22);
            this.textBoxDist.TabIndex = 3;
            this.textBoxDist.TabStop = false;
            this.textBoxDist.Text = "No Distance";
            // 
            // textBoxUV
            // 
            this.textBoxUV.Font = new System.Drawing.Font("Nina", 9F);
            this.textBoxUV.Location = new System.Drawing.Point(0, 104);
            this.textBoxUV.Name = "textBoxUV";
            this.textBoxUV.ReadOnly = true;
            this.textBoxUV.Size = new System.Drawing.Size(88, 22);
            this.textBoxUV.TabIndex = 1;
            this.textBoxUV.TabStop = false;
            this.textBoxUV.Text = "No UV";
            // 
            // textBoxLight
            // 
            this.textBoxLight.Font = new System.Drawing.Font("Nina", 9F);
            this.textBoxLight.Location = new System.Drawing.Point(96, 104);
            this.textBoxLight.Name = "textBoxLight";
            this.textBoxLight.ReadOnly = true;
            this.textBoxLight.Size = new System.Drawing.Size(80, 22);
            this.textBoxLight.TabIndex = 4;
            this.textBoxLight.TabStop = false;
            this.textBoxLight.Text = "No Light";
            // 
            // textBoxRate
            // 
            this.textBoxRate.Font = new System.Drawing.Font("Nina", 9F);
            this.textBoxRate.Location = new System.Drawing.Point(96, 104);
            this.textBoxRate.Name = "textBoxRate";
            this.textBoxRate.ReadOnly = true;
            this.textBoxRate.Size = new System.Drawing.Size(80, 22);
            this.textBoxRate.TabIndex = 4;
            this.textBoxRate.TabStop = false;
            this.textBoxRate.Text = "No Rate";
            // 
            // textBoxRFID
            // 
            this.textBoxRFID.Font = new System.Drawing.Font("Nina", 9F);
            this.textBoxRFID.Location = new System.Drawing.Point(96, 104);
            this.textBoxRFID.Name = "textBoxRFID";
            this.textBoxRFID.ReadOnly = true;
            this.textBoxRFID.Size = new System.Drawing.Size(80, 22);
            this.textBoxRFID.TabIndex = 4;
            this.textBoxRFID.TabStop = false;
            this.textBoxRFID.Text = "No RFID";
            // 
            // textBoxTemp
            // 
            this.textBoxTemp.Font = new System.Drawing.Font("Nina", 9F);
            this.textBoxTemp.Location = new System.Drawing.Point(96, 104);
            this.textBoxTemp.Name = "textBoxTemp";
            this.textBoxTemp.ReadOnly = true;
            this.textBoxTemp.Size = new System.Drawing.Size(80, 22);
            this.textBoxTemp.TabIndex = 4;
            this.textBoxTemp.TabStop = false;
            this.textBoxTemp.Text = "No Temp";
            // 
            // textBoxObject
            // 
            this.textBoxObject.Font = new System.Drawing.Font("Nina", 9F);
            this.textBoxObject.Location = new System.Drawing.Point(96, 104);
            this.textBoxObject.Name = "textBoxObject";
            this.textBoxObject.ReadOnly = true;
            this.textBoxObject.Size = new System.Drawing.Size(80, 22);
            this.textBoxObject.TabIndex = 4;
            this.textBoxObject.TabStop = false;
            this.textBoxObject.Text = "No object motion";
            // 
            // textBoxCurrent
            // 
            this.textBoxCurrent.Font = new System.Drawing.Font("Nina", 9F);
            this.textBoxCurrent.Location = new System.Drawing.Point(96, 104);
            this.textBoxCurrent.Name = "textBoxCurrent";
            this.textBoxCurrent.ReadOnly = true;
            this.textBoxCurrent.Size = new System.Drawing.Size(80, 22);
            this.textBoxCurrent.TabIndex = 4;
            this.textBoxCurrent.TabStop = false;
            this.textBoxCurrent.Text = "No Current";
            // 
            // labelPlottingStatus
            // 
            this.labelPlottingStatus.Font = new System.Drawing.Font("Nina", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPlottingStatus.Location = new System.Drawing.Point(8, 8);
            this.labelPlottingStatus.Name = "labelPlottingStatus";
            this.labelPlottingStatus.Size = new System.Drawing.Size(120, 24);
            this.labelPlottingStatus.TabIndex = 6;
            this.labelPlottingStatus.Text = "Plotting status";
            this.labelPlottingStatus.Visible = false;
            // 
            // menuItem21
            // 
            this.menuItem21.Index = 7;
            this.menuItem21.Text = "Set channels2";
            this.menuItem21.Click += new System.EventHandler(this.menuItem21_Click);
            // 
            // Form1
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(292, 559);
            this.Controls.Add(this.labelPlottingStatus);
            this.Controls.Add(this.textBoxAC1);
            this.Controls.Add(this.textBoxUV);
            this.Controls.Add(this.textBoxAC3);
            this.Controls.Add(this.textBoxDist);
            this.Controls.Add(this.textBoxRate);
            this.Controls.Add(this.textBoxLight);
            this.Controls.Add(this.textBoxHR);
            this.Controls.Add(this.textBoxSteps);
            this.Controls.Add(this.textBoxAC2);
            this.Controls.Add(this.textBoxCurrent);
            this.Controls.Add(this.textBoxRFID);
            this.Controls.Add(this.textBoxTemp);
            this.Controls.Add(this.textBoxObject);
            this.Controls.Add(this.textBoxAC4);
            this.Controls.Add(this.textBoxAC5);
            this.Controls.Add(this.textBoxAC6);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Menu = this.mainMenu1;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.Text = "MIT Environmental Sensors (MITes)";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Form1_KeyPress);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		#region Main form setup
		private void SetFormPositions()
		{
			xDim = Math.Min(this.ClientSize.Width, 480);
			yDim = Math.Min(this.ClientSize.Height, 1000);

			int width = (xDim - 2*gapDistance - gapDistance)/2;
			int yOff = textBoxAC2.Height + gapDistance;
			int xOff = width + 2*gapDistance; 

			textBoxRate.Width = width;
			textBoxLight.Width = width;
			textBoxAC1.Width = width;
			textBoxAC2.Width = width;
			textBoxAC3.Width = width;
			textBoxAC4.Width = width;
			textBoxAC5.Width = width;
			textBoxAC6.Width = width;
			textBoxDist.Width = width;
			textBoxHR.Width = width;
			textBoxSteps.Width = width;
			textBoxUV.Width = width;
			textBoxCurrent.Width = width;
			textBoxRFID.Width = width;
			textBoxTemp.Width = width;
			textBoxObject.Width = width;

			textBoxAC6.Location = new Point(gapDistance, yDim-yOff);
			textBoxAC5.Location = new Point(gapDistance, yDim-yOff*2);
			textBoxAC4.Location = new Point(gapDistance, yDim-yOff*3);
			textBoxAC3.Location = new Point(gapDistance, yDim-yOff*4);
			textBoxAC2.Location = new Point(gapDistance, yDim-yOff*5);
			textBoxAC1.Location = new Point(gapDistance, yDim-yOff*6);
			textBoxLight.Location = new Point(gapDistance, yDim-yOff*7);
			textBoxCurrent.Location = new Point(gapDistance, yDim-yOff*8);

			textBoxDist.Location = new Point(xOff, yDim-yOff);
			textBoxHR.Location = new Point(xOff, yDim-yOff*2);
			textBoxUV.Location = new Point(xOff, yDim-yOff*3);
			textBoxRate.Location = new Point(xOff, yDim-yOff*4);
			textBoxRFID.Location = new Point(xOff, yDim-yOff*5);
			textBoxTemp.Location = new Point(xOff, yDim-yOff*6);
			textBoxObject.Location = new Point(xOff, yDim-yOff*7);
			textBoxSteps.Location = new Point(xOff, yDim-yOff*8);
			
			//			buttonCalib.Location = new Point(xDim-(1*minButtonWidth+gapDistance),yDim-(4*(buttonHeight+gapDistance)));
			//			buttonSensorIDs.Location = new Point(xDim-(1*minButtonWidth+gapDistance),yDim-(3*(buttonHeight+gapDistance)));
			//			buttonSync.Location = new Point(xDim-(1*minButtonWidth+gapDistance),yDim-(2*(buttonHeight+gapDistance)));
			//			buttonQuit.Location = new Point(xDim-(1*minButtonWidth+gapDistance),yDim-(1*(buttonHeight+gapDistance)));
			//
			//			buttonCalib.Size = new Size(minButtonWidth,buttonHeight);
			//			buttonSensorIDs.Size = new Size(minButtonWidth,buttonHeight);
			//			buttonQuit.Size = new Size(minButtonWidth,buttonHeight);
			//			buttonSync.Size = new Size(minButtonWidth,buttonHeight);
			//

			if (aMITesPlotter != null)
				aMITesPlotter.SetupScaleFactor(GetGraphSize(),maxPlots);	
		
		}

		private void Form1_Resize(object sender, System.EventArgs e)
		{
			SetFormPositions();
			aMITesActivityLogger.WriteLogComment("Form resized");
			isResized = true;
		}

		#endregion

		#region Forms for MITes application
        private ReceiverConfigureForm rcf = null;
        private ReceiverConfigureForm rcf2 = null;
        //private DataCheckForm dcf = null; 
		#endregion
		
		#region Debugging methods
		private void Debug(String aMsg)
		{
			if (DEBUG)
				Console.WriteLine("DEBUG: " + aMsg);
		}

		private void Warning(String aMsg)
		{
			Console.WriteLine("WARNING: " + aMsg);
			Thread.Sleep(10000);
			Application.Exit();
		}
		#endregion
	
		#region Range beacon MITes functions
		private string GetDistString(int val)
		{
			if (val == MITesData.NO_VALUE)
				return "None ";
			else if (val == 0)
				return "Close";
			else if (val == 1)
				return "Near ";
			else if (val == 2)
				return "Far  ";
			else if (val == 3)
				return "Room+";
			else
				return "Unk  ";
		}

		/// <summary>
		/// Report results from Range sensor device.
		/// </summary>
		public void ReportRange()
		{
			string dist1;
			if (aMITesRangeAnalyzer1.GetID() != MITesData.NONE)
				dist1 = GetDistString(aMITesRangeAnalyzer1.GetEpochValue());
			else
				dist1 = "";
			
			string dist2;
			if (aMITesRangeAnalyzer2.GetID() != MITesData.NONE)
				dist2 = GetDistString(aMITesRangeAnalyzer2.GetEpochValue());
			else
				dist2 = "";

			string dist3;
			if (aMITesRangeAnalyzer3.GetID() != MITesData.NONE)
				dist3 = GetDistString(aMITesRangeAnalyzer3.GetEpochValue());
			else
				dist3 = "";
			
			string dist4;
			if (aMITesRangeAnalyzer4.GetID() != MITesData.NONE)
				dist4 = GetDistString(aMITesRangeAnalyzer4.GetEpochValue());
			else
				dist4 = "";
			
			textBoxDist.Text = "Distance beacon: " + dist1 + " " + dist2 + " " + dist3 + " " + dist4;
		}

		#endregion

		int lastHour;
		private bool IsShutdownTime()
		{
			//Console.WriteLine ("Hour: " + DateTime.Now.Hour);
			if (stopHour == -1) // No stop
				return false;

			if (DateTime.Now.Hour != lastHour)
			{
				// Hour changed
				if (DateTime.Now.Hour == stopHour)
				{
					lastHour = DateTime.Now.Hour;
					return true;
				}
				lastHour = DateTime.Now.Hour;
			}
			return false;
		}

		#region Light MITes functions
		String lmsg;
		/// <summary>
		/// Report the light reading. 
		/// </summary>
		public void ReportLight()
		{
			int light = aMITesLightAnalyzer.GetLastValue();
			double mean = aMITesLightAnalyzer.getEpochMean();
			double var = aMITesLightAnalyzer.GetEpochVariance();

			if ((var != MITesData.NO_VALUE) && (light != MITesData.NO_VALUE))
			{
				lmsg = "Light: " + light + " ";
				//textBoxLight.Text = "L: " + light + " " + (Math.Round (mean,1)) + "(" + (Math.Round(var,1)) + ")";
				if (mean < 10)
					lmsg += "Bright";
				else if (mean < 200)
					lmsg += "Some light";
				else if (mean < 800)
					lmsg += "Bit of light";
				else if (mean < 1100)
					lmsg += "Dim";
				else if (mean < 1800)
					lmsg += "Very dim";
				else 
					lmsg += "Pitch black";
				
				textBoxLight.Text = lmsg; 
			}
			else
				textBoxLight.Text = "L: none";
		}
		#endregion

		#region UV MITes functions

		int lastUVTime1 = Environment.TickCount;
		int lastUVTime2 = Environment.TickCount;

		int lastUVTime; 
		/// <summary>
		/// 
		/// </summary>
		public void ReportUV()
		{
			int uv = aMITesUVAnalyzer.GetLastLightValue ();
			string uvText = "";

			if (uv < 4)
				uvText += "(negligible)";

			if (uv != MITesData.NO_VALUE)
			{
				textBoxUV.Text = "UV: " + uv + " " + uvText;
				lastUVTime = Environment.TickCount;
			}
			else
			{
				if ((Environment.TickCount-lastUVTime)>3000)
					textBoxUV.Text = "UV: none";
			}
		}
		#endregion

		#region Current MITes functions
		int lastCurrentTime; 
		/// <summary>
		/// 
		/// </summary>
		public void ReportCurrent()
		{
			int current = aMITesCurrentAnalyzer.GetLastValue ();

			if (current != MITesData.NO_VALUE)
			{
				textBoxCurrent.Text = "Current: " + current;
				lastCurrentTime = Environment.TickCount;
			}
			else
			{
				if ((Environment.TickCount-lastCurrentTime)>3000)
					textBoxCurrent.Text = "Current: none";
			}
		}
		#endregion

		#region Temperature MITes functions
		int lastTempTime; 
		/// <summary>
		/// 
		/// </summary>
		public void ReportTemp()
		{
			int temp = aMITesTempAnalyzer.GetLastValue ();
			double cel;

			if (temp != MITesData.NO_VALUE)
			{
				cel = temp/10.0;
				textBoxTemp.Text = "Temp: " + Math.Round(cel,1) + " C (" + Math.Round((9/5.0)*cel + 32,0) + " F)";
				lastTempTime = Environment.TickCount;
			}
			else
			{
				if ((Environment.TickCount-lastTempTime)>3000)
					textBoxTemp.Text = "Temp: none";
			}
		}
		#endregion

		#region Object motion MITes functions
		int lastObjectTime; 
		/// <summary>
		/// 
		/// </summary>
		public void ReportObject()
		{
			int obj = aMITesObjectAnalyzer.GetLastValue ();

			if (obj != MITesData.NO_VALUE)
			{
				textBoxObject.Text = "Object " + aMITesObjectAnalyzer.GetID() + " moving!";
				lastObjectTime = Environment.TickCount;
			}
			else if ((Environment.TickCount-lastObjectTime)>500)
			{
				if (aMITesObjectAnalyzer.IsSeenEpoch ())
					textBoxObject.Text = "Object " + aMITesObjectAnalyzer.GetID() + " moved recently";
				else
					if ((Environment.TickCount-lastObjectTime)>3000)
					textBoxObject.Text = "No object motion";
			}
		}
		#endregion
		
		#region RFID MITes functions
		//		int lastRFIDTime; 
//		/// <summary>
//		/// 
//		/// </summary>
//		public void ReportRFID()
//		{
//			int current = aMITesRFIDAnalyzer.GetLastLightValue ();
//
//			if (current != MITesData.NO_VALUE)
//			{
//				textBoxRFID.Text = "RFID: " + current;
//				lastRFIDTime = Environment.TickCount;
//			}
//			else
//			{
//				if ((Environment.TickCount-lastRFIDTime)>3000)
//					textBoxRFID.Text = "RFID: none";
//			}
//		}
		#endregion

		#region Heart rate MITes functions
		/// <summary>
		/// Report the HR if getting received
		/// </summary>
		public void ReportHR()
		{
			int hr = aMITesHRAnalyzer.GetLastHR ();
			double meanHR = aMITesHRAnalyzer.GetLastMean();
			int lastHRTime = aMITesHRAnalyzer.GetLastTime ();
			
			if ((Environment.TickCount - lastHRTime) > 15000)
			{
				textBoxHR.Text = "No HR Data 15 sec";
				//if (isPlaySound)
				//	aMITesSoundPlayer.PlayHRERROR();
			}
			else if ((Environment.TickCount - lastHRTime) > 2000)
				textBoxHR.Text = "No HR Data";
			else if ((hr != 0) && (hr != MITesData.NONE))
			{
				//textBoxHR.Text = "HR: " + hr + " Avg: " + Math.Round(meanHR,1);
				textBoxHR.Text = "HR Avg: " + Math.Round(meanHR,1);
			}
		}
		#endregion

		#region Steps MITes functions
		/// <summary>
		/// Report the HR if getting received
		/// </summary>
//        public void ReportSteps()
//        {
//            double meanBPM = aMITesStepsAnalyzer.GetWalkingSpeedNow();
//            double bpm = meanBPM; // aMITesStepsAnalyzer.GetLastComputedSPM ();
//            double medianBPM = aMITesStepsAnalyzer.GetMedianSPM();
//            double medianMeanBPM = aMITesStepsAnalyzer.GetMedianMeanSPM();

////			int lastStepsTime = aMITesStepsAnalyzer.GetLastComputedSPMTime ();
////	
//            string s = "";
////
////			if ((Environment.TickCount - lastStepsTime) > 15000)
////			{
////				textBoxSteps.Text = "No Steps Data 15 sec";
////			}
////				//			else if ((Environment.TickCount - lastStepsTime) > 2000)
////				//				textBoxSteps.Text = "No Steps Data";
////			else
////			{
//                if (meanBPM != MITesData.NONE)
//                {
//                    //textBoxSteps.Text = "Steps Avg: " + Math.Round(meanBPM,1);
//                    s = "StepsPM: " + Math.Round(meanBPM,1);
////					aSocketTransmitter.SendData ("BPM " + Math.Round(meanBPM,1));
//                }

//                if (medianBPM != MITesData.NONE)
//                {
//                    s += " (" + Math.Round(medianBPM,1) + ")(" + Math.Round (medianMeanBPM,1) + ")";
//                    aSocketTransmitter.SendData ("BPM " + Math.Round(medianMeanBPM,1));
//                }

//                textBoxSteps.Text = s;
////			}
////			else
////			{
////				textBoxSteps.Text = "Steps BPM: Unk";				
////			}
//        }
		#endregion

		#region TV status functions
		private void ReportTV()
		{
			string s = GetTVState(aMITesLightAnalyzer);
			
			textBoxLight.Text = "TV: " + s + " using thresh: " + TVThreshold;
		}
		private int TVThreshold = 40;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="aMITesLightAnalyzer"></param>
		/// <returns></returns>
		public string GetTVState(MITesLightAnalyzer aMITesLightAnalyzer)
		{
			double mean = aMITesLightAnalyzer.getEpochMean();
			string s = "";

			if (mean == MITesData.NO_VALUE)
				s+= "UNK";
			else if (mean < TVThreshold)
				s+= "ON";
			else
				s+= "OFF";
			return s;
		}

		#endregion

		/// <summary>
		/// Report counts for up to three accelerometers, only called when Epoch has new data
		/// </summary>
		public void ReportActivityCounts()
		{
			double result = aMITesActivityCounter1.GetLastEpochValueAll();

			if (result == 0)
			{
				textBoxAC1.Text = "AC " + aMITesActivityCounter1.GetID () + ": none";
				//if (isPlaySound)
				//	aMITesSoundPlayer.PlayError1();
			}
			else
			{
				textBoxAC1.Text = "AC " + aMITesActivityCounter1.GetID () + ": "+ Math.Round(result,2);
				if (result<3.0)
					textBoxAC1.Text = "AC " + aMITesActivityCounter1.GetID () + ": still";
			}

			result = aMITesActivityCounter2.GetLastEpochValueAll();

			if (result == 0)
			{
				textBoxAC2.Text = "AC " + aMITesActivityCounter2.GetID () + ": none";
				//if (isPlaySound)
				//	aMITesSoundPlayer.PlayError2();
			}
			else
			{
				textBoxAC2.Text = "AC " + aMITesActivityCounter2.GetID () + ": "+ Math.Round(result,2);
				if (result<3.0)
					textBoxAC2.Text = "AC " + aMITesActivityCounter2.GetID () + ": still";
			}

			result = aMITesActivityCounter3.GetLastEpochValueAll();

			if (result == 0)
			{
				textBoxAC3.Text = "AC " + aMITesActivityCounter3.GetID () + ": none";
				//if (isPlaySound)
					//aMITesSoundPlayer.PlayError3();
			}
			else
			{
				textBoxAC3.Text = "AC " + aMITesActivityCounter3.GetID () + ": " + Math.Round(result,2);
				if (result<3.0)
					textBoxAC3.Text = "AC " + aMITesActivityCounter3.GetID () + ": still";
			}

			result = aMITesActivityCounter4.GetLastEpochValueAll();

			if (result == 0)
			{
				textBoxAC4.Text = "AC " + aMITesActivityCounter4.GetID () + ": none";
				//if (isPlaySound)
				//	aMITesSoundPlayer.PlayError4();
			}
			else
			{
				textBoxAC4.Text = "AC " + aMITesActivityCounter4.GetID () + ": " + Math.Round(result,2);
				if (result<3.0)
					textBoxAC4.Text = "AC " + aMITesActivityCounter4.GetID () + ": still";
			}

			result = aMITesActivityCounter5.GetLastEpochValueAll();

			if (result == 0)
			{
				textBoxAC5.Text = "AC " + aMITesActivityCounter5.GetID () + ": none";
				//if (isPlaySound)
				//	aMITesSoundPlayer.PlayError5();
			}
			else
			{
				textBoxAC5.Text = "AC " + aMITesActivityCounter5.GetID () + ": " + Math.Round(result,2);
				if (result<3.0)
					textBoxAC5.Text = "AC " + aMITesActivityCounter5.GetID () + ": still";
			}

			result = aMITesActivityCounter6.GetLastEpochValueAll();

			if (result == 0)
			{
				textBoxAC6.Text = "AC " + aMITesActivityCounter6.GetID () + ": None";
				//if (isPlaySound)
				//	aMITesSoundPlayer.PlayError6();
			}
			else
			{
				textBoxAC6.Text = "AC " + aMITesActivityCounter6.GetID () + ": " + Math.Round(result,2);
				if (result<3.0)
					textBoxAC6.Text = "AC " + aMITesActivityCounter6.GetID () + ": still";
			}
		}
        
     
      
        
        private int distID1, distID2, distID3, distID4, accelID1, accelID2, accelID3, accelID4, accelID5, accelID6, lightID1, tvThreshold, currentID, uvID, tempID, objectID;
		/// <summary>
		/// Main form of the application.
		/// </summary>
		public Form1(string mainPath, int distID1, int distID2, int distID3, int distID4, int accelID1, int accelID2, int accelID3, int accelID4, int accelID5, int accelID6, int lightID1, int tvThreshold, int currentID, int uvID, int tempID, int objectID)
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
            
            //InitializeInterface();            

         

        }

        ProgressForm progressForm;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="description"></param>
        /// <param name="step"></param>
        public void UpdateProgressBar(string description, int step)
        {
            this.progressForm.PerformStep(description, step);
            this.progressForm.Update();
        }
     
        /// <summary>
        /// 
        /// </summary>
        public void InitializeInterface(string activityProtocolFile, string sensorConfigurationFile, string dataDirectory)
        {
            this.progressForm = new ProgressForm();
            
            //this.progressForm.InitializeInterface();
            
            SimpleLogger.Logger logger = new SimpleLogger.Logger("..\\..\\NeededFiles\\log", SimpleLogger.Logger.Priority.FATAL);
            string startupPath=System.Windows.Forms.Application.StartupPath;

            progressForm.Show();
            progressForm.PerformStep("Loading Activity Protocol and Sensor Files ...", 1);

            //check if activity protocol is there
            if (!(File.Exists(activityProtocolFile))){
                MessageBox.Show("Fatal Error: The following activity protocol file does not exist : "+ activityProtocolFile+
                    ". To fix the problem create an activity protocol file and list the path for the file in the master protocols file at"+
                    startupPath+"\\"+AXML.ProtocolsReader.DEFAULT_XML_FILE  + ".");                         
                Environment.Exit(0);
            }

            //check if sensor configuration is there
            if (!(File.Exists(sensorConfigurationFile))){
                MessageBox.Show("Fatal Error: The following sensor configuration file does not exist : "+ sensorConfigurationFile+
                    ". To fix the problem create a sensor configuration file and list the path for the file in the master sensors file at"+
                    startupPath+"\\"+SXML.ConfigurationReader.DEFAULT_XML_FILE + ".");                  
                Environment.Exit(0);                                     
            }

            //check if activity xsd is there
             
            if (!(File.Exists(startupPath+ "\\" + AXML.Reader.DEFAULT_XSD_FILE))){
                MessageBox.Show("Fatal Error: The following activity protocol xsd does not exist : "+ startupPath+ "\\" + AXML.Reader.DEFAULT_XSD_FILE+
                    ". To fix the problem you need to get the xsd or reinstall the software.");                         
                Environment.Exit(0);
            }

            
            //check if sensor xsd is there
            if (!(File.Exists(startupPath+ "\\" + SXML.Reader.DEFAULT_XSD_FILE))){
                MessageBox.Show("Fatal Error: The following sensor data xsd does not exist : "+ startupPath+ "\\" + SXML.Reader.DEFAULT_XSD_FILE+
                    ". To fix the problem you need to get the xsd or reinstall the software.");                         
                Environment.Exit(0);
            }

            //check if the data directory exists
              if (!(Directory.Exists(dataDirectory))){
                MessageBox.Show("Fatal Error: The data directory that you picked does not exist "+dataDirectory+". To fix this problem, you need to create the directory.");                         
                Environment.Exit(0);
            }

            //if everything is there copy both the activitylabels and the sensordata xmls to the data directory
            

            try
            {
                File.Copy(sensorConfigurationFile, dataDirectory + "\\" + SXML.Reader.DEFAULT_XML_FILE);
                
            }
            catch (Exception e)
            {
                MessageBox.Show("Fatal Error: Exception occurred when copying " + sensorConfigurationFile +
                    " to "+dataDirectory + "\\" + SXML.Reader.DEFAULT_XML_FILE+ ". For more details check most recent log file at " +
                  startupPath + "\\" + SimpleLogger.Logger.LOGGER_DIRECTORY);
                logger.Fatal("Exception occurred when copying " + sensorConfigurationFile +
                    " to " + dataDirectory + "\\" + SXML.Reader.DEFAULT_XML_FILE + ". " + e.ToString());
                logger.Dispose();
                Environment.Exit(0);
            }

            try
            {
                File.Copy(activityProtocolFile, dataDirectory + "\\" + AXML.Reader.DEFAULT_XML_FILE);
            }
            catch (Exception e)
            {
                MessageBox.Show("Fatal Error: Exception occurred when copying " + activityProtocolFile +
                    " to " + dataDirectory + "\\" + AXML.Reader.DEFAULT_XML_FILE + ". For more details check the most recent log file at " +
                  startupPath + "\\" + SimpleLogger.Logger.LOGGER_DIRECTORY);
                logger.Fatal("Exception occurred when copying " + activityProtocolFile +
                    " to " + dataDirectory + "\\" + AXML.Reader.DEFAULT_XML_FILE + ". " + e.ToString());
                logger.Dispose();
                Environment.Exit(0);
            }

            
            //parse the activity labels 
            try
            {
                AXML.Reader reader = new AXML.Reader(dataDirectory, this);
                reader.validate();
            }
            catch (Exception e)
            {
                MessageBox.Show("Fatal Error: Exception occurred when parsing "+ dataDirectory + "\\" + AXML.Reader.DEFAULT_XML_FILE + 
                    ". For more details check the most recent log file at "  + startupPath + 
                    "\\" + SimpleLogger.Logger.LOGGER_DIRECTORY);
                logger.Fatal("Exception occurred when parsing "+ dataDirectory + "\\" + AXML.Reader.DEFAULT_XML_FILE + 
                     ". " + e.ToString());
                logger.Dispose();
                Environment.Exit(0);
            }

            //parse the sensor data file
            SXML.SensorAnnotation sensorAnnotation=null;
            try
            {
                SXML.Reader sreader = new SXML.Reader(dataDirectory);
                sreader.validate();
                sensorAnnotation = sreader.parse();
            }
            catch (Exception e)
            {
                MessageBox.Show("Fatal Error: Exception occurred when parsing " + dataDirectory + "\\" + SXML.Reader.DEFAULT_XML_FILE +
                    ". For more details check the most recent log file at " + startupPath +
                    "\\" + SimpleLogger.Logger.LOGGER_DIRECTORY);
                logger.Fatal("Exception occurred when parsing " + dataDirectory + "\\" + SXML.Reader.DEFAULT_XML_FILE +
                     ". " + e.ToString());
                logger.Dispose();
                Environment.Exit(0);
            }

        
            logger.Dispose();

            //Intialize MITes form

            
           

            //string sourceDirectory = System.Windows.Forms.Application.StartupPath;
            //aMITesSoundPlayer = new MITesSoundPlayer(this);

            lastHour = DateTime.Now.Hour;
            //this.mainPath = mainPath;
            //this.TVThreshold = tvThreshold;
            Console.WriteLine("Writing to directory: " + mainPath);

            // Start listening UDP server for background diff info
            //myUDPServer = new UDPServer(8225); //2
            //myUDPServer.InputDetected += new InputDetectedEventHandler(this.UDPInputProcessor);
            //myUDPServer.StartListen();
         
            
         
           // textBoxAC1.Text = "Search COM port...";
            mrc = new MITesReceiverController(MITesReceiverController.FIND_PORT, BYTES_BUFFER_SIZE, true, MITesReceiverController.USE_THREADS, this.UpdateProgressBar);
            //textBoxAC1.Text = "Search second COM port...";
            mrc2 = new MITesReceiverController(MITesReceiverController.FIND_PORT, BYTES_BUFFER_SIZE, true, MITesReceiverController.USE_THREADS, this.UpdateProgressBar);
            progressForm.Close();

            //check if the expected number of receivers in the xml matches the connected ones.
            if ((sensorAnnotation.SensorCount1 > 0) && (sensorAnnotation.SensorCount2 > 0) && ((mrc.IsRunning()==false) || (mrc2.IsRunning() == false)))
            {
                MessageBox.Show("Error: The selected sensor set requires 2 receivers. Please connect 2 receivers and then run the application. Quitting!");
                Environment.Exit(0);
            }
            else if (   ( ((sensorAnnotation.SensorCount1 > 0) && (sensorAnnotation.SensorCount2==0)) ||
                           ((sensorAnnotation.SensorCount2 > 0) && (sensorAnnotation.SensorCount1 == 0)) ) && ( ( ((mrc.IsRunning()==false)) && (mrc2.IsRunning() == false)) ||  ((mrc.IsRunning() == true) && (mrc2.IsRunning() == true)) ))
            {
                MessageBox.Show("Error: The selected sensor set requires 1 receiver, please connect 1 receiver only. Quitting!");
                Environment.Exit(0);
            }


         

            aMITesActivityLogger = new MITesActivityLogger(mainPath + "data\\activity\\MITesActivityData");
            aMITesActivityLogger.SetupDirectories(mainPath);
            InitializeComponent();
            SetFormPositions();
            aMITesDecoder = new MITesDecoder();
            aMITesDecoder2 = new MITesDecoder();

            aMITesPlotter = new MITesScalablePlotter(this, MITesScalablePlotter.DeviceTypes.PC, maxPlots, aMITesDecoder, GetGraphSize());

            aMITesActivityCounter1 = new MITesActivityCounter(aMITesDecoder, accelID1);
            aMITesActivityCounter2 = new MITesActivityCounter(aMITesDecoder, accelID2);
            aMITesActivityCounter3 = new MITesActivityCounter(aMITesDecoder, accelID3);
            aMITesActivityCounter4 = new MITesActivityCounter(aMITesDecoder, accelID4);
            aMITesActivityCounter5 = new MITesActivityCounter(aMITesDecoder, accelID5);
            aMITesActivityCounter6 = new MITesActivityCounter(aMITesDecoder, accelID6);

            aMITesHRAnalyzer = new MITesHRAnalyzer(aMITesDecoder);
            //			aMITesStepsAnalyzer = new MITesStepsAnalyzerNew(aMITesDecoder,8, isBeepOnStep, true, true);
            //			aMITesStepsAnalyzer.SetSocketTransmitter(aSocketTransmitter);

            aMITesRangeAnalyzer1 = new MITesRangeAnalyzer(aMITesDecoder, distID1);
            aMITesRangeAnalyzer2 = new MITesRangeAnalyzer(aMITesDecoder, distID2);
            aMITesRangeAnalyzer3 = new MITesRangeAnalyzer(aMITesDecoder, distID3);
            aMITesRangeAnalyzer4 = new MITesRangeAnalyzer(aMITesDecoder, distID4);

            aMITesLightAnalyzer = new MITesLightAnalyzer(aMITesDecoder, lightID1);

            aMITesCurrentAnalyzer = new MITesCurrentAnalyzer(aMITesDecoder, currentID);

            aMITesObjectAnalyzer = new MITesObjectAnalyzer(aMITesDecoder, objectID);

            aMITesDemoObjectAnalyzer = new MITesDemoObjectAnalyzer(aMITesDecoder);
            //aDemoForm = new DemoForm(aMITesDemoObjectAnalyzer);
            //aDemoForm.Visible = true;

            aMITesTempAnalyzer = new MITesTempAnalyzer(aMITesDecoder, tempID);

            //aMITesRFIDAnalyzer = new MITesRFIDAnalyzer(aMITesDecoder, RFIDID);

            aMITesUVAnalyzer = new MITesUVAnalyzer(aMITesDecoder, uvID);

            aMITesDataFilterer = new MITesDataFilterer(aMITesDecoder);

            aMITesSensorCalibrator = new MITesSensorCalibrator(aMITesDecoder);

            aMITesLogger = new MITesLoggerNew(aMITesDecoder,
                mainPath + "data\\raw\\MITesAccelBytes",
                mainPath + "data\\log\\MITesLogFileBytes");

            // RANDY 
            // Read from file 
            //aMITesLoggerReader = new MITesLoggerReaderNew(aMITesDecoder,"C://Documents and Settings//Stephen Intille//Desktop//testFromRandy.b");
            //aMITesLoggerReader = new MITesLoggerReaderNew(aMITesDecoder,"C://Documents and Settings//Stephen Intille//Desktop//test2.PLFormat");

            rcf = new ReceiverConfigureForm(mrc, this, aMITesActivityLogger);
            rcf2 = new ReceiverConfigureForm(mrc2, this, aMITesActivityLogger);

            aMITesActivityLogger.WriteLogComment("Application started with command line: " +
                 mainPath + " " +
                 distID1 + " " +
                 distID2 + " " +
                 distID3 + " " +
                 distID4 + " " +
                 accelID1 + " " +
                 accelID2 + " " +
                 accelID3 + " " +
                accelID4 + " " +
                accelID5 + " " +
                accelID6 + " " +
                lightID1 + " " +
                 tvThreshold + " " +
                 currentID + " " +
                 uvID + " " +
                 tempID + " " +
                 objectID + " ");

            //			(aMITesActivityCounter, aMITesHRAnalyzer, aMITesSensorCalibrator,
            //				mainPath + "data\\activity\\MITesActivityData");	

            this.currentSecond = DateTime.Now.Second;
            TimeSpan ts = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));            
            this.currentHRSecond = ts.TotalSeconds;
            //this.currentMinute = DateTime.Now.Second;
            this.currentSamplingRate = 0;
            //this.currentHeartRate = 0;
            try
            {
                this.annotationForm = new MitesAnnotater.MainForm(dataDirectory, dataDirectory);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                Environment.Exit(0);
            }
            this.configuredSensorCount = this.annotationForm.SensorConfiguration.Sensors.Count;
            // MITes Data Filterer stores performance stats for MITes
            // Initialize all performance counters for all MITES channels
            for (int i = 0; i < MITesData.MAX_MITES_CHANNELS; i++)
            {
                this.aMITesDataFilterer.MitesPerformanceTracker[i] = new MITesPerformanceStats(0);
            }

            // Based on the type of sensor HR or accelerometer and the number of sensors initialize the 
            // performance objects and set their expected sampling rates
            foreach (SXML.Sensor sensor in this.annotationForm.SensorConfiguration.Sensors)
            {
                int sensor_id = Convert.ToInt32(sensor.ID);

                if (Convert.ToInt32(sensor.Receiver) == 1)
                    this.configuredSensorCount = this.annotationForm.SensorConfiguration.SensorCount1;
                else
                    this.configuredSensorCount = this.annotationForm.SensorConfiguration.SensorCount2;

                if (sensor_id == 0)
                {
                    this.aMITesDataFilterer.MitesPerformanceTracker[sensor_id] = new MITesPerformanceStats(30 / this.configuredSensorCount);
                }
                else
                {
                    this.aMITesDataFilterer.MitesPerformanceTracker[sensor_id] = new MITesPerformanceStats(180 / this.configuredSensorCount);
                }
            }

            //Load the annotation form
            this.annotationForm.SetDataFilterer(aMITesDataFilterer);


           //calculate expected sampling rate

            if ((this.annotationForm.SensorConfiguration.SensorCount1 > 0) &&
                 (this.annotationForm.SensorConfiguration.SensorCount2 > 0))
            {

                expectedSamplingRate = 300;

            }
            else
            {
                expectedSamplingRate = 140;

            }

            //adjust for HR
            if (this.annotationForm.SensorConfiguration.IsHR)
            {
                expectedSamplingRate -= ((int)(expectedSamplingRate / (this.annotationForm.SensorConfiguration.SensorCount1 +
                    this.annotationForm.SensorConfiguration.SensorCount2)));
            }


            if ((mrc != null) && (mrc.GetComPortNumber() != 0))
            {
                // Found valid port
                textBoxAC1.Text = "COM: " + mrc.GetComPortNumber();
                isStartedReceiver = true;
            }
            else
            {
                textBoxAC1.Text = "No MITes Receiver!";
                isStartedReceiver = false;
                Thread.Sleep(10000);
                //Application.Exit ();
            }

            if ((mrc2 != null) && (mrc2.GetComPortNumber() != 0))
            {
                // Found valid port
                textBoxAC1.Text = "2nd COM: " + mrc2.GetComPortNumber();
                isStartedReceiver = true;
            }

                

        }

        /// <summary>
        /// 
        /// </summary>
        public void ShowForms()
        {
            this.Show();
            this.annotationForm.Show();   
        }
        

      
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			//myUDPServer.Stop = true;
			//Thread.Sleep (200);
			base.Dispose( disposing );
		}
	
		

		#region Graphing functions
		/// <summary>
		/// 
		/// </summary>
		/// <param name="pevent"></param>
		protected override void OnPaintBackground(PaintEventArgs pevent)
		{
		}


		private void GraphAccelerometerValues()
		{
			aMITesPlotter.SetAccelResultsData();
			aMITesPlotter.setPlotVals();
		}

		private void Form1_Paint(object sender, PaintEventArgs e)
		{
			if ((backBuffer == null) || (isResized))
			{
				backBuffer = new Bitmap(xDim,yDim);
				isResized = false;
				isNeedRedraw = true;
			}
		
			if (isPlotting)
			{
				using (Graphics g = Graphics.FromImage(backBuffer))
				{
					//g.PageUnit=GraphicsUnit.Pixel;
					if ((aMITesPlotter.IsNeedFullRedraw()) || isNeedRedraw)
					{
						//g.DrawImage(backgroundImage,0,0,backgroundImage.Width,backgroundImage.Height);
						// works: g.DrawImage(backgroundImage,0,0);
						g.FillRectangle (aBrush,0,0,xDim,yDim);

						Size graphArea = GetGraphSize();

						g.FillRectangle (blueBrush,gapDistance, gapDistance, graphArea.Width, graphArea.Height);

						aMITesPlotter.SetIsFirstTime(false);
						isNeedRedraw = false;
					}
					aMITesPlotter.DrawValsFast(g, false);
				}
			}
			//e.Graphics.DrawImage(backBuffer,0,0,backBuffer.Width,backBuffer.Height);
			e.Graphics.DrawImage(backBuffer,0,0);
		}

		/// <summary>
		/// 
		/// </summary>
		public void SetMaxPlots()
		{
			maxPlots = mrc.GetNumAccelChannels();
			//maxPlots = 3; //SSI
			SetFormPositions();
		}

		private Size GetGraphSize()
		{
			int xsize = xDim - NUM_TEXT_BOX_COLS*gapDistance;
			int ysize = yDim - NUM_TEXT_BOX_ROWS*(textBoxAC1.Height + gapDistance) - NUM_TEXT_BOX_ROWS*gapDistance;
			return new Size(xsize,ysize);			
		}
		#endregion

		#region Image differencing functions

		int diffSum = 0;
		int diffCount = 0; 

		/// <summary>
		/// Processes every new message received as an event; diff info
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void UDPInputProcessor(object sender, InputDetectedEventArgs e)
		{	
			int val = Int32.Parse(e.InputString);
			diffCount++;
			diffSum += val;
			//Console.WriteLine ("Diff: " + val + " Count:" + diffCount);
		}								

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public int GetDiffCount()
		{
			int result;
			if (diffCount == 0)
				result = 0;
			else
				result = (int) Math.Floor(diffSum/((double) diffCount));
			diffSum = 0;
			diffCount = 0;
			//Console.WriteLine ("DiffCount: " + result);
			return result;
		}
		#endregion

		private int printSamplingCount = 0;
		private int stepsToWarning = 0;
		private int time = Environment.TickCount;
		private double ave = 0;
		private int sum = 0;
		private int count = 0;
		int okTimer = 0;
		int flushTimer = 0;
		bool isWrittenKey = false; 

		private void timer1_Tick(object sender, System.EventArgs e)
		{
			if (okTimer == 0)
			{
				//if (isPlaySound)
				//	aMITesSoundPlayer.PlayOK();
			}
			if (okTimer > 3000)
				okTimer = -1;
			okTimer++;

			if (isStartedReceiver)
			{	
				count++;
				if ((Environment.TickCount-time) >= 1000)
				{
					ave = sum / (double) count; 
				
					//textBoxDist.Text = "R: " + count + " T: " + sum;
					aMITesLogger.WriteLogComment("R: " + count + " T: " + sum);
					sum = 0;
					count = 0;
					time = Environment.TickCount;
				}

//				if (sr.HasData())
//				{
//					Console.WriteLine (sr.
//				}

				// RANDY: uncomment this and comment next to save data in the standard way
				// To read data back in, leave as currently is. You need to set the 
				// binary file when aMITesLoggerReader is setup.
                aMITesDecoder.GetSensorData(mrc);
                aMITesDecoder2.GetSensorData(mrc2);

                //if (aMITesDecoder.IsWrongChannel(17))
                //    Console.WriteLine("Wrong channel");
                //if (aMITesDecoder2.IsWrongChannel(7))
                //    Console.WriteLine("Wrong channel");

                aMITesDecoder.MergeDataOrderProperly(aMITesDecoder2);

                //aMITesLoggerReader.GetSensorData(10, true); // true indicates PLFormat 
				
				sum += aMITesDecoder.GetLastByteNum ();
				
				aMITesLogger.SaveRawData();
				if (flushTimer == 0)
					aMITesLogger.FlushBytes();
				if (flushTimer > 6000)
					flushTimer = -1;
				flushTimer++;

				aMITesDataFilterer.RemoveZeroNoise ();


                // Modified the Count Non Noise to keep track of the sampling rate of individual
                // sensors
                int samplingRate = aMITesDataFilterer.CountNonNoise();
                aMITesDecoder.UpdateSamplingRate(samplingRate);

                // Check if we are in a new second (i.e. previous second ended)
                if (this.currentSecond == DateTime.Now.Second)
                    this.currentSamplingRate += samplingRate;
                else //if we are in a new second
                {
                    // Check if the overall sampling rate is of good quality and set
                    // the signal sign on the interface accordingly
                    // CHANGE: need to decide on a good thershold
                   
                    if (this.currentSamplingRate >= expectedSamplingRate)
                    {
                        this.annotationForm.SetSignalSign(true);
                    }
                    else
                    {
                        this.annotationForm.SetSignalSign(false);
                    }

                    // Initialize overall and individual performance counters for sensors that are listed
                    // in sensor.xml and store the counter for the previous second
                    this.currentSamplingRate = samplingRate;
                    foreach (SXML.Sensor sensor in this.annotationForm.SensorConfiguration.Sensors)
                    {
                        int sensor_id = Convert.ToInt32(sensor.ID);
                        TimeSpan ts = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));   
                        if ( (sensor_id == 0) && ((this.currentHRSecond+15) < ts.TotalSeconds))
                        {

                            this.aMITesDataFilterer.MitesPerformanceTracker[0].PreviousCounter = this.aMITesDataFilterer.MitesPerformanceTracker[0].SampleCounter;
                            this.aMITesDataFilterer.MitesPerformanceTracker[0].SampleCounter = 0;
                            this.currentHRSecond = ts.TotalSeconds;
                        }
                        if (sensor_id != 0) 
                        {
                            this.aMITesDataFilterer.MitesPerformanceTracker[sensor_id].PreviousCounter = this.aMITesDataFilterer.MitesPerformanceTracker[sensor_id].SampleCounter;
                            this.aMITesDataFilterer.MitesPerformanceTracker[sensor_id].SampleCounter = 0;
                        }
                    }

                    // Update the interface of the performance form by calling the appropriate delegate
                    this.annotationForm.updateMitesSampling();
                    // Update the current second
                    this.currentSecond = DateTime.Now.Second;
                    //this.currentMinute = DateTime.Now.Minute;
                }
                                                    

				if (printSamplingCount > 500)
				{
					textBoxRate.Text = "Samp: " + aMITesDecoder.GetSamplingRate();
					aMITesLogger.WriteLogComment(textBoxUV.Text);
					printSamplingCount = 0;
				}
				else
					printSamplingCount++;

				aMITesSensorCalibrator.UpdateSensorCalibrator();

				aMITesUVAnalyzer.GetRecentLightVals ();
				ReportUV();

				// Check HR values
				aMITesHRAnalyzer.Update();

				// Check Steps values
                //if (isComputingSteps)
                //    aMITesStepsAnalyzer.Update();

				// Check light values
				aMITesLightAnalyzer.Update();

				// Check temperature values
				aMITesTempAnalyzer.Update();

				// Check object motion 
				aMITesObjectAnalyzer.Update();

				// Check object motion for demo screen
				aMITesDemoObjectAnalyzer.Update();
				//aDemoForm.UpdateLabels();

				// Check current values
				aMITesCurrentAnalyzer.Update();

				// Compute distance 
				aMITesRangeAnalyzer1.UpdateDistanceVals();
				aMITesRangeAnalyzer2.UpdateDistanceVals();
				aMITesRangeAnalyzer3.UpdateDistanceVals();
				aMITesRangeAnalyzer4.UpdateDistanceVals();

				//Compute/get Activity Counts
				aMITesActivityCounter1.UpdateActivityCounts();
				aMITesActivityCounter2.UpdateActivityCounts();
				aMITesActivityCounter3.UpdateActivityCounts();
				aMITesActivityCounter4.UpdateActivityCounts();
				aMITesActivityCounter5.UpdateActivityCounts();
				aMITesActivityCounter6.UpdateActivityCounts();

				//Print max/min values
				aMITesActivityCounter1.PrintMaxMin();
				aMITesActivityCounter2.PrintMaxMin();
				aMITesActivityCounter3.PrintMaxMin();
				aMITesActivityCounter4.PrintMaxMin();
				aMITesActivityCounter5.PrintMaxMin();
				aMITesActivityCounter6.PrintMaxMin();
	
				// Want this to be immediate so moved out of loop
				ReportObject(); 

				if (aMITesActivityCounter1.IsNewEpoch(1000))
				{
					aMITesHRAnalyzer.ComputeEpoch(30000);
					ReportHR();

//FIX					aMITesStepsAnalyzer.ComputeEpoch(30000);
                    //if (isComputingSteps)	
                    //    ReportSteps();
                    //else
                    //    textBoxSteps.Text = "Steps off";

					aMITesActivityCounter1.ComputeEpoch();
					aMITesActivityCounter2.ComputeEpoch();
					aMITesActivityCounter3.ComputeEpoch();
					aMITesActivityCounter4.ComputeEpoch();
					aMITesActivityCounter5.ComputeEpoch();
					aMITesActivityCounter6.ComputeEpoch();
					ReportActivityCounts();

					// For display, set to 5s
					aMITesRangeAnalyzer1.SetEpochTimeMS (5000);
					aMITesRangeAnalyzer2.SetEpochTimeMS (5000);
					aMITesRangeAnalyzer3.SetEpochTimeMS (5000);
					aMITesRangeAnalyzer4.SetEpochTimeMS (5000);

					aMITesRangeAnalyzer1.ComputeEpoch();
					aMITesRangeAnalyzer2.ComputeEpoch();
					aMITesRangeAnalyzer3.ComputeEpoch();
					aMITesRangeAnalyzer4.ComputeEpoch();
					ReportRange();

					//aMITesLightAnalyzer.ComputeEpoch(30000); // 30 second epoch
					aMITesLightAnalyzer.ComputeEpoch(3000); // 3 second epoch
					ReportLight(); 

					aMITesCurrentAnalyzer.ComputeEpoch(3000); // 3 second epoch
					ReportCurrent(); 

					aMITesTempAnalyzer.ComputeEpoch(3000); // 3 second epoch
					ReportTemp(); 

					aMITesObjectAnalyzer.ComputeEpoch(6000); // 3 second epoch

					//ReportTV();

					if (!isWrittenKey) // Write the key once at the top of the file
					{
						isWrittenKey = true;
						aMITesActivityLogger.StartReportKeyLine ();
						aMITesActivityLogger.AddKeyLine (aMITesActivityCounter1);
						aMITesActivityLogger.AddKeyLine (aMITesActivityCounter2);
						aMITesActivityLogger.AddKeyLine (aMITesActivityCounter3);
						aMITesActivityLogger.AddKeyLine (aMITesActivityCounter4);
						aMITesActivityLogger.AddKeyLine (aMITesActivityCounter5);
						aMITesActivityLogger.AddKeyLine (aMITesActivityCounter6);
						aMITesActivityLogger.AddKeyLine (aMITesHRAnalyzer);

						aMITesActivityLogger.AddKeyLine (aMITesRangeAnalyzer1);
						aMITesRangeAnalyzer1.SetEpochTimeMS (30000);
						aMITesActivityLogger.AddKeyLine (aMITesRangeAnalyzer1);
						aMITesRangeAnalyzer1.SetEpochTimeMS (5000);
	
						aMITesActivityLogger.AddKeyLine (aMITesRangeAnalyzer2);
						aMITesRangeAnalyzer2.SetEpochTimeMS (30000);
						aMITesActivityLogger.AddKeyLine (aMITesRangeAnalyzer2);
						aMITesRangeAnalyzer2.SetEpochTimeMS (5000);

						aMITesActivityLogger.AddKeyLine (aMITesRangeAnalyzer3);
						aMITesRangeAnalyzer3.SetEpochTimeMS (30000);
						aMITesActivityLogger.AddKeyLine (aMITesRangeAnalyzer3);
						aMITesRangeAnalyzer3.SetEpochTimeMS (5000);
						
						aMITesActivityLogger.AddKeyLine (aMITesRangeAnalyzer4);
						aMITesRangeAnalyzer4.SetEpochTimeMS (30000);
						aMITesActivityLogger.AddKeyLine (aMITesRangeAnalyzer4);
						aMITesRangeAnalyzer4.SetEpochTimeMS (5000);
						
						aMITesActivityLogger.AddKeyLine (aMITesLightAnalyzer);

						aMITesActivityLogger.AddKeyLine (aMITesCurrentAnalyzer);

						aMITesActivityLogger.AddKeyLine (aMITesTempAnalyzer);

						aMITesActivityLogger.AddKeyLine (aMITesObjectAnalyzer);	

						String s = "TV with threshold " + TVThreshold;
						aMITesActivityLogger.AddKeyLine (s);

                        //s = "ImDiff";
                        //aMITesActivityLogger.AddKeyLine (s);
                        //aMITesActivityLogger.AddKeyLine (aMITesStepsAnalyzer);

						aMITesActivityLogger.SaveReportKeyLine ();
					}

					aMITesActivityLogger.StartReportLine ();
					aMITesActivityLogger.AddReportLine (aMITesActivityCounter1);
					aMITesActivityLogger.AddReportLine (aMITesActivityCounter2);
					aMITesActivityLogger.AddReportLine (aMITesActivityCounter3);
					aMITesActivityLogger.AddReportLine (aMITesActivityCounter4);
					aMITesActivityLogger.AddReportLine (aMITesActivityCounter5);
					aMITesActivityLogger.AddReportLine (aMITesActivityCounter6);
					aMITesActivityLogger.AddReportLine (aMITesHRAnalyzer);

					// First add 5s values then 30s values  
					aMITesActivityLogger.AddReportLine (aMITesRangeAnalyzer1);
					aMITesRangeAnalyzer1.SetEpochTimeMS (30000);
					aMITesRangeAnalyzer1.ComputeEpoch();
					aMITesActivityLogger.AddReportLine (aMITesRangeAnalyzer1);
					aMITesRangeAnalyzer1.SetEpochTimeMS (5000);
					
					aMITesActivityLogger.AddReportLine (aMITesRangeAnalyzer2);
					aMITesRangeAnalyzer2.SetEpochTimeMS (30000);
					aMITesRangeAnalyzer2.ComputeEpoch();
					aMITesActivityLogger.AddReportLine (aMITesRangeAnalyzer2);
					aMITesRangeAnalyzer2.SetEpochTimeMS (5000);

					aMITesActivityLogger.AddReportLine (aMITesRangeAnalyzer3);
					aMITesRangeAnalyzer3.SetEpochTimeMS (30000);
					aMITesRangeAnalyzer3.ComputeEpoch();
					aMITesActivityLogger.AddReportLine (aMITesRangeAnalyzer3);
					aMITesRangeAnalyzer3.SetEpochTimeMS (5000);
					
					aMITesActivityLogger.AddReportLine (aMITesRangeAnalyzer4);	
					aMITesRangeAnalyzer4.SetEpochTimeMS (30000);
					aMITesRangeAnalyzer4.ComputeEpoch();
					aMITesActivityLogger.AddReportLine (aMITesRangeAnalyzer4);	
					aMITesRangeAnalyzer4.SetEpochTimeMS (5000);					
					
					aMITesActivityLogger.AddReportLine (aMITesLightAnalyzer);	
					aMITesActivityLogger.AddReportLine (aMITesCurrentAnalyzer);
					aMITesActivityLogger.AddReportLine (aMITesTempAnalyzer);
					aMITesActivityLogger.AddReportLine (aMITesObjectAnalyzer);

					aMITesActivityLogger.AddReportLine (GetTVState(aMITesLightAnalyzer));
					
					aMITesActivityLogger.AddReportLine ("" + GetDiffCount());

//					aMITesActivityLogger.AddReportLine (aMITesStepsAnalyzer);
					aMITesActivityLogger.SaveReportLine ();
//					st.SendData(aMITesActivityLogger.GetLastWriteTime() + " " + (GetTVState(aMITesLightAnalyzer)));

				}
									
				// Graph accelerometer data 
				if (isPlotting)
					GraphAccelerometerValues();
				
				if (IsShutdownTime())
				{
					ProgramQuit("Program shutdown at shutdown time: " + stopHour);			
				}
				stepsToWarning++;
			}
		}

		#region Menu actions and key actions
		private void ProgramQuit(string aMsg)
		{			
		//	myUDPServer.Stop = true;
			if (mrc != null)
			{
				Thread.Sleep (100);
				mrc.Close ();
				Thread.Sleep (1000);
			}

            if (mrc2 != null)
            {
                Thread.Sleep(100);
                mrc2.Close();
                Thread.Sleep(1000);
            }

            aMITesActivityLogger.WriteLogComment(aMsg);
			Thread.Sleep (100);
			System.Environment.Exit(0);
			//Application.Exit();
		}

		private void menuItem1_Click(object sender, System.EventArgs e)
		{
			ProgramQuit("Program manually quit."); 
		}

		private void menuItem3_Click(object sender, System.EventArgs e)
		{
			aMITesActivityLogger.WriteLogComment("Receiver configure form opened.");
			rcf.Show();
			rcf.ReadChannels();	
		}

		private void menuItem4_Click(object sender, System.EventArgs e)
		{
		}

		private void menuItem7_Click(object sender, System.EventArgs e)
		{
			DateTime dt = DateTime.Now;
			//aMITesSoundPlayer.PlaySync();
			aMITesLogger.WriteLogComment("timeSync " + dt.Millisecond);
		}

		private void menuItem6_Click(object sender, System.EventArgs e)
		{
			DateTime dt = DateTime.Now;
			//aMITesSoundPlayer.PlaySync();
			aMITesLogger.WriteLogComment("timeSync start " + dt.Millisecond);		
		}

		private void menuItem8_Click(object sender, System.EventArgs e)
		{
			DateTime dt = DateTime.Now;
			//aMITesSoundPlayer.PlaySync();
			aMITesLogger.WriteLogComment("timeSync start " + dt.Millisecond);				
		}

		private void Form1_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			if (e.KeyChar == 'i')
			{
				TVThreshold++;
			}
			else if (e.KeyChar == 'd')
			{
				TVThreshold--;
			}
			Console.WriteLine ("TV Threshold changed to: " + TVThreshold);
			aMITesActivityLogger.WriteLogComment("TVThreshold changed to: " + TVThreshold);
		}
		#endregion

		private void menuItem11_Click(object sender, System.EventArgs e)
		{
			isPlotting = true;
			labelPlottingStatus.Text = "Plotting!";
			labelPlottingStatus.Visible = false;
		}

		private void menuItem12_Click(object sender, System.EventArgs e)
		{
			isPlotting = false;
			labelPlottingStatus.Visible = true;
			labelPlottingStatus.Text = "Plotting off!";
		}

		private void menuItem14_Click(object sender, System.EventArgs e)
		{
			//isPlaySound = false;
		}

		private void menuItem15_Click(object sender, System.EventArgs e)
		{
			//isPlaySound = true; 
		}

		private void menuItem17_Click(object sender, System.EventArgs e)
		{
            //isBeepOnStep = true;
            //aMITesStepsAnalyzer.SetBeepOnStep (isBeepOnStep);
		}

		private void menuItem18_Click(object sender, System.EventArgs e)
		{
            //isBeepOnStep = false;
            //aMITesStepsAnalyzer.SetBeepOnStep (isBeepOnStep);
		}

		private void menuItem19_Click(object sender, System.EventArgs e)
		{
            //isBeepOnStep = false;
            //aMITesStepsAnalyzer.SetBeepOnStep (isBeepOnStep);
		}

		private void menuItem19_Click_1(object sender, System.EventArgs e)
		{
            //isComputingSteps = true;
            //textBoxSteps.Text = "Compute steps";
		}

		private void menuItem20_Click(object sender, System.EventArgs e)
		{
            //isComputingSteps = false;
            //textBoxSteps.Text = "No steps processing";
		}

        private void menuItem21_Click(object sender, EventArgs e)
        {
            aMITesActivityLogger.WriteLogComment("Receiver2 configure form opened.");
            rcf2.Show();
            rcf2.ReadChannels();	
        }
	}
}
