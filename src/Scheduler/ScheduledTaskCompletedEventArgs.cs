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
/// Provides data for the <see cref="Scheduler{T}.TaskCompleted"/> event.
/// </summary>
/// <typeparam name="T">The type of action that can be executed by the task, derived from <see cref="ScheduledActionBase"/>.</typeparam>
public class ScheduledTaskCompletedEventArgs<T> : System.EventArgs where T : ScheduledActionBase, new()
{
	/// <summary>
	/// Gets the task that was executed.
	/// </summary>
	/// <value>A <see cref="ScheduledTask{T}"/>.</value>
	/// <value>The task that just executed.</value>
	public ScheduledTask<T> Task { get; private set; }

	/// <summary>
	/// Gets a value indicating if the <see cref="Task"/> completed successfully.
	/// </summary>
	/// <value><see langword="true"/>, if the <see cref="Task"/> completed successfully; otherwise, <see langword="false"/>.</value>
	public bool IsSuccess { get; private set; }

	/// <summary>
	/// Gets a value indicating if the <see cref="Task"/> was canceled or aborted.
	/// </summary>
	/// <value><see langword="true"/>, if the <see cref="Task"/> was canceled; otherwise, <see langword="false"/>.</value>
	public bool IsCanceled { get; private set; }

	/// <summary>
	/// Gets the exception that caused the action to fail.
	/// </summary>
	/// <value>If <see cref="IsSuccess"/> and <see cref="IsCanceled"/> is <see langword="false"/>, an <see cref="Exception"/>; otherwise, <see langword="null"/>.</value>
	public Exception Exception { get; private set; }

	/// <summary>
	/// Gets an arbitrary state object.
	/// </summary>
	/// <value>The return value of the executed task. May be <see langword="null"/>.</value>
	public object? State { get; private set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="ScheduledTaskCompletedEventArgs{T}"/> with the specified parameters.
	/// </summary>
	/// <param name="task">The task that was executed.</param>
	/// <param name="isSuccess">A value indicating if the <paramref name="task"/> completed successfully.</param>
	/// <param name="isCanceled">A value indicating if the <paramref name="task"/> was canceled or aborted.</param>
	/// <param name="ex">The exception that caused the action to fail.</param>
	/// <param name="state">An arbitrary state object.</param>
	/// <exception cref="CodedArgumentNullException"><paramref name="task"/> is <see langword="null" /></exception>
	public ScheduledTaskCompletedEventArgs(ScheduledTask<T> task, bool isSuccess, bool isCanceled, Exception ex, object state)
	{
		Task = task ?? throw new CodedArgumentNullException(HResult.Create(ErrorCodes.ScheduledTaskCompletedEventArgs_ctor_TaskNull), nameof(task));
		IsSuccess = isSuccess;
		IsCanceled = isCanceled;
		Exception = ex;
		State = state;
	}
}
