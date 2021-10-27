#region Copyright
/*******************************************************************************
 * NerdyDuck.Scheduler - Task scheduler for .NET with variable schedules.
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

using System.Globalization;
using System.Threading;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace NerdyDuck.Scheduler;

/// <summary>
/// The base class for an action that can be executed by the <see cref="Scheduler{T}"/>.
/// </summary>
/// <typeparam name="T">The type of action that is executed by the task.</typeparam>
[XmlRoot(ElementName = RootName, Namespace = Schedule.Namespace)]
public sealed class ScheduledTask<T> : IDisposable, IXmlSerializable where T : ScheduledActionBase, new()
{
	internal const string RootName = "scheduledTask";
	private const string IsEnabledName = "isEnabled";
	private const string LastStartTimeName = "lastStartTime";
	private const string LastEndTimeName = "lastEndTime";

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
			throw new CodedArgumentNullException(HResult.Create(ErrorCodes.ScheduledTask_ctor_ScheduleNull), nameof(schedule));
		if (action == null)
			throw new CodedArgumentNullException(HResult.Create(ErrorCodes.ScheduledTask_ctor_ActionNull), nameof(schedule));

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

	/// <summary>
	/// Destructor.
	/// </summary>
	~ScheduledTask()
	{
		Dispose(false);
	}

	/// <summary>
	/// Changes the <see cref="Schedule"/> for the <see cref="ScheduledTask{T}"/>;
	/// </summary>
	/// <param name="schedule">The new schedule.</param>
	public void Reschedule(Schedule schedule)
	{
		AssertActive();
		if (schedule == null)
			throw new CodedArgumentNullException(HResult.Create(ErrorCodes.ScheduledTask_Reschedule_ScheduleNull), nameof(schedule));
		mSchedule = schedule;
		if (mSchedule.ScheduleType == ScheduleType.OneTime)
			mLastStartTime = mLastEndTime = null;
		mNextDueDate = mSchedule.GetNextDueDate(DateTimeOffset.Now, mLastStartTime);
	}

	/// <summary>
	/// Recalculates the new due date of the <see cref="ScheduledTask{T}"/>.
	/// </summary>
	/// <param name="now">The date and time to use as current date and time.</param>
	public void UpdateSchedule(DateTimeOffset now)
	{
		mNextDueDate = mSchedule.GetNextDueDate(now, mLastStartTime);
	}

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
			else if (ex is ThreadAbortException)
				operation.IsCanceled = true;
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

	/// <summary>
	/// Releases allocated resources.
	/// </summary>
	/// <param name="disposing">A value indicating if the method was called by user code. If <see langword="false"/>, the method was called by the runtime in the finalizer.</param>
	/// <remarks>If <paramref name="disposing"/> is <see langword="false"/>, no other objects should be referenced.</remarks>
	private void Dispose(bool disposing)
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

	/// <summary>
	/// Reserved method.
	/// </summary>
	/// <returns>Always returns <see langword="null"/>.</returns>
	XmlSchema IXmlSerializable.GetSchema()
	{
		return null;
	}

	/// <summary>
	/// Reads the <see cref="ScheduledTask{T}"/> configuration from the specified <paramref name="reader"/>.
	/// </summary>
	/// <param name="reader">A <see cref="XmlReader"/> containing a serialized instance of a <see cref="ScheduledTask{T}"/>.</param>
	void IXmlSerializable.ReadXml(XmlReader reader)
	{
		if (reader == null)
		{
			throw new CodedArgumentNullException(HResult.Create(ErrorCodes.ScheduledTask_ReadXml_ArgNull), nameof(reader));
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
			if (reader.Name == Schedule.RootName)
			{
				mSchedule = new Schedule();
				((IXmlSerializable)mSchedule).ReadXml(reader);
			}
			else if (reader.Name == ScheduledActionBase.RootName)
			{
				mAction = new T();
				mAction.ReadXml(reader);
			}
		} while (!(reader.Name == root && reader.NodeType == XmlNodeType.EndElement));

		if (mSchedule == null)
		{
			throw new CodedXmlException(HResult.Create(ErrorCodes.ScheduledTask_ReadXml_ScheduleNull), string.Format(CultureInfo.CurrentCulture, TextResources.ScheduledTask_ReadXml_MissingElement, Schedule.RootName));
		}

		if (mAction == null)
		{
			throw new CodedXmlException(HResult.Create(ErrorCodes.ScheduledTask_ReadXml_ActionNull), string.Format(CultureInfo.CurrentCulture, TextResources.ScheduledTask_ReadXml_MissingElement, ScheduledActionBase.RootName));
		}
		mNextDueDate = mSchedule.GetNextDueDate(DateTimeOffset.Now, mLastStartTime);
		IsInitialized = true;
	}

	/// <summary>
	/// Writes the configuration of the <see cref="ScheduledTask{T}"/> to the specified <paramref name="writer"/>.
	/// </summary>
	/// <param name="writer">A <see cref="XmlWriter"/> that receives the configuration data.</param>
	void IXmlSerializable.WriteXml(XmlWriter writer)
	{
		if (writer == null)
		{
			throw new CodedArgumentNullException(HResult.Create(ErrorCodes.ScheduledTask_WriteXml_ArgNull), nameof(writer));
		}

		writer.WriteAttributeString(IsEnabledName, XmlConvert.ToString(mIsEnabled));
		if (mLastStartTime.HasValue)
			writer.WriteAttributeString(LastStartTimeName, XmlConvert.ToString(mLastStartTime.Value));
		if (mLastEndTime.HasValue)
			writer.WriteAttributeString(LastEndTimeName, XmlConvert.ToString(mLastEndTime.Value));
		writer.WriteStartElement(Schedule.RootName);
		((IXmlSerializable)mSchedule).WriteXml(writer);
		writer.WriteEndElement();
		writer.WriteStartElement(ScheduledActionBase.RootName);
		mAction.WriteXml(writer);
		writer.WriteEndElement();
	}

	/// <summary>
	/// Checks that the current object is initialized and not disposed.
	/// </summary>
	private void AssertActive()
	{
		if (IsDisposed)
			throw new ObjectDisposedException(ToString());

		if (!IsInitialized)
			throw new CodedInvalidOperationException(HResult.Create(ErrorCodes.ScheduledTask_AssertActive_NotInitialized), TextResources.ScheduledTask_AssertActive_NotInit);
	}
}
