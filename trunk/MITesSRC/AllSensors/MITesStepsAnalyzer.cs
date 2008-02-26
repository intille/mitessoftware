using System;
using System.Collections;
using System.Runtime.InteropServices;
using HousenCS.IO;
using HousenCS.MotionAnalysis;
using SocketServer;

namespace HousenCS.MITes
{
	/// <summary>
	/// Summary description for MITesActivityCounter.
	/// </summary>
	public class MITesStepsAnalyzer
	{
		// SPM == StepsPerMin

		private bool isBeepOnStep = false;

		private MITesDecoder aMITesDecoder;
		private short ID = (short) MITesData.NONE; 
		private int lastSampleTime = 0;

		private SocketTransmitter st;

		private double lastComputedSPM = 0;
		private int lastComputedSPMTime = 0;

		private int lastReportTime = 0;

		private double meanSPM = 0.0;
		private double meanSPMTime = 0.0;

		private double medianSPM = 0.0;
		private double medianMeanSPM = 0.0;
		private double medianSPMTime = 0.0;

		/// <summary>
		/// 
		/// </summary>
		public const int MIN_BPM = 35; 

		/// <summary>
		/// 
		/// </summary>
		public const int MAX_BPM = 83;

		/// <summary>
		/// 
		/// </summary>
		public const int MIN_STEP_SAMPLES = 4; // 4 is good
		
		/// <summary>
		/// 
		/// </summary>
		public const int MIN_MEDIAN_STEP_SAMPLES = 4; 
		
		private const double MAX_BEAT_DELAY_MS = 60000.0/((double)MIN_BPM); 
		private const double MIN_BEAT_DELAY_MS = 60000.0/((double)MAX_BPM); 
 
		private const int MAX_BPS = ((int) (MAX_BPM/60.0)) + 2;

		/// <summary>
		/// 
		/// </summary>
		public const int MEANTIME_S = 20; // 20 was good 
		private int meanTimeMS = MEANTIME_S*1000; 

		/// <summary>
		/// 
		/// </summary>
		public const int MEDIANTIME_S = 10; 
		private int medianTimeMS = MEDIANTIME_S*1000; 

		private short[] stepsVals = new short[MEANTIME_S*MAX_BPS];
		private int[] stepsValTimes = new int[MEANTIME_S*MAX_BPS];
		private int[] beatTime = new int[MEANTIME_S*MAX_BPS];

