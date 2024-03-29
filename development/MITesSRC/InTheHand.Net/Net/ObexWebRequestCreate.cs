// 32feet.NET - Personal Area Networking for .NET
//
// InTheHand.Net.ObexWebRequestCreate
// 
// Copyright (c) 2003-2006 In The Hand Ltd, All rights reserved.
// This source code is licensed under the In The Hand Community License - see License.txt

using System;
using System.Net;


namespace InTheHand.Net
{
	/// <summary>
	/// Used to create a new web request for obex uri scheme
	/// </summary>
	internal class ObexWebRequestCreate : IWebRequestCreate
	{
		#region IWebRequestCreate Members

		public WebRequest Create(Uri uri)
		{
			return new ObexWebRequest(uri);
		}

		#endregion
	}
}
