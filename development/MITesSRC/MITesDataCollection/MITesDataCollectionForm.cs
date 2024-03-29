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
#if (PocketPC)
using Bluetooth;
#endif



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

    public partial class MITesDataCollectionForm : Form, ControlCreator
    {

        #region Declarations of Objects

        #region Definition of Plotting and Graphing Variables
        /// <summary>
        /// Set when the form is resized
        /// </summary>
        private bool isResized = false;
        /// <summary>
        /// Set when the form needs to be redrawn
        /// </summary>
        private bool isNeedRedraw = false;
        /// <summary>
        /// The width of the main form - set dynamically based on screen size
        /// </summary>
        private int xDim = 240;
        /// <summary>
        /// The height of the main form - set dynamically based on screen size
        /// </summary>
        private int yDim = 320;
        /// <summary>
        /// The maximum number of plots on the screen, set dynamically based on the number of accelerometers in configuration files
        /// </summary>
        private int maxPlots = 3; // Changed from 6
        /// <summary>
        /// True when form plots otherwise false
        /// </summary>
        private bool isPlotting = true;
        /// <summary>
        /// Backbuffer for plotting the accelerometer data
        /// </summary>
        private Bitmap backBuffer = null;
        /// <summary>
        /// True when plotting full screen
        /// </summary>
        private bool isPlottingFullScreen = false;
        //TODO: change the name of the plotter to something reasonable since it is generic
        /// <summary>
        /// A plotter for accelerometer data
        /// </summary>
        private MITesScalablePlotter aMITesPlotter;
        
        private Pen aPen = new Pen(Color.Wheat);
        private SolidBrush aBrush = new SolidBrush(Color.White);
        private SolidBrush blueBrush = new SolidBrush(Color.LightBlue);
        private SolidBrush redBrush = new SolidBrush(Color.Red);
        private int gapDistance = 4;        

        #endregion Definition of Plotting and Graphing Variables

        #region Definition of different timers
        /// <summary>
        /// A Unique ID for timer of good quality data
        /// </summary>
        public const int GOOD_TIMER = 1;
        /// <summary>
        /// A unique ID for timer of all data
        /// </summary>
        public const int OVERALL_TIMER = 2;
        /// <summary>
        /// A unique ID for timer of an activity
        /// </summary>
        public const int ACTIVITY_TIMER = 3;
        /// <summary>
        /// Measures the time for good quality data during a data collection session
        /// </summary>
        private ATimer goodTimer;
        /// <summary>
        /// Measures the overall time for a data collection session
        /// </summary>
        private ATimer overallTimer;
        /// <summary>
        /// Measures the length of an activity as determined by the classifier
        /// </summary>
        private ATimer activityTimer;
        #endregion Definition of different timers

        #region GUI Delegates
        /// <summary>
        /// Delegate that sets a label from other threads
        /// </summary>
        /// <param name="label">Text for the label</param>
        /// <param name="control_id">Control ID for the label</param>
        delegate void SetTextCallback(string label, int control_id);
        /// <summary>
        /// Delegate that sets the graphics for the signal strength from other threads
        /// </summary>
        /// <param name="isGood">True if signal is good otherwise false</param>
        delegate void SetSignalCallback(bool isGood);
        /// <summary>
        /// Delegate that sets an error label from different threads (e.g. used to display bluetooth disconnection)
        /// </summary>
        /// <param name="label"></param>
        delegate void SetErrorCallback(string label);
        #endregion GUI Delegates

        #region Definition of GUI Components
        /// <summary>
        /// A hashtable for the labels of different snesors
        /// </summary>
        private Hashtable sensorLabels;
        private System.Windows.Forms.Label[] labels;
        /// <summary>
        /// Expected sampling rate labels
        /// </summary>
        private System.Windows.Forms.Label[] expectedLabels;
        /// <summary>
        /// Samples per second labels
        /// </summary>
        private System.Windows.Forms.Label[] samplesPerSecond;
        /// <summary>
        /// The message to be displayed by the progress thread
        /// </summary>
        private string progressMessage;
        /// <summary>
        /// The progress thread object
        /// </summary>
        private Thread aProgressThread = null;
        /// <summary>
        /// True if the progress thread should quit
        /// </summary>
        private bool progressThreadQuit = false;
        /// <summary>
        /// Counter to update the number of samples
        /// </summary>
        private int printSamplingCount = 0;
        /// <summary>
        /// An array list of the different buttons of the annotator
        /// </summary>
        private ArrayList categoryButtons;
        /// <summary>
        /// An array list that stores the current index for each button
        /// </summary>
        private ArrayList buttonIndex;
        /// <summary>
        /// A variable that stores the longest label on a category button for dynamic resizing of the buttons
        /// </summary>
        private string longest_label = "";
        #endregion Definition of GUI Components

        #region Definition of Logging Variables and Flags
        /// <summary>
        /// A constant that specifies how often to flush logging data
        /// </summary>
        private const int FLUSH_TIMER_MAX = 6000;
        /// <summary>
        /// Counter used to log status data when it reaches its max
        /// </summary>
        int flushTimer = 0;
        /// <summary>
        /// A flag used to write the key once for the log file
        /// </summary>
        bool isWrittenKey = false;
        #endregion Definition of Logging Variables and Flags

        #region Wockets and MITes Variables

        #region Definitions of general configuration variables
        /// <summary>
        /// An object that stores the annotation configuration file that initializes the annotation loaded from ActivityRealtime.xml
        /// </summary>
        private Annotation annotation;
        /// <summary>
        /// An object that stores the sensor configuration including different reception channels and sensors, loaded from SensorData.xml
        /// </summary>
        private SensorAnnotation sensors;        
        /// <summary>
        /// An object that stores a variety of configuration parameters for the software including machine learning and sampling values etc.
        /// </summary>
        private MITesFeatures.core.conf.GeneralConfiguration configuration;
        /// <summary>
        /// Directory where the collected data will be stored
        /// </summary>
        private string dataDirectory;
        //TODO: move it to configuration file
        /// <summary>
        /// Defines the buffer size for different controllers - expanded for wockets
        /// </summary>
        private const int BYTES_BUFFER_SIZE = 4096; //2048      
 
        #endregion Definitions of general configuration variables

        #region Definition of controllers for different reception channels
        //TODO: Define a single interface for ReceiverController and extend it to use USB,Bluetooth or DiamondTouch
        private MITesReceiverController[] mitesControllers;
#if (PocketPC)
        private BluetoothController[] bluetoothControllers;
#endif
        #endregion Definition of controllers for different reception channels

        #region Definition of data decoders
        // TO DO: Have a single decoder interface and extend it for MITes, wockets and HTC
        // Merge and timestamp at this point rather than before to ensure ordering...
        // Get rid of sorting completely
        // Efficiently store the exact number of decoders needed and allocate proper array etc... don't
        /// <summary>
        /// An array that stores a separate decoder for each sensor
        /// </summary>
        private MITesDecoder[] mitesDecoders;
        /// <summary>
        /// A single master decoder that does the actual decoding
        /// </summary>
        private MITesDecoder masterDecoder;

#if (PocketPC)
        /// <summary>
        /// A decoder for the builtin accelerometer of HTC Diamond Touch
        /// </summary>
        private PhoneAccelerometers.HTC.DiamondTouch.HTCDecoder htcDecoder;
#endif

        #endregion Definition of decoders

        #region Definition of performance objects and counters
        /// <summary>
        /// Stores activity counts for the MITes
        /// </summary>
        private Hashtable aMITesActivityCounters;
        /// <summary>
        /// Stores sampling rate
        /// </summary>
        private MITesDataFilterer aMITesDataFilterer;
        /// <summary>
        /// Analyzes MITes HR
        /// </summary>
        private MITesHRAnalyzer aMITesHRAnalyzer;
        #endregion Definition of performance objects and counters

        #region Definition of logging functions
        private MITesLoggerPLFormat aMITesLoggerPLFormat;
        private MITesActivityLogger aMITesActivityLogger;
        #endregion Definition of logging functions

        #region Definition of built-in sensors polling threads   (Pocket PC Only)
#if (PocketPC)
        /// <summary>
        /// Counter for the next polling time for the built-in accelerometer
        /// </summary>
        private int pollingTime = Environment.TickCount;
        /// <summary>
        /// A polling thread for the built-in accelerometer
        /// </summary>
        private Thread pollingThread;
#endif
        #endregion Definition of built-in sensors polling threads   (Pocket PC Only)

        #endregion Wockets and MITes Variables

        #region Definition of classification variables

        /// <summary>
        /// The classifier object that is used to do activity recognition
        /// </summary>
        private Classifier classifier;
        /// <summary>
        /// The feature vector that lists all features used by the classifier
        /// </summary>
        private FastVector fvWekaAttributes;
        /// <summary>
        /// A list of instances that are used for training or intializing a classifier
        /// </summary>
        private Instances instances;
        /// <summary>
        /// Stores the formatted labels for the classifier
        /// </summary>
        private string[] activityLabels;
        /// <summary>
        /// Stores the index of each label for fast search
        /// </summary>
        private Hashtable labelIndex;
        /// <summary>
        /// Stores the arff file name where the extracted features will be stored
        /// </summary>
        private string arffFileName;
        /// <summary>
        /// Arff file Text writer
        /// </summary>
        private TextWriter tw;
        /// <summary>
        /// The index of the current activity being trained in auto-training mode
        /// </summary>
        private int autoTrainingIndex;
        /// <summary>
        /// The time when training an activity started
        /// </summary>
        int startActivityTime;
        /// <summary>
        /// A counter for windows used in smoothening
        /// </summary>
        private int classificationCounter;
        /// <summary>
        /// Stores counters for each label to be used during smoothening
        /// </summary>
        private int[] labelCounters;

        #endregion Definition of classification variables

        #region Definition of different software modes

        /// <summary>
        /// True if the software is training
        /// </summary>
        private bool isTraining;
        /// <summary>
        /// True if the software is running in auto-training mode
        /// </summary>
        private bool isAutoTraining;
        /// <summary>
        /// True if the software is required to do real-time feature extraction otherwise false
        /// </summary>
        private bool isExtracting;
        /// <summary>
        /// True if the software is required to do real-time classification otherwise false
        /// </summary>
        private bool isClassifying;
        /// <summary>
        /// True if the software is required to claibrate sensors
        /// </summary>
        private bool isCalibrating = false;
        /// <summary>
        /// True if the software is required to log CSV data
        /// </summary>
        private bool isCollectingDetailedData = false;
        /// <summary>
        /// True if collecting annotation and sensor data
        /// </summary>
        private bool collectDataMode = false;
        /// <summary>
        /// True if the software is about to quit
        /// </summary>
        private bool isQuitting = false;
        #endregion Definition of different software modes

        #region Definition of objects that store values for CSV files
#if (!PocketPC)

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
#endif
        #endregion Definition of objects that store values for CSV files

        #region Definition of calibration objects
        private int[] calX, calY, calZ;
        private int calSensor = -1;
        private int calSensorPosition;
        private int calCounter;
        private int currentCalibrationSensorIndex;
        private System.Drawing.Image horizontalMITes;
        private System.Drawing.Image verticalMITes;
        #endregion Definition of calibration objects

        #region Definition of annotation objects
        /// <summary>
        /// Stores the current record that is being annotated
        /// </summary>
        private AnnotatedRecord currentRecord;

        #endregion Definition of annotation objects

#if (PocketPC)
        [DllImport("coredll.dll")]
        public static extern int PlaySound(
            string szSound,
            IntPtr hModule,
            int flags);
#endif

        #endregion Declarations of Objects



        /// <summary>
        /// This thread creates a progress form, showing the different steps
        /// as the software loads
        /// </summary>
        private void ProgressThread()
        {
            ProgressForm progressForm = new ProgressForm();
            progressForm.Show();
            while (progressThreadQuit == false)
            {
#if (PocketPC)
                Thread.Sleep(5);
#else
                Thread.Sleep(20);
#endif

                if (progressMessage != null)
                     progressForm.UpdateProgressBar(progressMessage);

            }
            progressForm.Close();
            aProgressThread.Abort();            
        }


        #region Calibration Constructor
        //This constructor initializes the software for calibration of a list
        //of sensors
        public MITesDataCollectionForm(SensorAnnotation uncalibratedSensors, string dataDirectory)
        {
            //Initialize the UNIX timer to use QueryPerformanceCounter
            UnixTime.InitializeTime();

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

            //setup where the sensordata file will be stored
            this.dataDirectory = dataDirectory;

            //setup plotting parameters
            isPlotting = true;
            this.maxPlots = this.sensors.Sensors.Count;

            //Spawn the progress thread
            progressMessage = null;
            aProgressThread = new Thread(new ThreadStart(ProgressThread));
            aProgressThread.Start();





            //Intialize the interface of the forms
            InitializeComponent();
            progressMessage += "Initializing Timers ...";
            InitializeTimers();
            progressMessage += " Completed\r\n";
            progressMessage += "Initializing GUI ...";
            InitializeInterface();
            progressMessage += " Completed\r\n";

            progressMessage += "Loading configuration file ...";
            MITesFeatures.core.conf.ConfigurationReader confreader = new MITesFeatures.core.conf.ConfigurationReader(dataDirectory);
            this.configuration = confreader.parse();
            progressMessage += " Completed\r\n";

#if (PocketPC)

            //setup the Bluetooth if needed
            if (this.configuration.Connection == MITesFeatures.core.conf.Constants.SOFTWARE_CONNECTION_BLUETOOTH)
            {
                progressMessage += "Initializing Bluetooth ...";
                /*
                this.bt = new BluetoothController();
                try
                {
                    this.bluetoothPort = bt.initialize(this.configuration.MacAddress, this.configuration.Passkey);
                }
                catch (Exception e)
                {
                   
                    progressMessage += " Failed\r\n";
                    MessageBox.Show("Failed to find Bluetooth Device... exiting!");
                    bt.close();
                    Application.Exit();
                    System.Diagnostics.Process.GetCurrentProcess().Kill();    
                }
                 */
                progressMessage += " Completed\r\n";
            }
#endif
            //Intialize the MITes Receivers, decoders and counters based
            //on the chosen sensors
            if ((this.sensors.TotalReceivers > 0) && (this.sensors.TotalReceivers <= Constants.MAX_CONTROLLERS))
            {
                this.mitesControllers = new MITesReceiverController[this.sensors.TotalReceivers];
                this.mitesDecoders = new MITesDecoder[this.sensors.TotalReceivers];
                //this.aMITesActivityCounters = new Hashtable();
                progressMessage += "Initializing MITes ... searching " + this.sensors.TotalReceivers + " receivers\r\n";
                if (InitializeMITes(dataDirectory) == false)
                {
                    MessageBox.Show("Exiting: You picked a configuration with " + this.sensors.TotalReceivers + " receivers. Please make sure they are attached to the computer.");
#if (PocketPC)
                    /*
                    bt.close();
                     */
                    Application.Exit();
                    System.Diagnostics.Process.GetCurrentProcess().Kill();    
#else

                    Environment.Exit(0);
                    Application.Exit();
#endif
                }
            }


            //Setup the resize event for each different form
#if (PocketPC)
            this.Resize += new EventHandler(OnResize);
#else
            this.form1.Resize += new EventHandler(OnResizeForm1);
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


#if (PocketPC)
            this.tabControl1.SelectedIndex = 0;
#endif


            progressThreadQuit = true;
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

        #region Annotator-Only Constructor
        public MITesDataCollectionForm()
        {

            //Initialize the UNIX QueryPerformanceCounter
            UnixTime.InitializeTime();

            //intialize the mode of the software
            this.collectDataMode = false;
            this.isCollectingDetailedData = false;
            this.isPlotting = false;
            this.isExtracting = false;

            //initialize the progress thread
            progressMessage = null;
            aProgressThread = new Thread(new ThreadStart(ProgressThread));
            aProgressThread.Start();

            //initialize the interface components
            InitializeComponent();

            //Initialize where the data will be stored and where the configuration
            //files exist
            this.dataDirectory = Constants.DEFAULT_DATA_STORAGE_DIRECTORY;


            //load the activity and sensor configuration files
            progressMessage = "Loading XML protocol and sensors ...";
            AXML.Reader reader = new AXML.Reader(Constants.MASTER_DIRECTORY, Constants.DEFAULT_DATA_STORAGE_DIRECTORY);
            this.annotation = reader.parse();
            this.annotation.DataDirectory = Constants.DEFAULT_DATA_STORAGE_DIRECTORY;

            SXML.Reader sreader = new SXML.Reader(Constants.MASTER_DIRECTORY, Constants.DEFAULT_DATA_STORAGE_DIRECTORY);
            this.sensors = sreader.parse(Constants.MAX_CONTROLLERS);

            progressMessage += " Completed\r\n";

            progressMessage += "Loading configuration file ...";
            MITesFeatures.core.conf.ConfigurationReader creader = new MITesFeatures.core.conf.ConfigurationReader(Constants.DEFAULT_DATA_STORAGE_DIRECTORY);
            this.configuration = creader.parse();
            progressMessage += " Completed\r\n";

            //calculate how many plots to be drawn
            if (this.sensors.IsHR)
                this.maxPlots = this.sensors.Sensors.Count - 1;
            else
                this.maxPlots = this.sensors.Sensors.Count;


            //Initialize the timers
            progressMessage += "Initializing Timers ...";
            InitializeTimers();
            progressMessage += " Completed\r\n";

            //Initialize different GUI components
            progressMessage += "Initializing GUI ...";
            InitializeInterface();
            progressMessage += " Completed\r\n";

#if (PocketPC)
            this.tabControl1.TabPages.RemoveAt(4);
            this.tabControl1.TabPages.RemoveAt(3);
            this.tabControl1.TabPages.RemoveAt(2);
            this.tabControl1.TabPages.RemoveAt(0);                       
            this.tabControl1.SelectedIndex = 0;
#endif
            progressThreadQuit = true;
        }
        #endregion Annotator-Only Constructor

        #region Collect Data Constructor (Wockets, MITes, Builtin)
        public MITesDataCollectionForm(string dataDirectory)
        {

           //where data is being stored
            this.dataDirectory = dataDirectory;

            //Initialize high resolution unix timer
            UnixTime.InitializeTime();

            //Initialize and start GUI progress thread
            progressMessage = null;
            aProgressThread = new Thread(new ThreadStart(ProgressThread));
            aProgressThread.Start();


            #region Load Configuration files
            //load the activity and sensor configuration files
            progressMessage = "Loading XML protocol and sensors ...";
            AXML.Reader reader = new AXML.Reader(Constants.MASTER_DIRECTORY, dataDirectory);
#if (!PocketPC)
            if (reader.validate() == false)
            {
                throw new Exception("Error Code 0: XML format error - activities.xml does not match activities.xsd!");
            }
            else
            {
#endif
            this.annotation = reader.parse();
            this.annotation.DataDirectory = dataDirectory;
            SXML.Reader sreader = new SXML.Reader(Constants.MASTER_DIRECTORY, dataDirectory);
#if (!PocketPC)

                if (sreader.validate() == false)
                {
                    throw new Exception("Error Code 0: XML format error - sensors.xml does not match sensors.xsd!");
                }
                else
                {
#endif
            this.sensors = sreader.parse(Constants.MAX_CONTROLLERS);
            progressMessage += " Completed\r\n";

            //TODO: remove BT components
            progressMessage += "Loading configuration file ...";
            MITesFeatures.core.conf.ConfigurationReader creader = new MITesFeatures.core.conf.ConfigurationReader(dataDirectory);
            this.configuration = creader.parse();
            progressMessage += " Completed\r\n";
#if (!PocketPC)
                }
            }
#endif
            #endregion Load Configuration files

            //Initialize 1 master decoder
            this.masterDecoder = new MITesDecoder();


            #region Initialize External Data Reception Channels
            //Initialize Data reception for Bluetooth and USB
            if ((this.sensors.TotalReceivers > 0) && (this.sensors.TotalReceivers <= Constants.MAX_CONTROLLERS))
            {

                //Initialize arrays to store USB and Bluetooth controllers
                this.mitesControllers = new MITesReceiverController[this.sensors.TotalWiredReceivers];
#if (PocketPC)
                this.bluetoothControllers = new BluetoothController[this.sensors.TotalBluetoothReceivers];
               // this.ts = new Thread[this.sensors.TotalBluetoothReceivers];
#endif

                //Initialize array to store Bluetooth connection status
                //this.bluetoothConnectionStatus = new bool[this.sensors.TotalBluetoothReceivers];

                //Initialize a decoder for each sensor
                this.mitesDecoders = new MITesDecoder[this.sensors.TotalReceivers];
                
                this.aMITesActivityCounters = new Hashtable();

#if (PocketPC)
                #region Bluetooth reception channels initialization
                //Initialize and search for wockets connections
                progressMessage += "Initializing Bluetooth receivers ... searching " + this.sensors.TotalBluetoothReceivers+ " BT receivers\r\n";                
                //Try to initialize all Bluetooth receivers 10 times then exit
                int initializationAttempt = 0;
                while (initializationAttempt <= 10)
                {
                    if (InitializeBluetoothReceivers() == false)
                    {
                        initializationAttempt++;

                        if (initializationAttempt == 10)
                        {
                            MessageBox.Show("Exiting: Some Bluetooth receivers in your configuration were not initialized.");

                            Application.Exit();
                            System.Diagnostics.Process.GetCurrentProcess().Kill();
                        }
                        else
                            progressMessage += "Failed to initialize all BT connections. Retrying (" + initializationAttempt + ")...\r\n";

                    }
                    else
                        break;
                    Thread.Sleep(2000);
                }
                #endregion Bluetooth reception channels initialization
#endif

                #region USB reception channels initialization

                if (InitializeUSBReceivers() == false)
                {
                    MessageBox.Show("Exiting: Some USB receivers in your configuration were not initialized.");
#if (PocketPC)
                    Application.Exit();
                    System.Diagnostics.Process.GetCurrentProcess().Kill();
#else
                    Environment.Exit(0);
#endif

                }
                #endregion USB reception channels initialization
            }
            #endregion Initialize External Data Reception Channels

#if (PocketPC)
            #region Initialize Builtin Data Reception Channels
            if (InitializeBuiltinReceivers() == false)
            {
                MessageBox.Show("Exiting: A built in receiver channel was not found.");

                Application.Exit();
                System.Diagnostics.Process.GetCurrentProcess().Kill();


            }
            #endregion Initialize Builtin Data Reception Channels
#endif


            #region Initialize GUI Components
            //initialize the interface components
            InitializeComponent();
            //Initialize GUI timers
            progressMessage += "Initializing Timers ...";
            InitializeTimers();
            progressMessage += " Completed\r\n";

            //Initialize different GUI components
            progressMessage += "Initializing GUI ...";
            InitializeInterface();
            progressMessage += " Completed\r\n";

            this.isPlotting = true;
            //count the number of accelerometers
            if (this.sensors.IsHR)
                this.maxPlots = this.sensors.Sensors.Count - 1;
            else
                this.maxPlots = this.sensors.Sensors.Count;
            SetFormPositions();
            if (this.sensors.TotalReceivers > 0)
                aMITesPlotter = new MITesScalablePlotter(this.panel1, MITesScalablePlotter.DeviceTypes.IPAQ, maxPlots, this.masterDecoder, GetGraphSize(false));
            else
                aMITesPlotter = new MITesScalablePlotter(this.panel1, MITesScalablePlotter.DeviceTypes.IPAQ, maxPlots, this.masterDecoder, GetGraphSize(false));

            //Override the resize event
#if (PocketPC)
            this.Resize += new EventHandler(OnResize);
#else
            this.form1.Resize += new EventHandler(OnResizeForm1);
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

            //Remove classifier tabs
#if (PocketPC)

            this.tabControl1.TabPages.RemoveAt(4);
            this.tabControl1.SelectedIndex = 0;
#else
            this.ShowForms();
#endif


            #endregion Initialize GUI Components

            #region Initialize Feature Extraction
            this.isExtracting = false;
            if (this.sensors.TotalReceivers > 0) // if there is at least 1 MIT
                //Extractor.Initialize(this.mitesDecoders[0], dataDirectory, this.annotation, this.sensors, this.configuration);
                Extractor.Initialize(this.masterDecoder, dataDirectory, this.annotation, this.sensors, this.configuration);
            else if (this.sensors.Sensors.Count > 0) // only built in
                Extractor.Initialize(new MITesDecoder(), dataDirectory, this.annotation, this.sensors, this.configuration);
            #endregion Initialize Feature Extraction

            #region Initialize Quality Tracking variables
            InitializeQuality();
            #endregion Initialize Quality Tracking variables

            #region Initialize Logging
            InitializeLogging(dataDirectory);
            #endregion Initialize Logging

            #region Initialize CSV Storage (PC Only)
#if (!PocketPC)

            //create some counters for activity counts
            averageX = new int[this.sensors.MaximumSensorID + 1];
            averageY = new int[this.sensors.MaximumSensorID + 1];
            averageZ = new int[this.sensors.MaximumSensorID + 1];

            averageRawX = new int[this.sensors.MaximumSensorID + 1];
            averageRawY = new int[this.sensors.MaximumSensorID + 1];
            averageRawZ = new int[this.sensors.MaximumSensorID + 1];

            prevX = new int[this.sensors.MaximumSensorID + 1];
            prevY = new int[this.sensors.MaximumSensorID + 1];
            prevZ = new int[this.sensors.MaximumSensorID + 1];
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
                master_csv_header += "," + category.Name;


            foreach (Sensor sensor in this.sensors.Sensors)
            {
                int sensor_id = Convert.ToInt32(sensor.ID);
                string location = sensor.Location.Replace(' ', '-');
                if (sensor_id > 0) //exclude HR
                {
                    activityCountCSVs[sensor_id] = new StreamWriter(dataDirectory + "\\MITes_" + sensor_id.ToString("00") + "_ActivityCount_" + location + ".csv");
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

            #endregion Initialize CSV Storage (PC Only)

            #region Start Collecting Data
            this.collectDataMode = true;
#if (PocketPC)
            this.isCollectingDetailedData = false;
#else
            this.isCollectingDetailedData = true;
#endif

            //if (this.sensors.TotalReceivers > 0)
            //    isStartedReceiver = true;
            //Start the built in polling thread            
#if (PocketPC)
            if (this.sensors.HasBuiltinSensors)
            {
                this.pollingThread = new Thread(new ThreadStart(this.pollingData));
                this.pollingThread.Priority = ThreadPriority.Lowest;
                this.pollingThread.Start();
            }
#endif

            //Terminate the progress thread
            progressThreadQuit = true;

           
            //Enable all timer functions
            this.readDataTimer.Enabled = true;
            this.qualityTimer.Enabled = true;
            if (this.sensors.IsHR)
                this.HRTimer.Enabled = true;

            #endregion Start Collecting Data

        }
        #endregion Collect Data Constructor (Wockets, MITes, Builtin)

        #region Classifier constructor

        public MITesDataCollectionForm(string dataDirectory, string arffFile, bool isHierarchical)
        { 
        

                       //where data is being stored
            this.dataDirectory = dataDirectory;

            //Initialize high resolution unix timer
            UnixTime.InitializeTime();

            //Initialize and start GUI progress thread
            progressMessage = null;
            aProgressThread = new Thread(new ThreadStart(ProgressThread));
            aProgressThread.Start();


            #region Load Configuration files
            //load the activity and sensor configuration files
            progressMessage = "Loading XML protocol and sensors ...";
            AXML.Reader reader = new AXML.Reader(Constants.MASTER_DIRECTORY, dataDirectory);
#if (!PocketPC)
            if (reader.validate() == false)
            {
                throw new Exception("Error Code 0: XML format error - activities.xml does not match activities.xsd!");
            }
            else
            {
#endif
            this.annotation = reader.parse();
            this.annotation.DataDirectory = dataDirectory;
            SXML.Reader sreader = new SXML.Reader(Constants.MASTER_DIRECTORY, dataDirectory);
#if (!PocketPC)

                if (sreader.validate() == false)
                {
                    throw new Exception("Error Code 0: XML format error - sensors.xml does not match sensors.xsd!");
                }
                else
                {
#endif
            this.sensors = sreader.parse(Constants.MAX_CONTROLLERS);
            progressMessage += " Completed\r\n";

            //TODO: remove BT components
            progressMessage += "Loading configuration file ...";
            MITesFeatures.core.conf.ConfigurationReader creader = new MITesFeatures.core.conf.ConfigurationReader(dataDirectory);
            this.configuration = creader.parse();
            progressMessage += " Completed\r\n";
#if (!PocketPC)
                }
            }
#endif
            #endregion Load Configuration files



            #region Initialize External Data Reception Channels

                //Initialize 1 master decoder
                this.masterDecoder = new MITesDecoder();

                //Initialize the software mode
                isExtracting = false;
                isCollectingDetailedData = false;
                isPlotting = true;
                isClassifying = true;


                #region Initialize Feature Extraction
                this.isExtracting = false;
                if (this.sensors.TotalReceivers > 0) // if there is at least 1 MIT
                    //Extractor.Initialize(this.mitesDecoders[0], dataDirectory, this.annotation, this.sensors, this.configuration);
                    Extractor.Initialize(this.masterDecoder, dataDirectory, this.annotation, this.sensors, this.configuration);
                else if (this.sensors.Sensors.Count > 0) // only built in
                    Extractor.Initialize(this.masterDecoder, dataDirectory, this.annotation, this.sensors, this.configuration);
                #endregion Initialize Feature Extraction

                labelIndex = new Hashtable();
                instances = new Instances(new StreamReader(arffFile));
                instances.Class = instances.attribute(Extractor.ArffAttributeLabels.Length);
                classifier = new J48();
                if (!File.Exists("model.xml"))
                {
                    classifier.buildClassifier(instances);
                    TextWriter tc = new StreamWriter("model.xml");
                    classifier.toXML(tc);
                    tc.Flush();
                    tc.Close();
                }
                else
                    classifier.buildClassifier("model.xml", instances);

               
                fvWekaAttributes = new FastVector(Extractor.ArffAttributeLabels.Length + 1);
                for (int i = 0; (i < Extractor.ArffAttributeLabels.Length); i++)
                    fvWekaAttributes.addElement(new weka.core.Attribute(Extractor.ArffAttributeLabels[i]));

                FastVector fvClassVal = new FastVector();
                labelCounters = new int[((AXML.Category)this.annotation.Categories[0]).Labels.Count + 1];
                activityLabels = new string[((AXML.Category)this.annotation.Categories[0]).Labels.Count + 1];
                for (int i = 0; (i < ((AXML.Category)this.annotation.Categories[0]).Labels.Count); i++)
                {
                    labelCounters[i] = 0;
                    string label = "";
                    int j = 0;
                    for (j = 0; (j < this.annotation.Categories.Count - 1); j++)
                        label += ((AXML.Label)((AXML.Category)this.annotation.Categories[j]).Labels[i]).Name.Replace(' ', '_') + "_";
                    label += ((AXML.Label)((AXML.Category)this.annotation.Categories[j]).Labels[i]).Name.Replace(' ', '_');
                    activityLabels[i] = label;
                    labelIndex.Add(label, i);
                    fvClassVal.addElement(label);
                }

                weka.core.Attribute ClassAttribute = new weka.core.Attribute("activity", fvClassVal);

                isClassifying = true;

                this.aMITesActivityCounters = new Hashtable();


                if (!((this.sensors.Sensors.Count == 1) && (this.sensors.HasBuiltinSensors)))
                {
                    //Initialize arrays to store USB and Bluetooth controllers
                    this.mitesControllers = new MITesReceiverController[this.sensors.TotalWiredReceivers];

#if (PocketPC)
                    this.bluetoothControllers = new BluetoothController[this.sensors.TotalBluetoothReceivers];
                    //this.ts = new Thread[this.sensors.TotalBluetoothReceivers];
#endif

                    //Initialize array to store Bluetooth connection status
                    //this.bluetoothConnectionStatus = new bool[this.sensors.TotalBluetoothReceivers];

                    //Initialize a decoder for each sensor
                    this.mitesDecoders = new MITesDecoder[this.sensors.TotalReceivers];


#if (PocketPC)
                    #region Bluetooth reception channels initialization
                    //Initialize and search for wockets connections
                    progressMessage += "Initializing Bluetooth receivers ... searching " + this.sensors.TotalBluetoothReceivers + " BT receivers\r\n";
                    //Try to initialize all Bluetooth receivers 10 times then exit
                    int initializationAttempt = 0;
                    while (initializationAttempt <= 10)
                    {
                        if (InitializeBluetoothReceivers() == false)
                        {
                            initializationAttempt++;

                            if (initializationAttempt == 10)
                            {
                                MessageBox.Show("Exiting: Some Bluetooth receivers in your configuration were not initialized.");
                                Application.Exit();
                                System.Diagnostics.Process.GetCurrentProcess().Kill();

                            }
                            else
                                progressMessage += "Failed to initialize all BT connections. Retrying (" + initializationAttempt + ")...\r\n";

                        }
                        else
                            break;
                        Thread.Sleep(2000);
                    }
                    #endregion Bluetooth reception channels initialization
#endif
                    #region USB reception channels initialization

                    if (InitializeUSBReceivers() == false)
                    {
                        MessageBox.Show("Exiting: Some USB receivers in your configuration were not initialized.");
#if (PocketPC)
                        Application.Exit();
                        System.Diagnostics.Process.GetCurrentProcess().Kill();
#else
                    Environment.Exit(0);
#endif

                    }
                    #endregion USB reception channels initialization

                }
                    //}
            #endregion Initialize External Data Reception Channels

#if (PocketPC)
            #region Initialize Builtin Data Reception Channels
            if (InitializeBuiltinReceivers() == false)
            {
                MessageBox.Show("Exiting: A built in receiver channel was not found.");
                Application.Exit();
                System.Diagnostics.Process.GetCurrentProcess().Kill();

            }
            #endregion Initialize Builtin Data Reception Channels

#endif            

            #region Initialize GUI Components
            //initialize the interface components
            InitializeComponent();
            //Initialize GUI timers
            progressMessage += "Initializing Timers ...";
            InitializeTimers();
            progressMessage += " Completed\r\n";

            //Initialize different GUI components
            progressMessage += "Initializing GUI ...";
            InitializeInterface();
            progressMessage += " Completed\r\n";

            this.isPlotting = true;
            //count the number of accelerometers
            if (this.sensors.IsHR)
                this.maxPlots = this.sensors.Sensors.Count - 1;
            else
                this.maxPlots = this.sensors.Sensors.Count;
            SetFormPositions();
            if (this.sensors.TotalReceivers > 0)
                aMITesPlotter = new MITesScalablePlotter(this.panel1, MITesScalablePlotter.DeviceTypes.IPAQ, maxPlots, this.masterDecoder, GetGraphSize(false));
            else
                aMITesPlotter = new MITesScalablePlotter(this.panel1, MITesScalablePlotter.DeviceTypes.IPAQ, maxPlots, this.masterDecoder, GetGraphSize(false));

            //Override the resize event
#if (PocketPC)
            this.Resize += new EventHandler(OnResize);
#else
            this.form1.Resize += new EventHandler(OnResizeForm1);
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

            //Remove classifier tabs
#if (PocketPC)

            this.tabControl1.TabPages.RemoveAt(4);
            this.tabControl1.SelectedIndex = 0;
#else
            this.ShowForms();
#endif


            #endregion Initialize GUI Components



            #region Initialize Quality Tracking variables
            InitializeQuality();
            #endregion Initialize Quality Tracking variables

            #region Initialize Logging
            InitializeLogging(dataDirectory);
            #endregion Initialize Logging

            #region Initialize CSV Storage (PC Only)
#if (!PocketPC)

            //create some counters for activity counts
            averageX = new int[this.sensors.MaximumSensorID + 1];
            averageY = new int[this.sensors.MaximumSensorID + 1];
            averageZ = new int[this.sensors.MaximumSensorID + 1];

            averageRawX = new int[this.sensors.MaximumSensorID + 1];
            averageRawY = new int[this.sensors.MaximumSensorID + 1];
            averageRawZ = new int[this.sensors.MaximumSensorID + 1];

            prevX = new int[this.sensors.MaximumSensorID + 1];
            prevY = new int[this.sensors.MaximumSensorID + 1];
            prevZ = new int[this.sensors.MaximumSensorID + 1];
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
                master_csv_header += "," + category.Name;


            foreach (Sensor sensor in this.sensors.Sensors)
            {
                int sensor_id = Convert.ToInt32(sensor.ID);
                string location = sensor.Location.Replace(' ', '-');
                if (sensor_id > 0) //exclude HR
                {
                    activityCountCSVs[sensor_id] = new StreamWriter(dataDirectory + "\\MITes_" + sensor_id.ToString("00") + "_ActivityCount_" + location + ".csv");
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

            #endregion Initialize CSV Storage (PC Only)

            #region Start Collecting Data



            //if (this.sensors.TotalReceivers > 0)
            //    isStartedReceiver = true;
            //Start the built in polling thread            
#if (PocketPC)
            if (this.sensors.HasBuiltinSensors)
            {
                this.pollingThread = new Thread(new ThreadStart(this.pollingData));
                this.pollingThread.Priority = ThreadPriority.Lowest;
                this.pollingThread.Start();
            }
#endif

            //Terminate the progress thread
            progressThreadQuit = true;

           
            //Enable all timer functions
            this.readDataTimer.Enabled = true;
            this.qualityTimer.Enabled = true;
            if (this.sensors.IsHR)
                this.HRTimer.Enabled = true;

            #endregion Start Collecting Data

        }
        /*
        public MITesDataCollectionForm(string dataDirectory, string arffFile, bool isHierarchical)
        {
            int i = 0, j = 0;
            //Initialize the unix timer
            UnixTime.InitializeTime(); //passed to adjust time when its granularity is not good

            //Initialize the software mode
            this.classifyDataMode = true;
            isExtracting = false;
            isCollectingDetailedData = false;
            isPlotting = true;
            isClassifying = true;

            //Initialize the progress bar thread
            progressMessage = null;
            t = new Thread(new ThreadStart(ProgressThread));
            t.Start();

            //Initialize the interface
            InitializeComponent();
            this.dataDirectory = dataDirectory;


            //read the sensor configuration file to determine the number of receivers
            //read the activity configuration file
            progressMessage = "Loading XML protocol and sensors ...";
            AXML.Reader reader = new AXML.Reader(Constants.MASTER_DIRECTORY, dataDirectory);
#if (!PocketPC)
            if (reader.validate() == false)
            {
                throw new Exception("Error Code 0: XML format error - activities.xml does not match activities.xsd!");
            }
            else
            {
#endif
                this.annotation = reader.parse();
                this.annotation.DataDirectory = dataDirectory;


                SXML.Reader sreader = new SXML.Reader(Constants.MASTER_DIRECTORY, dataDirectory);
#if (!PocketPC)
                if (sreader.validate() == false)
                {
                    throw new Exception("Error Code 0: XML format error - sensors.xml does not match sensors.xsd!");
                }
                else
                {
#endif
                    this.sensors = sreader.parse(Constants.MAX_CONTROLLERS);

                    progressMessage += " Completed\r\n";

                    progressMessage += "Loading configuration file ...";
                    MITesFeatures.core.conf.ConfigurationReader creader = new MITesFeatures.core.conf.ConfigurationReader(dataDirectory);
                    this.configuration = creader.parse();
                    progressMessage += " Completed\r\n";
#if (!PocketPC)

                }
            }
#endif

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

#if (PocketPC)
                //setup the Bluetooth if needed
                if (this.configuration.Connection == MITesFeatures.core.conf.Constants.SOFTWARE_CONNECTION_BLUETOOTH)
                {
                    progressMessage += "Initializing Bluetooth ...";

                    progressMessage += " Completed\r\n";
                }
#endif
                progressMessage += "Initializing MITes ... searching " + this.sensors.TotalReceivers + " receivers\r\n";
                if (InitializeMITes(dataDirectory) == false)
                {
                    MessageBox.Show("Exiting: You picked a configuration with " + this.sensors.TotalReceivers + " receivers. Please make sure they are attached to the computer.");
#if (PocketPC)
                    Application.Exit();
                    System.Diagnostics.Process.GetCurrentProcess().Kill();    
#else
                    Environment.Exit(0);
#endif
                }
            }


            //Override the resize event
#if (PocketPC)
            this.Resize += new EventHandler(OnResize);
#else
            this.form1.Resize += new EventHandler(OnResizeForm1);
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


#if (PocketPC)
            #region Builtin Accelerometer Initialization
            progressMessage += "Initializing built-in sensors... \r\n";
            foreach (Sensor sensor in this.sensors.Sensors)
            {
                if (sensor.SensorClass == SXML.Constants.BUILTIN)
                {
                    progressMessage += "Initializing ... " + sensor.SensorClass + "\r\n";
                    if (sensor.Type == PhoneAccelerometers.Constants.DIAMOND_TOUCH_NAME)
                        this.htcDecoder = new PhoneAccelerometers.HTC.DiamondTouch.HTCDecoder();
                }
            }

            #endregion Builtin Accelerometer Initialization
#endif

            //Initialize the feature extractor
            Extractor.Initialize(this.mitesDecoders[0], dataDirectory, this.annotation, this.sensors, this.configuration);

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
            InitializeTimers();
            //Initializing decision tree classifiers
            progressMessage += "Initializing Decision Tree Classifiers ...";

#if (PocketPC)
            labelIndex = new Hashtable();
            instances = new Instances(new StreamReader(arffFile));
            instances.Class = instances.attribute(Extractor.ArffAttributeLabels.Length);
            classifier = new J48();
            classifier.buildClassifier(instances);
            fvWekaAttributes = new FastVector(Extractor.ArffAttributeLabels.Length + 1);
            for (i = 0; (i < Extractor.ArffAttributeLabels.Length); i++)
                fvWekaAttributes.addElement(new weka.core.Attribute(Extractor.ArffAttributeLabels[i]));

            FastVector fvClassVal = new FastVector();
            labelCounters = new int[((AXML.Category)this.annotation.Categories[0]).Labels.Count + 1];
            activityLabels = new string[((AXML.Category)this.annotation.Categories[0]).Labels.Count + 1];
            for (i = 0; (i < ((AXML.Category)this.annotation.Categories[0]).Labels.Count); i++)
            {
                labelCounters[i] = 0;
                string label = "";
                for (j = 0; (j < this.annotation.Categories.Count - 1); j++)
                    label += ((AXML.Label)((AXML.Category)this.annotation.Categories[j]).Labels[i]).Name.Replace(' ', '_') + "_";
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

            //if (isHierarchical == true)
            //    //Generate the hierarchical files
            //{
            //    Hashtable clusters = FeatureSelector.ClusterByOrientations(OrientationForm.OrientationMatrix);
            //    Hashtable clusterFeatures = FeatureSelector.GetFeaturesByMobility(clusters, MobilityForm.MobilityMatrix);
            //    this.hrClassifier = new HierarchicalClassifier(this.mitesDecoders[0], this.annotation,
            //        this.sensors, dataDirectory, this.configuration);
            //    //copy the arff file to a root file
            //    File.Copy(arffFile, dataDirectory + "\\root.arff",true);
               
            //    //String[] masterActivities = new String[clusters.Count];
            //    string[] rootFeatures = new string[Extractor.ArffAttributeLabels.Length];                
            //    //all activities included for the root
            //    ArrayList rootActivities = new ArrayList();
            //    foreach (AXML.Label label in ((AXML.Category)this.annotation.Categories[0]).Labels)                
            //        rootActivities.Add(label.Name);                
            //    foreach(DictionaryEntry entry in clusters)
            //    {
            //        int cluster_id=(int) entry.Key;
            //        ArrayList cluster=(ArrayList) entry.Value;
            //        ArrayList features=(ArrayList) clusterFeatures[cluster_id];
            //        DTClassifier dtClassifier = new DTClassifier();
 
            //        String[] acts=new String[cluster.Count];
            //        for (int k = 0; (k < cluster.Count); k++)
            //        {
            //            acts[k] = (string)SimilarActivitiesForm.ConfusingActivities[(int)cluster[k]];
            //            dtClassifier.Classes.Add(acts[k]);
            //        }

            //        ArrayList filteredFeatures=new ArrayList();
            //        for (int k = 0; (k < Extractor.AttributeLocation.Count); k++)
            //        {
            //            string featureName=Extractor.ArffAttributeLabels[k];
            //            rootFeatures[k] = featureName;
            //            if (Extractor.AttributeLocation[featureName] != null)
            //            {
            //                int featureLocation = (int)Extractor.AttributeLocation[featureName];
            //                for (int l = 0; (l < features.Count); l++)
            //                    if (featureLocation == (int)features[l])
            //                        filteredFeatures.Add(featureName);
            //            }
            //        }
            //        String[] attrs=new String[filteredFeatures.Count];
            //        for (int k = 0; (k < filteredFeatures.Count); k++)
            //        {
            //            attrs[k] = (string)filteredFeatures[k];
            //            dtClassifier.Features.Add(attrs[k]);
            //        }
                   
                   
            //        //Only generate arff subtree files when there is more than
            //        //one activity and at least one attribute to use.
            //        if ((attrs.Length > 0) && (acts.Length > 1))
            //        {
                       
            //            FilterList filter = new FilterList();
            //            filter.replaceString(dataDirectory + "\\root.arff", dataDirectory + "\\temp", "cluster" + cluster_id, acts);
            //            filter.filter(arffFile, dataDirectory + "\\cluster" + cluster_id + ".arff", attrs, acts);
            //            File.Copy(dataDirectory + "\\temp", dataDirectory + "\\root.arff",true);
                       
            //            dtClassifier.Filename = dataDirectory + "\\cluster" + cluster_id + ".arff";
            //            dtClassifier.Name = "cluster" + cluster_id;
            //            hrClassifier.Classifiers.Add(dtClassifier.Name, dtClassifier);
            //            //if a cluster is added, add it and remove its activities from the root
            //            rootActivities.Add("cluster" + cluster_id);
            //            for (int k = 0; (k < cluster.Count); k++)                        
            //                rootActivities.Remove((string)SimilarActivitiesForm.ConfusingActivities[(int)cluster[k]]);                                                  
            //        }
            //    }


            //    //root classifier        
             
            //    DTClassifier rootClassifier = new DTClassifier();
            //    rootClassifier.Filename = dataDirectory + "\\root.arff";
            //    rootClassifier.Name = "root";
            //    for (int k = 0; (k < rootActivities.Count); k++)
            //        rootClassifier.Classes.Add(rootActivities[k]);
            //    for (int k = 0; (k < rootFeatures.Length); k++)
            //        rootClassifier.Features.Add(rootFeatures[k]);
            //    hrClassifier.Classifiers.Add("root", rootClassifier);

            //    TextWriter tw = new StreamWriter(dataDirectory + "\\hierarchy.xml");
            //    tw.WriteLine(this.hrClassifier.toXML());
            //    tw.Close();
               
            //  //at this point the HR classifier is ready to be built
            //}
#else
#endif
            //Here initialize the classifier






            //Initialize the receiver threads
            bool startReceiverThreads = true;
            for (i = 0; (i < this.sensors.TotalReceivers); i++)
            {
                if ((this.mitesControllers[i] == null) || (this.mitesControllers[i].GetComPortNumber() == 0))
                    startReceiverThreads = false;
            }

            if (startReceiverThreads == true)
            {
                if (this.sensors.TotalReceivers > 0)
                    isStartedReceiver = true;
                else
                    isStartedReceiver = false;
            }
            else
            {
#if (PocketPC)
                Application.Exit();
                System.Diagnostics.Process.GetCurrentProcess().Kill();
#else
                Environment.Exit(0);
#endif

            }


            //Start the built in polling thread            
#if (PocketPC)
            if (this.sensors.HasBuiltinSensors)
            {
                this.pollingThread = new Thread(new ThreadStart(this.pollingData));
                this.pollingThread.Priority = ThreadPriority.Lowest;
                this.pollingThread.Start();
            }
#endif


            //Terminate the progress thread
            //t.Abort();
            progressThreadQuit = true;

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
        */
        #endregion Classifier constructor

        #region Initialization Methods

        //Initialize timers for the GUI interface
        public void InitializeTimers()
        {
            this.goodTimer = new ATimer(this, GOOD_TIMER);
            this.overallTimer = new ATimer(this, OVERALL_TIMER);
            this.activityTimer = new ATimer(this, ACTIVITY_TIMER);

        }

        int maxPortSearched = 1;
       
        //Initialize USB receiver channels, applies only for the MITes
        private bool InitializeUSBReceivers()
        {                  
            //Initialize all defined reception channels that use USB
            foreach (Receiver receiver in this.sensors.Receivers)
            {
                //If the reception channel is a USB channel (only applies to MITes)
                if (receiver.Type == SXML.Constants.RECEIVER_USB)
                {

                    progressMessage += "Searching for receiver " + receiver.ID + "...\r\n";
                    this.mitesControllers[receiver.ID] = new MITesReceiverController(MITesReceiverController.FIND_PORT, BYTES_BUFFER_SIZE);
                    int portNumber = MITesReceiverController.FIND_PORT;

                    try
                    {
#if (PocketPC)
                        portNumber = 9;
                        progressMessage += "Testing COM Port " + portNumber;
                        if (this.mitesControllers[receiver.ID].TestPort(portNumber, BYTES_BUFFER_SIZE))
                        {
                            progressMessage += "... Success\r\n";
                        }
                        else
                        {
                            progressMessage += "... Failed\r\n";
                            portNumber = MITesReceiverController.FIND_PORT;
                        }
#else
                    for (int j = maxPortSearched; (j < Constants.MAX_PORT); j++)
                    {
                        portNumber = maxPortSearched = j;
                        progressMessage += "Testing COM Port " + portNumber;
                        if (this.mitesControllers[receiver.ID].TestPort(portNumber, BYTES_BUFFER_SIZE))
                        {
                            progressMessage += "... Success\r\n";
                            break;
                        }
                        else
                            progressMessage += "... Failed\r\n";
                    }
#endif
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Exiting: Could not find a valid COM port with a MITes receiver!");
                        for (int j = 0; (j < this.sensors.TotalReceivers); j++)
                            this.mitesControllers[j].Close();
#if (PocketPC)
                        Application.Exit();
                        System.Diagnostics.Process.GetCurrentProcess().Kill();
#else
                    Environment.Exit(0);
#endif
                    }


                    if (portNumber == MITesReceiverController.FIND_PORT)
                    {
                        progressMessage += "Could not find a valid COM port with a MITes receiver!";
                        return false;
                    }

                    //USB only applies to MITes
                    this.mitesControllers[receiver.ID].InitializeController(portNumber, BYTES_BUFFER_SIZE, true, MITesReceiverController.USE_THREADS);
                    this.mitesDecoders[receiver.ID] = new MITesDecoder();


                    // Set the channels for a USB MITes                   
                    int[] channels = new int[6];
                    int channelCounter = 0;
                    for (int j = 0; (j < this.sensors.Sensors.Count); j++)
                    {
                        if (Convert.ToInt32(((Sensor)this.sensors.Sensors[j]).Receiver) == receiver.ID)
                        {
                            int channelID = Convert.ToInt32(((Sensor)this.sensors.Sensors[j]).ID);
#if(PocketPC)
                            if (channelID != PhoneAccelerometers.Constants.BUILT_IN_ACCELEROMETER_CHANNEL_ID)
                            {
#endif
                                channels[channelCounter] = Convert.ToInt32(((Sensor)this.sensors.Sensors[j]).ID);
                                channelCounter++;
#if (PocketPC)
                            }
#endif
                        }
                    }

                    this.mitesControllers[receiver.ID].SetChannels(this.sensors.GetNumberSensors(receiver.ID), channels);                   
                }
            }

            return true;
        }

#if (PocketPC)
        //Initialize Bluetooth receiver channels includes wockets, sparkfun, Bluetooth enabled MITes
        private bool InitializeBluetoothReceivers()
        {
            //Initialize all defined reception channels Bluetooth
            foreach (Receiver receiver in this.sensors.Receivers)
            {
                //If reception channel is of type Bluetooth and is not already initialized
                if ((receiver.Type == SXML.Constants.RECEIVER_BLUETOOTH)  && (receiver.Running == false))
                {
                    //Create a Bluetooth controller
                    progressMessage += "Initializing Bluetooth for "+receiver.Decoder+":"+receiver.MAC+" ...\r\n";
                    this.bluetoothControllers[receiver.ID] = new BluetoothController();
                    try
                    {                        
                        this.bluetoothControllers[receiver.ID].initialize(receiver.MacAddress, receiver.PassKey);
                    }
                    catch (Exception e)
                    {
                        progressMessage += "Failed to find" + receiver.Decoder + ":" + receiver.MAC + " ...\r\n";
                        return false;
                    }                   
                    this.mitesDecoders[receiver.ID] = new MITesDecoder();
                    receiver.Running = true;

                    //for MITes, we need to initialize the channels as well
                    if (receiver.Decoder == SXML.Constants.DECODER_MITES) 
                    {
                        int[] channels = new int[6];
                        int channelCounter = 0;
                        for (int j = 0; (j < this.sensors.Sensors.Count); j++)
                        {
                            if (Convert.ToInt32(((Sensor)this.sensors.Sensors[j]).Receiver) == receiver.ID)
                            {
                                int channelID = Convert.ToInt32(((Sensor)this.sensors.Sensors[j]).ID);

                                if (channelID != PhoneAccelerometers.Constants.BUILT_IN_ACCELEROMETER_CHANNEL_ID)
                                {

                                    channels[channelCounter] = Convert.ToInt32(((Sensor)this.sensors.Sensors[j]).ID);
                                    channelCounter++;

                                }

                            }
                        }
                  
                        //TODO:for bluetooth enabled mites we need to write to the BT stream the channels
                        //this.mitesControllers[i].SetChannels(this.sensors.GetNumberSensors(i), channels);
                        
                    }
                }
                
            }

            return true;
        }

#endif

#if (PocketPC)
        //Initialize Built in receiver channels includes Diamond Touch
        private bool InitializeBuiltinReceivers()
        {

        #region Builtin Accelerometer Initialization
            try
            {
                progressMessage += "Initializing built-in reception channels... \r\n";
                foreach (Sensor sensor in this.sensors.Sensors)
                {
                    if (sensor.SensorClass == SXML.Constants.BUILTIN)
                    {
                        progressMessage += "Initializing ... " + sensor.SensorClass + "\r\n";
                        if (sensor.Type == PhoneAccelerometers.Constants.DIAMOND_TOUCH_NAME)
                            this.htcDecoder = new PhoneAccelerometers.HTC.DiamondTouch.HTCDecoder();
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }

            #endregion Builtin Accelerometer Initialization

        }
#endif
        //Initializes objects that count frames for GUI reporting and quality control
        private void InitializeQuality()
        {
            aMITesActivityCounters = new Hashtable();

            //for each sensor created a counter
            for (int i = 0; (i < this.sensors.Sensors.Count); i++)
            {
                int sensor_id = Convert.ToInt32(((SXML.Sensor)this.sensors.Sensors[i]).ID);
                if (sensor_id != 0)
                    aMITesActivityCounters.Add(sensor_id, new MITesActivityCounter(this.masterDecoder, sensor_id));
            }
            aMITesHRAnalyzer = new MITesHRAnalyzer(this.masterDecoder);
            aMITesDataFilterer = new MITesDataFilterer(this.masterDecoder);

            for (int i = 0; i < MITesData.MAX_MITES_CHANNELS; i++)
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
                else if (sensor_id == MITesDecoder.MAX_CHANNEL)
                {
                    MITesDataFilterer.MITesPerformanceTracker[sensor_id].GoodRate = (int)(sensor.SamplingRate * (1 - Extractor.Configuration.MaximumNonconsecutiveFrameLoss));
                    MITesDataFilterer.MITesPerformanceTracker[sensor_id].PerfectRate = sensor.SamplingRate;
                }
                else
                {
                    int goodSamplingRate = (int)((Extractor.Configuration.ExpectedSamplingRate * (1 - Extractor.Configuration.MaximumNonconsecutiveFrameLoss)) / this.sensors.NumberSensors[receiver_id]);
                    MITesDataFilterer.MITesPerformanceTracker[sensor_id].GoodRate = goodSamplingRate;
                    MITesDataFilterer.MITesPerformanceTracker[sensor_id].PerfectRate = (int)((Extractor.Configuration.ExpectedSamplingRate) / this.sensors.NumberSensors[receiver_id]);
                }
            }
        }

        //Initialize objects for logging and storing wockets and MITes data
        private void InitializeLogging(string dataDirectory)
        {
            aMITesActivityLogger = new MITesActivityLogger(dataDirectory + "\\data\\activity\\MITesActivityData");
            aMITesActivityLogger.SetupDirectories(dataDirectory);            
            aMITesLoggerPLFormat = new MITesLoggerPLFormat(this.masterDecoder,
                                                         dataDirectory + "\\data\\raw\\PLFormat\\");
          
            aMITesActivityLogger.WriteLogComment("Application started with command line: " +
                 dataDirectory + " ");
            foreach (Receiver receiver in this.sensors.Receivers)            
                aMITesActivityLogger.WriteLogComment("Receiver"+receiver.ID+":"+receiver.Decoder+ " ");            
            foreach (SXML.Sensor sensor in this.sensors.Sensors)
                aMITesActivityLogger.WriteLogComment("Sensor" + sensor.ID + " ");            
      
        }
        //Initialize MITes Receivers
        
        private bool InitializeMITes(string dataDirectory)
        {

                //for (int i = 0; (i < this.sensors.TotalReceivers); i++)
                foreach(Receiver receiver in this.sensors.Receivers)
                {
#if (PocketPC)
                    if (receiver.Type == SXML.Constants.RECEIVER_BLUETOOTH)
                    {
                        progressMessage += "Initializing Bluetooth ...";
                        this.bluetoothControllers[receiver.ID] = new BluetoothController();                     
                        try
                        {
                            //this.bluetoothControllers[i].initialize(this.configuration.MacAddress, this.configuration.Passkey);
                            this.bluetoothControllers[receiver.ID].initialize(receiver.MacAddress, receiver.PassKey);
                        }
                        catch (Exception e)
                        {
                            progressMessage += "Could not find a valid Bluetooth Wockets receiver!";
                            return false;
                        }
                        receiver.Running = true;

                        if (receiver.Decoder==SXML.Constants.DECODER_MITES)
                            this.mitesDecoders[receiver.ID] = new MITesDecoder();
                        else if (receiver.Decoder==SXML.Constants.DECODER_WOCKETS)
                            this.mitesDecoders[receiver.ID]= new MITesDecoder();
                        else if (receiver.Decoder == SXML.Constants.DECODER_SPARKFUN)
                            this.mitesDecoders[receiver.ID] = new MITesDecoder();
                    }
                    else 
#endif                        
                    if (receiver.Type == SXML.Constants.RECEIVER_USB)
                    {

                        progressMessage += "Searching for receiver " + receiver.ID + "...\r\n";
                        this.mitesControllers[receiver.ID] = new MITesReceiverController(MITesReceiverController.FIND_PORT, BYTES_BUFFER_SIZE);
                        int portNumber = MITesReceiverController.FIND_PORT;

                        //#if (PocketPC)


                        try
                        {
#if (PocketPC)
                            portNumber = 9;
                            progressMessage += "Testing COM Port " + portNumber;
                            if (this.mitesControllers[receiver.ID].TestPort(portNumber, BYTES_BUFFER_SIZE))
                            {
                                progressMessage += "... Success\r\n";
                            }
                            else
                            {
                                progressMessage += "... Failed\r\n";
                                portNumber = MITesReceiverController.FIND_PORT;
                            }
#else
                    for (int j = maxPortSearched; (j < Constants.MAX_PORT); j++)
                    {
                        portNumber = maxPortSearched = j;
                        progressMessage += "Testing COM Port " + portNumber;
                        if (this.mitesControllers[receiver.ID].TestPort(portNumber, BYTES_BUFFER_SIZE))
                        {
                            progressMessage += "... Success\r\n";
                            break;
                        }
                        else
                            progressMessage += "... Failed\r\n";
                    }
#endif
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("Exiting: Could not find a valid COM port with a MITes receiver!");
                            for (int j = 0; (j < this.sensors.TotalReceivers); j++)
                                this.mitesControllers[j].Close();
#if (PocketPC)
                            Application.Exit();
                            System.Diagnostics.Process.GetCurrentProcess().Kill();
#else
                    Environment.Exit(0);
#endif
                        }


                        if (portNumber == MITesReceiverController.FIND_PORT)
                        {
                            progressMessage += "Could not find a valid COM port with a MITes receiver!";
                            //MessageBox.Show("Exiting: Could not find a valid COM port with a MITes receiver!");
#if (PocketPC)
                            //Application.Exit();
                            //System.Diagnostics.Process.GetCurrentProcess().Kill();    
#else
                    //Environment.Exit(0);
#endif
                            return false;
                        }
                        this.mitesControllers[receiver.ID].InitializeController(portNumber, BYTES_BUFFER_SIZE, true, MITesReceiverController.USE_THREADS);

                        this.mitesDecoders[receiver.ID] = new MITesDecoder();

                    }
                }
           // }

            aMITesActivityLogger = new MITesActivityLogger(dataDirectory + "\\data\\activity\\MITesActivityData");
            aMITesActivityLogger.SetupDirectories(dataDirectory);
            aMITesActivityCounters = new Hashtable();


            //aMITesPlotter = new MITesScalablePlotter(this.panel1, MITesScalablePlotter.DeviceTypes.IPAQ, maxPlots, this.mitesDecoders[0], GetGraphSize(false));

            //for each sensor created a counter
            for (int i = 0; (i < this.sensors.Sensors.Count); i++)
            {
                int sensor_id = Convert.ToInt32(((SXML.Sensor)this.sensors.Sensors[i]).ID);
                if (sensor_id != 0)
                    aMITesActivityCounters.Add(sensor_id, new MITesActivityCounter(this.mitesDecoders[0], sensor_id));
                    //aMITesActivityCounters.Add(sensor_id, new MITesActivityCounter(this.masterDecoder, sensor_id));
            }
            aMITesHRAnalyzer = new MITesHRAnalyzer(this.masterDecoder);//this.mitesDecoders[0]);
            aMITesDataFilterer = new MITesDataFilterer(this.masterDecoder);//this.mitesDecoders[0]);
            aMITesLoggerPLFormat = new MITesLoggerPLFormat(this.masterDecoder,//this.mitesDecoders[0],
                                                         dataDirectory + "\\data\\raw\\PLFormat\\");
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
                        int channelID=Convert.ToInt32(((Sensor)this.sensors.Sensors[j]).ID);
#if(PocketPC)
                        if (channelID != PhoneAccelerometers.Constants.BUILT_IN_ACCELEROMETER_CHANNEL_ID)
                        {
#endif
                            channels[channelCounter] = Convert.ToInt32(((Sensor)this.sensors.Sensors[j]).ID);
                            channelCounter++;
#if (PocketPC)
                        }
#endif
                    }
                }
                //Need to do the same thing for the Bluetooth
                if (this.configuration.Connection == MITesFeatures.core.conf.Constants.SOFTWARE_CONNECTION_USB)
                {
                    this.mitesControllers[i].SetChannels(this.sensors.GetNumberSensors(i), channels);
                }
            }

            return true;
        }

        
        #endregion Initialization Methods



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
                int textBoxHeight = ((int)(0.40 * this.tabPage1.ClientSize.Height) - ((this.sensors.Sensors.Count  - 1) * Constants.WIDGET_SPACING)) / num_rows;
                int textBoxWidth = ((this.tabPage1.ClientSize.Width - (3 * Constants.WIDGET_SPACING)) / 2);
                int currentTextY = (int)(this.tabPage1.Height * 0.60);
                int leftTextX = Constants.WIDGET_SPACING;
                int rightTextX = (Constants.WIDGET_SPACING * 2) + textBoxWidth;
                int currentTextX = Constants.WIDGET_SPACING;
                this.button1.Width = textBoxWidth;
                this.button1.Height = textBoxHeight;
               
                Font textFont;
                if (this.sensors.HasBuiltinSensors)
                    textFont = this.button1.Font =
                    GUI.CalculateBestFitFont(this.button1.Parent.CreateGraphics(), Constants.MIN_FONT,
                       Constants.MAX_FONT, this.button1.Size, "Diamond Touch:Still", this.button1.Font, (float)0.9, (float)0.9);
                else
                    textFont = this.button1.Font =
                    GUI.CalculateBestFitFont(this.button1.Parent.CreateGraphics(), Constants.MIN_FONT,
                       Constants.MAX_FONT, this.button1.Size, "textBoxAC11", this.button1.Font, (float)0.9, (float)0.9);

                System.Windows.Forms.Label t;
                foreach (Sensor sensor in this.sensors.Sensors)
                {

                    string labelKey = "";
                   
                    if (Convert.ToInt32(sensor.ID)==MITesDecoder.MAX_CHANNEL)
                        labelKey = sensor.Type;
                    else
                        labelKey= "MITes" + sensor.ID;

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


                //foreach (Sensor sensor in this.sensors.BuiltinSensors)
                //{

                //    string labelKey = sensor.Type;
                //    t = (System.Windows.Forms.Label)this.sensorLabels[labelKey];
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
                if (this.sensors.TotalReceivers>0)
                    aMITesPlotter = new MITesScalablePlotter(this.panel1, MITesScalablePlotter.DeviceTypes.IPAQ, maxPlots, /*this.mitesDecoders[0]*/this.masterDecoder, new Size(this.panel1.Width, (int)(0.60 * this.panel1.Height)));
                else
                    aMITesPlotter = new MITesScalablePlotter(this.panel1, MITesScalablePlotter.DeviceTypes.IPAQ, maxPlots, this.masterDecoder, new Size(this.panel1.Width, (int)(0.60 * this.panel1.Height)));
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
                this.label7.Font = this.label8.Font = this.label9.Font = textFont;
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
                Form f = new Form();
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
                this.readDataTimer.Enabled = false;
                this.qualityTimer.Enabled = false;

                if (MainForm.SelectedForm != Constants.MAIN_SELECTED_ANNOTATION)
                {
                    if (this.configuration.Connection == MITesFeatures.core.conf.Constants.SOFTWARE_CONNECTION_BLUETOOTH)
                    {
                        for (int i = 0; (i < this.sensors.TotalReceivers); i++)
                        {
#if (PocketPC)
                            if (this.bluetoothControllers[i] != null)
                            {
                                Thread.Sleep(100);
                                this.bluetoothControllers[i].cleanup();
                                Thread.Sleep(1000);
                            }    
#endif                        
                            Thread.Sleep(100);
                        }
                    }
                    else if (this.configuration.Connection == MITesFeatures.core.conf.Constants.SOFTWARE_CONNECTION_USB)
                    {
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
                    }

#if (PocketPC)

                    if (this.sensors.HasBuiltinSensors)
                    {
                        this.htcDecoder.isQuitting = true;
                        Thread.Sleep(100);
                        this.pollingThread.Abort();
                    }
                    aMITesLoggerPLFormat.FlushBytes();
                    aMITesLoggerPLFormat.ShutdownFiles();
                    //ShutdownFiles()
#else
                foreach (Sensor sensor in this.sensors.Sensors)
                {
                    int sensor_id = Convert.ToInt32(sensor.ID);
                    if (sensor_id > 0)
                    {
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
                    if (this.sensors.TotalReceivers > 0)
                        Extractor.Cleanup();
                }



#if (PocketPC)

                Application.Exit();
                System.Diagnostics.Process.GetCurrentProcess().Kill();    
               
#else
                Environment.Exit(0);
#endif
            }
        }

        private void menuItem22_Click(object sender, EventArgs e)
        {
            if (this.isCollectingDetailedData == true)
            {
                this.isCollectingDetailedData = false;
                this.menuItem22.Checked = false;
                aMITesLoggerPLFormat.SetActive(false);

            }
            else
            {
                this.isCollectingDetailedData = true;
                this.menuItem22.Checked = true;
                aMITesLoggerPLFormat.SetActive(true);

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
#if (PocketPC)
                else if (control_id == ACTIVITY_TIMER)
                {
                    pieChart.SetTime(label);
                    pieChart.Invalidate();
                }
#endif
            }
        }


        public void SetErrorLabel(string label)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (((System.Windows.Forms.Label)this.sensorLabels["ErrorLabel"]).InvokeRequired)
            {
                SetErrorCallback d = new SetErrorCallback(SetErrorLabel);
                this.Invoke(d, new object[] { label});
            }
            else
            {
                ((System.Windows.Forms.Label)this.sensorLabels["ErrorLabel"]).Text = label;
                ((System.Windows.Forms.Label)this.sensorLabels["ErrorLabel"]).Refresh();
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
#if (PocketPC)
                if (sensor_id == MITesDecoder.MAX_CHANNEL) //builtin sensor
                {
                    string key = sensor.Type;
                    double result = this.htcDecoder.ActivityCount;
                    if (result == 0)
                        ((System.Windows.Forms.Label)this.sensorLabels[key]).Text = sensor.Type + ": none";
                    else
                    {
                        ((System.Windows.Forms.Label)this.sensorLabels[key]).Text = sensor.Type + ": " + Math.Round(result, 2);

                        if (result < 3.0)
                            ((System.Windows.Forms.Label)this.sensorLabels[key]).Text = sensor.Type + ": still";
                    }
                }
                else 
#endif                    
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
            //#if (PocketPC)
            //            foreach (Sensor sensor in this.sensors.BuiltinSensors)
            //            {
            //                string key = sensor.Type;
            //                double result = this.htcDecoder.ActivityCount;
            //                if (result == 0)
            //                    ((System.Windows.Forms.Label)this.sensorLabels[key]).Text = sensor.Type + ": none";
            //                else
            //                {
            //                    ((System.Windows.Forms.Label)this.sensorLabels[key]).Text = sensor.Type + ": " + Math.Round(result, 2);

            //                    if (result < 3.0)
            //                        ((System.Windows.Forms.Label)this.sensorLabels[key]).Text = sensor.Type + ": still";
            //                }
            //            }
            //#endif
        }

        #endregion Helper Methods






#if (!PocketPC)
        void form_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isQuitting == false)
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
            //this.form5.Show();
            this.form1.DesktopLocation = this.form1.Location = new Point(Constants.SCREEN_LEFT_MARGIN, Constants.SCREEN_TOP_MARGIN);
            this.form3.DesktopLocation = this.form3.Location = new Point(Constants.SCREEN_LEFT_MARGIN, Constants.SCREEN_TOP_MARGIN + Constants.SCREEN_TOP_MARGIN + this.form1.Height);
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
            aMITesPlotter = new MITesScalablePlotter(this.panel1, MITesScalablePlotter.DeviceTypes.IPAQ, maxPlots, /*this.mitesDecoders[0]*/this.masterDecoder, GetGraphSize(true));
        }

        private void SetPlotterPartialScreen()
        {
            aMITesPlotter = new MITesScalablePlotter(this.panel1, MITesScalablePlotter.DeviceTypes.IPAQ, maxPlots, /*this.mitesDecoders[0]*/this.masterDecoder, GetGraphSize(false));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="pevent"></param>
        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
        }

        private void GraphAccelerometerValues(GenericAccelerometerData data)
        {
            //if (isStartedReceiver)
                aMITesPlotter.SetAccelResultsData();

            if (this.sensors.IsHR)
                aMITesPlotter.setPlotVals(data, this.sensors.Sensors.Count - this.sensors.TotalBuiltInSensors-1);
            else
                aMITesPlotter.setPlotVals(data, this.sensors.Sensors.Count - this.sensors.TotalBuiltInSensors);            
        }
        private void GraphAccelerometerValues()
        {
            aMITesPlotter.SetAccelResultsData();
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
                //this.builtInDataReady = false;
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
                double goodRate = (1 - Extractor.Configuration.MaximumNonconsecutiveFrameLoss) * 100;

                foreach (SXML.Sensor sensor in this.sensors.Sensors)
                {
                    int sensor_id = Convert.ToInt32(sensor.ID);

#if (PocketPC)
                    if (sensor_id == MITesDecoder.MAX_CHANNEL) //BUILTIN
                    {
                        double rate = ((double)this.htcDecoder.SamplingRate) * 100 / sensor.SamplingRate;
                        if (rate > 100)
                            rate = 100;
                        else if (rate < 0)
                            rate = 0;
                        this.expectedLabels[sensor_id].Text = rate.ToString("00.00") + "%";
                        this.samplesPerSecond[sensor_id].Text = this.htcDecoder.SamplingRate.ToString() + "/" + sensor.SamplingRate.ToString();

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
                    else
#endif
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
            

#if (PocketPC)

                if (this.tabControl1.TabIndex == 0)
                {
#endif
                    ReportActivityCounts();
                    ReportHR();

#if (PocketPC)
                }
#endif
       
            }


   
        #region Builtin Accelerometr Polling Thread
#if (PocketPC)
        private bool unprocessedBuiltin = false;
        private GenericAccelerometerData builtinData;
        private string previousActivity = "";
        private void pollingData()
        {
            while (true)
            {
                if ((unprocessedBuiltin==false) && (Environment.TickCount - pollingTime >= 60))
                {

                    this.builtinData = this.htcDecoder.PollBuiltInSensors();
                    pollingTime = Environment.TickCount;
                    unprocessedBuiltin = true;
                }
                Thread.Sleep(50);
            }
        }
#endif
        #endregion Builtin Accelerometr Polling Thread


        private int totalCalories = 0;
        private int currentCalories = 0;
        private int ttwcounter = 0;
        private bool btRestablished = false;



    
        private void readDataTimer_Tick(object sender, EventArgs e)
        {
                    
            #region Bluetooth Reconnection Code
#if (PocketPC)



            //Start a reconnection thread
            foreach (Receiver receiver in this.sensors.Receivers)
            {
                if (!receiver.Running) //&& (!receiver.Restarting))
                {
                    aMITesActivityLogger.WriteLogComment("Connection broke with " + receiver.ID + ". Restarting");   
                    //reconnectionThreadQuit[receiver.ID] = false;

                    BluetoothConnector btc=new BluetoothConnector(receiver, this.bluetoothControllers, this.mitesDecoders);
                    aMITesActivityLogger.WriteLogComment("Initializing reconnection thread");   
                    //ts[receiver.ID] = new Thread(new ThreadStart(btc.Reconnect));
                    //ts[receiver.ID].Start();
                    btc.Reconnect();
                    aMITesActivityLogger.WriteLogComment("Reconnection thread started");     
                    receiver.Restarting = true;
                  
                }
                else if (receiver.Restarted)
                {               

                    aMITesActivityLogger.WriteLogComment("Reconnection completed"); 
                    receiver.Restarted = false;
                    receiver.Restarting = false;
                    receiver.Running = true;
                }

            }


#endif

                #endregion Bluetooth Reconnection Code

            #region Poll All Wockets and MITes and Decode Data
       
            Receiver currentReceiver = null;
            try
            {
                //Poll each CONNECTED receiver channel with the right decoder
                foreach (Receiver receiver in this.sensors.Receivers)
                {
                    currentReceiver = receiver;
#if (PocketPC)
                    if ((receiver.Type == SXML.Constants.RECEIVER_BLUETOOTH) && (receiver.Running == true))
                    {
                        if (receiver.Decoder == SXML.Constants.DECODER_MITES)
                            this.mitesDecoders[receiver.ID].GetSensorData(this.bluetoothControllers[receiver.ID], MITesDecoder.MITES_SENSOR);
                        else if (receiver.Decoder == SXML.Constants.DECODER_WOCKETS)
                            this.mitesDecoders[receiver.ID].GetSensorData(this.bluetoothControllers[receiver.ID], MITesDecoder.WOCKETS_SENSOR);
                        else if (receiver.Decoder == SXML.Constants.DECODER_SPARKFUN)
                            this.mitesDecoders[receiver.ID].GetSensorData(this.bluetoothControllers[receiver.ID], MITesDecoder.SPARKFUN_SENSOR);
                    }
                    else 
#endif                        
                        if (receiver.Type == SXML.Constants.RECEIVER_USB)
                        this.mitesDecoders[receiver.ID].GetSensorData(this.mitesControllers[receiver.ID]);
                }

                if (++ttwcounter == 6000)
                {
                    TextWriter ttw = new StreamWriter("\\Internal Storage\\ts.txt");
                    ttw.WriteLine(DateTime.Now.ToLongTimeString());
                    ttw.Close();
                    ttwcounter = 0;
                }

            }
            //Thrown when there is a Bluetooth failure                    
            //TODO: Make sure no USB failure happening
            catch (Exception ex)
            {

                currentReceiver.Running = false;
                //connectionLost = true;
                return;
            }

            //Reset the index of the master decoder and copy all data in order into the master decoder
            this.masterDecoder.someMITesDataIndex = 0;    
            //Only decode running receivers (i.e. with no connection failure)              
            foreach (Receiver receiver in this.sensors.Receivers)
                if (receiver.Running)
                    this.masterDecoder.MergeDataOrderProperly(this.mitesDecoders[receiver.ID]);


            #endregion Poll All Wockets and MITes and Decode Data

            #region Poll Builtin Data

#if (PocketPC)
            GenericAccelerometerData polledData = null;
            if (unprocessedBuiltin == true)
            {
                this.builtinData.Timestamp = Environment.TickCount;
                this.builtinData.Unixtimestamp = UnixTime.GetUnixTime();
                polledData = this.builtinData;
            }
#endif
            #endregion Poll Builtin Data

            #region Train in realtime and generate ARFF File
            if (IsTraining == true)
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
                        this.startActivityTime = Environment.TickCount + Extractor.Configuration.TrainingWaitTime * 1000;//Constants.TRAINING_GAP;
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
#if (PocketPC)
                            PlaySound(@"\Windows\Voicbeep", IntPtr.Zero, (int)(PlaySoundFlags.SND_FILENAME | PlaySoundFlags.SND_SYNC));
                            PlaySound(@"\Windows\Voicbeep", IntPtr.Zero, (int)(PlaySoundFlags.SND_FILENAME | PlaySoundFlags.SND_SYNC));
#endif
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
#if (PocketPC)
                        if ((this.sensors.HasBuiltinSensors) && (polledData != null))
                        {
                            //store it in Extractor Buffers as well
                            lastTimeStamp = Extractor.StoreBuiltinData(polledData);
                        }
#endif
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

#if (PocketPC)
            if (isClassifying == true)
            {
                double lastTimeStamp = Extractor.StoreMITesWindow();
                if ((this.sensors.HasBuiltinSensors) && (polledData != null))
                {
                    //aMITesLoggerPLFormat.SaveRawMITesBuiltinData(polledData);
                    //store it in Extractor Buffers as well
                    lastTimeStamp = Extractor.StoreBuiltinData(polledData);
                }

                if (Extractor.GenerateFeatureVector(lastTimeStamp))
                {
                    Instance newinstance = new Instance(instances.numAttributes());
                    newinstance.Dataset = instances;
                    for (int i = 0; (i < Extractor.Features.Length); i++)
                        newinstance.setValue(instances.attribute(i), Extractor.Features[i]);
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
                            if (labelCounters[j] > mostCount)
                            {
                                mostActivity = activityLabels[j];
                                mostCount = labelCounters[j];
                            }
                            labelCounters[j] = 0;
                        }

                        pieChart.SetActivity(mostActivity);
                        if (this.aList.getEmptyPercent() == 1)
                            this.aList.reset();
                        else
                            this.aList.increment(mostActivity);

                        if (previousActivity != mostActivity)
                        {
                            this.activityTimer.stop();
                            this.activityTimer.reset();
                            currentCalories = 0;
                        }
                        else
                        {
                            if (this.activityTimer.isRunning() == false)
                                this.activityTimer.start();
                        }

                        if (mostActivity == "standing")
                        {
                            currentCalories += 1;
                            totalCalories += 1;
                        }
                        else if (mostActivity == "walking")
                        {
                            currentCalories += 2;
                            totalCalories += 2;
                        }
                        else if (mostActivity == "brisk-walking")
                        {
                            currentCalories += 4;
                            totalCalories += 4;
                        }
                        else
                        {
                            currentCalories += 1;
                            totalCalories += 1;
                        }
                        pieChart.SetCalories(totalCalories, currentCalories);
                        pieChart.Data = this.aList.toPercentHashtable();
                        pieChart.Invalidate();
                        previousActivity = mostActivity;
                    }
                }
            }

#endif


            #endregion Classifying activities

            #region Storing CSV data for the grapher (PC Only)
#if (!PocketPC)

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

            #endregion Storing CSV data for the grapher (PC Only)

            #region Calibration and CSV Calculateion Code
            if ((isCalibrating) || (isCollectingDetailedData == true))
            {

                //store sum of abs values of consecutive accelerometer readings
                for (int i = 0; (i < this.masterDecoder.someMITesDataIndex); i++)
                {
                    if ((this.masterDecoder.someMITesData[i].type != (int)MITesTypes.NOISE) &&
                          (this.masterDecoder.someMITesData[i].type == (int)MITesTypes.ACCEL))
                    {
                        int channel = 0, x = 0, y = 0, z = 0;
                        channel = (int)this.masterDecoder.someMITesData[i].channel;
                        x = (int)this.masterDecoder.someMITesData[i].x;
                        y = (int)this.masterDecoder.someMITesData[i].y;
                        z = (int)this.masterDecoder.someMITesData[i].z;


                        #region Calibration Calculation
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
#if (PockectPC)                          
                                    if (this.calSensor != PhoneAccelerometers.Constants.BUILT_IN_ACCELEROMETER_CHANNEL_ID)
                                    {
#endif
                                    channels[0] = this.calSensor;
                                    this.mitesControllers[receiver_id].SetChannels(1, channels);
#if (PockectPC)  
                                    }
#endif
                                }
                                else //all sensors are calibrated
                                {
                                    TextWriter tw = new StreamWriter("SensorData.xml");
                                    tw.WriteLine(this.sensors.toXML());
                                    tw.Close();
                                    MessageBox.Show("Calibration... Completed");
#if (PocketPC)
                                    Application.Exit();
                                    System.Diagnostics.Process.GetCurrentProcess().Kill();
#else
                                    Environment.Exit(0);
#endif
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
                        #endregion Calibration Calculation

                        #region CSV values calculation (PC Only)
#if (!PocketPC)
                        else if (isCollectingDetailedData)
                        {

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

                        }
#endif
                        #endregion CSV values calculation (PC Only)
                    }
                }
            }
            #endregion Calibration and CSV Calculateion Code

            //Remove data with any values =0 or 1022
            aMITesDataFilterer.RemoveZeroNoise();

            this.masterDecoder.UpdateSamplingRate(aMITesDataFilterer.CountNonNoise());

            if (printSamplingCount > 500)
            {
                //((System.Windows.Forms.Label)this.sensorLabels["SampRate"]).Text = "Samp: " + this.mitesDecoders[0].GetSamplingRate();
                ((System.Windows.Forms.Label)this.sensorLabels["SampRate"]).Text = "Samp: ttt";//+this.masterDecoder.GetSamplingRate();
                //textBoxRate.Text = "Samp: " + aMITesDecoder.GetSamplingRate();
                //aMITesLogger.WriteLogComment(textBoxUV.Text);
                printSamplingCount = 0;
            }
            else
                printSamplingCount++;



            // Check HR values
            int hr = aMITesHRAnalyzer.Update();
#if (PocketPC)

#else
            if (hr > 0)
            {
                sumHR += hr;
                hrCount++;
            }
#endif



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
           

            #region Store the sensor data



            if (flushTimer == 0)
                aMITesLoggerPLFormat.FlushBytes();
            if (flushTimer > FLUSH_TIMER_MAX)
                flushTimer = -1;
            flushTimer++;

            #endregion Store the sensor data


            // Graph accelerometer data for multiple recievers
#if (PocketPC)
            if (isPlotting)
                GraphAccelerometerValues(polledData);
#endif


#if (PocketPC)
            if (polledData != null)
            {
                this.builtinData = null;
                this.unprocessedBuiltin = false;
            }
#endif

        }

     
        #endregion Timer Methods








    }

}
