using System;
using System.Collections.Generic;
using System.Text;

namespace MITesDataCollection
{
    class Constants
    {
        //General constants
        public static int SCREEN_WIDTH_MARGIN = 0;
        public static int SCREEN_HEIGHT_MARGIN = 0;
        public static int WIDGET_SPACING = 1;
        public static int FORM_MIN_WIDTH = 30;
        public static int FORM_MIN_HEIGHT = 30;
        public static string FONT_FAMILY = "Microsoft Sans Serif";
        public static int MIN_FONT = 6;
        public static int MAX_FONT = 64;
        public static string MITES_DATA_STORAGE_DIRECTORY = "\\SD-MMCard\\MITes\\";

        //Main Form Constants

        public static int MAIN_FORM_WIDGETS_COUNT = 3;
        public static string MAIN_FORM_LABEL1 = "What do you want to do?";
        public static string MAIN_FORM_BUTTON1 = "Collect MITes data";
        public static string MAIN_FORM_BUTTON2 = "Estimate Energy";
        public static string MAIN_FORM_BUTTON3 = "Correct Software";
        public static int MAIN_SELECTED_COLLECT_DATA = 0;
        public static int MAIN_SELECTED_ESTIMATE_ENERGY=1;

        //Activity FORM Placement
        public static string ACTIVITY_FORM_LABEL1 = "Choose an activity protocol:";
        public static double ACTIVITY_LIST_HEIGHT_PERCENTAGE = 0.60;
        public static double ACTIVITY_LABEL_PERCENTAGE = 0.15;
        public static double ACTIVITY_BUTTONS_HEIGHT_PERCENTAGE = 0.20;
        public static double ACTIVITY_BUTTONS_WIDTH_PERCENTAGE = 0.45;
        public static double ACTIVITY_LIST_WIDTH_PERCENTAGE = 0.90;


        //Sensor FORM Placement
        public static string SENSOR_FORM_LABEL1 = "Choose a sensor configuration:";
        public static double SENSOR_LIST_HEIGHT_PERCENTAGE = 0.60;
        public static double SENSOR_LABEL_PERCENTAGE = 0.15;
        public static double SENSOR_BUTTONS_HEIGHT_PERCENTAGE = 0.20;
        public static double SENSOR_BUTTONS_WIDTH_PERCENTAGE = 0.45;
        public static double SENSOR_LIST_WIDTH_PERCENTAGE = 0.90;

        //WhereToStore FORM Placement
        public static string WHERE_FORM_LABEL1 = "Where do you want to store your data?";
        public static int WHERE_FORM_WIDGETS_COUNT = 4;
        public static double WHERE_BUTTONS_HEIGHT_PERCENTAGE = 1.0;
        public static double WHERE_BUTTONS_WIDTH_PERCENTAGE = 0.50;


        //PATH Constants
#if (PocketPC)
        public static string NEEDED_FILES_PATH = "\\Program Files\\mitesdatacollection\\NeededFiles\\"; //fullpath required for pocketpc
#else 
        public static string NEEDED_FILES_PATH = "..\\NeededFiles\\"; //relative to bin
#endif 
        
        public static string MASTER_DIRECTORY = NEEDED_FILES_PATH + "Master\\";
        public static string ACTIVITY_PROTOCOLS_DIRECTORY = NEEDED_FILES_PATH + "ActivityProtocols\\";
        public static string SENSOR_CONFIGURATIONS_DIRECTORY = NEEDED_FILES_PATH + "SensorConfigurations\\";
        public static string ACTIVITY_COUNT_FILENAME = "activity_count.csv";

        //GENERAL SCREEN PLACEMENT PROPERTIES
        public static int SCREEN_LEFT_MARGIN = 10;
        public static int SCREEN_RIGHT_MARGIN = 10;
        public static int SCREEN_TOP_MARGIN = 10;
        public static int SCREEN_BOTTOM_MARGIN = 13;
        public static int MAX_SCREEN_WIDTH = 800;
        public static int MAX_SCREEN_HEIGHT = 600;
       






               //int aid1 = 1;
            //int aid2 = 4;
            //int aid3 = 7;
            //int aid4 = 8;
            //int aid5 = 11;
            //int aid6 = 17;
        //MITEs Constants

        public static int MAX_CONTROLLERS = 3;
        public static int ACCEL_ID1 = 1;
        public static int ACCEL_ID2 = 4;
        public static int ACCEL_ID3 = 7;
        public static int ACCEL_ID4 = 8;
        public static int ACCEL_ID5 = 11;
        public static int ACCEL_ID6 = 17;
        public static int HR_SAMPLING_RATE = 60;
        public static int MAX_PORT = 30;

        //Annotation Messages

        public static string AUTO_MODE_SESSION = "auto";
        public static string MANUAL_MODE_SESSION = "manual";
        public static int TRAINING_GAP = 10000; //10 seconds

    }
}
