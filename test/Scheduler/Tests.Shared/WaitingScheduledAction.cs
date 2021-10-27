﻿#region Copyright
/*******************************************************************************
 * NerdyDuck.Tests.Scheduler - Unit tests for the
 * NerdyDuck.Scheduler assembly
 * 
 * The MIT License (MIT)
 *
 * Copyright (c) Daniel Kopp, dak@nerdyduck.de
 *
 * All rights reserved.
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 ******************************************************************************/
#endregion

using System.Threading;

namespace NerdyDuck.Tests.Scheduler
{
	/// <summary>
	/// Implementation of ScheduledActionBase that waits until a ManualResetEvent is set.
	/// </summary>
	[ExcludeFromCodeCoverage]
	public class WaitingScheduledAction : ScheduledActionBase
	{
		private ManualResetEvent mStartWaiter;
		private ManualResetEvent mWaiter;

		/// <summary>
		/// Gets a WaitHandle that blocks until Run is executing
		/// </summary>
		public ManualResetEvent StartWaiter
		{
			get { return mStartWaiter; }
		}

		/// <summary>
		/// Gets a WaitHandle that can be set to stop running Run
		/// </summary>
		public ManualResetEvent Waiter
		{
			get { return mWaiter; }
		}

		/// <summary>
		/// Initializes a new instance of the BlockingScheduledAction class.
		/// </summary>
		public WaitingScheduledAction()
		{
			mWaiter = new ManualResetEvent(false);
			mStartWaiter = new ManualResetEvent(false);
		}

		/// <summary>
		/// Increments a counter, and sets Waiter when the counter is equal to Iterations.
		/// </summary>
		/// <param name="lastExecuted"></param>
		/// <param name="operation"></param>
		/// <returns></returns>
		public override object Run(DateTimeOffset? lastExecuted, ISchedulerAction operation)
		{
			mStartWaiter.Set();
			mWaiter.WaitOne();
			return null;
		}
	}
}
