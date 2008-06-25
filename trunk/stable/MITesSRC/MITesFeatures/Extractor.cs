using System;
using System.Collections.Generic;
using System.Text;
using MITesFeatures.core;
using System.IO;
using HousenCS.MITes;
using MITesFeatures.Utils;
using MITesFeatures;
using MITesFeatures.core.conf;
using System.Collections;

namespace MITesFeatures
{
    public class Extractor
    {
        private static double[] features;
        private static double[][] standardized;
        private static double[] means;
        private static int[] inputFFT;

        private static bool autoMode = false;

        private static int num_features = 0;
        private static int inputRowSize;
        private static int inputColumnSize;
        private static int fftInterpolationPower;
        private static int fftMaximumFrequencies;
        //private static int trainingTimePerClass;
        //private static int trainingWaitTime;
        private static MITesDecoder aMITesDecoder;
        private static SXML.SensorAnnotation sannotation;
        private static AXML.Annotation aannotation;
        //private static int lastTrainedActivity = "";
        private static GeneralConfiguration dconfiguration;
        private static string[] arffAttriburesLabels;

        private static int discardedLossRateWindows = 0;
        private static int discardedConsecutiveLossWindows = 0;
        private static double averageLossRate;

        //Calculating windowing parameters

        //total number of points per interpolated window
        private static int INTERPOLATED_SAMPLING_RATE_PER_WINDOW;  
        //expected sampling rate per MITes
        private static int EXPECTED_SAMPLING_RATE;
        //expected samples per window
        private static int EXPECTED_WINDOW_SIZE;
        //what would be considered a good sampling rate
        private static int EXPECTED_GOOD_SAMPLING_RATE;
        //space between samples
        public static double EXPECTED_SAMPLES_SPACING;
        //space between interpolated samples
        private static double INTERPOLATED_SAMPLES_SPACING;

        //window counters and delimiters
        private static double next_window_end=0;
        private static int total_window_count = 0;
        private static int num_feature_windows = 0;

        //data quality variables
        private static bool isAcceptableLossRate = true;
        private static bool isAcceptableConsecutiveLoss = true;
        private static int unacceptable_window_count = 0;
        private static int unacceptable_consecutive_window_loss_count = 0;

        private static double[][] data;
        private static double[][] interpolated_data;
        private static int[] y_index;       


        private static double WINDOWING_START_TIME = 0.0;
        private static double WINDOWING_END_TIME = 0.0;

        //realtime variables
        private static Hashtable trainingTime;
        private static bool trainingCompleted=false;

        public static int DiscardedLossRateWindows
        {
            get
            {
                return Extractor.discardedLossRateWindows;
            }
        }

        public static int DiscardedConsecutiveLossWindows
        {
            get
            {
                return Extractor.discardedConsecutiveLossWindows;
            }
        }

        public static double AverageLossRate
        {
            get
            {
                return Extractor.averageLossRate;
            }
        }
        public static GeneralConfiguration Configuration
        {
            get
            {
                return Extractor.dconfiguration;
            }
        }
        public static bool AutoMode
        {
            set
            {
                Extractor.autoMode = value;
            }
            get
            {
                return Extractor.autoMode;
            }
        }
        public static Hashtable TrainingTime
        {
            get
            {
                return Extractor.trainingTime;
            }
           
        }

        public static bool IsTrained(string activity)
        {
            if (activity.Equals(""))
                return false;

            //int index = -1;
            //for (int i = 0; (i < ((AXML.Category)aannotation.Categories[0]).Labels.Count); i++)
                //if (((AXML.Label)((AXML.Category)aannotation.Categories[0]).Labels[i]).Name == activity)
                  //  index = i;
            //if ((index!=-1) &&(Extractor.trainingTime[index] < Extractor.dconfiguration.TrainingTime))
            if ((int) Extractor.trainingTime[activity] < (Extractor.dconfiguration.TrainingTime*1000))
                return false;

            return true;
        }



        public static bool IsTrainingCompleted()
        {
            if (trainingCompleted)
                return trainingCompleted;
            else
            {
                //for (int i = 0; (i < Extractor.trainingTime); i++)
                foreach(string activity in Extractor.trainingTime.Keys)
                {
                    if ((int)Extractor.trainingTime[activity] < (Extractor.dconfiguration.TrainingTime*1000))                    
                        return false;
                    
                }

                Extractor.trainingCompleted = true;
                return true;
            }
        }

        public static string[] ArffAttributeLabels
        {
            get
            {
                return Extractor.arffAttriburesLabels;
            }
        }

        public static double[] Features
        {
            get
            {
                return Extractor.features;
            }
        }

        public static void Initialize(int inputRowSize, int fftInterpolationPower, int fftMaximumFrequencies)
        {
            Extractor.inputRowSize = inputRowSize;
            Extractor.fftInterpolationPower = fftInterpolationPower;
            Extractor.fftMaximumFrequencies = fftMaximumFrequencies;
            Extractor.inputColumnSize= (int) Math.Pow(2,Extractor.fftInterpolationPower);


            Extractor.num_features = Extractor.inputRowSize; // number of distances
            Extractor.num_features += 1; //total mean;
            Extractor.num_features += Extractor.inputRowSize; // number of variances
            Extractor.num_features += Extractor.inputRowSize; // number of ranges
            Extractor.num_features += 2 * Extractor.fftMaximumFrequencies * Extractor.inputRowSize; // number of fft magnitudes and frequencies
            Extractor.num_features += Extractor.inputRowSize; // number of energies
            Extractor.num_features += ((Extractor.inputRowSize * Extractor.inputRowSize) - Extractor.inputRowSize) / 2; //correlation coefficients off-di
            Extractor.features = new double[Extractor.num_features];
            Extractor.standardized = new double[inputRowSize][];
            for (int i = 0; (i < inputRowSize); i++)
                Extractor.standardized[i] = new double[Extractor.inputColumnSize];//input[0].Length];

            Extractor.means = new double[inputRowSize];

            inputFFT = new int[Extractor.inputColumnSize];
            FFT.Initialize(fftInterpolationPower, fftMaximumFrequencies);
            
        }


