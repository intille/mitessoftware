using System;
using System.Collections;
using System.IO;
using System.Threading;
using HousenCS.IO;
using HousenCS.MITes;

namespace HousenCS.MITes
{
	/// <summary>
	/// A MITesLoggerReader with multiple receivers reads the binary file saved by a MITesLogger and
	/// can replace an aMITesDecoder.GetSensorData(mrc) call. 
	/// </summary>
	public class MITesLoggerReaderMR
	{
        private class BFileInfo
        {
            public BFileInfo()
            {                
            }

            //public DateTime startTime;
            public string fileRec1 = "";
            public string fileRec2 = ""; 
        }

	    private static DateTime aRefDate = DateTime.Now;
	    private static bool isRefDateSet = false; 

        private ByteReader br1;
        private ByteReader br2;

        private int fileIndex = 0; 

        private MITesDecoder aMITesDecoder;

        ArrayList someBinaryFiles = new ArrayList();

        private void GenerateBinaryFileList(string aDataDir)
        {
            // Determine number of receivers

            string rec1Dir = aDataDir + "\\1";
            string rec2Dir = aDataDir + "\\2";

            int numRec = 0;
            if (Directory.Exists(rec1Dir))
                numRec++; 
            if (Directory.Exists(rec2Dir))
                numRec++; 

            if (numRec == 0)
                return;

            string[] someDays1 = new string[0];
            string[] someDays2 = new string[0];

            // First, find all days 
            someDays1 = Directory.GetDirectories(rec1Dir); 
            if (numRec == 2)
                someDays2 = Directory.GetDirectories(rec2Dir); 
            
            //Merge and sort the days from both receiver directories 
            ArrayList someDays = new ArrayList();

            foreach (string d in someDays1)
            {
                someDays.Add(Path.GetFileName(d));
            }
            foreach (string d in someDays2)
            {
                if (!someDays.Contains(Path.GetFileName(d)))
                    someDays.Add(Path.GetFileName(d));
            }
            someDays.Sort();

            foreach (string d in someDays)
            {
                Console.WriteLine("DAY: " + d);
            } 

            // Now that we have a list of days sorted by date, go into each folder
            // for both receivers, and put together ordered list of binary files. 

            foreach (string d in someDays)
            {
                // Loop over possible hours and add to list
                for (int i = 0; i < 24; i++)
                {
                    BFileInfo aBFileInfo = new BFileInfo();
                    // Check for hour dir in rec 1
                    if (Directory.Exists(rec1Dir + "\\" + d + "\\" + i))
                        aBFileInfo.fileRec1 = GetBinaryMITesFile(rec1Dir + "\\" + d + "\\" + i);
                    if (Directory.Exists(rec2Dir + "\\" + d + "\\" + i))
                        aBFileInfo.fileRec2 = GetBinaryMITesFile(rec2Dir + "\\" + d + "\\" + i);

                    // Add if one of the receiver directories had a file
                    if ((aBFileInfo.fileRec1 != "") || (aBFileInfo.fileRec2 != ""))
                        someBinaryFiles.Add(aBFileInfo);
                }
            }
            Console.WriteLine(someBinaryFiles); 
        }

        /// <summary>
        /// Look for a MITesAccelBytes*.b file and return the full path
        /// </summary>
        /// <param name="aDirPath"></param>
        /// <returns></returns>
        private string GetBinaryMITesFile(string aDirPath)
        {
            string[] matchFiles = Directory.GetFiles(aDirPath, "MITesAccelBytes*.b");
            if (matchFiles.Length != 1)
            {
                Console.WriteLine("No data file found in " + aDirPath);
                return ""; 
            }
            else
            {
                return matchFiles[0];
            }            
        }

		/// <summary>
		/// Initialize an object that will read raw binary data in PlaceLab multi-receiver format and open first files.
		/// </summary>
		/// <param name="aMITesDecoder">MITesDecoder object</param>
        /// <param name="aDataDir">Data directory for MITes data</param>
        public MITesLoggerReaderMR(MITesDecoder aMITesDecoder, String aDataDir)
		{
		    GenerateBinaryFileList(aDataDir);

			this.aMITesDecoder = aMITesDecoder;
            SetupNextFiles(0);
		}

