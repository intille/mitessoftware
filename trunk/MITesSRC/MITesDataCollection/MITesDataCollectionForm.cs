using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.Runtime.InteropServices;
using System.IO;
using HousenCS.MITes;
using System.Threading;
using System.Collections;
using AXML;
using SXML;
using MITesDataCollection.Utils;
using MITesFeatures;
using weka.core;
using weka.classifiers;
using weka.classifiers.trees;
using System.IO.Ports;
using System.Text.RegularExpressions;

namespace MITesDataCollection
{
    public enum PlaySoundFlags : int
    {
        SND_SYNC = 0x0,     // play synchronously (default)
        SND_ASYNC = 0x1,    // play asynchronously
        SND_NODEFAULT = 0x2,    // silence (!default) if sound not found
        SND_MEMORY = 0x4,       // pszSound points to a memory file
        SND_LOOP = 0x8,     // loop the sound until next sndPlaySound
        SND_NOSTOP = 0x10,      // don't stop any currently playing sound
        SND_NOWAIT = 0x2000,    // don't wait if the driver is busy
        SND_ALIAS = 0x10000,    // name is a registry alias
        SND_ALIAS_ID = 0x110000,// alias is a predefined ID
        SND_FILENAME = 0x20000, // name is file name
        SND_RESOURCE = 0x40004, // name is resource name or atom
    }

    public partial class MITesDataCollectionForm : Form,ControlCreator
    {


        private static readonly bool DEBUG = true;
        private bool isStartedReceiver = false;
        private double[,] returnVals = new double[3, 4];
        //private MITesReceiverController mrc = null;
        private const int BYTES_BUFFER_SIZE = 4000; //2048 
        private byte[] someBytes = new byte[BYTES_BUFFER_SIZE];
        private bool isResized = false;
        private bool isNeedRedraw = false;
        private int xDim = 240;
        private int yDim = 320;
        private int maxPlots = 3; // Changed from 6
        private bool isPlotting = true;
        private Bitmap backBuffer = null;
        //private static int NUM_TEXT_BOX_ROWS = 3;
        //private static int NUM_TEXT_BOX_COLS = 2;
        private int printSamplingCount = 0;
        private int stepsToWarning = 0;
        private int time = Environment.TickCount;
        private double ave = 0;
        private int sum = 0;
        private int count = 0;
        int okTimer = 0;
        int flushTimer = 0;
        bool isWrittenKey = false;
        private bool isPlottingFullScreen = false;

        Annotation annotation;
        SensorAnnotation sensors;
        AnnotatedRecord currentRecord;
        private string dataDirectory;
        private string arffFileName;
        private TextWriter tw;
        private int autoTrainingIndex;

        //for simple data collection
        private bool isCollectingSimpleData,isCollectingDetailedData;
        //private TextWriter[] tws;
        //private double[] mitesLastTimeStamps;
        //private int[] mitesSampleCounters;
        //private int[] xs;
        //private int[] ys;
        //private int[] zs;

        private int classificationCounter;
        private bool isExtracting;
        private bool isClassifying;
        Classifier classifier;
        FastVector fvWekaAttributes;
        Instances instances;

        private System.Windows.Forms.MainMenu mainMenuTab2;
        private System.Windows.Forms.MenuItem menuItem1Tab2;
        private System.Windows.Forms.MenuItem menuItem2Tab2;
        private System.Windows.Forms.MenuItem menuItem3Tab2;
        private System.Windows.Forms.MenuItem menuItem4Tab2;
        private System.Windows.Forms.MenuItem menuItem5Tab2;
        private System.Windows.Forms.MenuItem menuItem6Tab2;
        private System.Windows.Forms.MenuItem menuItem7Tab2;
        private System.Windows.Forms.MenuItem menuItem8Tab2;
        //private ProgressForm progressForm;

        #region Definitions of all key MITes related C# objects
        private Hashtable aMITesActivityCounters;
        //private MITesActivityCounter aMITesActivityCounter1;
        //private MITesActivityCounter aMITesActivityCounter2;
        //private MITesActivityCounter aMITesActivityCounter3;
        //private MITesActivityCounter aMITesActivityCounter4;
        //private MITesActivityCounter aMITesActivityCounter5;
        //private MITesActivityCounter aMITesActivityCounter6;
        //private MITesDecoder aMITesDecoder;
        private MITesScalablePlotter aMITesPlotter;
        private MITesHRAnalyzer aMITesHRAnalyzer;
        //private MITesUVAnalyzer aMITesUVAnalyzer;
        //private MITesObjectAnalyzer aMITesObjectAnalyzer;
        private MITesDataFilterer aMITesDataFilterer;
        private MITesLoggerNew aMITesLogger;
        //private MITesSensorCalibrator aMITesSensorCalibrator;
        private MITesActivityLogger aMITesActivityLogger;

        #endregion

        #region GUI objects and global variables
        private Pen aPen = new Pen(Color.Wheat);
        private SolidBrush aBrush = new SolidBrush(Color.White);
        private SolidBrush blueBrush = new SolidBrush(Color.LightBlue);
        private SolidBrush redBrush = new SolidBrush(Color.Red);
        private int gapDistance = 4;
        #endregion

        private ReceiverConfigureForm rcf = null;

        private MITesReceiverController[] mitesControllers;
        private MITesDecoder[] mitesDecoders;

        public MITesDataCollectionForm()
        {
        }


        private string progressMessage;



        private void ProgressThread()
        {
            ProgressForm progressForm = new ProgressForm();
            progressForm.Show();
            while (true)
            {
#if (PocketPC)
                Thread.Sleep(5);
#else
                Thread.Sleep(20);
#endif
                
                if (progressMessage != null)
                    progressForm.UpdateProgressBar(progressMessage);
                //progressForm.Refresh();
            }
        }

        //activity count csv variables
        private int[] averageX;
        private int[] averageY;
        private int[] averageZ;
        private int[] averageRawX;
        private int[] averageRawY;
        private int[] averageRawZ;
        private int[] prevX;
        private int[] prevY;
        private int[] prevZ;
        private int[] acCounters;
        private int activityCountWindowSize;
        private TextWriter[] activityCountCSVs;
        private TextWriter[] samplingCSVs;
        private TextWriter[] averagedRaw;
        private TextWriter masterCSV;
        private TextWriter hrCSV;


        public MITesDataCollectionForm(string dataDirectory)
        {
            progressMessage = null;
            Thread t = new Thread(new ThreadStart(ProgressThread));
            t.Start();         
                 

            InitializeComponent();            
            this.dataDirectory = dataDirectory;
         

            //read the sensor configuration file to determine the number of receivers
            //read the activity configuration file
            progressMessage="Loading XML protocol and sensors ...";
            AXML.Reader reader = new AXML.Reader(Constants.MASTER_DIRECTORY, dataDirectory);
            if (reader.validate() == false)
            {
                throw new Exception("Error Code 0: XML format error - activities.xml does not match activities.xsd!");
            }
            else
            {
                this.annotation = reader.parse();
                this.annotation.DataDirectory = dataDirectory;


                SXML.Reader sreader = new SXML.Reader(Constants.MASTER_DIRECTORY, dataDirectory);
                if (sreader.validate() == false)
                {
                    throw new Exception("Error Code 0: XML format error - sensors.xml does not match sensors.xsd!");
                }
                else
                {
                    this.sensors = sreader.parse(Constants.MAX_CONTROLLERS);
                   
                    progressMessage+=" Completed\r\n";
                }
            }

            if (this.sensors.IsHR)
                this.maxPlots = this.sensors.Sensors.Count - 1;
            else
                this.maxPlots = this.sensors.Sensors.Count;

            //check number of sensors etc.

            progressMessage+= "Initializing Timers ...";
            InitializeTimers();
            progressMessage += " Completed\r\n";

            progressMessage += "Initializing GUI ...";
            InitializeInterface();
            progressMessage += " Completed\r\n";

            if ((this.sensors.TotalReceivers > 0) && (this.sensors.TotalReceivers <=Constants.MAX_CONTROLLERS))
            {
                this.mitesControllers = new MITesReceiverController[this.sensors.TotalReceivers];               
                this.mitesDecoders = new MITesDecoder[this.sensors.TotalReceivers];
                this.aMITesActivityCounters = new Hashtable();
                //if (this.sensors.IsHR)
                //    this.aMITesActivityCounters = new MITesActivityCounter[this.sensors.Sensors.Count-1];
                //else
                //    this.aMITesActivityCounters = new MITesActivityCounter[this.sensors.Sensors.Count - 1];

                progressMessage += "Initializing MITes ... searching " + this.sensors.TotalReceivers + " receivers\r\n";
                if (InitializeMITes(dataDirectory) == false)
                {
                    MessageBox.Show("Exiting: You picked a configuration with "+this.sensors.TotalReceivers +" receivers. Please make sure they are attached to the computer.");
#if (PocketPC)
                        Application.Exit();
#else
                    Environment.Exit(0);
#endif
                }
            }


            progressMessage += "Initializing MITes Quality GUI ...";
            InitializeQualityInterface();
            progressMessage += " Completed\r\n";


                //pass data directory
            isExtracting = false;
            Extractor.Initialize( this.mitesDecoders[0], dataDirectory,this.annotation,this.sensors);

            // MITes Data Filterer stores performance stats for MITes
            // Initialize all performance counters for all MITES channels
            //calculate good sampling rate           
            //you need to initialize them all because sometimes mites get data from non-exisiting IDs???
            for (int i = 0; i < MITesData.MAX_MITES_CHANNELS; i++)            
               MITesDataFilterer.MITesPerformanceTracker[i] = new MITesPerformanceStats(0);
            //based on how many receivers and to what channels they are listening adjust the good sampling rate
            foreach (Sensor sensor in this.sensors.Sensors)
            {
                int sensor_id = Convert.ToInt32(sensor.ID);
                int receiver_id = Convert.ToInt32(sensor.Receiver);
                if (sensor_id == 0) //HR sensor
                {
                    MITesDataFilterer.MITesPerformanceTracker[sensor_id].GoodRate =(int)(Constants.HR_SAMPLING_RATE*0.8);
                    MITesDataFilterer.MITesPerformanceTracker[sensor_id].PerfectRate = Constants.HR_SAMPLING_RATE;
                }
                else
                {
                    int goodSamplingRate = (int)((Extractor.Configuration.ExpectedSamplingRate * (1 - Extractor.Configuration.MaximumNonconsecutiveFrameLoss)) / this.sensors.NumberSensors[receiver_id]);
                    MITesDataFilterer.MITesPerformanceTracker[sensor_id].GoodRate = goodSamplingRate;
                    MITesDataFilterer.MITesPerformanceTracker[sensor_id].PerfectRate = (int)((Extractor.Configuration.ExpectedSamplingRate) / this.sensors.NumberSensors[receiver_id]);
                }
            }


            //create some counters for activity counts
            averageX=new int[this.sensors.MaximumSensorID+1];
            averageY= new int[this.sensors.MaximumSensorID+1];
            averageZ= new int[this.sensors.MaximumSensorID+1];

            averageRawX = new int[this.sensors.MaximumSensorID + 1];
            averageRawY = new int[this.sensors.MaximumSensorID + 1];
            averageRawZ = new int[this.sensors.MaximumSensorID + 1];

            prevX= new int[this.sensors.MaximumSensorID+1];
            prevY= new int[this.sensors.MaximumSensorID+1];
            prevZ= new int[this.sensors.MaximumSensorID+1];
            acCounters = new int[this.sensors.MaximumSensorID + 1];
            activityCountWindowSize = 0;

            activityCountCSVs = new StreamWriter[this.sensors.MaximumSensorID + 1];
            samplingCSVs = new StreamWriter[this.sensors.MaximumSensorID + 1];
            averagedRaw = new StreamWriter[this.sensors.MaximumSensorID + 1];
            masterCSV = new StreamWriter(dataDirectory + "\\MITesSummaryData.csv");
            hrCSV = new StreamWriter(dataDirectory + "\\HeartRate_MITes.csv");
            
            string csv_line1 = "UnixTimeStamp,TimeStamp,X,Y,Z";
            string csv_line2 = "UnixTimeStamp,TimeStamp,Sampling";
            string hr_csv_header = "UnixTimeStamp,TimeStamp,HR";  
            string master_csv_header = "UnixTimeStamp,TimeStamp";
            foreach (Category category in this.annotation.Categories)
                master_csv_header += ","+ category.Name;


            foreach (Sensor sensor in this.sensors.Sensors)
            {
                int sensor_id = Convert.ToInt32(sensor.ID);
                string location = sensor.Location.Replace(' ', '-');
                if (sensor_id > 0) //exclude HR
                {
                    activityCountCSVs[sensor_id] = new StreamWriter(dataDirectory + "\\MITes_" +sensor_id.ToString("00")+"_ActivityCount_"+location+".csv");
                    activityCountCSVs[sensor_id].WriteLine(csv_line1);
                    averagedRaw[sensor_id] = new StreamWriter(dataDirectory + "\\MITes_" + sensor_id.ToString("00") + "_1s-RawMean_" + location + ".csv");
                    averagedRaw[sensor_id].WriteLine(csv_line1);
                    samplingCSVs[sensor_id] = new StreamWriter(dataDirectory + "\\MITes_" + sensor_id.ToString("00") + "_SampleRate_" + location + ".csv");
                    samplingCSVs[sensor_id].WriteLine(csv_line2);
                    master_csv_header += ",MITes" + sensor_id.ToString("00") + "_SR," + "MITes" + sensor_id.ToString("00") + "_AVRaw_X," +
                        "MITes" + sensor_id.ToString("00") + "_AVRaw_Y," + "MITes" + sensor_id.ToString("00") + "_AVRaw_Z," + "MITes" + sensor_id.ToString("00") + "_AC_X," +
                        "MITes" + sensor_id.ToString("00") + "_AC_Y," + "MITes" + sensor_id.ToString("00") + "_AC_Z";

                }
            }

            master_csv_header += ",HR";
            this.masterCSV.WriteLine(master_csv_header);
            this.hrCSV.WriteLine(hr_csv_header);

            //activityCountCSV=new StreamWriter(dataDirectory+"\\"+Constants.ACTIVITY_COUNT_FILENAME);
            //string csv_line = "";
            //csv_line += "UnixTimeStamp";
            //foreach (Sensor sensor in this.sensors.Sensors)
            //{
            //    int sensor_id = Convert.ToInt32(sensor.ID);
            //    if (sensor_id > 0) //exclude HR
            //    {
            //        csv_line += ",ACC_" + sensor_id + "_SAMP";
            //        csv_line += ",ACC_" + sensor_id + "_X";
            //        csv_line += ",ACC_" + sensor_id + "_Y";
            //        csv_line += ",ACC_" + sensor_id + "_Z";
            //    }
            //}
            //activityCountCSV.WriteLine(csv_line);

            UnixTime.InitializeTime(); //passed to adjust time when its granularity is not good
#if (PocketPC)
            this.tabControl1.SelectedIndex= 0;
#endif
            isCollectingSimpleData = false;
            isCollectingDetailedData = false;
            isPlotting = true;

            //simple data collection per second
            //this.tws = new TextWriter[MITesData.MAX_MITES_CHANNELS];
            //for (int i = 0; (i < MITesData.MAX_MITES_CHANNELS); i++)
            //    this.tws[i] = null;

            //this.mitesLastTimeStamps=new double[MITesData.MAX_MITES_CHANNELS];
            //this.mitesSampleCounters= new int[MITesData.MAX_MITES_CHANNELS];
            //this.xs=new int[MITesData.MAX_MITES_CHANNELS];
            //this.ys = new int[MITesData.MAX_MITES_CHANNELS];
            //this.zs = new int[MITesData.MAX_MITES_CHANNELS];

            //For demo
            //this.textBoxHR.Visible = false;
            //this.textBoxAC3.Visible = false;
            //this.label11.Text = "";

            bool startReceiverThreads = true;
            for (int i = 0; (i<this.sensors.TotalReceivers); i++)
            {
                if ((this.mitesControllers[i] == null) || (this.mitesControllers[i].GetComPortNumber() == 0))
                    startReceiverThreads = false;
            }

            if (startReceiverThreads == true)
                isStartedReceiver = true;
            else
            {
                Application.Exit();
            }


            t.Abort();
#if (PocketPC)
#else
            this.ShowForms();
#endif

            
            //Last thing enable the timers
            this.readDataTimer.Enabled = true;
            this.qualityTimer.Enabled = true;
            this.HRTimer.Enabled = true;
        }