        public static void Initialize(MITesDecoder aMITesDecoder, string aDataDirectory,
            AXML.Annotation aannotation,SXML.SensorAnnotation sannotation)//, string masterDirectory)
        {

            Extractor.aannotation = aannotation;
            Extractor.sannotation = sannotation;

            //load sensor data
            //SXML.Reader sreader = new SXML.Reader(masterDirectory, aDataDirectory);
            //Extractor.sannotation = sreader.parse();

            //load configuration
            ConfigurationReader creader = new ConfigurationReader(aDataDirectory);
            Extractor.dconfiguration = creader.parse();

            
            //load annotation data
            //AXML.Reader reader = new AXML.Reader(masterDirectory, aDataDirectory + "\\" + AXML.Reader.DEFAULT_XML_FILE);
            //Extractor.aannotation = reader.parse();          
            //CHANGE: gathers training samples based on the first category only 
            Extractor.trainingTime = new Hashtable();//int[((AXML.Category)Extractor.aannotation.Categories[0]).Labels.Count];
            //for (int i = 0; (i < Extractor.trainingTime.Length); i++)
            foreach (AXML.Label label in ((AXML.Category)Extractor.aannotation.Categories[0]).Labels)
                Extractor.trainingTime.Add(label.Name, 0);
                //Extractor.trainingTime[i] = 0;
            Extractor.trainingCompleted = false;

            Extractor.inputRowSize = sannotation.Sensors.Count * 3;
            Extractor.fftInterpolationPower = dconfiguration.FFTInterpolatedPower;
            Extractor.fftMaximumFrequencies = dconfiguration.FFTMaximumFrequencies;
            //Extractor.trainingTimePerClass = configuration.TrainingTime;
            //Extractor.trainingWaitTime = configuration.TrainingWaitTime;         
            Extractor.inputColumnSize = (int)Math.Pow(2, Extractor.fftInterpolationPower);


            Extractor.num_features = Extractor.inputRowSize; // number of distances
            Extractor.num_features += 1; //total mean;
            Extractor.num_features += Extractor.inputRowSize; // number of variances
            Extractor.num_features += Extractor.inputRowSize; // number of ranges
            Extractor.num_features += 2 * Extractor.fftMaximumFrequencies * Extractor.inputRowSize; // number of fft magnitudes and frequencies
            Extractor.num_features += Extractor.inputRowSize; // number of energies
            Extractor.num_features += ((Extractor.inputRowSize * Extractor.inputRowSize) - Extractor.inputRowSize) / 2; //correlation coefficients off-di
            Extractor.features = new double[Extractor.num_features];
            Extractor.arffAttriburesLabels = new string[Extractor.num_features];

            Extractor.standardized = new double[inputRowSize][];
            for (int i = 0; (i < inputRowSize); i++)
                Extractor.standardized[i] = new double[Extractor.inputColumnSize];//input[0].Length];

            Extractor.means = new double[inputRowSize];

            inputFFT = new int[Extractor.inputColumnSize];
            FFT.Initialize(fftInterpolationPower, fftMaximumFrequencies);
            Extractor.aMITesDecoder = aMITesDecoder;

            //Create the ARFF File header
            string arffHeader = "@RELATION wockets\n\n" + Extractor.GetArffHeader();//sannotation.Sensors.Count * 3, configuration.FFTMaximumFrequencies);
            arffHeader += "@ATTRIBUTE activity {";
            foreach (AXML.Label label in ((AXML.Category)Extractor.aannotation.Categories[0]).Labels)
                arffHeader += label.Name.Replace(' ', '_') + ",";
            arffHeader += "unknown}\n";
            arffHeader += "\n@DATA\n\n";

            //Calculating windowing parameters

            //total number of points per interpolated window
            Extractor.INTERPOLATED_SAMPLING_RATE_PER_WINDOW = (int)Math.Pow(2, dconfiguration.FFTInterpolatedPower); //128;  
            //expected sampling rate per MITes
            Extractor.EXPECTED_SAMPLING_RATE = dconfiguration.ExpectedSamplingRate / sannotation.Sensors.Count; //samples per second
            //expected samples per window
            Extractor.EXPECTED_WINDOW_SIZE = (int)(EXPECTED_SAMPLING_RATE * (dconfiguration.WindowTime / 1000.0)); // expectedSamplingRate per window
            //what would be considered a good sampling rate
            Extractor.EXPECTED_GOOD_SAMPLING_RATE = EXPECTED_WINDOW_SIZE - (int)(dconfiguration.MaximumNonconsecutiveFrameLoss * EXPECTED_WINDOW_SIZE); //number of packets lost per second                      
            //space between samples
            Extractor.EXPECTED_SAMPLES_SPACING = (double)dconfiguration.WindowTime / EXPECTED_WINDOW_SIZE;
            //space between interpolated samples
            Extractor.INTERPOLATED_SAMPLES_SPACING = (double)dconfiguration.WindowTime / INTERPOLATED_SAMPLING_RATE_PER_WINDOW;

            //window counters and delimiters
            Extractor.next_window_end = 0;
            Extractor.total_window_count = 0;
            Extractor.num_feature_windows = 0;

            //data quality variables
            Extractor.isAcceptableLossRate = true;
            Extractor.isAcceptableConsecutiveLoss = true;
            Extractor.unacceptable_window_count = 0;
            Extractor.unacceptable_consecutive_window_loss_count = 0;


            //2 D array that stores Sensor axes + time stamps on each row  X expected WINDOW SIZE
            Extractor.data = new double[Extractor.sannotation.Sensors.Count * 4][]; // 1 row for each axis

            // 2D array that stores Sensor axes X INTERPOLATED WINDOW SIZE
            Extractor.interpolated_data = new double[Extractor.sannotation.Sensors.Count * 3][];

            // array to store the y location for each axes as data is received
            // will be different for every sensor of course
            Extractor.y_index = new int[Extractor.sannotation.Sensors.Count];


            //Initialize expected data array
            for (int j = 0; (j < (Extractor.sannotation.Sensors.Count * 4)); j++)
            {
                Extractor.data[j] = new double[EXPECTED_WINDOW_SIZE];
                for (int k = 0; (k < EXPECTED_WINDOW_SIZE); k++)
                    Extractor.data[j][k] = 0;
            }

            //Initialize interpolated data array
            for (int j = 0; (j < (Extractor.sannotation.Sensors.Count * 3)); j++)
            {
                Extractor.interpolated_data[j] = new double[INTERPOLATED_SAMPLING_RATE_PER_WINDOW];
                for (int k = 0; (k < INTERPOLATED_SAMPLING_RATE_PER_WINDOW); k++)
                    Extractor.interpolated_data[j][k] = 0;
            }

            //Initialize y index for each sensor
            for (int j = 0; (j < Extractor.sannotation.Sensors.Count); j++)
                Extractor.y_index[j] = 0;

        }







