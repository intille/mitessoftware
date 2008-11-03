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

            //the forms that are linked to choices on the MainForm
            Form[] nextForms = new Form[4];
            nextForms[0] = activityProtocolForm;
            nextForms[1] = buildModelFeatureForm;
            nextForms[2] = troubleshootModelForm;
            nextForms[3] = calibrateSensorsForm;


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
            if (MainForm.SelectedForm == Constants.MAIN_SELECTED_COLLECT_DATA)
            {
                //check all required paramters were selected
                if ((ActivityProtocolForm.SelectedProtocol==null)||(ActivityProtocolForm.SelectedProtocol.FileName == "") ||(SensorConfigurationForm.SelectedSensors==null)|| (SensorConfigurationForm.SelectedSensors.FileName == "") ||
                    (WhereStoreDataForm.SelectedFolder == "") || (WhereStoreDataForm.SelectedFolder==null))
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
                           WhereStoreDataForm.SelectedFolder + "\\" + AXML.Reader.DEFAULT_XML_FILE);

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
                             WhereStoreDataForm.SelectedFolder + "\\" + SXML.Reader.DEFAULT_XML_FILE);
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
                            WhereStoreDataForm.SelectedFolder + "\\" + MITesFeatures.core.conf.ConfigurationReader.DEFAULT_XML_FILE);
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
                            WhereStoreDataForm.SelectedFolder + "\\" + ActivitySummary.Reader.DEFAULT_XML_FILE);
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
                    Application.Run(new MITesDataCollectionForm(WhereStoreDataForm.SelectedFolder));
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