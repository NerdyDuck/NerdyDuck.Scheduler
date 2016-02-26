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

using NerdyDuck.CodedExceptions;
using NerdyDuck.Collections.Concurrent;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.System.Threading;

namespace NerdyDuck.Scheduler
{
	// UWP specific part of implementation
	partial class Scheduler<T>
	{
		#region Private fields
		private BlockingConcurrentDictionary<ScheduledTask<T>, IAsyncAction> ScheduledTasks;
		#endregion

		#region Private methods
		/// <summary>
		/// Initializes all implementation-specific fields.
		/// </summary>
		private void Init()
		{
			ScheduledTasks = new BlockingConcurrentDictionary<ScheduledTask<T>, IAsyncAction>();
		}

		/// <summary>
		/// Request the cancellation of all tasks currently active.
		/// </summary>
		private void CancelAllTasks()
		{
			foreach (KeyValuePair<ScheduledTask<T>, IAsyncAction> pair in ScheduledTasks)
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
						WorkItemHandler wih = new WorkItemHandler((IAsyncAction operation) =>
						{
							AsyncActionSchedulerAction sa = new AsyncActionSchedulerAction(operation);
							try
							{
								if (sa.IsCancellationRequested || OnTaskStarting(task))
								{
									sa.IsCanceled = true;
								}
								else
								{
									task.Run(sa);
								}
							}
							catch (OperationCanceledException ex)
							{
								sa.IsSuccess = false;
								sa.IsCanceled = true;
								sa.Exception = ex;
							}
							catch (Exception ex)
							{
								sa.IsSuccess = false;
								sa.IsCanceled = false;
								sa.Exception = ex;
							}
							finally
							{
								OnTaskCompleted(task, sa);
								SpinWait.SpinUntil(() => { return ScheduledTasks.Remove(task); }, 100);
								task.IsScheduled = false;
								sa.Operation.Close();
							}
						});
						task.IsScheduled = true;
						ScheduledTasks.Add(task, ThreadPool.RunAsync(wih, WorkItemPriority.Normal, WorkItemOptions.TimeSliced));
					}
				}
			}
		}
		#endregion

		#region Private classes
		/// <summary>
		/// Implementation if <see cref="ISchedulerAction"/> for use with a <see cref="IAsyncAction"/>.
		/// </summary>
		internal class AsyncActionSchedulerAction : ISchedulerAction
		{
			#region Private fields
			private IAsyncAction mOperation;
			#endregion

			#region Constructor
			/// <summary>
			/// Initializes a new instance of the <see cref="AsyncActionSchedulerAction"/> class with the specified asynchronous operation.
			/// </summary>
			public AsyncActionSchedulerAction(IAsyncAction operation)
			{
				mOperation = operation;
			}
			#endregion

			/// <summary>
			/// Gets whether cancellation has been requested for this action.
			/// </summary>
			/// <value><see langword="true"/>, if the action should be canceled; otherwise, <see langword="false"/>.</value>
			public bool IsCancellationRequested
			{
				get { return mOperation.Status == AsyncStatus.Canceled; }
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
			/// Gets the underlying <see cref="IAsyncAction"/>.
			/// </summary>
			public IAsyncAction Operation
			{
				get { return mOperation; }
			}

			/// <summary>
			/// Throws an exception if the action was canceled.
			/// </summary>
			/// <exception cref="OperationCanceledException"><see cref="IsCancellationRequested"/> is <see langword="true"/>.</exception>
			public void ThrowIfCancellationRequested()
			{
				if (mOperation.Status == AsyncStatus.Canceled)
				{
					throw new OperationCanceledException();
				}
			}

			/// <summary>
			/// Sets <see cref="IsCancellationRequested"/> to <see langword="true"/>.
			/// </summary>
			public void Cancel()
			{
				mOperation.Cancel();
			}
		}
		#endregion
	}
}
