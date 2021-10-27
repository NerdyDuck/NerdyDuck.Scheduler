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
/// Specifies the type of schedule for a task.
/// </summary>
[Serializable]
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
