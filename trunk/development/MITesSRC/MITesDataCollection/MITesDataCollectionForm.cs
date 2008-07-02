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
using MITesFeatures.Utils.DTs;

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

        #region Definitions of MITes Interface and timer Variables
        
        private static readonly bool DEBUG = true;
        private bool isStartedReceiver = false;
        private double[,] returnVals = new double[3, 4];
        private const int BYTES_BUFFER_SIZE = 4096; //2048 
        private byte[] someBytes = new byte[BYTES_BUFFER_SIZE];
        private bool isResized = false;
        private bool isNeedRedraw = false;
        private int xDim = 240;
        private int yDim = 320;
        private int maxPlots = 3; // Changed from 6
        private bool isPlotting = true;
        private Bitmap backBuffer = null;        
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
        private string progressMessage;
        private ATimer goodTimer, overallTimer;
        private ArrayList categoryButtons;
        private ArrayList buttonIndex;
        private string longest_label = "";
        public const int GOOD_TIMER = 1;
        public const int OVERALL_TIMER = 2;

        delegate void SetTextCallback(string label, int control_id);
        delegate void SetSignalCallback(bool isGood);

        #endregion Definitions of MITes Interface Variables

        #region Definitions of annotation and configuration variables
        private Annotation annotation;
        private SensorAnnotation sensors;
        private AnnotatedRecord currentRecord;
        private string dataDirectory;

        #endregion Definitions of annotation and configuration variables

        #region Definitions of all key MITes related C# objects

        private Hashtable aMITesActivityCounters;
        private MITesScalablePlotter aMITesPlotter;
        private MITesHRAnalyzer aMITesHRAnalyzer;
        private MITesDataFilterer aMITesDataFilterer;
        private MITesLoggerNew aMITesLogger;
        private MITesActivityLogger aMITesActivityLogger;
        private ReceiverConfigureForm rcf = null;
        private MITesReceiverController[] mitesControllers;
        private MITesDecoder[] mitesDecoders;

        #endregion

        #region Definition of classifier variables
        private int classificationCounter, masterclassificationCounter;
        private bool isExtracting;
        private bool isClassifying;
        private bool isCalibrating = false;
        
        private Classifier classifier;
        private FastVector fvWekaAttributes;
        private Instances instances;


        private Classifier masterclassifier;
        private FastVector masterfvWekaAttributes;
        private Instances masterinstances;
        private string arffFileName;
        private int autoTrainingIndex;


        private int[] labelCounters;
        private string[] activityLabels;
        private Hashtable labelIndex;



        private int[] masterlabelCounters;
        private string[] masteractivityLabels;
        private Hashtable masterlabelIndex;
#if (PocketPC)
           HierarchicalClassifier hrClassifier;
#else

#endif

        #endregion Definition of classifier variables


#if (PocketPC)
#else
        #region Definition of objects that store values for CSV files

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
        private int sumHR = 0;
        private int hrCount = 0;
        #endregion Definition of objects that store values for CSV files