        private int[] labelCounters;
        private string[] activityLabels;
        private Hashtable labelIndex;
        public MITesDataCollectionForm(string dataDirectory,string arffFile)
        {
            /*
            int i = 0, j = 0;

            labelIndex = new Hashtable();
            this.dataDirectory = dataDirectory;
            //this.mitesControllers = new ArrayList();

            InitializeComponent();
            this.tabControl1.TabPages.RemoveAt(1);
            //this.tabControl1.TabPages.RemoveAt(3);
            this.tabControl1.Size = new Size(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height - Constants.SCREEN_TOP_MARGIN - Constants.SCREEN_BOTTOM_MARGIN - (this.tabControl1.Size.Height - this.tabControl1.ClientSize.Height));
            InitializeAnnotator(dataDirectory);
            InitializeMITes();
            //pass data directory
            Extractor.Initialize(aMITesDecoder, dataDirectory, this.annotation, this.sensors);
            UnixTime.InitializeTime((int)Extractor.EXPECTED_SAMPLES_SPACING);

            instances = new Instances(new StreamReader(arffFile));            
            instances.Class = instances.attribute(Extractor.ArffAttributeLabels.Length);
            classifier = new J48();
            classifier.buildClassifier(instances);
            fvWekaAttributes = new FastVector(Extractor.ArffAttributeLabels.Length + 1);
            for (i = 0; (i < Extractor.ArffAttributeLabels.Length); i++)
                fvWekaAttributes.addElement(new weka.core.Attribute(Extractor.ArffAttributeLabels[i]));

            FastVector fvClassVal = new FastVector();
            labelCounters = new int[((AXML.Category)this.annotation.Categories[0]).Labels.Count+1];
            activityLabels = new string[((AXML.Category)this.annotation.Categories[0]).Labels.Count+1];
            for (i = 0; (i < ((AXML.Category)this.annotation.Categories[0]).Labels.Count); i++)
            {
                labelCounters[i] = 0;
                string label = "";
                for (j = 0; (j < this.annotation.Categories.Count - 1); j++)
                    label += ((AXML.Label)((AXML.Category)this.annotation.Categories[j]).Labels[i]).Name.Replace(' ','_') + "_";
                label += ((AXML.Label)((AXML.Category)this.annotation.Categories[j]).Labels[i]).Name.Replace(' ', '_');
                //labelCounters.Add(label, 0);
                activityLabels[i] = label;
                labelIndex.Add(label, i);
                fvClassVal.addElement(label);
            }
            //activityLabels[i] = "unknown";
            //labelIndex.Add("unknown", i);
            //fvClassVal.addElement("unknown");
            weka.core.Attribute ClassAttribute = new weka.core.Attribute("activity", fvClassVal);
            
            isClassifying = true;
            this.tabControl1.SelectedIndex = 0;

            isCollectingSimpleData = false;
            isCollectingDetailedData = false;

            isPlotting = false;

            //for simple data
            //this.tws = new TextWriter[MITesData.MAX_MITES_CHANNELS];
            //for (i = 0; (i < MITesData.MAX_MITES_CHANNELS); i++)
            //    this.tws[i] = null;
            //this.mitesLastTimeStamps=new double[MITesData.MAX_MITES_CHANNELS];
            //this.mitesSampleCounters= new int[MITesData.MAX_MITES_CHANNELS];
            //this.xs = new int[MITesData.MAX_MITES_CHANNELS];
            //this.ys = new int[MITesData.MAX_MITES_CHANNELS];
            //this.zs = new int[MITesData.MAX_MITES_CHANNELS];


            //For demo
            //this.textBoxHR.Visible = false;
           // this.textBoxAC3.Visible = false;
            //this.label11.Text = "";
            if ((mrc != null) && (mrc.GetComPortNumber() != 0))
            {
                // Found valid port
               // textBoxAC1.Text = "COM: " + mrc.GetComPortNumber();
                isStartedReceiver = true;
            }
            else
            {
               // textBoxAC1.Text = "No MITes Receiver!";
                isStartedReceiver = false;
                Thread.Sleep(10000);
                //Application.Exit ();
            }
            */
        }

        public bool IsClassifying
        {
            get
            {
                return this.isClassifying;
            }
            set
            {
                this.isClassifying = value;
            }
        }


        #region Annotator

        private ATimer goodTimer, overallTimer;
        private ArrayList categoryButtons;
        private ArrayList buttonIndex;
        private string longest_label = "";
        public const int GOOD_TIMER = 1;
        public const int OVERALL_TIMER = 2;

        delegate void SetTextCallback(string label, int control_id);
        delegate void SetSignalCallback(bool isGood);

        public void SetText(string label, int control_id)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.label1.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { label, control_id });
            }
            else
            {
                if (control_id == GOOD_TIMER)
                    this.label1.Text = label;
                else if (control_id == OVERALL_TIMER)
                {
                    this.label3.Text = label;

                }
            }
        }


        public void SetSignalSign(bool isGood)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.pictureBox1.InvokeRequired)
            {
                SetSignalCallback d = new SetSignalCallback(SetSignalSign);
                this.Invoke(d, new object[] { isGood });
            }
            else
            {
                //set the sign + control the good/bad timer
                if (isGood)
                {
                    if (this.startStopButton.BackColor == System.Drawing.Color.Red)
                        this.goodTimer.start();
                    //this.pictureBox1.Image = System.Drawing.ImageFromFile(STRONG_SIGNAL_FILE);
                }
                else
                {
                    if (this.startStopButton.BackColor == System.Drawing.Color.Red)
                        this.goodTimer.stop();
                    // this.pictureBox1.Image = System.Drawing.Image.FromFile(WEAK_SIGNAL_FILE);
                }

                //check for heart rate
                //double rate = ((double)this.aMITesDataFilterer.MitesPerformanceTracker[0].PreviousCounter / (double)this.aMITesDataFilterer.MitesPerformanceTracker[0].GoodRate) * 100;
                //Testing - remove random component

                //int randvar = new Random().Next(3);
                //if ((rate > 30))
               // {
               //     this.label4.ForeColor = System.Drawing.Color.Green;
                //}
                //else
               // {
                //    this.label4.ForeColor = System.Drawing.Color.Red;
                //}
            }
        }

        private void InitializeAnnotator(string dataDirectory)
        {
            AXML.Reader reader = new AXML.Reader(Constants.MASTER_DIRECTORY,dataDirectory);
            if (reader.validate() == false)
            {
                throw new Exception("Error Code 0: XML format error - activities.xml does not match activities.xsd!");
            }
            else
            {
                this.annotation = reader.parse();
                this.annotation.DataDirectory = dataDirectory;


                SXML.Reader sreader = new SXML.Reader(Constants.MASTER_DIRECTORY, dataDirectory);
                if (sreader.validate() == false)
                {
                    throw new Exception("Error Code 0: XML format error - sensors.xml does not match sensors.xsd!");
                }
                else
                {
                    this.sensors = sreader.parse(Constants.MAX_CONTROLLERS);
                    InitializeTimers();
                    //InitializeSound();
                    InitializeInterface();
                }
            }
        }

#if (PocketPC)
        private void OnResize(object sender, EventArgs ee)
        {

            if ((this.Width > Constants.FORM_MIN_WIDTH) && (this.Height > Constants.FORM_MIN_HEIGHT))
            {

                this.tabControl1.Width = this.ClientSize.Width;
                this.tabControl1.Height = this.ClientSize.Height;
                this.tabPage1.Width =this.panel1.Width  = this.tabPage2.Width = this.tabPage3.Width = this.tabPage4.Width = this.tabControl1.ClientSize.Width;
                this.tabPage1.Height = this.panel1.Height = this.tabPage2.Height = this.tabPage3.Height = this.tabPage4.Height = this.tabControl1.ClientSize.Height;


                //Intialize Labels 40% of the screen

                int num_rows = (int)((this.sensors.Sensors.Count + 2) / 2); //additional row for HR and total sampling rate
                int textBoxHeight = ((int)(0.40 * this.tabPage1.ClientSize.Height) - ((this.sensors.Sensors.Count - 1) * Constants.WIDGET_SPACING)) / num_rows;
                int textBoxWidth = ((this.tabPage1.ClientSize.Width - (3 * Constants.WIDGET_SPACING)) / 2);
                int currentTextY = (int)(this.tabPage1.Height * 0.60);
                int leftTextX = Constants.WIDGET_SPACING;
                int rightTextX = (Constants.WIDGET_SPACING * 2) + textBoxWidth;
                int currentTextX = Constants.WIDGET_SPACING;
                //System.Windows.Forms.Label samplingLabel = (System.Windows.Forms.Label)this.textBoxes[0];
                //samplingLabel.Width = textBoxWidth;
                //samplingLabel.Height = textBoxHeight;
                Form f = new Form();
                f.Width = textBoxWidth;
                f.Height = textBoxHeight;
                Font textFont = GUI.CalculateBestFitFont(f.CreateGraphics(), Constants.MIN_FONT, Constants.MAX_FONT,
                     f.ClientSize, "textBoxAC11", new Font(Constants.FONT_FAMILY, (float)32.0, FontStyle.Bold), (float)0.9, (float)0.9);

                System.Windows.Forms.Label t;
                foreach (Sensor sensor in this.sensors.Sensors)
                {
                    
                    string labelKey = "MITes" + sensor.ID;

                    t=(System.Windows.Forms.Label)this.sensorLabels[labelKey];
                    t.Font = textFont;
                    t.Size = new System.Drawing.Size(textBoxWidth, textBoxHeight);
                    t.Location = new System.Drawing.Point(currentTextX, currentTextY);
                    if (currentTextX == leftTextX)
                        currentTextX = rightTextX;
                    else
                    {
                        currentTextX = leftTextX;
                        currentTextY += (textBoxHeight + Constants.WIDGET_SPACING);
                    }                    
                }
          
                t = (System.Windows.Forms.Label)this.sensorLabels["SampRate"];
                t.Font = textFont;
                t.Size = new System.Drawing.Size(textBoxWidth, textBoxHeight);
                t.Location = new System.Drawing.Point(currentTextX, currentTextY);
                if (currentTextX == leftTextX)
                    currentTextX = rightTextX;
                else
                {
                    currentTextX = leftTextX;
                    currentTextY += (textBoxHeight + Constants.WIDGET_SPACING);
                }   

                //foreach (System.Windows.Forms.Label t in this.sensorLabels)
                //{
                //    t.Font = textFont;
                //    t.Size = new System.Drawing.Size(textBoxWidth, textBoxHeight);
                //    t.Location = new System.Drawing.Point(currentTextX, currentTextY);
                //    if (currentTextX == leftTextX)
                //        currentTextX = rightTextX;
                //    else
                //    {
                //        currentTextX = leftTextX;
                //        currentTextY += (textBoxHeight + Constants.WIDGET_SPACING);
                //    }
                //}


                //Initialize Buttons
                int button_width = this.tabPage2.ClientSize.Width - Constants.SCREEN_LEFT_MARGIN - Constants.SCREEN_RIGHT_MARGIN;
                int button_height = (this.tabPage2.ClientSize.Height - Constants.SCREEN_TOP_MARGIN - Constants.SCREEN_BOTTOM_MARGIN - (this.annotation.Categories.Count * Constants.WIDGET_SPACING)) / (this.annotation.Categories.Count + 1);
                int button_x = Constants.SCREEN_LEFT_MARGIN;
                int button_y = Constants.SCREEN_TOP_MARGIN * 2;

                int delta_y = button_height + Constants.WIDGET_SPACING;
                int button_id = 0;

                f.Size = new Size(button_width, button_height);
                Font buttonFont = GUI.CalculateBestFitFont(f.CreateGraphics(), Constants.MIN_FONT, Constants.MAX_FONT,
                    f.ClientSize, longest_label, new Font(Constants.FONT_FAMILY, (float)32.0, FontStyle.Bold), (float)0.9, (float)0.9);
                foreach (System.Windows.Forms.Button button in categoryButtons)
                {
                    button.Location = new System.Drawing.Point(button_x, button_y + button_id * delta_y);
                    button.Font = buttonFont;
                    button.Size = new System.Drawing.Size(button_width, button_height);
                    button_id++;
                }

                //adjust round buttons start/stop -reset
                button_width = (this.Size.Width - Constants.SCREEN_LEFT_MARGIN - Constants.SCREEN_RIGHT_MARGIN - Constants.WIDGET_SPACING) / 2;
                this.startStopButton.Size = new System.Drawing.Size(button_width, button_height);
                this.resetButton.Size = new System.Drawing.Size(button_width, button_height);
                this.startStopButton.Location = new System.Drawing.Point(Constants.SCREEN_LEFT_MARGIN, button_y + button_id * delta_y);
                this.resetButton.Location = new System.Drawing.Point(this.startStopButton.Location.X + this.startStopButton.Size.Width + Constants.WIDGET_SPACING, button_y + button_id * delta_y);
                this.startStopButton.Font = buttonFont;
                this.resetButton.Font = buttonFont;

                //adjust the size of the plotter
                aMITesPlotter = new MITesScalablePlotter(this.panel1, MITesScalablePlotter.DeviceTypes.IPAQ, maxPlots, this.mitesDecoders[0], new Size(this.panel1.Width, (int)(0.60 * this.panel1.Height)));
                SetFormPositions();
                this.isResized = true;
            }
        }
