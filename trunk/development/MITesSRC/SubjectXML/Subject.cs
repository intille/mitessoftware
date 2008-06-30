using System;
using System.Collections.Generic;
using System.Text;

namespace SubjectXML
{
    public class Subject
    {
        private string id;
        private int age;
        private string sex;
        private double weight;
        private double height;
        private double fatMass;
        private double boneMass;
        private double waterMass;
        private string ethnicity;
        private string shoe;

        public Subject()
        {
        }

        public string ID
        {
            get
            {
                return this.id;

            }
            set
            {
                this.id = value;
            }
        }
        public int Age
        {
            get
            {
                return this.age;
            }

            set
            {
                this.age = value;
            }
        }

        public string Sex
        {
            get
            {
                return this.sex;
            }
            set
            {
                this.sex = value;
            }
        }

        public double Weight
        {
            get
            {
                return this.weight;
            }
            set
            {
                this.weight = value;
            }
        }

        public double Height
        {
            get
            {
                return this.height;
            }
            set
            {
                this.height = value;
            }
        }

        public double FatMass
        {
            get
            {
                return this.fatMass;
            }

            set
            {
                this.fatMass = value;
            }
        }

        public double BoneMass
        {
            get
            {
                return this.boneMass;
            }
            set
            {
                this.boneMass = value;
            }
        }

        public double WaterMass
        {
            get
            {
                return this.waterMass;
            }
            set
            {
                this.waterMass=value;
            }
        }


        public string Ethnicity
        {
            get
            {
                return this.ethnicity;
            }

            set
            {
                this.ethnicity = value;
            }
        }

        public string Shoe
        {
            get
            {
                return this.shoe;
            }
            set
            {
                this.shoe = value;
            }
        }

        public string ToXML()
        {
            string xml = "";

            xml += "<" + Constants.SUBJECT_ELEMENT +" "+Constants.ID_ATTRIBUTE+"=\""+this.id +"\" >\n";
            xml += "<"+ Constants.AGE_ELEMENT+" "+Constants.VALUE_ATTRIBUTE+"=\""+this.age+"\" />\n";
            xml += "<" + Constants.SEX_ELEMENT + " " + Constants.VALUE_ATTRIBUTE + "=\"" + this.sex + "\" />\n";
            xml += "<" + Constants.WEIGHT_ELEMENT + " " + Constants.VALUE_ATTRIBUTE + "=\"" + this.weight + "\" />\n";
            xml += "<" + Constants.HEIGHT_ELEMENT + " " + Constants.VALUE_ATTRIBUTE + "=\"" + this.height+ "\" />\n";
            xml += "<" + Constants.FATMASS_ELEMENT + " " + Constants.VALUE_ATTRIBUTE + "=\"" + this.fatMass + "\" />\n";
            xml += "<" + Constants.BONEMASS_ELEMENT + " " + Constants.VALUE_ATTRIBUTE + "=\"" + this.boneMass + "\" />\n";
            xml += "<" + Constants.WATERMASS_ELEMENT + " " + Constants.VALUE_ATTRIBUTE + "=\"" + this.waterMass + "\" />\n";
            xml += "<" + Constants.ETHNICITY_ELEMENT + " " + Constants.VALUE_ATTRIBUTE + "=\"" + this.ethnicity + "\" />\n";
            xml += "<" + Constants.SHOE_ELEMENT + " " + Constants.TEXT_ATTRIBUTE + "=\"" + this.shoe + "\" />\n";
            xml += "</" + Constants.SUBJECT_ELEMENT + ">\n";

            return xml;

        }
    }
}
