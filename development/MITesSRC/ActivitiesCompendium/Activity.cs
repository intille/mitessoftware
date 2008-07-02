using System;
using System.Collections.Generic;
using System.Text;

namespace ActivitiesCompendium
{
    public class Activity
    {
        private String name;        //label for activity (short)
        private String code;       //code given to activity
        private String mets;       //proportional to activity intensity
        private String type;      //category of the activity
        private String description; //short description of activity

        public Activity()
        {
            name = "";
            code = "";
            mets = "";
            type = "";
            description = "";
        }

        public Activity(String Name, String Code, String Mets, String Type, String Descp)
        {
            this.name = Name;
            this.code = Code;
            this.mets = Mets;
            this.type = Type;
            this.description = Descp;
        }

        public String Name
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

        public String Code
        {
            get
            {
                return this.code;
            }
            set
            {
                this.code = value;
            }
        }

        public String Mets
        {
            get
            {
                return this.mets;
            }
            set
            {
                this.mets = value;
            }
        }

        public String Type
        {
            get
            {
                return this.type;
            }
            set
            {
                this.type = value;
            }
        }

        public String Description
        {
            get
            {
                return this.description;
            }
            set
            {
                this.description = value;
            }
        }
    }
}
