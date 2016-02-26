#region Copyright
/*******************************************************************************
 * <copyright file="ISchedulerAction.cs" owner="Daniel Kopp">
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
 * <file name="ISchedulerAction.cs" date="2016-02-17">
 * Represents an asynchronous action executed by a Scheduler&lt;T&gt;.
 * </file>
 ******************************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NerdyDuck.Scheduler
{
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
}
