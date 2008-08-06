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
using weka.filters.unsupervised.attribute;
using AXML;
namespace TestApplication
{
    class Program
    {
        static void Main(string[] args)
        {
           /* AXML.Reader areader1 = new AXML.Reader("..\\NeededFiles\\Master\\", @"C:\MITesData\SamplePLFormat", "Copy of ActivityLabels.xml");
            AXML.Reader areader2 = new AXML.Reader("..\\NeededFiles\\Master\\", @"C:\MITesData\SamplePLFormat", "ActivityLabels.xml");
            AXML.Annotation annotation1 = areader1.parse();
            AXML.Annotation annotation2 = areader2.parse();
            AXML.Annotation interesection = annotation1.Intersect(annotation2);
            AXML.Annotation difference = annotation1.Difference(annotation2);
            
            TextWriter tw = new StreamWriter(@"C:\MITesData\SamplePLFormat\intersection.xml");
            tw.WriteLine(interesection.ToXML());
            tw.Close();
            tw = new StreamWriter(@"C:\MITesData\SamplePLFormat\Difference.xml");
            tw.WriteLine(difference.ToXML());
            tw.Close();
            */



            //Generates an Arff file from a PlaceLab format file
            Extractor.toARFF(@"C:\MITesData\SamplePLFormat",
             "..\\NeededFiles\\Master\\", 3,"ActivityLabels",2);

            Instances data = new Instances(new StreamReader(@"C:\MITesData\SamplePLFormat\output.arff"));
            
            //Filter labels unknown, gooddata,a_flaprock_maybe
            RemoveWithValues preprocessFilter = new RemoveWithValues();
            int annotatorAttribute = data.numAttributes();
            preprocessFilter.set_AttributeIndex(annotatorAttribute.ToString());
            preprocessFilter.set_InvertSelection(false);
            preprocessFilter.set_NominalIndices("4");
            preprocessFilter.setInputFormat(data);
            data = Filter.useFilter(data, preprocessFilter);

            preprocessFilter = new RemoveWithValues();
            annotatorAttribute = data.numAttributes()-1;
            preprocessFilter.set_AttributeIndex(annotatorAttribute.ToString());
            preprocessFilter.set_InvertSelection(false);
            preprocessFilter.set_NominalIndices("4");
            preprocessFilter.setInputFormat(data);
            data = Filter.useFilter(data, preprocessFilter);

            
            //Filter intersection instances
            int intersectionAttribute = data.numAttributes() - 2;                        
            RemoveWithValues intersectionFilter = new RemoveWithValues();
            intersectionFilter.set_AttributeIndex(intersectionAttribute.ToString());
            intersectionFilter.set_InvertSelection(false);
            intersectionFilter.set_SplitPoint(1.0);
            intersectionFilter.setInputFormat(data);
            Instances agreementData = Filter.useFilter(data, intersectionFilter);


            
            //remove redundant annotator and annotator agreement flag
            Remove removeAttributeFilter = new Remove();
            int[] removeAttribute= new int[2];
            removeAttribute[0] = data.numAttributes()-1;
            removeAttribute[1] = data.numAttributes() - 3;          
            removeAttributeFilter.set_InvertSelection(false);
            removeAttributeFilter.SetAttributeIndicesArray(removeAttribute);         
            //NOTE, you have to set the attribute indicies before the inputFormat to do
            //the filtering
            removeAttributeFilter.setInputFormat(agreementData);          
            agreementData = Filter.useFilter(agreementData, removeAttributeFilter);
            agreementData.ClassIndex = agreementData.numAttributes() - 1;



            //filter instances in disagreement
            intersectionFilter = new RemoveWithValues();
            intersectionFilter.set_AttributeIndex(intersectionAttribute.ToString());
            intersectionFilter.set_InvertSelection(true);
            intersectionFilter.set_SplitPoint(1.0);
            intersectionFilter.setInputFormat(data);
            
            //remove redundant annotator and annotator agreement
            Instances disagreementData = Filter.useFilter(data, intersectionFilter);            
            removeAttributeFilter = new Remove();            
            removeAttribute = new int[2];
            removeAttribute[0] = data.numAttributes() - 1;
            removeAttribute[1] = data.numAttributes() - 3;
            removeAttributeFilter.set_InvertSelection(false);
            removeAttributeFilter.SetAttributeIndicesArray(removeAttribute);
            removeAttributeFilter.setInputFormat(disagreementData);  
            disagreementData = Filter.useFilter(disagreementData, removeAttributeFilter);
            disagreementData.ClassIndex = disagreementData.numAttributes() - 1;


            //Here I have agreement and disagreement data


            //Testing the disagreement portion
            J48 tree = new J48();         // new instance of tree
            tree.set_MinNumObj(10);
            tree.set_ConfidenceFactor((float)0.25);
            tree.buildClassifier(agreementData);   // build classifier


            //Generates the confusion matrix for the disagreement portion of the data
            int numClasses=disagreementData.attribute(disagreementData.numAttributes() - 1).numValues();
            int[,] confusionMatrix=new int[numClasses,numClasses];
            for (int j = 0; (j < disagreementData.numInstances()); j++)
            {

                Instance newinstance = disagreementData.instance(j);
                newinstance.Dataset = disagreementData;
                double predicted = tree.classifyInstance(newinstance);
                string predicted_activity = newinstance.dataset().classAttribute().value_Renamed((int)predicted);
                if (predicted == newinstance.classValue())
                    confusionMatrix[(int)predicted, (int)predicted] = confusionMatrix[(int)predicted, (int)predicted] + 1;
                else
                    confusionMatrix[(int)newinstance.classValue(), (int)predicted] = confusionMatrix[(int)newinstance.classValue(), (int)predicted] + 1;

            }

            Evaluation eval = new Evaluation(agreementData);
            eval.evaluateModel(tree, disagreementData);//.crossValidateModel(tree, data, 10, new Random(1));


            int[,] confusionMatrix2 = new int[numClasses, numClasses];
            int numFolds = 10;

            for (int i = 1; (i <= numFolds); i++)
            {
                //Training folds filter
                RemoveFolds trainingFoldsFilter = new RemoveFolds();
                trainingFoldsFilter.set_NumFolds(numFolds);
                trainingFoldsFilter.inputFormat(agreementData);
                trainingFoldsFilter.set_InvertSelection(true);
                trainingFoldsFilter.set_Fold(i);
                Instances alltraining = Filter.useFilter(agreementData, trainingFoldsFilter);

                RemoveFolds testFoldsFilter = new RemoveFolds();
                testFoldsFilter.set_NumFolds(numFolds);
                testFoldsFilter.inputFormat(agreementData);
                testFoldsFilter.set_InvertSelection(false);
                trainingFoldsFilter.set_Fold(i);
                Instances test = Filter.useFilter(agreementData, testFoldsFilter);
            }
            //Generate the folds
            Evaluation eval2 = new Evaluation(agreementData);
            eval2.crossValidateModel(tree, agreementData, 10, new Random(1));



           // Instances intersectionData=



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
