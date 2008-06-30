using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Schema;

namespace BodyXML
{
    public class Parser
    {
        public const String DEFAULT_OUTPUT = "bodyparts.xml"; 
        private String xmlFile;

        public Parser(String masterDirectory)
        {
            xmlFile = masterDirectory + "\\" + DEFAULT_OUTPUT;
        }

        public BodyParts parse()
        {
            BodyParts bodyparts = new BodyParts();
            XmlDocument doc = new XmlDocument();
            doc.Load(this.xmlFile);
            XmlNode xNode = doc.DocumentElement;

            if ((xNode.Name == Constants.FILENAME) && (xNode.HasChildNodes))
            {
                foreach (XmlNode node in xNode.ChildNodes)
                {
                    //parse BODYPART tags
                    if (node.Name == Constants.BODYPART_ELEMENT)
                    {
                        BodyPart bodypart = new BodyPart(node.Attributes["NAME"].Value);
                        //parse ORIENTATION tags
                        foreach (XmlNode child in node.ChildNodes)
                        {
                            if (child.Name == Constants.ORIENTATION_ELEMENT)
                            {
                                Orientation o = new Orientation();
                                //fill in orientation objects based on imagefile, label, desc tags
                                foreach (XmlNode grandchild in child.ChildNodes)
                                {
                                    if (grandchild.Name == Constants.IMAGEFILE_ELEMENT)
                                    {
                                        o.Imagefile = grandchild.Attributes["NAME"].Value;
                                    }
                                    else if (grandchild.Name == Constants.DESCRIPTION_ELEMENT)
                                    {
                                        o.Description = grandchild.Attributes["NAME"].Value;
                                    }
                                    else if (grandchild.Name == Constants.LABEL_ELEMENT)
                                    {
                                        o.Label = grandchild.Attributes["NAME"].Value;
                                    }
                                }
                                bodypart.Orientations.Add(o);
                            }
                        }
                        bodyparts.Bodyparts.Add(bodypart);
                    }
                }
            }

            return bodyparts;
        }

        public void writeXMLFile(BodyParts input)
        {
            StreamWriter writer = new StreamWriter(DEFAULT_OUTPUT);
            String xml = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\n";
            xml += input.toXML();
            writer.Write(xml);
            writer.Close();
        }
    }
}
