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
using weka.classifiers.evaluation;
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
            //Extractor.toARFF(@"C:\MITesData\SamplePLFormat",
            // "..\\NeededFiles\\Master\\", 3);


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


            string[] filter = new string[5];
            filter[0] = "maybe";
            filter[1] = "good";
            filter[2] = "annotation";
            filter[3] = "setup";
            filter[4] = "not";
   

            //Generates an Arff file from a PlaceLab format file
            Extractor.toARFF(@"C:\MITesData\SamplePLFormat",
             "..\\NeededFiles\\Master\\", 3, "ActivityLabels", 2, filter);

            Instances data = new Instances(new StreamReader(@"C:\MITesData\SamplePLFormat\output.arff"));
            Instances originalData = new Instances(new StreamReader(@"C:\MITesData\SamplePLFormat\output.arff"));
            int annotaters = 2;
            int[,] labeledData = new int[data.numInstances(), 3]; //each instance has 2 annotators and a classifier

            //Filter labels unknown, gooddata,a_flaprock_maybe
            //RemoveWithValues preprocessFilter = new RemoveWithValues();
            //int annotatorAttribute = data.numAttributes();
            //preprocessFilter.set_AttributeIndex(annotatorAttribute.ToString());
            //preprocessFilter.set_InvertSelection(false);
            //preprocessFilter.set_NominalIndices("1,4");
            //preprocessFilter.setInputFormat(data);    
            //data = Filter.useFilter(data, preprocessFilter);

            //preprocessFilter = new RemoveWithValues();
            //annotatorAttribute = data.numAttributes()-1;
            //preprocessFilter.set_AttributeIndex(annotatorAttribute.ToString());
            //preprocessFilter.set_InvertSelection(false);
            //preprocessFilter.set_NominalIndices("1,4");
            //preprocessFilter.setInputFormat(data);
            //data = Filter.useFilter(data, preprocessFilter);


            //Filter intersection instances
            int intersectionAttribute = data.numAttributes() - 2;
            RemoveWithValues intersectionFilter = new RemoveWithValues();
            intersectionFilter.set_AttributeIndex(intersectionAttribute.ToString());
            intersectionFilter.set_InvertSelection(false);
            intersectionFilter.set_SplitPoint(1.0);
            intersectionFilter.setInputFormat(data);
            Instances agreementData = Filter.useFilter(data, intersectionFilter);

            TextWriter tw3 = new StreamWriter(@"C:\MITesData\SamplePLFormat\agreement.arff");
            tw3.WriteLine(agreementData.ToString());
            tw3.Close();
            //remove redundant annotator and annotator agreement flag
            Remove removeAttributeFilter = new Remove();
            int[] removeAttribute = new int[2];
            removeAttribute[0] = data.numAttributes() - 1;
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
            Instances[] disagreementData = new Instances[annotaters];
            for (int i = 0; (i < annotaters); i++)
            {
                disagreementData[i] = Filter.useFilter(data, intersectionFilter);
                removeAttributeFilter = new Remove();
                removeAttribute = new int[2];
                removeAttribute[0] = data.numAttributes() - 1 - i; //remove labels
                removeAttribute[1] = data.numAttributes() - 3;
                removeAttributeFilter.set_InvertSelection(false);
                removeAttributeFilter.SetAttributeIndicesArray(removeAttribute);
                removeAttributeFilter.setInputFormat(disagreementData[i]);
                disagreementData[i] = Filter.useFilter(disagreementData[i], removeAttributeFilter);
                disagreementData[i].ClassIndex = disagreementData[i].numAttributes() - 1;
                tw3 = new StreamWriter(@"C:\MITesData\SamplePLFormat\disagreement" + i + ".arff");
                tw3.WriteLine(disagreementData[i].ToString());
                tw3.Close();
            }

            //Here I have agreement and disagreement data


            //Testing the disagreement portion
            J48 tree = new J48();         // new instance of tree
            tree.set_MinNumObj(10);
            tree.set_ConfidenceFactor((float)0.25);
            tree.buildClassifier(agreementData);   // build classifier



            //Generates the confusion matrix for the disagreement portion of the data
            int numClasses = disagreementData[0].attribute(disagreementData[0].numAttributes() - 1).numValues();
            //int[,] confusionMatrix=new int[numClasses,numClasses];
            string[] className = new string[numClasses];
            for (int j = 0; (j < numClasses); j++)
                className[j] = disagreementData[0].attribute(disagreementData[0].numAttributes() - 1).value_Renamed(j);

            ConfusionMatrix[] cMatricies = new ConfusionMatrix[annotaters];
            for (int i = 0; (i < annotaters); i++)
                cMatricies[i] = new ConfusionMatrix(className);


            for (int j = 0; (j < disagreementData[0].numInstances()); j++)
            {

                Instance[] newinstance = new Instance[annotaters];
                for (int i = 0; (i < annotaters); i++)
                    newinstance[i] = disagreementData[i].instance(j);

                //ID in all anotaters is the same and prediction is the same
                int instanceID = (int)newinstance[0].value_Renamed(disagreementData[0].numAttributes() - 2);
                newinstance[0].Dataset = disagreementData[0];
                double predicted = tree.classifyInstance(newinstance[0]);
                string predicted_activity = newinstance[0].dataset().classAttribute().value_Renamed((int)predicted);


                int k = 0;
                for (k = 0; (k < annotaters); k++)
                {
                    int annotationID = (int)originalData.instance(instanceID).value_Renamed(data.numAttributes() - 1 - k);
                    labeledData[instanceID, k] = (int)annotationID;

                    if (predicted == newinstance[k].classValue())// when filling CF make sure you compare to the right label
                        cMatricies[k].addElement((int)predicted, (int)predicted, 1.0);
                    else
                        cMatricies[k].addElement((int)newinstance[k].classValue(), (int)predicted, 1.0);
                    //annotator_activities += "," + newinstance.dataset().classAttribute().value_Renamed((int)annotationID);                      
                }

                labeledData[instanceID, k] = (int)predicted;
                //string outputData = instanceID + annotator_activities + "," + predicted_activity;
            }


            //Evaluation eval = new Evaluation(agreementData);
            //eval.evaluateModel(tree, disagreementData);//.crossValidateModel(tree, data, 10, new Random(1));





            //cross validate the intersection data

            //Randomize the data
            Randomize randomizeFilter = new Randomize();
            randomizeFilter.setInputFormat(agreementData);
            Instances randomData = Filter.useFilter(agreementData, randomizeFilter);

            ConfusionMatrix cMatrix2 = new ConfusionMatrix(className);
            //int[,] confusionMatrix2 = new int[numClasses, numClasses];
            int numFolds = 10;



            for (int i = 1; (i <= numFolds); i++)
            {
                //Training folds filter


                RemoveFolds trainingFoldsFilter = new RemoveFolds();
                trainingFoldsFilter.set_NumFolds(numFolds);
                trainingFoldsFilter.set_InvertSelection(true);
                trainingFoldsFilter.set_Fold(i);
                trainingFoldsFilter.inputFormat(randomData);
                Instances training = Filter.useFilter(randomData, trainingFoldsFilter);

                RemoveFolds testFoldsFilter = new RemoveFolds();
                testFoldsFilter.set_NumFolds(numFolds);
                testFoldsFilter.set_InvertSelection(false);
                testFoldsFilter.set_Fold(i);
                testFoldsFilter.inputFormat(randomData);
                Instances test = Filter.useFilter(randomData, testFoldsFilter);

                //ready for training and testing
                tree = new J48(); // new instance of tree
                tree.set_MinNumObj(10);
                tree.set_ConfidenceFactor((float)0.25);
                tree.buildClassifier(training); // build classifier


                for (int j = 0; (j < test.numInstances()); j++)
                {

                    Instance newinstance = test.instance(j);
                    newinstance.Dataset = test;
                    int instanceID = (int)newinstance.value_Renamed(test.numAttributes() - 2);


                    double predicted = tree.classifyInstance(newinstance);
                    string predicted_activity = newinstance.dataset().classAttribute().value_Renamed((int)predicted);
                    if (predicted == newinstance.classValue())
                        cMatrix2.addElement((int)predicted, (int)predicted, 1.0);
                    //confusionMatrix[(int)predicted, (int)predicted] = confusionMatrix[(int)predicted, (int)predicted] + 1;
                    else
                        cMatrix2.addElement((int)newinstance.classValue(), (int)predicted, 1.0);


                    int k = 0;
                    for (k = 0; (k < 2); k++)
                    {
                        int annotationID = (int)originalData.instance(instanceID).value_Renamed(data.numAttributes() - 1 - k);
                        labeledData[instanceID, k] = (int)annotationID;
                        //annotator_activities += "," + newinstance.dataset().classAttribute().value_Renamed((int)annotationID);
                    }
                    labeledData[instanceID, k] = (int)predicted;

                }

            }



            for (int i = 0; (i < className.Length); i++)
            {
                TextWriter tw = new StreamWriter(@"C:\MITesData\SamplePLFormat\" + className[i] + ".csv");
                TextWriter twavg = new StreamWriter(@"C:\MITesData\SamplePLFormat\average-" + className[i] + ".csv");
                for (int j = 0; (j < data.numInstances()); j++)
                {
                    tw.Write(j + ",");
                    twavg.Write(j + ",");
                    double sum = 0;
                    for (int k = 0; (k < annotaters + 1); k++)
                    {
                        if (labeledData[j, k] == i){ //annotator matches the class we are outputing
                            tw.Write(1 + ",");

                            if (k<annotaters)
                                sum+=1;
                        }
                        else
                            tw.Write(0 + ",");
                        
                    }
                    
                    twavg.Write(((double)(sum/annotaters)).ToString("0.00") + ","); //average annotaters
                    if (labeledData[j, annotaters] == i)
                        twavg.Write("1"); //classifeir
                    else
                        twavg.Write("0");

                    tw.WriteLine();
                    twavg.WriteLine();
                }
                tw.Close();
                twavg.Close();
            }


            //compute inter-rater reliability
            int[,] reliabilityMatrix = new int[className.Length, className.Length];
            int[] sumRows = new int[className.Length];
            int[] sumColumns = new int[className.Length];
            int totalSum = 0;
            int agreements = 0;
            double[] expectedFrequency = new double[className.Length];

            for (int i = 0; (i < data.numInstances()); i++)
            {
                Instance newinstance = data.instance(i);
                int instanceID = (int)newinstance.value_Renamed(data.numAttributes() - 4);
                reliabilityMatrix[labeledData[instanceID, 0], labeledData[instanceID, 1]] += 1;
            }

            for (int i = 0; (i < className.Length); i++)
                for (int j = 0; (j < className.Length); j++)
                {
                    sumRows[i] += reliabilityMatrix[i, j];
                    sumColumns[j] += reliabilityMatrix[i, j];
                    totalSum += reliabilityMatrix[i, j];
                    if (i == j)
                        agreements += reliabilityMatrix[i, j];
                }

            double totalExpectedFrequencies = 0.0;
            for (int i = 0; (i < className.Length); i++)
            {
                expectedFrequency[i] = (double)sumRows[i] * (double)sumColumns[i] / (double)totalSum;
                totalExpectedFrequencies += expectedFrequency[i];
            }

            double kappa = ((double)agreements - totalExpectedFrequencies) / ((double)totalSum - totalExpectedFrequencies);


            TextWriter tw2 = new StreamWriter(@"C:\MITesData\SamplePLFormat\accuracy.txt");
            //ConfusionMatrix totalMatrix = new ConfusionMatrix(className);
            //double totalMatrix1=0, correctMatrix1=0;
            //double totalMatrix2=0 ,correctMatrix2=0;
            //for (int i = 0; (i < className.Length); i++)
            //{
            // for (int j = 0; (j < className.Length); j++)
            // {
            // if (i == j)
            // {
            // correctMatrix1 += cMatrix1.getRow(i)[j];
            // correctMatrix2 += cMatrix2.getRow(i)[j];
            // }
            // totalMatrix1 += cMatrix1.getRow(i)[j];
            // totalMatrix2 += cMatrix2.getRow(i)[j];
            // totalMatrix.addElement(i, j, cMatrix1.getRow(i)[j] + cMatrix2.getRow(i)[j]);
            // }
            //}



            ConfusionMatrix[] totalMatrix = new ConfusionMatrix[annotaters];
            double[] totalMatricies = new double[annotaters];
            double[] correctMatricies = new double[annotaters];
            double agreementCorrectMatrix = 0;
            double agreementTotalMatrix = 0;
            //double[] totalMatricies2 = new double[annotaters];
            //double[] correctMatricies2 = new double[annotaters];
            for (int k = 0; (k < annotaters); k++)
                totalMatrix[k] = new ConfusionMatrix(className);

            for (int i = 0; (i < className.Length); i++)
            {
                for (int j = 0; (j < className.Length); j++)
                {
                    if (i == j)
                    {
                        for (int k = 0; (k < annotaters); k++)
                            correctMatricies[k] += cMatricies[k].getRow(i)[j]; //disagreement diagonal elements
                        agreementCorrectMatrix += cMatrix2.getRow(i)[j]; //agreement diagonal elements
                    }


                    agreementTotalMatrix += cMatrix2.getRow(i)[j]; //agrement elements
                    for (int k = 0; (k < annotaters); k++)
                    {
                        totalMatricies[k] += cMatricies[k].getRow(i)[j]; //disagreement elements
                        totalMatrix[k].addElement(i, j, cMatricies[k].getRow(i)[j] + cMatrix2.getRow(i)[j]);
                    }
                }
            }






            tw2.WriteLine("General Statistics\n------------------\n");
            tw2.WriteLine("Kappa: " + kappa.ToString("0.00") + "\n");
            tw2.WriteLine("No. Examples in Agreement: " + agreements.ToString() + "\n");
            tw2.WriteLine("No. Examples in Disagreement: " + (totalSum - agreements).ToString() + "\n");
            tw2.WriteLine("Total Examples: " + totalSum.ToString() + "\n");
            tw2.WriteLine("Disagreement\n------------------\n");
            for (int i = 0; (i < annotaters); i++)
            {
                tw2.WriteLine("RecognitionRate annotater " + i + " :" + ((double)(correctMatricies[i] / totalMatricies[i]) * 100).ToString("0.00") + "%\n");
                tw2.WriteLine(cMatricies[i].toString());
            }
            tw2.WriteLine("Agreement\n------------------\n");
            tw2.WriteLine("RecognitionRate:" + ((double)(agreementCorrectMatrix / agreementTotalMatrix) * 100).ToString("0.00") + "%\n");
            tw2.WriteLine(cMatrix2.toString());


            tw2.WriteLine("Combined\n------------------\n");
            for (int i = 0; (i < annotaters); i++)
            {
                tw2.WriteLine("RecognitionRate " + i + " :" + ((double)((correctMatricies[i] + agreementCorrectMatrix) / (totalMatricies[i] + agreementTotalMatrix)) * 100).ToString("0.00") + "%\n");
                tw2.WriteLine(totalMatrix[i].toString());
            }
            tw2.Close();


            //Generate the folds
            //Evaluation eval2 = new Evaluation(agreementData);
            //eval2.crossValidateModel(tree, agreementData, 10, new Random(1));



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

