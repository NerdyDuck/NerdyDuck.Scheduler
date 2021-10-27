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
/// Error codes for the NerdyDuck.Scheduler assembly.
/// </summary>
internal enum ErrorCodes
{
	/// <summary>
	/// 0x0001; Schedule.ReadXml; reader is null.
	/// </summary>
	Schedule_ReadXml_ArgNull = 0x01,

	/// <summary>
	/// 0x0002; Schedule.ParseAttribute; type attribute invalid.
	/// </summary>
	Schedule_ParseAttribute_TypeInvalid,

	/// <summary>
	/// 0x0003; Schedule.ParseAttribute; weekDay or weekDays attribute invalid.
	/// </summary>
	Schedule_ParseAttribute_WeekDaysInvalid,

	/// <summary>
	/// 0x0004; Schedule.ParseAttribute; months attribute invalid.
	/// </summary>
	Schedule_ParseAttribute_MonthsInvalid,

	/// <summary>
	/// 0x0005; Schedule.ParseAttribute; weekInMonth attribute invalid.
	/// </summary>
	Schedule_ParseAttribute_WeekInMonthInvalid,

	/// <summary>
	/// 0x0006; Schedule.ReadXml; type attribute missing.
	/// </summary>
	Schedule_ReadXml_TypeMissing,

	/// <summary>
	/// 0x0007; Schedule.WriteXml; writer is null.
	/// </summary>
	Schedule_WriteXml_ArgNull,

	/// <summary>
	/// 0x0008; Schedule.AssertScheduledDateTime; scheduledDateTime is null.
	/// </summary>
	Schedule_AssertScheduledDateTime_Null,

	/// <summary>
	/// 0x0009; Schedule.AssertInterval; interval is null.
	/// </summary>
	Schedule_AssertInterval_Null,

	/// <summary>
	/// 0x000a; Schedule.AssertWeekdays; weekdays is null.
	/// </summary>
	Schedule_AssertWeekdays_Null,

	/// <summary>
	/// 0x000b; Schedule.AssertStartTime; startTime is null.
	/// </summary>
	Schedule_AssertStartTime_Null,

	/// <summary>
	/// 0x000c; Schedule.AssertDayInMonth; dayInMonth is null.
	/// </summary>
	Schedule_AssertDayInMonth_Null,

	/// <summary>
	/// 0x000d; Schedule.AssertMonths; months is null.
	/// </summary>
	Schedule_AssertMonths_Null,

	/// <summary>
	/// 0x000e; Schedule.AssertWeekInMonth; weekInMonth is null.
	/// </summary>
	Schedule_AssertWeekInMonth_Null,

	/// <summary>
	/// 0x000f; Schedule.AssertTimeSpanInterval; interval is negative.
	/// </summary>
	Schedule_AssertTimeSpanInterval_Negative,

	/// <summary>
	/// 0x0010; Schedule.AssertWeekdays; No weekday specified.
	/// </summary>
	Schedule_AssertWeekdays_NoWeekdays,

	/// <summary>
	/// 0x0011; Schedule.AssertStartTime; startTime is negative or greater than 24h.
	/// </summary>
	Schedule_AssertStartTime_OutOfRange,

	/// <summary>
	/// 0x0012; Schedule.AssertDayMonthInterval; interval is not positive.
	/// </summary>
	Schedule_AssertDayMonthInterval_NotPositive,

	/// <summary>
	/// 0x0013; Schedule.AssertDayInMonth; dayInMonth is smaller than -31 or greater than 31.
	/// </summary>
	Schedule_AssertDayInMonth_OutOfRange,

	/// <summary>
	/// 0x0014; Schedule.AssertMonths; No months specified.
	/// </summary>
	Schedule_AssertMonths_NoMonths,

	/// <summary>
	/// 0x0015; Schedule.AssertWeekDay; More than one week day specified.
	/// </summary>
	Schedule_AssertWeekDay_OutOfRange,

	/// <summary>
	/// 0x0016; ScheduledTask.ReadXml; reader is null.
	/// </summary>
	ScheduledTask_ReadXml_ArgNull,

	/// <summary>
	/// 0x0017; ScheduledTask.WriteXml; writer is null.
	/// </summary>
	ScheduledTask_WriteXml_ArgNull,

	/// <summary>
	/// 0x0018; ScheduledTask.ReadXml; schedule is null.
	/// </summary>
	ScheduledTask_ReadXml_ScheduleNull,

	/// <summary>
	/// 0x0019; ScheduledTask.ReadXml; action is null.
	/// </summary>
	ScheduledTask_ReadXml_ActionNull,

	/// <summary>
	/// 0x001a; ScheduledTask.ctor; schedule is null.
	/// </summary>
	ScheduledTask_ctor_ScheduleNull,

	/// <summary>
	/// 0x001b; ScheduledTask.ctor; action is null.
	/// </summary>
	ScheduledTask_ctor_ActionNull,

	/// <summary>
	/// 0x001c; ScheduledTask.Reschedule; schedule is null.
	/// </summary>
	ScheduledTask_Reschedule_ScheduleNull,

	/// <summary>
	/// 0x001d; ScheduledTask.AssertActive; Object is not initialized.
	/// </summary>
	ScheduledTask_AssertActive_NotInitialized,

	/// <summary>
	/// 0x001e; Scheduler.AssertInterval; interval may not be negative.
	/// </summary>
	Scheduler_AssertInterval_Negative,

	/// <summary>
	/// 0x001f; Scheduler.Stop; timeout may not be negative.
	/// </summary>
	Scheduler_Stop_Negative,

	/// <summary>
	/// 0x0020; Scheduler.Stop; timeout has elapsed, but tasks are still running.
	/// </summary>
	Scheduler_Stop_TasksRunning,

	/// <summary>
	/// 0x0021; ScheduledTaskStartingEventArgs.ctor; task is null.
	/// </summary>
	ScheduledTaskStartingEventArgs_ctor_TaskNull,

	/// <summary>
	/// 0x0022; ScheduledTaskCompletedEventArgs.ctor; task is null.
	/// </summary>
	ScheduledTaskCompletedEventArgs_ctor_TaskNull,

	/// <summary>
	/// 0x0023; Schedule.AssertInitialize; Object has not been initialized.
	/// </summary>
	Schedule_AssertInitialized_Failed,

	/// <summary>
	/// 0x0024; ScheduledTask.ReadXml; Schedule element has no attributes
	/// </summary>
	ScheduledTask_ReadXml_NoAttributes,

	/// <summary>
	/// 0x0025; Schedule.AssertProperties; ScheduleType is None.
	/// </summary>
	ScheduledTask_AssertProperties_TypeNone,
}
