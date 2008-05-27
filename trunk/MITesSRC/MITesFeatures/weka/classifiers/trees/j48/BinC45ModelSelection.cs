/*
*    BinC45ModelSelection.java
*    Copyright (C) 1999 Eibe Frank
*
*/
using System;
using weka.core;
namespace weka.classifiers.trees.j48
{
	
	/// <summary> Class for selecting a C4.5-like binary (!) split for a given dataset.
	/// 
	/// </summary>
	/// <author>  Eibe Frank (eibe@cs.waikato.ac.nz)
	/// </author>
	/// <version>  $Revision: 1.8 $
	/// </version>

	public class BinC45ModelSelection:ModelSelection
	{
		
		/// <summary>Minimum number of instances in interval. </summary>
		private int m_minNoObj;
		
		/// <summary>The FULL training dataset. </summary>
		private Instances m_allData;
		
		/// <summary> Initializes the split selection method with the given parameters.
		/// 
		/// </summary>
		/// <param name="minNoObj">minimum number of instances that have to occur in
		/// at least two subsets induced by split
		/// </param>
		/// <param name="allData">FULL training dataset (necessary for selection of
		/// split points).  
		/// </param>
		public BinC45ModelSelection(int minNoObj, Instances allData)
		{
			m_minNoObj = minNoObj;
			m_allData = allData;
		}
		
		/// <summary> Sets reference to training data to null.</summary>
		public virtual void  cleanup()
		{
			
			m_allData = null;
		}
		
		/// <summary> Selects C4.5-type split for the given dataset.</summary>
		public override ClassifierSplitModel selectModel(Instances data)
		{
			
			double minResult;
			//double currentResult;
			BinC45Split[] currentModel;
			BinC45Split bestModel = null;
			NoSplit noSplitModel = null;
			double averageInfoGain = 0;
			int validModels = 0;
			bool multiVal = true;
			Distribution checkDistribution;
			double sumOfWeights;
			int i;
			
			try
			{
				
				// Check if all Instances belong to one class or if not
				// enough Instances to split.
				checkDistribution = new Distribution(data);
				noSplitModel = new NoSplit(checkDistribution);
				if (Utils.sm(checkDistribution.total(), 2 * m_minNoObj) || Utils.eq(checkDistribution.total(), checkDistribution.perClass(checkDistribution.maxClass())))
					return noSplitModel;
				
				// Check if all attributes are nominal and have a 
				// lot of values.
				System.Collections.IEnumerator enu = data.enumerateAttributes();
				//UPGRADE_TODO: Method 'java.util.Enumeration.hasMoreElements' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilEnumerationhasMoreElements'"
				while (enu.MoveNext())
				{
					//UPGRADE_TODO: Method 'java.util.Enumeration.nextElement' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilEnumerationnextElement'"
                    weka.core.Attribute attribute = (weka.core.Attribute)enu.Current;
					if ((attribute.Numeric) || (Utils.sm((double) attribute.numValues(), (0.3 * (double) m_allData.numInstances()))))
					{
						multiVal = false;
						break;
					}
				}
				currentModel = new BinC45Split[data.numAttributes()];
				sumOfWeights = data.sumOfWeights();
				
				// For each attribute.
				for (i = 0; i < data.numAttributes(); i++)
				{
					
					// Apart from class attribute.
					if (i != (data).classIndex())
					{
						
						// Get models for current attribute.
						currentModel[i] = new BinC45Split(i, m_minNoObj, sumOfWeights);
						currentModel[i].buildClassifier(data);
						
						// Check if useful split for current attribute
						// exists and check for enumerated attributes with 
						// a lot of values.
						if (currentModel[i].checkModel())
							if ((data.attribute(i).Numeric) || (multiVal || Utils.sm((double) data.attribute(i).numValues(), (0.3 * (double) m_allData.numInstances()))))
							{
								averageInfoGain = averageInfoGain + currentModel[i].infoGain();
								validModels++;
							}
					}
					else
						currentModel[i] = null;
				}
				
				// Check if any useful split was found.
				if (validModels == 0)
					return noSplitModel;
				averageInfoGain = averageInfoGain / (double) validModels;
				
				// Find "best" attribute to split on.
				minResult = 0;
				for (i = 0; i < data.numAttributes(); i++)
				{
					if ((i != (data).classIndex()) && (currentModel[i].checkModel()))
					// Use 1E-3 here to get a closer approximation to the original
					// implementation.
						if ((currentModel[i].infoGain() >= (averageInfoGain - 1e-3)) && Utils.gr(currentModel[i].gainRatio(), minResult))
						{
							bestModel = currentModel[i];
							minResult = currentModel[i].gainRatio();
						}
				}
				
				// Check if useful split was found.
				if (Utils.eq(minResult, 0))
					return noSplitModel;
				
				// Add all Instances with unknown values for the corresponding
				// attribute to the distribution for the model, so that
				// the complete distribution is stored with the model. 
				bestModel.distribution().addInstWithUnknown(data, bestModel.attIndex());
				
				// Set the split point analogue to C45 if attribute numeric.
				bestModel.SplitPoint = m_allData;
				return bestModel;
			}
			catch (System.Exception e)
			{
                System.Console.WriteLine(e.StackTrace + " " + e.Message);
			}
			return null;
		}
		
		/// <summary> Selects C4.5-type split for the given dataset.</summary>
		public override ClassifierSplitModel selectModel(Instances train, Instances test)
		{
			
			return selectModel(train);
		}
	}
}