        /// <summary>
        /// Check if the dirPath is in the expected format. 
        /// </summary>
        /// <param name="aDirPath"></param>
        public static bool IsValidDirectory(string aDirPath)
        {
            string subjectDataFile = aDirPath + "\\SubjectData.xml";
            string sensorDataFile = aDirPath + "\\SensorData.xml";
            string activityLabelsFile = aDirPath + "\\ActivityLabels.xml";

            if (File.Exists(subjectDataFile) &&
                File.Exists(sensorDataFile) &&
                File.Exists(activityLabelsFile))
                return true;
            else
                return false; 
        }

	    private bool SetupNextFiles(int index)
		{
	        dTimeStamp1 = 0;
	        dTimeStamp2 = 0; 

            if (br1 != null)
                br1.CloseFile();
	        if (br2 != null)
                br2.CloseFile();

	        br1 = null;
	        br2 = null; 
	       
	        string f1 = ((BFileInfo) someBinaryFiles[index]).fileRec1;  
	        string f2 = ((BFileInfo) someBinaryFiles[index]).fileRec2;  
            if (f1 != "")
            {
                br1 = new ByteReader(f1);
                br1.OpenFile();              
                Console.WriteLine("Opening file for read: " + f1);
            }
            if (f2 != "")
            {
                br2 = new ByteReader(f2);
                br2.OpenFile();
                Console.WriteLine("Opening file for read: " + f2);
            }

            if ((br1 == null) || (br2 == null))
                return false;
            else
            {
                return true;                 
            }
		}

        //static int MAX_BYTES = 5 * 30;
        //byte[] someBytes = new byte[MAX_BYTES];  

		private static byte[] b = new byte[1];
        private static int[] ts = new int[1];
		
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="br"></param>
        ///// <param name="theLastTime"></param>
        ///// <returns></returns>
        ////public static int ReadTimeStamp(ByteReader br, int theLastTime)
        //{
        //    int lastTime = theLastTime;  
        //    //Console.WriteLine ("Read time stamp");
        //    bool readValid = br.ReadByte(b);

        //    if (!readValid)
        //    {
        //        return MITesData.NONE;
        //    }

        //    if (b[0] == ((int) 255))
        //    {
        //        //Console.WriteLine ("Marker");
        //        // Marker, so read the whole timecode
        //            br.ReadInt (ts);
        //            lastTime = ts[0];
        //            //Console.WriteLine ("Timecode: " + lastTime);
        //    }
        //    else
        //    {
        //        // Read only the difference between this and previous time (less than 255 ms)
        //        lastTime += (int) b[0];
        //        //Console.WriteLine ("Diff byte: " + b[0] + " modified Timecode: " + lastTime);
        //    }
        //    return lastTime;
        //}

        private static byte[] b6 = new byte[6];

