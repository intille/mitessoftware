using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace BodyXML
{
    public class BodyPart
    {
        private ArrayList orientations;
        private String label;

        public BodyPart(String name)
        {
            orientations = new ArrayList();
            label = name;
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

        public ArrayList Orientations
        {
            get
            {
                return this.orientations;
            }
        }

        public String toXML()
        {
            String xml="";
            xml += "<" + Constants.BODYPART_ELEMENT + " " + Constants.NAME_ATTRIBUTE +
                    "=\"" + this.label + "\">\n";
            foreach (Orientation o in orientations)
            {
                xml += o.toXML();
            }
            xml += "</" + Constants.BODYPART_ELEMENT + ">\n";
            return xml;
        }
    }
}
