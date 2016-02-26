#region Copyright
/*******************************************************************************
 * <copyright file="WeekDays.cs" owner="Daniel Kopp">
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
 * <file name="WeekDays.cs" date="2016-02-02">
 * Specifies the days in a week.
 * </file>
 ******************************************************************************/
#endregion

using System;
using System.Runtime.Serialization;

namespace NerdyDuck.Scheduler
{
	/// <summary>
	/// Specifies the days in a week.
	/// </summary>
	/// <remarks>Unlike the <see cref="System.DayOfWeek"/> enumeration, the values of this enumeration can be combined.</remarks>
#if WINDOWS_DESKTOP
	[System.Serializable]
#endif
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
}
