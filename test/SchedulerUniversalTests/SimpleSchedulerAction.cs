#region Copyright
/*******************************************************************************
 * <copyright file="SimpleSchedulerAction.cs" owner="Daniel Kopp">
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
 * <assembly name="NerdyDuck.Tests.Scheduler">
 * Unit tests for NerdyDuck.Scheduler assembly.
 * </assembly>
 * <file name="SimpleSchedulerAction.cs" date="2016-02-19">
 * Simple implementation of ISchedulerAction.
 * </file>
 ******************************************************************************/
#endregion

using NerdyDuck.Scheduler;
using System;
#if WINDOWS_DESKTOP
using System.Diagnostics.CodeAnalysis;
#endif


namespace NerdyDuck.Tests.Scheduler
{
	/// <summary>
	/// Simple implementation of ISchedulerAction.
	/// </summary>
#if WINDOWS_DESKTOP
	[ExcludeFromCodeCoverage]
#endif
	public class SimpleSchedulerAction : ISchedulerAction
	{
		private bool mIsCancellationRequested;

		public Exception Exception { get; set; }

		public bool IsCanceled { get; set; }

		public bool IsCancellationRequested
		{
			get { return mIsCancellationRequested; }
		}

		public bool IsSuccess { get; set; }

		public object State { get; set; }

		public SimpleSchedulerAction()
		{
			mIsCancellationRequested = false;
		}

		public void Cancel()
		{
			mIsCancellationRequested = true;
		}

		public void ThrowIfCancellationRequested()
		{
			if (mIsCancellationRequested)
				throw new OperationCanceledException();
		}
	}
}
