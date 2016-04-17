#region Copyright
/*******************************************************************************
 * <copyright file="ScheduledTaskCompletedEventArgs.cs" owner="Daniel Kopp">
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
 * <file name="ScheduledTaskCompletedEventArgs.cs" date="2016-02-18">
 * Provides data for the Scheduler&lt;T&gt;.TaskCompleted event.
 * </file>
 ******************************************************************************/
#endregion

using NerdyDuck.CodedExceptions;
using System;

namespace NerdyDuck.Scheduler
{
	/// <summary>
	/// Provides data for the <see cref="Scheduler{T}.TaskCompleted"/> event.
	/// </summary>
	/// <typeparam name="T">The type of action that can be executed by the task, derived from <see cref="ScheduledActionBase"/>.</typeparam>
	public class ScheduledTaskCompletedEventArgs<T> : System.EventArgs where T : ScheduledActionBase, new()
	{
		#region Private fields
		private ScheduledTask<T> mTask;
		private bool mIsSuccess;
		private bool mIsCanceled;
		private Exception mException;
		private object mState;
		#endregion

		#region Properties
		/// <summary>
		/// Gets the task that was executed.
		/// </summary>
		/// <value>A <see cref="ScheduledTask{T}"/>.</value>
		/// <value>The task that just executed.</value>
		public ScheduledTask<T> Task
		{
			get { return mTask; }
		}

		/// <summary>
		/// Gets a value indicating if the <see cref="Task"/> completed successfully.
		/// </summary>
		/// <value><see langword="true"/>, if the <see cref="Task"/> completed successfully; otherwise, <see langword="false"/>.</value>
		public bool IsSuccess
		{
			get { return mIsSuccess; }
		}

		/// <summary>
		/// Gets a value indicating if the <see cref="Task"/> was canceled or aborted.
		/// </summary>
		/// <value><see langword="true"/>, if the <see cref="Task"/> was canceled; otherwise, <see langword="false"/>.</value>
		public bool IsCanceled
		{
			get { return mIsCanceled; }
		}

		/// <summary>
		/// Gets the exception that caused the action to fail.
		/// </summary>
		/// <value>If <see cref="IsSuccess"/> and <see cref="IsCanceled"/> is <see langword="false"/>, an <see cref="Exception"/>; otherwise, <see langword="null"/>.</value>
		public Exception Exception
		{
			get { return mException; }
		}

		/// <summary>
		/// Gets an arbitrary state object.
		/// </summary>
		/// <value>The return value of the executed task. May be <see langword="null"/>.</value>
		public object State
		{
			get { return mState; }
		}
		#endregion

		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="ScheduledTaskCompletedEventArgs{T}"/> with the specified parameters.
		/// </summary>
		/// <param name="task">The task that was executed.</param>
		/// <param name="isSuccess">A value indicating if the <paramref name="task"/> completed successfully.</param>
		/// <param name="isCanceled">A value indicating if the <paramref name="task"/> was canceled or aborted.</param>
		/// <param name="ex">The exception that caused the action to fail.</param>
		/// <param name="state">An arbitrary state object.</param>
		public ScheduledTaskCompletedEventArgs(ScheduledTask<T> task, bool isSuccess, bool isCanceled, Exception ex, object state)
		{
			if (task == null)
				throw new CodedArgumentNullException(Errors.CreateHResult(ErrorCodes.ScheduledTaskCompletedEventArgs_ctor_TaskNull), nameof(task));

			mTask = task;
			mIsSuccess = isSuccess;
			mIsCanceled = isCanceled;
			mException = ex;
			mState = state;
		}
		#endregion
	}
}