        private static byte[] refByte = { 0x7e, 0x7e, 0x7e, 0x7e, 0x7e, 0x7e };
        private static double refTime = UnixTime.DecodeUnixTimeCodeBytes(refByte);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="br"></param>
    /// <param name="aLastUnixTime"></param>
    /// <returns></returns>
        public static double ReadUnixTimeStamp(ByteReader br, double aLastUnixTime)
		{
		    double lastUnixTime = aLastUnixTime; 
			//Console.WriteLine ("Read UNIX time stamp");
			bool readValid = br.ReadByte(b);

			if (!readValid)
			{
				return MITesData.NONE;
			}

			if (b[0] == ((int) 255))
			{
                Console.WriteLine("Marker: " + debugCount);
				// Marker, so read the whole timecode
				readValid = br.ReadBytes(b6);

                if (!readValid)
                    return MITesData.NONE; 

				lastUnixTime = UnixTime.DecodeUnixTimeCodeBytesFixed(b6); // SSI Added test

                if (lastUnixTime == refTime)
                {
                    // This is a the so-called sync byte. Need to read 5 more bytes and ignore
                    for (int r = 0; r < 5; r++)
                    {
                        readValid = br.ReadByte(b);

                        if (!readValid)
                            return MITesData.NONE; 
                    }
                    //Console.WriteLine("SYNC byte: keep time as: " + lastUnixTime);

                    //Now read the time byte and add to the existing time
                    readValid = br.ReadByte(b);
                    
                    if (!readValid)
                        return MITesData.NONE;
                    
                    return aLastUnixTime + (int)b[0];
                }

			    //Console.WriteLine ("UNIX Timecode: " + lastUnixTime);
                DateTime junk = new DateTime();
                UnixTime.GetDateTime((long)lastUnixTime, out junk);
                //Console.WriteLine("UNIX Timecode date: " + junk);

                //junk = new DateTime(2007,9,9,8,16,5,609);
                //double junkme = UnixTime.GetUnixTime(junk);
                //UnixTime.GetDateTime((long)junkme,out junk);
                //Console.WriteLine("NEW DATE: " + junk);
                //byte[] somebytes = UnixTime.GetUnixTimeBytes(junkme);
                //double newUnixTime = UnixTime.DecodeUnixTimeCodeBytesFixed(somebytes);
                //UnixTime.GetDateTime((long) newUnixTime, out junk); 
                //Console.WriteLine("NEW DATE: " + junk);

                if (!isRefDateSet)
                {
                    aRefDate = junk.AddDays(-2);
                    isRefDateSet = true;                     
                }

                //lastUnixTime = UnixTime.DecodeUnixTimeCodeBytesFixed(b6);
                //DateTime dt = new DateTime();
                //UnixTime.GetDateTime((long) lastUnixTime, out dt);
                //Console.WriteLine("TEST: " + dt);                
			}
			else
			{

                if (lastUnixTime == 0)
                {
                    Console.WriteLine("ERROR: Last unix time is zero for some reason!");
                }
				// Read only the difference between this and previous time (less than 255 ms)
				lastUnixTime += (int) b[0];
				//Console.WriteLine ("Diff byte: " + b[0] + " modified UNIX Timecode: " + lastUnixTime);
			}
			return lastUnixTime;
		}

		static byte[] temp = new byte[1];
		int someMITesDataIndex;


        private double dTimeStamp1 = 0;
        private double dTimeStamp2 = 0;
        private double dLastTimeStamp1 = 0;
        private double dLastTimeStamp2 = 0;
        //private int diffTime1 = 0;  
        //private int diffTime2 = 0;
        //private int diffMS1 = 0;
        //private int diffMS2 = 0;
        private double lastTimeStampTime1 = 0;
        private double lastTimeStampTime2 = 0; 

        private bool isEndFile1 = false;
	    private bool isEndFile2 = false;
        //private int timeToWait1 = 0;
        //private int timeToWait2 = 0;

        private byte[] tempByte = new byte[1];

        private DateTime GetDateTime(double aUnixTime)
        {
            DateTime dt = new DateTime();
            UnixTime.GetDateTime((long) aUnixTime,out dt);
            return dt; 
        }

        private double lastGoodTime1 = 0;
        private double lastGoodTime2 = 0;
//	    private bool isLastMatch = true;
	    private static long debugCount = 0;

