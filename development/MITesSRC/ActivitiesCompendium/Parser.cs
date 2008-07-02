using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Xml;
using System.Xml.Schema;

namespace ActivitiesCompendium
{
    public class Parser
    {      
        private ArrayList activities;
        private String xmlFile;

        public Parser(String input)
        {
            activities = new ArrayList();
            xmlFile = input;
        }

        public ArrayList Activities
        {
            get
            {
                return this.activities;
            }
        }


        public ArrayList parseXML()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(this.xmlFile);
            XmlNode xNode = doc.DocumentElement;

            if (xNode.Name == "ACTIVITIES" && xNode.HasChildNodes)
            {
                foreach (XmlNode child in xNode.ChildNodes)
                {
                    Activity a = new Activity();
                    a.Name = child.Attributes["VALUE"].Value;
                    foreach (XmlNode grandchild in child.ChildNodes)
                    {
                        if (grandchild.Name == "CODE")
                        {
                            a.Code = grandchild.Attributes["VALUE"].Value;
                        }
                        else if (grandchild.Name == "METS")
                        {
                            a.Mets = grandchild.Attributes["VALUE"].Value;
                        }
                        else if (grandchild.Name == "TYPE")
                        {
                            a.Type = grandchild.Attributes["VALUE"].Value;
                        }
                        else if (grandchild.Name == "DESCRIPTION")
                        {
                            a.Description = grandchild.Attributes["VALUE"].Value;
                        }
                    }
                    activities.Add(a);
                }
            }
            return activities;
        }
    }
}
