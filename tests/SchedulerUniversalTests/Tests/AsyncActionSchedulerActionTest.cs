#region Copyright
/*******************************************************************************
 * <copyright file="AsyncActionSchedulerActionTest.cs" owner="Daniel Kopp">
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
 * <file name="AsyncActionSchedulerActionTest.cs" date="2016-02-19">
 * Contains test methods to test the
 * NerdyDuck.Scheduler.AsyncActionSchedulerAction class.
 * </file>
 ******************************************************************************/
#endregion

#if WINDOWS_UWP
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#endif
#if WINDOWS_DESKTOP
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.CodeAnalysis;
#endif
using NerdyDuck.CodedExceptions;
using NerdyDuck.Scheduler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml;
using Windows.Foundation;
using Windows.System.Threading;
using Windows.System.Threading.Core;

namespace NerdyDuck.Tests.Scheduler
{
	/// <summary>
	/// Contains test methods to test the NerdyDuck.Scheduler.AsyncActionSchedulerAction class.
	/// </summary>
#if WINDOWS_DESKTOP
	[ExcludeFromCodeCoverage]
#endif
	[TestClass]
	public class AsyncActionSchedulerActionTest
	{
		[TestMethod]
		public async System.Threading.Tasks.Task ThrowIfCancellationRequested_Success()
		{
			ManualResetEventSlim Waiter = new ManualResetEventSlim(false);

			await ThreadPool.RunAsync(new WorkItemHandler((target) =>
			{
				Scheduler<WaitingScheduledAction>.AsyncActionSchedulerAction action = new Scheduler<WaitingScheduledAction>.AsyncActionSchedulerAction(target);
				action.ThrowIfCancellationRequested();
				Waiter.Set();
			}));

			Waiter.Wait();
		}

		[TestMethod]
		public void ThrowIfCancellationRequested_Error()
		{
			ManualResetEventSlim StartWaiter = new ManualResetEventSlim(false);
			ManualResetEventSlim Waiter = new ManualResetEventSlim(false);
			PreallocatedWorkItem workitem = new PreallocatedWorkItem(new WorkItemHandler((target) =>
			{
				Scheduler<WaitingScheduledAction>.AsyncActionSchedulerAction action = new Scheduler<WaitingScheduledAction>.AsyncActionSchedulerAction(target);
				StartWaiter.Wait();

				CustomAssert.ThrowsException<OperationCanceledException>(() =>
				{
					action.ThrowIfCancellationRequested();
				});
				Waiter.Set();
			}));

			IAsyncAction iaa = workitem.RunAsync();
			iaa.Cancel();
			StartWaiter.Set();
			Waiter.Wait();
		}
	}
}