        //start collecting features from MITes decoder, do windowing plus calculate features
        public static double StoreMITesWindow()
        {
            double unixtimestamp = 0.0;

            for (int i = 0; i < Extractor.aMITesDecoder.someMITesDataIndex; i++)
            {
                if ((aMITesDecoder.someMITesData[i].type != (int)MITesTypes.NOISE) &&
                     (aMITesDecoder.someMITesData[i].type == (int)MITesTypes.ACCEL))
                {
                    int channel = 0, x = 0, y = 0, z = 0;                    
                    channel = (int)aMITesDecoder.someMITesData[i].channel;
                    x = (int)aMITesDecoder.someMITesData[i].x;
                    y = (int)aMITesDecoder.someMITesData[i].y;
                    z = (int)aMITesDecoder.someMITesData[i].z;
                    unixtimestamp = aMITesDecoder.someMITesData[i].unixTimeStamp;
   


                    int x_index = -1;
                    for (int ii = 0; (ii < Extractor.sannotation.Sensors.Count); ii++)
                    {
                        if (channel == Convert.ToInt32(((SXML.Sensor)Extractor.sannotation.Sensors[ii]).ID))
                        {
                            x_index = ii;
                            break;
                        }
                    }
                    if (x_index == -1)//ignore mites data that do not belong to the setup
                        continue;

                    //calculate the x index in the data array for this particular sensor
                    int adjusted_x_index = x_index * 4; //base row for storing x,y,z,timestamp for this channel

                    // store the values in the current frame in the correct column based of the EXPECTED_WINDOW data array
                    // on the y_index for the sensor
                    Extractor.data[adjusted_x_index][Extractor.y_index[x_index]] = x;
                    Extractor.data[adjusted_x_index + 1][Extractor.y_index[x_index]] = y;
                    Extractor.data[adjusted_x_index + 2][Extractor.y_index[x_index]] = z;
                    Extractor.data[adjusted_x_index + 3][Extractor.y_index[x_index]] = unixtimestamp;

                    //increment the y_index for the sensor and wrap around if needed
                    Extractor.y_index[x_index] = (Extractor.y_index[x_index] + 1) % Extractor.EXPECTED_WINDOW_SIZE;

                }               
                
            }

            return unixtimestamp;
            
        }

        public static bool GenerateFeatureVector(double lastTimeStamp)
        {

            if (lastTimeStamp < Extractor.next_window_end)
                return false;

 //           if (previousTS == lastTimeStamp)
 //               Console.WriteLine("problem");
 //           previousTS = lastTimeStamp;
            // the last time stamp is more than the next expected window end
            // At this point, we have a complete window ready for feature calculation

            //compute the boundaries for the current window
            double window_start_time = lastTimeStamp- Extractor.dconfiguration.WindowTime;
            double window_end_time = lastTimeStamp;
            double current_time = window_end_time;
            //compute the end of the next overlapping window
            next_window_end = window_end_time + (Extractor.dconfiguration.WindowTime * Extractor.dconfiguration.WindowOverlap);

            #region sensors window grabbing and interpolation

            // Go through each sensor and extract the collected data within 
            // the current time window
            for (int j = 0; (j < sannotation.Sensors.Count); j++)
            {

                // Check that the previous sensor in the loop did not report
                // deteriorated quality for its data
                #region sensors window quality
                if (isAcceptableLossRate == false)                    
                    break;
                
                // check if earlier axes reported excessive consecutive loss of data frames
                if (isAcceptableConsecutiveLoss == false)
                {
                    Extractor.discardedConsecutiveLossWindows++;
                    break;
                }
                #endregion sensors window quality

                // determine the base index for the current sensor, each sensor has 4 rows (x,y,z,timestamp)
                int sensor_index = j * 4;
                int time_index = sensor_index + 3;

                // determine the last read data sample for the current sensor
                // by looking at its index
                int last_sample = 0;
                if (y_index[j] == 0)
                    last_sample = EXPECTED_WINDOW_SIZE - 1;
                else
                    last_sample = y_index[j] - 1;
                int total_data_points = 0, distinct_data_points = 0;


                //Grab the readings for each axis of a sensor and smoothen it
                #region sensor window grabbing and interpolation
                // Go through each axis of the current sensor and smooth
                // it using the cubic spline
                for (int axes_num = 0; (axes_num < 3); axes_num++)
                {

                    //calculate the exact index based on the 
                    // base sensor index and the axis number
                    int axes_index = sensor_index + axes_num;  //for data array
                    int interpolated_axes_index = j * 3+ axes_num; //for interpolated data array

                    // create 2 arrays to store x and y values for the cubic spline
                    // it is sufficient to have an array of expected sampling rate window size
                    // for 3 mites that would be 180/60
                    double[] xvals = new double[EXPECTED_WINDOW_SIZE];
                    double[] yvals = new double[EXPECTED_WINDOW_SIZE];

                    //point to the last sample that was read and get its time
                    int sample_index = last_sample;
                    current_time = data[time_index][sample_index];
                    //copy samples in the last time window
                    total_data_points = 0;
                    distinct_data_points = 0;
                    double previous_time = 0;


                    //Grab the values for a specific sensor axes between
                    //window start and window end
                    #region window grabbing
                    // Start going back from the current time (window end) till the start of the window
                    // without exceeding the expected sampling rate and fill in the data in the signal
                    // value for the axis in yvals and the relative time value from the window start
                    while ((current_time >= window_start_time) && (current_time <= window_end_time)
                        && (total_data_points < EXPECTED_WINDOW_SIZE) && (distinct_data_points < EXPECTED_WINDOW_SIZE))
                    {

                        //some time stamps from the mites are identical
                        // for interpolation that will cause an error
                        // simply take the first value for a time point and ignore the
                        // rest, another strategy would be to average over these values
                        if (current_time == previous_time)
                        {
                            //Get the time of the previous sample and skip the sample
                            if (sample_index == 0)
                                sample_index = EXPECTED_WINDOW_SIZE - 1;
                            else
                                sample_index--;
                            current_time = data[time_index][sample_index];
                            total_data_points++;
                            continue;
                        }

                        // Quality Control
                        // check the time between consecutive data frames and make sure it does
                        // not exceed maximum_consecutive_loss, do not do that for the first
                        // entry of the window
                        // Not suitable for the phone due to time resolution
                        //if (distinct_data_points > 0)
                        //{
                        //    int consecutive_lost_packets = (int)((previous_time - current_time) / EXPECTED_SAMPLES_SPACING);
                        //    if (consecutive_lost_packets > Extractor.dconfiguration.MaximumConsecutiveFrameLoss)
                        //    {
                        //        Extractor.discardedConsecutiveLossWindows++;      
                        //        unacceptable_consecutive_window_loss_count++;
                        //        isAcceptableConsecutiveLoss = false;
                        //        break;
                        //    }
                        //}


                        //DateTime dt = (new DateTime(1970, 1, 1, 0, 0, 0)).AddMilliseconds(current_time);
                        //relative time value from window start
                        xvals[distinct_data_points] = (int)(current_time - window_start_time);
                        //signal value for the current sample and current axis.
                        yvals[distinct_data_points] = data[axes_index][sample_index];


                        //Point to the previous sample in the current window
                        if (sample_index == 0)
                            sample_index = EXPECTED_WINDOW_SIZE - 1;
                        else
                            sample_index--;

                        //store the previous sample time
                        previous_time = current_time;

                        //Get the time of the new sample
                        current_time = data[time_index][sample_index];

                        //Point to the next entry in the interpolation array
                        distinct_data_points++;

                        total_data_points++;
                    }
                    #endregion window grabbing

                    //Check if the captured window has acceptable loss rate
                    #region window quality checks
                    //Do not proceed if there was excessive consecutive loss of data frames
                    if (isAcceptableConsecutiveLoss == false)
                        break;

                    // all data for a specific sensor axis for the current window are stored
                    // in xvals and yvals
                    // check if the data is admissible for feature calculation according to the following
                    // criteria:
                    // 1- total lost data frames are within the loss rate
                    // 2- the number of consecutive lost packets is within our maximum_consecutive_loss parameter
                    if (distinct_data_points < EXPECTED_GOOD_SAMPLING_RATE) //discard this whole window of data
                    {
                        Extractor.discardedLossRateWindows++;      
                        isAcceptableLossRate = false;
                        unacceptable_window_count++;
                        break;
                    }

                    #endregion window quality checks

                    //smoothen the axis values and store them in interpolated data array
                    #region window interpolation

                    //create 2 arrays with the exact size of the data points for interpolation
                    double[] admissible_xvals = new double[distinct_data_points];
                    double[] admissible_yvals = new double[distinct_data_points];
                    for (int k = 0; (k < distinct_data_points); k++)
                    {
                        admissible_xvals[k] = xvals[distinct_data_points - k - 1];
                        admissible_yvals[k] = yvals[distinct_data_points - k - 1];
                    }
                    // smooth it using a cubic spline
                    CubicSpline cs = new CubicSpline(admissible_xvals, admissible_yvals);

                    // shrink or expand the data window using interpolation                
                    for (int k = 0; (k < INTERPOLATED_SAMPLING_RATE_PER_WINDOW); k++)
                    {
                        interpolated_data[interpolated_axes_index][k] = cs.interpolate(k * INTERPOLATED_SAMPLES_SPACING);
                        //check that the intrepolated values make sense.
                        if ((interpolated_data[interpolated_axes_index][k] <= 0) || (interpolated_data[interpolated_axes_index][k] > 1024))
                            return false;
                    }


                    #endregion window interpolation
                }
                #endregion sensor window grabbing and interpolation



            }
            #endregion sensors window grabbing and interpolation

            //If the data is of acceptable quality, calculate the features
            #region Calculate Feature Vector

            if ((isAcceptableLossRate == true) && (isAcceptableConsecutiveLoss == true))
            {           
                //Extract the features from the interpolated data
                Extractor.Extract(interpolated_data);

                //Output the data to the ARFF file
                // tw.WriteLine(Extractor.toString() + "," + current_activity);
                //num_feature_windows++;
                // make sure the values of the features make sense... for example distance can 
                // only be -1024---- or so
                return true;
            }
            else  //the window is of poor quality, reinitialize and continue
            {
                isAcceptableConsecutiveLoss = true;
                isAcceptableLossRate = true;

                return false;
            }

            #endregion Calculate Feature Vector

           
        }

