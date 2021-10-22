#region Copyright
/*******************************************************************************
 * <copyright file="BlockingScheduledAction.cs" owner="Daniel Kopp">
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
 * <file name="BlockingScheduledAction.cs" date="2016-02-19">
 * Implementation of ScheduledActionBase that signals a ManualResetEvent
 * when executed.
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
	/// Implementation of ScheduledActionBase that signals a ManualResetEvent when executed.
	/// </summary>
#if WINDOWS_DESKTOP
	[ExcludeFromCodeCoverage]
#endif
	public class BlockingScheduledAction : ScheduledActionBase
	{
		private ManualResetEvent mWaiter;
		private int mIterations;
		private int IterationsExecuted;

		/// <summary>
		/// Gets a WaitHandle that blocks until Run was called a number of times equal to Iterations.
		/// </summary>
		public ManualResetEvent Waiter
		{
			get { return mWaiter; }
		}

		/// <summary>
		/// Gets the number of times to call Run until Waiter unblocks.
		/// </summary>
		public int Iterations
		{
			get { return mIterations; }
		}

		/// <summary>
		/// Initializes a new instance of the BlockingScheduledAction class.
		/// </summary>
		public BlockingScheduledAction()
			: this(1)
		{
		}

		/// <summary>
		/// Initializes a new instance of the BlockingScheduledAction class with the number of iterations.
		/// </summary>
		/// <param name="iterations">The number of times to call Run until Waiter unblocks.</param>
		public BlockingScheduledAction(int iterations)
		{
			mWaiter = new ManualResetEvent(false);
			mIterations = iterations;
			IterationsExecuted = 0;
		}

		/// <summary>
		/// Increments a counter, and sets Waiter when the counter is equal to Iterations.
		/// </summary>
		/// <param name="lastExecuted"></param>
		/// <param name="operation"></param>
		/// <returns></returns>
		public override object Run(DateTimeOffset? lastExecuted, ISchedulerAction operation)
		{
			IterationsExecuted++;
			if (IterationsExecuted == mIterations)
			{
				mWaiter.Set();
			}
			return null;
		}
	}
}
