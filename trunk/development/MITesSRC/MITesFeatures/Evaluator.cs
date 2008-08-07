using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using weka.core;
using weka.classifiers;
using weka.classifiers.trees;
using weka.filters;
using weka.filters.unsupervised.instance;

namespace MITesFeatures
{
    public class Evaluator
    {
        public static int DEFAULT_FOLDS = 10;
        private Instances data;
        //RemoveFolds foldsFilter;

        int classCount;
        int numExamples;

        //Evaluation of Training Data Size
        double[, ,] trainingSizeMatrix;
        private static int TRAINING_INCRMENETS = 30;

        public Evaluator(string arffFile)
        {

            this.data = new Instances(new StreamReader(arffFile));
            this.data.ClassIndex = this.data.numAttributes() - 1;
            this.numExamples = this.data.m_Instances.size();
            this.classCount = this.data.attribute(this.data.numAttributes() - 1).numValues();




            // this.trainingSizeMatrix=new double[
        }


        public void EvaluateIncrementalBatches(int batchSize)
        {
            //Randomize Filter
            Randomize randomizeFilter = new Randomize();
            randomizeFilter.setInputFormat(this.data);

     

            //RemoveRange Filter

            //number of classes
            int numClasses = this.data.attribute(this.data.numAttributes() - 1).numValues();
            Instances[] classInstances = new Instances[numClasses];
            for (int i = 1; (i <= numClasses); i++)
            {
                //RemoveWithValues Filter
                RemoveWithValues removeValuesFilter = new RemoveWithValues();     
                removeValuesFilter.setInputFormat(this.data);
               // removeValuesFilter.set_AttributeIndex("last");
               // removeValuesFilter.
                removeValuesFilter.set_MatchMissingValues(false);

                
                removeValuesFilter.set_NominalIndices("1-1");
                classInstances[i] = Filter.useFilter(this.data, removeValuesFilter);
            }
           
        }
        public void EvaluateIncrementalExamples()
        {
            //Calculate the number of increments for the training data based on the increment size
            int numberIncrements = (int)Math.Ceiling((double)(this.numExamples * (DEFAULT_FOLDS - 1) / DEFAULT_FOLDS) / (double)TRAINING_INCRMENETS);
            this.trainingSizeMatrix = new double[this.classCount, DEFAULT_FOLDS, numberIncrements];
            for (int i = 0; (i < this.classCount); i++)
                for (int j = 0; (j < DEFAULT_FOLDS); j++)
                    for (int k = 0; (k < numberIncrements); k++)
                        this.trainingSizeMatrix[i, j, k] = 0.0;

            //Randomize the data
            Randomize randomizeFilter = new Randomize();
            randomizeFilter.setInputFormat(this.data);
            Instances randomData = Filter.useFilter(this.data, randomizeFilter);




            //Run incremental training data for each fold and store the results for each activity

            for (int i = 1; (i <= DEFAULT_FOLDS); i++)
            {
                //Training folds filter
                RemoveFolds trainingFoldsFilter = new RemoveFolds();
                trainingFoldsFilter.set_NumFolds(DEFAULT_FOLDS);
                trainingFoldsFilter.inputFormat(randomData);
                trainingFoldsFilter.set_InvertSelection(true);
                trainingFoldsFilter.set_Fold(i);
                Instances alltraining = Filter.useFilter(randomData, trainingFoldsFilter);

                RemoveFolds testFoldsFilter = new RemoveFolds();
                testFoldsFilter.set_NumFolds(DEFAULT_FOLDS);
                testFoldsFilter.inputFormat(randomData);
                testFoldsFilter.set_InvertSelection(false);
                testFoldsFilter.set_Fold(i);
                Instances test = Filter.useFilter(randomData, testFoldsFilter);
                for (int j = 1; (j <= numberIncrements); j++)
                {
                    //Range Filter
                    RemoveRange rangeFilter = new RemoveRange();
                    rangeFilter.setInputFormat(alltraining);
                    int first = 1;
                    int last = j * TRAINING_INCRMENETS;
                    if (last > (alltraining.m_Instances.size()))
                        last = alltraining.m_Instances.size();
                    string range = first.ToString() + "-" + last.ToString();
                    rangeFilter.set_InstancesIndices(range);
                    rangeFilter.set_InvertSelection(true);
                    Instances training = Filter.useFilter(alltraining, rangeFilter);

                    //ready for training and testing
                    J48 tree = new J48();         // new instance of tree
                    tree.set_MinNumObj(10);
                    tree.set_ConfidenceFactor((float)0.25);
                    tree.buildClassifier(training);   // build classifier
                    Evaluation eval = new Evaluation(training);
                    eval.evaluateModel(tree, test);


                    //store the results for each activity
                    for (int k = 0; (k < this.classCount); k++)
                    {
                        double tpRate = eval.truePositiveRate(k);
                        trainingSizeMatrix[k, i - 1, j - 1] = +eval.truePositiveRate(k);
                    }

                }


            }


            TextWriter tw = new StreamWriter("evaluation.txt");
            for (int i = 0; (i < this.classCount); i++)
            {
                string line = randomData.attribute(this.data.numAttributes() - 1).value_Renamed(i);

                for (int k = 0; (k < numberIncrements); k++)
                {
                    double percentage = 0.0;
                    for (int j = 0; (j < DEFAULT_FOLDS); j++)
                        percentage += this.trainingSizeMatrix[i, j, k];
                    percentage /= DEFAULT_FOLDS;
                    percentage *= 100;
                    line += "," + percentage.ToString("0.00");
                }
                tw.WriteLine(line);
            }
            tw.Close();


            for (int i = 0; (i < this.classCount); i++)
            {
                string activity = randomData.attribute(this.data.numAttributes() - 1).value_Renamed(i);
                tw = new StreamWriter("evaluation-" + activity + ".txt");
                for (int j = 0; (j < DEFAULT_FOLDS); j++)
                {
                    string line = j.ToString();
                    for (int k = 0; (k < numberIncrements); k++)
                    {
                        double percentage = this.trainingSizeMatrix[i, j, k];
                        percentage /= DEFAULT_FOLDS;
                        percentage *= 100;
                        line += "\t" + percentage.ToString("0.00");
                    }
                    tw.WriteLine(line);
                }
                tw.Close();
            }

        }

    }
}
