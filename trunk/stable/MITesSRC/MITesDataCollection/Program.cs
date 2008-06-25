using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Threading;

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

            //Application.Run(new ProgressForm());
            MainForm mainForm = new MainForm();
            ActivityProtocolForm activityProtocolForm = new ActivityProtocolForm();
            SensorConfigurationForm sensorConfigurationForm = new SensorConfigurationForm();
            BuildModelFeatureForm buildModelFeatureForm = new BuildModelFeatureForm();            
            WhereStoreDataForm whereStoreDataForm = new WhereStoreDataForm();
        
            activityProtocolForm.PreviousForm = mainForm;
            activityProtocolForm.NextForm = sensorConfigurationForm;
            sensorConfigurationForm.PreviousForm = activityProtocolForm;
            sensorConfigurationForm.NextForm = whereStoreDataForm;
            whereStoreDataForm.PreviousForm = sensorConfigurationForm;
            whereStoreDataForm.NextForm = null;
            
            Form[] nextForms = new Form[2];
            nextForms[0] = activityProtocolForm;
            nextForms[1] = buildModelFeatureForm;
            mainForm.SetNextForms(nextForms);

            mainForm.ShowDialog();
 

            //cleanup all forms
            activityProtocolForm.Close();
            sensorConfigurationForm.Close();          
            buildModelFeatureForm.Close();
            whereStoreDataForm.Close();
            
            if (MainForm.SelectedForm == Constants.MAIN_SELECTED_COLLECT_DATA)
            {
                //check all required paramters were selected
                if ((ActivityProtocolForm.SelectedProtocol.FileName == "") || (SensorConfigurationForm.SelectedSensors.FileName == "") ||
                    (WhereStoreDataForm.SelectedFolder == ""))
                {
                    MessageBox.Show("Exiting: You need to select an activity protocol, sensor configuration and a directory to store your data");
#if (PocketPC)
                        Application.Exit();
#else
                        Environment.Exit(0);
#endif
                }
                else
                {
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
#else
                        Environment.Exit(0);
#endif
                    }

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
#else
                        Environment.Exit(0);
#endif
                    }

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
#else
                        Environment.Exit(0);
#endif
                    }


                   // t.Abort();
                    //progressForm.Close();
                    Application.Run(new MITesDataCollectionForm(WhereStoreDataForm.SelectedFolder));
                }
            }




            //copy the files to the folder
            






            
        }
    }
}