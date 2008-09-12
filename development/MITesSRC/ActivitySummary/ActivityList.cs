using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

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
    }
}
