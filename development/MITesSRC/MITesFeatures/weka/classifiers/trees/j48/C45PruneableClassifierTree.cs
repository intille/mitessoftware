/*
*    C45PruneableClassifierTree.java
*    Copyright (C) 1999 Eibe Frank
*
*/
using System;
using weka.core;
namespace weka.classifiers.trees.j48
{
	
	/// <summary> Class for handling a tree structure that can
	/// be pruned using C4.5 procedures.
	/// 
	/// </summary>
	/// <author>  Eibe Frank (eibe@cs.waikato.ac.nz)
	/// </author>
	/// <version>  $Revision: 1.11 $
	/// </version>
#if !PocketPC
    [Serializable()]  
#endif
	public class C45PruneableClassifierTree:ClassifierTree
	{
		/// <summary> Computes estimated errors for tree.</summary>
		private double EstimatedErrors
		{
			get
			{
				double errors = 0;
				int i;
				
				if (m_isLeaf)
					return getEstimatedErrorsForDistribution(localModel().distribution());
				else
				{
					for (i = 0; i < m_sons.Length; i++)
						errors = errors + son(i).EstimatedErrors;
					return errors;
				}
			}
			
		}
		/// <summary> Computes errors of tree on training data.</summary>
		private double TrainingErrors
		{
			get
			{
				double errors = 0;
				int i;
				
				if (m_isLeaf)
					return localModel().distribution().numIncorrect();
				else
				{
					for (i = 0; i < m_sons.Length; i++)
						errors = errors + son(i).TrainingErrors;
					return errors;
				}
			}
			
		}
		
		/// <summary>True if the tree is to be pruned. </summary>
		internal bool m_pruneTheTree = false;
		
		/// <summary>The confidence factor for pruning. </summary>
		internal float m_CF = 0.25f;
		
		/// <summary>Is subtree raising to be performed? </summary>
		internal bool m_subtreeRaising = true;
		
		/// <summary>Cleanup after the tree has been built. </summary>
		internal bool m_cleanup = true;
		
		/// <summary> Constructor for pruneable tree structure. Stores reference
		/// to associated training data at each node.
		/// 
		/// </summary>
		/// <param name="toSelectLocModel">selection method for local splitting model
		/// </param>
		/// <param name="pruneTree">true if the tree is to be pruned
		/// </param>
		/// <param name="cf">the confidence factor for pruning
		/// </param>
		/// <exception cref="Exception">if something goes wrong
		/// </exception>
		public C45PruneableClassifierTree(ModelSelection toSelectLocModel, bool pruneTree, float cf, bool raiseTree, bool cleanup):base(toSelectLocModel)
		{
			
			m_pruneTheTree = pruneTree;
			m_CF = cf;
			m_subtreeRaising = raiseTree;
			m_cleanup = cleanup;
		}
		
		/// <summary> Method for building a pruneable classifier tree.
		/// 
		/// </summary>
		/// <exception cref="Exception">if something goes wrong
		/// </exception>
		public override void  buildClassifier(Instances data)
		{
			
			if (data.classAttribute().Numeric)
				throw new Exception("Class is numeric!");
			if (data.checkForStringAttributes())
			{
				throw new Exception("Cannot handle string attributes!");
			}
			data = new Instances(data);
			data.deleteWithMissingClass();
			buildTree(data, m_subtreeRaising);
			collapse();
			if (m_pruneTheTree)
			{
				prune();
			}
			if (m_cleanup)
			{
				cleanup(new Instances(data, 0));
			}
		}
		
		/// <summary> Collapses a tree to a node if training error doesn't increase.</summary>
		public void  collapse()
		{
			
			double errorsOfSubtree;
			double errorsOfTree;
			int i;
			
			if (!m_isLeaf)
			{
				errorsOfSubtree = TrainingErrors;
				errorsOfTree = localModel().distribution().numIncorrect();
				if (errorsOfSubtree >= errorsOfTree - 1e-3)
				{
					
					// Free adjacent trees
					m_sons = null;
					m_isLeaf = true;
					
					// Get NoSplit Model for tree.
					m_localModel = new NoSplit(localModel().distribution());
				}
				else
					for (i = 0; i < m_sons.Length; i++)
						son(i).collapse();
			}
		}
		
