using System;
using System.Text;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;

namespace ActivitiesCompendium
{
    public class DataRetrieval
    {
        private ArrayList activities;   //list of all activities from XML file
        private SortedDictionary<int, int> codeTable;   //sorted by code
        private SortedDictionary<double, ArrayList> metsTable;  //sorted by mets
        private SortedDictionary<string, ArrayList> typeTable;      //sorted by category

        public DataRetrieval(ArrayList input)
        {
            activities = input;
            codeTable = new SortedDictionary<int, int>();
            metsTable = new SortedDictionary<double, ArrayList>();
            typeTable = new SortedDictionary<string, ArrayList>();
            populateTables();
        }

        /**Iterates through arraylist of activities and organizes data first by code, then
         * by mets, and finally by type.
         * */
        private void populateTables()
        {
            for (int i = 0; i < activities.Count; i++)
            {
                //populates code table: key = code, value = index
                codeTable.Add(int.Parse(((Activity)activities[i]).Code), i);

                //populates mets table: key = met, value = arraylist of indices 
                double met = double.Parse(((Activity)activities[i]).Mets); 
                if (metsTable.ContainsKey(met))
                {
                    metsTable[met].Add(i);
                }
                else
                {
                    ArrayList temp = new ArrayList();
                    temp.Add(i);
                    metsTable.Add(met, temp);
                }

                //populates type table: key = type, value = arraylist of indices
                String type = ((Activity)activities[i]).Type;
                if (typeTable.ContainsKey(type))
                {
                    typeTable[type].Add(i);
                }
                else
                {
                    ArrayList temp = new ArrayList();
                    temp.Add(i);
                    typeTable.Add(type, temp);
                }
            }
        }

        /** Takes code and finds the corresponding activity
         * @code: code of the activity 
         * @output: the activity
         * */
        public Activity getByCode(int code)
        {
            return (Activity)activities[codeTable[code]];
        }

        /**look up by mets - one specific met value corresponds to an array of activity indices
         * @input: double representation of met
         * @output: array of activities with that met value
         * */
        public Activity[] getByMet(double met)
        {
            ArrayList temp = new ArrayList();
            foreach (int i in metsTable[met])
            {
                temp.Add((Activity)activities[i]);
            }
            return (Activity[])temp.ToArray(typeof(Activity));
        }

        /**look up by mets - given a lower and upper boundary for suitable met values
         * @lower: double representation of lower met boundary
         * @upper: double representation of upper met boundary
         * @output: array of activities with the appropriate met values
         * */
        public Activity[] getByMet(double lower, double upper)
        {
            ArrayList temp = new ArrayList();
            foreach (double key in metsTable.Keys)
            {
                if (key < lower)
                {
                    continue;
                }
                else if (key > upper)
                {
                    break;
                }
                foreach (int i in metsTable[key])
                {
                    temp.Add((Activity)activities[i]);
                }
            }
            return (Activity[])temp.ToArray(typeof(Activity));
        }

        /**look up by type, returns an array showing all activities belonging 
         * that to that general activity type
         * @type: string representation of type of activities
         * @output: array of activities satisfying the input type
         * */
        public Activity[] getByType(String type)
        {
            ArrayList temp = new ArrayList();
            foreach (int i in typeTable[type])
            {
                temp.Add((Activity)activities[i]);
            }
            return (Activity[])temp.ToArray(typeof(Activity));
        }

        /**look up by types, returns an array showing all activities belonging 
         * that to the array of types passed as the argument
         * @types: array of string representations of types of activities
         * @output: array of activities satisfying the input types
         * */
        public Activity[] getByType(String[] types)
        {
            ArrayList temp = new ArrayList();
            foreach (String type in types)
            {
                foreach (int i in typeTable[type])
                {
                    temp.Add((Activity)activities[i]);
                }
            }
            return (Activity[])temp.ToArray(typeof(Activity));
        }
    }
}