        /// <summary>
        /// Decode multiple receiver data that has been saved by MITesLogger (always send multiple of 4 bytes). A MITesLogger saves
        /// MITesData. This reads it back in so it behaves exactly as data read from the 
        /// serial port. Useful for "playing back" data that has been saved. 
        /// </summary>
        /// <param name="someData">The array in which the resulting MITesData will be stored.</param>
        /// <param name="dataIndex">The current index of the array in which the MITesData will be stored. (This will append data onto the end of existing data if needed).</param>
        /// <param name="br1">A ByteReader object that has been opened to the proper file for receiver 1.</param>
        /// <param name="br2">A ByteReader object that has been opened to the proper file for receiver 2.</param>
        /// <returns>The new index for the someData array.</returns>
        public int DecodePLFormatMR(MITesData[] someData, int dataIndex, ByteReader br1, ByteReader br2)
        {
            isEndFile1 = false;
	        isEndFile2 = false;

            int fileUsed = 0; 

            // Determine if consumed next data point from each file. Value of 0 indicates yes and get next value. 

            if (dTimeStamp1 == 0)
                dTimeStamp1 = ReadUnixTimeStamp(br1, dLastTimeStamp1);
            if (dTimeStamp2 == 0)
                dTimeStamp2 = ReadUnixTimeStamp(br2, dLastTimeStamp2);

            debugCount++;

            DateTime dt1 = new DateTime();
            UnixTime.GetDateTime((long)dTimeStamp1, out dt1);
            DateTime dt2 = new DateTime();
                UnixTime.GetDateTime((long)dTimeStamp2, out dt2);

            //if (((dTimeStamp1 != MITesData.NONE) && (dTimeStamp2 != MITesData.NONE)) &&
            //    ((dt1.Second != dt2.Second) || (dt1.Minute != dt2.Minute)))
            //{
            //    //isLastMatch = false;
            //        Console.WriteLine("DATES: " + Environment.NewLine + dt1 + Environment.NewLine + dt2 + "    " + debugCount);
            //}

            if (dTimeStamp1 == (double)MITesData.NONE)
            {
                //Console.WriteLine("End of file 1: " + GetDateTime(lastGoodTime1) + " " + GetDateTime(lastGoodTime2));
                isEndFile1 = true; 
            }

            if (dTimeStamp2 == (double)MITesData.NONE)
            {
                //Console.WriteLine("End of file 2: " + GetDateTime(lastGoodTime1) + " " + GetDateTime(lastGoodTime2));
                isEndFile2 = true;
            }

            if (isEndFile1 && isEndFile2)
            {
                Console.WriteLine("End of both files.");
                return 0;                
            }

            // If at this point, there is some data to read in one of the files 

            #region Thread wait (do in the future) 
            // Insert waiting code here in the future for graphing capability option 
            //diffMS1 = (int)(dTimeStamp1 - dLastTimeStamp1);
            //if ((dLastTimeStamp1 != 0) && (dTimeStamp1 != 0))
            //    timeToWait1 = diffMS1;
            //else
            //    timeToWait1 = 0;

            //diffMS2 = (int)(dTimeStamp1 - dLastTimeStamp1);
            //if ((dLastTimeStamp1 != 0) && (dTimeStamp1 != 0))
            //    timeToWait2 = diffMS1;
            //else
            //    timeToWait2 = 0;

            //// Wait the right number of MS if needed from last time data grabbed
            //diffTime = Environment.TickCount - lastTimeStampTime;
            //if ((timeToWait - diffTime) > 0)
            //{
            //    Thread.Sleep(timeToWait - diffTime);
            //    timeToWait = 0;
            //}

            #endregion

            if ((dTimeStamp1 != -1) && (dLastTimeStamp1 != -1) && (dTimeStamp1 < dLastTimeStamp1))
                Console.WriteLine("Jumpback1: " + debugCount);
            if ((dTimeStamp2 != -1) && (dLastTimeStamp2 != -1) && (dTimeStamp2 < dLastTimeStamp2))
                Console.WriteLine("Jumpback2: " + debugCount); 

            dLastTimeStamp1 = dTimeStamp1;
            dLastTimeStamp2 = dTimeStamp2;
            lastTimeStampTime1 = Environment.TickCount;
            lastTimeStampTime2 = Environment.TickCount;

            //DateTime junkme = new DateTime();
            //UnixTime.GetDateTime((long) dTimeStamp1, out junkme);
            //Console.WriteLine("                               DTIMESTAMP1: " + junkme);

            //UnixTime.GetDateTime((long)dTimeStamp2, out junkme);
            //Console.WriteLine("                               DTIMESTAMP2: " + junkme);


            // Read packet that is first in time from whichever file. Leave other be  

            ByteReader brTemp;

            brTemp = br1; 

            if ((!isEndFile1) && (!isEndFile2)) // both active 
            {
                if ((dTimeStamp1 <= dTimeStamp2) && (dTimeStamp1 != 0))
                {
                    lastGoodTime1 = dTimeStamp1;
                        brTemp = br1;
                    fileUsed = 1; 
                }
                else if (dTimeStamp2 != 0)
                {
                    lastGoodTime2 = dTimeStamp2;
                    brTemp = br2;
                    fileUsed = 2; 
                }
                else
                {
                    Console.WriteLine("ERROR1 -- Should not be here!! ----------------------------");
                }
            }
            else
            {
                if ((dTimeStamp1 != 0) && (!isEndFile1))
                {
                    lastGoodTime1 = dTimeStamp1;
                                        brTemp = br1;
                    fileUsed = 1; 
                }
                else if ((dTimeStamp2 != 0) && (!isEndFile2))
                {
                    lastGoodTime2 = dTimeStamp2;
                    brTemp = br2;
                    fileUsed = 2; 
                }
                else
                {
                    Console.WriteLine("ERROR2 -- Should not be here!! ----------------------------");
                }
            }

            brTemp.ReadByte(tempByte);
            aMITesDecoder.packet[0] = tempByte[0];
            brTemp.ReadByte(tempByte);
            aMITesDecoder.packet[1] = tempByte[0];
            brTemp.ReadByte(tempByte);
            aMITesDecoder.packet[2] = tempByte[0];
            brTemp.ReadByte(tempByte);
            aMITesDecoder.packet[3] = tempByte[0];
            brTemp.ReadByte(tempByte);
            aMITesDecoder.packet[4] = tempByte[0];
            aMITesDecoder.DecodeLastPacket(someData[dataIndex], false); // Don't swap bytes

            //Console.WriteLine("FileUsed: " + fileUsed);

            if (fileUsed == 1)
            {
                someData[dataIndex].timeStamp = UnixTime.IntTimeFromUnixTime(dTimeStamp1,aRefDate);
                aMITesDecoder.SetUnixTime(someData[dataIndex], dTimeStamp1); // Set the time
                someData[dataIndex].fileID = 1; 
                dTimeStamp1 = 0; // Reset so gets read next time from file 
            }
            else if (fileUsed == 2)
            {
                someData[dataIndex].timeStamp = UnixTime.IntTimeFromUnixTime(dTimeStamp2, aRefDate);
                aMITesDecoder.SetUnixTime(someData[dataIndex], dTimeStamp2); // Set the time
                dTimeStamp2 = 0; // Reset so gets read next time from file 
                someData[dataIndex].fileID = 2;
            }
            else
            {
                Console.WriteLine("ERROR: no file used");
            }

//            dataIndex++;
            return 1;
        }

