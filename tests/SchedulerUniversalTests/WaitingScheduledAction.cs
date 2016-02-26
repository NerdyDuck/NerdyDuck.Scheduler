#region Copyright
/*******************************************************************************
 * <copyright file="WaitingScheduledAction.cs" owner="Daniel Kopp">
 * Copyright 2015-2016 Daniel Kopp
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * </copyright>
 * <author name="Daniel Kopp" email="dak@nerdyduck.de" />
 * <assembly name="NerdyDuck.Tests.Scheduler">
 * Unit tests for NerdyDuck.Scheduler assembly.
 * </assembly>
 * <file name="WaitingScheduledAction.cs" date="2016-02-19">
 * Implementation of ScheduledActionBase that waits until a ManualResetEvent
 * is set.
 * </file>
 ******************************************************************************/
#endregion

using NerdyDuck.Scheduler;
using System;
using System.Threading;
#if WINDOWS_DESKTOP
using System.Diagnostics.CodeAnalysis;
#endif


namespace NerdyDuck.Tests.Scheduler
{
	/// <summary>
	/// Implementation of ScheduledActionBase that waits until a ManualResetEvent is set.
	/// </summary>
#if WINDOWS_DESKTOP
	[ExcludeFromCodeCoverage]
#endif
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
