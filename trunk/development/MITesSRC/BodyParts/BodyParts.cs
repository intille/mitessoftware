using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace BodyXML
{
    public class BodyParts
    {
        private ArrayList bodyparts;

        public BodyParts()
        {
            bodyparts = new ArrayList();
        }

        public ArrayList Bodyparts
        {
            get
            {
                return this.bodyparts;
            }
        }

        public String toXML()
        {
            String xml = "<" + Constants.FILENAME + " dataset=\"house_n data\" xmlns=\"urn:mites-schema\">";
            foreach (BodyPart b in bodyparts)
            {
                xml += b.toXML();
            }
            xml += "</" + Constants.FILENAME + ">\n";
            return xml;
        }
    }
}
