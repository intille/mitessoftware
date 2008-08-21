// 32feet.NET - Personal Area Networking for .NET
//
// InTheHand.Net.Bluetooth.BLUETOOTH_FIND_RADIO_PARAMS
// 
// Copyright (c) 2003-2006 In The Hand Ltd, All rights reserved.
// This source code is licensed under the In The Hand Community License - see License.txt

#if WinXP

using System;

namespace InTheHand.Net.Bluetooth
{
	// <summary>
	// The BLUETOOTH_FIND_RADIO_PARAMS structure facilitates enumerating installed Bluetooth radios.
	// </summary>
	internal struct BLUETOOTH_FIND_RADIO_PARAMS
	{
		public int dwSize;
	}
}

#endif