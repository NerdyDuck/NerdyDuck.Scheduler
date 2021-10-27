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
/// Represents an asynchronous action executed by a <see cref="Scheduler{T}"/>.
/// </summary>
public interface ISchedulerAction
{
	/// <summary>
	/// When implemented, gets whether cancellation has been requested for this action.
	/// </summary>
	/// <value><see langword="true"/>, if the action should be canceled; otherwise, <see langword="false"/>.</value>
	bool IsCancellationRequested { get; }

	/// <summary>
	/// When implemented, gets or sets a value indicating if the action completed successfully.
	/// </summary>
	/// <value><see langword="true"/>, if the action completed successfully; otherwise, <see langword="false"/>.</value>
	bool IsSuccess { get; set; }

	/// <summary>
	/// When implemented, gets or sets a value indicating if the action was canceled or aborted.
	/// </summary>
	/// <value><see langword="true"/>, if the action was canceled; otherwise, <see langword="false"/>.</value>
	bool IsCanceled { get; set; }

	/// <summary>
	/// When implemented, gets or sets the exception that caused the action to fail.
	/// </summary>
	/// <value>If <see cref="IsSuccess"/> and <see cref="IsCanceled"/> is <see langword="false"/>, an <see cref="Exception"/>; otherwise, <see langword="null"/>.</value>
	Exception Exception { get; set; }

	/// <summary>
	/// When implemented, gets or sets an arbitrary state object.
	/// </summary>
	/// <value>A return value from the executed action. May be <see langword="null"/>.</value>
	object State { get; set; }

	/// <summary>
	/// When implemented, throws an exception if the action was canceled.
	/// </summary>
	/// <exception cref="OperationCanceledException"><see cref="IsCancellationRequested"/> is <see langword="true"/>.</exception>
	void ThrowIfCancellationRequested();

	/// <summary>
	/// When implemented, sets <see cref="IsCancellationRequested"/> to <see langword="true"/>.
	/// </summary>
	void Cancel();
}