#else
        private void OnResizeForm3(object sender, EventArgs ee)
        {

        }

       
        private void OnResizeForm4(object sender, EventArgs ee)
        {


            if ((this.form4.Width > Constants.FORM_MIN_WIDTH) && (this.form4.Height > Constants.FORM_MIN_HEIGHT))
            {

                this.panel4.Width = this.form4.ClientSize.Width;
                this.panel4.Height = this.form4.ClientSize.Height;

                int counter = 0;
                int label_width = (this.panel4.ClientSize.Width - Constants.SCREEN_LEFT_MARGIN - Constants.SCREEN_RIGHT_MARGIN) / 3;
                int label_height = (this.panel4.ClientSize.Height - Constants.SCREEN_TOP_MARGIN - Constants.SCREEN_BOTTOM_MARGIN - (this.sensors.Sensors.Count * Constants.WIDGET_SPACING)) / (this.sensors.Sensors.Count + 1);
                Form f = new Form();
                f.Width = label_width;
                f.Height = label_height;
                Font textFont = GUI.CalculateBestFitFont(f.CreateGraphics(), Constants.MIN_FONT, Constants.MAX_FONT,
                     f.ClientSize, "E(Samp. Rate) ", new Font(Constants.FONT_FAMILY, (float)32.0, FontStyle.Bold), (float)0.9, (float)0.9);


                this.label7.Size = this.label8.Size = this.label9.Size = new Size(label_width, label_height);
                this.label7.Font = this.label8.Font =this.label9.Font = textFont;
                this.label7.Location = new System.Drawing.Point(Constants.SCREEN_LEFT_MARGIN, Constants.SCREEN_TOP_MARGIN + (counter * (label_height + Constants.WIDGET_SPACING)));
                this.label8.Location = new System.Drawing.Point(Constants.SCREEN_LEFT_MARGIN + label_width + Constants.WIDGET_SPACING, Constants.SCREEN_TOP_MARGIN + (counter * (label_height + Constants.WIDGET_SPACING)));
                this.label9.Location = new System.Drawing.Point(Constants.SCREEN_LEFT_MARGIN + label_width + Constants.WIDGET_SPACING + label_width + Constants.WIDGET_SPACING, Constants.SCREEN_TOP_MARGIN + (counter * (label_height + Constants.WIDGET_SPACING)));
  
                counter++;
                foreach (Sensor sensor in this.sensors.Sensors)
                {

                    //setup the labels for the expected sampling rates
                    int sensor_id = Convert.ToInt32(sensor.ID);
                    if (sensor_id > 0) //exclude HR sensor
                    {
                        this.labels[sensor_id].Size = new Size(label_width, label_height);
                        this.labels[sensor_id].Location = new System.Drawing.Point(Constants.SCREEN_LEFT_MARGIN, Constants.SCREEN_TOP_MARGIN + (counter * (label_height + Constants.WIDGET_SPACING)));
                        this.labels[sensor_id].Font = textFont;
                        
                        this.expectedLabels[sensor_id].Size = new Size(label_width, label_height);
                        this.expectedLabels[sensor_id].Location = new System.Drawing.Point(Constants.SCREEN_LEFT_MARGIN + label_width + Constants.WIDGET_SPACING, Constants.SCREEN_TOP_MARGIN + (counter * (label_height + Constants.WIDGET_SPACING)));
                        this.expectedLabels[sensor_id].Font = textFont;

                        this.samplesPerSecond[sensor_id].Size = new Size(label_width, label_height);
                        this.samplesPerSecond[sensor_id].Location = new System.Drawing.Point(Constants.SCREEN_LEFT_MARGIN + label_width + Constants.WIDGET_SPACING + label_width + Constants.WIDGET_SPACING, Constants.SCREEN_TOP_MARGIN + (counter * (label_height + Constants.WIDGET_SPACING)));
                        this.samplesPerSecond[sensor_id].Font = textFont;
                        counter++;
                    }
                }
            }
        }

        private void OnResizeForm2(object sender, EventArgs ee)
        {
            if ((this.form2.Width > Constants.FORM_MIN_WIDTH) && (this.form2.Height > Constants.FORM_MIN_HEIGHT))
            {

                this.panel2.Width = this.form2.ClientSize.Width;
                this.panel2.Height = this.form2.ClientSize.Height;

                //Initialize Buttons
                int button_width = this.panel2.ClientSize.Width - Constants.SCREEN_LEFT_MARGIN - Constants.SCREEN_RIGHT_MARGIN;
                int button_height = (this.panel2.ClientSize.Height - Constants.SCREEN_TOP_MARGIN - Constants.SCREEN_BOTTOM_MARGIN - (this.annotation.Categories.Count * Constants.WIDGET_SPACING)) / (this.annotation.Categories.Count + 1);
                int button_x = Constants.SCREEN_LEFT_MARGIN;
                int button_y = Constants.SCREEN_TOP_MARGIN * 2;

                int delta_y = button_height + Constants.WIDGET_SPACING;
                int button_id = 0;
                Form f=new Form();
                f.Size = new Size(button_width, button_height);
                Font buttonFont = GUI.CalculateBestFitFont(f.CreateGraphics(), Constants.MIN_FONT, Constants.MAX_FONT,
                    f.ClientSize, longest_label, new Font(Constants.FONT_FAMILY, (float)32.0, FontStyle.Bold), (float)0.9, (float)0.9);
                foreach (System.Windows.Forms.Button button in categoryButtons)
                {
                    button.Location = new System.Drawing.Point(button_x, button_y + button_id * delta_y);
                    button.Font = buttonFont;
                    button.Size = new System.Drawing.Size(button_width, button_height);
                    button_id++;
                }

                //adjust round buttons start/stop -reset
                button_width = (this.panel2.Size.Width - Constants.SCREEN_LEFT_MARGIN - Constants.SCREEN_RIGHT_MARGIN - Constants.WIDGET_SPACING) / 2;
                this.startStopButton.Size = new System.Drawing.Size(button_width, button_height);
                this.resetButton.Size = new System.Drawing.Size(button_width, button_height);
                this.startStopButton.Location = new System.Drawing.Point(Constants.SCREEN_LEFT_MARGIN, button_y + button_id * delta_y);
                this.resetButton.Location = new System.Drawing.Point(this.startStopButton.Location.X + this.startStopButton.Size.Width + Constants.WIDGET_SPACING, button_y + button_id * delta_y);
                this.startStopButton.Font = buttonFont;
                this.resetButton.Font = buttonFont;

            }
        }
        private void OnResizeForm1(object sender, EventArgs ee)
        {

            if ((this.form1.Width > Constants.FORM_MIN_WIDTH) && (this.form1.Height > Constants.FORM_MIN_HEIGHT))
            {

                this.panel1.Width = this.form1.ClientSize.Width;
                this.panel1.Height = this.form1.ClientSize.Height;


                //Intialize Labels 40% of the screen

                int num_rows = (int)((this.sensors.Sensors.Count + 2) / 2); //additional row for HR and total sampling rate
                int textBoxHeight = ((int)(0.40 * this.panel1.ClientSize.Height) - ((this.sensors.Sensors.Count - 1) * Constants.WIDGET_SPACING)) / num_rows;
                int textBoxWidth = ((this.panel1.ClientSize.Width - (3 * Constants.WIDGET_SPACING)) / 2);
                int currentTextY = (int)(this.panel1.Height * 0.60);
                int leftTextX = Constants.WIDGET_SPACING;
                int rightTextX = (Constants.WIDGET_SPACING * 2) + textBoxWidth;
                int currentTextX = Constants.WIDGET_SPACING;
                //System.Windows.Forms.Label samplingLabel = (System.Windows.Forms.Label)this.textBoxes[0];
                //samplingLabel.Width = textBoxWidth;
                //samplingLabel.Height = textBoxHeight;
                Form f = new Form();
                f.Width = textBoxWidth;
                f.Height = textBoxHeight;
                Font textFont = GUI.CalculateBestFitFont(f.CreateGraphics(), Constants.MIN_FONT, Constants.MAX_FONT,
                     f.ClientSize, "textBoxAC11", new Font(Constants.FONT_FAMILY, (float)32.0, FontStyle.Bold), (float)0.9, (float)0.9);

                System.Windows.Forms.Label t;
                foreach (Sensor sensor in this.sensors.Sensors)
                {

                    string labelKey = "MITes" + sensor.ID;

                    t = (System.Windows.Forms.Label)this.sensorLabels[labelKey];
                    t.Font = textFont;
                    t.Size = new System.Drawing.Size(textBoxWidth, textBoxHeight);
                    t.Location = new System.Drawing.Point(currentTextX, currentTextY);
                    if (currentTextX == leftTextX)
                        currentTextX = rightTextX;
                    else
                    {
                        currentTextX = leftTextX;
                        currentTextY += (textBoxHeight + Constants.WIDGET_SPACING);
                    }
                }

                t = (System.Windows.Forms.Label)this.sensorLabels["SampRate"];
                t.Font = textFont;
                t.Size = new System.Drawing.Size(textBoxWidth, textBoxHeight);
                t.Location = new System.Drawing.Point(currentTextX, currentTextY);
                if (currentTextX == leftTextX)
                    currentTextX = rightTextX;
                else
                {
                    currentTextX = leftTextX;
                    currentTextY += (textBoxHeight + Constants.WIDGET_SPACING);
                }

                
                //adjust the size of the plotter
                aMITesPlotter = new MITesScalablePlotter(this.panel1, MITesScalablePlotter.DeviceTypes.IPAQ, maxPlots, this.mitesDecoders[0], new Size(this.panel1.Width, (int)(0.60 * this.panel1.Height)));
                SetFormPositions();
                this.isResized = true;
            }
        }

