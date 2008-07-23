using System;
using System.Collections.Generic;
using System.Text;

namespace SOXML
{
    public class Constants
    {
        public static string SENSORS_ELEMENT = "SENSORS";
        public static string SENSOR_ELEMENT = "SENSOR";
        public static string ORIENTATION_ELEMENT = "ORIENTATION";
        public static string IMAGE_ELEMENT = "IMAGE";
        public static string AVERAGE_ELEMENT = "AVERAGE";
        public static string ACCELERATIONS_ELEMENT = "ACCELERATIONS";
        public static string ACCELERATION_ELEMENT = "ACCELERATION";

        public static string ID_ATTRIBUTE = "id";
        public static string DESCRIPTION_ATTRIBUTE = "description";
        public static string FILE_ATTRIBUTE = "file";
        public static string X_ATTRIBUTE = "x";
        public static string Y_ATTRIBUTE = "y";
        public static string Z_ATTRIBUTE = "z";
        public static string DIRECTION_ATTRIBUTE = "direction";
        public static string MINX_ATTRIBUTE = "minX";
        public static string MAXX_ATTRIBUTE = "maxX";
        public static string MINY_ATTRIBUTE = "minY";
        public static string MAXY_ATTRIBUTE = "maxY";
        public static string MINZ_ATTRIBUTE = "minZ";
        public static string MAXZ_ATTRIBUTE = "maxZ";

        public static int ACCELERATION_DIRECTIONS = 3;
        public static int SENSOR_ORIENTATIONS = 8;
    }
}
