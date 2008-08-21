// 32feet.NET - Personal Area Networking for .NET
//
// InTheHand.Net.Bluetooth.BTHNS_BLOB
// 
// Copyright (c) 2003-2006 In The Hand Ltd, All rights reserved.
// This source code is licensed under the In The Hand Community License - see License.txt

using System;

namespace InTheHand.Net.Bluetooth
{
	/// <summary>
	/// 
	/// </summary>
	internal abstract class BTHNS_BLOB : IDisposable
	{
		internal byte[] m_data;

		/// <summary>
		/// Size of the structure.
		/// </summary>
		internal int Length
		{
			get
			{
				return m_data.Length;
			}
		}

		/// <summary>
		/// Internal bytes
		/// </summary>
		/// <returns></returns>
		internal byte[] ToByteArray()
		{
			return m_data;
		}

		#region IDisposable Members

		protected virtual void Dispose(bool disposing)
		{
			if(disposing)
			{
				m_data = null;
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~BTHNS_BLOB()
		{
			Dispose(false);
		}
		#endregion
	}
}
