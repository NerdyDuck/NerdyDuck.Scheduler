#region Copyright
/*******************************************************************************
 * <copyright file="ScheduledTaskStartingEventArgs.cs" owner="Daniel Kopp">
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
 * <file name="ScheduledTaskStartingEventArgs.cs" date="2016-02-18">
 * Provides data for the Scheduler&lt;T&gt;.TaskStarting event.
 * </file>
 ******************************************************************************/
#endregion

using NerdyDuck.CodedExceptions;

namespace NerdyDuck.Scheduler
{
	/// <summary>
	/// Provides data for the <see cref="Scheduler{T}.TaskStarting"/> event.
	/// </summary>
	/// <typeparam name="T">The type of action that can be executed by the task, derived from <see cref="ScheduledActionBase"/>.</typeparam>
	public class ScheduledTaskStartingEventArgs<T> : System.EventArgs where T : ScheduledActionBase, new()
	{
		#region Private fields
		private bool mCancel;
		private ScheduledTask<T> mTask;
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets a value indicating if the task execution should be canceled.
		/// </summary>
		/// <value><see langword="true"/> if the execution should be canceled; otherwise, <see langword="false"/>.</value>
		public bool Cancel
		{
			get { return mCancel; }
			set { mCancel = value; }
		}

		/// <summary>
		/// Gets the task that is going to be executed.
		/// </summary>
		/// <value>A <see cref="ScheduledTask{T}"/>.</value>
		public ScheduledTask<T> Task
		{
			get { return mTask; }
		}
		#endregion

		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="ScheduledTaskStartingEventArgs{T}"/> class with the specified task.
		/// </summary>
		/// <param name="task">The task that is going to be executed.</param>
		public ScheduledTaskStartingEventArgs(ScheduledTask<T> task)
		{
			if (task == null)
				throw new CodedArgumentNullException(Errors.CreateHResult(0x21), nameof(task));

			mTask = task;
			mCancel = false;
		}
		#endregion
	}
}
