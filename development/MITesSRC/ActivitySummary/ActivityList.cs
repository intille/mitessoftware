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

        public void reset()
        {
            int count = 0;
            foreach (Activity a in this.activities)
            {
                if (a.Name != "empty2")
                {
                    a.Percent = 1;
                    count++;
                }
            }

            setPercent("empty2", 100 - count);                

        }
        public void setPercent(string activity, int percent)
        {
            foreach (Activity a in this.activities)
            {
                if (a.Name == activity)
                {
                    a.Percent = percent;
                    return;
                }
            }
        }

        public void decrement(string activity)
        {
            foreach (Activity a in this.activities)
            {
                if (a.Name == activity)
                {
                    a.Percent = a.Percent - 1;
                    return;
                }
            }
        }

        public void increment(string activity)
        {
            foreach (Activity a in this.activities)
            {
                if (a.Name == activity)
                {
                    a.Percent = a.Percent+1;
                    decrement("empty2");
                    return;
                }
            }
        }

        public int getEmptyPercent()
        {
            foreach (Activity a in this.activities)
                if (a.Name == "empty2")
                    return a.Percent;
            return 0;
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