		PeakBeeper pb = new PeakBeeper();

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public String GetReportKey()
		{
			String s;
			s = ", Steps per min " + ID;
			s += ", Steps per min " + ID + " mean over " + meanTimeMS + "ms";
			s += ", Steps per min " + ID + " median over " + medianTimeMS + "ms";
			s += ", Steps per min " + ID + " mean outliers removed over " + medianTimeMS + "ms";
			return s;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="aTime"></param>
		/// <returns></returns>
		public bool IsNewComputedSPM(int aTime)
		{
			if (aTime < lastComputedSPMTime)
				return true;
			else
				return false;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="aSocketTransmitter"></param>
	public void SetSocketTransmitter(SocketTransmitter aSocketTransmitter)
	{
		st = aSocketTransmitter;
	}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public double GetLastComputedSPM()
		{
			return (double) lastComputedSPM;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public int GetLastComputedSPMTime()
		{
			return lastComputedSPMTime;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public double GetMeanSPM()
		{
			return meanSPM;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public double GetMedianSPM()
		{
			return medianSPM;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public double GetMedianMeanSPM()
		{
			return medianMeanSPM;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public double GetMeanSPMTime()
		{
			return meanSPMTime;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public double GetMedianSPMTime()
		{
			return medianSPMTime;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public String GetReport()
		{
			String str; 
			if (IsNewComputedSPM(lastReportTime))
			{
				str = "," + GetLastComputedSPM();
				lastReportTime = GetLastComputedSPMTime();
			}
			else
				str = "," + MITesActivityLogger.NONE_STRING;
 
			if (GetMeanSPM() == 0)
				str = str + "," + MITesActivityLogger.NONE_STRING;
			else
				str	= str + "," + Math.Round(GetMeanSPM(),2);
	
			if (GetMedianSPM() == 0)
				str = str + "," + MITesActivityLogger.NONE_STRING;
			else
				str	= str + "," + Math.Round(GetMedianSPM(),2);

			if (GetMedianMeanSPM() == 0)
				str = str + "," + MITesActivityLogger.NONE_STRING;
			else
				str	= str + "," + Math.Round(GetMedianMeanSPM(),2);

			return str; 
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="aMITesDecoder"></param>
		/// <param name="isBeep"></param>
		/// <param name="anID"></param>
		public MITesStepsAnalyzer(MITesDecoder aMITesDecoder, int anID, bool isBeep)
		{
			isBeepOnStep = isBeep; 
			this.ID = (short) anID; 
			this.aMITesDecoder = aMITesDecoder;

			for (int i = 0; i < stepsVals.Length; i++)
			{
				meanSPM = 0;
				meanSPMTime = 0;
				medianSPM = 0;
				medianMeanSPM = 0;
				medianSPMTime = 0;
				lastComputedSPM = 0;
				lastComputedSPMTime = 0;
				stepsVals[i] = 0;
				stepsValTimes[i] = 0;
			}
		}


		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public int GetMeanTimeMS()
		{
			return meanTimeMS;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="aMITesDecoder"></param>
		public MITesStepsAnalyzer(MITesDecoder aMITesDecoder): this(aMITesDecoder, MITesData.NONE, false)
		{
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="isBeep"></param>
		public void SetBeepOnStep(bool isBeep)
		{
			isBeepOnStep = isBeep;
		}

		private void Warning(String aMsg)
		{
			Console.WriteLine("WARNING: " + aMsg);
		}


		/// <summary>
		/// Check if computed steps per min value is in a reasonable range for a normal walking or brisk walking person ??? -???.
		/// </summary>
		/// <param name="stepsPerMinVal">A steps per minute rate value</param>
		/// <returns>True if a reasonable steps per minute value</returns>
		public bool InReasonableRange(int stepsPerMinVal)
		{	
			if (stepsPerMinVal <= 65)
				return false; // Too slow 
			else if (stepsPerMinVal >= 140)
				return false; // Too fast 
			else
				return true;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="aTimeIntervalMS"></param>
		/// <returns></returns>
		public double GetRecentSPM(int aTimeIntervalMS)
		{
			if ((Environment.TickCount-lastComputedSPMTime) < aTimeIntervalMS)
				return lastComputedSPM;
			else
				return (double) MITesData.NONE; 
		}

		[DllImport("kernel32.dll")]
		static extern bool Beep(int freq,int duration);

		//bool isMainPeak = false;
		private void DetectPeak()
		{
			peakTime = pb.IsPeak ();
			if (peakTime != 0)
			{
				if ((peakTime - lastTimePeak) > MIN_BEAT_DELAY_MS)
				{

					lastComputedSPM = Math.Round((60000.0/(peakTime-lastTimePeak)),2);	//isMainPeak = !isMainPeak;
					lastComputedSPMTime = peakTime; 

					//if (isMainPeak)
					//{
						if (isBeepOnStep)
							Beep(3000,50);
					st.SendData ("STEP");
					//Beep(2000,5);
						lastTimePeak = peakTime;
						AddSteps(1, peakTime, meanTimeMS);
						Console.WriteLine ("PEAK");
					//}
					//else
					//{
						//Beep(1000,5);
					//	Console.WriteLine ("Secondary PEAK");
					//}
					//Beep(2000,10);
				}
				else
				{
					//Console.WriteLine ("Double PEAK");					
				}
			}			
		}

		private int lastTimePeak = 0;
		int peakTime = 0;
		/// <summary>
		/// Main function that does the compuations
		/// </summary>
		private void RecomputeStepsPerMinute()
		{
			int time = Environment.TickCount;
			// Now compute the actual steps per minute over the window time 
			meanSPM = MeanSPM(time, meanTimeMS);
			meanSPMTime = time;

			medianSPM = MedianSPM(time, medianTimeMS);
			medianSPMTime = time;

			//Console.WriteLine ("Steps per minute" + lastComputedSPM);
		}

		/// <summary>
		/// 
		/// </summary>
		public void Update()
		{
			for (int i = 0; i < aMITesDecoder.someMITesDataIndex; i++)
			{
				if (aMITesDecoder.someMITesData[i].type == (int) MITesTypes.ACCEL) 
				{
					if (ID == aMITesDecoder.someMITesData[i].channel) //Check right accel ID
					{
						// Got a good accel value, so add it to the peak detector 
						lastSampleTime = Environment.TickCount;
						pb.AddSample (aMITesDecoder.someMITesData[i].x, aMITesDecoder.someMITesData[i].y, aMITesDecoder.someMITesData[i].z, aMITesDecoder.someMITesData[i].timeStamp);
						DetectPeak();
					}
				}
			}
			RecomputeStepsPerMinute();
		}

		private int GetOpenStepsSlot(int aTimeMS, int aveTimeMS)
		{
			int index = 0;
			bool isFound = false;
			while ((!isFound) && (index < stepsVals.Length))
			{
				if ((aTimeMS - stepsValTimes[index]) > aveTimeMS)
					return index;
				index++;
			}
			Console.WriteLine ("ERROR in GetOpenStepsSlot");
			return 0;
		}

		int j; 
		int lastBeatTime = 0; 
		int diffTime = 0; 

		private void AddSteps(short stepsVal, int stepsTime, int windowTimeMS)
		{
			j = GetOpenStepsSlot(stepsTime, windowTimeMS);
			stepsVals[j] = stepsVal;
			stepsValTimes[j] = stepsTime;
			diffTime = stepsTime - lastBeatTime;
 
			Console.WriteLine ("BeatTime: " + diffTime + "    " + Math.Round ((60000.0/diffTime),1));
			if (diffTime < MAX_BEAT_DELAY_MS)
				beatTime[j] = diffTime; 
			else
				beatTime[j] = 0; 

			lastBeatTime = stepsTime; 
		}

		private double MeanSPM(int aTimeMS, int aveTimeMS)
		{
			int sum = 0; 
			int count = 0;
//			int minTime = Environment.TickCount;
//			int maxTime = 0; 

			for (int i = 0; i < stepsVals.Length; i++)
			{
				if ((aTimeMS - stepsValTimes[i]) <= aveTimeMS)
				{
					if (beatTime[i] !=0)
					{
						sum += beatTime[i];
						count++;
					}
				}
			}
//			double mins = 0;
//			if ((count == 0) || ((maxTime-minTime)<=0))
//				mins = 0;
//			else
//				mins = ((maxTime - minTime)/1000.0);
			if (count > MIN_STEP_SAMPLES) //4
			{
				return (60000.0/((double) sum / count));
			}
			else
				return 0.0;
		}

		ArrayList median = new ArrayList(60);
		int medCount = 0;
		int halfCount = 0;
		int val = 0;
		double ret = 0.0;
		double medSum = 0.0;
		private double MedianSPM(int aTimeMS, int aveTimeMS)
		{
			medCount = 0; 
			median.Clear ();
			
			for (int i = 0; i < stepsVals.Length; i++)
			{
				if ((aTimeMS - stepsValTimes[i]) <= aveTimeMS)
				{
					if (beatTime[i] !=0)
					{
						median.Add (beatTime[i]);
						medCount++;
					}
				}
			}

			median.Sort ();

			if (medCount > MIN_MEDIAN_STEP_SAMPLES) //4
			{
				halfCount = (int) Math.Floor(medCount/2.0);
				val = (int) median[halfCount];
				ret = 60000.0/((double)val);

				int outliers = (int) Math.Floor(Math.Max(1,medCount*.2));
				medSum = 0;
				for (int i = outliers; i < (medCount-outliers); i++)
					medSum += (int) median[i];
				medianMeanSPM = 60000.0/((double) medSum/(medCount -2*outliers));

				return ret;
			}
			else
			{
				medianMeanSPM = 0;
				return 0.0;				
			}
		}

	
//			for (int i = 0; i < stepsVals.Length; i++)
//			{
//				if ((aTimeMS - stepsValTimes[i]) <= aveTimeMS)
//				{
//					if (maxTime <= stepsValTimes[i])
//						maxTime = stepsValTimes[i];
//					if (minTime >= stepsValTimes[i])
//						minTime = stepsValTimes[i];
//					sum += stepsVals[i];
//					count++;
//				}
//			}
//			double mins = 0;
//			if ((count == 0) || ((maxTime-minTime)<=0))
//				mins = 0;
//			else
//				mins = ((maxTime - minTime)/1000.0);
//			if (count > 4)
//			{
//				return (((double) sum / mins)*60);
//			}
//			else
//				return 0.0;
//		}
	}
}
