#region Copyright
/*******************************************************************************
 * <copyright file="FailableScheduledAction.cs" owner="Daniel Kopp">
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
 * <file name="FailableScheduledAction.cs" date="2016-02-19">
 * Implementation of ScheduledActionBase that throws can either complete
 * successfully or throw exceptions when run.
 * </file>
 ******************************************************************************/
#endregion

using NerdyDuck.Scheduler;
using System;
using System.Xml;
#if WINDOWS_DESKTOP
using System.Diagnostics.CodeAnalysis;
#endif


namespace NerdyDuck.Tests.Scheduler
{
	/// <summary>
	/// Implementation of ScheduledActionBase that throws can either complete successfully or throw exceptions when run.
	/// </summary>
#if WINDOWS_DESKTOP
	[ExcludeFromCodeCoverage]
#endif
	public class FailableScheduledAction : ScheduledActionBase
	{
		private DateTimeOffset mLastRun;
		private DummyAction mAction;

		/// <summary>
		/// Gets the last time the action was run.
		/// </summary>
		public DateTimeOffset LastRun
		{
			get { return mLastRun; }
		}

		/// <summary>
		/// Gets the type of action to execute when Run is called.
		/// </summary>
		public DummyAction Action
		{
			get { return mAction; }
		}

		/// <summary>
		/// Gets a value indicating that the object was disposed.
		/// </summary>
		public bool IsDisposedTest
		{
			get { return base.IsDisposed; }
		}

		/// <summary>
		/// Initializes a new instance of the FailableScheduledAction class with the action to execute.
		/// </summary>
		/// <param name="action">The type of action to execute when Run is called.</param>
		public FailableScheduledAction(DummyAction action)
		{
			mAction = action;
			mLastRun = DateTimeOffset.MinValue;
		}

		/// <summary>
		/// Initializes a new instance of the FailableScheduledAction class.
		/// </summary>
		public FailableScheduledAction()
		{
			mAction = DummyAction.Run;
			mLastRun = DateTimeOffset.MinValue;
		}

		/// <summary>
		/// Depending on Action, Run either completes or throws OperationCanceledExceptions or InvalidOperationExceptions.
		/// </summary>
		/// <param name="lastExecuted"></param>
		/// <param name="operation"></param>
		/// <returns></returns>
		public override object Run(DateTimeOffset? lastExecuted, ISchedulerAction operation)
		{
			mLastRun = DateTimeOffset.Now;
			if (mAction == DummyAction.Cancel)
			{
				throw new OperationCanceledException();
			}
			else if (mAction == DummyAction.ThrowException)
			{
				throw new InvalidOperationException();
			}
			return 42;
		}

		/// <summary>
		/// Test implementation of IXmlSerializable
		/// </summary>
		/// <param name="writer"></param>
		public override void WriteXml(XmlWriter writer)
		{
			writer.WriteAttributeString("behavior", mAction.ToString());
		}

		/// <summary>
		/// Test implementation of IXmlSerializable
		/// </summary>
		/// <param name="reader"></param>
		public override void ReadXml(XmlReader reader)
		{
			mAction = (DummyAction)Enum.Parse(typeof(DummyAction), reader.GetAttribute("behavior"));
		}

		/// <summary>
		/// The possible actions that Run will execute.
		/// </summary>
		public enum DummyAction
		{
			Run,
			Cancel,
			ThrowException
		}
	}
}
