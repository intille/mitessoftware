using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.IO;


namespace ActivitySummary
{
    public class ActivityList
    {
        private ArrayList activities;

        public ActivityList()
        {
            this.activities = new ArrayList();
        }

        public ArrayList Activities
        {
            get
            {
                return this.activities;
            }
        }

        public Hashtable toPercentHashtable()
        {
            Hashtable activities = new Hashtable();
            foreach (Activity a in this.activities)
                activities.Add(a.Name, a.Percent);
            return activities;
        }

        public string toString()
        {
            string output = "<"+Constants.ACTIVITIES_SUMMARY_ELEMENT+">\n";
            foreach (Activity a in this.activities)
                output += a.toString();
            output += "</" + Constants.ACTIVITIES_SUMMARY_ELEMENT + ">\n";            
            return output;
        }

        public void toXML()
        {
            TextWriter tw = new StreamWriter(Reader.DEFAULT_XML_FILE);
            tw.WriteLine(toString());
            tw.Close();
        }
    }
}