#endif 
        private Hashtable sensorLabels;

        private System.Windows.Forms.Label[] labels;
        private System.Windows.Forms.Label[] expectedLabels;
        private System.Windows.Forms.Label[] samplesPerSecond;



        private void InitializeQualityInterface()
        {
            this.labels = new System.Windows.Forms.Label[this.sensors.MaximumSensorID+1];
            this.expectedLabels = new System.Windows.Forms.Label[this.sensors.MaximumSensorID+1];
            this.samplesPerSecond = new System.Windows.Forms.Label[this.sensors.MaximumSensorID + 1];

            int counter = 0;
            int label_width = (this.panel4.ClientSize.Width - Constants.SCREEN_LEFT_MARGIN - Constants.SCREEN_RIGHT_MARGIN)/3;

            int label_height = 0;

            if (this.sensors.IsHR)
                label_height=(this.panel4.ClientSize.Height - Constants.SCREEN_TOP_MARGIN - Constants.SCREEN_BOTTOM_MARGIN - (this.sensors.Sensors.Count * Constants.WIDGET_SPACING)) / (this.sensors.Sensors.Count);
            else
                label_height = (this.panel4.ClientSize.Height - Constants.SCREEN_TOP_MARGIN - Constants.SCREEN_BOTTOM_MARGIN - (this.sensors.Sensors.Count * Constants.WIDGET_SPACING)) / (this.sensors.Sensors.Count+1);

            Form f = new Form();
            f.Width = label_width;
            f.Height = label_height;
            Font textFont = GUI.CalculateBestFitFont(f.CreateGraphics(), Constants.MIN_FONT, Constants.MAX_FONT,
                 f.ClientSize, "E(Samp. Rate) ", new Font(Constants.FONT_FAMILY, (float)32.0, FontStyle.Bold), (float)0.9, (float)0.9);


            this.label7.Size = this.label8.Size = this.label9.Size = new Size(label_width, label_height);
            this.label7.Font = this.label8.Font = this.label9.Font = textFont;
            this.label7.Location= new System.Drawing.Point(Constants.SCREEN_LEFT_MARGIN, Constants.SCREEN_TOP_MARGIN + (counter * (label_height + Constants.WIDGET_SPACING)));
            this.label8.Location = new System.Drawing.Point(Constants.SCREEN_LEFT_MARGIN + label_width + Constants.WIDGET_SPACING, Constants.SCREEN_TOP_MARGIN + (counter * (label_height + Constants.WIDGET_SPACING)));
            this.label8.Text = "E(Samp. Rate)";

            this.label9.Location = new System.Drawing.Point(Constants.SCREEN_LEFT_MARGIN + label_width + Constants.WIDGET_SPACING + label_width + Constants.WIDGET_SPACING, Constants.SCREEN_TOP_MARGIN + (counter * (label_height + Constants.WIDGET_SPACING)));
            this.label9.Text = "Samples/Second";

            counter++;
            foreach (Sensor sensor in this.sensors.Sensors)
            {

                //setup the labels for the expected sampling rates
                int sensor_id = Convert.ToInt32(sensor.ID);

                if (sensor_id > 0) //exclude HR sensor
                {
                    System.Windows.Forms.Label label = new System.Windows.Forms.Label();
                    //label.AutoSize = true;
                    label.Size = new Size(label_width, label_height);
                    label.Location = new System.Drawing.Point(Constants.SCREEN_LEFT_MARGIN, Constants.SCREEN_TOP_MARGIN + (counter * (label_height + Constants.WIDGET_SPACING)));
                    label.Name = sensor.ID;
                    label.Text = "Sensor " + sensor.ID;
                    label.Font = textFont;
                    this.labels[sensor_id] = label;
                    this.panel4.Controls.Add(label);

                    System.Windows.Forms.Label label2 = new System.Windows.Forms.Label();
                    //label2.AutoSize = true;
                    label2.Size = new Size(label_width, label_height);
                    label2.Location = new System.Drawing.Point(Constants.SCREEN_LEFT_MARGIN + label_width + Constants.WIDGET_SPACING, Constants.SCREEN_TOP_MARGIN + (counter * (label_height + Constants.WIDGET_SPACING)));
                    label2.Name = "E(SR) " + sensor.ID;
                    label2.Text = "unknown"; //rate.ToString("00.00") + "%";
                    label2.Font = textFont;
                    this.panel4.Controls.Add(label2);
                    this.expectedLabels[sensor_id] = label2;

                    System.Windows.Forms.Label label3 = new System.Windows.Forms.Label();
                    //label2.AutoSize = true;
                    label3.Size = new Size(label_width, label_height);
                    label3.Location = new System.Drawing.Point(Constants.SCREEN_LEFT_MARGIN + label_width + Constants.WIDGET_SPACING + label_width + Constants.WIDGET_SPACING, Constants.SCREEN_TOP_MARGIN + (counter * (label_height + Constants.WIDGET_SPACING)));
                    label2.Name = "Samples " + sensor.ID;
                    label3.Text = "unknown"; //rate.ToString("00.00") + "%";
                    label3.Font = textFont;
                    this.panel4.Controls.Add(label3);
                    this.samplesPerSecond[sensor_id] = label3;

                    counter++;
                }
            }
        }
        private void InitializeInterface()
        {


            #region Common PC and Pocket PC Widgets
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MITesDataCollectionForm));
            this.mainMenu1 = new System.Windows.Forms.MainMenu();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.menuItem4 = new System.Windows.Forms.MenuItem();
            this.menuItem5 = new System.Windows.Forms.MenuItem();
            this.menuItem6 = new System.Windows.Forms.MenuItem();
            this.menuItem7 = new System.Windows.Forms.MenuItem();
            this.menuItem8 = new System.Windows.Forms.MenuItem();
            this.menuItem9 = new System.Windows.Forms.MenuItem();
            this.menuItem21 = new System.Windows.Forms.MenuItem();
            this.menuItem22 = new System.Windows.Forms.MenuItem();
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
            //this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.resetButton = new System.Windows.Forms.Button();
            this.startStopButton = new System.Windows.Forms.Button();
            this.oxyconButton = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.readDataTimer = new System.Windows.Forms.Timer();
            this.panel1 = new Panel();
            this.panel2 = new Panel();
            this.panel3 = new Panel();
            this.panel4 = new Panel();

            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.Add(this.menuItem1);
            this.mainMenu1.MenuItems.Add(this.menuItem2);
            // 
            // menuItem1
            // 
            this.menuItem1.Text = "Quit";
            this.menuItem1.Click += new System.EventHandler(this.menuItem1_Click);
            // 
            // menuItem2
            // 
            this.menuItem2.MenuItems.Add(this.menuItem3);
            this.menuItem2.MenuItems.Add(this.menuItem4);
            this.menuItem2.MenuItems.Add(this.menuItem5);
            this.menuItem2.MenuItems.Add(this.menuItem9);
            this.menuItem2.MenuItems.Add(this.menuItem10);
            this.menuItem2.MenuItems.Add(this.menuItem13);
            this.menuItem2.MenuItems.Add(this.menuItem16);
            this.menuItem2.Text = "Options";
            // 
            // menuItem3
            // 
            this.menuItem3.Text = "Set Channels";
            this.menuItem3.Click += new System.EventHandler(this.menuItem3_Click);
            this.menuItem3.Enabled = false;
            // 
            // menuItem4
            // 
            this.menuItem4.Text = "Check Data";
            this.menuItem4.Enabled = false;
            // 
            // menuItem5
            // 
            this.menuItem5.MenuItems.Add(this.menuItem6);
            this.menuItem5.MenuItems.Add(this.menuItem7);
            this.menuItem5.MenuItems.Add(this.menuItem8);
            this.menuItem5.Text = "Sync";
            this.menuItem5.Click += new System.EventHandler(this.menuItem5_Click);
            this.menuItem5.Enabled = false;
            // 
            // menuItem6
            // 
            this.menuItem6.Text = "Misc";
            this.menuItem6.Enabled = false;
            // 
            // menuItem7
            // 
            this.menuItem7.Text = "Start Code";
            this.menuItem7.Enabled = false;
            // 
            // menuItem8
            // 
            this.menuItem8.Text = "End Code";
            this.menuItem8.Enabled = false;
            // 
            // menuItem9
            // 
            this.menuItem9.MenuItems.Add(this.menuItem21);
            this.menuItem9.MenuItems.Add(this.menuItem22);
            this.menuItem9.Text = "Files";
            this.menuItem9.Enabled = false;
            // 
            // menuItem21
            // 
            this.menuItem21.Text = "Simple";
            this.menuItem21.Click += new System.EventHandler(this.menuItem21_Click);
            this.menuItem21.Enabled = false;
            // 
            // menuItem22
            // 
            this.menuItem22.Text = "Detailed";
            this.menuItem22.Click += new System.EventHandler(this.menuItem22_Click);
            this.menuItem22.Enabled = false;
            // 
            // menuItem10
            // 
            this.menuItem10.MenuItems.Add(this.menuItem11);
            this.menuItem10.MenuItems.Add(this.menuItem12);
            this.menuItem10.Text = "Plotting";
            // 
            // menuItem11
            // 
            this.menuItem11.Text = "Show";
            this.menuItem11.Click += new System.EventHandler(this.menuItem11_Click);
            // 
            // menuItem12
            // 
            this.menuItem12.Text = "Full Screen";
            this.menuItem12.Enabled = false;
            // 
            // menuItem13
            // 
            this.menuItem13.MenuItems.Add(this.menuItem14);
            this.menuItem13.MenuItems.Add(this.menuItem15);
            this.menuItem13.Text = "Sound";
            this.menuItem13.Enabled = false;
            // 
            // menuItem14
            // 
            this.menuItem14.Text = "Turn off";
            this.menuItem14.Enabled = false;
            // 
            // menuItem15
            // 
            this.menuItem15.Text = "Turn on";
            this.menuItem15.Enabled = false;
            // 
            // menuItem16
            // 
            this.menuItem16.MenuItems.Add(this.menuItem17);
            this.menuItem16.MenuItems.Add(this.menuItem18);
            this.menuItem16.MenuItems.Add(this.menuItem19);
            this.menuItem16.MenuItems.Add(this.menuItem20);
            this.menuItem16.Text = "Steps";
            this.menuItem16.Enabled = false;
            // 
            // menuItem17
            // 
            this.menuItem17.Text = "Beep On";
            this.menuItem17.Enabled = false;
            // 
            // menuItem18
            // 
            this.menuItem18.Text = "Beep Off";
            this.menuItem18.Enabled = false;
            // 
            // menuItem19
            // 
            this.menuItem19.Text = "Compute On";
            this.menuItem19.Enabled = false;
            // 
            // menuItem20
            // 
            this.menuItem20.Text = "Compute Off";
            this.menuItem20.Enabled = false;


            //prepare common PC and Pocket PC widgets

            // 
            // label5
            // 
            //this.label5.Location = new System.Drawing.Point(106, 1);
            //this.label5.Name = "label5";
            //this.label5.Size = new System.Drawing.Size(81, 14);
            //this.label5.Text = "stopped";
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.label4.ForeColor = System.Drawing.Color.Red;
            this.label4.Location = new System.Drawing.Point(187, 2);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 13);
            this.label4.Text = "HR";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(209, -1);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(26, 20);
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.label3.Location = new System.Drawing.Point(53, 2);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 13);
            this.label3.Text = "0:00:00";
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.label2.Location = new System.Drawing.Point(45, 2);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(13, 13);
            this.label2.Text = "/";
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.Green;
            this.label1.Location = new System.Drawing.Point(2, 2);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 13);
            this.label1.Text = "0:00:00";
            // 
            // resetButton
            // 
            this.resetButton.BackColor = System.Drawing.Color.Yellow;
            this.resetButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular);
            this.resetButton.Location = new System.Drawing.Point(127, 182);
            this.resetButton.Name = "resetButton";
            this.resetButton.Size = new System.Drawing.Size(78, 57);
            this.resetButton.TabIndex = 12;
            this.resetButton.Text = "Reset";
            this.resetButton.Click += new System.EventHandler(this.resetButton_Click);
            // 
            // startStopButton
            // 
            this.startStopButton.BackColor = System.Drawing.Color.Green;
            this.startStopButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular);
            this.startStopButton.Location = new System.Drawing.Point(32, 182);
            this.startStopButton.Name = "startStopButton";
            this.startStopButton.Size = new System.Drawing.Size(78, 57);
            this.startStopButton.TabIndex = 11;
            this.startStopButton.Text = "Start";
            this.startStopButton.Click += new System.EventHandler(this.startStopButton_Click);

            // 
            // Oxycon Button
            // 
            this.oxyconButton.BackColor = System.Drawing.Color.Green;
            this.oxyconButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular);
            this.oxyconButton.Location = new System.Drawing.Point(2, 2);
            this.oxyconButton.Name = "oxyconButton";
            this.oxyconButton.Size = new System.Drawing.Size(100, 100);
            this.oxyconButton.TabIndex = 11;
            this.oxyconButton.Text = "Sync Oxycon";
            this.oxyconButton.Click += new System.EventHandler(this.oxycon_Click);


            // 
            // label6
            // 
            this.label6.Font = new System.Drawing.Font("Tahoma", 24F, System.Drawing.FontStyle.Bold);
            this.label6.Location = new System.Drawing.Point(10, 30);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(200, 66);
            this.label6.Text = "label6";

            // 
            // label8
            // 
            this.label8.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.label8.Location = new System.Drawing.Point(103, 9);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(130, 20);
            this.label8.Text = "E (Sampling Rate)";
            // 
            // label7
            // 
            this.label7.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.label7.Location = new System.Drawing.Point(7, 8);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(100, 20);
            this.label7.Text = "Sensor ID";


            //Prepare common PC and Pocket PC panels 
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint);
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel4.Location = new System.Drawing.Point(0, 0);


            // 
            // readDataTimer
            // 
            this.readDataTimer.Enabled = false;
            this.readDataTimer.Interval = 10;
            this.readDataTimer.Tick += new System.EventHandler(this.readDataTimer_Tick);
            // 
            // MITesDataCollectionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.KeyPreview = true;
            this.Name = "MITesDataCollectionForm";
            this.Text = "Collect Data...";


            #endregion Common PC and Pocket PC Widgets

            #region PC and PocketPC specific Widgets
#if (PocketPC)
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.tabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();

                       
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(240, 265);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Location =this.panel1.Location = new System.Drawing.Point(0, 0);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(240, 242);
            this.tabPage1.Text = "Visualize";

            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.label5);
            this.tabPage2.Controls.Add(this.label4);
            this.tabPage2.Controls.Add(this.pictureBox1);
            this.tabPage2.Controls.Add(this.label3);
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Controls.Add(this.label1);
            this.tabPage2.Controls.Add(this.resetButton);
            this.tabPage2.Controls.Add(this.startStopButton);
            this.tabPage2.Location = new System.Drawing.Point(0, 0);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Size = new System.Drawing.Size(232, 239);
            this.tabPage2.Text = "Annotate";
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.label6);
            this.tabPage3.Location = new System.Drawing.Point(0, 0);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(232, 239);
            this.tabPage3.Text = "Classifier";
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.label8);
            this.tabPage4.Controls.Add(this.label7);
            this.tabPage4.Location = new System.Drawing.Point(0, 0);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(232, 239);
            this.tabPage4.Text = "Quality";

            //Add Panels to the tab pages
            this.tabPage1.Controls.Add(this.panel1);
            this.tabPage2.Controls.Add(this.panel2);
            this.tabPage3.Controls.Add(this.panel3);
            this.tabPage4.Controls.Add(this.panel4);

            this.Controls.Add(this.tabControl1);
            this.tabControl1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.Menu = this.mainMenu1;
#else

            this.form1 = new Form();
            this.form2 = new Form();
            this.form3 = new Form();
            this.form4 = new Form();
            this.form1.SuspendLayout();
            this.form2.SuspendLayout();
            this.form3.SuspendLayout();
            this.form4.SuspendLayout();


            // 
            // form1
            // 
            this.form1.Location =new System.Drawing.Point(0, 0);
            this.form1.Name = "form1";
            this.form1.Size = new System.Drawing.Size(240, 242);
            this.form1.Text = "Visualize";

            // 
            // form2
            // 
            this.form2.Location = new System.Drawing.Point(0, 0);
            this.form2.Name = "form2";
            this.form2.Size = new System.Drawing.Size(240, 242);
            this.form2.Text = "Annotate";

            // 
            // form3
            // 
            this.form3.Location = new System.Drawing.Point(0, 0);
            this.form3.Name = "form3";
            this.form3.Size = new System.Drawing.Size(120, 120);
            this.form3.Text = "Oxycon";

            // 
            // form4
            // 
            this.form4.Location = new System.Drawing.Point(0, 0);
            this.form4.Name = "form4";
            this.form4.Size = new System.Drawing.Size(240, 242);
            this.form4.Text = "Quality";


            this.form1.Controls.Add(this.panel1);
            this.form2.Controls.Add(this.panel2);
            this.form3.Controls.Add(this.panel3);
            this.form4.Controls.Add(this.panel4);

            // 
            // tabPage2
            // 
            //this.form2.Controls.Add(this.label5);
            this.form2.Controls.Add(this.label4);
            this.form2.Controls.Add(this.pictureBox1);
            this.form2.Controls.Add(this.label3);
            this.form2.Controls.Add(this.label2);
            this.form2.Controls.Add(this.label1);
            this.form2.Controls.Add(this.resetButton);
            this.form2.Controls.Add(this.startStopButton);
            this.form3.Controls.Add(this.oxyconButton);
            //this.form3.Controls.Add(this.label6);
            this.form4.Controls.Add(this.label8);
            this.form4.Controls.Add(this.label7);
            this.form4.Controls.Add(this.label9);
          


            //Add Panels to the tab pages
            this.form1.Controls.Add(this.panel1);
            this.form2.Controls.Add(this.panel2);
            this.form3.Controls.Add(this.panel3);
            this.form4.Controls.Add(this.panel4);
            this.form1.Menu = this.mainMenu1;
            this.form2.Menu = this.mainMenuTab2;

