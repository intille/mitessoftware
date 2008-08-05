using System;
using System.Collections.Generic;
using System.Text;
using MITesFeatures;
using System.IO;
using weka.core;
using weka.classifiers;
using weka.classifiers.trees;
using weka.filters;
using weka.filters.unsupervised.instance;
using AXML;
namespace TestApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            AXML.Reader areader1 = new AXML.Reader("..\\NeededFiles\\Master\\", @"C:\MITesData\SamplePLFormat", "Copy of ActivityLabels.xml");
            AXML.Reader areader2 = new AXML.Reader("..\\NeededFiles\\Master\\", @"C:\MITesData\SamplePLFormat", "ActivityLabels.xml");
            AXML.Annotation annotation1 = areader1.parse();
            AXML.Annotation annotation2 = areader2.parse();
            AXML.Annotation interesection = annotation1.Intersect(annotation2);
            AXML.Annotation difference1 = annotation1.Difference(annotation2);
            AXML.Annotation difference2 = annotation2.Difference(annotation1);
            TextWriter tw = new StreamWriter(@"C:\MITesData\SamplePLFormat\intersection.xml");
            tw.WriteLine(interesection.ToXML());
            tw.Close();
            tw = new StreamWriter(@"C:\MITesData\SamplePLFormat\Difference1.xml");
            tw.WriteLine(difference1.ToXML());
            tw.Close();

            tw = new StreamWriter(@"C:\MITesData\SamplePLFormat\Difference2.xml");
            tw.WriteLine(difference2.ToXML());
            tw.Close();


            //Generates an Arff file from a PlaceLab format file
           // Extractor.toARFF(@"..\NeededFiles\SamplePLFormat",
             //   "..\\NeededFiles\\Master\\", 3);



            //Extractor.toARFF(@"C:\MITesData\SamplePLFormat",
            //    "..\\NeededFiles\\Master\\", 3);


           // Evaluator e = new Evaluator(@"C:\MITesData\Edith-07-24-2008\all-1second-0.50loss-5cons.arff");
           // e.EvaluateIncrementalBatches(10);

            //Instances data = new Instances(new StreamReader(@"C:\MITesData\Edith-07-09-2008\all-2second-0.20loss-5cons.arff"));
            //data.ClassIndex = data.numAttributes() - 1;
            //Randomize randomizeFilter = new Randomize();
            //randomizeFilter.setInputFormat(data);


            //Instances training = Filter.useFilter(data, randomizeFilter);
            //RemoveFolds foldsFilter = new RemoveFolds();
            //foldsFilter.set_Fold(1);
            //foldsFilter.set_NumFolds(10);
            //foldsFilter.set_InvertSelection(false);
            //foldsFilter.inputFormat(training);
            //training = Filter.useFilter(training, foldsFilter);

            //J48 tree = new J48();         // new instance of tree
            //tree.set_MinNumObj(2);
            //tree.set_ConfidenceFactor((float)0.25);
            //tree.buildClassifier(data);   // build classifier

            //Evaluation eval = new Evaluation(data);
            //eval.crossValidateModel(tree, data, 10, new Random(1));
            
            
            //eval.
            //Console.WriteLine(eval.toSummaryString("\nResults\n=======\n", false));


        }
    }
}
