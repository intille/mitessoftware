using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using HousenCS.MITes;
using MITesFeatures;
using AXML;
using SXML;
using System.Xml;
using System.Xml.Schema;

namespace MITesFeatures.Utils.DTs
{
    public class HierarchicalClassifier
    {
        public static string DEFAULT_HIERARCHY_FILE = "hierarchy.xml";
        private string dataDirectory;
        private Hashtable classifiers;


        public HierarchicalClassifier(MITesDecoder aMITesDecoder, Annotation aannotation, SensorAnnotation sannotation, string dataDirectory)
        {
            this.classifiers = new Hashtable();
            this.dataDirectory = dataDirectory;
            Extractor.Initialize(aMITesDecoder, dataDirectory, aannotation, sannotation);
        }


        public void Build()
        {
            XmlDocument dom = new XmlDocument();
            dom.Load(this.dataDirectory + "\\" + DEFAULT_HIERARCHY_FILE);
            XmlNode xNode = dom.DocumentElement;

            if ((xNode.Name == Constants.CLASSIFIERS_ELEMENT) && (xNode.HasChildNodes))
            {
               
                foreach (XmlNode iNode in xNode.ChildNodes)
                {
                    DTClassifier classifier = new DTClassifier();
                    if (iNode.Name == Constants.CLASSIFIER_ELEMENT)
                    {
                        foreach (XmlAttribute iAttribute in iNode.Attributes)
                        {
                            if (iAttribute.Name == Constants.NAME_ATTRIBUTE)
                                classifier.Name = iAttribute.Value;
                            else if (iAttribute.Name == Constants.FILE_ATTRIBUTE)
                                classifier.Filename = iAttribute.Value;
                        }

                        foreach (XmlNode jNode in iNode.ChildNodes)
                        {
                            if (jNode.Name == Constants.FEATURES_ELEMENT)
                            {
                                foreach (XmlNode kNode in jNode.ChildNodes)
                                {
                                    foreach (XmlAttribute kAttribute in kNode.Attributes)
                                    {
                                        if (kAttribute.Name == Constants.NAME_ATTRIBUTE)
                                            classifier.Features.Add(kAttribute.Value);
                                    }
                                }
                            }
                            else if (jNode.Name == Constants.CLASSES_ELEMENT)
                            {
                                foreach (XmlNode kNode in jNode.ChildNodes)
                                {
                                    foreach (XmlAttribute kAttribute in kNode.Attributes)
                                    {
                                        if (kAttribute.Name == Constants.NAME_ATTRIBUTE)
                                            classifier.Classes.Add(kAttribute.Value);
                                    }
                                }
                            }
                        }
                    }

                    classifier.Build();
                    this.classifiers.Add(classifier.Name, classifier);
                }
            }

        }

        public string Classify(double lastTimestamp)
        {
            bool terminal=false;
            string predicted_class=null;
            DTClassifier nextClassifier=(DTClassifier)this.classifiers[Constants.ROOT_CLASSIFIER];
            do
            {
                predicted_class = nextClassifier.Classify(lastTimestamp);
                //check if the classifier returned null or if it is a terminal classification               
                if (this.classifiers.Contains(predicted_class) == true)
                    nextClassifier = (DTClassifier)this.classifiers[predicted_class];
                else
                    terminal = true;
            } while (terminal == false);
            return predicted_class;
        }

        public string toXML()
        {
            string xml = "<" + Constants.CLASSIFIERS_ELEMENT + ">\n";
            foreach(DictionaryEntry classifier in this.classifiers)
            {
                xml += ((DTClassifier)classifier.Value).toXML()+"\n";
            }
            return xml + "</" + Constants.CLASSIFIERS_ELEMENT + ">\n";
        }
        public Hashtable Classifiers
        {
            get
            {
                return this.classifiers;
            }
        }
    }
}
