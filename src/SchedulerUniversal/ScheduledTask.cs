#region Copyright
/*******************************************************************************
 * <copyright file="ScheduledTask.cs" owner="Daniel Kopp">
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
 * <assembly name="NerdyDuck.Scheduler">
 * Task scheduler for .NET with variable schedules.
 * </assembly>
 * <file name="ScheduledTask.cs" date="2016-02-02">
 * The base class for an action that can be executed by the Scheduler.
 * </file>
 ******************************************************************************/
#endregion

using NerdyDuck.CodedExceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace NerdyDuck.Scheduler
{
	/// <summary>
	/// The base class for an action that can be executed by the <see cref="Scheduler{T}"/>.
	/// </summary>
	/// <typeparam name="T">The type of action that is executed by the task.</typeparam>
	[XmlRoot(ElementName = RootName, Namespace = Schedule.Namespace)]
	public class ScheduledTask<T> : IDisposable, IXmlSerializable where T : ScheduledActionBase, new()
	{
		#region Constants
		private const string RootName = "scheduledTask";
		private const string IsEnabledName = "isEnabled";
		private const string LastStartTimeName = "lastStartTime";
		private const string LastEndTimeName = "lastEndTime";
		private const string ScheduleName = "schedule";
		private const string ActionName = "action";
		#endregion

		#region Private fields
		private bool IsDisposed;
		private bool IsInitialized;
		private T mAction;
		private Schedule mSchedule;
		private bool mIsEnabled;
		private bool mIsActive;
		private DateTimeOffset? mLastStartTime;
		private DateTimeOffset? mLastEndTime;
		private DateTimeOffset mNextDueDate;
		internal bool IsScheduled;
		#endregion

		#region Properties
		/// <summary>
		/// Gets the action that is scheduled by the <see cref="ScheduledTask{T}"/>.
		/// </summary>
		/// <value>A instance of a class derived from <see cref="ScheduledActionBase"/>.</value>
		public T Action
		{
			get
			{
				AssertActive();
				return mAction;
			}
		}

		/// <summary>
		/// Gets the <see cref="Schedule"/> that activates the task.
		/// </summary>
		/// <value>A <see cref="Schedule"/>.</value>
		public Schedule Schedule
		{
			get
			{
				AssertActive();
				return mSchedule;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating if the <see cref="ScheduledTask{T}"/> is enabled.
		/// </summary>
		/// <value>If <see langword="true"/>, the <see cref="Schedule"/> is evaluated by the <see cref="Scheduler{T}"/> to determine when the task is due. Otherwise, the task will not run, even if it is due.</value>
		public bool IsEnabled
		{
			get { return mIsEnabled; }
			set { mIsEnabled = value; }
		}

		/// <summary>
		/// Gets a value indicating if the <see cref="ScheduledTask{T}"/> is currently being executed.
		/// </summary>
		/// <value>If <see langword="true"/>, the task is currently running.</value>
		public bool IsActive
		{
			get { return mIsActive; }
		}

		/// <summary>
		/// Gets the date and time when the <see cref="ScheduledTask{T}"/> was last activated. May be <see langword="null"/>.
		/// </summary>
		/// <value>The last time the task was executed. If the value is <see langword="null"/>, the task has not yet run.</value>
		public DateTimeOffset? LastStartTime
		{
			get { return mLastStartTime; }
		}

		/// <summary>
		/// Gets the date and time when the <see cref="ScheduledTask{T}"/> last completed successfully. May be <see langword="null"/>.
		/// </summary>
		/// <value>The last time the task completed (and was not canceled). If the value is <see langword="null"/>, the task has yet to complete for the first time.</value>
		public DateTimeOffset? LastEndTime
		{
			get { return mLastEndTime; }
		}

		/// <summary>
		/// Gets the next date and time when the <see cref="ScheduledTask{T}"/> is next due to run.
		/// </summary>
		/// <value>If the value is smaller or equal to the current date and time, the task is due to run.</value>
		public DateTimeOffset NextDueDate
		{
			get { return mNextDueDate; }
		}

		/// <summary>
		/// Gets a value indicating if the task is due to run.
		/// </summary>
		/// <value>If <see langword="true"/>, the task is due to run.</value>
		public bool IsTaskDue
		{
			get
			{
				if (!mIsEnabled || mIsActive || IsScheduled || IsDisposed || !IsInitialized)
					return false;

				return (mNextDueDate <= DateTimeOffset.Now);
			}
		}
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="ScheduledTask{T}"/> class.
		/// </summary>
		public ScheduledTask()
		{
			mAction = null;
			mSchedule = null;
			mIsEnabled = false;
			mIsActive = false;
			mLastStartTime = null;
			mLastEndTime = null;
			mNextDueDate = DateTimeOffset.MaxValue;
			IsInitialized = false;
			IsScheduled = false;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ScheduledTask{T}"/> class with the specified parameters.
		/// </summary>
		/// <param name="schedule">The <see cref="Schedule"/> that activates the task.</param>
		/// <param name="action">The action that is scheduled by the <see cref="ScheduledTask{T}"/>.</param>
		/// <param name="isEnabled">A value indicating if the <see cref="ScheduledTask{T}"/> is enabled.</param>
		/// <param name="lastStartTime">The date and time when the <see cref="ScheduledTask{T}"/> was last activated. May be <see langword="null"/>.</param>
		/// <param name="lastEndTime">The date and time when the <see cref="ScheduledTask{T}"/> last completed successfully. May be <see langword="null"/>.</param>
		public ScheduledTask(Schedule schedule, T action, bool isEnabled, DateTimeOffset? lastStartTime, DateTimeOffset? lastEndTime)
		{
			if (schedule == null)
				throw new CodedArgumentNullException(Errors.CreateHResult(0x1a), nameof(schedule));
			if (action == null)
				throw new CodedArgumentNullException(Errors.CreateHResult(0x1b), nameof(schedule));

			mSchedule = schedule;
			mAction = action;
			mIsEnabled = isEnabled;
			mIsActive = false;
			mLastStartTime = lastStartTime;
			if (mLastStartTime.HasValue)
				mLastEndTime = lastEndTime;

			mNextDueDate = mSchedule.GetNextDueDate(DateTimeOffset.Now, mLastStartTime);
			IsInitialized = true;
			IsScheduled = false;
		}
		#endregion

		#region Destructor
		/// <summary>
		/// Destructor.
		/// </summary>
		~ScheduledTask()
		{
			Dispose(false);
		}
		#endregion

		#region Public methods
		#region Reschedule
		/// <summary>
		/// Changes the <see cref="Schedule"/> for the <see cref="ScheduledTask{T}"/>;
		/// </summary>
		/// <param name="schedule">The new schedule.</param>
		public void Reschedule(Schedule schedule)
		{
			AssertActive();
			if (schedule == null)
				throw new CodedArgumentNullException(Errors.CreateHResult(0x1c), nameof(schedule));
			mSchedule = schedule;
			if (mSchedule.ScheduleType == ScheduleType.OneTime)
				mLastStartTime = mLastEndTime = null;
			mNextDueDate = mSchedule.GetNextDueDate(DateTimeOffset.Now, mLastStartTime);
		}
		#endregion

		#region UpdateSchedule
		/// <summary>
		/// Recalculates the new due date of the <see cref="ScheduledTask{T}"/>.
		/// </summary>
		/// <param name="now">The date and time to use as current date and time.</param>
		public void UpdateSchedule(DateTimeOffset now)
		{
			mNextDueDate = mSchedule.GetNextDueDate(now, mLastStartTime);
		}
		#endregion

		#region Run
		/// <summary>
		/// Executes the <see cref="Action"/> of the <see cref="ScheduledTask{T}"/>.
		/// </summary>
		/// <param name="operation">Contains the execution state and results.</param>
		/// <exception cref="CodedArgumentNullException"><paramref name="operation"/> is <see langword="null"/>.</exception>
		public void Run(ISchedulerAction operation)
		{
			if (operation == null)
			{
				return;
			}
			DateTimeOffset NewStartTime = DateTimeOffset.Now;
			try
			{
				AssertActive();

				mIsActive = true;

				operation.State = mAction.Run(mLastStartTime, operation);
				if (operation.IsCancellationRequested)
				{
					operation.IsCanceled = true;
					operation.IsSuccess = false;
				}
				else
				{
					operation.IsSuccess = true;
				}

			}
			catch (Exception ex)
			{
				if (ex is OperationCanceledException)
					operation.IsCanceled = true;
#if WINDOWS_DESKTOP
				else if (ex is ThreadAbortException)
					operation.IsCanceled = true;
#endif
				operation.IsSuccess = false;
				operation.Exception = ex;
			}
			finally
			{
				if (!operation.IsCanceled)
				{
					mLastStartTime = NewStartTime;
				}
				if (operation.IsSuccess)
				{
					mLastEndTime = DateTimeOffset.Now;
				}
				else
				{
					mLastEndTime = null;
				}
				mIsActive = false;
			}
		}
		#endregion

		#region IDisposable implementation
		/// <summary>
		/// Releases allocated resources.
		/// </summary>
		/// <param name="disposing">A value indicating if the method was called by user code. If <see langword="false"/>, the method was called by the runtime in the finalizer.</param>
		/// <remarks>If <paramref name="disposing"/> is <see langword="false"/>, no other objects should be referenced.</remarks>
		protected virtual void Dispose(bool disposing)
		{
			if (IsDisposed)
				return;

			IsDisposed = true;
			if (disposing)
			{
				if (mAction != null)
				{
					mAction.Dispose();
				}
			}
			mAction = null;
			mSchedule = null;
		}

		/// <summary>
		/// Releases allocated resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		#endregion

		#region IXmlSerializable implementation
		/// <summary>
		/// Reserved method.
		/// </summary>
		/// <returns>Always returns <see langword="null"/>.</returns>
		public XmlSchema GetSchema()
		{
			return null;
		}

		/// <summary>
		/// Reads the <see cref="ScheduledTask{T}"/> configuration from the specified <paramref name="reader"/>.
		/// </summary>
		/// <param name="reader">A <see cref="XmlReader"/> containing a serialized instance of a <see cref="ScheduledTask{T}"/>.</param>
		public void ReadXml(XmlReader reader)
		{
			if (reader == null)
			{
				throw new CodedArgumentNullException(Errors.CreateHResult(0x16), nameof(reader));
			}

			string root = reader.Name;
			if (reader.MoveToFirstAttribute())
			{
				do
				{
					switch (reader.Name)
					{
						case IsEnabledName:
							mIsEnabled = XmlConvert.ToBoolean(reader.Value);
							break;
						case LastStartTimeName:
							mLastStartTime = XmlConvert.ToDateTimeOffset(reader.Value);
							break;
						case LastEndTimeName:
							mLastEndTime = XmlConvert.ToDateTimeOffset(reader.Value);
							break;
					}
				} while (reader.MoveToNextAttribute());
			}

			do
			{
				reader.ReadStartElement();
				if (reader.Name == ScheduleName)
				{
					mSchedule = new Schedule();
					mSchedule.ReadXml(reader);
				}
				else if (reader.Name == ActionName)
				{
					mAction = new T();
					mAction.ReadXml(reader);
				}
			} while (!(reader.Name == root && reader.NodeType == XmlNodeType.EndElement));

			if (mAction == null)
			{
				throw new CodedXmlException(Errors.CreateHResult(0x19), string.Format(Properties.Resources.ScheduledTask_ReadXml_MissingElement, ActionName));
			}

			if (mSchedule == null)
			{
				throw new CodedXmlException(Errors.CreateHResult(0x18), string.Format(Properties.Resources.ScheduledTask_ReadXml_MissingElement, ScheduleName));
			}
			mNextDueDate = mSchedule.GetNextDueDate(DateTimeOffset.Now, mLastStartTime);
			IsInitialized = true;
		}

		/// <summary>
		/// Writes the configuration of the <see cref="ScheduledTask{T}"/> to the specified <paramref name="writer"/>.
		/// </summary>
		/// <param name="writer">A <see cref="XmlWriter"/> that receives the configuration data.</param>
		public void WriteXml(XmlWriter writer)
		{
			if (writer == null)
			{
				throw new CodedArgumentNullException(Errors.CreateHResult(0x17), nameof(writer));
			}

			writer.WriteAttributeString(IsEnabledName, XmlConvert.ToString(mIsEnabled));
			if (mLastStartTime.HasValue)
				writer.WriteAttributeString(LastStartTimeName, XmlConvert.ToString(mLastStartTime.Value));
			if (mLastEndTime.HasValue)
				writer.WriteAttributeString(LastEndTimeName, XmlConvert.ToString(mLastEndTime.Value));
			writer.WriteStartElement(ScheduleName);
			mSchedule.WriteXml(writer);
			writer.WriteEndElement();
			writer.WriteStartElement(ActionName);
			mAction.WriteXml(writer);
			writer.WriteEndElement();
		}
		#endregion
		#endregion

		#region Private methods
		#region AssertActive
		/// <summary>
		/// Checks that the current object is initialized and not disposed.
		/// </summary>
		private void AssertActive()
		{
			if (IsDisposed)
				throw new ObjectDisposedException(this.ToString());

			if (!IsInitialized)
				throw new CodedInvalidOperationException(Errors.CreateHResult(0x1c), Properties.Resources.ScheduledTask_AssertActive_NotInit);
		}
		#endregion
		#endregion
	}
}
