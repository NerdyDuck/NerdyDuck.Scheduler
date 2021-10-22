#region Copyright
/*******************************************************************************
 * <copyright file="CancellationTokenSchedulerActionTest.cs" owner="Daniel Kopp">
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
 * <file name="CancellationTokenSchedulerActionTest.cs" date="2016-02-19">
 * Contains test methods to test the
 * NerdyDuck.Scheduler.CancellationTokenSchedulerAction class.
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
using System.Xml;

namespace NerdyDuck.Tests.Scheduler
{
	/// <summary>
	/// Contains test methods to test the NerdyDuck.Scheduler.CancellationTokenSchedulerAction class.
	/// </summary>
#if WINDOWS_DESKTOP
	[ExcludeFromCodeCoverage]
#endif
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

			CustomAssert.ThrowsException<OperationCanceledException>(() =>
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

			CustomAssert.ThrowsException<ObjectDisposedException>(() =>
			{
				action.ThrowIfCancellationRequested();
			});
		}
	}
}
