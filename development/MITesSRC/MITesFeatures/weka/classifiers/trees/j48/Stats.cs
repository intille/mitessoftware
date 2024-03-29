/*
*    Stats.java
*    Copyright (C) 1999 Eibe Frank
*
*/
using System;
using weka.core;
namespace weka.classifiers.trees.j48
{
	
	/// <summary> Class implementing a statistical routine needed by J48 to
	/// compute its error estimate.
	/// 
	/// </summary>
	/// <author>  Eibe Frank (eibe@cs.waikato.ac.nz)
	/// </author>
	/// <version>  $Revision: 1.7 $
	/// </version>
	public class Stats
	{
		
		/// <summary> Computes estimated extra error for given total number of instances
		/// and error using normal approximation to binomial distribution
		/// (and continuity correction).
		/// 
		/// </summary>
		/// <param name="N">number of instances
		/// </param>
		/// <param name="e">observed error
		/// </param>
		/// <param name="CF">confidence value
		/// </param>
		public static double addErrs(double N, double e, float CF)
		{
			
			// Ignore stupid values for CF
			if (CF > 0.5)
			{
				System.Console.Error.WriteLine("WARNING: confidence value for pruning " + " too high. Error estimate not modified.");
				return 0;
			}
			
			// Check for extreme cases at the low end because the
			// normal approximation won't work
			if (e < 1)
			{
				
				// Base case (i.e. e == 0) from documenta Geigy Scientific
				// Tables, 6th edition, page 185
				double base_Renamed = N * (1 - System.Math.Pow(CF, 1 / N));
				if (e == 0)
				{
					return base_Renamed;
				}
				
				// Use linear interpolation between 0 and 1 like C4.5 does
				return base_Renamed + e * (addErrs(N, 1, CF) - base_Renamed);
			}
			
			// Use linear interpolation at the high end (i.e. between N - 0.5
			// and N) because of the continuity correction
			if (e + 0.5 >= N)
			{
				
				// Make sure that we never return anything smaller than zero
				return System.Math.Max(N - e, 0);
			}
			
			// Get z-score corresponding to CF
			double z = Statistics.normalInverse(1 - CF);
			
			// Compute upper limit of confidence interval
			double f = (e + 0.5) / N;
			double r = (f + (z * z) / (2 * N) + z * System.Math.Sqrt((f / N) - (f * f / N) + (z * z / (4 * N * N)))) / (1 + (z * z) / N);
			
			return (r * N) - e;
		}
	}
}