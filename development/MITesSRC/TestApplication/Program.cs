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
using ActivitySummary;
using System.Collections;

namespace TestApplication
{
    class Program
    {
        static void Main(string[] args)
        {

            //Extractor.toARFF(@"C:\SamplePLFormat",
            //"..\\NeededFiles\\Master\\", 3);

            //Instances training = new Instances(new StreamReader(@"C:\SamplePLFormat\training.arff"));
            //J48 tree = new J48();         // new instance of tree
            //tree.set_MinNumObj(10);
            //tree.set_ConfidenceFactor((float)0.25);
            //training.ClassIndex = training.numAttributes() - 1;
            //tree.buildClassifier(training);   // build classifier

            //Instances testing = new Instances(new StreamReader(@"C:\SamplePLFormat\testing.arff"));
            //testing.ClassIndex = testing.numAttributes() - 1;
            //Hashtable h = new Hashtable();
            //h.Add("standing", 0);
            //h.Add("sitting", 0);
            //h.Add("sleeping", 0);
            //h.Add("brisk_walking", 0);
            //h.Add("cycling", 0);




            //string last_activity = "";
            //int timer = 0;
            //double calories = 0;
            //double totalCalories = 0;
            //TextWriter tw = new StreamWriter("results.txt");
            //tw.WriteLine("Total Examples " + testing.numInstances() + "\n");
            //for (int j = 0; (j < testing.numInstances()); j++)
            //{

            //    Instance newinstance = testing.instance(j);
            //    newinstance.Dataset = testing;                
            //    double predicted = tree.classifyInstance(newinstance);
            //    string predicted_activity = newinstance.dataset().classAttribute().value_Renamed((int)predicted);
            //    if (last_activity != predicted_activity)
            //    {
            //        last_activity = predicted_activity;
            //        timer = 0;
            //    }
            //    else
            //        timer += 200;

            //    h[predicted_activity] = (int) h[predicted_activity] + 1;

            //    calories = 0;
            //    if (j == 30000)
            //    {
            //        totalCalories = 0;
            //        tw.WriteLine("1/4:");
            //        foreach (string key in h.Keys)
            //        {
            //            tw.WriteLine(key + ": " + h[key]);
            //            double mets = 8.0;
            //            if (key=="walking")
            //                mets=4.0;
            //            else if (key=="standing")
            //                mets=2.0;
            //            else if (key=="sitting")
            //                mets=2.0;
            //            else
            //                mets=0.9;
            //            calories=(83 * mets * (int)h[key] * 200.0) / (1000 * 60 * 60.0);
            //            totalCalories += calories;
            //            tw.WriteLine("Calories:" + calories.ToString("0.00"));
            //            tw.WriteLine("Last Activity:" + last_activity + " Time:" + timer / 1000.0);
            //        }

            //        tw.WriteLine("Total cals:" + totalCalories + "\n");
            //    }
            //    else if (j == 60000)
            //    {
            //        tw.WriteLine("1/2:");
            //        totalCalories = 0;
            //        foreach (string key in h.Keys)
            //        {
            //            Console.WriteLine(key + ": " + h[key]);
            //            double mets = 8.0;
            //            if (key == "walking")
            //                mets = 4.0;
            //            else if (key == "standing")
            //                mets = 2.0;
            //            else if (key == "sitting")
            //                mets = 2.0;
            //            else
            //                mets = 0.9;
            //            calories = (83 * mets * (int)h[key] * 200.0) / (1000 * 60 * 60.0);
            //            totalCalories += calories;
            //            tw.WriteLine("Calories:" + calories.ToString("0.00"));
            //            tw.WriteLine("Last Activity:" + last_activity + " Time:" + timer / 1000.0);
            //        }
            //        tw.WriteLine("Total cals:" + totalCalories + "\n");
            //    }
            //    else if (j == 90000)
            //    {
            //        tw.WriteLine("3/4:");
            //        totalCalories = 0;
            //        foreach (string key in h.Keys)
            //        {
            //            tw.WriteLine(key + ": " + h[key]);
            //            double mets = 8.0;
            //            if (key == "walking")
            //                mets = 4.0;
            //            else if (key == "standing")
            //                mets = 2.0;
            //            else if (key == "sitting")
            //                mets = 2.0;
            //            else
            //                mets = 0.9;
            //            calories = (83 * mets * (int)h[key] * 200.0) / (1000 * 60 * 60.0);
            //            totalCalories += calories;
            //            tw.WriteLine("Calories:" + calories.ToString("0.00"));
            //            tw.WriteLine("Last Activity:" + last_activity + " Time:" + timer / 1000.0);
            //        }
            //        tw.WriteLine("Total cals:" + totalCalories + "\n");
            //    }
            //}


            //calories = 0;
            //totalCalories = 0;
            //tw.WriteLine("Total:");
            //foreach (string key in h.Keys)
            //{
            //    tw.WriteLine(key + ": " + h[key]);
            //    double mets = 8.0;
            //    if (key == "walking")
            //        mets = 4.0;
            //    else if (key == "standing")
            //        mets = 2.0;
            //    else if (key == "sitting")
            //        mets = 2.0;
            //    else
            //        mets = 0.9;
            //    calories = (83 * mets * (int)h[key] * 200.0) / (1000 * 60 * 60.0);
            //    totalCalories += calories;
            //    tw.WriteLine("Calories:" + calories.ToString("0.00"));
            //    tw.WriteLine("Last Activity:" + last_activity + " Time:" + timer / 1000.0);
            //}


            //tw.WriteLine("Total cals:" + totalCalories);

            //tw.WriteLine();
            //tw.Close();


            string[] filter = new string[2];           
            filter[0] = "annotation";
            filter[1] = "setup";
   
            Extractor.toARFF(@"C:\SamplePLFormat","..\\NeededFiles\\Master\\", 3,filter);

            /* Autism Analysis Start


            string[] filter = new string[5];
            filter[0] = "maybe";
            filter[1] = "good";
            filter[2] = "annotation";
            filter[3] = "setup";
            filter[4] = "not";
            bool realtime = true;

            //Extractor.toARFF(@"C:\SamplePLFormat",
            //"..\\NeededFiles\\Master\\", 3);
            //Generates an Arff file from a PlaceLab format file
            Extractor.toARFF(@"C:\SamplePLFormat",
             "..\\NeededFiles\\Master\\", 3, "ActivityLabels", 2, filter);



            Instances data = new Instances(new StreamReader(@"C:\SamplePLFormat\output-ActivityLabels.arff"));
            Instances originalData = new Instances(new StreamReader(@"C:\SamplePLFormat\output-ActivityLabels.arff"));
       

            int annotaters = 2;
            int[,] labeledData = new int[data.numInstances(), 5]; //each instance has 2 annotators and a classifier, realtime,realtimeclassifier


       

            //Filter intersection instances
            int intersectionAttribute = data.numAttributes() - 3;
            RemoveWithValues intersectionFilter = new RemoveWithValues();
            intersectionFilter.set_AttributeIndex(intersectionAttribute.ToString());
            intersectionFilter.set_InvertSelection(false);
            intersectionFilter.set_SplitPoint(1.0);
            intersectionFilter.setInputFormat(data);
            Instances agreementData = Filter.useFilter(data, intersectionFilter);
            Instances originalAgreementData = Filter.useFilter(data, intersectionFilter);
            Instances agreementDatarealtime = Filter.useFilter(data, intersectionFilter);


            TextWriter tw3 = new StreamWriter(@"C:\SamplePLFormat\agreement.arff");
            tw3.WriteLine(agreementData.ToString());
            tw3.Close();


            //remove redundant annotator,realtime and annotator agreement flag
            Remove removeAttributeFilter = new Remove();
            int[] removeAttribute = new int[3];
            removeAttribute[0] = data.numAttributes() - 1;
            removeAttribute[1] = data.numAttributes() - 2;
            removeAttribute[2] = data.numAttributes() - 4;
            removeAttributeFilter.set_InvertSelection(false);
            removeAttributeFilter.SetAttributeIndicesArray(removeAttribute);

            //NOTE, you have to set the attribute indicies before the inputFormat to do
            //the filtering
            removeAttributeFilter.setInputFormat(agreementData);
            agreementData = Filter.useFilter(agreementData, removeAttributeFilter);
            agreementData.ClassIndex = agreementData.numAttributes() - 1;

            

            removeAttributeFilter = new Remove();
            removeAttribute = new int[3];
            removeAttribute[0] = data.numAttributes() - 3;
            removeAttribute[1] = data.numAttributes() - 2;
            removeAttribute[2] = data.numAttributes() - 4;
            removeAttributeFilter.set_InvertSelection(false);
            removeAttributeFilter.SetAttributeIndicesArray(removeAttribute);

            //NOTE, you have to set the attribute indicies before the inputFormat to do
            //the filtering
            removeAttributeFilter.setInputFormat(agreementDatarealtime);
            agreementDatarealtime = Filter.useFilter(agreementDatarealtime, removeAttributeFilter);
            agreementDatarealtime.ClassIndex = agreementDatarealtime.numAttributes() - 1;





            //filter instances in disagreement
            intersectionFilter = new RemoveWithValues();
            intersectionFilter.set_AttributeIndex(intersectionAttribute.ToString());
            intersectionFilter.set_InvertSelection(true);
            intersectionFilter.set_SplitPoint(1.0);
            intersectionFilter.setInputFormat(data);



            //remove redundant annotator and annotator agreement
            Instances[] disagreementData = new Instances[annotaters];
            Instances disagreementRealtime = null;
            for (int i = 0; (i < annotaters); i++)
            {
                disagreementData[i] = Filter.useFilter(data, intersectionFilter);
                removeAttributeFilter = new Remove();
                removeAttribute = new int[3];
                removeAttribute[0] = data.numAttributes() - 2 - i; //remove labels
                removeAttribute[1] = data.numAttributes() - 4;
                removeAttribute[2] = data.numAttributes() - 1;
                removeAttributeFilter.set_InvertSelection(false);
                removeAttributeFilter.SetAttributeIndicesArray(removeAttribute);
                removeAttributeFilter.setInputFormat(disagreementData[i]);
                disagreementData[i] = Filter.useFilter(disagreementData[i], removeAttributeFilter);
                disagreementData[i].ClassIndex = disagreementData[i].numAttributes() - 1;
                tw3 = new StreamWriter(@"C:\SamplePLFormat\disagreement" + i + ".arff");
                tw3.WriteLine(disagreementData[i].ToString());
                tw3.Close();
            }


            disagreementRealtime = Filter.useFilter(data, intersectionFilter);
            removeAttributeFilter = new Remove();
            removeAttribute = new int[3];
            removeAttribute[0] = data.numAttributes() - 2; //remove labels
            removeAttribute[1] = data.numAttributes() - 4;
            removeAttribute[2] = data.numAttributes() - 3;
            removeAttributeFilter.set_InvertSelection(false);
            removeAttributeFilter.SetAttributeIndicesArray(removeAttribute);
            removeAttributeFilter.setInputFormat(disagreementRealtime);
            disagreementRealtime = Filter.useFilter(disagreementRealtime, removeAttributeFilter);
            disagreementRealtime.ClassIndex = disagreementRealtime.numAttributes() - 1;


            //Here I have agreement and disagreement data
            weka.filters.supervised.instance.Resample resampler = new weka.filters.supervised.instance.Resample();
            resampler.set_BiasToUniformClass(1.0); //balance the samples
            resampler.set_RandomSeed(1);
            resampler.set_SampleSizePercent(100.0);
            Instances trainingData = null;
            // if (realtime == true)
            //{
            //  Extractor.toARFF(@"C:\SamplePLFormat", "..\\NeededFiles\\Master\\", 3,filter);
            // trainingData = new Instances(new StreamReader(@"C:\SamplePLFormat\realtime-output.arff"));
            // trainingData.ClassIndex = trainingData.numAttributes() - 1;
            // resampler.setInputFormat(trainingData);
            // trainingData = Filter.useFilter(trainingData, resampler);
            // }
            // else
            // {

            resampler.setInputFormat(agreementData);
            trainingData = Filter.useFilter(agreementData, resampler);
            //}

            Instances trainingData2 = Filter.useFilter(agreementDatarealtime, resampler);

            //Testing the disagreement portion
            J48 tree = new J48();         // new instance of tree
            tree.set_MinNumObj(5);
            tree.set_ConfidenceFactor((float)0.25);
            tree.buildClassifier(trainingData);   // build classifier

            J48 realtimetree = new J48();         // new instance of tree
            realtimetree.set_MinNumObj(5);
            realtimetree.set_ConfidenceFactor((float)0.25);
            realtimetree.buildClassifier(trainingData2);   // build classifier
            //tree.buildClassifier(trainingData);

        


            //Generates the confusion matrix for the disagreement portion of the data
            int numClasses = disagreementData[0].attribute(disagreementData[0].numAttributes() - 1).numValues();
            //int[,] confusionMatrix=new int[numClasses,numClasses];
            string[] className = new string[numClasses];
            for (int j = 0; (j < numClasses); j++)
                className[j] = disagreementData[0].attribute(disagreementData[0].numAttributes() - 1).value_Renamed(j);

            ConfusionMatrix[] cMatricies = new ConfusionMatrix[annotaters];
            ConfusionMatrix realtimeMatrix = new ConfusionMatrix(className);
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
                for (k = annotaters - 1; (k >= 0); k--)
                {
                    int annotationID = (int)originalData.instance(instanceID).value_Renamed(data.numAttributes() - 2 - k);
                    labeledData[instanceID, k] = (int)annotationID;

                    if (predicted == newinstance[k].classValue())// when filling CF make sure you compare to the right label
                        cMatricies[k].addElement((int)predicted, (int)predicted, 1.0);
                    else
                        cMatricies[k].addElement((int)newinstance[k].classValue(), (int)predicted, 1.0);
                    //annotator_activities += "," + newinstance.dataset().classAttribute().value_Renamed((int)annotationID);                      
                }

                labeledData[instanceID, annotaters] = (int)predicted;


                predicted = realtimetree.classifyInstance(disagreementRealtime.instance(j));
                predicted_activity = disagreementRealtime.instance(j).dataset().classAttribute().value_Renamed((int)predicted);


                int annotationID2 = (int)originalData.instance(instanceID).value_Renamed(data.numAttributes() - 1);
                labeledData[instanceID, annotaters + 2] = (int)annotationID2;//realtime annotation
                if (predicted == disagreementRealtime.instance(j).classValue())// when filling CF make sure you compare to the right label
                    realtimeMatrix.addElement((int)predicted, (int)predicted, 1.0);
                else
                    realtimeMatrix.addElement((int)disagreementRealtime.instance(j).classValue(), (int)predicted, 1.0);
                labeledData[instanceID, annotaters + 1] = (int)predicted; //realtime classifier
                //string outputData = instanceID + annotator_activities + "," + predicted_activity;
            }




            //cross validate the intersection data

            //Randomize the data
            Randomize randomizeFilter = new Randomize();
            randomizeFilter.setInputFormat(originalAgreementData);
            Instances randomData = Filter.useFilter(originalAgreementData, randomizeFilter);

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


                removeAttributeFilter = new Remove();
                removeAttribute = new int[3];
                removeAttribute[0] = data.numAttributes() - 1;
                removeAttribute[1] = data.numAttributes() - 2;
                removeAttribute[2] = data.numAttributes() - 4;
                removeAttributeFilter.set_InvertSelection(false);
                removeAttributeFilter.SetAttributeIndicesArray(removeAttribute);
                //NOTE, you have to set the attribute indicies before the inputFormat to do
                //the filtering
                removeAttributeFilter.setInputFormat(training);
                Instances trainingOffline = Filter.useFilter(training, removeAttributeFilter);
                trainingOffline.ClassIndex = trainingOffline.numAttributes() - 1;


                resampler = new weka.filters.supervised.instance.Resample();
                resampler.set_BiasToUniformClass(1.0); //balance the samples
                resampler.set_RandomSeed(1);
                resampler.set_SampleSizePercent(100.0);
                resampler.setInputFormat(trainingOffline);
                trainingOffline = Filter.useFilter(trainingOffline, resampler);

                RemoveFolds testFoldsFilter = new RemoveFolds();
                testFoldsFilter.set_NumFolds(numFolds);
                testFoldsFilter.set_InvertSelection(false);
                testFoldsFilter.set_Fold(i);
                testFoldsFilter.inputFormat(randomData);
                Instances test = Filter.useFilter(randomData, testFoldsFilter);

                Instances testOffline = Filter.useFilter(test, removeAttributeFilter);
                testOffline.ClassIndex = testOffline.numAttributes() - 1;

                //ready for training and testing
                tree = new J48(); // new instance of tree
                tree.set_MinNumObj(5);
                tree.set_ConfidenceFactor((float)0.25);
                tree.buildClassifier(trainingOffline); // build classifier




                //realtime classifier
                removeAttributeFilter = new Remove();
                removeAttribute = new int[3];
                removeAttribute[0] = data.numAttributes() - 3;
                removeAttribute[1] = data.numAttributes() - 2;
                removeAttribute[2] = data.numAttributes() - 4;
                removeAttributeFilter.set_InvertSelection(false);
                removeAttributeFilter.SetAttributeIndicesArray(removeAttribute);
                //NOTE, you have to set the attribute indicies before the inputFormat to do
                //the filtering
                removeAttributeFilter.setInputFormat(training);
                Instances trainingRealtime = Filter.useFilter(training, removeAttributeFilter);
                trainingRealtime.ClassIndex = trainingRealtime.numAttributes() - 1;

                resampler = new weka.filters.supervised.instance.Resample();
                resampler.set_BiasToUniformClass(1.0); //balance the samples
                resampler.set_RandomSeed(1);
                resampler.set_SampleSizePercent(100.0);
                resampler.setInputFormat(trainingOffline);
                trainingRealtime = Filter.useFilter(trainingRealtime, resampler);

                Instances testRealtime = Filter.useFilter(test, removeAttributeFilter);
                testRealtime.ClassIndex = testRealtime.numAttributes() - 1;


                //ready for training and testing
                realtimetree = new J48(); // new instance of tree
                realtimetree.set_MinNumObj(5);
                realtimetree.set_ConfidenceFactor((float)0.25);
                realtimetree.buildClassifier(trainingRealtime); // build classifier


                for (int j = 0; (j < testOffline.numInstances()); j++)
                {

                    Instance newinstance = testOffline.instance(j);
                    newinstance.Dataset = testOffline;



                    int instanceID = (int)newinstance.value_Renamed(testOffline.numAttributes() - 2);


                    double predicted = tree.classifyInstance(newinstance);
                    string predicted_activity = newinstance.dataset().classAttribute().value_Renamed((int)predicted);
                    if (predicted == newinstance.classValue())
                        cMatrix2.addElement((int)predicted, (int)predicted, 1.0);
                    //confusionMatrix[(int)predicted, (int)predicted] = confusionMatrix[(int)predicted, (int)predicted] + 1;
                    else
                        cMatrix2.addElement((int)newinstance.classValue(), (int)predicted, 1.0);


                    int k = 0;
                    for (k = 1; (k >= 0); k--)
                    {
                        int annotationID = (int)originalData.instance(instanceID).value_Renamed(data.numAttributes() - 2 - k);
                        labeledData[instanceID, k] = (int)annotationID;
                        //annotator_activities += "," + newinstance.dataset().classAttribute().value_Renamed((int)annotationID);
                    }
                    labeledData[instanceID, annotaters] = (int)predicted;




                    predicted = realtimetree.classifyInstance(testRealtime.instance(j));
                    predicted_activity = testRealtime.instance(j).dataset().classAttribute().value_Renamed((int)predicted);

                    int annotationID2 = (int)originalData.instance(instanceID).value_Renamed(data.numAttributes() - 1);
                    labeledData[instanceID, annotaters + 2] = (int)annotationID2;//realtime annotation
                    if (predicted == testRealtime.instance(j).classValue())// when filling CF make sure you compare to the right label
                        realtimeMatrix.addElement((int)predicted, (int)predicted, 1.0);
                    else
                        realtimeMatrix.addElement((int)testRealtime.instance(j).classValue(), (int)predicted, 1.0);
                    labeledData[instanceID, annotaters + 1] = (int)predicted; //realtime classifier


                }

            }



            //for (int i = 0; (i < className.Length); i++)
            //{
            //    TextWriter tw = new StreamWriter(@"C:\SamplePLFormat\" + className[i] + ".csv");
            //    TextWriter twavg = new StreamWriter(@"C:\SamplePLFormat\average-" + className[i] + ".csv");
            //    for (int j = 0; (j < data.numInstances()); j++)
            //    {
            //        tw.Write(j + ",");
            //        twavg.Write(j + ",");
            //        double sum = 0;
            //        for (int k = 0; (k < annotaters + 1); k++)
            //        {
            //            if (labeledData[j, k] == i){ //annotator matches the class we are outputing
            //                tw.Write(1 + ",");

            //                if (k<annotaters)
            //                    sum+=1;
            //            }
            //            else
            //                tw.Write(0 + ",");

            //        }

            //        twavg.Write(((double)(sum/annotaters)).ToString("0.00") + ","); //average annotaters
            //        if (labeledData[j, annotaters] == i)
            //            twavg.Write("1"); //classifeir
            //        else
            //            twavg.Write("0");

            //        tw.WriteLine();
            //        twavg.WriteLine();
            //    }
            //    tw.Close();
            //    twavg.Close();
            //}


            //compute inter-rater reliability
            int[,] reliabilityMatrix = new int[className.Length, className.Length];
            int[] sumRows = new int[className.Length];
            int[] sumColumns = new int[className.Length];
            int totalSum = 0;
            int agreements = 0;
            double[] expectedFrequency = new double[className.Length];


            int[,] reliabilityMatrix2 = new int[className.Length, className.Length];
            int[] sumRows2 = new int[className.Length];
            int[] sumColumns2 = new int[className.Length];
            int totalSum2 = 0;
            int agreements2 = 0;
            double[] expectedFrequency2 = new double[className.Length];

            for (int i = 0; (i < data.numInstances()); i++)
            {
                Instance newinstance = data.instance(i);
                int instanceID = (int)newinstance.value_Renamed(data.numAttributes() - 5);
                reliabilityMatrix[labeledData[instanceID, 0], labeledData[instanceID, 1]] += 1;
                reliabilityMatrix2[labeledData[instanceID, 0], labeledData[instanceID, 3]] += 1;
            }

            for (int i = 0; (i < className.Length); i++)
                for (int j = 0; (j < className.Length); j++)
                {
                    sumRows[i] += reliabilityMatrix[i, j];
                    sumColumns[j] += reliabilityMatrix[i, j];
                    totalSum += reliabilityMatrix[i, j];
                    if (i == j)
                        agreements += reliabilityMatrix[i, j];


                    sumRows2[i] += reliabilityMatrix2[i, j];
                    sumColumns2[j] += reliabilityMatrix2[i, j];
                    totalSum2 += reliabilityMatrix2[i, j];
                    if (i == j)
                        agreements2 += reliabilityMatrix2[i, j];
                }

            double totalExpectedFrequencies = 0.0;
            double totalExpectedFrequencies2 = 0.0;
            for (int i = 0; (i < className.Length); i++)
            {
                expectedFrequency[i] = (double)sumRows[i] * (double)sumColumns[i] / (double)totalSum;
                totalExpectedFrequencies += expectedFrequency[i];

                expectedFrequency2[i] = (double)sumRows2[i] * (double)sumColumns2[i] / (double)totalSum2;
                totalExpectedFrequencies2 += expectedFrequency2[i];
            }

            double kappa = ((double)agreements - totalExpectedFrequencies) / ((double)totalSum - totalExpectedFrequencies);
            double kappa2 = ((double)agreements2 - totalExpectedFrequencies2) / ((double)totalSum2 - totalExpectedFrequencies2);


            TextWriter tw2 = new StreamWriter(@"C:\SamplePLFormat\accuracy.csv");
            TextWriter tw4 = new StreamWriter(@"C:\SamplePLFormat\matricies.txt");

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


            //full data test


        
            Instances fullData = new Instances(new StreamReader(@"C:\SamplePLFormat\output-ActivityLabels.arff"));
            //removeAttributeFilter = new Remove();
            //removeAttribute = new int[3];
            //removeAttribute[0] = data.numAttributes() - 1;
            //removeAttribute[1] = data.numAttributes() - 2;
            //removeAttribute[2] = data.numAttributes() - 4;
            //removeAttributeFilter.set_InvertSelection(false);
            //removeAttributeFilter.SetAttributeIndicesArray(removeAttribute);
            //removeAttributeFilter.setInputFormat(fullData);
            //fullData = Filter.useFilter(fullData, removeAttributeFilter);
            //fullData.ClassIndex = fullData.numAttributes() - 1;

            //fulldata testing
            //J48 fulldatatree = new J48();         // new instance of tree
            //fulldatatree.set_MinNumObj(5);
            //fulldatatree.set_ConfidenceFactor((float)0.25);
            //fulldatatree.buildClassifier(fullData);   // build classifier



            ConfusionMatrix cMatrix3 = new ConfusionMatrix(className);
            ConfusionMatrix cMatrix4 = new ConfusionMatrix(className);
            ConfusionMatrix cMatrix5 = new ConfusionMatrix(className);
            int[,] labeledData2 = new int[data.numInstances(), 3]; //each instance has 2 annotators and a classifier, realtime,realtimeclassifier
            for (int i = 1; (i <= numFolds); i++)
            {
                //Training folds filter


                RemoveFolds trainingFoldsFilter = new RemoveFolds();
                trainingFoldsFilter.set_NumFolds(numFolds);
                trainingFoldsFilter.set_InvertSelection(true);
                trainingFoldsFilter.set_Fold(i);
                trainingFoldsFilter.inputFormat(fullData);
                Instances training = Filter.useFilter(fullData, trainingFoldsFilter);


                removeAttributeFilter = new Remove();
                removeAttribute = new int[3];
                removeAttribute[0] = data.numAttributes() - 1;
                removeAttribute[1] = data.numAttributes() - 2;
                removeAttribute[2] = data.numAttributes() - 4;
                removeAttributeFilter.set_InvertSelection(false);
                removeAttributeFilter.SetAttributeIndicesArray(removeAttribute);
                //NOTE, you have to set the attribute indicies before the inputFormat to do
                //the filtering
                removeAttributeFilter.setInputFormat(training);
                Instances trainingOffline = Filter.useFilter(training, removeAttributeFilter);
                trainingOffline.ClassIndex = trainingOffline.numAttributes() - 1;


                //resampler = new weka.filters.supervised.instance.Resample();
                //resampler.set_BiasToUniformClass(1.0); //balance the samples
                //resampler.set_RandomSeed(1);
                //resampler.set_SampleSizePercent(100.0);
                //resampler.setInputFormat(trainingOffline);
                //trainingOffline = Filter.useFilter(trainingOffline, resampler);

                RemoveFolds testFoldsFilter = new RemoveFolds();
                testFoldsFilter.set_NumFolds(numFolds);
                testFoldsFilter.set_InvertSelection(false);
                testFoldsFilter.set_Fold(i);
                testFoldsFilter.inputFormat(fullData);
                Instances test = Filter.useFilter(fullData, testFoldsFilter);

                Instances testOffline = Filter.useFilter(test, removeAttributeFilter);
                testOffline.ClassIndex = testOffline.numAttributes() - 1;

                //ready for training and testing
                tree = new J48(); // new instance of tree
                tree.set_MinNumObj(5);
                tree.set_ConfidenceFactor((float)0.25);
                tree.buildClassifier(trainingOffline); // build classifier


                for (int j = 0; (j < testOffline.numInstances()); j++)
                {

                    Instance newinstance = testOffline.instance(j);
                    newinstance.Dataset = testOffline;
                    int instanceID = (int)newinstance.value_Renamed(testOffline.numAttributes() - 2);


                    double predicted = tree.classifyInstance(newinstance);
                    string predicted_activity = newinstance.dataset().classAttribute().value_Renamed((int)predicted);
                    if (predicted == newinstance.classValue())
                        cMatrix3.addElement((int)predicted, (int)predicted, 1.0);                    
                    else
                        cMatrix3.addElement((int)newinstance.classValue(), (int)predicted, 1.0);

                    int annotator1 = (int)originalData.instance(instanceID).value_Renamed(data.numAttributes() - 3);
                    int annotator0 = (int)originalData.instance(instanceID).value_Renamed(data.numAttributes() - 2);
                    if (annotator0 == annotator1)
                    {
                        if (predicted == newinstance.classValue())
                            cMatrix4.addElement((int)predicted, (int)predicted, 1.0);
                        else
                            cMatrix4.addElement((int)newinstance.classValue(), (int)predicted, 1.0);
                    }
                    else
                    {
                        if (predicted == newinstance.classValue())
                            cMatrix5.addElement((int)predicted, (int)predicted, 1.0);
                        else
                            cMatrix5.addElement((int)newinstance.classValue(), (int)predicted, 1.0);
                    }
                    
                    //int k = 0;
                    //for (k = 1; (k >= 0); k--)
                   // {
                     //   int annotationID = (int)originalData.instance(instanceID).value_Renamed(data.numAttributes() - 2 - k);
                      //  labeledData2[instanceID, k] = (int)annotationID;
                        //annotator_activities += "," + newinstance.dataset().classAttribute().value_Renamed((int)annotationID);
                    //}
                    //labeledData2[instanceID, annotaters] = (int)predicted;
                }

            }



 


            //Evaluation eval = new Evaluation(fullData);
            //eval.crossValidateModel(fulldatatree, fullData, 10, new Random(1));
            //string result= eval.toSummaryString()+"\n";
            //result+=eval.toClassDetailsString();


            //balance in full
            fullData = new Instances(new StreamReader(@"C:\SamplePLFormat\output-ActivityLabels.arff"));

            ConfusionMatrix cMatrix6 = new ConfusionMatrix(className);
            ConfusionMatrix cMatrix7 = new ConfusionMatrix(className);
            ConfusionMatrix cMatrix8 = new ConfusionMatrix(className);
            int[,] labeledData3 = new int[data.numInstances(), 3]; //each instance has 2 annotators and a classifier, realtime,realtimeclassifier
            for (int i = 1; (i <= numFolds); i++)
            {
                //Training folds filter


                RemoveFolds trainingFoldsFilter = new RemoveFolds();
                trainingFoldsFilter.set_NumFolds(numFolds);
                trainingFoldsFilter.set_InvertSelection(true);
                trainingFoldsFilter.set_Fold(i);
                trainingFoldsFilter.inputFormat(fullData);
                Instances training = Filter.useFilter(fullData, trainingFoldsFilter);


                removeAttributeFilter = new Remove();
                removeAttribute = new int[3];
                removeAttribute[0] = data.numAttributes() - 1;
                removeAttribute[1] = data.numAttributes() - 2;
                removeAttribute[2] = data.numAttributes() - 4;
                removeAttributeFilter.set_InvertSelection(false);
                removeAttributeFilter.SetAttributeIndicesArray(removeAttribute);
                //NOTE, you have to set the attribute indicies before the inputFormat to do
                //the filtering
                removeAttributeFilter.setInputFormat(training);
                Instances trainingOffline = Filter.useFilter(training, removeAttributeFilter);
                trainingOffline.ClassIndex = trainingOffline.numAttributes() - 1;


                resampler = new weka.filters.supervised.instance.Resample();
                resampler.set_BiasToUniformClass(1.0); //balance the samples
                resampler.set_RandomSeed(1);
                resampler.set_SampleSizePercent(100.0);
                resampler.setInputFormat(trainingOffline);
                trainingOffline = Filter.useFilter(trainingOffline, resampler);

                RemoveFolds testFoldsFilter = new RemoveFolds();
                testFoldsFilter.set_NumFolds(numFolds);
                testFoldsFilter.set_InvertSelection(false);
                testFoldsFilter.set_Fold(i);
                testFoldsFilter.inputFormat(fullData);
                Instances test = Filter.useFilter(fullData, testFoldsFilter);

                Instances testOffline = Filter.useFilter(test, removeAttributeFilter);
                testOffline.ClassIndex = testOffline.numAttributes() - 1;

                //ready for training and testing
                tree = new J48(); // new instance of tree
                tree.set_MinNumObj(5);
                tree.set_ConfidenceFactor((float)0.25);
                tree.buildClassifier(trainingOffline); // build classifier


                for (int j = 0; (j < testOffline.numInstances()); j++)
                {

                    Instance newinstance = testOffline.instance(j);
                    newinstance.Dataset = testOffline;
                    int instanceID = (int)newinstance.value_Renamed(testOffline.numAttributes() - 2);

                    double predicted = tree.classifyInstance(newinstance);
                    string predicted_activity = newinstance.dataset().classAttribute().value_Renamed((int)predicted);
                    if (predicted == newinstance.classValue())
                        cMatrix6.addElement((int)predicted, (int)predicted, 1.0);
                    //confusionMatrix[(int)predicted, (int)predicted] = confusionMatrix[(int)predicted, (int)predicted] + 1;
                    else
                        cMatrix6.addElement((int)newinstance.classValue(), (int)predicted, 1.0);


                    int annotator1 = (int)originalData.instance(instanceID).value_Renamed(data.numAttributes() - 3);
                    int annotator0 = (int)originalData.instance(instanceID).value_Renamed(data.numAttributes() - 2);
                    if (annotator0 == annotator1)
                    {
                        if (predicted == newinstance.classValue())
                            cMatrix7.addElement((int)predicted, (int)predicted, 1.0);
                        else
                            cMatrix7.addElement((int)newinstance.classValue(), (int)predicted, 1.0);
                    }
                    else
                    {
                        if (predicted == newinstance.classValue())
                            cMatrix8.addElement((int)predicted, (int)predicted, 1.0);
                        else
                            cMatrix8.addElement((int)newinstance.classValue(), (int)predicted, 1.0);
                    }
                }

            }

            //removeAttributeFilter = new Remove();
            //removeAttribute = new int[3];
            //removeAttribute[0] = data.numAttributes() - 1;
            //removeAttribute[1] = data.numAttributes() - 2;
            //removeAttribute[2] = data.numAttributes() - 4;
            //removeAttributeFilter.set_InvertSelection(false);
            //removeAttributeFilter.SetAttributeIndicesArray(removeAttribute);
            //removeAttributeFilter.setInputFormat(fullData);
            //fullData = Filter.useFilter(fullData, removeAttributeFilter);
            //fullData.ClassIndex = fullData.numAttributes() - 1;

            ////balanced
            //resampler = new weka.filters.supervised.instance.Resample();
            //resampler.set_BiasToUniformClass(1.0); //balance the samples
            //resampler.set_RandomSeed(1);
            //resampler.set_SampleSizePercent(100.0);
            //resampler.setInputFormat(fullData);
            //fullData = Filter.useFilter(fullData, resampler);

            ////fulldata testing
            //fulldatatree = new J48();         // new instance of tree
            //fulldatatree.set_MinNumObj(5);
            //fulldatatree.set_ConfidenceFactor((float)0.25);
            //fulldatatree.buildClassifier(fullData);   // build classifier

            //eval = new Evaluation(fullData);
            //eval.crossValidateModel(fulldatatree, fullData, 10, new Random(1));
            //string result2 = eval.toSummaryString() + "\n";
            //result2 += eval.toClassDetailsString();


    




            tw2.WriteLine("Subject 9,01-17-08,Classroom\n");
            tw2.WriteLine("Kappa, offline," + kappa.ToString("0.00"));
            tw2.WriteLine("Kappa, realtime," + kappa2.ToString("0.00"));
            tw2.WriteLine("Total Examples," + totalSum + ",,Agree," + agreements.ToString() + ",Disagree," + (totalSum - agreements).ToString());

            tw2.WriteLine("\n\nCase 1, Train Offline Test Offline\n");

            tw2.WriteLine("\n\nTotal-Accuracy(combined)," + ((double)(cMatrix3.correct() / (cMatrix3.correct() + cMatrix3.incorrect())) * 100).ToString("0.00") + "%");
            tw2.WriteLine("Activity,Num Examples,TP Rate,FP Rate,Precision,Recall");
            for (int j = 0; (j < cMatrix3.numColumns()); j++)
                tw2.WriteLine(cMatrix3.className(j) + "," + (cMatrix3.getTwoClassStats(j).FalseNegative + cMatrix3.getTwoClassStats(j).TruePositive) + "," + cMatrix3.getTwoClassStats(j).TruePositiveRate.ToString("0.00") + "," + cMatrix3.getTwoClassStats(j).FalsePositiveRate.ToString("0.00") + "," + cMatrix3.getTwoClassStats(j).Precision.ToString("0.00") + "," + cMatrix3.getTwoClassStats(j).Recall.ToString("0.00"));


            tw2.WriteLine("\n\nAgree-Accuracy," + ((double)(cMatrix4.correct() / (cMatrix4.correct() + cMatrix4.incorrect())) * 100).ToString("0.00") + "%");
            tw2.WriteLine("Activity,Num Examples,TP Rate,FP Rate,Precision,Recall");
            for (int j = 0; (j < cMatrix4.numColumns()); j++)
                tw2.WriteLine(cMatrix4.className(j) + "," + (cMatrix4.getTwoClassStats(j).FalseNegative + cMatrix4.getTwoClassStats(j).TruePositive) + "," + cMatrix4.getTwoClassStats(j).TruePositiveRate.ToString("0.00") + "," + cMatrix4.getTwoClassStats(j).FalsePositiveRate.ToString("0.00") + "," + cMatrix4.getTwoClassStats(j).Precision.ToString("0.00") + "," + cMatrix4.getTwoClassStats(j).Recall.ToString("0.00"));


            tw2.WriteLine("\n\nDisgree-Accuracy," + ((double)(cMatrix5.correct() / (cMatrix5.correct() + cMatrix5.incorrect())) * 100).ToString("0.00") + "%");
            tw2.WriteLine("Activity,Num Examples,TP Rate,FP Rate,Precision,Recall");
            for (int j = 0; (j < cMatrix5.numColumns()); j++)
                tw2.WriteLine(cMatrix5.className(j) + "," + (cMatrix5.getTwoClassStats(j).FalseNegative + cMatrix5.getTwoClassStats(j).TruePositive) + "," + cMatrix5.getTwoClassStats(j).TruePositiveRate.ToString("0.00") + "," + cMatrix5.getTwoClassStats(j).FalsePositiveRate.ToString("0.00") + "," + cMatrix5.getTwoClassStats(j).Precision.ToString("0.00") + "," + cMatrix5.getTwoClassStats(j).Recall.ToString("0.00"));


            tw2.WriteLine("\n\nCase 2, Train Offline (Balanced) Test Offline\n");
            //tw2.WriteLine(result2);

            tw2.WriteLine("\n\nTotal-Accuracy(combined)," + ((double)(cMatrix6.correct() / (cMatrix6.correct() + cMatrix6.incorrect())) * 100).ToString("0.00") + "%");
            tw2.WriteLine("Activity,Num Examples,TP Rate,FP Rate,Precision,Recall");
            for (int j = 0; (j < cMatrix6.numColumns()); j++)
                tw2.WriteLine(cMatrix6.className(j) + "," + (cMatrix6.getTwoClassStats(j).FalseNegative + cMatrix6.getTwoClassStats(j).TruePositive) + "," + cMatrix6.getTwoClassStats(j).TruePositiveRate.ToString("0.00") + "," + cMatrix6.getTwoClassStats(j).FalsePositiveRate.ToString("0.00") + "," + cMatrix6.getTwoClassStats(j).Precision.ToString("0.00") + "," + cMatrix6.getTwoClassStats(j).Recall.ToString("0.00"));


            tw2.WriteLine("\n\nAgree-Accuracy," + ((double)(cMatrix7.correct() / (cMatrix7.correct() + cMatrix7.incorrect())) * 100).ToString("0.00") + "%");
            tw2.WriteLine("Activity,Num Examples,TP Rate,FP Rate,Precision,Recall");
            for (int j = 0; (j < cMatrix7.numColumns()); j++)
                tw2.WriteLine(cMatrix7.className(j) + "," + (cMatrix7.getTwoClassStats(j).FalseNegative + cMatrix7.getTwoClassStats(j).TruePositive) + "," + cMatrix7.getTwoClassStats(j).TruePositiveRate.ToString("0.00") + "," + cMatrix7.getTwoClassStats(j).FalsePositiveRate.ToString("0.00") + "," + cMatrix7.getTwoClassStats(j).Precision.ToString("0.00") + "," + cMatrix7.getTwoClassStats(j).Recall.ToString("0.00"));


            tw2.WriteLine("\n\nDisgree-Accuracy," + ((double)(cMatrix8.correct() / (cMatrix8.correct() + cMatrix8.incorrect())) * 100).ToString("0.00") + "%");
            tw2.WriteLine("Activity,Num Examples,TP Rate,FP Rate,Precision,Recall");
            for (int j = 0; (j < cMatrix8.numColumns()); j++)
                tw2.WriteLine(cMatrix8.className(j) + "," + (cMatrix8.getTwoClassStats(j).FalseNegative + cMatrix8.getTwoClassStats(j).TruePositive) + "," + cMatrix8.getTwoClassStats(j).TruePositiveRate.ToString("0.00") + "," + cMatrix8.getTwoClassStats(j).FalsePositiveRate.ToString("0.00") + "," + cMatrix8.getTwoClassStats(j).Precision.ToString("0.00") + "," + cMatrix8.getTwoClassStats(j).Recall.ToString("0.00"));



            tw2.WriteLine("\n\nCase 3, Train Offline (Agreement) Test Offline\n");

            tw2.WriteLine("\n\nTotal-Accuracy(combined)," + ((double)((correctMatricies[0] + agreementCorrectMatrix) / (totalMatricies[0] + agreementTotalMatrix)) * 100).ToString("0.00") + "%");
            tw2.WriteLine("Activity,Num Examples,TP Rate,FP Rate,Precision,Recall");
            for (int j = 0; (j < cMatrix2.numColumns()); j++)
                tw2.WriteLine(cMatricies[0].className(j) + "," + (totalMatrix[0].getTwoClassStats(j).FalseNegative + totalMatrix[0].getTwoClassStats(j).TruePositive) + "," + totalMatrix[0].getTwoClassStats(j).TruePositiveRate.ToString("0.00") + "," + totalMatrix[0].getTwoClassStats(j).FalsePositiveRate.ToString("0.00") + "," + totalMatrix[0].getTwoClassStats(j).Precision.ToString("0.00") + "," + totalMatrix[0].getTwoClassStats(j).Recall.ToString("0.00"));


            tw2.WriteLine("\nAccuracy," + ((double)((correctMatricies[0] + agreementCorrectMatrix) / (totalMatricies[0] + agreementTotalMatrix)) * 100).ToString("0.00") + "%");
            tw2.WriteLine("\n\nAgree-Accuracy," + ((double)(agreementCorrectMatrix / agreementTotalMatrix) * 100).ToString("0.00") + "%");
            tw2.WriteLine("Activity,Num Examples,TP Rate,FP Rate,Precision,Recall");
            for (int j = 0; (j < cMatrix2.numColumns()); j++)
                tw2.WriteLine(cMatrix2.className(j) + "," + (cMatrix2.getTwoClassStats(j).FalseNegative + cMatrix2.getTwoClassStats(j).TruePositive) + "," + cMatrix2.getTwoClassStats(j).TruePositiveRate.ToString("0.00") + "," + cMatrix2.getTwoClassStats(j).FalsePositiveRate.ToString("0.00") + "," + cMatrix2.getTwoClassStats(j).Precision.ToString("0.00") + "," + cMatrix2.getTwoClassStats(j).Recall.ToString("0.00"));

            tw2.WriteLine("\n\nDisagree-Accuracy," + ((double)(correctMatricies[0] / totalMatricies[0]) * 100).ToString("0.00") + "%");
            tw2.WriteLine("Activity,Num Examples,TP Rate,FP Rate,Precision,Recall");
            for (int j = 0; (j < cMatrix2.numColumns()); j++)
                tw2.WriteLine(cMatricies[0].className(j) + "," + (cMatricies[0].getTwoClassStats(j).FalseNegative + cMatricies[0].getTwoClassStats(j).TruePositive) + "," + cMatricies[0].getTwoClassStats(j).TruePositiveRate.ToString("0.00") + "," + cMatricies[0].getTwoClassStats(j).FalsePositiveRate.ToString("0.00") + "," + cMatricies[0].getTwoClassStats(j).Precision.ToString("0.00") + "," + cMatricies[0].getTwoClassStats(j).Recall.ToString("0.00"));


            tw2.WriteLine("\n\nCase 4, Train Realtime - Test Offline\n");
            tw2.WriteLine("\nAccuracy," + ((double)(realtimeMatrix.correct() / (realtimeMatrix.correct() + realtimeMatrix.incorrect())) * 100).ToString("0.00") + "%");
            tw2.WriteLine("Activity,Num Examples,TP Rate,FP Rate,Precision,Recall");
            for (int j = 0; (j < realtimeMatrix.numColumns()); j++)
                tw2.WriteLine(realtimeMatrix.className(j) + "," + (realtimeMatrix.getTwoClassStats(j).FalseNegative + realtimeMatrix.getTwoClassStats(j).TruePositive) + "," + realtimeMatrix.getTwoClassStats(j).TruePositiveRate.ToString("0.00") + "," + realtimeMatrix.getTwoClassStats(j).FalsePositiveRate.ToString("0.00") + "," + realtimeMatrix.getTwoClassStats(j).Precision.ToString("0.00") + "," + realtimeMatrix.getTwoClassStats(j).Recall.ToString("0.00"));
            tw2.Close();


            tw4.WriteLine("Unbalanced Offline Test");
            tw4.WriteLine(cMatrix3.toString());
            tw4.WriteLine(cMatrix4.toString());
            tw4.WriteLine(cMatrix5.toString());

            tw4.WriteLine("Balanced Offline Test");
            tw4.WriteLine(cMatrix6.toString());
            tw4.WriteLine(cMatrix7.toString());
            tw4.WriteLine(cMatrix8.toString());

            //tw4.WriteLine(eval.toMatrixString());
            tw4.WriteLine("Agreement\n");
            tw4.WriteLine(cMatrix2.toString());
            for (int i = 0; (i < annotaters); i++)
            {
                tw4.WriteLine("Disagreement " + i + "\n");
                tw4.WriteLine(cMatricies[i].toString());
                tw4.WriteLine("Total " + i + "\n");
                tw4.WriteLine(totalMatrix[i].toString());
            }
            tw4.WriteLine("Realtime\n");
            tw4.WriteLine(realtimeMatrix.toString());
            tw4.Close();
   


            Autism Analysis end*/


        }
    }
}

