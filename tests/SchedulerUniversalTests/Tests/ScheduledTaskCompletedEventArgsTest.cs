#region Copyright
/*******************************************************************************
 * <copyright file="ScheduledTaskCompletedEventArgsTest.cs" owner="Daniel Kopp">
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
 * <file name="ScheduledTaskCompletedEventArgsTest.cs" date="2016-02-19">
 * Contains test methods to test the
 * NerdyDuck.Scheduler.ScheduledTaskCompletedEventArgs class.
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

namespace NerdyDuck.Tests.Scheduler
{
	/// <summary>
	/// Contains test methods to test the NerdyDuck.Scheduler.ScheduledTaskCompletedEventArgs class.
	/// </summary>
#if WINDOWS_DESKTOP
	[ExcludeFromCodeCoverage]
#endif
	[TestClass]
	public class ScheduledTaskCompletedEventArgsTest
	{
		[TestMethod]
		public void Ctor_Success()
		{
			DateTimeOffset now = new DateTimeOffset(2013, 6, 1, 12, 0, 0, 0, TimeSpan.Zero);
			DateTimeOffset now2 = new DateTimeOffset(2013, 6, 1, 12, 0, 1, 0, TimeSpan.Zero);
			ScheduledTask<FailableScheduledAction> task = new ScheduledTask<FailableScheduledAction>(Schedule.CreateOneTime(now), new FailableScheduledAction(FailableScheduledAction.DummyAction.Run), true, now, now2);
			InvalidOperationException ex = new InvalidOperationException();
			object state = new object();

			ScheduledTaskCompletedEventArgs<FailableScheduledAction> e = new ScheduledTaskCompletedEventArgs<FailableScheduledAction>(task, true, true, ex, state);

			Assert.AreSame(task, e.Task);
			Assert.IsTrue(e.IsCanceled);
			Assert.IsTrue(e.IsSuccess);
			Assert.AreSame(ex, e.Exception);
			Assert.AreSame(state, e.State);
		}

		[TestMethod]
		public void Ctor_TaskNull_Error()
		{
			ScheduledTask<FailableScheduledAction> task = null;
			InvalidOperationException ex = new InvalidOperationException();
			object state = new object();

			CustomAssert.ThrowsException<CodedArgumentNullException>(() =>
			{
				ScheduledTaskCompletedEventArgs<FailableScheduledAction> e = new ScheduledTaskCompletedEventArgs<FailableScheduledAction>(task, true, true, ex, state);
			});
		}
	}
}
