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
/// Specifies the months in a year.
/// </summary>
[Serializable]
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
