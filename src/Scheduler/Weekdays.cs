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
/// Specifies the days in a week.
/// </summary>
/// <remarks>Unlike the <see cref="DayOfWeek"/> enumeration, the values of this enumeration can be combined.</remarks>
[Serializable]
[DataContract(Namespace = Schedule.Namespace)]
[Flags]
public enum Weekdays
{
	/// <summary>
	/// No weekday specified.
	/// </summary>
	[EnumMember]
	None = 0x00,

	/// <summary>
	/// Monday is specified.
	/// </summary>
	[EnumMember]
	Monday = 0x01,

	/// <summary>
	/// Tuesday is specified.
	/// </summary>
	[EnumMember]
	Tuesday = 0x02,

	/// <summary>
	/// Wednesday is specified.
	/// </summary>
	[EnumMember]
	Wednesday = 0x04,

	/// <summary>
	/// Thursday is specified.
	/// </summary>
	[EnumMember]
	Thursday = 0x08,

	/// <summary>
	/// Friday is specified.
	/// </summary>
	[EnumMember]
	Friday = 0x10,

	/// <summary>
	/// Saturday is specified.
	/// </summary>
	[EnumMember]
	Saturday = 0x20,

	/// <summary>
	/// Sunday is specified.
	/// </summary>
	[EnumMember]
	Sunday = 0x40,

	/// <summary>
	/// Monday through Friday is specified.
	/// </summary>
	[EnumMember]
	MondayToFriday = 0x1f,

	/// <summary>
	/// Monday through Saturday is specified.
	/// </summary>
	[EnumMember]
	MondayToSaturday = 0x3f,

	/// <summary>
	/// Saturday and Sunday is specified.
	/// </summary>
	[EnumMember]
	Weekend = 0x60,

	/// <summary>
	/// All days are specified.
	/// </summary>
	[EnumMember]
	All = 0x7f
}
