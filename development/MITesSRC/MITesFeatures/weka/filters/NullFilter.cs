/*
*    NullFilter.java
*    Copyright (C) 1999 Len Trigg
*
*/
using System;
using weka.core;
namespace weka.filters
{
	
	/// <summary> A simple instance filter that allows no instances to pass
	/// through. Basically just for testing purposes.
	/// 
	/// </summary>
	/// <author>  Len Trigg (trigg@cs.waikato.ac.nz)
	/// </author>
	/// <version>  $Revision: 1.7 $
	/// </version>
	/// <attribute>  System.ComponentModel.DescriptionAttribute("A simple instance filter that allows no instances to pass through.")  </attribute>
	/// <attribute>  System.ComponentModel.BrowsableAttribute(false)  </attribute>
	[Serializable]
	public class NullFilter:Filter
	{
		/// <summary> Sets the format of the input instances.
		/// 
		/// </summary>
		/// <param name="instanceInfo">an Instances object containing the input instance
		/// structure (any instances contained in the object are ignored - only the
		/// structure is required).
		/// </param>
		/// <returns> true if the outputFormat may be collected immediately
		/// </returns>
		public override bool setInputFormat(Instances instanceInfo)
		{
			base.setInputFormat(instanceInfo);
			setOutputFormat(instanceInfo);
			return true;
		}
		
		/// <summary> Input an instance for filtering. Ordinarily the instance is processed
		/// and made available for output immediately. Some filters require all
		/// instances be read before producing output.
		/// 
		/// </summary>
		/// <param name="instance">the input instance
		/// </param>
		/// <returns> true if the filtered instance may now be
		/// collected with output().
		/// </returns>
		/// <exception cref="IllegalStateException">if no input format has been set.
		/// </exception>
		public override bool input(Instance instance)
		{
			if (getInputFormat() == null)
			{
				throw new System.SystemException("No input instance format defined");
			}
			return false;
		}
		
		/// <summary> Main method for testing this class.
		/// 
		/// </summary>
		/// <param name="argv">should contain arguments to the filter: use -h for help
		/// </param>
		//	public static void main(String [] argv) 
		//	{
		//
		//		try 
		//		{
		//			if (Utils.getFlag('b', argv)) 
		//			{
		//				Filter.batchFilterFile(new NullFilter(), argv);
		//			} 
		//			else 
		//			{
		//				Filter.filterFile(new NullFilter(), argv);
		//			}
		//		} 
		//		catch (Exception ex) 
		//		{
		//			System.out.println(ex.getMessage());
		//		}
		//	}
	}
}