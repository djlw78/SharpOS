// 
// (C) 2006-2007 The SharpOS Project Team (http://www.sharpos.org)
//
// Authors:
//	William Lahti <xfurious@gmail.com>
//
// Licensed under the terms of the GNU GPL License version 2.
//

namespace SharpOS
{
	public enum KernelError: uint {
		Unknown = 0,
		
		Success,
		MultibootError,

		/// <summary>
		/// Scheduler Queue is empty and this was not expected
		/// </summary>
		SchedulerQueueEmpty
	}
}