#endif

        #region Definition of calibration objects
        private int[] calX, calY, calZ;
        private int calSensor = -1;
        private int calSensorPosition;
        private int calCounter;
        private int currentCalibrationSensorIndex;
        private System.Drawing.Image horizontalMITes;
        private System.Drawing.Image verticalMITes;
        #endregion Definition of calibration objects

        private TextWriter tw;
        //for simple data collection
        private bool isCollectingSimpleData=false;
        private bool isCollectingDetailedData=false;
        private bool collectDataMode=false;
        private bool classifyDataMode=false;
              

        #region GUI objects and global variables
        private Pen aPen = new Pen(Color.Wheat);
        private SolidBrush aBrush = new SolidBrush(Color.White);
        private SolidBrush blueBrush = new SolidBrush(Color.LightBlue);
        private SolidBrush redBrush = new SolidBrush(Color.Red);
        private int gapDistance = 4;
        #endregion

        int startActivityTime;
        private static string[] lastLabels = new string[10];
        private static int lastLabelIndex = 0;

        [DllImport("coredll.dll")]
        public static extern int PlaySound(
            string szSound,
            IntPtr hModule,
            int flags);
        private bool isQuitting = false;

        private Hashtable sensorLabels;
        private System.Windows.Forms.Label[] labels;
        private System.Windows.Forms.Label[] expectedLabels;
        private System.Windows.Forms.Label[] samplesPerSecond;


        //This method is executed as a seperate thread to manage the progress
        //form
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
              
            }
        }


        #region Calibration Constructor
        //This constructor initializes the software for calibration of a list
        //of sensors
        public MITesDataCollectionForm(SensorAnnotation uncalibratedSensors,string dataDirectory){

            //Setup the initial state of the calibration variables
            this.sensors = uncalibratedSensors;            
            this.currentCalibrationSensorIndex = 0;
            this.calCounter = 0;

#if (PocketPC)
            this.horizontalMITes = (System.Drawing.Image)new System.Drawing.Bitmap(Constants.MITES_HORIZONTAL_96_96);
            this.verticalMITes = (System.Drawing.Image)new System.Drawing.Bitmap(Constants.MITES_VERTICAL_96_96);
#else
            this.horizontalMITes = (System.Drawing.Image)new System.Drawing.Bitmap(Constants.MITES_HORIZONTAL_288_288);
            this.verticalMITes = (System.Drawing.Image)new System.Drawing.Bitmap(Constants.MITES_VERTICAL_288_288);
#endif

            //create a dummy annotation
            this.annotation = new Annotation();            

            //make sure the software will not collect or extract features
            this.collectDataMode = false;
            this.isClassifying = false;
            this.isExtracting = false;
            this.isCollectingDetailedData = false;
            this.isCollectingSimpleData = false;

            //setup where the sensordata file will be stored
            this.dataDirectory = dataDirectory;

            //setup plotting parameters
            isPlotting = true;            
            this.maxPlots = this.sensors.Sensors.Count;

            //Spawn the progress thread
            progressMessage = null;
            Thread t = new Thread(new ThreadStart(ProgressThread));
            t.Start();


            //Intialize the interface of the forms
            InitializeComponent();         
            progressMessage += "Initializing Timers ...";
            InitializeTimers();
            progressMessage += " Completed\r\n";
            progressMessage += "Initializing GUI ...";
            InitializeInterface();
            progressMessage += " Completed\r\n";

            //Intialize the MITes Receivers, decoders and counters based
            //on the chosen sensors
            if ((this.sensors.TotalReceivers > 0) && (this.sensors.TotalReceivers <= Constants.MAX_CONTROLLERS))
            {
                this.mitesControllers = new MITesReceiverController[this.sensors.TotalReceivers];
                this.mitesDecoders = new MITesDecoder[this.sensors.TotalReceivers];
                this.aMITesActivityCounters = new Hashtable();
                progressMessage += "Initializing MITes ... searching " + this.sensors.TotalReceivers + " receivers\r\n";
                if (InitializeMITes(dataDirectory) == false)
                {
                    MessageBox.Show("Exiting: You picked a configuration with " + this.sensors.TotalReceivers + " receivers. Please make sure they are attached to the computer.");
#if (PocketPC)
                    Application.Exit();
#else
                    Environment.Exit(0);
#endif
                }
            }


            //Setup the resize event for each different form
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

            //Initialize the interface for reporting the quality of the MITes
            //transmission
            progressMessage += "Initializing MITes Quality GUI ...";
            InitializeQualityInterface();
            progressMessage += " Completed\r\n";

            //Intialize the MITes performance tracking objects
            for (int i = 0; i < MITesData.MAX_MITES_CHANNELS; i++)
                MITesDataFilterer.MITesPerformanceTracker[i] = new MITesPerformanceStats(0);
            foreach (Sensor sensor in this.sensors.Sensors)
            {
                int sensor_id = Convert.ToInt32(sensor.ID);
                int receiver_id = Convert.ToInt32(sensor.Receiver);
                if (sensor_id == 0) //HR sensor
                {
                    MITesDataFilterer.MITesPerformanceTracker[sensor_id].GoodRate = (int)(Constants.HR_SAMPLING_RATE * 0.8);
                    MITesDataFilterer.MITesPerformanceTracker[sensor_id].PerfectRate = Constants.HR_SAMPLING_RATE;
                }
                else
                {
                    int goodSamplingRate = 150;
                    MITesDataFilterer.MITesPerformanceTracker[sensor_id].GoodRate = goodSamplingRate;
                    MITesDataFilterer.MITesPerformanceTracker[sensor_id].PerfectRate = 180;
                }
            }

            //Initialize the UNIX timer to use QueryPerformanceCounter
            UnixTime.InitializeTime();
#if (PocketPC)
            this.tabControl1.SelectedIndex = 0;
#endif
    
            //Start the receiver threads
            bool startReceiverThreads = true;
            for (int i = 0; (i < this.sensors.TotalReceivers); i++)
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

            //terminate the progress thread before starting the receivers
            t.Abort();
            //remove unnecessary forms or pages
#if (PocketPC)
            this.tabControl1.TabPages.RemoveAt(1);
            this.tabControl1.TabPages.RemoveAt(2);
            //this.tabControl1.TabPages.RemoveAt(3);
#else
            this.ShowForms();
#endif


            //Only enable the read data time since we are just calibrating
            this.readDataTimer.Enabled = true;


        }

        #endregion Calibration Constructor

        #region Data collection constructor

        public MITesDataCollectionForm(string dataDirectory)
        {
            //intialize the mode of the software
            this.collectDataMode = true;
            this.isCollectingSimpleData = false;
#if (PocketPC)
            this.isCollectingDetailedData = false;
#else
            this.isCollectingDetailedData=true;
#endif
            this.isPlotting = true;
            this.isExtracting = false;

            //initialize the progress thread
            progressMessage = null;
            Thread t = new Thread(new ThreadStart(ProgressThread));
            t.Start();         
                 

            //initialize the interface components
            InitializeComponent();            

            //Initialize where the data will be stored and where the configuration
            //files exist
            this.dataDirectory = dataDirectory;

         
            //load the activity and sensor configuration files
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

            //calculate how many plots to be drawn
            if (this.sensors.IsHR)
                this.maxPlots = this.sensors.Sensors.Count - 1;
            else
                this.maxPlots = this.sensors.Sensors.Count;

            
            //Initialize the timers
            progressMessage+= "Initializing Timers ...";
            InitializeTimers();
            progressMessage += " Completed\r\n";

            //Initialize different GUI components
            progressMessage += "Initializing GUI ...";
            InitializeInterface();
            progressMessage += " Completed\r\n";

            //Initialize the MITes receivers
            if ((this.sensors.TotalReceivers > 0) && (this.sensors.TotalReceivers <=Constants.MAX_CONTROLLERS))
            {
                this.mitesControllers = new MITesReceiverController[this.sensors.TotalReceivers];               
                this.mitesDecoders = new MITesDecoder[this.sensors.TotalReceivers];
                this.aMITesActivityCounters = new Hashtable();
                
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


            //Override the resize event
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

            //Initialize the quality interface
            progressMessage += "Initializing MITes Quality GUI ...";
            InitializeQualityInterface();
            progressMessage += " Completed\r\n";


            //Initialize the feature extraction algorithm but by default do not
            //turn it on
            Extractor.Initialize( this.mitesDecoders[0], dataDirectory,this.annotation,this.sensors);


            //Initialize performance counters for each sensor
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


            //Initialize objects that will store intermediate values for
            //generating csv files
#if (PocketPC)
#else
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
#endif


            //Initialize the UNIX QueryPerformanceCounter
            UnixTime.InitializeTime(); 


            //Start the receiver threads
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


            //Terminate the progress thread
            t.Abort();

            //Initialize the interface
#if (PocketPC)
            this.tabControl1.SelectedIndex = 0;
#else
            this.ShowForms();
#endif

            
            //Enable the read data, quality reporting and heart rate reporting threads
            this.readDataTimer.Enabled = true;
            this.qualityTimer.Enabled = true;
            if (this.sensors.IsHR)
                this.HRTimer.Enabled = true;
        }

        #endregion Data collection constructor

        #region Classifier constructor
        public MITesDataCollectionForm(string dataDirectory,string arffFile,bool isHierarchical)
        {
            int i = 0, j = 0;

            //Initialize the software mode
            this.classifyDataMode = true;
            isExtracting = false;
            isCollectingSimpleData = false;
            isCollectingDetailedData = false;
            isPlotting = true;
            isClassifying = true;

            //Initialize the progress bar thread
            progressMessage = null;
            Thread t = new Thread(new ThreadStart(ProgressThread));
            t.Start();

            //Initialize the interface
            InitializeComponent();
            this.dataDirectory = dataDirectory;


            //read the sensor configuration file to determine the number of receivers
            //read the activity configuration file
            progressMessage = "Loading XML protocol and sensors ...";
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

                    progressMessage += " Completed\r\n";
                }
            }

            //calculate how many plots should be shown
            if (this.sensors.IsHR)
                this.maxPlots = this.sensors.Sensors.Count - 1;
            else
                this.maxPlots = this.sensors.Sensors.Count;


            //Initialize different interface components
            progressMessage += "Initializing GUI ...";
            InitializeInterface();
            progressMessage += " Completed\r\n";



            //Initialize the MITes receivers
            if ((this.sensors.TotalReceivers > 0) && (this.sensors.TotalReceivers <= Constants.MAX_CONTROLLERS))
            {
                this.mitesControllers = new MITesReceiverController[this.sensors.TotalReceivers];
                this.mitesDecoders = new MITesDecoder[this.sensors.TotalReceivers];
                this.aMITesActivityCounters = new Hashtable();
                progressMessage += "Initializing MITes ... searching " + this.sensors.TotalReceivers + " receivers\r\n";
                if (InitializeMITes(dataDirectory) == false)
                {
                    MessageBox.Show("Exiting: You picked a configuration with " + this.sensors.TotalReceivers + " receivers. Please make sure they are attached to the computer.");
#if (PocketPC)
                    Application.Exit();
#else
                    Environment.Exit(0);
#endif
                }
            }


            //Override the resize event
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

            //Initialize the MITes quality interface
            progressMessage += "Initializing MITes Quality GUI ...";
            InitializeQualityInterface();
            progressMessage += " Completed\r\n";


            //Initialize the feature extractor
            Extractor.Initialize(this.mitesDecoders[0], dataDirectory, this.annotation, this.sensors);

            //Initialize the performance counters
            // MITes Data Filterer stores performance stats for MITes
            // Initialize all performance counters for all MITES channels
            //calculate good sampling rate           
            //you need to initialize them all because sometimes mites get data from non-exisiting IDs???
            for (i = 0; i < MITesData.MAX_MITES_CHANNELS; i++)
                MITesDataFilterer.MITesPerformanceTracker[i] = new MITesPerformanceStats(0);
            //based on how many receivers and to what channels they are listening adjust the good sampling rate
            foreach (Sensor sensor in this.sensors.Sensors)
            {
                int sensor_id = Convert.ToInt32(sensor.ID);
                int receiver_id = Convert.ToInt32(sensor.Receiver);
                if (sensor_id == 0) //HR sensor
                {
                    MITesDataFilterer.MITesPerformanceTracker[sensor_id].GoodRate = (int)(Constants.HR_SAMPLING_RATE * 0.8);
                    MITesDataFilterer.MITesPerformanceTracker[sensor_id].PerfectRate = Constants.HR_SAMPLING_RATE;
                }
                else
                {
                    int goodSamplingRate = (int)((Extractor.Configuration.ExpectedSamplingRate * (1 - Extractor.Configuration.MaximumNonconsecutiveFrameLoss)) / this.sensors.NumberSensors[receiver_id]);
                    MITesDataFilterer.MITesPerformanceTracker[sensor_id].GoodRate = goodSamplingRate;
                    MITesDataFilterer.MITesPerformanceTracker[sensor_id].PerfectRate = (int)((Extractor.Configuration.ExpectedSamplingRate) / this.sensors.NumberSensors[receiver_id]);
                }
            }

            //Initializing decision tree classifiers
            progressMessage += "Initializing Decision Tree Classifiers ...";

