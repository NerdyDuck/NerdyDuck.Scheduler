#region Copyright
/*******************************************************************************
 * <copyright file="ScheduledTaskStartingEventArgsTest.cs" owner="Daniel Kopp">
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
 * <file name="ScheduledTaskStartingEventArgsTest.cs" date="2016-02-19">
 * Contains test methods to test the
 * NerdyDuck.Scheduler.ScheduledTaskStartingEventArgs class.
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

			CustomAssert.ThrowsException<CodedArgumentNullException>(() =>
			{
				ScheduledTaskStartingEventArgs<FailableScheduledAction> e = new ScheduledTaskStartingEventArgs<FailableScheduledAction>(task);
			});
		}
	}
}
