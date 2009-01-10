using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Collections;

namespace MITesDataCollection
{
    static class Program
    {

        /*
        private static MainFormManager mainFormManager;

        public static MainFormManager MainFormManager
        {
            get { return mainFormManager; }
        }

        public static Form _Form1;
        public static Form _Form_w_1;



        /// <summary>
        /// The main entry point for the application.
        /// </summary>

        [MTAThread]
        static void Main()
        {
            mainFormManager = new MainFormManager();

            MainFormManager.CurrentForm = new Form_w_0();
            Application.Run(MainFormManager.CurrentForm);

        }
        */
       
        private static bool isDirectoryEmpty(string path)
        {
            string[] subDirs = Directory.GetDirectories(path);
            if (0 == subDirs.Length)
            {
                string[] files = Directory.GetFiles(path);
                return (0 == files.Length);
            }
            return false;
        }
        
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
       // [STAThread]
        
    static void Main()
    {

        

        //Create forms that query the program to setup the software depending
        //on user choices
        MainForm mainForm = new MainForm();
        ActivityProtocolForm activityProtocolForm = new ActivityProtocolForm();
        SensorConfigurationForm sensorConfigurationForm = new SensorConfigurationForm();
        BuildModelFeatureForm buildModelFeatureForm = new BuildModelFeatureForm();            
        WhereStoreDataForm whereStoreDataForm = new WhereStoreDataForm();
        TroubleshootModel troubleshootModelForm = new TroubleshootModel();
        SimilarActivitiesForm similarActivitiesForm = new SimilarActivitiesForm();
        MobilityForm mobilityForm = new MobilityForm();
        OrientationForm orientationForm = new OrientationForm();         
        CalibrateSensors calibrateSensorsForm = new CalibrateSensors();
        CalibrateSensor calibrateSensorForm = new CalibrateSensor();
        ActivityProtocolForm annotationActivityProtocolForm = new ActivityProtocolForm();
        SensorConfigurationForm annotationSensorConfigurationForm = new SensorConfigurationForm();

                       
        
        //Forms that are used to collect MITes Data
        activityProtocolForm.PreviousForm = mainForm;
        activityProtocolForm.NextForm = sensorConfigurationForm;
        sensorConfigurationForm.PreviousForm = activityProtocolForm;
        sensorConfigurationForm.NextForm = whereStoreDataForm;
        whereStoreDataForm.PreviousForm = sensorConfigurationForm;
        whereStoreDataForm.NextForm = null;
        buildModelFeatureForm.PreviousForm = mainForm;
        buildModelFeatureForm.NextForm = null;

           
        //Forms that are used to troubleshoot a model and reconstruct
        //a hierarchical tree
        troubleshootModelForm.NextForm = similarActivitiesForm;
        troubleshootModelForm.PreviousForm = mainForm;
        similarActivitiesForm.PreviousForm = troubleshootModelForm;
        similarActivitiesForm.NextForm = mobilityForm;
        mobilityForm.PreviousForm = similarActivitiesForm;
        mobilityForm.NextForm = orientationForm;
        orientationForm.PreviousForm = mobilityForm;
        orientationForm.NextForm = null;


        //Calibration forms
        calibrateSensorsForm.NextForm = calibrateSensorForm;
        calibrateSensorsForm.PreviousForm = mainForm;
        calibrateSensorForm.NextForm = null;
        calibrateSensorForm.PreviousForm = calibrateSensorsForm;

        //forms that are used for annotation only
        annotationActivityProtocolForm.PreviousForm = mainForm;
        annotationActivityProtocolForm.NextForm = annotationSensorConfigurationForm;
        annotationSensorConfigurationForm.PreviousForm = annotationActivityProtocolForm;
        annotationSensorConfigurationForm.NextForm = null;

        //the forms that are linked to choices on the MainForm
        Form[] nextForms = new Form[5];
        nextForms[0] = activityProtocolForm;
        nextForms[1] = buildModelFeatureForm;
        nextForms[2] = troubleshootModelForm;
        nextForms[3] = calibrateSensorsForm;
        nextForms[4] = annotationActivityProtocolForm;


        mainForm.SetNextForms(nextForms);

        mainForm.ShowDialog();
 

        //Once the user makes the choices, cleanup all the forms and close them            
        activityProtocolForm.Close();
        sensorConfigurationForm.Close();          
        buildModelFeatureForm.Close();
        whereStoreDataForm.Close();

   #if(PocketPC)
        calibrateSensorsForm.Close();
        calibrateSensorForm.Close();
#else
#endif
        //choice #1: collect data
        if ( (MainForm.SelectedForm == Constants.MAIN_SELECTED_COLLECT_DATA) ||
            (MainForm.SelectedForm == Constants.MAIN_SELECTED_ANNOTATION))
        {
      


            string destinationFolder = "";
            if (MainForm.SelectedForm == Constants.MAIN_SELECTED_COLLECT_DATA)
                destinationFolder = WhereStoreDataForm.SelectedFolder;
            else
            {
                //Create the storage directory if it does not exist
                if (Directory.Exists(Constants.DEFAULT_DATA_STORAGE_DIRECTORY) == false)
                    Directory.CreateDirectory(Constants.DEFAULT_DATA_STORAGE_DIRECTORY);

                //Check if the directory is empty
                if (isDirectoryEmpty(Constants.DEFAULT_DATA_STORAGE_DIRECTORY) == false)
                {
                    MessageBox.Show("Please delete the content of " + Constants.DEFAULT_DATA_STORAGE_DIRECTORY + ".");
                    Application.Exit();
                    System.Diagnostics.Process.GetCurrentProcess().Kill();
                }
                destinationFolder = Constants.DEFAULT_DATA_STORAGE_DIRECTORY;
            }
           
            //check all required paramters were selected
            if ((ActivityProtocolForm.SelectedProtocol==null)||(ActivityProtocolForm.SelectedProtocol.FileName == "") ||(SensorConfigurationForm.SelectedSensors==null)|| (SensorConfigurationForm.SelectedSensors.FileName == "") ||
                (destinationFolder == "") || (destinationFolder == null))
            {
                MessageBox.Show("Exiting: You need to select an activity protocol, sensor configuration and a directory to store your data");
#if (PocketPC)
                Application.Exit();
                System.Diagnostics.Process.GetCurrentProcess().Kill();    
#else
                    Environment.Exit(0);
#endif
            }
            else
            {
                //Copy the activity protocol to the data directory
                try
                {
                    File.Copy(Constants.ACTIVITY_PROTOCOLS_DIRECTORY + ActivityProtocolForm.SelectedProtocol.FileName,
                       destinationFolder + "\\" + AXML.Reader.DEFAULT_XML_FILE);

                }
                catch (Exception)
                {
                    MessageBox.Show("Exiting: Please choose an empty storage directory");
#if (PocketPC)
                    Application.Exit();
                    System.Diagnostics.Process.GetCurrentProcess().Kill();    
#else
                    Environment.Exit(0);
#endif
                }

                //Copy the sensor configuration to the data directory
                try
                {
                    File.Copy(Constants.SENSOR_CONFIGURATIONS_DIRECTORY + SensorConfigurationForm.SelectedSensors.FileName,
                         destinationFolder + "\\" + SXML.Reader.DEFAULT_XML_FILE);
                }
                catch (Exception)
                {
                    MessageBox.Show("Exiting: Please choose an empty storage directory");
#if (PocketPC)
                    Application.Exit();
                    System.Diagnostics.Process.GetCurrentProcess().Kill();    
#else
                    Environment.Exit(0);
#endif
                }

                //Copy the configuration file to the data directory
                try
                {
                    File.Copy(Constants.MASTER_DIRECTORY + MITesFeatures.core.conf.ConfigurationReader.DEFAULT_XML_FILE,
                        destinationFolder + "\\" + MITesFeatures.core.conf.ConfigurationReader.DEFAULT_XML_FILE);
                }
                catch (Exception)
                {
                    MessageBox.Show("Exiting: Please choose an empty storage directory");
#if (PocketPC)
                    Application.Exit();
                    System.Diagnostics.Process.GetCurrentProcess().Kill();    
#else
                    Environment.Exit(0);
#endif
                }

#if(PocketPC)
                try
                {
                    File.Copy(Constants.MASTER_DIRECTORY + ActivitySummary.Reader.DEFAULT_XML_FILE,
                        destinationFolder + "\\" + ActivitySummary.Reader.DEFAULT_XML_FILE);
                }
                catch (Exception)
                {
                    MessageBox.Show("Exiting: Please choose an empty storage directory");
#if (PocketPC)
                    Application.Exit();
                    System.Diagnostics.Process.GetCurrentProcess().Kill();
#else
                    Environment.Exit(0);
#endif
                }

#endif
                if (MainForm.SelectedForm==Constants.MAIN_SELECTED_COLLECT_DATA)
                    Application.Run(new MITesDataCollectionForm(WhereStoreDataForm.SelectedFolder));
                else 
                    Application.Run(new MITesDataCollectionForm());
            }
        }

        //Choice #2: Load an existing model and estimate energy expenditure
        else if (MainForm.SelectedForm == Constants.MAIN_SELECTED_ESTIMATE_ENERGY)            
            Application.Run(new MITesDataCollectionForm(BuildModelFeatureForm.SelectedFolder,BuildModelFeatureForm.SelectedFile,false));
            
        //Choice #3: Troubleshoot the model by reconstructing a hierarchical decision tree
        //based on user responses
        else if (MainForm.SelectedForm == Constants.MAIN_SELECTED_TROUBLESHOOT)
            Application.Run(new MITesDataCollectionForm(TroubleshootModel.SelectedFolder, TroubleshootModel.SelectedFile, true));      
            //Choice #4: Calibrate the sensors in a setup
        else if (MainForm.SelectedForm == Constants.MAIN_SELECTED_CALIBRATE)
            Application.Run(new MITesDataCollectionForm(CalibrateSensors.Sensors,"/test"));


#if (PocketPC)
        Application.Exit();
        System.Diagnostics.Process.GetCurrentProcess().Kill();    
#else
        Environment.Exit(0);
#endif
        
    }

        
    }
}