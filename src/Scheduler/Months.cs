#region Copyright
/*******************************************************************************
 * <copyright file="Months.cs" owner="Daniel Kopp">
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
 * <file name="Months.cs" date="2016-02-02">
 * Specifies the months in a year.
 * </file>
 ******************************************************************************/
#endregion

using System;
using System.Runtime.Serialization;

namespace NerdyDuck.Scheduler
{
	/// <summary>
	/// Specifies the months in a year.
	/// </summary>
#if WINDOWS_DESKTOP
	[System.Serializable]
#endif
	[DataContract(Namespace = Schedule.Namespace)]
	[Flags]
	public enum Months
	{
		/// <summary>
		/// No month selected.
		/// </summary>
		[EnumMember]
		None = 0x0000,

		/// <summary>
		/// Indicates January.
		/// </summary>
		[EnumMember]
		January = 0x0001,

		/// <summary>
		/// Indicates February.
		/// </summary>
		[EnumMember]
		February = 0x0002,

		/// <summary>
		/// Indicates March.
		/// </summary>
		[EnumMember]
		March = 0x0004,

		/// <summary>
		/// Indicates April.
		/// </summary>
		[EnumMember]
		April = 0x0008,

		/// <summary>
		/// Indicates May.
		/// </summary>
		[EnumMember]
		May = 0x0010,

		/// <summary>
		/// Indicates June.
		/// </summary>
		[EnumMember]
		June = 0x0020,

		/// <summary>
		/// Indicates July.
		/// </summary>
		[EnumMember]
		July = 0x0040,

		/// <summary>
		/// Indicates August.
		/// </summary>
		[EnumMember]
		August = 0x0080,

		/// <summary>
		/// Indicates September.
		/// </summary>
		[EnumMember]
		September = 0x0100,

		/// <summary>
		/// Indicates October.
		/// </summary>
		[EnumMember]
		October = 0x0200,

		/// <summary>
		/// Indicates November.
		/// </summary>
		[EnumMember]
		November = 0x0400,

		/// <summary>
		/// Indicates December.
		/// </summary>
		[EnumMember]
		December = 0x0800,

		/// <summary>
		/// Indicates all months.
		/// </summary>
		[EnumMember]
		All = 0x0fff
	}
}
