#region Copyright
/*******************************************************************************
 * <copyright file="Scheduler.Common.cs" owner="Daniel Kopp">
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
 * <file name="Scheduler.Common.cs" date="2016-02-17">
 * Schedules tasks to be executed at a predefined time, like the Windows&reg;
 * Task Scheduler.
 * </file>
 ******************************************************************************/
#endregion

using NerdyDuck.CodedExceptions;
using NerdyDuck.Collections.Concurrent;
using System;
using System.Collections.Generic;
using System.Threading;

namespace NerdyDuck.Scheduler
{
	/// <summary>
	/// Schedules tasks to be executed at a predefined time, like the Windows(r) Task Scheduler.
	/// </summary>
	/// <typeparam name="T">The type of action that can be executed by the task, derived from <see cref="ScheduledActionBase"/>.</typeparam>
	public partial class Scheduler<T> : IDisposable where T : ScheduledActionBase, new()
	{
		#region Events
		/// <summary>
		/// The event that is fired before a <see cref="ScheduledTask{T}"/> is executed.
		/// </summary>
		public event EventHandler<ScheduledTaskStartingEventArgs<T>> TaskStarting;

		/// <summary>
		/// The event that is fired after a <see cref="ScheduledTask{T}"/> was executed.
		/// </summary>
		public event EventHandler<ScheduledTaskCompletedEventArgs<T>> TaskCompleted;
		#endregion

		#region Private fields
		private bool IsDisposed;
		private bool mIsEnabled;
		private object SyncRoot;
		private TimeSpan mInterval;
		private Timer TriggerTimer;
		private NonBlockingConcurrentList<ScheduledTask<T>> mTasks;
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the time interval in which the <see cref="Scheduler{T}"/> checks for tasks that are due.
		/// </summary>
		/// <value>When the interval elapses, all <see cref="Tasks"/> are evaluated if they are due. Must be positive.</value>
		public TimeSpan Interval
		{
			get { return mInterval; }
			set
			{
				AssertDisposed();
				AssertInterval(value);
				mInterval = value;
				lock (SyncRoot)
				{
					if (mIsEnabled)
					{
						TriggerTimer.Change(mInterval, mInterval);
					}
				}
			}
		}

		/// <summary>
		/// Gets a value indicating if the <see cref="Scheduler{T}"/> is running.
		/// </summary>
		/// <value>If <see langword="true"/>, the <see cref="Scheduler{T}"/> is active and runs tasks when they are due.</value>
		public bool IsEnabled
		{
			get { return mIsEnabled; }
		}

