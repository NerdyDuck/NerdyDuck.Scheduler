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

using System.Xml;


namespace NerdyDuck.Tests.Scheduler
{
	/// <summary>
	/// Implementation of ScheduledActionBase that throws can either complete successfully or throw exceptions when run.
	/// </summary>
	[ExcludeFromCodeCoverage]
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
			get { return base.IsDisposed == 1; }
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
