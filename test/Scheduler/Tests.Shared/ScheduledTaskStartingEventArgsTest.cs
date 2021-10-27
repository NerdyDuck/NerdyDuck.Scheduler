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

namespace NerdyDuck.Tests.Scheduler
{
	/// <summary>
	/// Contains test methods to test the NerdyDuck.Scheduler.ScheduledTaskStartingEventArgs class.
	/// </summary>
#if WINDOWS_DESKTOP
	[ExcludeFromCodeCoverage]
#endif
	[TestClass]
	public class ScheduledTaskStartingEventArgsTest
	{
		[TestMethod]
		public void Ctor_Success()
		{
			DateTimeOffset now = new DateTimeOffset(2013, 6, 1, 12, 0, 0, 0, TimeSpan.Zero);
			DateTimeOffset now2 = new DateTimeOffset(2013, 6, 1, 12, 0, 1, 0, TimeSpan.Zero);
			ScheduledTask<FailableScheduledAction> task = new ScheduledTask<FailableScheduledAction>(Schedule.CreateOneTime(now), new FailableScheduledAction(FailableScheduledAction.DummyAction.Run), true, now, now2);

			ScheduledTaskStartingEventArgs<FailableScheduledAction> e = new ScheduledTaskStartingEventArgs<FailableScheduledAction>(task);

			Assert.AreSame(task, e.Task);
			Assert.IsFalse(e.Cancel);
			e.Cancel = true;
			Assert.IsTrue(e.Cancel);
		}

		[TestMethod]
		public void Ctor_TaskNull_Error()
		{
			ScheduledTask<FailableScheduledAction> task = null;

			Assert.ThrowsException<CodedArgumentNullException>(() =>
			{
				ScheduledTaskStartingEventArgs<FailableScheduledAction> e = new ScheduledTaskStartingEventArgs<FailableScheduledAction>(task);
			});
		}
	}
}
