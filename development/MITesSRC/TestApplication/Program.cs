using System;
using System.Collections.Generic;
using System.Text;
using MITesFeatures;
using System.IO;
using weka.core;
using weka.classifiers;
using weka.classifiers.trees;
namespace TestApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            //Generates an Arff file from a PlaceLab format file
           // Extractor.toARFF(@"..\NeededFiles\SamplePLFormat",
             //   "..\\NeededFiles\\Master\\", 3);

          //  Extractor.toARFF(@"C:\MITesData\SamplePLFormat",
           //     "..\\NeededFiles\\Master\\", 3);

            Instances data = new Instances(new StreamReader(@"C:\MITesData\Edith-07-09-2008\all-2second-0.20loss-5cons.arff"));
            data.ClassIndex = data.numAttributes() - 1;
           

            J48 tree = new J48();         // new instance of tree
            tree.set_MinNumObj(2);
            tree.set_ConfidenceFactor((float)0.25);
            tree.buildClassifier(data);   // build classifier

            Evaluation eval = new Evaluation(data);
            eval.crossValidateModel(tree, data, 10, new Random(1));
            Console.WriteLine(eval.toSummaryString("\nResults\n=======\n", false));


        }
    }
}