        //stop collecting features from MITes decoder close the file and terminate
        //public static void Cleanup()
       // {
        //}

       /*
        public static void toARFF(string aDataDirectory,string masterDirectory, string labelsFile)
        {
            #region Initialization
            //true as long as there is data
            bool isData = true;

            //load annotation data
            AXML.Reader reader = new AXML.Reader(masterDirectory,aDataDirectory + "\\" + labelsFile);
            Extractor.aannotation = reader.parse();


            //load sensor data
            SXML.Reader sreader = new SXML.Reader(masterDirectory,aDataDirectory);
            Extractor.sannotation = sreader.parse();

            //load decision learning parameters such as interpolation size, maximum frequencies included
            ConfigurationReader creader = new ConfigurationReader(aDataDirectory);
            GeneralConfiguration configuration = creader.parse();

            //Initialize the feature extractor based on the above parameters
            Extractor.Initialize(sannotation.Sensors.Count * 3, configuration.FFTInterpolatedPower, configuration.FFTMaximumFrequencies);

            //Create the ARFF File header
            string arffHeader = "@RELATION wockets\n\n" + Extractor.GetArffHeader();//sannotation.Sensors.Count * 3, configuration.FFTMaximumFrequencies);
            arffHeader += "@ATTRIBUTE activity {";
            foreach (AXML.Label label in ((AXML.Category)Extractor.aannotation.Categories[0]).Labels)
                arffHeader += label.Name.Replace(' ', '_') + ",";
            arffHeader += "unknown}\n";
            arffHeader += "\n@DATA\n\n";


            //Calculating windowing parameters
            
            //total number of points per interpolated window
            INTERPOLATED_SAMPLING_RATE_PER_WINDOW = (int)Math.Pow(2, configuration.FFTInterpolatedPower); //128;  
            //expected sampling rate per MITes
            EXPECTED_SAMPLING_RATE = configuration.ExpectedSamplingRate / sannotation.Sensors.Count; //samples per second
            //expected samples per window
            EXPECTED_WINDOW_SIZE = (int)(EXPECTED_SAMPLING_RATE * (configuration.WindowTime / 1000.0)); // expectedSamplingRate per window
            //what would be considered a good sampling rate
            EXPECTED_GOOD_SAMPLING_RATE = EXPECTED_WINDOW_SIZE - (int)(configuration.MaximumNonconsecutiveFrameLoss * EXPECTED_WINDOW_SIZE); //number of packets lost per second                      
            //space between samples
            EXPECTED_SAMPLES_SPACING = (double)configuration.WindowTime / EXPECTED_WINDOW_SIZE;
            //space between interpolated samples
            INTERPOLATED_SAMPLES_SPACING = (double)configuration.WindowTime / INTERPOLATED_SAMPLING_RATE_PER_WINDOW;

            //window counters and delimiters
            next_window_end = 0;
            total_window_count = 0;
            num_feature_windows = 0;

            //data quality variables
            isAcceptableLossRate = true;
            isAcceptableConsecutiveLoss = true;
            unacceptable_window_count = 0;
            unacceptable_consecutive_window_loss_count = 0;

            //2 D array that stores Sensor axes + time stamps on each row  X expected WINDOW SIZE
            data = new double[sannotation.Sensors.Count * 4][]; // 1 row for each axis

            // 2D array that stores Sensor axes X INTERPOLATED WINDOW SIZE
            interpolated_data = new double[sannotation.Sensors.Count * 3][];

            // array to store the y location for each axes as data is received
            // will be different for every sensor of course
            y_index = new int[sannotation.Sensors.Count];

            //Initialize expected data array
            for (int j = 0; (j < (sannotation.Sensors.Count * 4)); j++)
            {
                data[j] = new double[EXPECTED_WINDOW_SIZE];
                for (int k = 0; (k < EXPECTED_WINDOW_SIZE); k++)
                    data[j][k] = 0;
            }

            //Initialize interpolated data array
            for (int j = 0; (j < (sannotation.Sensors.Count * 3)); j++)
            {
                interpolated_data[j] = new double[INTERPOLATED_SAMPLING_RATE_PER_WINDOW];
                for (int k = 0; (k < INTERPOLATED_SAMPLING_RATE_PER_WINDOW); k++)
                    interpolated_data[j][k] = 0;
            }

            //Initialize y index for each sensor
            for (int j = 0; (j < sannotation.Sensors.Count); j++)
                y_index[j] = 0;




            //Initialize the first window start and the last window end time
            int i;  //offline data
            i = 0;
            WINDOWING_START_TIME = ((AXML.AnnotatedRecord)Extractor.aannotation.Data[0]).StartUnix;
            WINDOWING_END_TIME = ((AXML.AnnotatedRecord)Extractor.aannotation.Data[Extractor.aannotation.Data.Count - 1]).EndUnix;


            //Initialize the MITes decoder for the passed directory
            MITesDecoder aMITesDecoder = new MITesDecoder();
            MITesLoggerReaderMR aMITesLoggerReaderMR = new MITesLoggerReaderMR(aMITesDecoder, aDataDirectory);

            //Read the first annotation
            AXML.AnnotatedRecord annotatedRecord = (AXML.AnnotatedRecord)Extractor.aannotation.Data[i];
            int samples = 0;
            int otherSamples = 0;

            #endregion Initialization

            //Create the ARFF File
            TextWriter tw = new StreamWriter("output.arff");
            tw.WriteLine(arffHeader);

            // Loop through the MITes data in the directory
            // do not go beyond the last annotated record

            //Get the the first <=10 frames
            isData = aMITesLoggerReaderMR.GetSensorData(10);
            while ((isData) && (i < Extractor.aannotation.Data.Count))
            {

                
                #region Read MITes data until current time point passes
                // To calculate the features for at a specific Time point t
                // make sure that all frames from different sensors with time
                // t or less have been received
                // keep grabbing frames until t has passed
                // assumption: frames are timestamped in order                
                int channel = 0, x = 0, y = 0, z = 0;
                double unixtimestamp = 0.0;
                int x_index = 0;
                do
                {
                    //decode the frame
                    channel = aMITesDecoder.GetSomeMITesData()[0].channel;
                    x = aMITesDecoder.GetSomeMITesData()[0].x;
                    y = aMITesDecoder.GetSomeMITesData()[0].y;
                    z = aMITesDecoder.GetSomeMITesData()[0].z;
                    unixtimestamp = aMITesDecoder.GetSomeMITesData()[0].unixTimeStamp;
                    x_index = -1;

                    //CHANGE To pick the right index for any size
                    //determine the index of the sensor in the sensorData file
                    for(int ii=0;(ii<sannotation.Sensors.Count);ii++)
                    {
                        if (channel == Convert.ToInt32(((SXML.Sensor)sannotation.Sensors[ii]).ID))
                        {
                            x_index = ii;
                            break;
                        }
                    }
                    if (x_index == -1)//ignore mites data that do not belong to the setup
                        continue;

                   // if (channel == Convert.ToInt32(((SXML.Sensor)sannotation.Sensors[0]).ID))
                   //     x_index = 0;
                   // else if (channel == Convert.ToInt32(((SXML.Sensor)sannotation.Sensors[1]).ID))
                   //    x_index = 1;
                   // else if (channel == Convert.ToInt32(((SXML.Sensor)sannotation.Sensors[2]).ID))
                   //     x_index = 2;
                   // else //ignore mites data that do not belong to the setup
                   //     continue;


                    //calculate the x index in the data array for this particular sensor
                    int adjusted_x_index = x_index * (sannotation.Sensors.Count + 1);

                    // store the values in the current frame in the correct column based of the EXPECTED_WINDOW data array
                    // on the y_index for the sensor
                    data[adjusted_x_index][y_index[x_index]] = x;
                    data[adjusted_x_index + 1][y_index[x_index]] = y;
                    data[adjusted_x_index + 2][y_index[x_index]] = z;
                    data[adjusted_x_index + 3][y_index[x_index]] = unixtimestamp;
                    
                    //increment the y_index for the sensor and wrap around if needed
                    y_index[x_index] = (y_index[x_index] + 1) % EXPECTED_WINDOW_SIZE;

                    //continue while there is data and while the time stamp for previous sample and the current are the same
                    //typically when starting to read data or to make sure that all data from different sensors for
                    // a specific time stamp have been collected
                } while ((isData = aMITesLoggerReaderMR.GetSensorData(10)) && (unixtimestamp == aMITesDecoder.GetSomeMITesData()[0].unixTimeStamp));

                #endregion Read MITes data until current time point passes

                //Check if there is no more data, if so break from the data collection loop
                if (!isData)
                    break;


                // Here ALL data from all sensors <=time t have been collected, you can now attempt to
                // calculate the features for the window at time t


                #region Check if window captured and calculate features


                //If an annotation just ended, go to the next annotation
                if (unixtimestamp >= annotatedRecord.EndUnix)
                //Check if the passed time point has concluded an activity
                {
                    Console.WriteLine(((AXML.Label)annotatedRecord.Labels[0]).Name + " " + annotatedRecord.StartUnix +
                        " " + annotatedRecord.EndUnix + " " + samples);
                    i++;
                    samples = 0;
                    
                    if (i < Extractor.aannotation.Data.Count)
                        annotatedRecord = (AXML.AnnotatedRecord)Extractor.aannotation.Data[i];
                }
                // We calculate the features at the end of each window
                // If the passed time point is:
                // 1- within the starting of the first activity and the end of the last activity
                // 2- is equal or exceeds the end of the next expected window
                // then calculate the features

                else if ((unixtimestamp >= WINDOWING_START_TIME) && (unixtimestamp < WINDOWING_END_TIME)
                    && (unixtimestamp >= next_window_end))
                //Check if a time point is within an activity
                {

                    //compute the boundaries for the current window
                    double window_start_time = unixtimestamp - configuration.WindowTime;
                    double window_end_time = unixtimestamp;
                    double current_time = window_end_time;
                    //compute the end of the next overlapping window
                    next_window_end = window_end_time + (configuration.WindowTime * configuration.WindowOverlap);

                    //increment window counter
                    total_window_count++;


                    //for all sensors grab their data windows and smoothen them
                    #region sensors window grabbing and interpolation

                    // Go through each sensor and extract the collected data within 
                    // the current time window
                    for (int j = 0; (j < sannotation.Sensors.Count); j++)
                    {

                        // Check that the previous sensor in the loop did not report
                        // deteriorated quality for its data
                        #region sensors window quality
                        if (isAcceptableLossRate == false)
                            break;
                        // check if earlier axes reported excessive consecutive loss of data frames
                        if (isAcceptableConsecutiveLoss == false)
                            break;
                        #endregion sensors window quality


                        // determine the base index for the current sensor, each sensor has 4 rows (x,y,z,timestamp)
                        int sensor_index = j * (sannotation.Sensors.Count + 1);
                        int time_index = sensor_index + 3;

                        // determine the last read data sample for the current sensor
                        // by looking at its index
                        int last_sample = 0;
                        if (y_index[j] == 0)
                            last_sample = EXPECTED_WINDOW_SIZE - 1;
                        else
                            last_sample = y_index[j] - 1;
                        int total_data_points = 0, distinct_data_points = 0;

                        //Grab the readings for each axis of a sensor and smoothen it
                        #region sensor window grabbing and interpolation
                        // Go through each axis of the current sensor and smooth
                        // it using the cubic spline
                        for (int axes_num = 0; (axes_num < 3); axes_num++)
                        {

                            //calculate the exact index based on the 
                            // base sensor index and the axis number
                            int axes_index = sensor_index + axes_num;  //for data array
                            int interpolated_axes_index = j * (sannotation.Sensors.Count) + axes_num; //for interpolated data array

                            // create 2 arrays to store x and y values for the cubic spline
                            // it is sufficient to have an array of expected sampling rate window size
                            // for 3 mites that would be 180/60
                            double[] xvals = new double[EXPECTED_WINDOW_SIZE];
                            double[] yvals = new double[EXPECTED_WINDOW_SIZE];

                            //point to the last sample that was read and get its time
                            int sample_index = last_sample;
                            current_time = data[time_index][sample_index];
                            //copy samples in the last time window
                            total_data_points = 0;
                            distinct_data_points = 0;
                            double previous_time = 0;


                            //Grab the values for a specific sensor axes between
                            //window start and window end
                            #region window grabbing
                            // Start going back from the current time (window end) till the start of the window
                            // without exceeding the expected sampling rate and fill in the data in the signal
                            // value for the axis in yvals and the relative time value from the window start
                            while ((current_time >= window_start_time) && (current_time <= window_end_time)
                                && (total_data_points < EXPECTED_WINDOW_SIZE) && (distinct_data_points < EXPECTED_WINDOW_SIZE))
                            {

                                //some time stamps from the mites are identical
                                // for interpolation that will cause an error
                                // simply take the first value for a time point and ignore the
                                // rest, another strategy would be to average over these values
                                if (current_time == previous_time)
                                {
                                    //Get the time of the previous sample and skip the sample
                                    if (sample_index == 0)
                                        sample_index = EXPECTED_WINDOW_SIZE - 1;
                                    else
                                        sample_index--;
                                    current_time = data[time_index][sample_index];
                                    total_data_points++;
                                    continue;
                                }

                                // Quality Control
                                // check the time between consecutive data frames and make sure it does
                                // not exceed maximum_consecutive_loss, do not do that for the first
                                // entry of the window
                                if (distinct_data_points > 0)
                                {
                                    int consecutive_lost_packets = (int)((previous_time - current_time) / EXPECTED_SAMPLES_SPACING);
                                    if (consecutive_lost_packets > configuration.MaximumConsecutiveFrameLoss)
                                    {
                                        unacceptable_consecutive_window_loss_count++;
                                        isAcceptableConsecutiveLoss = false;
                                        break;
                                    }
                                }



                                //relative time value from window start
                                xvals[distinct_data_points] = (int)(current_time - window_start_time);
                                //signal value for the current sample and current axis.
                                yvals[distinct_data_points] = data[axes_index][sample_index];


                                //Point to the previous sample in the current window
                                if (sample_index == 0)
                                    sample_index = EXPECTED_WINDOW_SIZE - 1;
                                else
                                    sample_index--;

                                //store the previous sample time
                                previous_time = current_time;

                                //Get the time of the new sample
                                current_time = data[time_index][sample_index];
             
                                //Point to the next entry in the interpolation array
                                distinct_data_points++;

                                total_data_points++;
                            }
                            #endregion window grabbing


                            //Check if the captured window has acceptable loss rate
                            #region window quality checks
                            //Do not proceed if there was excessive consecutive loss of data frames
                            if (isAcceptableConsecutiveLoss == false)
                                break;

                            // all data for a specific sensor axis for the current window are stored
                            // in xvals and yvals
                            // check if the data is admissible for feature calculation according to the following
                            // criteria:
                            // 1- total lost data frames are within the loss rate
                            // 2- the number of consecutive lost packets is within our maximum_consecutive_loss parameter
                            if (distinct_data_points < EXPECTED_GOOD_SAMPLING_RATE) //discard this whole window of data
                            {
                                isAcceptableLossRate = false;
                                unacceptable_window_count++;
                                break;
                            }

                            #endregion window quality checks

                            //smoothen the axis values and store them in interpolated data array
                            #region window interpolation
                            
                            //create 2 arrays with the exact size of the data points for interpolation
                            double[] admissible_xvals = new double[distinct_data_points];
                            double[] admissible_yvals = new double[distinct_data_points];
                            for (int k = 0; (k < distinct_data_points); k++)
                            {
                                admissible_xvals[k] = xvals[distinct_data_points - k - 1];
                                admissible_yvals[k] = yvals[distinct_data_points - k - 1];
                            }
                            // smooth it using a cubic spline
                            CubicSpline cs = new CubicSpline(admissible_xvals, admissible_yvals);

                            // shrink or expand the data window using interpolation                
                            for (int k = 0; (k < INTERPOLATED_SAMPLING_RATE_PER_WINDOW); k++)                           
                                interpolated_data[interpolated_axes_index][k] = cs.interpolate(k * INTERPOLATED_SAMPLES_SPACING);
                                                        

                            #endregion window interpolation

                        }
                        #endregion sensor window grabbing and interpolation



                    }
                    #endregion  sensors window grabbing and interpolation


                    //If the data is of acceptable quality, calculate the features
                    #region Calculate Feature Vector

                    if ((isAcceptableLossRate == true) && (isAcceptableConsecutiveLoss == true))
                    {

                        //If the current activity is annotated pick it otherwise pick unknown
                        string current_activity = "unknown";
                        if ((unixtimestamp >= annotatedRecord.StartUnix) && (unixtimestamp < annotatedRecord.EndUnix))                        
                            current_activity = ((AXML.Label)(annotatedRecord.Labels[0])).Name;
                        
                        //Extract the features from the interpolated data
                        Extractor.Extract(interpolated_data);

                        //current_activity = current_activity.Replace(' ', '_');
                        //int aIndex = (int)activityIndex[current_activity];
                        //featuresData[aIndex].Add(Extractor.toString());
                        //labelData[aIndex].Add(current_activity.Replace(' ', '_'));
                        //sampleCounters[aIndex] += 1;
                        
                        //Output the data to the ARFF file
                        tw.WriteLine(Extractor.toString() + "," + current_activity);
                        num_feature_windows++;
                    }
                    else  //the window is of poor quality, reinitialize and continue
                    {
                        isAcceptableConsecutiveLoss = true;
                        isAcceptableLossRate = true;
                    }

                    #endregion Calculate Feature Vector


                }
                else
                    otherSamples++;

                #endregion Check if window captured and calculate features


                //here we have the x,y,z data stored in the first elements
                //of aMitesDecoder.someData with a time stamp

            }

            // Close the ARFF file and release resources
            tw.Close();
            Extractor.Cleanup();





            ////Generate the 10-fold cross validation
            ////break data into 10 sets of size n/10

            //// generate 10 training arff with 9  - balanced 
            //// generate 10 testing arff with 1


            //if (num_feature_windows > 0)
            //{
            //    int maxClassSize = 0;
            //    for (int j = 0; (j < sampleCounters.Length - 1); j++)
            //    {
            //        if (sampleCounters[j] > maxClassSize)
            //            maxClassSize = sampleCounters[j];
            //    }



            //    TextWriter tw1 = new StreamWriter("balanced_output_train.arff");
            //    //tw1.WriteLine("@RELATION wockets\n");
            //    tw1.Write(arffHeader);
            //    //tw1.Write("@ATTRIBUTE activity {");

            //    //foreach (AXML.Label label in ((AXML.Category)Extractor.aannotation.Categories[0]).Labels)
            //    //{
            //    //    tw1.Write(label.Name.Replace(' ', '_') + ",");
            //    //}
            //    //tw1.WriteLine("unknown}");
            //    //tw1.WriteLine("\n@DATA\n\n");

            //    for (int k = 0; (k < sampleCounters.Length - 1); k++)
            //    {

            //        for (int z = 0; (z < sampleCounters[k]); z++)
            //        {
            //            tw1.WriteLine(((string)featuresData[k][z]) + "," + ((string)labelData[k][z]));
            //        }

            //    }

            //    // for the unkown
            //    Random random = new Random();
            //    //training data
            //    for (int z = 0; (z < maxClassSize); z++)
            //    {
            //        int index = random.Next(sampleCounters[sampleCounters.Length - 1] - 1) % featuresData[sampleCounters.Length - 1].Count;
            //        tw1.WriteLine(((string)featuresData[sampleCounters.Length - 1][index]) + ",unknown");
            //    }

            //    tw1.Close();






            //    for (int j = 0; (j < 10); j++)
            //    {
            //        tw1 = new StreamWriter("training" + j + ".arff");
            //        tw1.Write(arffHeader);
            //        //tw1.WriteLine("@RELATION wockets\n");
            //        //tw1.Write(relations);
            //        //tw1.Write("@ATTRIBUTE activity {");
            //        //foreach (AXML.Label label in ((AXML.Category)Extractor.aannotation.Categories[0]).Labels)
            //        //{
            //        //    tw1.Write(label.Name.Replace(' ', '_') + ",");
            //        //}
            //        //tw1.WriteLine("unknown}");
            //        //tw1.WriteLine("\n@DATA\n\n");

            //        TextWriter tw2 = new StreamWriter("testing" + j + ".arff");
            //        tw2.Write(arffHeader);
            //        //tw2.WriteLine("@RELATION wockets\n");
            //        //tw2.Write(relations);
            //        //tw2.Write("@ATTRIBUTE activity {");
            //        //foreach (AXML.Label label in ((AXML.Category)Extractor.aannotation.Categories[0]).Labels)
            //        //{
            //        //    tw2.Write(label.Name.Replace(' ', '_') + ",");
            //        //}
            //        //tw2.WriteLine("unknown}");
            //        //tw2.WriteLine("\n@DATA\n\n");

            //        int unknownTrainingSize = 0;
            //        //activities
            //        for (int k = 0; (k < sampleCounters.Length); k++)
            //        {

            //            int foldSize = sampleCounters[k] / 10;
            //            int testingFoldStart = j * foldSize;
            //            int testingFoldEnd = testingFoldStart + foldSize;


            //            for (int z = 0; (z < sampleCounters[k]); z++)
            //            {
            //                if (k == (sampleCounters.Length - 1)) //unknown activity
            //                {
            //                    //skip fold if needed
            //                    if (((z < testingFoldStart) || (z > testingFoldEnd)) && (unknownTrainingSize <= maxClassSize))
            //                    { // add to training
            //                        tw1.WriteLine(((string)featuresData[k][z]) + "," + ((string)labelData[k][z]));
            //                        unknownTrainingSize++;
            //                    }
            //                    else // add to test data
            //                        tw2.WriteLine(((string)featuresData[k][z]) + "," + ((string)labelData[k][z]));
            //                }
            //                else
            //                {
            //                    //skip fold if needed
            //                    if ((z < testingFoldStart) || (z > testingFoldEnd)) // add to training
            //                        tw1.WriteLine(((string)featuresData[k][z]) + "," + ((string)labelData[k][z]));
            //                    else // add to test data
            //                        tw2.WriteLine(((string)featuresData[k][z]) + "," + ((string)labelData[k][z]));
            //                }

            //            }
            //        }



            //        tw1.Close();
            //        tw2.Close();
            //    }
            //}

          
            Console.WriteLine("Total Windows Count:" + total_window_count);
            Console.WriteLine("Inadmissible Windows Count (loss rate):" + unacceptable_window_count);
            Console.WriteLine("Inadmissible Windows Count (consecutive loss):" + unacceptable_consecutive_window_loss_count);
            Console.WriteLine("Admissible Feature Windows:" + num_feature_windows);
        }

       */
        public static void Cleanup()
        {
            FFT.Cleanup(); 
        }