		/// <summary>
		/// Gets a list of scheduled tasks.
		/// </summary>
		/// <value>The tasks to execute.</value>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
		public IList<ScheduledTask<T>> Tasks
		{
			get
			{
				AssertDisposed();
				return mTasks;
			}
		}
		#endregion

		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="Scheduler{T}"/> class.
		/// </summary>
		public Scheduler()
		{
			IsDisposed = false;
			mIsEnabled = false;
			SyncRoot = new object();
			mInterval = TimeSpan.FromMinutes(1.0);
			TriggerTimer = new Timer(new TimerCallback(TriggerCallback), this, Timeout.Infinite, Timeout.Infinite);
			mTasks = new NonBlockingConcurrentList<ScheduledTask<T>>();
			Init();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Scheduler{T}"/> class with the specified interval.
		/// </summary>
		/// <param name="interval">The time interval in which the <see cref="Scheduler{T}"/> checks for tasks that are due.</param>
		public Scheduler(TimeSpan interval)
			: this()
		{
			AssertInterval(interval);
			mInterval = interval;
		}
		#endregion

		#region Destructor
		/// <summary>
		/// Destructor.
		/// </summary>
		~Scheduler()
		{
			Dispose(false);
		}
		#endregion

		#region Public methods
		#region Start
		/// <summary>
		/// Starts the <see cref="Scheduler{T}"/>.
		/// </summary>
		public void Start()
		{
			AssertDisposed();
			lock (SyncRoot)
			{
				if (mIsEnabled)
					return;

				DateTimeOffset now = DateTimeOffset.Now;
				foreach (ScheduledTask<T> task in mTasks)
				{
					task.UpdateSchedule(now);
				}
				mIsEnabled = true;
				TriggerTimer.Change(TimeSpan.Zero, mInterval);
			}
		}
		#endregion

		#region Stop
		/// <summary>
		/// Stops the <see cref="Scheduler{T}"/> waiting for all tasks to finish or cancel.
		/// </summary>
		public void Stop()
		{
			Stop(TimeSpan.Zero);
		}

		/// <summary>
		/// Stops the <see cref="Scheduler{T}"/>, waiting a specified time for all tasks to finish or cancel.
		/// </summary>
		/// <param name="timeout">The maximum time to wait for all tasks to finish or cancel. If <see cref="TimeSpan.Zero"/> is specified, the method waits indefinitely.</param>
		/// <exception cref="CodedArgumentOutOfRangeException"><paramref name="timeout"/> is negative.</exception>
		/// <exception cref="CodedTimeoutException"><paramref name="timeout"/> has elapsed, but there are still tasks running.</exception>
		public void Stop(TimeSpan timeout)
		{
			if (timeout < TimeSpan.Zero)
			{
				throw new CodedArgumentOutOfRangeException(Errors.CreateHResult(0x1f), nameof(timeout), timeout, Properties.Resources.Scheduler_Stop_TimeoutNegative);
			}

			if (IsDisposed)
				return;

			bool WaitForTasks = false;
			lock (SyncRoot)
			{
				if (!mIsEnabled)
					return;

				mIsEnabled = false;
				TriggerTimer.Change(Timeout.Infinite, Timeout.Infinite);
				if (ScheduledTasks.Count > 0)
				{
					CancelAllTasks();
					WaitForTasks = true;
				}
			}


			if (WaitForTasks)
			{
				bool WaitResult = true;
				if (timeout != TimeSpan.Zero)
				{
					WaitResult = SpinWait.SpinUntil(() => { return ScheduledTasks.Count == 0; }, timeout);
				}
				else
				{
					SpinWait.SpinUntil(() => { return ScheduledTasks.Count == 0; });
				}

				if (!WaitResult)
				{
					throw new CodedTimeoutException(Errors.CreateHResult(0x20), Properties.Resources.Scheduler_Stop_Timeout);
				}
			}
		}
		#endregion

		#region IDisposable implementation
		/// <summary>
		/// Releases allocated resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Releases allocated resources.
		/// </summary>
		/// <param name="disposing">A value indicating if the method was called by user code. If <see langword="false"/>, the method was called by the runtime in the finalizer.</param>
		/// <remarks>If <paramref name="disposing"/> is <see langword="false"/>, no other objects should be referenced.</remarks>
		protected virtual void Dispose(bool disposing)
		{
			if (IsDisposed)
				return;

			try
			{
				if (disposing)
				{
					if (mIsEnabled)
					{
						Stop();
					}
					if (TriggerTimer != null)
					{
						TriggerTimer.Dispose();
					}
					if (mTasks != null)
					{
						foreach (ScheduledTask<T> task in mTasks)
						{
							task.Dispose();
						}
						mTasks.Clear();
					}
					if (ScheduledTasks != null)
					{
						ScheduledTasks.Dispose();
					}
				}
			}
			finally
			{
				IsDisposed = true;
			}
			TriggerTimer = null;
			mTasks = null;
			ScheduledTasks = null;
		}
		#endregion
		#endregion

		#region Private methods
		/// <summary>
		/// Checks if the <see cref="ScheduledTask{T}"/> is disposed.
		/// </summary>
		private void AssertDisposed()
		{
			if (IsDisposed)
			{
				throw new ObjectDisposedException(this.ToString());
			}
		}

		/// <summary>
		/// Checks if the interval is valid.
		/// </summary>
		/// <param name="interval">The interval to check.</param>
		private static void AssertInterval(TimeSpan interval)
		{
			if (interval < TimeSpan.Zero)
			{
				throw new CodedArgumentOutOfRangeException(Errors.CreateHResult(0x1e), nameof(interval), Properties.Resources.Scheduler_AssertInterval_Negative);
			}
		}

		/// <summary>
		/// Raises the <see cref="TaskStarting"/> event.
		/// </summary>
		/// <param name="task">The <see cref="ScheduledTask{T}"/> that is being executed.</param>
		/// <returns><see langword="true"/>, if the task execution should be canceled; otherwise, <see langword="false"/>.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		private bool OnTaskStarting(ScheduledTask<T> task)
		{
			ScheduledTaskStartingEventArgs<T> e = new ScheduledTaskStartingEventArgs<T>(task);
			try
			{
				TaskStarting?.Invoke(this, e);
			}
			catch { }
			return e.Cancel;
		}

		/// <summary>
		/// Raises the <see cref="TaskCompleted"/> event.
		/// </summary>
		/// <param name="task">The <see cref="ScheduledTask{T}"/> that is was executed.</param>
		/// <param name="action">The result of the execution.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		private void OnTaskCompleted(ScheduledTask<T> task, ISchedulerAction action)
		{
			try
			{
				TaskCompleted?.Invoke(this, new ScheduledTaskCompletedEventArgs<T>(task, action.IsSuccess, action.IsCanceled, action.Exception, action.State));
			}
			catch { }
		}
		#endregion
	}
}
