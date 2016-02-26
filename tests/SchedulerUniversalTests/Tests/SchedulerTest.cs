#region Copyright
/*******************************************************************************
 * <copyright file="SchedulerTest.cs" owner="Daniel Kopp">
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
 * <file name="SchedulerTest.cs" date="2016-02-19">
 * Contains test methods to test the
 * NerdyDuck.Scheduler.Scheduler class.
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
using System.Xml.Serialization;
using System.Xml;
using System.Diagnostics;
using System.Threading;

namespace NerdyDuck.Tests.Scheduler
{
	/// <summary>
	/// Contains test methods to test the NerdyDuck.Scheduler.Scheduler class.
	/// </summary>
#if WINDOWS_DESKTOP
	[ExcludeFromCodeCoverage]
#endif
	[TestClass]
	public class SchedulerTest
	{
		[TestMethod]
		public void Ctor_TimeSpan_Success()
		{
			Scheduler<BlockingScheduledAction> scheduler = new Scheduler<BlockingScheduledAction>(TimeSpan.FromSeconds(0.1));
			BlockingScheduledAction action = new BlockingScheduledAction();
			scheduler.Tasks.Add(new ScheduledTask<BlockingScheduledAction>(Schedule.CreateOneTime(DateTimeOffset.Now.AddSeconds(1.0)), action, true, null, null));
			scheduler.Start();
			Assert.IsTrue(scheduler.IsEnabled);

			Stopwatch watch = Stopwatch.StartNew();
			action.Waiter.WaitOne();
			Assert.IsTrue(watch.ElapsedMilliseconds < 2000);
			scheduler.Dispose();
		}

		[TestMethod]
		public void MultipleIterations_Success()
		{
			Scheduler<BlockingScheduledAction> scheduler = new Scheduler<BlockingScheduledAction>(TimeSpan.FromSeconds(0.1));
			BlockingScheduledAction action1 = new BlockingScheduledAction(10);
			scheduler.Tasks.Add(new ScheduledTask<BlockingScheduledAction>(Schedule.CreateInterval(DateTimeOffset.Now, TimeSpan.FromMilliseconds(100.0), Weekdays.All), action1, true, null, null));
			BlockingScheduledAction action2 = new BlockingScheduledAction(5);
			scheduler.Tasks.Add(new ScheduledTask<BlockingScheduledAction>(Schedule.CreateInterval(DateTimeOffset.Now, TimeSpan.FromMilliseconds(200.0), Weekdays.All), action2, true, null, null));
			scheduler.Start();

			Stopwatch watch = Stopwatch.StartNew();
			ManualResetEvent.WaitAny(new WaitHandle[] { action1.Waiter, action2.Waiter });
			Assert.IsTrue(watch.ElapsedMilliseconds < 1200);
			scheduler.Dispose();
		}

		[TestMethod]
		public void CancelOperations_Success()
		{
			Scheduler<CancellableScheduledAction> scheduler = new Scheduler<CancellableScheduledAction>(TimeSpan.FromSeconds(0.1));
			CancellableScheduledAction action1 = new CancellableScheduledAction();
			scheduler.Tasks.Add(new ScheduledTask<CancellableScheduledAction>(Schedule.CreateInterval(DateTimeOffset.Now, TimeSpan.FromMilliseconds(100.0), Weekdays.All), action1, true, null, null));
			CancellableScheduledAction action2 = new CancellableScheduledAction();
			scheduler.Tasks.Add(new ScheduledTask<CancellableScheduledAction>(Schedule.CreateInterval(DateTimeOffset.Now, TimeSpan.FromMilliseconds(200.0), Weekdays.All), action2, true, null, null));
			scheduler.Start();

			ManualResetEvent.WaitAny(new WaitHandle[] { action1.Waiter, action2.Waiter });
			Stopwatch watch = Stopwatch.StartNew();
			scheduler.Stop();
			Assert.IsTrue(watch.ElapsedMilliseconds < 200);
			scheduler.Dispose();
		}

		[TestMethod]
		public void Stop_TimeoutNegative_Error()
		{
			Scheduler<CancellableScheduledAction> scheduler = new Scheduler<CancellableScheduledAction>(TimeSpan.FromSeconds(0.1));
			scheduler.Stop();
			scheduler.Start();

			CustomAssert.ThrowsException<CodedArgumentOutOfRangeException>(() =>
			{
				scheduler.Stop(TimeSpan.FromSeconds(-1.0));
			});
			scheduler.Dispose();
		}

		[TestMethod]
		public void Stop_TimeoutExpired_Error()
		{
			Scheduler<WaitingScheduledAction> scheduler = new Scheduler<WaitingScheduledAction>(TimeSpan.FromSeconds(0.1));
			WaitingScheduledAction action1 = new WaitingScheduledAction();
			scheduler.Tasks.Add(new ScheduledTask<WaitingScheduledAction>(Schedule.CreateOneTime(DateTimeOffset.Now), action1, true, null, null));
			scheduler.Start();

			action1.StartWaiter.WaitOne();
			CustomAssert.ThrowsException<CodedTimeoutException>(() =>
			{
				scheduler.Stop(TimeSpan.FromSeconds(0.5));
			});
			action1.Waiter.Set();
			scheduler.Dispose();
		}

		[TestMethod]
		public void Interval_Success()
		{
			Scheduler<BlockingScheduledAction> scheduler = new Scheduler<BlockingScheduledAction>(TimeSpan.FromSeconds(1000.0));
			BlockingScheduledAction action1 = new BlockingScheduledAction(5);
			scheduler.Tasks.Add(new ScheduledTask<BlockingScheduledAction>(Schedule.CreateInterval(DateTimeOffset.Now, TimeSpan.FromMilliseconds(200.0), Weekdays.All), action1, true, null, null));
			scheduler.Start();

			Assert.AreEqual(1000.0, scheduler.Interval.TotalSeconds);
			scheduler.Interval = TimeSpan.FromSeconds(0.1);

			Stopwatch watch = Stopwatch.StartNew();
			action1.Waiter.WaitOne();
			Assert.IsTrue(watch.ElapsedMilliseconds < 1200);
			scheduler.Dispose();
		}

		[TestMethod]
		public void TaskStarting_TaskCompleted_Success()
		{
			bool TaskStartingExecuted = false;
			bool TaskCompletedExecuted = false;
			Scheduler<BlockingScheduledAction> scheduler = new Scheduler<BlockingScheduledAction>(TimeSpan.FromSeconds(0.1));
			scheduler.TaskCompleted += (sender, e) =>
			{
				TaskCompletedExecuted = true;
			};
			scheduler.TaskStarting += (sender, e) =>
			{
				TaskStartingExecuted = true;
			};
			BlockingScheduledAction action1 = new BlockingScheduledAction();
			scheduler.Tasks.Add(new ScheduledTask<BlockingScheduledAction>(Schedule.CreateOneTime(DateTimeOffset.Now), action1, true, null, null));
			scheduler.Start();

			action1.Waiter.WaitOne();
			scheduler.Stop();

			Assert.IsTrue(TaskCompletedExecuted, nameof(TaskCompletedExecuted));
			Assert.IsTrue(TaskStartingExecuted, nameof(TaskStartingExecuted));

			scheduler.Dispose();
		}

		[TestMethod]
		public void TaskStarting_Canceled_Success()
		{
			bool TaskStartingExecuted = false;
			Scheduler<BlockingScheduledAction> scheduler = new Scheduler<BlockingScheduledAction>(TimeSpan.FromSeconds(0.1));
			scheduler.TaskStarting += (sender, e) =>
			{
				e.Cancel = true;
				TaskStartingExecuted = true;
			};
			BlockingScheduledAction action1 = new BlockingScheduledAction();
			ScheduledTask<BlockingScheduledAction> task = new ScheduledTask<BlockingScheduledAction>(Schedule.CreateOneTime(DateTimeOffset.Now), action1, true, null, null);
			scheduler.Tasks.Add(task);
			scheduler.Start();

			SpinWait.SpinUntil(() => { return TaskStartingExecuted; });
			scheduler.Stop();

			Assert.IsNull(task.LastEndTime);

			scheduler.Dispose();
		}


		[TestMethod]
		public void Ctor_TimeSpanNegative_Error()
		{
			CustomAssert.ThrowsException<CodedArgumentOutOfRangeException>(() =>
			{
				Scheduler<BlockingScheduledAction> scheduler = new Scheduler<BlockingScheduledAction>(TimeSpan.FromSeconds(-1.0));
			});
		}

		[TestMethod]
		public void Disposed_Error()
		{
			Scheduler<BlockingScheduledAction> scheduler = new Scheduler<BlockingScheduledAction>();
			scheduler.Tasks.Add(new ScheduledTask<BlockingScheduledAction>(Schedule.CreateOneTime(DateTimeOffset.Now), new BlockingScheduledAction(), true, null, null));
			scheduler.Start();
			scheduler.Start();

			scheduler.Dispose();
			scheduler.Dispose();
			scheduler.Stop();

			CustomAssert.ThrowsException<ObjectDisposedException>(() =>
			{
				int i = scheduler.Tasks.Count;
			});
		}
	}
}