#endif

            #endregion PC and PocketPC specific Widgets

            this.ResumeLayout(false);

            #region Calculation of Widgets locations and Sizes
            //Initialize screen dimensions           
            this.Width = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width - Constants.SCREEN_WIDTH_MARGIN;
            this.Height = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height - Constants.SCREEN_HEIGHT_MARGIN;            
            if ((this.Width > Constants.MAX_SCREEN_WIDTH) || (this.Height > Constants.MAX_SCREEN_HEIGHT))
            {
                this.Width = this.Width / 2;
                this.Height = this.Height / 2;
            }

#if (PocketPC)
            //Initialize Tab control dimensions
            this.tabControl1.Width = this.ClientSize.Width;
            this.tabControl1.Height = this.ClientSize.Height;
            this.tabPage1.Width=this.panel1.Width = this.tabPage2.Width = this.tabPage3.Width = this.tabPage4.Width = this.tabControl1.ClientSize.Width -Constants.SCREEN_LEFT_MARGIN-Constants.SCREEN_RIGHT_MARGIN;
            this.tabPage1.Height= this.panel1.Height = this.tabPage2.Height = this.tabPage3.Height = this.tabPage4.Height = this.tabControl1.ClientSize.Height;
#else
            this.form1.Width = this.form2.Width = this.form3.Width = this.form4.Width = this.ClientSize.Width;
            this.form1.Height= this.form2.Height = this.form3.Height = this.form4.Height = this.ClientSize.Height;
            this.panel1.Width = this.panel2.Width = this.panel4.Width = this.form1.ClientSize.Width - Constants.SCREEN_LEFT_MARGIN - Constants.SCREEN_RIGHT_MARGIN;
            this.panel2.Height = this.panel4.Height = this.form1.ClientSize.Height;

            this.panel1.Height = (int) (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height * 0.70);
            this.panel3.Width = (int)(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height * 0.20); this.panel3.Height = (int)(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height * 0.20);
#endif

            //Intialize Labels 40% of the screen
            this.sensorLabels = new Hashtable();
            int num_rows= (int)((this.sensors.Sensors.Count + 2) / 2); //additional row for HR and total sampling rate
            int textBoxHeight = ((int)(0.40 * this.panel1.ClientSize.Height) - ((this.sensors.Sensors.Count - 1) * Constants.WIDGET_SPACING)) / num_rows;
            int textBoxWidth = ((this.panel1.ClientSize.Width - (3 * Constants.WIDGET_SPACING)) / 2);
            int currentTextY = (int)(this.panel1.Height * 0.60);
            int leftTextX = Constants.WIDGET_SPACING;
            int rightTextX = (Constants.WIDGET_SPACING * 2) + textBoxWidth;
            int currentTextX = Constants.WIDGET_SPACING;
            System.Windows.Forms.Label  samplingLabel= new System.Windows.Forms.Label();
            samplingLabel.Width = textBoxWidth;
            samplingLabel.Height = textBoxHeight;
            Form f = new Form();
            f.Width = textBoxWidth;
            f.Height = textBoxHeight;
            Font textFont = GUI.CalculateBestFitFont(f.CreateGraphics(), Constants.MIN_FONT, Constants.MAX_FONT,
                 f.ClientSize, "textBoxAC11", new Font(Constants.FONT_FAMILY, (float)32.0, FontStyle.Bold), (float)0.9, (float)0.9);

            foreach (Sensor sensor in this.sensors.Sensors)
            {
                System.Windows.Forms.Label t = new System.Windows.Forms.Label();
                t.Text = "MITes" + sensor.ID;
                t.Font = textFont;
                t.Name = t.Text;
                t.Size = new System.Drawing.Size(textBoxWidth, textBoxHeight);
                t.Location = new System.Drawing.Point(currentTextX, currentTextY);
                this.sensorLabels.Add(t.Text,t);
                //this.tabPage1.Controls.Add(t);
                this.panel1.Controls.Add(t);
                if (currentTextX == leftTextX)
                    currentTextX = rightTextX;
                else
                {
                    currentTextX = leftTextX;
                    currentTextY += (textBoxHeight + Constants.WIDGET_SPACING);
                }

            }
            samplingLabel.Text = "SampRate";
            samplingLabel.Name = samplingLabel.Text;
            samplingLabel.Font = textFont;
            samplingLabel.Size = new System.Drawing.Size(textBoxWidth, textBoxHeight);
            samplingLabel.Location = new System.Drawing.Point(currentTextX, currentTextY);
            this.sensorLabels.Add("SampRate",samplingLabel);
            //this.tabPage1.Controls.Add(samplingLabel);
            this.panel1.Controls.Add(samplingLabel);

            //Initialize Buttons

            this.categoryButtons = new ArrayList();
            this.buttonIndex = new ArrayList();
            int button_width = this.panel2.ClientSize.Width - Constants.SCREEN_LEFT_MARGIN - Constants.SCREEN_RIGHT_MARGIN;
            int button_height = (this.panel2.ClientSize.Height - Constants.SCREEN_TOP_MARGIN- Constants.SCREEN_BOTTOM_MARGIN - (this.annotation.Categories.Count * Constants.WIDGET_SPACING)) / (this.annotation.Categories.Count + 1);
            int button_x = Constants.SCREEN_LEFT_MARGIN;
            int button_y = Constants.SCREEN_TOP_MARGIN*2;
            int delta_y = button_height + Constants.WIDGET_SPACING;
            int button_id = 0;


            foreach (Category category in this.annotation.Categories)
            {
                System.Windows.Forms.Button button = new System.Windows.Forms.Button();
               
                button.Location = new System.Drawing.Point(button_x, button_y + button_id * delta_y);
                button.Name = button_id.ToString();
                button.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular);
                button.Size = new System.Drawing.Size(button_width, button_height);
                //button.TabIndex = button_id;
                button.Text = ((AXML.Label)category.Labels[0]).Name;
                //button.UseVisualStyleBackColor = true;
                button.Click += new System.EventHandler(this.button_Click);
                this.categoryButtons.Add(button);
                this.panel2.Controls.Add(button);

                //check the longest label for this button
                foreach (AXML.Label label in category.Labels)
                {
                    string newlabel = label.Name;

                    if (newlabel.Length > longest_label.Length)
                        longest_label = newlabel;
                }
                this.buttonIndex.Add(0);
                button_id++;
            }

            if (longest_label.Length < 5)
                longest_label = "RESET";

            Size oldSize=this.Size;
            this.Size=new Size(button_width,button_height);            
            Font buttonFont=GUI.CalculateBestFitFont(this.CreateGraphics(), Constants.MIN_FONT, Constants.MAX_FONT,
                this.ClientSize, longest_label, new Font(Constants.FONT_FAMILY,(float)32.0,FontStyle.Bold), (float)0.9, (float)0.9);            
            foreach (System.Windows.Forms.Button button in categoryButtons)
                button.Font = buttonFont;            
            //adjust round buttons start/stop -reset
            button_width = (this.panel2.Size.Width - Constants.SCREEN_LEFT_MARGIN - Constants.SCREEN_RIGHT_MARGIN-Constants.WIDGET_SPACING) / 2;
            this.resetButton.Font = this.startStopButton.Font = buttonFont;//this.startStopButton.Font = GUI.CalculateBestFitFont(this.CreateGraphics(), Constants.MIN_FONT, Constants.MAX_FONT,
                //this.ClientSize, "RESET", new Font(Constants.FONT_FAMILY, (float)32.0, FontStyle.Bold), (float)0.90, (float)0.90);
            this.Size = oldSize;

            this.startStopButton.Size = new System.Drawing.Size(button_width, button_height);
            this.resetButton.Size = new System.Drawing.Size(button_width, button_height);
            this.startStopButton.Location = new System.Drawing.Point(Constants.SCREEN_LEFT_MARGIN, button_y + button_id * delta_y);
            this.resetButton.Location = new System.Drawing.Point(this.startStopButton.Location.X + this.startStopButton.Size.Width + Constants.WIDGET_SPACING, button_y + button_id * delta_y);



            //Menu Tab 2
            this.mainMenuTab2 = new System.Windows.Forms.MainMenu();
            this.menuItem1Tab2 = new System.Windows.Forms.MenuItem();
            this.menuItem2Tab2 = new System.Windows.Forms.MenuItem();
            this.menuItem3Tab2 = new System.Windows.Forms.MenuItem();
            this.menuItem4Tab2 = new System.Windows.Forms.MenuItem();
            this.menuItem5Tab2 = new System.Windows.Forms.MenuItem();
            this.menuItem6Tab2 = new System.Windows.Forms.MenuItem();
            this.menuItem7Tab2 = new System.Windows.Forms.MenuItem();
            this.menuItem8Tab2 = new System.Windows.Forms.MenuItem();

            
            this.menuItem1Tab2.Text = "Quit";
            this.menuItem1Tab2.Click += new System.EventHandler(this.menuItem1_Click);
            this.menuItem2Tab2.Text = "Options";

            this.mainMenuTab2.MenuItems.Add(this.menuItem1Tab2);
            this.mainMenuTab2.MenuItems.Add(this.menuItem2Tab2);
            this.menuItem3Tab2.Text = "Session";
            this.menuItem4Tab2.Text = "Training";

            this.menuItem2Tab2.MenuItems.Add(this.menuItem3Tab2);
            this.menuItem2Tab2.MenuItems.Add(this.menuItem4Tab2);


            this.menuItem5Tab2.Text = "Start";
            this.menuItem6Tab2.Text = "End";
            this.menuItem3Tab2.MenuItems.Add(this.menuItem5Tab2);
            this.menuItem3Tab2.MenuItems.Add(this.menuItem6Tab2);
            this.menuItem5Tab2.Click += new EventHandler(menuItem5Tab2_Click);
            this.menuItem6Tab2.Click += new EventHandler(menuItem6Tab2_Click);


            this.menuItem7Tab2.Text = "Auto";
            this.menuItem8Tab2.Text = "Manual";
            this.menuItem4Tab2.MenuItems.Add(this.menuItem7Tab2);
            this.menuItem4Tab2.MenuItems.Add(this.menuItem8Tab2);
            this.menuItem7Tab2.Click += new EventHandler(menuItem7Tab2_Click);
            this.menuItem8Tab2.Click += new EventHandler(menuItem8Tab2_Click);

#if (PocketPC)
            this.tabControl1.SelectedIndexChanged += new EventHandler(tabControl1_Changed);
#endif
           
            //if there is more than one category, manual training is the only option
            if (this.annotation.Categories.Count > 1)
            {
                this.menuItem7Tab2.Enabled = false;
                this.menuItem8Tab2.Enabled = false;
                this.menuItem8Tab2.Checked = true;
            }
            this.menuItem6Tab2.Enabled = false;
            this.menuItem8Tab2.Checked = true;
            this.startStopButton.Enabled = true;
            this.resetButton.Enabled = true;
            //this.label5.Text = Constants.MANUAL_MODE_SESSION;

#if (PocketPC)
            this.ClientSize= new Size(this.tabControl1.Width, this.tabControl1.Height);
#else
            this.form1.ClientSize = new Size(this.panel1.Width, this.panel1.Height);
            this.form2.ClientSize = new Size(this.panel2.Width, this.panel2.Height);
            this.form3.ClientSize = new Size(this.panel3.Width, this.panel3.Height);
            this.form4.ClientSize = new Size(this.panel4.Width, this.panel4.Height);
#endif

#if (PocketPC)
            this.Resize += new EventHandler(OnResize);
#else
            this.form1.Resize+=new EventHandler(OnResizeForm1);
            this.form1.FormClosing += new FormClosingEventHandler(form_FormClosing);
            this.form2.Resize += new EventHandler(OnResizeForm2);
            this.form2.FormClosing += new FormClosingEventHandler(form_FormClosing);
            this.form3.Resize += new EventHandler(OnResizeForm3);
            this.form3.FormClosing += new FormClosingEventHandler(form_FormClosing);
            this.form4.Resize += new EventHandler(OnResizeForm4);
            this.form4.FormClosing += new FormClosingEventHandler(form_FormClosing);
#endif
            #endregion Calculation of Widgets locations and sizes
        }

        void form_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isQuitting ==false)
                e.Cancel = true;
        }



  

        void menuItem6Tab2_Click(object sender, EventArgs e)
        {
            EndTraining();
            this.overallTimer.reset();
            this.goodTimer.reset();
        }


        
        //starting a session
        void menuItem5Tab2_Click(object sender, EventArgs e)
        {

            this.arffFileName = this.dataDirectory + "\\output" + DateTime.Now.ToString().Replace('/', '_').Replace(':', '_').Replace(' ', '_') + ".arff";
            tw = new StreamWriter(arffFileName);
            if (AutoTraining == true)
            {
                int i=0;
                tw.WriteLine("@RELATION wockets");
                tw.WriteLine(Extractor.GetArffHeader());
                tw.Write("@ATTRIBUTE activity {");
                for (i=0; (i<((AXML.Category)this.annotation.Categories[0]).Labels.Count - 1);i++)                                 
                    tw.Write( ((AXML.Label)((AXML.Category)this.annotation.Categories[0]).Labels[i]).Name.Replace(' ', '_') +",");
                tw.WriteLine(((AXML.Label)((AXML.Category)this.annotation.Categories[0]).Labels[i]).Name.Replace(' ', '_') + "}");            
                tw.WriteLine("\n@DATA\n\n");
            }

            this.menuItem5Tab2.Checked = true;
            this.menuItem5Tab2.Enabled = false;
           
            if (this.menuItem8Tab2.Checked==true) // manual, you can end it
                this.menuItem6Tab2.Enabled = true;

            //cannot change the traing mode if a session started
            this.menuItem8Tab2.Enabled = false;
            this.menuItem7Tab2.Enabled = false;

            //enable stop/start and reset buttons
            this.startStopButton.Enabled = true;
            this.resetButton.Enabled = true;
            if (AutoTraining==true)
            {
                this.autoTrainingIndex = 0;
                this.startActivityTime = Environment.TickCount + Extractor.Configuration.TrainingWaitTime*1000;
            }

            this.overallTimer.start();
            
        }

