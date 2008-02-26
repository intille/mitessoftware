using System;
using System.Collections.Generic;
using System.Text;

namespace HousenCS.MITes
{
    /// <summary>
    /// This class keeps track of the expected and actual sampling rates for a MITes sensor
    /// 
    /// </summary>
    public class MITesPerformanceStats
    {
        /// <summary>
        /// 
        ///  
        /// </summary>
        /// 

        public  int goodRate;
        private int sampleCounter;
        private int previousCounter;

        /// <summary>
        /// 
        ///  
        /// </summary>
        public MITesPerformanceStats(int goodRate)
        {

            this.goodRate = goodRate;
            this.sampleCounter = 0;
            this.previousCounter = 0;
        }

        /// <summary>
        /// 
        ///  
        /// </summary>
        /// 
        public int PreviousCounter
        {
            get
            {
                return this.previousCounter;
            }
            set
            {
                this.previousCounter = value;
            }
        }

        /// <summary>
        /// 
        ///  
        /// </summary>
        /// 
        public int GoodRate
        {
            get
            {
                return this.goodRate;
            }
            set
            {
                this.goodRate = value;
            }
        }



        /// <summary>
        /// 
        ///  
        /// </summary>
        public int SampleCounter
        {
            get
            {
                return this.sampleCounter;
            }
            set
            {
                this.sampleCounter= value;
            }
        }


    }
}
