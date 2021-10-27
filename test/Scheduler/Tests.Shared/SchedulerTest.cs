#region Copyright
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

using System.Diagnostics;
using System.Threading;

namespace NerdyDuck.Tests.Scheduler
{
	/// <summary>
	/// Contains test methods to test the NerdyDuck.Scheduler.Scheduler class.
	/// </summary>
	[ExcludeFromCodeCoverage]
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

			Assert.ThrowsException<CodedArgumentOutOfRangeException>(() =>
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
			Assert.ThrowsException<CodedTimeoutException>(() =>
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
			Assert.ThrowsException<CodedArgumentOutOfRangeException>(() =>
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

			Assert.ThrowsException<ObjectDisposedException>(() =>
			{
				int i = scheduler.Tasks.Count;
			});
		}
	}
}