#if (PocketPC)

            if (isHierarchical == true)
                //Generate the hierarchical files
            {
                Hashtable clusters = FeatureSelector.ClusterByOrientations(OrientationForm.OrientationMatrix);
                Hashtable clusterFeatures = FeatureSelector.GetFeaturesByMobility(clusters, MobilityForm.MobilityMatrix);
                this.hrClassifier = new HierarchicalClassifier(this.mitesDecoders[0], this.annotation,
                    this.sensors, dataDirectory);
                //copy the arff file to a root file
                File.Copy(arffFile, dataDirectory + "\\root.arff",true);
                
                //String[] masterActivities = new String[clusters.Count];
                string[] rootFeatures = new string[Extractor.ArffAttributeLabels.Length];                
                //all activities included for the root
                ArrayList rootActivities = new ArrayList();
                foreach (AXML.Label label in ((AXML.Category)this.annotation.Categories[0]).Labels)                
                    rootActivities.Add(label.Name);                
                foreach(DictionaryEntry entry in clusters)
                {
                    int cluster_id=(int) entry.Key;
                    ArrayList cluster=(ArrayList) entry.Value;
                    ArrayList features=(ArrayList) clusterFeatures[cluster_id];
                    DTClassifier dtClassifier = new DTClassifier();
 
                    String[] acts=new String[cluster.Count];
                    for (int k = 0; (k < cluster.Count); k++)
                    {
                        acts[k] = (string)SimilarActivitiesForm.ConfusingActivities[(int)cluster[k]];
                        dtClassifier.Classes.Add(acts[k]);
                    }

                    ArrayList filteredFeatures=new ArrayList();
                    for (int k = 0; (k < Extractor.AttributeLocation.Count); k++)
                    {
                        string featureName=Extractor.ArffAttributeLabels[k];
                        rootFeatures[k] = featureName;
                        if (Extractor.AttributeLocation[featureName] != null)
                        {
                            int featureLocation = (int)Extractor.AttributeLocation[featureName];
                            for (int l = 0; (l < features.Count); l++)
                                if (featureLocation == (int)features[l])
                                    filteredFeatures.Add(featureName);
                        }
                    }
                    String[] attrs=new String[filteredFeatures.Count];
                    for (int k = 0; (k < filteredFeatures.Count); k++)
                    {
                        attrs[k] = (string)filteredFeatures[k];
                        dtClassifier.Features.Add(attrs[k]);
                    }
                    
                    
                    //Only generate arff subtree files when there is more than
                    //one activity and at least one attribute to use.
                    if ((attrs.Length > 0) && (acts.Length > 1))
                    {
                        
                        FilterList filter = new FilterList();
                        filter.replaceString(dataDirectory + "\\root.arff", dataDirectory + "\\temp", "cluster" + cluster_id, acts);
                        filter.filter(arffFile, dataDirectory + "\\cluster" + cluster_id + ".arff", attrs, acts);
                        File.Copy(dataDirectory + "\\temp", dataDirectory + "\\root.arff",true);
                        
                        dtClassifier.Filename = dataDirectory + "\\cluster" + cluster_id + ".arff";
                        dtClassifier.Name = "cluster" + cluster_id;
                        hrClassifier.Classifiers.Add(dtClassifier.Name, dtClassifier);
                        //if a cluster is added, add it and remove its activities from the root
                        rootActivities.Add("cluster" + cluster_id);
                        for (int k = 0; (k < cluster.Count); k++)                        
                            rootActivities.Remove((string)SimilarActivitiesForm.ConfusingActivities[(int)cluster[k]]);                                                   
                    }
                }

                //root classifier         
              
                DTClassifier rootClassifier = new DTClassifier();
                rootClassifier.Filename = dataDirectory + "\\root.arff";
                rootClassifier.Name = "root";
                for (int k = 0; (k < rootActivities.Count); k++)
                    rootClassifier.Classes.Add(rootActivities[k]);
                for (int k = 0; (k < rootFeatures.Length); k++)
                    rootClassifier.Features.Add(rootFeatures[k]);
                hrClassifier.Classifiers.Add("root", rootClassifier);

                TextWriter tw = new StreamWriter(dataDirectory + "\\hierarchy.xml");
                tw.WriteLine(this.hrClassifier.toXML());
                tw.Close();
                
              //at this point the HR classifier is ready to be built
            }
