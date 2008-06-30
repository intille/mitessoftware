using System;
using System.Collections.Generic;
using System.Text;

namespace BodyXML
{
    public class Orientation
    {
        private String imagefile;
        private String description;
        private String label;

        public Orientation()
        {
        }

        public Orientation(String file, String desc, String name)
        {
            this.imagefile = file;
            this.description = desc;
            this.label = name;
        }

        public String Imagefile
        {
            get
            {
                return this.imagefile;
            }
            set
            {
                this.imagefile = value;
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

        public String Label
        {
            get
            {
                return this.label;
            }
            set
            {
                this.label = value;
            }
        }

        public String toXML()
        {
            String xml = "";
            xml += "<" + Constants.ORIENTATION_ELEMENT + ">\n";
            xml += "<" + Constants.IMAGEFILE_ELEMENT + " " + Constants.NAME_ATTRIBUTE +
                    "=\"" + this.imagefile + "\"/>\n";
            xml += "<" + Constants.DESCRIPTION_ELEMENT + " " + Constants.NAME_ATTRIBUTE +
                    "=\"" + this.description + "\"/>\n";
            xml += "<" + Constants.LABEL_ELEMENT + " " + Constants.NAME_ATTRIBUTE +
                    "=\"" + this.label + "\"/>\n";
            xml += "</" + Constants.ORIENTATION_ELEMENT + ">\n";
            return xml;
        }

    }
}
