using System;
using System.Collections.Generic;
using System.Text;

namespace MITesFeatures.core.conf
{
    public class Constants
    {
        public static string CONFIGURATION_ELEMENT = "CONFIGURATION";
        public static string WINDOW_ELEMENT = "WINDOW";
        public static string SAMPLING_ELEMENT = "SAMPLING";
        public static string ERROR_ELEMENT = "ERROR";
        public static string FFT_ELEMENT = "FFT";
        public static string TRAINING_ELEMENT = "TRAINING";
        public static string CLASSIFIER_ELEMENT = "CLASSIFIER";
        public static string SOFTWARE_ELEMENT = "SOFTWARE";
        public static string QUALITY_ELEMENT = "QUALITY";

        public static string SOFTWARE_MODE_ATTRIBUTE = "mode";
        public static string SOFTWARE_MODE_RELEASE = "release";
        public static string SOFTWARE_MODE_TEST = "test";

        public static string QUALITY_WINDOW_SIZE = "window_size";

        public static string WINDOW_TIME_ATTRIBUTE = "time";
        public static string WINDOW_OVERLAP_ATTRIBUTE = "overlap";
        public static string SAMPLING_RATE_ATTRIBUTE = "rate";
        public static string CONSECUTIVE_ERROR_ATTRIBUTE = "consecutive";
        public static string NONCONSECUTIVE_ERROR_ATTRIBUTE = "nonconsecutive";
        public static string INTERPOLATION_POWER_ATTRIBUTE = "interpolation_power";
        public static string MAX_FREQUENCIES_ATTRIBUTE = "maximum_frequencies";
        public static string WAIT_TIME_ATTRIBUTE = "wait_time";
        public static string TRAINING_TIME_ATTRIBUTE = "training_time";
        public static string SMOOTH_WINDOW_ATTRIBUTE = "smooth_windows";

    }
}