        //input is a 2D array 3*SensorCount X 2^ FFT Interpolation Power e.g. for 3 MITes INT power=7  9 X 128
        public static void Extract(double[][] input)//,int fftInterpolationPower, int fftMaximumFrequencies)
        {

            int j = 0,i = 0;
            double sum = 0, min =0, max = 0,total = 0,variance = 0;

            int distanceIndex = 0;//number of means on every row + 1 for total mean, 0 based index
            int varianceIndex = distanceIndex + inputRowSize + 1; // add number of distances
            int rangeIndex = varianceIndex + inputRowSize;
            int fftIndex = rangeIndex + inputRowSize;
            int energyIndex = fftIndex + (2* fftMaximumFrequencies * inputRowSize);
            int correlationIndex = energyIndex + inputRowSize; //add number of variances         
            

            //for good cache locality go through the rows then columns
            for (i = 0; (i < inputRowSize); i++)
            {
                min = 999999999.0;
                max = -999999999.0;
                sum = 0;
                
                for (j = 0; (j < Extractor.inputColumnSize); j++)
                {
                    if (input[i][j] < min)
                        min = input[i][j];
                    if (input[i][j] > max)
                        max = input[i][j];
                    inputFFT[j] = (int)(input[i][j]);
                    sum += input[i][j];
                }

                means[i] = sum / Extractor.inputColumnSize;   //mean
                total += means[i];  //total mean

                if ((i + 1) % 3 == 0)
                {
                    features[distanceIndex++] = means[i - 2] - means[i - 1];
                    features[distanceIndex++] = means[i - 2] - means[i];
                    features[distanceIndex++] = means[i - 1] - means[i];
                }


             
                //fill variance
                variance = 0;
                for (j = 0; (j < Extractor.inputColumnSize); j++)
                {
                    variance += Math.Pow(input[i][j] - means[i], 2);
                    //***mean subtracted
                    standardized[i][j] = input[i][j] - means[i]; //mean subtracted

                }
                features[varianceIndex++] = variance / (Extractor.inputColumnSize - 1);

                //calculate the range
                features[rangeIndex++] = Math.Abs(max - min);

                //calculate FFT                
                FFT.Calculate(inputFFT);
               
                
                features[energyIndex++] = FFT.Energy;
                double[] maxFreqs = FFT.MaximumFrequencies;

                for (int k = 0; (k < maxFreqs.Length); k++)
                {
                    features[fftIndex++] = maxFreqs[k++];
                    features[fftIndex++] = maxFreqs[k];
                }

                //JFFT jfft = new JFFT(128);
                //jfft.transform(inputFFTD);       //check if this instruction corrupts values of data
                //double[] jreal = jfft.getReal();
                //double[] jim = jfft.getImaginary();               //imaginary part is the negative of the matlab result
                //jfft.computeMagnitudeAngle();
                //double[] jmag = jfft.getMagnitude();
                //double[] mymag = FFT.Magnitudes;


                //***correlation coefficients
                for (int k = i - 1; k >= 0; k--)
                {
                    for (int w = 0; (w < Extractor.inputColumnSize); w++)
                        features[correlationIndex] += standardized[i][w] * standardized[k][w];
                    features[correlationIndex] /= (Extractor.inputColumnSize - 1);
                    features[correlationIndex] /= Math.Sqrt(features[varianceIndex - 1]);  // ith std deviation
                    features[correlationIndex] /= Math.Sqrt(features[varianceIndex - 1 - (i - k)]);  //kth std deviation 
                    correlationIndex++;
                }

            }

            features[inputRowSize] = total;
        }

        
        //public static string GetArffHeader(int inputRowSize, int fftMaximumFrequencies)
        public static string GetArffHeader()
        {
            string DISTANCE_ATTRIBUTES = "";
            string TOTAL_ATTRIBUTE = "";
            string VARIANCE_ATTRIBUTES = "";
            string RANGE_ATTRIBUTES = "";
            string FFT_ATTRIBUTES = "";
            string ENERGY_ATTRIBUTES = "";
            string CORRELATION_ATTRIBUTES = "";

            int distanceIndex = 0;//number of means on every row + 1 for total mean, 0 based index
            int varianceIndex = distanceIndex + inputRowSize + 1; // add number of distances
            int rangeIndex = varianceIndex + inputRowSize;
            int fftIndex = rangeIndex + inputRowSize;
            int energyIndex = fftIndex + (2 * fftMaximumFrequencies * inputRowSize);
            int correlationIndex = energyIndex + inputRowSize; //add number of variances   

            for (int i = 0; (i < inputRowSize); i++)
            {
                if ((i + 1) % 3 == 0)
                {
                    DISTANCE_ATTRIBUTES += "@ATTRIBUTE Dist_Mean" + (i - 2) + "_Mean" + (i - 1) + " NUMERIC\n";
                    arffAttriburesLabels[distanceIndex++] = "Dist_Mean" + (i - 2) + "_Mean" + (i - 1);
                    DISTANCE_ATTRIBUTES += "@ATTRIBUTE Dist_Mean" + (i - 2) + "_Mean" + (i) + " NUMERIC\n";
                    arffAttriburesLabels[distanceIndex++] = "Dist_Mean" + (i - 2) + "_Mean" + (i);
                    DISTANCE_ATTRIBUTES += "@ATTRIBUTE Dist_Mean" + (i - 1) + "_Mean" + (i) + " NUMERIC\n";
                    arffAttriburesLabels[distanceIndex++] = "Dist_Mean" + (i - 1) + "_Mean" + (i);
                }
               

                VARIANCE_ATTRIBUTES += "@ATTRIBUTE Variance_" + i + " NUMERIC\n";
                arffAttriburesLabels[varianceIndex++] = "Variance_" + i;

                RANGE_ATTRIBUTES += "@ATTRIBUTE RANGE_" + i + " NUMERIC\n";
                arffAttriburesLabels[rangeIndex++] = "RANGE_" + i;
                for (int k = 0; (k < (fftMaximumFrequencies*2)); k++)
                {
                    k++;
                    FFT_ATTRIBUTES += "@ATTRIBUTE FFT_MAX_FREQ" + i + "_" + k + " NUMERIC\n";
                    arffAttriburesLabels[fftIndex++] = "FFT_MAX_FREQ" + i + "_" + k;
                    FFT_ATTRIBUTES += "@ATTRIBUTE FFT_MAX_MAG" + i + "_" + k + " NUMERIC\n";
                    arffAttriburesLabels[fftIndex++] = "FFT_MAX_MAG" + i + "_" + k;
                }

                ENERGY_ATTRIBUTES += "@ATTRIBUTE ENERGY_" + i + " NUMERIC\n";
                arffAttriburesLabels[energyIndex++] = "ENERGY_" + i;

                //***correlation coefficients
                for (int k = i - 1; k >= 0; k--)
                {
                    CORRELATION_ATTRIBUTES += "@ATTRIBUTE CORRELATION_" + k + "_" + i + " NUMERIC\n";
                    arffAttriburesLabels[correlationIndex++] = "CORRELATION_" + k + "_" + i;
                }
            }
            
            TOTAL_ATTRIBUTE += "@ATTRIBUTE TotalMean NUMERIC\n";
            arffAttriburesLabels[distanceIndex] = "TotalMean";

            return DISTANCE_ATTRIBUTES + TOTAL_ATTRIBUTE + VARIANCE_ATTRIBUTES + RANGE_ATTRIBUTES +
               FFT_ATTRIBUTES + ENERGY_ATTRIBUTES + CORRELATION_ATTRIBUTES;
        }

        public static string toString()
        {
            string s = "";
            int i = 0;           

            for (i = 0; (i < features.Length - 1); i++)
                s += features[i].ToString("F3") + ",";
            s += features[i].ToString("F3");
            return s;

        }

    }
}
