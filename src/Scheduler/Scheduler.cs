#region Copyright
/*******************************************************************************
 * <copyright file="Scheduler.cs" owner="Daniel Kopp">
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
 * <file name="Scheduler.cs" date="2016-02-17">
 * Schedules tasks to be executed at a predefined time, like the Windows&reg;
 * Task Scheduler.
 * </file>
 ******************************************************************************/
#endregion

using NerdyDuck.Collections.Concurrent;
using System;
using System.Collections.Generic;
using System.Threading;

namespace NerdyDuck.Scheduler
{
	// Full framework specific part of implementation
	partial class Scheduler<T>
	{
		#region Private fields
		private BlockingConcurrentDictionary<ScheduledTask<T>, CancellationTokenSchedulerAction> ScheduledTasks;
		#endregion

		#region Private methods
		/// <summary>
		/// Initializes all implementation-specific fields.
		/// </summary>
		private void Init()
		{
			ScheduledTasks = new BlockingConcurrentDictionary<ScheduledTask<T>, CancellationTokenSchedulerAction>();
		}

		/// <summary>
		/// Request the cancellation of all tasks currently active.
		/// </summary>
		private void CancelAllTasks()
		{
			foreach (KeyValuePair<ScheduledTask<T>, CancellationTokenSchedulerAction> pair in ScheduledTasks)
			{
				pair.Value.Cancel();
			}
		}

		/// <summary>
		/// Checks if a task is due, and starts it.
		/// </summary>
		/// <param name="state">The current <see cref="Scheduler{T}"/>.</param>
		private void TriggerCallback(object state)
		{
			if (IsDisposed || !mIsEnabled)
				return;
			lock (SyncRoot)
			{
				foreach (ScheduledTask<T> task in mTasks)
				{
					if (!mIsEnabled)
						break;
					if (task.IsTaskDue && !ScheduledTasks.ContainsKey(task))
					{
						CancellationTokenSchedulerAction action = new CancellationTokenSchedulerAction();
						task.IsScheduled = true;
						ScheduledTasks.Add(task, action);
						ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadPoolCallback), new Tuple<ScheduledTask<T>, CancellationTokenSchedulerAction>(task, action));
					}
				}
			}
		}

		/// <summary>
		/// The method that is executed by the thread pool.
		/// </summary>
		/// <param name="state">A state object. Contains the task to execute.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "This is a thread's Main method. No other place to handle any exception.")]
		private void ThreadPoolCallback(object state)
		{
			Tuple<ScheduledTask<T>, CancellationTokenSchedulerAction> Parameters = state as Tuple<ScheduledTask<T>, CancellationTokenSchedulerAction>;
			try
			{
				if (Parameters.Item2.IsCancellationRequested || OnTaskStarting(Parameters.Item1))
				{
					Parameters.Item2.IsCanceled = true;
				}
				else
				{
					Parameters.Item1.Run(Parameters.Item2);
				}
			}
			catch (Exception ex)
			{
				Parameters.Item2.IsSuccess = false;
				Parameters.Item2.IsCanceled = false;
				Parameters.Item2.Exception = ex;
			}
			finally
			{
				if (!IsDisposed)
				{
					OnTaskCompleted(Parameters.Item1, Parameters.Item2);
					BlockingConcurrentDictionary<ScheduledTask<T>, CancellationTokenSchedulerAction> tasks = ScheduledTasks;
					if (tasks != null)
					{
						tasks.Remove(Parameters.Item1);
					}
					Parameters.Item1.UpdateSchedule(DateTimeOffset.Now);
					Parameters.Item1.IsScheduled = false;
					Parameters.Item2.Source.Dispose();
				}
			}
		}
		#endregion

		#region Private classes
		/// <summary>
		/// Implementation if <see cref="ISchedulerAction"/> for use with a <see cref="CancellationTokenSource"/>.
		/// </summary>
		internal class CancellationTokenSchedulerAction : ISchedulerAction, IDisposable
		{
			#region Private fields
			private CancellationTokenSource mSource;
			private bool IsDisposed;
			#endregion

			#region Private fields
			/// <summary>
			/// Gets whether cancellation has been requested for this action.
			/// </summary>
			/// <value><see langword="true"/>, if the action should be canceled; otherwise, <see langword="false"/>.</value>
			public bool IsCancellationRequested
			{
				get
				{
					AssertDisposed();
					return mSource.Token.IsCancellationRequested;
				}
			}

			/// <summary>
			/// Gets or sets a value indicating if the action completed successfully.
			/// </summary>
			/// <value><see langword="true"/>, if the action completed successfully; otherwise, <see langword="false"/>.</value>
			public bool IsSuccess { get; set; }

			/// <summary>
			/// Gets or sets a value indicating if the action was canceled or aborted.
			/// </summary>
			/// <value><see langword="true"/>, if the action was canceled; otherwise, <see langword="false"/>.</value>
			public bool IsCanceled { get; set; }

			/// <summary>
			/// Gets or sets the exception that caused the action to fail.
			/// </summary>
			/// <value>If <see cref="IsSuccess"/> and <see cref="IsCanceled"/> is <see langword="false"/>, an <see cref="Exception"/>; otherwise, <see langword="null"/>.</value>
			public Exception Exception { get; set; }

			/// <summary>
			/// Gets or sets an arbitrary state object.
			/// </summary>
			public object State { get; set; }

			/// <summary>
			/// Gets the underlying <see cref="CancellationTokenSource"/>.
			/// </summary>
			public CancellationTokenSource Source
			{
				get { return mSource; }
			}
			#endregion

			#region Constructor
			/// <summary>
			/// Initializes a new instance of the <see cref="CancellationTokenSchedulerAction"/> class.
			/// </summary>
			public CancellationTokenSchedulerAction()
			{
				mSource = new CancellationTokenSource();
				IsDisposed = false;
			}
			#endregion

			#region Destructor
			/// <summary>
			/// Destructor.
			/// </summary>
			~CancellationTokenSchedulerAction()
			{
				Dispose(false);
			}
			#endregion

			#region Public methods
			/// <summary>
			/// Throws an exception if the action was canceled.
			/// </summary>
			/// <exception cref="OperationCanceledException"><see cref="IsCancellationRequested"/> is <see langword="true"/>.</exception>
			public void ThrowIfCancellationRequested()
			{
				AssertDisposed();
				mSource.Token.ThrowIfCancellationRequested();
			}

			/// <summary>
			/// Sets <see cref="IsCancellationRequested"/> to <see langword="true"/>.
			/// </summary>
			public void Cancel()
			{
				AssertDisposed();
				mSource.Cancel();
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

				IsDisposed = true;
				if (disposing)
				{
					mSource.Dispose();
				}
				mSource = null;
			}
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
			#endregion
		}
		#endregion
	}
}