#if (PocketPC)
#else
        public void ShowForms()
        {
            this.form1.Show();
            this.form2.Show();
            this.form3.Show();
            this.form4.Show();
            this.form1.DesktopLocation = this.form1.Location = new Point(Constants.SCREEN_LEFT_MARGIN, Constants.SCREEN_TOP_MARGIN);
            this.form3.DesktopLocation = this.form3.Location = new Point(Constants.SCREEN_LEFT_MARGIN , Constants.SCREEN_TOP_MARGIN + Constants.SCREEN_TOP_MARGIN + this.form1.Height);

            this.form2.DesktopLocation = this.form2.Location = new Point(Constants.SCREEN_LEFT_MARGIN + Constants.SCREEN_LEFT_MARGIN + this.form1.Width, Constants.SCREEN_TOP_MARGIN);            
            this.form4.DesktopLocation = this.form4.Location = new Point(Constants.SCREEN_LEFT_MARGIN + Constants.SCREEN_LEFT_MARGIN + this.form1.Width, Constants.SCREEN_TOP_MARGIN + Constants.SCREEN_TOP_MARGIN + this.form2.Height);
           
            
        }
#endif
        void menuItem8Tab2_Click(object sender, EventArgs e)
        {
            if (this.annotation.Categories.Count == 1)
            {
                this.menuItem8Tab2.Checked = true;
                this.menuItem8Tab2.Enabled = false;
                this.menuItem7Tab2.Checked = false;
                this.menuItem7Tab2.Enabled = true;
               // this.label5.Text = Constants.MANUAL_MODE_SESSION;
            }
        }

        void menuItem7Tab2_Click(object sender, EventArgs e)
        {
            if (this.annotation.Categories.Count == 1)
            {
                this.menuItem7Tab2.Checked = true;
                this.menuItem7Tab2.Enabled = false;
                this.menuItem8Tab2.Checked = false;
                this.menuItem8Tab2.Enabled = true;
                //this.label5.Text = Constants.AUTO_MODE_SESSION;
            }
        }

#if (PocketPC)
        void tabControl1_Changed(object sender, EventArgs e)
        {
            if (this.tabControl1.SelectedIndex == 1)
                this.Menu = this.mainMenuTab2;
            else if (this.tabControl1.SelectedIndex == 0)
                this.Menu = this.mainMenu1;
        }
#endif
        public void EndTraining()
        {
            tw.Close();
            this.menuItem6Tab2.Enabled = false;
            this.menuItem5Tab2.Checked = false;
            this.menuItem5Tab2.Enabled = true;

            if (this.annotation.Categories.Count == 1) //if 1 category 
            {
                //enable whatever was not chosen to allow the user to switch the training mode
                if (this.menuItem8Tab2.Checked)
                    this.menuItem7Tab2.Enabled = true;
                else
                    this.menuItem8Tab2.Enabled = true;
            }

            //disable stop/start and reset buttons
            this.startStopButton.Enabled = false;
            this.resetButton.Enabled = false;     
        }

        public bool AutoTraining
        {
            get
            {
                return this.menuItem7Tab2.Checked;
            }            
        }

        public bool IsTraining
        {
            get
            {
                return this.menuItem5Tab2.Checked;
            }
        }


        public void InitializeTimers()
        {
            this.goodTimer = new ATimer(this, GOOD_TIMER);
            this.overallTimer = new ATimer(this, OVERALL_TIMER);

        }      
        private void oxycon_Click(object sender, EventArgs e)
        {
            DateTime now= DateTime.Now;
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff = now.Subtract(origin);                    
            string timestamp = diff.TotalMilliseconds + "," + now.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ssK");
            TextWriter tw = new StreamWriter(this.dataDirectory + "\\OxyconSyncronizationTime.txt");
            tw.WriteLine(timestamp);
            tw.Close();
            this.oxyconButton.Enabled = false;
        }

        private void button_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            int button_id = Convert.ToInt32(button.Name);
            Category category = (Category)this.annotation.Categories[button_id];
            int nextIndex = ((int)this.buttonIndex[button_id] + 1) % category.Labels.Count;
            //this.clickSound.Play();
            button.Text =((AXML.Label)category.Labels[nextIndex]).Name;
            //((AXML.Label)category.Labels[nextIndex]).PlayTone();
            this.buttonIndex[button_id] = nextIndex;
        }
        #endregion Annotator

        #region Main form setup
        private void SetFormPositions()
        {

            xDim = this.panel1.ClientSize.Width;
            yDim = (int) (this.panel1.ClientSize.Height* 0.60) ;

            if (aMITesPlotter != null)
                aMITesPlotter.SetupScaleFactor(GetGraphSize(false), maxPlots);
        }

        private void TurnOffTextDisplays()
        {
           // textBoxRate.Visible = false;
           // textBoxAC1.Visible = false;
           // textBoxAC2.Visible = false;
           // textBoxAC3.Visible = false;
           // textBoxAC4.Visible = false;
           //textBoxAC5.Visible = false;
           // textBoxAC6.Visible = false;
           // textBoxHR.Visible = false;

        }

        private void TurnOnTextDisplays()
        {
            //textBoxRate.Visible = true;
            //textBoxAC1.Visible = true;
            //textBoxAC2.Visible = true;
            //textBoxAC3.Visible = true;
            //textBoxAC4.Visible = true;
            //textBoxAC5.Visible = true;
            //textBoxAC6.Visible = true;
            //textBoxHR.Visible = true;    
        }

        private void SetPlotterFullScreen()
        {
            aMITesPlotter = new MITesScalablePlotter(this.panel1, MITesScalablePlotter.DeviceTypes.IPAQ, maxPlots, this.mitesDecoders[0], GetGraphSize(true));
        }

        private void SetPlotterPartialScreen()
        {
            aMITesPlotter = new MITesScalablePlotter(this.panel1, MITesScalablePlotter.DeviceTypes.IPAQ, maxPlots,  this.mitesDecoders[0], GetGraphSize(false));
        }


        #endregion


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
#if (PocketPC)
            if (this.tabControl1.TabIndex == 0)
            {
#endif
            if ((backBuffer == null) || (isResized))
                {
                    backBuffer = new Bitmap(xDim, yDim);
                    isResized = false;
                    isNeedRedraw = true;

                    using (Graphics g = Graphics.FromImage(backBuffer))
                    {
                        g.FillRectangle(redBrush, 0, 0, xDim, yDim);
                    }
                    e.Graphics.DrawImage(backBuffer, 0, 0);
                    this.panel1.Refresh();
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
                            g.FillRectangle(aBrush, 0, 0, xDim, yDim);

                            Size graphArea = GetGraphSize(isPlottingFullScreen);

                            g.FillRectangle(blueBrush, gapDistance, gapDistance, graphArea.Width - gapDistance, graphArea.Height);

                            aMITesPlotter.SetIsFirstTime(false);
                            isNeedRedraw = false;
                        }
                        aMITesPlotter.DrawValsFast(g);
                    }
                }
                //e.Graphics.DrawImage(backBuffer,0,0,backBuffer.Width,backBuffer.Height);

                e.Graphics.DrawImage(backBuffer, 0, 0);

#if (PocketPC)
            }
