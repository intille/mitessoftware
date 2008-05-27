using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace AXML
{
    public class AnnotatedRecord
    {
        private ArrayList labels;
        private int start_hour;
        private int start_minute;
        private int start_second;
        private double start_unix;
        private string start_date;
        private string end_date;
        private int end_hour;
        private int end_minute;
        private int end_second;
        private double end_unix;
        private float quality;

        public AnnotatedRecord()
        {
            this.labels=new ArrayList();
        }


        public ArrayList Labels
        {
            get
            {
                return this.labels;
            }
        }
        public float Quality
        {
            get
            {
                return this.quality;
            }

            set
            {
                this.quality = value;
            }
        }
        public string EndDate
        {
            get
            {
                return this.end_date;
            }
            set
            {
                this.end_date = value;
            }
        }

        public string StartDate
        {
            get
            {
                return this.start_date;
            }
            set
            {
                this.start_date = value;
            }
        }

        public double StartUnix
        {
            get
            {
                return this.start_unix;
            }
            set
            {
                this.start_unix = value;
            }
        }

        public double EndUnix
        {
            get
            {
                return this.end_unix;
            }
            set
            {
                this.end_unix = value;
            }
        }
        public int StartHour
        {
            get
            {
                return this.start_hour;
            }
            set
            {
                this.start_hour = value;
            }
        }

        public int StartMinute
        {
            get
            {
                return this.start_minute;
            }

            set
            {
                this.start_minute = value;
            }
        }

        public int StartSecond
        {
            get
            {
                return this.start_second;
            }
            set
            {
                this.start_second = value;
            }
        }



        public int EndHour
        {
            get
            {
                return this.end_hour;
            }
            set
            {
                this.end_hour = value;
            }
        }

        public int EndMinute
        {
            get
            {
                return this.end_minute;
            }

            set
            {
                this.end_minute = value;
            }
        }

        public int EndSecond
        {
            get
            {
                return this.end_second;
            }
            set
            {
                this.end_second = value;
            }
        }

        public string ToXML()
        {
            string xml = "";
            xml += "<" + Constants.LABEL_ELEMENT + " " + Constants.STARTDATE_ATTRIBUTE + "=\"" + this.start_date + "\" " +
                Constants.ENDDATE_ATTRIBUTE + "=\"" + this.end_date + "\" " + Constants.STARTTIME_ATTRIBUTE + "=\"" +
                this.start_hour + ":" + this.start_minute + ":" + this.start_second + "\" " + Constants.ENDTIME_ATTRIBUTE + "=\"" +
                this.end_hour + ":" + this.end_minute + ":" + this.end_second + "\">";

            foreach (Label label in this.labels)
            {
                xml+=label.ToValueXML();
            }
            xml += "</" + Constants.LABEL_ELEMENT + ">\n";
            return xml;
        }


        public string ToCSV()
        {
 
            string csv = "";

            csv += this.start_date + "," + this.end_date;
            foreach (Label label in this.labels)
            {
                csv += ","+label.Name ;
            }
            
            return csv;
        }
    }
}