		/// <summary> Prunes a tree using C4.5's pruning procedure.
		/// 
		/// </summary>
		/// <exception cref="Exception">if something goes wrong
		/// </exception>
		public virtual void  prune()
		{
			
			double errorsLargestBranch;
			double errorsLeaf;
			double errorsTree;
			int indexOfLargestBranch;
			C45PruneableClassifierTree largestBranch;
			int i;
			
			if (!m_isLeaf)
			{
				
				// Prune all subtrees.
				for (i = 0; i < m_sons.Length; i++)
					son(i).prune();
				
				// Compute error for largest branch
				indexOfLargestBranch = localModel().distribution().maxBag();
				if (m_subtreeRaising)
				{
					errorsLargestBranch = son(indexOfLargestBranch).getEstimatedErrorsForBranch((Instances) m_train);
				}
				else
				{
					//UPGRADE_TODO: The equivalent in .NET for field 'java.lang.Double.MAX_VALUE' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
					errorsLargestBranch = System.Double.MaxValue;
				}
				
				// Compute error if this Tree would be leaf
				errorsLeaf = getEstimatedErrorsForDistribution(localModel().distribution());
				
				// Compute error for the whole subtree
				errorsTree = EstimatedErrors;
				
				// Decide if leaf is best choice.
				if (Utils.smOrEq(errorsLeaf, errorsTree + 0.1) && Utils.smOrEq(errorsLeaf, errorsLargestBranch + 0.1))
				{
					
					// Free son Trees
					m_sons = null;
					m_isLeaf = true;
					
					// Get NoSplit Model for node.
					m_localModel = new NoSplit(localModel().distribution());
					return ;
				}
				
				// Decide if largest branch is better choice
				// than whole subtree.
				if (Utils.smOrEq(errorsLargestBranch, errorsTree + 0.1))
				{
					largestBranch = son(indexOfLargestBranch);
					m_sons = largestBranch.m_sons;
					m_localModel = largestBranch.localModel();
					m_isLeaf = largestBranch.m_isLeaf;
					newDistribution(m_train);
					prune();
				}
			}
		}
		
		/// <summary> Returns a newly created tree.
		/// 
		/// </summary>
		/// <exception cref="Exception">if something goes wrong
		/// </exception>
		protected internal override ClassifierTree getNewTree(Instances data)
		{
			
			C45PruneableClassifierTree newTree = new C45PruneableClassifierTree(m_toSelectModel, m_pruneTheTree, m_CF, m_subtreeRaising, m_cleanup);
			newTree.buildTree((Instances) data, m_subtreeRaising);
			
			return newTree;
		}
		
		/// <summary> Computes estimated errors for one branch.
		/// 
		/// </summary>
		/// <exception cref="Exception">if something goes wrong
		/// </exception>
		private double getEstimatedErrorsForBranch(Instances data)
		{
			Instances[] localInstances;
			double errors = 0;
			int i;
			
			if (m_isLeaf)
				return getEstimatedErrorsForDistribution(new Distribution(data));
			else
			{
				Distribution savedDist = localModel().m_distribution;
				localModel().resetDistribution(data);
				localInstances = (Instances[]) localModel().split(data);
				localModel().m_distribution = savedDist;
				for (i = 0; i < m_sons.Length; i++)
					errors = errors + son(i).getEstimatedErrorsForBranch(localInstances[i]);
				return errors;
			}
		}
		
		/// <summary> Computes estimated errors for leaf.</summary>
		private double getEstimatedErrorsForDistribution(Distribution theDistribution)
		{
			
			if (Utils.eq(theDistribution.total(), 0))
				return 0;
			else
				return theDistribution.numIncorrect() + Stats.addErrs(theDistribution.total(), theDistribution.numIncorrect(), m_CF);
		}
		
		/// <summary> Method just exists to make program easier to read.</summary>
		private ClassifierSplitModel localModel()
		{
			
			return (ClassifierSplitModel) m_localModel;
		}
		
		/// <summary> Computes new distributions of instances for nodes
		/// in tree.
		/// 
		/// </summary>
		/// <exception cref="Exception">if something goes wrong
		/// </exception>
		private void  newDistribution(Instances data)
		{
			
			Instances[] localInstances;
			
			localModel().resetDistribution(data);
			m_train = data;
			if (!m_isLeaf)
			{
				localInstances = (Instances[]) localModel().split(data);
				for (int i = 0; i < m_sons.Length; i++)
					son(i).newDistribution(localInstances[i]);
			}
			else
			{
				
				// Check whether there are some instances at the leaf now!
				if (!Utils.eq(data.sumOfWeights(), 0))
				{
					m_isEmpty = false;
				}
			}
		}
		
		/// <summary> Method just exists to make program easier to read.</summary>
		private C45PruneableClassifierTree son(int index)
		{
			
			return (C45PruneableClassifierTree) m_sons[index];
		}
	}
}