        private bool isNewFiles = true; 
		/// <summary>
		/// Grab sensor data from the saved MITesLogger binary file and use a MITesDecoder to decode it. 
		/// </summary>
		/// <param name="numPackets">The number of MITes packets to get</param>
        public bool GetSensorData(int numPackets)
		{
		    someMITesDataIndex = 0;
		    int tmpIndex = DecodePLFormatMR(aMITesDecoder.someMITesData, someMITesDataIndex, br1, br2);
		    if (tmpIndex == 0)
		    {
                // Both files reached endpoints so go to next set of files for new hour
		        fileIndex += 1;
                if (fileIndex < someBinaryFiles.Count)
                {
                    isNewFiles = SetupNextFiles(fileIndex);
                    if (!isNewFiles)
                        return false;

                    dLastTimeStamp1 = lastGoodTime1;
                    dLastTimeStamp2 = lastGoodTime2;

                    tmpIndex = DecodePLFormatMR(aMITesDecoder.someMITesData, someMITesDataIndex, br1, br2);
                    if (tmpIndex == 0)
                    {
                        Console.WriteLine("ERROR: Should not have gotten this twice!!!  ---------------");
                        return false; 
                    }
                    else
                    {
                        aMITesDecoder.someMITesDataIndex = tmpIndex;
                        return true;
                    }
                }
                else
                {
                    aMITesDecoder.someMITesDataIndex = 0; // End of all data
                    return false;                     
                }
		    }
		    else
		    {
		        aMITesDecoder.someMITesDataIndex = tmpIndex;
		        return true; 
		    }
        }

	    private void ShutdownFiles()
		{
            if (br1 != null)
	    		br1.CloseFile();
            if (br2 != null)
                br2.CloseFile();
        }
	}
}