#endif
        }



        private Size GetGraphSize(bool isFullScreen)
        {
            int xsize = xDim - gapDistance;
            int ysize = yDim;
            //if (!isFullScreen) // Leave space for buttons 
           // {
            //    xsize = xDim - NUM_TEXT_BOX_COLS * gapDistance + gapDistance;
             //   ysize = yDim - NUM_TEXT_BOX_ROWS * (textBoxAC1.Height + gapDistance) - NUM_TEXT_BOX_ROWS * gapDistance;
          // }

            return new Size(xsize, ysize);
        }
        #endregion

        //private MITesDataCollectionForm thisForm = null;

        private bool InitializeMITes(string dataDirectory)
        {

           // thisForm = this;
            SetFormPositions();
            
            //depending on the number of receivers initialize mites objects
            int maxPortSearched = 1;
            for (int i = 0; (i < this.sensors.TotalReceivers); i++)
            {
                progressMessage += "Searching for receiver " +i +"...\r\n";
                this.mitesControllers[i] = new MITesReceiverController(MITesReceiverController.FIND_PORT, BYTES_BUFFER_SIZE);
                int portNumber = MITesReceiverController.FIND_PORT;

//#if (PocketPC)


                try
                {
                    for (int j = maxPortSearched; (j < Constants.MAX_PORT); j++)
                    {
                        portNumber = maxPortSearched = j;                        
                        progressMessage += "Testing COM Port " + portNumber;
                        if (this.mitesControllers[i].TestPort(portNumber, BYTES_BUFFER_SIZE))
                        {
                            progressMessage += "... Success\r\n";
                            break;
                        }
                        else
                            progressMessage += "... Failed\r\n";
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Exiting: Could not find a valid COM port with a MITes receiver!");
#if (PocketPC)
                    Application.Exit();
#else
                    Environment.Exit(0);
#endif
                }

//#else
                //string[] portNames = SerialPort.GetPortNames();
                //Regex comregex = new Regex("COM([0-9]+)");
                //for (int j = 0; (j < portNames.Length); j++)
                //{
                //    Match m = comregex.Match(portNames[j]);
                //    if (m.Success)
                //    {
                //        portNumber = Convert.ToInt32(m.Groups[1].Value);
                //        progressMessage += "Testing COM Port " + portNumber;
                //        if (this.mitesControllers[i].TestPort(portNumber, BYTES_BUFFER_SIZE))
                //        {
                //            progressMessage += "... Success\r\n";
                //            break;
                //        }
                //        else
                //        {
                //            progressMessage += "... Failed\r\n";
                //            portNumber = MITesReceiverController.FIND_PORT;
                //        }

                //    }
                //}
//#endif


                if (portNumber == MITesReceiverController.FIND_PORT)
                {
                    progressMessage += "Could not find a valid COM port with a MITes receiver!";
                    MessageBox.Show("Exiting: Could not find a valid COM port with a MITes receiver!");
#if (PocketPC)
                    Application.Exit();
#else
                    Environment.Exit(0);
#endif
                    return false;
                }
                this.mitesControllers[i].InitializeController(portNumber, BYTES_BUFFER_SIZE, true, MITesReceiverController.USE_THREADS);
                this.mitesDecoders[i] = new MITesDecoder();
            }

            aMITesActivityLogger = new MITesActivityLogger(dataDirectory + "\\data\\activity\\MITesActivityData");
            aMITesActivityLogger.SetupDirectories(dataDirectory);

            //aMITesDecoder = new MITesDecoder();
            aMITesPlotter = new MITesScalablePlotter(this.panel1, MITesScalablePlotter.DeviceTypes.IPAQ, maxPlots,  this.mitesDecoders[0], GetGraphSize(false));

            //for each sensor created a counter
            for (int i = 0; (i < this.sensors.Sensors.Count); i++)
            {
                int sensor_id=Convert.ToInt32(((SXML.Sensor)this.sensors.Sensors[i]).ID);
                if (sensor_id!=0)
                    aMITesActivityCounters.Add(sensor_id,new MITesActivityCounter(this.mitesDecoders[0],sensor_id));
            }
            aMITesHRAnalyzer = new MITesHRAnalyzer(this.mitesDecoders[0]);
            aMITesDataFilterer = new MITesDataFilterer(this.mitesDecoders[0]);
            aMITesLogger = new MITesLoggerNew(this.mitesDecoders[0],
                dataDirectory + "\\data\\raw\\MITesAccelBytes",
                dataDirectory + "\\data\\log\\MITesLogFileBytes");
            //aMITesLogger.SetActive(false);


            aMITesActivityLogger.WriteLogComment("Application started with command line: " +
                 dataDirectory + " " +
                 Constants.ACCEL_ID1 + " " +
                 Constants.ACCEL_ID2 + " " +
                 Constants.ACCEL_ID3 + " " );//+
                //Constants.ACCEL_ID4 + " " +
                //Constants.ACCEL_ID5 + " " +
                //Constants.ACCEL_ID6 + " ");

           

 
            // Set the correct channels based on sannotation automatically
            for (int i = 0; (i < this.sensors.TotalReceivers); i++)
            {
                int[] channels = new int[6];
                int channelCounter = 0;
                for (int j = 0; (j < this.sensors.Sensors.Count); j++)
                {
                    if (Convert.ToInt32(((Sensor)this.sensors.Sensors[j]).Receiver) == i)
                    {
                        channels[channelCounter] = Convert.ToInt32(((Sensor)this.sensors.Sensors[j]).ID);
                        channelCounter++;
                    }
                }
                this.mitesControllers[i].SetChannels(this.sensors.GetNumberSensors(i), channels);
            }
            //isPlotting = true;
            return true;
        }



        #region Heart rate MITes functions
        /// <summary>
        /// Report the HR if getting received
        /// </summary>
        public void ReportHR()
        {
            string key = "MITes0";
           

            if (this.sensorLabels[key]!=null)
            {
                int hr = aMITesHRAnalyzer.GetLastHR();
                double meanHR = aMITesHRAnalyzer.GetLastMean();
                int lastHRTime = aMITesHRAnalyzer.GetLastTime();
                if ((Environment.TickCount - lastHRTime) > 15000)
                    ((System.Windows.Forms.Label)this.sensorLabels[key]).Text = "No HR Data 15 sec";
                else if ((Environment.TickCount - lastHRTime) > 2000)
                    ((System.Windows.Forms.Label)this.sensorLabels[key]).Text = "No HR Data";
                else if ((hr != 0) && (hr != MITesData.NONE))
                    ((System.Windows.Forms.Label)this.sensorLabels[key]).Text = "HR Avg: " + Math.Round(meanHR, 1);
            }
        }
        #endregion



        /// <summary>
        /// Report counts for up to three accelerometers, only called when Epoch has new data
        /// </summary>
        public void ReportActivityCounts()
        {
           
           
            foreach (Sensor sensor in this.sensors.Sensors)
            {
                int sensor_id = Convert.ToInt32(sensor.ID);                
                if (sensor_id > 0)
                {
                    double result = ((MITesActivityCounter)this.aMITesActivityCounters[sensor_id]).GetLastEpochValueAll();
                    string key = "MITes" + sensor.ID;
                    if (result == 0)
                        ((System.Windows.Forms.Label)this.sensorLabels[key]).Text = "AC " + sensor_id + ": none";
                    else
                    {
                        ((System.Windows.Forms.Label)this.sensorLabels[key]).Text = "AC " + sensor_id + ": " + Math.Round(result, 2);
                        
                        if (result<3.0)
                            ((System.Windows.Forms.Label)this.sensorLabels[key]).Text = "AC " + sensor_id + ": still";
                    }

                }
                                  
            }

        }


        private void menuItem5_Click(object sender, EventArgs e)
        {

        }
        int startActivityTime;
        private static string[] lastLabels = new string[10];
        private static int lastLabelIndex = 0;

        [DllImport("coredll.dll")]
        public static extern int PlaySound(
            string szSound,
            IntPtr hModule,
            int flags);


        void HRTimer_Tick(object sender, System.EventArgs e)
        {
            if (MITesDataFilterer.MITesPerformanceTracker[0].SampleCounter >= MITesDataFilterer.MITesPerformanceTracker[0].GoodRate)
            {
                this.label4.Text = "Good HR";               
                this.label4.ForeColor = System.Drawing.Color.Green;
            }
            else
            { //not enough HR samples         
                this.label4.Text = "HR";
                this.label4.ForeColor = System.Drawing.Color.Red;
            }
            MITesDataFilterer.MITesPerformanceTracker[0].SampleCounter = 0;
        }
        void qualityTimer_Tick(object sender, System.EventArgs e)
        {
            bool overallGoodQuality = true;
            double goodRate=( 1- Extractor.Configuration.MaximumNonconsecutiveFrameLoss)*100;

            foreach (SXML.Sensor sensor in this.sensors.Sensors)
            {
                int sensor_id = Convert.ToInt32(sensor.ID);
                if (sensor_id > 0) // don't include HR
                {
                    double rate = ((double)MITesDataFilterer.MITesPerformanceTracker[sensor_id].SampleCounter / (double)MITesDataFilterer.MITesPerformanceTracker[sensor_id].GoodRate) * 100;
                    if (rate > 100)
                        rate = 100;
                    else if (rate < 0)
                        rate = 0;
                    this.expectedLabels[sensor_id].Text = rate.ToString("00.00") + "%";
                    this.samplesPerSecond[sensor_id].Text = MITesDataFilterer.MITesPerformanceTracker[sensor_id].SampleCounter.ToString() + "/" + MITesDataFilterer.MITesPerformanceTracker[sensor_id].PerfectRate.ToString();
                    
                    if (rate < goodRate)
                    {
                        this.expectedLabels[sensor_id].ForeColor = Color.Red;
                        this.samplesPerSecond[sensor_id].ForeColor = Color.Red;
                        this.labels[sensor_id].ForeColor = Color.Red;
                    }
                    else
                    {
                        this.expectedLabels[sensor_id].ForeColor = Color.Black;
                        this.samplesPerSecond[sensor_id].ForeColor = Color.Black;
                        this.labels[sensor_id].ForeColor = Color.Black;
                    }

                    MITesDataFilterer.MITesPerformanceTracker[sensor_id].LastSamplingRate = rate;
                    MITesDataFilterer.MITesPerformanceTracker[sensor_id].SampleCounter = 0;

                    if (rate < MITesDataFilterer.MITesPerformanceTracker[sensor_id].GoodRate)
                        overallGoodQuality = false;
                }
            }

            //stop the good timer if the overall timer is running (i.e. something is being annotated), the overall quality is bad
            if ( (this.overallTimer.isRunning()) && (overallGoodQuality == false) && (this.goodTimer.isRunning()))
                this.goodTimer.stop();
            //start the good timer if the overall timer is running (i.e. something is being annotated), the overall quality is good
            else if ((this.overallTimer.isRunning()) && (overallGoodQuality == true) && (this.goodTimer.isRunning() == false))
                this.goodTimer.start();
        }

        private int sumHR=0;
        private int hrCount = 0;

        private void readDataTimer_Tick(object sender, EventArgs e)
        {
            if (okTimer > 3000)
                okTimer = -1;
            okTimer++;

#if (PocketPC)
#else
            if (this.Visible)
                this.Visible = false;
#endif
            if (isStartedReceiver)
            {
                count++;
                if ((Environment.TickCount - time) >= 1000)
                {
                    ave = sum / (double)count;

                    //textBoxDist.Text = "R: " + count + " T: " + sum;
                    aMITesLogger.WriteLogComment("R: " + count + " T: " + sum);
                    sum = 0;
                    count = 0;
                    time = Environment.TickCount;
                }


                //aMITesDecoder.GetSensorData(this.mitesControllers[0]);
                for (int i = 0; (i < this.sensors.TotalReceivers); i++)
                    this.mitesDecoders[i].GetSensorData(this.mitesControllers[i]);                

                for (int i = 1; (i < this.sensors.TotalReceivers); i++)
                    this.mitesDecoders[0].MergeDataOrderProperly(this.mitesDecoders[i]);


                //A training session has started
                if (IsTraining==true)
                {
                    if (AutoTraining == true)
                    {
                        string current_activity = ((AXML.Label)((AXML.Category)this.annotation.Categories[0]).Labels[autoTrainingIndex]).Name;
                        if (Extractor.IsTrained(current_activity))
                        {

                            this.startActivityTime = Environment.TickCount + Extractor.Configuration.TrainingWaitTime*1000;//Constants.TRAINING_GAP;
                            autoTrainingIndex++;

                           //check if auto training completed
                            if (autoTrainingIndex == ((AXML.Category)this.annotation.Categories[0]).Labels.Count)
                            {
                                //this.label5.Text = "TR completed";
                                //this.label11.Text = "Training Completed";
                                EndTraining();
                                this.goodTimer.reset();
                                this.overallTimer.reset();
                            }
                            else
                            {
                                ((Button)this.categoryButtons[0]).Text = ((AXML.Label)((AXML.Category)this.annotation.Categories[0]).Labels[autoTrainingIndex]).Name;
                                this.goodTimer.reset();
                                PlaySound(@"\Windows\Voicbeep", IntPtr.Zero, (int)(PlaySoundFlags.SND_FILENAME | PlaySoundFlags.SND_SYNC));
                                PlaySound(@"\Windows\Voicbeep", IntPtr.Zero, (int)(PlaySoundFlags.SND_FILENAME | PlaySoundFlags.SND_SYNC));
                            }
                        }
                        else if (this.startActivityTime < Environment.TickCount) // TRAINING_GAP passed 
                        {
                           // this.label5.Text = "TR in progress";
                            //this.label11.Text = "Training " + current_activity;
                            this.goodTimer.start();
                            double lastTimeStamp = Extractor.StoreMITesWindow();
                            if (Extractor.GenerateFeatureVector(lastTimeStamp))
                            {
                                Extractor.TrainingTime[current_activity] = (int)Extractor.TrainingTime[current_activity] + 200;//Extractor.Configuration.OverlapTime;// get it from configuration
                                string arffSample = Extractor.toString() + "," + current_activity.Replace(' ', '_');
                                this.tw.WriteLine(arffSample);
                                this.label8.Text = Extractor.DiscardedLossRateWindows.ToString();
                                //this.label10.Text = Extractor.DiscardedConsecutiveLossWindows.ToString();
                            }
                        }
                        else
                        {
                           // this.label5.Text = "TR in " + ((int)(this.startActivityTime - Environment.TickCount) / 1000) + " secs";
                           // this.label11.Text = "Training " + current_activity + " in " + ((int)(this.startActivityTime - Environment.TickCount) / 1000) + " secs";
                        }
                    }
                    else // Manual Training
                    {
                    }
                }




                //Classifying

                if (isClassifying==true)
                {
                    double lastTimeStamp = Extractor.StoreMITesWindow();
                    if (Extractor.GenerateFeatureVector(lastTimeStamp))
                    {
                        Instance newinstance = new Instance(instances.numAttributes());
                        newinstance.Dataset = instances;
                        for (int i = 0; (i < Extractor.Features.Length); i++)
                            newinstance.setValue(instances.attribute(i), Extractor.Features[i]);
                        double predicted = classifier.classifyInstance(newinstance);
                        string predicted_activity = newinstance.dataset().classAttribute().value_Renamed((int)predicted);
                       
                        int currentIndex=(int)labelIndex[predicted_activity];
                        labelCounters[currentIndex] = (int)labelCounters[currentIndex] + 1;                        
                        classificationCounter++;

                        if (classificationCounter == Extractor.Configuration.SmoothWindows)
                        {
                            classificationCounter = 0;
                            int mostCount = 0;
                            string mostActivity = "";
                            for (int j=0;(j<labelCounters.Length);j++)
                            {                                
                                if (labelCounters[j] > mostCount)
                                    mostActivity = activityLabels[j];
                                labelCounters[j] = 0;
                            }

                            this.label6.Text = mostActivity;
                            //this.label11.Text = "Fahd is "+mostActivity;
                        }
                    }
                }


                if (activityCountWindowSize > Extractor.Configuration.QualityWindowSize) //write a line to CSV and initialize
                {
                    DateTime now= DateTime.Now;
                    DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                    TimeSpan diff = now.Subtract(origin);                    
                    string timestamp = diff.TotalMilliseconds + "," + now.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ssK");
                    string master_csv_line = timestamp;
                    string hr_csv_line = timestamp;
                    //to restore the date
                    //DateTime restored = origin.AddMilliseconds(diff.TotalMilliseconds);
                    if (this.overallTimer.isRunning())
                    {
                        foreach (Button button in this.categoryButtons)
                            master_csv_line += "," + button.Text;
                    }
                    else
                    {
                        foreach (Button button in this.categoryButtons)
                            master_csv_line += ",";
                    }

                    foreach (Sensor sensor in this.sensors.Sensors)
                    {
                        string csv_line1 = timestamp;
                        string csv_line2 = timestamp;
                        string csv_line3 = timestamp;
                       

                        int sensor_id = Convert.ToInt32(sensor.ID);
                        if (sensor_id > 0) //No HR
                        {
                            if (acCounters[sensor_id] > 0)
                            {
                                csv_line2 += "," + MITesDataFilterer.MITesPerformanceTracker[sensor_id].LastSamplingRate;

                                csv_line1 += "," + ((double)(averageX[sensor_id] / (double)acCounters[sensor_id])).ToString("00.00") + ",";
                                csv_line1 += ((double)(averageY[sensor_id] / (double)acCounters[sensor_id])).ToString("00.00") + ",";
                                csv_line1 += ((double)(averageZ[sensor_id] / (double)acCounters[sensor_id])).ToString("00.00");

                                csv_line3 += "," + ((int)(averageRawX[sensor_id] / acCounters[sensor_id])) + ",";
                                csv_line3 += ((int)(averageRawY[sensor_id] / acCounters[sensor_id])) + ",";
                                csv_line3 += ((int)(averageRawZ[sensor_id] / acCounters[sensor_id]));

                     

                                master_csv_line += "," + MITesDataFilterer.MITesPerformanceTracker[sensor_id].LastSamplingRate;
                                master_csv_line += "," + ((int)(averageRawX[sensor_id] / acCounters[sensor_id])) + ",";
                                master_csv_line += ((int)(averageRawY[sensor_id] / acCounters[sensor_id])) + ",";
                                master_csv_line += ((int)(averageRawZ[sensor_id] / acCounters[sensor_id]));
                                master_csv_line += "," + ((double)(averageX[sensor_id] / (double)acCounters[sensor_id])).ToString("00.00") + ",";
                                master_csv_line += ((double)(averageY[sensor_id] / (double)acCounters[sensor_id])).ToString("00.00") + ",";
                                master_csv_line += ((double)(averageZ[sensor_id] / (double)acCounters[sensor_id])).ToString("00.00");


                            }
                            else
                            {
                                csv_line1 += ",,,,";
                                csv_line3 += ",,,,";
                                csv_line2 += ",0";
                                master_csv_line += ",0,,,,,,";
                            }

                            this.activityCountCSVs[sensor_id].WriteLine(csv_line1);
                            this.samplingCSVs[sensor_id].WriteLine(csv_line2);
                            this.averagedRaw[sensor_id].WriteLine(csv_line3);                                                      
                        }
                        
                        averageX[sensor_id] = 0;
                        averageY[sensor_id] = 0;
                        averageZ[sensor_id] = 0;
                        averageRawX[sensor_id] = 0;
                        averageRawY[sensor_id] = 0;
                        averageRawZ[sensor_id] = 0;
                        prevX[sensor_id]=0;
                        prevY[sensor_id]=0;
                        prevY[sensor_id]=0;
                        acCounters[sensor_id] = 0;
                    }


                    if (hrCount > 0)
                    {
                        this.hrCSV.WriteLine(hr_csv_line+"," + (int)(sumHR / hrCount));
                        this.masterCSV.WriteLine(master_csv_line + "," + (int)(sumHR / hrCount));
                    }
                    else
                    {
                        this.hrCSV.WriteLine(hr_csv_line+",");
                        this.masterCSV.WriteLine(master_csv_line + ",");
                    }

                    hrCount = 0;
                    sumHR = 0;
                    activityCountWindowSize = 0;
                }
                
                activityCountWindowSize += 10; //add 10 milliseconds
                
                //store sum of abs values of consecutive accelerometer readings
                for (int i = 0; (i < this.mitesDecoders[0].someMITesDataIndex); i++)
                {
                    if ((this.mitesDecoders[0].someMITesData[i].type != (int)MITesTypes.NOISE) &&
                          (this.mitesDecoders[0].someMITesData[i].type == (int)MITesTypes.ACCEL))
                    {
                        int channel = 0, x = 0, y = 0, z = 0;
                        channel = (int)this.mitesDecoders[0].someMITesData[i].channel;
                        x = (int)this.mitesDecoders[0].someMITesData[i].x;
                        y = (int)this.mitesDecoders[0].someMITesData[i].y;
                        z = (int)this.mitesDecoders[0].someMITesData[i].z;

                        if (channel <= this.sensors.MaximumSensorID) //if junk comes ignore it
                        {
                            if ((prevX[channel] > 0) && (prevY[channel] > 0) && (prevZ[channel] > 0) && (x>0) && (y>0) && (z>0))
                            {
                                averageX[channel]=averageX[channel]+Math.Abs(prevX[channel]-x);
                                averageRawX[channel] = averageRawX[channel] + x;
                                averageY[channel] = averageY[channel] + Math.Abs(prevY[channel] - y);
                                averageRawY[channel] = averageRawY[channel] + y;
                                averageZ[channel] = averageZ[channel] + Math.Abs(prevZ[channel] - z);
                                averageRawZ[channel] = averageRawZ[channel] + z;
                                acCounters[channel] = acCounters[channel] + 1;
                            }

                            prevX[channel] = x;
                            prevY[channel] = y;
                            prevZ[channel] = z;
                        }


                    }
                }
                

              // if (this.isCollectingDetailedData == true)
               //   aMITesLogger.SaveRawData();
                
                //Collecting Simple plotting data for demos
                // for each channel create a file and write to it

                //if (this.isCollectingSimpleData == true)
                //{
                //    for (int i = 0; i < aMITesDecoder.someMITesDataIndex; i++)
                //    {
                //        if ((aMITesDecoder.someMITesData[i].type != (int)MITesTypes.NOISE) &&
                //             (aMITesDecoder.someMITesData[i].type == (int)MITesTypes.ACCEL))
                //        {
                //            int channel = 0, x = 0, y = 0, z = 0;
                //            channel = (int)aMITesDecoder.someMITesData[i].channel;
                //            double unixtimestamp = aMITesDecoder.someMITesData[i].unixTimeStamp;       

                //            if (this.tws[channel] == null)
                //            {
                //                xs[channel] = 0;
                //                ys[channel] = 0;
                //                zs[channel] = 0;
                //                this.mitesLastTimeStamps[channel] = unixtimestamp;
                //                this.mitesSampleCounters[channel] = 0;
                //                this.tws[channel] = new StreamWriter(this.dataDirectory + "\\MITes_Channel" + channel + ".dat");
                //            }
                            
                //            xs[channel] = xs[channel]+ (int)aMITesDecoder.someMITesData[i].x;
                //            ys[channel] = ys[channel] +(int)aMITesDecoder.someMITesData[i].y;
                //            zs[channel] = zs[channel] + (int)aMITesDecoder.someMITesData[i].z;                                                
                //            this.mitesSampleCounters[channel]=this.mitesSampleCounters[channel]+1;
                //          //  if ((unixtimestamp-this.mitesLastTimeStamps[channel])>=1000)
                //            //{
                //                this.tws[channel].WriteLine(unixtimestamp + "," +
                //                    (int)(xs[channel] / this.mitesSampleCounters[channel]) + "," +
                //                    (int)(ys[channel] / this.mitesSampleCounters[channel]) + "," +
                //                    (int)(zs[channel] / this.mitesSampleCounters[channel]));

                //                xs[channel] = 0;
                //                ys[channel] = 0;
                //                zs[channel] = 0;
                //                this.mitesLastTimeStamps[channel]=unixtimestamp;
                //                System.DateTime dateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
                //                dateTime = dateTime.AddMilliseconds(unixtimestamp);

                //                this.mitesSampleCounters[channel]=0;
                //            //}
                                
                //        }
                //    }
                //}








                //aMITesLoggerReader.GetSensorData(10, true); // true indicates PLFormat 

                sum += this.mitesDecoders[0].GetLastByteNum();
                aMITesLogger.SaveRawData();
                if (flushTimer == 0)
                    aMITesLogger.FlushBytes();
                if (flushTimer > 6000)
                    flushTimer = -1;
                flushTimer++;

                aMITesDataFilterer.RemoveZeroNoise();
                this.mitesDecoders[0].UpdateSamplingRate(aMITesDataFilterer.CountNonNoise());

                if (printSamplingCount > 500)
                {
                    ((System.Windows.Forms.Label)this.sensorLabels["SampRate"]).Text = "Samp: " + this.mitesDecoders[0].GetSamplingRate();
                    //textBoxRate.Text = "Samp: " + aMITesDecoder.GetSamplingRate();
                    //aMITesLogger.WriteLogComment(textBoxUV.Text);
                    printSamplingCount = 0;
                }
                else
                    printSamplingCount++;



                // Check HR values
                int hr = aMITesHRAnalyzer.Update();
                if (hr > 0)
                {
                    sumHR += hr;
                    hrCount++;
                }
              
             
     
                //Compute/get Activity Counts
                for (int i = 0; (i < this.sensors.Sensors.Count); i++)
                {
                    int sensor_id = Convert.ToInt32(((SXML.Sensor)this.sensors.Sensors[i]).ID);
                    if (sensor_id > 0)
                        ((MITesActivityCounter)this.aMITesActivityCounters[sensor_id]).UpdateActivityCounts();
                    //else if (sensor_id == 0)
                    //    aMITesHRAnalyzer.Update();
                }

                for (int i = 0; (i < this.sensors.Sensors.Count); i++)
                {
                    int sensor_id = Convert.ToInt32(((SXML.Sensor)this.sensors.Sensors[i]).ID);
                    if (sensor_id > 0)
                        ((MITesActivityCounter)this.aMITesActivityCounters[sensor_id]).PrintMaxMin();

                }

                if (((MITesActivityCounter)this.aMITesActivityCounters[this.sensors.FirstAccelerometer]).IsNewEpoch(1000))
                {
                    aMITesHRAnalyzer.ComputeEpoch(30000);
                    //if (this.tabControl1.TabIndex == 0)
                      //  ReportHR();

                    for (int i = 0; (i < this.sensors.Sensors.Count); i++)
                    {
                        int sensor_id = Convert.ToInt32(((SXML.Sensor)this.sensors.Sensors[i]).ID);
                        if (sensor_id > 0)
                            ((MITesActivityCounter)this.aMITesActivityCounters[sensor_id]).ComputeEpoch();
                    }

#if (PocketPC)
                    if (this.tabControl1.TabIndex == 0)
                    {
#endif
                        ReportActivityCounts();
                        ReportHR();
                    
#if (PocketPC)
                    }
#endif

                    if (!isWrittenKey) // Write the key once at the top of the file
                    {
                        isWrittenKey = true;
                        aMITesActivityLogger.StartReportKeyLine();
                        for (int i = 0; (i < this.sensors.Sensors.Count); i++)
                        {
                            int sensor_id = Convert.ToInt32(((SXML.Sensor)this.sensors.Sensors[i]).ID);
                            if (sensor_id > 0)
                                aMITesActivityLogger.AddKeyLine(((MITesActivityCounter)this.aMITesActivityCounters[sensor_id]));
                        }

                        aMITesActivityLogger.AddKeyLine(aMITesHRAnalyzer);
                        aMITesActivityLogger.SaveReportKeyLine();
                    }
                    aMITesActivityLogger.StartReportLine();

                    for (int i = 0; (i < this.sensors.Sensors.Count); i++)
                    {
                        int sensor_id = Convert.ToInt32(((SXML.Sensor)this.sensors.Sensors[i]).ID);
                        if (sensor_id > 0)
                            aMITesActivityLogger.AddReportLine(((MITesActivityCounter)this.aMITesActivityCounters[sensor_id]));
                    }

                    aMITesActivityLogger.AddReportLine(aMITesHRAnalyzer);
                    aMITesActivityLogger.SaveReportLine();
                }

                // Graph accelerometer data 
                if (isPlotting)
                    GraphAccelerometerValues();

                stepsToWarning++;
            }
        }

        private bool isQuitting = false;
        private void menuItem1_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to Quit MITes Software?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                isQuitting = true;
                for (int i = 0; (i < this.sensors.TotalReceivers); i++)
                {
                    if (this.mitesControllers[i] != null)
                    {
                        Thread.Sleep(100);
                        this.mitesControllers[i].Close();
                        Thread.Sleep(1000);
                    }
                    //aMITesActivityLogger.WriteLogComment(aMsg);
                    Thread.Sleep(100);
                }

                foreach (Sensor sensor in this.sensors.Sensors)
                {
                    int sensor_id = Convert.ToInt32(sensor.ID);
                    if (sensor_id>0){
                        this.activityCountCSVs[sensor_id].Flush();
                        this.samplingCSVs[sensor_id].Flush();
                        this.averagedRaw[sensor_id].Flush();
                        this.activityCountCSVs[sensor_id].Close();
                        this.samplingCSVs[sensor_id].Close();
                        this.averagedRaw[sensor_id].Close();
                    }
                }

                this.masterCSV.Flush();
                this.hrCSV.Flush();
                this.masterCSV.Close();
                this.hrCSV.Close();

                //for (int i = 0; (i < MITesData.MAX_MITES_CHANNELS); i++)
                //    if (this.tws[i] != null)
                //    {
                //        this.tws[i].Flush();
                //        this.tws[i].Close();
                //    }
                Application.Exit();
            }
        }

        private void menuItem3_Click(object sender, EventArgs e)
        {
            aMITesActivityLogger.WriteLogComment("Receiver configure form opened.");
            rcf.Show();
            rcf.ReadChannels();
        }

        private void roundButton1_Click(object sender, EventArgs e)
        {

        }

        private void roundButton2_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void startStopButton_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            //button state is now start
            if (button.BackColor == System.Drawing.Color.Green)
            {
                // this.startSound.Play();
                //Generator generator = new Generator();
                //generator.InitializeSound(this.Handle.ToInt32());
                //generator.CreateBuffer();

                this.startStopButton.Text = "Stop";
                this.startStopButton.BackColor = System.Drawing.Color.Red;
                this.overallTimer.reset();
                this.overallTimer.start();
                this.goodTimer.reset();
                this.goodTimer.start();

                //store the current state of the categories
                this.currentRecord = new AnnotatedRecord();
                this.currentRecord.StartDate = DateTime.Now.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ssK");
                this.currentRecord.StartHour = DateTime.Now.Hour;
                this.currentRecord.StartMinute = DateTime.Now.Minute;
                this.currentRecord.StartSecond = DateTime.Now.Second;
                TimeSpan ts = (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0));
                this.currentRecord.StartUnix = ts.TotalSeconds;

                //check all buttons values, store them and disable them
                foreach (Button category_button in categoryButtons)
                {
                    int button_id = Convert.ToInt32(category_button.Name);
                    Category category = (Category)this.annotation.Categories[button_id];
                    string current_label = ((AXML.Label)category.Labels[(int)this.buttonIndex[button_id]]).Name;
                    this.currentRecord.Labels.Add(new AXML.Label(current_label, category.Name));
                    category_button.Enabled = false;
                }

            }

            else if (button.BackColor == System.Drawing.Color.Red)
            {
                // this.stopSound.Play();
                this.startStopButton.Text = "Start";
                this.startStopButton.BackColor = System.Drawing.Color.Green;
                this.overallTimer.reset();
                this.goodTimer.reset();

                //store the current state of the categories
                this.currentRecord.EndDate = DateTime.Now.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ssK");
                this.currentRecord.EndHour = DateTime.Now.Hour;
                this.currentRecord.EndMinute = DateTime.Now.Minute;
                this.currentRecord.EndSecond = DateTime.Now.Second;
                TimeSpan ts = (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0));
                this.currentRecord.EndUnix = ts.TotalSeconds;
                this.annotation.Data.Add(this.currentRecord);

                //each time an activity is stopped, rewrite the file on disk, need to backup file to avoid corruption
                this.annotation.ToXMLFile();
                this.annotation.ToCSVFile();

                foreach (Button category_button in categoryButtons)
                    category_button.Enabled = true;                

            }
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            //this.resetSound.Play();
            this.startStopButton.Text = "Start";
            this.startStopButton.BackColor = System.Drawing.Color.Green;
            //this.overallTimer.stop();
            this.overallTimer.reset();
            this.goodTimer.reset();

            foreach (Button category_button in categoryButtons)
            {
                int button_id = Convert.ToInt32(category_button.Name);
                Category category = (Category)this.annotation.Categories[button_id];
                this.buttonIndex[button_id] = 0;
                category_button.Text =  ((AXML.Label)category.Labels[0]).Name;
                category_button.Enabled = true;
            }
        }

        private void menuItem21_Click(object sender, EventArgs e)
        {
            if (this.isCollectingSimpleData == true)
            {
                this.isCollectingSimpleData = false;
                this.menuItem21.Checked = false;

            }
            else
            {
                this.isCollectingSimpleData = true;
                this.menuItem21.Checked = true;

            }
        }

        private void textBoxAC3_TextChanged(object sender, EventArgs e)
        {
        }

        private void menuItem22_Click(object sender, EventArgs e)
        {
            if (this.isCollectingDetailedData == true)
            {
                this.isCollectingDetailedData = false;
                this.menuItem22.Checked = false;
                aMITesLogger.SetActive(false);

            }
            else
            {
                this.isCollectingDetailedData = true;
                this.menuItem22.Checked = true;
                aMITesLogger.SetActive(true);

            }

        }

        private void menuItem11_Click(object sender, EventArgs e)
        {
            if (this.isPlotting == true)
            {
                this.isPlotting = false;
                this.menuItem11.Checked = false;
            }
            else
            {
                this.isPlotting = true;
                this.menuItem11.Checked = true;
            }
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }


 

    }
}