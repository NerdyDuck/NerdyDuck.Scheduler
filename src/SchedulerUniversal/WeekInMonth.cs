#region Copyright
/*******************************************************************************
 * <copyright file="WeekInMonth.cs" owner="Daniel Kopp">
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
 * <file name="WeekInMonth.cs" date="2016-02-02">
 * Specifies the position of a week day within a month.
 * </file>
 ******************************************************************************/
#endregion

using System;
using System.Runtime.Serialization;

namespace NerdyDuck.Scheduler
{
	/// <summary>
	/// Specifies the position of a week day within a month.
	/// </summary>
#if WINDOWS_DESKTOP
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")]
	[System.Serializable]
#endif
	[DataContract(Namespace = Schedule.Namespace)]
	public enum WeekInMonth
	{
		/// <summary>
		/// The first occurrence of a weekday in a month.
		/// </summary>
		[EnumMember]
		First = 1,

		/// <summary>
		/// The second occurrence of a weekday in a month.
		/// </summary>
		[EnumMember]
		Second = 2,

		/// <summary>
		/// The third occurrence of a weekday in a month.
		/// </summary>
		[EnumMember]
		Third = 3,

		/// <summary>
		/// The fourth occurrence of a weekday in a month.
		/// </summary>
		[EnumMember]
		Fourth = 4,

		/// <summary>
		/// The last occurrence of a weekday in a month.
		/// </summary>
		[EnumMember]
		Last = 5
	}
}