#else
#endif
            //Here initialize the classifier

            //Initialize the unix timer
            UnixTime.InitializeTime(); //passed to adjust time when its granularity is not good




            //Initialize the receiver threads
            bool startReceiverThreads = true;
            for (i = 0; (i < this.sensors.TotalReceivers); i++)
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

            //Terminate the progress thread
            t.Abort();

            //Initialize the interface
#if (PocketPC)
            this.tabControl1.TabPages.RemoveAt(1); //remove annotation tab
            this.tabControl1.SelectedIndex = 0;
#else
            this.ShowForms();
#endif


            //Last thing enable the timers
            this.readDataTimer.Enabled = true;
            this.qualityTimer.Enabled = true;
            if (this.sensors.IsHR)
                this.HRTimer.Enabled = true;

        }

        #endregion Classifier constructor

        #region Initialization Methods

        public void InitializeTimers()
        {
            this.goodTimer = new ATimer(this, GOOD_TIMER);
            this.overallTimer = new ATimer(this, OVERALL_TIMER);

        }

        //Initialize MITes Receivers
        private bool InitializeMITes(string dataDirectory)
        {

            // thisForm = this;
            SetFormPositions();

            //depending on the number of receivers initialize mites objects
            int maxPortSearched = 1;
            for (int i = 0; (i < this.sensors.TotalReceivers); i++)
            {
                progressMessage += "Searching for receiver " + i + "...\r\n";
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


            aMITesPlotter = new MITesScalablePlotter(this.panel1, MITesScalablePlotter.DeviceTypes.IPAQ, maxPlots, this.mitesDecoders[0], GetGraphSize(false));

            //for each sensor created a counter
            for (int i = 0; (i < this.sensors.Sensors.Count); i++)
            {
                int sensor_id = Convert.ToInt32(((SXML.Sensor)this.sensors.Sensors[i]).ID);
                if (sensor_id != 0)
                    aMITesActivityCounters.Add(sensor_id, new MITesActivityCounter(this.mitesDecoders[0], sensor_id));
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
                 Constants.ACCEL_ID3 + " ");//+
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

            return true;
        }

        #endregion Initialization Methods

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




        #region Resize Event Handlers
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
                this.button1.Width = textBoxWidth;
                this.button1.Height = textBoxHeight;
                Font textFont = this.button1.Font =
                    GUI.CalculateBestFitFont(this.button1.Parent.CreateGraphics(), Constants.MIN_FONT,
                       Constants.MAX_FONT, this.button1.Size, "textBoxAC11", this.button1.Font, (float)0.9, (float)0.9);

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

   
                //Initialize Buttons
                int button_width = this.tabPage2.ClientSize.Width - Constants.SCREEN_LEFT_MARGIN - Constants.SCREEN_RIGHT_MARGIN;
                int button_height = (this.tabPage2.ClientSize.Height - Constants.SCREEN_TOP_MARGIN - Constants.SCREEN_BOTTOM_MARGIN - (this.annotation.Categories.Count * Constants.WIDGET_SPACING)) / (this.annotation.Categories.Count + 1);
                int button_x = Constants.SCREEN_LEFT_MARGIN;
                int button_y = Constants.SCREEN_TOP_MARGIN * 2;

                int delta_y = button_height + Constants.WIDGET_SPACING;
                int button_id = 0;


                this.button1.Width = button_width;
                this.button1.Height = button_height;
                Font buttonFont = this.button1.Font =
                    GUI.CalculateBestFitFont(this.button1.Parent.CreateGraphics(), Constants.MIN_FONT,
                       Constants.MAX_FONT, this.button1.Size, longest_label, this.button1.Font, (float)0.9, (float)0.9);

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

        #endregion Resize Event Handlers

        #region Click Event Handlers

        //Click end training
        void menuItem6Tab2_Click(object sender, EventArgs e)
        {
            EndTraining();
            this.overallTimer.reset();
            this.goodTimer.reset();
        }

        //Start a training session
        void menuItem5Tab2_Click(object sender, EventArgs e)
        {

            this.arffFileName = this.dataDirectory + "\\output" + DateTime.Now.ToString().Replace('/', '_').Replace(':', '_').Replace(' ', '_') + ".arff";
            tw = new StreamWriter(arffFileName);
            if (AutoTraining == true)
            {
                int i = 0;
                tw.WriteLine("@RELATION wockets");
                tw.WriteLine(Extractor.GetArffHeader());
                tw.Write("@ATTRIBUTE activity {");
                for (i = 0; (i < ((AXML.Category)this.annotation.Categories[0]).Labels.Count - 1); i++)
                    tw.Write(((AXML.Label)((AXML.Category)this.annotation.Categories[0]).Labels[i]).Name.Replace(' ', '_') + ",");
                tw.WriteLine(((AXML.Label)((AXML.Category)this.annotation.Categories[0]).Labels[i]).Name.Replace(' ', '_') + "}");
                tw.WriteLine("\n@DATA\n\n");


            }

            this.menuItem5Tab2.Checked = true;
            this.menuItem5Tab2.Enabled = false;

            if (this.menuItem8Tab2.Checked == true) // manual, you can end it
                this.menuItem6Tab2.Enabled = true;

            //cannot change the traing mode if a session started
            this.menuItem8Tab2.Enabled = false;
            this.menuItem7Tab2.Enabled = false;

            //enable stop/start and reset buttons
            this.startStopButton.Enabled = true;
            this.resetButton.Enabled = true;
            if (AutoTraining == true)
            {
                this.startStopButton.Enabled = false;
                this.resetButton.Enabled = false;
                ((Button)this.categoryButtons[0]).Enabled = false;
                ((Button)this.categoryButtons[0]).Visible = false;

                //temporary label for auto training

                if (this.trainingLabel == null)
                {

                    this.trainingLabel = new System.Windows.Forms.Label();
                    this.trainingLabel.Location = new Point(((Button)this.categoryButtons[0]).Location.X, ((Button)this.categoryButtons[0]).Location.Y);
                    this.trainingLabel.Size = new Size(((Button)this.categoryButtons[0]).Size.Width, ((Button)this.categoryButtons[0]).Size.Height);
                    this.trainingLabel.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
                    this.panel2.Controls.Add(trainingLabel);
                }

                this.trainingLabel.Visible = true;

                this.autoTrainingIndex = 0;
                this.startActivityTime = Environment.TickCount + Extractor.Configuration.TrainingWaitTime * 1000;
            }

            this.overallTimer.start();

        }

        //Select manual mode session
        void menuItem8Tab2_Click(object sender, EventArgs e)
        {
            if (this.annotation.Categories.Count == 1)
            {
                this.menuItem8Tab2.Checked = true;
                this.menuItem8Tab2.Enabled = false;
                this.menuItem7Tab2.Checked = false;
                this.menuItem7Tab2.Enabled = true;
            }
        }

        //select auto mode session
        void menuItem7Tab2_Click(object sender, EventArgs e)
        {
            if (this.annotation.Categories.Count == 1)
            {
                this.menuItem7Tab2.Checked = true;
                this.menuItem7Tab2.Enabled = false;
                this.menuItem8Tab2.Checked = false;
                this.menuItem8Tab2.Enabled = true;
            }
        }

        private void oxycon_Click(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff = now.Subtract(origin);
            string timestamp = diff.TotalMilliseconds + "," + now.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ssK");
            TextWriter tw = new StreamWriter(this.dataDirectory + "\\OxyconSyncronizationTime.txt");
            tw.WriteLine(timestamp);
            tw.Close();
            this.oxyconButton.Enabled = false;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            this.button2.Enabled = false;
            this.isCalibrating = true;

        }
        private void button_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            int button_id = Convert.ToInt32(button.Name);
            Category category = (Category)this.annotation.Categories[button_id];
            int nextIndex = ((int)this.buttonIndex[button_id] + 1) % category.Labels.Count;
            //this.clickSound.Play();
            button.Text = ((AXML.Label)category.Labels[nextIndex]).Name;
            //((AXML.Label)category.Labels[nextIndex]).PlayTone();
            this.buttonIndex[button_id] = nextIndex;
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
                category_button.Text = ((AXML.Label)category.Labels[0]).Name;
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

        private void menuItem1_Click(object sender, EventArgs e)
        {
#if (PocketPC)
            if (MessageBox.Show("Are you sure you want to Quit MITes Software?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
#else
            if (MessageBox.Show("Are you sure you want to Quit MITes Software?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
#endif
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

#if (PocketPC)
#else
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
#endif
                Extractor.Cleanup();
                Application.Exit();
            }
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

        #endregion Click Event Handlers

        #region Helper Methods

        //close the ARFF training file and reset menus
        public void EndTraining()
        {
            tw.Close();
            this.menuItem6Tab2.Enabled = false;
            this.menuItem5Tab2.Checked = false;
            this.menuItem5Tab2.Enabled = true;
            this.trainingLabel.Visible = false;

            if (this.annotation.Categories.Count == 1) //if 1 category 
            {
                //enable whatever was not chosen to allow the user to switch the training mode
                if (this.menuItem8Tab2.Checked)
                    this.menuItem7Tab2.Enabled = true;
                else
                    this.menuItem8Tab2.Enabled = true;
            }
            this.startStopButton.Enabled = false;
            this.resetButton.Enabled = false;
            ((Button)this.categoryButtons[0]).Visible = true;
            ((Button)this.categoryButtons[0]).Enabled = true;
        }

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
                }
                else
                {
                    if (this.startStopButton.BackColor == System.Drawing.Color.Red)
                        this.goodTimer.stop();
                }
            }
        }

        /// <summary>
        /// Report the HR if getting received
        /// </summary>
        public void ReportHR()
        {
            string key = "MITes0";
            if (this.sensorLabels[key] != null)
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

                        if (result < 3.0)
                            ((System.Windows.Forms.Label)this.sensorLabels[key]).Text = "AC " + sensor_id + ": still";
                    }

                }

            }

        }

        #endregion Helper Methods

   




