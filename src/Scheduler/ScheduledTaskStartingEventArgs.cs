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

namespace NerdyDuck.Scheduler;

/// <summary>
/// Provides data for the <see cref="Scheduler{T}.TaskStarting"/> event.
/// </summary>
/// <typeparam name="T">The type of action that can be executed by the task, derived from <see cref="ScheduledActionBase"/>.</typeparam>
public class ScheduledTaskStartingEventArgs<T> : EventArgs where T : ScheduledActionBase, new()
{
	/// <summary>
	/// Gets or sets a value indicating if the task execution should be canceled.
	/// </summary>
	/// <value><see langword="true"/> if the execution should be canceled; otherwise, <see langword="false"/>.</value>
	public bool Cancel { get; set; }

	/// <summary>
	/// Gets the task that is going to be executed.
	/// </summary>
	/// <value>A <see cref="ScheduledTask{T}"/>.</value>
	public ScheduledTask<T> Task { get; private set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="ScheduledTaskStartingEventArgs{T}"/> class with the specified task.
	/// </summary>
	/// <param name="task">The task that is going to be executed.</param>
	public ScheduledTaskStartingEventArgs(ScheduledTask<T> task)
	{
		Task = task ?? throw new CodedArgumentNullException(HResult.Create(ErrorCodes.ScheduledTaskStartingEventArgs_ctor_TaskNull), nameof(task));
		Cancel = false;
	}
}
