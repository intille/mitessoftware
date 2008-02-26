using System;

namespace HousenCS.MITes
{
	/// <summary>
	/// Summary description for UnixTime.
	/// </summary>
	public class UnixTime
	{
		/// <summary>
		/// 
		/// </summary>
		public static DateTime UnixRef = new DateTime(1970,1,1,0,0,0,0).ToUniversalTime();

        /// <summary>
		/// 
		/// </summary>
		public static DateTime UnixRefNoUTC = new DateTime(1970,1,1,0,0,0,0);

		/// <summary>
		/// 
		/// </summary>
		public static double MilliInDay = 86400000;
		/// <summary>
		/// 
		/// </summary>
		public static double MilliInHour = 3600000;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="unixTime"></param>
		/// <param name="dt"></param>
		public static void GetDateTime(long unixTime, out DateTime dt)
		{
			dt = UnixRefNoUTC.AddMilliseconds(unixTime);			
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="unixTime"></param>
        /// <param name="aDate"></param>
        /// <returns></returns>
        public static int IntTimeFromUnixTime(double unixTime, DateTime aDate)
        {
            double tmp = unixTime - GetUnixTime(aDate); 
            if (tmp > Int32.MaxValue)
                Console.WriteLine("ERROR IN conversion UnixTime to int time");
            return ((int) tmp); 
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dt"></param>
		/// <returns></returns>
		public static double GetUnixTime(DateTime dt)
		{
			TimeSpan ts = dt.ToUniversalTime().Subtract(UnixRef);
			return  ts.TotalMilliseconds;
		
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static double GetUnixTime()
		{
			return ((TimeSpan)(DateTime.UtcNow.Subtract(UnixRef))).TotalMilliseconds;
		
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static TimeSpan GetUnixTimeSpan()
		{
			return ((TimeSpan)(DateTime.UtcNow.Subtract(UnixRef)));

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="unixTime"></param>
		/// <returns></returns>
		public static int GetUnixTimeSecond(double unixTime)
		{
			try
			{
				return Convert.ToInt32(Math.Floor(unixTime/1000));
			}
			catch(Exception e)
			{
                e.ToString();
				//Console.Out.WriteLine("Error: UnixTime: GetUnixTimeSec: "+e.ToString());
				return 0;
			}
			
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="unixTime"></param>
		/// <returns></returns>
		public static short GetUnixTimeMSecond(double unixTime)
		{
			try
			{
				return Convert.ToInt16(Math.Round(unixTime%1000));
			}
			catch(Exception e)
			{
                e.ToString();
				//Console.Out.WriteLine("Error: UnixTime: GetUnixTimeSec: "+e.ToString());
				return 0;
			}	
		
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="unixTime"></param>
		/// <param name="retBytes"></param>
		public static void GetUnixTimeBytesOld(double unixTime, byte[] retBytes)
		{
//			ushort ms = UnixTime.GetUnixTimeMSecond(unixTime);
//			uint sec = UnixTime.GetUnixTimeSecond(unixTime);
//			byte[] temp;
//			temp = System.BitConverter.GetBytes(sec);
//	
//			retBytes[5]=temp[0];
//			retBytes[4]=temp[1];
//			retBytes[3]=temp[2];
//			retBytes[2]=temp[3];
//
//			temp = System.BitConverter.GetBytes(ms);
//				
//			retBytes[1] = temp[0];
//			retBytes[0] = temp[1];			
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="unixTime"></param>
		/// <param name="retBytes"></param>
		public static void GetUnixTimeBytes(double unixTime, byte[] retBytes)
		{
			if (BitConverter.IsLittleEndian == false)
				Console.WriteLine ("ERROR: assumes littleendian!");

			short ms = UnixTime.GetUnixTimeMSecond(unixTime);
			int sec = UnixTime.GetUnixTimeSecond(unixTime);
			byte[] temp;
			temp = System.BitConverter.GetBytes(sec);
	
			retBytes[0]=temp[0];
			retBytes[1]=temp[1];
			retBytes[2]=temp[2];
			retBytes[3]=temp[3];

			temp = System.BitConverter.GetBytes(ms);
				
			retBytes[4] = temp[0];
			retBytes[5] = temp[1];			
		}

		private static byte[] temp2 = new byte[2];
		private static byte[] temp4 = new byte[4];
		

		/// <summary>
		/// 
		/// </summary>
		/// <param name="someBytes"></param>
		/// <returns>A timecode (based on UNIX time) in MS</returns>
		public static double DecodeUnixTimeCodeBytes(byte[] someBytes)
		{
			int sec = DecodeUnixTimeSecBytes(someBytes);
			short ms = DecodeUnixTimeMSBytes(someBytes);

			double ds = (double) sec;
			double dms = (double) ms;

			return ((ds*1000) + dms); 
		}

        /// <summary>
        /// Works SSI 
        /// </summary>
        /// <param name="someBytes"></param>
        /// <returns>A timecode (based on UNIX time) in MS</returns>
        public static double DecodeUnixTimeCodeBytesFixed(byte[] someBytes)
        {
            int sec = DecodeUnixTimeSecBytes(someBytes);
            int ms = (int)DecodeUnixTimeMSBytesLong(someBytes);

            double ds = (double)sec;
            double dms = (double)ms;

            return ((ds * 1000) + dms);
        }
        
        /// <summary>
		/// 
		/// </summary>
		/// <param name="someBytes"></param>
		/// <returns></returns>
		public static double DecodeUnixTimeCodeBytesOld(byte[] someBytes)
		{
//			uint sec = DecodeUnixTimeSecBytes(someBytes);
//			ushort ms = DecodeUnixTimeMSBytes(someBytes);
//
//			double ds = (double) sec;
//			double dms = (double) ms;
//
//			return ((ds*1000) + dms); 
			return 0;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="someBytes"></param>
		/// <returns></returns>
		public static int DecodeUnixTimeSecBytes(byte[] someBytes)
		{
			temp4[0] = someBytes[0];
			temp4[1] = someBytes[1];
			temp4[2] = someBytes[2];
			temp4[3] = someBytes[3];
			return System.BitConverter.ToInt32(temp4,0);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="someBytes"></param>
		/// <returns></returns>
		public static short DecodeUnixTimeMSBytes(byte[] someBytes)
		{
			temp2[0] = someBytes[1]; 
			temp2[1] = someBytes[0];
			return System.BitConverter.ToInt16(temp2,0);
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="someBytes"></param>
        /// <returns></returns>
        public static long DecodeUnixTimeMSBytesLong(byte[] someBytes)
        {
            temp4[0] = someBytes[4];
            temp4[1] = someBytes[5];
            temp4[2] = 0;
            temp4[3] = 0;
            return System.BitConverter.ToInt32(temp4, 0);
        }

		/// <summary>
		/// Returns bytes in format seconds[5-2][LSB-MSB] milliseconds[1-0][LSB-MSB]
		/// </summary>
		/// <param name="unixTime"></param>
		/// <returns></returns>
		public static byte[] GetUnixTimeBytes(double unixTime)
		{
			byte[] ret = new byte[6];
			GetUnixTimeBytes(unixTime, ret);
			return ret;
		}
	}
}