#if (PocketPC)
#else
        void form_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isQuitting ==false)
                e.Cancel = true;
        }

#endif
  


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


#if (PocketPC)
        void tabControl1_Changed(object sender, EventArgs e)
        {
            if (this.tabControl1.SelectedIndex == 1)
                this.Menu = this.mainMenuTab2;
            else if (this.tabControl1.SelectedIndex == 0)
                this.Menu = this.mainMenu1;
        }
#endif


        #region Graphing functions


        private void SetFormPositions()
        {

            xDim = this.panel1.ClientSize.Width;
            yDim = (int)(this.panel1.ClientSize.Height * 0.60);

            if (aMITesPlotter != null)
                aMITesPlotter.SetupScaleFactor(GetGraphSize(false), maxPlots);
        }



        private void SetPlotterFullScreen()
        {
            aMITesPlotter = new MITesScalablePlotter(this.panel1, MITesScalablePlotter.DeviceTypes.IPAQ, maxPlots, this.mitesDecoders[0], GetGraphSize(true));
        }

        private void SetPlotterPartialScreen()
        {
            aMITesPlotter = new MITesScalablePlotter(this.panel1, MITesScalablePlotter.DeviceTypes.IPAQ, maxPlots, this.mitesDecoders[0], GetGraphSize(false));
        }

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
            return new Size(xsize, ysize);
        }
        #endregion Graphing functions


        #region Timer Methods

        /// <summary>
        /// This is called every 30 seconds to check the heart rate data and update the interface
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void HRTimer_Tick(object sender, System.EventArgs e)
        {
            if (this.collectDataMode)
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
        }

        /// <summary>
        /// This is called once every second to check the sampling rate for accelerometers, update the quality form and
        /// control the good timer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

            if (this.collectDataMode) // if we are collecting data then control the timers
            {
                //stop the good timer if the overall timer is running (i.e. something is being annotated), the overall quality is bad
                if ((this.overallTimer.isRunning()) && (overallGoodQuality == false) && (this.goodTimer.isRunning()))
                    this.goodTimer.stop();
                //start the good timer if the overall timer is running (i.e. something is being annotated), the overall quality is good
                else if ((this.overallTimer.isRunning()) && (overallGoodQuality == true) && (this.goodTimer.isRunning() == false))
                    this.goodTimer.start();
            }
        }



       
        /// <summary>
        /// This methods is invoked every 10 milliseconds
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            //if all receivers are started
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


                #region MERGE MITes Data
                //aMITesDecoder.GetSensorData(this.mitesControllers[0]);
                for (int i = 0; (i < this.sensors.TotalReceivers); i++) // FIX SSI
                    this.mitesDecoders[i].GetSensorData(this.mitesControllers[i]);                

                for (int i = 1; (i < this.sensors.TotalReceivers); i++)
                    this.mitesDecoders[0].MergeDataOrderProperly(this.mitesDecoders[i]);

                #endregion MERGE MITes Data


                #region Train in realtime and generate ARFF File
                if (IsTraining==true)
                {
                    //We are autotraining each activity after the other
                    if (AutoTraining == true)
                    {
                        //Get current activity to train
                        string current_activity = ((AXML.Label)((AXML.Category)this.annotation.Categories[0]).Labels[autoTrainingIndex]).Name;
                        
                        //Check if trained
                        if (Extractor.IsTrained(current_activity))
                        {
                            //store the completed annotation and reset the variables
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
                            isExtracting = false;

                            //Point to the next activity and Calculate the delay to start training
                            this.startActivityTime = Environment.TickCount + Extractor.Configuration.TrainingWaitTime*1000;//Constants.TRAINING_GAP;
                            autoTrainingIndex++;

                           
                            //If we exceeded the last activity then training is completed
                            if (autoTrainingIndex == ((AXML.Category)this.annotation.Categories[0]).Labels.Count)
                            {
                                this.trainingLabel.Text = "Training Completed";                            
                                this.goodTimer.reset();
                                this.overallTimer.reset();
                                Thread.Sleep(3000);
                                EndTraining();
                             
                            }
                            else // if there are still activities to train beep twice and reset good timer
                            {
                                this.goodTimer.reset();
                                PlaySound(@"\Windows\Voicbeep", IntPtr.Zero, (int)(PlaySoundFlags.SND_FILENAME | PlaySoundFlags.SND_SYNC));
                                PlaySound(@"\Windows\Voicbeep", IntPtr.Zero, (int)(PlaySoundFlags.SND_FILENAME | PlaySoundFlags.SND_SYNC));
                            }
                        }
                            //if the current activity is not trained and the start time is more that the current time
                            //then calculate the feature vector
                        else if (this.startActivityTime < Environment.TickCount) // TRAINING_GAP passed 
                        {
                            //initialize for extraction
                            if (isExtracting == false)
                            {
                                this.trainingLabel.Text = "Training " + current_activity;
                                this.goodTimer.start();
                                //store the current state of the categories
                                this.currentRecord = new AnnotatedRecord();
                                this.currentRecord.StartDate = DateTime.Now.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ssK");
                                this.currentRecord.StartHour = DateTime.Now.Hour;
                                this.currentRecord.StartMinute = DateTime.Now.Minute;
                                this.currentRecord.StartSecond = DateTime.Now.Second;
                                TimeSpan ts = (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0));
                                this.currentRecord.StartUnix = ts.TotalSeconds;
                                this.currentRecord.Labels.Add(new AXML.Label(current_activity, "none"));
                                isExtracting = true;
                            }

                            //Extract feature vector from accelerometer data and write an arff line
                            double lastTimeStamp = Extractor.StoreMITesWindow();
                            if (Extractor.GenerateFeatureVector(lastTimeStamp))
                            {
                                Extractor.TrainingTime[current_activity] = (int)Extractor.TrainingTime[current_activity] + Extractor.Configuration.OverlapTime;// get it from configuration
                                string arffSample = Extractor.toString() + "," + current_activity.Replace(' ', '_');
                                this.tw.WriteLine(arffSample);
                                this.label8.Text = Extractor.DiscardedLossRateWindows.ToString();
                            }         
                        }
                            //if we are waiting for the activity to be trained
                        else                        
                            this.trainingLabel.Text = "Training " + current_activity + " in " + ((int)(this.startActivityTime - Environment.TickCount) / 1000) + " secs";
                        
                    }
                    else // Manual Training
                    {
                    }
                }



                #endregion Train in realtime and generate ARFF File

                #region Classifying activities

                if (isClassifying==true)
                {
                    double lastTimeStamp = Extractor.StoreMITesWindow();
                    if (Extractor.GenerateFeatureVector(lastTimeStamp))
                    {
                        this.label6.ForeColor = Color.Black;
                        Instance newinstance = new Instance(instances.numAttributes());
                        newinstance.Dataset = instances;


                        for (int i = 0; (i < Extractor.Features.Length); i++)
                        {
                            newinstance.setValue(instances.attribute(i), Extractor.Features[i]);
                        }
                        double predicted = classifier.classifyInstance(newinstance);


                        string predicted_activity = newinstance.dataset().classAttribute().value_Renamed((int)predicted);

                        int currentIndex = (int)labelIndex[predicted_activity];
                        labelCounters[currentIndex] = (int)labelCounters[currentIndex] + 1;
                        classificationCounter++;


                        if (classificationCounter == Extractor.Configuration.SmoothWindows)
                        {
                            classificationCounter = 0;
                            int mostCount = 0;
                            string mostActivity = "";
                            for (int j = 0; (j < labelCounters.Length); j++)
                            {
                                if ((labelCounters[j] > mostCount) && (labelCounters[j] > 3) )
                                { 
                                    mostActivity = activityLabels[j];
                                    mostCount = labelCounters[j];
                                }
                              

                                labelCounters[j] = 0;
                            }

                            if (mostActivity.Equals(""))
                                mostActivity = "Not Sure";
                            this.label6.Text = mostActivity;
                        }

                    }
                }

                #endregion Classifying activities

                #region Storing CSV data for the grapher
