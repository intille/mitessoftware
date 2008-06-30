using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.IO;

using MITesFeatures;
using weka.core;
using weka.classifiers;
using weka.classifiers.trees;

namespace MITesFeatures.Utils.DTs
{
    public class DTClassifier
    {
        private string name;
        private string filename;
        private ArrayList features;
        private ArrayList classes;
        private int[] featureIndex;
        Classifier classifier;
        Instances instances;



        public DTClassifier()
        {                   
            this.features = new ArrayList();
            this.classes = new ArrayList();
        }

        public string toXML()
        {
            string classifierXML = "<" + Constants.CLASSIFIER_ELEMENT + " " +
                Constants.NAME_ATTRIBUTE+"=\""+this.name+"\" "+Constants.FILE_ATTRIBUTE+"=\""+
                this.filename+"\" >\n";
            classifierXML += "<" + Constants.FEATURES_ELEMENT + ">\n";
            for (int i = 0; (i < this.features.Count); i++)            
                classifierXML += "<" + Constants.FEATURE_ELEMENT + " " + Constants.NAME_ATTRIBUTE +
                    "=\"" + (string)this.features[i] + "\" />\n";
            classifierXML += "</" + Constants.FEATURES_ELEMENT + ">\n";

            classifierXML += "<" + Constants.CLASSES_ELEMENT + ">\n";
            for (int i = 0; (i < this.classes.Count); i++)
                classifierXML += "<" + Constants.CLASS_ELEMENT + " " + Constants.NAME_ATTRIBUTE +
                    "=\"" + (string)this.classes[i] + "\" />\n";
            classifierXML += "</" + Constants.CLASSES_ELEMENT + ">\n";
            classifierXML += "</" + Constants.CLASSIFIER_ELEMENT + ">\n";
            
            return classifierXML;
        }
        public void Build()
        {
            this.featureIndex = new int[this.features.Count];
            for(int i=0;(i< this.features.Count);i++)
            {
                for (int j = 0; (j < Extractor.ArffAttributeLabels.Length); j++)
                    if (((string)this.features[i]).Equals(Extractor.ArffAttributeLabels[j]))
                    {
                        this.featureIndex[i] = j;
                        break;
                    }
            }
            instances = new Instances(new StreamReader(this.filename));
            instances.Class = instances.attribute(this.features.Count);
            classifier = new J48();
            classifier.buildClassifier(instances);
            
            //setup the feature vector 
            //fvWekaAttributes = new FastVector(this.features.Count + 1);
            //for (i = 0; (i < this.features.Count); i++)
            //    fvWekaAttributes.addElement(new weka.core.Attribute(this.features[i]));
            
            
            //Setup the class attribute
            //FastVector fvClassVal = new FastVector();
            //for (i = 0; (i < this.classes.Count); i++)           
             //   fvClassVal.addElement(this.classes[i]);            
            //weka.core.Attribute ClassAttribute = new weka.core.Attribute("activity", fvClassVal);
        }

        public string Classify(double lastTimestamp)
        {
            string predicted_class = null;
            //attempt to generate a feature vector
            if (Extractor.GenerateFeatureVector(lastTimestamp))
            {
                Instance newinstance = new Instance(instances.numAttributes());
                newinstance.Dataset = instances;
                for (int i = 0; (i < this.features.Count); i++)
                    newinstance.setValue(instances.attribute(i), Extractor.Features[this.featureIndex[i]]);
                double predicted = classifier.classifyInstance(newinstance);
                predicted_class = newinstance.dataset().classAttribute().value_Renamed((int)predicted);                
            }
            return predicted_class;
        }

        public int[] FeatureIndex
        {
            get
            {
                return this.FeatureIndex;
            }
        }
        public ArrayList Features
        {
            get
            {
                return this.features;
            }
        }

        public ArrayList Classes
        {
            get
            {
                return this.classes;
            }
        }
        public string Name
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

        public string Filename
        {
            get
            {
                return this.filename;
            }
            set
            {
                this.filename = value;
            }
        }
    }
}
