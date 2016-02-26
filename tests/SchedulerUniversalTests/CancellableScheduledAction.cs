#region Copyright
/*******************************************************************************
 * <copyright file="CancellableScheduledAction.cs" owner="Daniel Kopp">
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
 * <file name="CancellableScheduledAction.cs" date="2016-02-19">
 * Implementation of ScheduledActionBase that throws an
 * OperationCancelledException when an execution is canceled.
 * </file>
 ******************************************************************************/
#endregion

using NerdyDuck.Scheduler;
using System;
using System.Threading;
using System.Diagnostics;
#if WINDOWS_DESKTOP
using System.Diagnostics.CodeAnalysis;
#endif


namespace NerdyDuck.Tests.Scheduler
{
	/// <summary>
	/// Implementation of ScheduledActionBase that throws an OperationCancelledException when an execution is canceled.
	/// </summary>
#if WINDOWS_DESKTOP
	[ExcludeFromCodeCoverage]
#endif
	public class CancellableScheduledAction : ScheduledActionBase
	{
		private ManualResetEvent mWaiter;

		/// <summary>
		/// Gets a wait handle to wait for the Run method to start.
		/// </summary>
		public ManualResetEvent Waiter
		{
			get { return mWaiter; }
		}

		/// <summary>
		/// Initializes a new instance of the CancellableScheduledAction class.
		/// </summary>
		public CancellableScheduledAction()
		{
			mWaiter = new ManualResetEvent(false);
		}

		/// <summary>
		/// Sets Waiter, then waits until the operation is canceled using the ISchedulerAction.
		/// </summary>
		/// <param name="lastExecuted"></param>
		/// <param name="operation"></param>
		/// <returns></returns>
		public override object Run(DateTimeOffset? lastExecuted, ISchedulerAction operation)
		{
			mWaiter.Set();

			SpinWait.SpinUntil(() => { return operation.IsCancellationRequested; });
			throw new OperationCanceledException();
		}
	}
}

