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

namespace NerdyDuck.Tests.Scheduler
{
	/// <summary>
	/// Contains test methods to test the NerdyDuck.Scheduler.CancellationTokenSchedulerAction class.
	/// </summary>
	[ExcludeFromCodeCoverage]
	[TestClass]
	public class CancellationTokenSchedulerActionTest
	{
		[TestMethod]
		public void ThrowIfCancellationRequested_Success()
		{
			Scheduler<WaitingScheduledAction>.CancellationTokenSchedulerAction action = new Scheduler<WaitingScheduledAction>.CancellationTokenSchedulerAction();
			action.ThrowIfCancellationRequested();
		}

		[TestMethod]
		public void ThrowIfCancellationRequested_Error()
		{
			Scheduler<WaitingScheduledAction>.CancellationTokenSchedulerAction action = new Scheduler<WaitingScheduledAction>.CancellationTokenSchedulerAction();
			action.Cancel();

			Assert.ThrowsException<OperationCanceledException>(() =>
			{
				action.ThrowIfCancellationRequested();
			});
		}

		[TestMethod]
		public void Dispose_Error()
		{
			Scheduler<WaitingScheduledAction>.CancellationTokenSchedulerAction action = new Scheduler<WaitingScheduledAction>.CancellationTokenSchedulerAction();
			action.Dispose();
			action.Dispose();

			Assert.ThrowsException<ObjectDisposedException>(() =>
			{
				action.ThrowIfCancellationRequested();
			});
		}
	}
}