#if (PocketPC)
#else

                if (isCollectingDetailedData == true)
                {
                    if (activityCountWindowSize > Extractor.Configuration.QualityWindowSize) //write a line to CSV and initialize
                    {
                        DateTime now = DateTime.Now;
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
                            prevX[sensor_id] = 0;
                            prevY[sensor_id] = 0;
                            prevY[sensor_id] = 0;
                            acCounters[sensor_id] = 0;
                        }


                        if (hrCount > 0)
                        {
                            this.hrCSV.WriteLine(hr_csv_line + "," + (int)(sumHR / hrCount));
                            this.masterCSV.WriteLine(master_csv_line + "," + (int)(sumHR / hrCount));
                        }
                        else
                        {
                            this.hrCSV.WriteLine(hr_csv_line + ",");
                            this.masterCSV.WriteLine(master_csv_line + ",");
                        }

                        hrCount = 0;
                        sumHR = 0;
                        activityCountWindowSize = 0;
                    }

                    activityCountWindowSize += 10; //add 10 milliseconds
                }

#endif

                #endregion Storing CSV data for the grapher


                if ((isCalibrating) || (isCollectingDetailedData == true))
                {
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


                            if (isCalibrating)
                            {
                                if (this.calSensor == -1)
                                {
                                    if (this.currentCalibrationSensorIndex < this.sensors.Sensors.Count)
                                    {
                                        this.calX = new int[Constants.CALIBRATION_SAMPLES];
                                        this.calY = new int[Constants.CALIBRATION_SAMPLES];
                                        this.calZ = new int[Constants.CALIBRATION_SAMPLES];
                                        this.calSensor = Convert.ToInt32(((Sensor)this.sensors.Sensors[this.currentCalibrationSensorIndex]).ID);
                                        this.calSensorPosition = Constants.CALIBRATION_FLAT_HORIZONTAL_POSITION;

                                        int receiver_id = Convert.ToInt32(((Sensor)this.sensors.Sensors[this.currentCalibrationSensorIndex]).Receiver);
                                        int[] channels = new int[6];
                                        channels[0] = this.calSensor;
                                        this.mitesControllers[receiver_id].SetChannels(1, channels);

                                    }
                                    else //all sensors are calibrated
                                    {
                                        TextWriter tw = new StreamWriter("SensorData.xml");
                                        tw.WriteLine(this.sensors.toXML());
                                        tw.Close();
                                        MessageBox.Show("Calibration... Completed");
                                        Application.Exit();
                                    }

                                }

                                if (channel == this.calSensor)
                                {
                                    if (this.calSensorPosition == Constants.CALIBRATION_FLAT_HORIZONTAL_POSITION)
                                    {
                                        this.calX[this.calCounter] = x;
                                        this.calY[this.calCounter] = y;
                                        this.calCounter++;
                                    }
                                    else //vertical
                                    {
                                        this.calZ[this.calCounter] = z;
                                        this.calCounter++;
                                    }

                                    // if all required samples are collected
                                    if (this.calCounter == Constants.CALIBRATION_SAMPLES)
                                    {

                                        if (this.calSensorPosition == Constants.CALIBRATION_FLAT_HORIZONTAL_POSITION)
                                        {

                                            this.calSensorPosition = Constants.CALIBRATION_SIDE_VERTICAL_POSITION;
                                            double meanX = 0.0, meanY = 0.0;
                                            double stdX = 0.0, stdY = 0.0;
                                            this.label17.Text = "Calibration Completed! Please place the sensor " + ((SXML.Sensor)this.sensors.Sensors[this.currentCalibrationSensorIndex]).ID + " vertical on a flat surface then click start.";
                                            this.pictureBox2.Image = this.verticalMITes;
                                            this.button2.Enabled = true;
                                            for (int j = 0; (j < this.calCounter); j++)
                                            {
                                                meanX += (double)this.calX[j];
                                                meanY += (double)this.calY[j];
                                            }
                                            meanX = meanX / this.calCounter;
                                            meanY = meanY / this.calCounter;

                                            ((SXML.Sensor)this.sensors.Sensors[this.currentCalibrationSensorIndex]).XMean = meanX;
                                            ((SXML.Sensor)this.sensors.Sensors[this.currentCalibrationSensorIndex]).YMean = meanY;
                                            for (int j = 0; (j < this.calCounter); j++)
                                            {
                                                stdX += Math.Pow(this.calX[j] - meanX, 2);
                                                stdY += Math.Pow(this.calY[j] - meanY, 2);
                                            }
                                            stdX = Math.Sqrt(stdX / (this.calCounter - 1));
                                            stdY = Math.Sqrt(stdY / (this.calCounter - 1));

                                            ((SXML.Sensor)this.sensors.Sensors[this.currentCalibrationSensorIndex]).XStd = stdX;
                                            ((SXML.Sensor)this.sensors.Sensors[this.currentCalibrationSensorIndex]).YStd = stdY;
                                        }
                                        else
                                        {


                                            if (this.currentCalibrationSensorIndex < this.sensors.Sensors.Count)
                                            {
                                                this.label17.Text = "Calibration Completed! Please place the sensor " + ((SXML.Sensor)this.sensors.Sensors[this.currentCalibrationSensorIndex]).ID + " horizontal on a flat surface then click start.";
                                                this.pictureBox2.Image = this.horizontalMITes;
                                                this.button2.Enabled = true;
                                            }
                                            this.calSensorPosition = Constants.CALIBRATION_FLAT_HORIZONTAL_POSITION;
                                            double meanZ = 0.0;
                                            double stdZ = 0.0;

                                            for (int j = 0; (j < this.calCounter); j++)
                                                meanZ += (double)this.calZ[j];
                                            meanZ = meanZ / this.calCounter;
                                            ((SXML.Sensor)this.sensors.Sensors[this.currentCalibrationSensorIndex]).ZMean = meanZ;

                                            for (int j = 0; (j < this.calCounter); j++)
                                                stdZ += Math.Pow(this.calZ[j] - meanZ, 2);
                                            stdZ = Math.Sqrt(stdZ / (this.calCounter - 1));
                                            ((SXML.Sensor)this.sensors.Sensors[this.currentCalibrationSensorIndex]).ZStd = stdZ;
                                            this.currentCalibrationSensorIndex++;
                                            this.calSensor = -1;
                                        }

                                        this.isCalibrating = false;
                                        this.calCounter = 0;


                                    }
                                }

                            }
#if (PocketPC)
#else
                            if (channel <= this.sensors.MaximumSensorID) //if junk comes ignore it
                            {
                                if ((prevX[channel] > 0) && (prevY[channel] > 0) && (prevZ[channel] > 0) && (x > 0) && (y > 0) && (z > 0))
                                {
                                    averageX[channel] = averageX[channel] + Math.Abs(prevX[channel] - x);
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
#endif
                        }
                    }

                }
                



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

        #endregion Timer Methods






 

    }
}