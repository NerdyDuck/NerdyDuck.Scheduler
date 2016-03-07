#region Copyright
/*******************************************************************************
 * <copyright file="ScheduledTaskTest.cs" owner="Daniel Kopp">
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
 * <file name="ScheduledTaskTest.cs" date="2016-02-19">
 * Contains test methods to test the
 * NerdyDuck.Scheduler.ScheduledTask class.
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
using System.Xml.Serialization;

namespace NerdyDuck.Tests.Scheduler
{
	/// <summary>
	/// Contains test methods to test the NerdyDuck.Scheduler.ScheduledTask class.
	/// </summary>
#if WINDOWS_DESKTOP
	[ExcludeFromCodeCoverage]
#endif
	[TestClass]
	public class ScheduledTaskTest
	{
		[TestMethod]
		public void Ctor_Success()
		{
			DateTimeOffset now = new DateTimeOffset(2013, 6, 1, 12, 0, 0, 0, TimeSpan.Zero);
			DateTimeOffset now2 = new DateTimeOffset(2013, 6, 1, 12, 0, 1, 0, TimeSpan.Zero);
			ScheduledTask<FailableScheduledAction> task = new ScheduledTask<FailableScheduledAction>(Schedule.CreateOneTime(now), new FailableScheduledAction(FailableScheduledAction.DummyAction.Run), true, now, now2);

			Assert.IsNotNull(task.Schedule);
			Assert.IsNotNull(task.Action);
			Assert.IsTrue(task.IsEnabled);
			Assert.IsFalse(task.IsActive);
			Assert.IsFalse(task.IsTaskDue);
			Assert.AreEqual(now, task.LastStartTime);
			Assert.AreEqual(now2, task.LastEndTime);

		}

		[TestMethod]
		public void Ctor_Void_Error()
		{
			ScheduledTask<FailableScheduledAction> task = new ScheduledTask<FailableScheduledAction>();

			CustomAssert.ThrowsException<CodedInvalidOperationException>(() =>
			{
				task.Reschedule(null);
			});

		}

		[TestMethod]
		public void Ctor_TaskActionNull_Error()
		{
			DateTimeOffset now = new DateTimeOffset(2013, 6, 1, 12, 0, 0, 0, TimeSpan.Zero);
			DateTimeOffset now2 = new DateTimeOffset(2013, 6, 1, 12, 0, 1, 0, TimeSpan.Zero);

			CustomAssert.ThrowsException<CodedArgumentNullException>(() =>
			{
				ScheduledTask<FailableScheduledAction> task = new ScheduledTask<FailableScheduledAction>(Schedule.CreateOneTime(now), null, true, now, now2);
			});

			CustomAssert.ThrowsException<CodedArgumentNullException>(() =>
			{
				ScheduledTask<FailableScheduledAction> task = new ScheduledTask<FailableScheduledAction>(null, new FailableScheduledAction(FailableScheduledAction.DummyAction.Run), true, now, now2);
			});
		}

		[TestMethod]
		public void Reschedule_UpdateSchedule_Success()
		{
			DateTimeOffset now = new DateTimeOffset(2013, 6, 1, 12, 0, 0, 0, TimeSpan.Zero);
			DateTimeOffset now2 = new DateTimeOffset(2013, 6, 1, 12, 0, 1, 0, TimeSpan.Zero);
			ScheduledTask<FailableScheduledAction> task = new ScheduledTask<FailableScheduledAction>(Schedule.CreateOneTime(now), new FailableScheduledAction(FailableScheduledAction.DummyAction.Run), true, now, now2);

			Schedule NewSchedule = Schedule.CreateInterval(now, TimeSpan.FromHours(1.0), Weekdays.All);

			task.Reschedule(NewSchedule);
			Assert.AreSame(NewSchedule, task.Schedule);

			task.UpdateSchedule(DateTimeOffset.Now);
		}

		[TestMethod]
		public void Reschedule_UpdateScheduleOneTime_Success()
		{
			DateTimeOffset now = new DateTimeOffset(2013, 6, 1, 12, 0, 0, 0, TimeSpan.Zero);
			DateTimeOffset now2 = new DateTimeOffset(2013, 6, 1, 12, 0, 1, 0, TimeSpan.Zero);
			ScheduledTask<FailableScheduledAction> task = new ScheduledTask<FailableScheduledAction>(Schedule.CreateOneTime(now), new FailableScheduledAction(FailableScheduledAction.DummyAction.Run), true, now, now2);

			Schedule NewSchedule = Schedule.CreateOneTime(DateTimeOffset.Now.AddDays(1.0));

			task.Reschedule(NewSchedule);
			Assert.AreSame(NewSchedule, task.Schedule);

			task.UpdateSchedule(DateTimeOffset.Now);
			Assert.AreEqual(NewSchedule.ScheduledDateTime.Value, task.NextDueDate);
			Assert.IsNull(task.LastStartTime);
		}

		[TestMethod]
		public void Reschedule_Null_Error()
		{
			DateTimeOffset now = new DateTimeOffset(2013, 6, 1, 12, 0, 0, 0, TimeSpan.Zero);
			DateTimeOffset now2 = new DateTimeOffset(2013, 6, 1, 12, 0, 1, 0, TimeSpan.Zero);
			ScheduledTask<FailableScheduledAction> task = new ScheduledTask<FailableScheduledAction>(Schedule.CreateOneTime(now), new FailableScheduledAction(FailableScheduledAction.DummyAction.Run), true, now, now2);

			CustomAssert.ThrowsException<CodedArgumentNullException>(() =>
			{
				task.Reschedule(null);
			});
		}

		[TestMethod]
		public void Dispose_Success()
		{
			DateTimeOffset now = new DateTimeOffset(2013, 6, 1, 12, 0, 0, 0, TimeSpan.Zero);
			DateTimeOffset now2 = new DateTimeOffset(2013, 6, 1, 12, 0, 1, 0, TimeSpan.Zero);
			ScheduledTask<FailableScheduledAction> task = new ScheduledTask<FailableScheduledAction>(Schedule.CreateOneTime(now), new FailableScheduledAction(FailableScheduledAction.DummyAction.Run), true, now, now2);

			task.Dispose();

			CustomAssert.ThrowsException<ObjectDisposedException>(() =>
			{
				FailableScheduledAction action = task.Action;
			});

			task.Dispose();
		}

		[TestMethod]
		public void GetSchema_Success()
		{
			DateTimeOffset now = new DateTimeOffset(2013, 6, 1, 12, 0, 0, 0, TimeSpan.Zero);
			DateTimeOffset now2 = new DateTimeOffset(2013, 6, 1, 12, 0, 1, 0, TimeSpan.Zero);
			ScheduledTask<FailableScheduledAction> task = new ScheduledTask<FailableScheduledAction>(Schedule.CreateOneTime(now), new FailableScheduledAction(FailableScheduledAction.DummyAction.Run), true, now, now2);

			Assert.IsNull(((IXmlSerializable)task).GetSchema());
		}

		[TestMethod]
		public void Run_Success()
		{
			DateTimeOffset now = new DateTimeOffset(2013, 6, 1, 12, 0, 0, 0, TimeSpan.Zero);
			DateTimeOffset now2 = new DateTimeOffset(2013, 6, 1, 12, 0, 1, 0, TimeSpan.Zero);
			ScheduledTask<FailableScheduledAction> task = new ScheduledTask<FailableScheduledAction>(Schedule.CreateOneTime(now), new FailableScheduledAction(FailableScheduledAction.DummyAction.Run), true, now, now2);

			SimpleSchedulerAction action = new SimpleSchedulerAction();
			task.Run(action);

			Assert.IsTrue(action.IsSuccess);
			Assert.IsFalse(action.IsCanceled);
			Assert.IsNull(action.Exception);
			Assert.IsInstanceOfType(action.State, typeof(int));
		}

		[TestMethod]
		public void Run_Canceled_Success()
		{
			DateTimeOffset now = new DateTimeOffset(2013, 6, 1, 12, 0, 0, 0, TimeSpan.Zero);
			DateTimeOffset now2 = new DateTimeOffset(2013, 6, 1, 12, 0, 1, 0, TimeSpan.Zero);
			ScheduledTask<FailableScheduledAction> task = new ScheduledTask<FailableScheduledAction>(Schedule.CreateOneTime(now), new FailableScheduledAction(FailableScheduledAction.DummyAction.Run), true, now, now2);

			SimpleSchedulerAction action = new SimpleSchedulerAction();
			action.Cancel();
			task.Run(action);

			Assert.IsFalse(action.IsSuccess);
			Assert.IsTrue(action.IsCanceled);
			Assert.IsNull(action.Exception, nameof(action.Exception));
			Assert.IsNotNull(action.State, nameof(action.State));
		}

		[TestMethod]
		public void Run_CanceledException_Success()
		{
			DateTimeOffset now = new DateTimeOffset(2013, 6, 1, 12, 0, 0, 0, TimeSpan.Zero);
			DateTimeOffset now2 = new DateTimeOffset(2013, 6, 1, 12, 0, 1, 0, TimeSpan.Zero);
			ScheduledTask<FailableScheduledAction> task = new ScheduledTask<FailableScheduledAction>(Schedule.CreateOneTime(now), new FailableScheduledAction(FailableScheduledAction.DummyAction.Cancel), true, now, now2);

			SimpleSchedulerAction action = new SimpleSchedulerAction();
			task.Run(action);

			Assert.IsFalse(action.IsSuccess);
			Assert.IsTrue(action.IsCanceled);
			Assert.IsNotNull(action.Exception, nameof(action.Exception));
			Assert.IsNull(action.State, nameof(action.State));
		}

		[TestMethod]
		public void Run_Exception_Success()
		{
			DateTimeOffset now = new DateTimeOffset(2013, 6, 1, 12, 0, 0, 0, TimeSpan.Zero);
			DateTimeOffset now2 = new DateTimeOffset(2013, 6, 1, 12, 0, 1, 0, TimeSpan.Zero);
			ScheduledTask<FailableScheduledAction> task = new ScheduledTask<FailableScheduledAction>(Schedule.CreateOneTime(now), new FailableScheduledAction(FailableScheduledAction.DummyAction.ThrowException), true, now, now2);

			SimpleSchedulerAction action = new SimpleSchedulerAction();
			task.Run(action);

			Assert.IsFalse(action.IsSuccess);
			Assert.IsFalse(action.IsCanceled);
			Assert.IsNotNull(action.Exception, nameof(action.Exception));
			Assert.IsNull(action.State, nameof(action.State));
		}

		[TestMethod]
		public void WriteXml_Success()
		{
			DateTimeOffset now = new DateTimeOffset(2013, 6, 1, 12, 0, 0, 0, TimeSpan.Zero);
			DateTimeOffset now2 = new DateTimeOffset(2013, 6, 1, 12, 0, 1, 0, TimeSpan.Zero);
			ScheduledTask<FailableScheduledAction> task = new ScheduledTask<FailableScheduledAction>(Schedule.CreateOneTime(now), new FailableScheduledAction(FailableScheduledAction.DummyAction.ThrowException), true, now, now2);

			StringWriter wr = new StringWriter();
			XmlSerializer serializer = new XmlSerializer(typeof(ScheduledTask<FailableScheduledAction>));
			serializer.Serialize(wr, task);
			string output = wr.ToString();
			StringAssert.Contains(output, "<scheduledTask");
			StringAssert.Contains(output, "<schedule ");
			StringAssert.Contains(output, "<scheduledAction ");
		}

		[TestMethod]
		public void WriteXml_WriterNull_Error()
		{
			DateTimeOffset now = new DateTimeOffset(2013, 6, 1, 12, 0, 0, 0, TimeSpan.Zero);
			DateTimeOffset now2 = new DateTimeOffset(2013, 6, 1, 12, 0, 1, 0, TimeSpan.Zero);
			ScheduledTask<FailableScheduledAction> task = new ScheduledTask<FailableScheduledAction>(Schedule.CreateOneTime(now), new FailableScheduledAction(FailableScheduledAction.DummyAction.ThrowException), true, now, now2);

			XmlWriter w = null;
			CustomAssert.ThrowsException<CodedArgumentNullException>(() =>
			{
				((IXmlSerializable)task).WriteXml(w);
			});
		}

		[TestMethod]
		public void IsTaskDue_False_Success()
		{
			ScheduledTask<FailableScheduledAction> task = new ScheduledTask<FailableScheduledAction>(Schedule.CreateOneTime(DateTimeOffset.Now.AddMinutes(-1.0)), new FailableScheduledAction(FailableScheduledAction.DummyAction.ThrowException), true, null, null);

			task.IsEnabled = false;
			Assert.IsFalse(task.IsTaskDue);
			task.IsEnabled = true;

			task.IsScheduled = true;
			Assert.IsFalse(task.IsTaskDue);
			task.IsScheduled = false;

			task.Dispose();
			Assert.IsFalse(task.IsTaskDue);

			task = new ScheduledTask<FailableScheduledAction>();
			Assert.IsFalse(task.IsTaskDue);
		}

		[TestMethod]
		public void ReadXml_Success()
		{
			string input = "<?xml version=\"1.0\" encoding=\"utf-16\"?><scheduledTask isEnabled=\"true\" lastStartTime=\"2013-06-01T12:00:00Z\" lastEndTime=\"2013-06-01T12:00:01Z\" xmlns=\"http://www.nerdyduck.de/Scheduler\"><schedule type=\"OneTime\" scheduledDateTime=\"2013-06-01T12:00:00Z\" /><scheduledAction behavior=\"ThrowException\" /></scheduledTask>";
			StringReader r = new StringReader(input);

			XmlSerializer serializer = new XmlSerializer(typeof(ScheduledTask<FailableScheduledAction>));
			ScheduledTask<FailableScheduledAction> task = (ScheduledTask<FailableScheduledAction>)serializer.Deserialize(r);

			Assert.AreEqual(2013, task.LastStartTime.Value.Year);
			Assert.AreEqual(2013, task.LastEndTime.Value.Year);
			Assert.IsTrue(task.IsEnabled);
			Assert.AreEqual(ScheduleType.OneTime, task.Schedule.ScheduleType);
		}

		[TestMethod]
		public void ReadXml_NoSchedule_Error()
		{
			string input = "<?xml version=\"1.0\" encoding=\"utf-16\"?><scheduledTask isEnabled=\"true\" lastStartTime=\"2013-06-01T12:00:00Z\" lastEndTime=\"2013-06-01T12:00:01Z\" xmlns=\"http://www.nerdyduck.de/Scheduler\"><scheduledAction behavior=\"ThrowException\" /></scheduledTask>";
			StringReader r = new StringReader(input);

			XmlSerializer serializer = new XmlSerializer(typeof(ScheduledTask<FailableScheduledAction>));
			CustomAssert.ThrowsException<InvalidOperationException>(() =>
			{
				ScheduledTask<FailableScheduledAction> task = (ScheduledTask<FailableScheduledAction>)serializer.Deserialize(r);
			});

		}

		[TestMethod]
		public void ReadXml_NoAction_Error()
		{
			string input = "<?xml version=\"1.0\" encoding=\"utf-16\"?><scheduledTask isEnabled=\"true\" lastStartTime=\"2013-06-01T12:00:00Z\" lastEndTime=\"2013-06-01T12:00:01Z\" xmlns=\"http://www.nerdyduck.de/Scheduler\"><schedule type=\"OneTime\" scheduledDateTime=\"2013-06-01T12:00:00Z\" /></scheduledTask>";
			StringReader r = new StringReader(input);

			XmlSerializer serializer = new XmlSerializer(typeof(ScheduledTask<FailableScheduledAction>));
			CustomAssert.ThrowsException<InvalidOperationException>(() =>
			{
				ScheduledTask<FailableScheduledAction> task = (ScheduledTask<FailableScheduledAction>)serializer.Deserialize(r);
			});

		}


		[TestMethod]
		public void ReadXml_ReaderNull_Error()
		{
			ScheduledTask<FailableScheduledAction> task = new ScheduledTask<FailableScheduledAction>();

			XmlReader w = null;
			CustomAssert.ThrowsException<CodedArgumentNullException>(() =>
			{
				((IXmlSerializable)task).ReadXml(w);
			});
		}

	}
}
