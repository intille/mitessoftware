using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.IO;
using weka.core;
using weka.classifiers;
using weka.classifiers.trees;

namespace MITesDataCollection
{
    public partial class ClassificationForm : Form
    {
        //Classifier classifier;
        //FastVector fvWekaAttributes;
        //Instances instances;

        public ClassificationForm(string arffFile)
        {
            InitializeComponent();

            //instances = new Instances(new StreamReader(arffFile));            
            //instances.Class = instances.attribute(28);
            //classifier = new J48();
            //classifier.buildClassifier(instances);

            //fvWekaAttributes = new FastVector(29);
            ////for (int i = 0; (i < Extractor.ArffAttributeLabels.Length); i++)
            //fvWekaAttributes.addElement(new weka.core.Attribute("Dist_Mean0_Mean1"));
            //fvWekaAttributes.addElement(new weka.core.Attribute("Dist_Mean0_Mean2"));
            //fvWekaAttributes.addElement(new weka.core.Attribute("Dist_Mean1_Mean2"));

            //fvWekaAttributes.addElement(new weka.core.Attribute("TotalMean"));
            //fvWekaAttributes.addElement(new weka.core.Attribute("Variance_0"));
            //fvWekaAttributes.addElement(new weka.core.Attribute("Variance_1"));
            //fvWekaAttributes.addElement(new weka.core.Attribute("Variance_2"));

            //fvWekaAttributes.addElement(new weka.core.Attribute("RANGE_0"));
            //fvWekaAttributes.addElement(new weka.core.Attribute("RANGE_1"));
            //fvWekaAttributes.addElement(new weka.core.Attribute("RANGE_2"));

            //fvWekaAttributes.addElement(new weka.core.Attribute("FFT_MAX_FREQ0_1"));
            //fvWekaAttributes.addElement(new weka.core.Attribute("FFT_MAX_MAG0_1"));
            //fvWekaAttributes.addElement(new weka.core.Attribute("FFT_MAX_FREQ0_3"));
            //fvWekaAttributes.addElement(new weka.core.Attribute("FFT_MAX_MAG0_3"));
            //fvWekaAttributes.addElement(new weka.core.Attribute("FFT_MAX_FREQ1_1"));
            //fvWekaAttributes.addElement(new weka.core.Attribute("FFT_MAX_MAG1_1"));
            //fvWekaAttributes.addElement(new weka.core.Attribute("FFT_MAX_FREQ1_3"));
            //fvWekaAttributes.addElement(new weka.core.Attribute("FFT_MAX_MAG1_3"));
            //fvWekaAttributes.addElement(new weka.core.Attribute("FFT_MAX_FREQ2_1"));
            //fvWekaAttributes.addElement(new weka.core.Attribute("FFT_MAX_MAG2_1"));
            //fvWekaAttributes.addElement(new weka.core.Attribute("FFT_MAX_FREQ2_3"));
            //fvWekaAttributes.addElement(new weka.core.Attribute("FFT_MAX_MAG2_3"));
            //fvWekaAttributes.addElement(new weka.core.Attribute("ENERGY_0"));
            //fvWekaAttributes.addElement(new weka.core.Attribute("ENERGY_1"));
            //fvWekaAttributes.addElement(new weka.core.Attribute("ENERGY_2"));
            //fvWekaAttributes.addElement(new weka.core.Attribute("CORRELATION_0_1"));
            //fvWekaAttributes.addElement(new weka.core.Attribute("CORRELATION_1_2"));
            //fvWekaAttributes.addElement(new weka.core.Attribute("CORRELATION_0_2"));

            //FastVector fvClassVal = new FastVector();
            //fvClassVal.addElement("Chopping");
            //fvClassVal.addElement("Brushing_Teeth");
            //fvClassVal.addElement("Resting");
            //fvClassVal.addElement("Writing");
            //fvClassVal.addElement("unknown");
            //weka.core.Attribute ClassAttribute = new weka.core.Attribute("activity", fvClassVal);

            //fvWekaAttributes.addElement(ClassAttribute);

        }
    }
}