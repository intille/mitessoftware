using System;
using System.Collections.Generic;
using System.Text;

namespace ActivitySummary
{
    public class Activity
    {
        private string name;
        private double unixStart;
        private double unixEnd;
        private int percent;

        public Activity()
        {
        }

        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                this.name = value;
            }
        }

        public double StartTime
        {
            get
            {
                return this.unixStart;
            }
            set
            {
                this.unixStart = value;
            }
        }

        public double EndTime
        {
            get
            {
                return this.unixEnd;
            }
            set
            {
                this.unixEnd = value;
            }
        }

        public int Percent
        {
            get
            {
                return this.percent;
            }

            set
            {
                this.percent = value;
            }
        }
    }
}
