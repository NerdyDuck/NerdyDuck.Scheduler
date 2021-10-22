#region Copyright
/*******************************************************************************
 * <copyright file="ScheduleType.cs" owner="Daniel Kopp">
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
 * <file name="ScheduleType.cs" date="2016-02-02">
 * Specifies the type of schedule for a task.
 * </file>
 ******************************************************************************/
#endregion

using System;
using System.Runtime.Serialization;

namespace NerdyDuck.Scheduler
{
	/// <summary>
	/// Specifies the type of schedule for a task.
	/// </summary>
#if WINDOWS_DESKTOP
	[System.Serializable]
#endif
	[DataContract(Namespace = Schedule.Namespace)]
	public enum ScheduleType
	{
		/// <summary>
		/// The task will not run. Do not use.
		/// </summary>
		[EnumMember]
		None = 0,

		/// <summary>
		/// The task will only run once.
		/// </summary>
		[EnumMember]
		OneTime = 1,

		/// <summary>
		/// The task will run daily (or after a set number of days) at a specific time.
		/// </summary>
		[EnumMember]
		Daily = 2,

		/// <summary>
		/// The task will run weekly (or after a set number of weeks) at a specified weekday and time.
		/// </summary>
		[EnumMember]
		Weekly = 3,

		/// <summary>
		/// The task will run every month (or a selection of months) at a specified weekday (or its n-th occurrence in the month) and time.
		/// </summary>
		[EnumMember]
		MonthlyWeekday = 4,

		/// <summary>
		/// The task will run every month (or a selection of months) at a specified day of the month, and time.
		/// </summary>
		[EnumMember]
		MonthlyDay = 5,

		/// <summary>
		/// The task will run in a specified interval, at specified weekdays.
		/// </summary>
		[EnumMember]
		Interval = 6
	}
}
