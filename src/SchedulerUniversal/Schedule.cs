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
 * The schedule that triggers a ScheduledTask&lt;T&gt.
 * </file>
 ******************************************************************************/
#endregion

using NerdyDuck.CodedExceptions;
using System;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace NerdyDuck.Scheduler
{
	/// <summary>
	/// The schedule that triggers a <see cref="ScheduledTask{T}"/>.
	/// </summary>
#if WINDOWS_DESKTOP
	[Serializable]
#endif
	[XmlRoot(ElementName = RootName, Namespace = Schedule.Namespace)]
	public sealed class Schedule : IXmlSerializable
#if WINDOWS_DESKTOP
		, ISerializable
#endif
	{
		#region Constants
		internal const string Namespace = "http://www.nerdyduck.de/Scheduler";
		private const string RootName = "schedule";
		private const string TypeName = "type";
		private const string ScheduledDateTimeName = "scheduledDateTime";
		private const string IntervalName = "interval";
		private const string WeekdayName = "weekday";
		private const string WeekdaysName = "weekdays";
		private const string StartTimeName = "startTime";
		private const string IntervalDaysName = "intervalDays";
		private const string IntervalWeeksName = "intervalWeeks";
		private const string DayInMonthName = "dayInMonth";
		private const string MonthsName = "months";
		private const string WeekInMonthName = "weekInMonth";
		#endregion

		#region Private fields
		private bool IsInitialized;
		private ScheduleType mScheduleType;
		private DateTimeOffset? mScheduledDateTime;
		private TimeSpan? mStartTimeOrInterval;
		private Weekdays? mWeekdays;
		private short? mDayInMonthOrInterval;
		private Months? mMonths;
		private WeekInMonth? mWeekInMonth;
		private static readonly TimeSpan OneDay = new TimeSpan(1, 0, 0, 0);
		#endregion

		#region Properties
		/// <summary>
		/// Gets the type of the <see cref="Schedule"/>.
		/// </summary>
		/// <value>One of the <see cref="Scheduler.ScheduleType"/> values.</value>
		public ScheduleType ScheduleType
		{
			get { return mScheduleType; }
		}

		/// <summary>
		/// Gets the date and time that a schedule of type <see cref="ScheduleType.OneTime"/> is due,
		/// or a schedule of type <see cref="ScheduleType.Interval"/> is due for the first time.
		/// </summary>
		/// <value>A date and time, if <see cref="ScheduleType"/> is <see cref="ScheduleType.OneTime"/> or <see cref="ScheduleType.Interval"/>; <see langword="null"/>, otherwise.</value>
		public DateTimeOffset? ScheduledDateTime
		{
			get { return mScheduledDateTime; }
		}

		/// <summary>
		/// Gets the time of day when a schedule not of type <see cref="ScheduleType.OneTime"/> is due,
		/// or the interval in which a schedule of type <see cref="ScheduleType.Interval"/> is due.
		/// </summary>
		/// <value>A <see cref="TimeSpan"/> smaller than 24 hours; <see langword="null"/>, if <see cref="ScheduleType"/> is <see cref="ScheduleType.OneTime"/>.</value>
		public TimeSpan? StartTimeOrInterval
		{
			get { return mStartTimeOrInterval; }
		}

		/// <summary>
		/// Gets the day(s) when a daily, weekly or monthly schedule is due.
		/// Only schedules of type <see cref="ScheduleType.Daily"/> allow the combination of multiple
		/// <see cref="Weekdays"/> values, weekly and monthly schedules only allow a single value.
		/// </summary>
		/// <value>One or more values of the <see cref="Weekdays"/> enumeration, if <see cref="ScheduleType"/> is <see cref="ScheduleType.Daily"/>; A single value if <see cref="ScheduleType"/> is <see cref="ScheduleType.Weekly"/> or <see cref="ScheduleType.MonthlyWeekday"/>; otherwise <see langword="null"/>.</value>
		public Weekdays? Weekdays
		{
			get { return mWeekdays; }
		}

		/// <summary>
		/// Gets the calendrical day in a month when a schedule is due,
		/// or the interval in days or weeks after which a schedule of type <see cref="ScheduleType.Daily"/>
		/// or <see cref="ScheduleType.Weekly"/> is due.
		/// </summary>
		/// <value>If <see cref="ScheduleType"/> is <see cref="ScheduleType.MonthlyDay"/>, a positive value means the
		/// calendrical day in the month, 0 means the last day in the month, and negative values mean the last day
		/// in the month minus the value (second to last, and so on). If <see cref="ScheduleType"/> is <see cref="ScheduleType.Daily"/> or <see cref="ScheduleType.Weekly"/>, a positive value specifying the interval of days or weeks; otherwise, <see langword="null"/>.</value>
		public short? DayInMonthOrInterval
		{
			get { return mDayInMonthOrInterval; }
		}

		/// <summary>
		/// Gets the month(s) when a monthly schedule is due.
		/// </summary>
		/// <value>One or more values of the <see cref="Months"/> enumeration, if <see cref="ScheduleType"/> is <see cref="ScheduleType.MonthlyDay"/> or <see cref="ScheduleType.MonthlyWeekday"/>; otherwise, <see langword="null"/>.</value>
		public Months? Months
		{
			get { return mMonths; }
		}

		/// <summary>
		/// Gets an enumeration indicating at which week in a month a schedule is due.
		/// </summary>
		/// <value>One of the <see cref="WeekInMonth"/> values, if <see cref="ScheduleType"/> is <see cref="ScheduleType.MonthlyWeekday"/>; otherwise, <see langword="null"/>.</value>
		public WeekInMonth? WeekInMonth
		{
			get { return mWeekInMonth; }
		}
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="Schedule"/> class.
		/// </summary>
		/// <remarks>The resulting instance is not configured and will not work properly. This constructor is thought to be used in combination with XML serialization only.</remarks>
		public Schedule()
		{
			mScheduleType = ScheduleType.OneTime;
			mScheduledDateTime = null;
			mStartTimeOrInterval = null;
			mWeekdays = null;
			mDayInMonthOrInterval = null;
			mMonths = null;
			mWeekInMonth = null;
			IsInitialized = false;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Schedule"/> class with the specified parameters.
		/// </summary>
		/// <param name="type">The type of the <see cref="Schedule"/>.</param>
		/// <param name="scheduledDateTime">The date and time that a schedule of type <see cref="ScheduleType.OneTime"/> is due, or a schedule of type <see cref="ScheduleType.Interval"/> is due for the first time.</param>
		/// <param name="startTimeOrInterval">The time of day when a schedule not of type <see cref="ScheduleType.OneTime"/> is due, or the interval in which a schedule of type <see cref="ScheduleType.Interval"/> is due.</param>
		/// <param name="weekdays">The day(s) when a daily, weekly or monthly schedule is due. Only schedules of type <see cref="ScheduleType.Daily"/> allow the combination of multiple <see cref="Weekdays"/> values, weekly and monthly schedules only allow a single value.</param>
		/// <param name="dayInMonthOrInterval">The calendrical day in a month when a schedule is due, or the interval in days or weeks after which a schedule of type <see cref="ScheduleType.Daily"/> or <see cref="ScheduleType.Weekly"/> is due.</param>
		/// <param name="months">The month(s) when a monthly schedule is due.</param>
		/// <param name="weekInMonth">An enumeration indicating at which week in a month a schedule is due.</param>
		/// <exception cref="CodedArgumentNullException">At least one argument required for a schedule specified in <paramref name="type"/> is <see langword="null"/>.</exception>
		/// <exception cref="CodedArgumentOutOfRangeException">An argument required for a schedule specified in <paramref name="type"/> has an invalid value.</exception>
		public Schedule(ScheduleType type, DateTimeOffset? scheduledDateTime, TimeSpan? startTimeOrInterval, Weekdays? weekdays, short? dayInMonthOrInterval, Months? months, WeekInMonth? weekInMonth)
		{
			mScheduleType = type;
			mScheduledDateTime = scheduledDateTime;
			mStartTimeOrInterval = startTimeOrInterval;
			mWeekdays = weekdays;
			mDayInMonthOrInterval = dayInMonthOrInterval;
			mMonths = months;
			mWeekInMonth = weekInMonth;
			IsInitialized = false;
			AssertProperties();
		}

#if WINDOWS_DESKTOP
		/// <summary>
		/// Initializes a new instance of the <see cref="Schedule"/> class with serialized data.
		/// </summary>
		/// <param name="info">The object that holds the serialized object data.</param>
		/// <param name="context">The contextual information about the source or destination.</param>
		/// <exception cref="ArgumentNullException">The <paramref name="info"/> argument is <see langword="null"/>.</exception>
		/// <exception cref="System.Runtime.Serialization.SerializationException">The object could not be deserialized correctly.</exception>
		private Schedule(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException(nameof(info));
			}
			mScheduleType = (ScheduleType)info.GetValue(nameof(ScheduleType), typeof(ScheduleType));
			mScheduledDateTime = (DateTimeOffset?)info.GetValue(nameof(ScheduledDateTime), typeof(DateTimeOffset?));
			mStartTimeOrInterval = (TimeSpan?)info.GetValue(nameof(StartTimeOrInterval), typeof(TimeSpan?));
			mWeekdays = (Weekdays?)info.GetValue(nameof(Weekdays), typeof(Weekdays?));
			mDayInMonthOrInterval = (short?)info.GetValue(nameof(DayInMonthOrInterval), typeof(short?));
			mMonths = (Months?)info.GetValue(nameof(Months), typeof(Months?));
			mWeekInMonth = (WeekInMonth?)info.GetValue(nameof(WeekInMonth), typeof(WeekInMonth?));
		}
#endif
#endregion

		#region Public methods
		#region Create methods
		#region Create
		/// <summary>
		/// Create a schedule based on the <paramref name="type"/> of schedule specified.
		/// </summary>
		/// <param name="type">The type of <see cref="Schedule"/> to create.</param>
		/// <param name="scheduledDateTime">The date and time when the schedule is due (for the first time).</param>
		/// <param name="startTimeOrInterval">The time of the day when the schedule is due, or the time span between two runs.</param>
		/// <param name="weekdays">The days in the week where the schedule is active, or the week day when the schedule is due.</param>
		/// <param name="dayInMonthOrInterval">The day of the month when the schedule is due or the number of days between two runs.</param>
		/// <param name="months">The month(s) in which the schedule is due.</param>
		/// <param name="weekInMonth">A value indicating in which week of the month the schedule is due.</param>
		/// <returns>A <see cref="Schedule"/>.</returns>
		/// <remarks>Use this method if you recreate a trigger from an external source other than the XML representation of a <see cref="Schedule"/>.</remarks>
		/// <exception cref="CodedArgumentNullException">At least one argument required for a schedule specified in <paramref name="type"/> is <see langword="null"/>.</exception>
		/// <exception cref="CodedArgumentOutOfRangeException">An argument required for a schedule specified in <paramref name="type"/> has an invalid value.</exception>
		public static Schedule Create(ScheduleType type, DateTimeOffset? scheduledDateTime, TimeSpan? startTimeOrInterval, Weekdays? weekdays, short? dayInMonthOrInterval, Months? months, WeekInMonth? weekInMonth)
		{
			return new Schedule(type, scheduledDateTime, startTimeOrInterval, weekdays, dayInMonthOrInterval, months, weekInMonth);
		}
		#endregion

		#region CreateOneTime
		/// <summary>
		/// Creates a schedule that is run only once.
		/// </summary>
		/// <param name="scheduledDateTime">The date and time when the schedule is due.</param>
		/// <returns>A <see cref="Schedule"/>.</returns>
		public static Schedule CreateOneTime(DateTimeOffset scheduledDateTime)
		{
			return new Schedule(ScheduleType.OneTime, scheduledDateTime, null, null, null, null, null);
		}
		#endregion

		#region CreateInterval
		/// <summary>
		/// Creates a schedule with a fixed interval.
		/// </summary>
		/// <param name="scheduledDateTime">The date and time when the schedule is due for the first time.</param>
		/// <param name="interval">The time span between two runs.</param>
		/// <param name="weekdays">The days in the week where the schedule is active. If the next regular due date is not in the list of week days, then the next week day in the list is chosen.</param>
		/// <returns>A <see cref="Schedule"/>.</returns>
		/// <exception cref="CodedArgumentOutOfRangeException"><paramref name="weekdays"/> is <see cref="Weekdays.None"/> or <paramref name="interval"/> is negative.</exception>
		public static Schedule CreateInterval(DateTimeOffset scheduledDateTime, TimeSpan interval, Weekdays weekdays)
		{
			return new Schedule(ScheduleType.Interval, scheduledDateTime, interval, weekdays, null, null, null);
		}
		#endregion

		#region CreateDaily
		/// <summary>
		/// Creates a daily schedule.
		/// </summary>
		/// <param name="startTime">The time of the day when the schedule is due.</param>
		/// <param name="interval">The number of days between two runs. Must be a positive value.</param>
		/// <param name="weekdays">The days in the week where the schedule is active. If the next regular due date is not in the list of week days, then the next week day in the list is chosen.</param>
		/// <returns>A <see cref="Schedule"/>.</returns>
		/// <exception cref="CodedArgumentOutOfRangeException"><paramref name="weekdays"/> is <see cref="Weekdays.None"/>, or <paramref name="interval"/> is not positive, or <paramref name="startTime"/> is negative.</exception>
		public static Schedule CreateDaily(TimeSpan startTime, short interval, Weekdays weekdays)
		{
			return new Schedule(ScheduleType.Daily, null, startTime, weekdays, interval, null, null);
		}
		#endregion

		#region CreateWeekly
		/// <summary>
		/// Creates a weekly schedule.
		/// </summary>
		/// <param name="startTime">The time of the day when the schedule is due.</param>
		/// <param name="interval">The number of weeks between two runs.</param>
		/// <param name="weekday">The week day when the schedule is due. Only one single day is allowed.</param>
		/// <returns>A <see cref="Schedule"/>.</returns>
		/// <exception cref="CodedArgumentOutOfRangeException"><paramref name="weekday"/> is <see cref="Weekdays.None"/> or contains more than a single day; or <paramref name="interval"/> is not positive, or <paramref name="startTime"/> is negative.</exception>
		public static Schedule CreateWeekly(TimeSpan startTime, short interval, Weekdays weekday)
		{
			return new Schedule(ScheduleType.Weekly, null, startTime, weekday, interval, null, null);
		}
		#endregion

		#region CreateMonthlyDay
		/// <summary>
		/// Creates a monthly schedule with a fixed due day.
		/// </summary>
		/// <param name="startTime">The time of the day when the schedule is due.</param>
		/// <param name="dayInMonth">The day of the month when the schedule is due. 0 means the last day of the month. A negative value means the last day minus the specified value.</param>
		/// <param name="months">The month(s) in which the schedule is due.</param>
		/// <returns>A <see cref="Schedule"/>.</returns>
		/// <exception cref="CodedArgumentOutOfRangeException"><paramref name="dayInMonth"/> is larger than 31 or smaller than -31, or <paramref name="startTime"/> is negative, or <paramref name="months"/> is <see cref="Months.None"/>.</exception>
		public static Schedule CreateMonthlyDay(TimeSpan startTime, short dayInMonth, Months months)
		{
			return new Schedule(ScheduleType.MonthlyDay, null, startTime, null, dayInMonth, months, null);
		}
		#endregion

		#region CreateMonthlyWeekday
		/// <summary>
		/// Creates a monthly schedule where the task is run on a specified weekday of a specified week.
		/// </summary>
		/// <param name="startTime">The time of the day when the schedule is due.</param>
		/// <param name="weekday">The week day when the schedule is due. Only one value is allowed.</param>
		/// <param name="months">The month(s) in which the schedule is due.</param>
		/// <param name="weekInMonth">A value indicating in which week of the month the schedule is due.</param>
		/// <returns>A <see cref="Schedule"/>.</returns>
		/// <exception cref="CodedArgumentOutOfRangeException"><paramref name="startTime"/> is negative, or <paramref name="months"/> is <see cref="Months.None"/>, or <paramref name="weekday"/> is either <see cref="Weekdays.None"/> or contains more than a single day.</exception>
		public static Schedule CreateMonthlyWeekday(TimeSpan startTime, Weekdays weekday, Months months, WeekInMonth weekInMonth)
		{
			return new Schedule(ScheduleType.MonthlyWeekday, null, startTime, weekday, null, months, weekInMonth);
		}
		#endregion
		#endregion

		#region GetNextDueDate
		/// <summary>
		/// Calculates the date and time when the task is due (again).
		/// </summary>
		/// <param name="currentDate">The current date and time.</param>
		/// <param name="lastExecuted">The date and time when the task was last executed. May be <see langword="null"/>.</param>
		/// <returns>A <see cref="DateTimeOffset"/> structure with the next date due.</returns>
		/// <remarks><paramref name="currentDate"/> is required as a parameter, so multiple <see cref="Schedule"/>s can be examined with exactly the same time.</remarks>
		public DateTimeOffset GetNextDueDate(DateTimeOffset currentDate, DateTimeOffset? lastExecuted)
		{
			AssertInitialized();
			switch (mScheduleType)
			{
				case ScheduleType.Daily:
					return CalculateDaily(currentDate, lastExecuted);
				case ScheduleType.Interval:
					return CalculateInterval(currentDate, lastExecuted);
				case ScheduleType.Weekly:
					return CalculateWeekly(currentDate, lastExecuted);
				case ScheduleType.MonthlyDay:
					return CalculateMonthlyDay(currentDate);
				case ScheduleType.MonthlyWeekday:
					return CalculateMonthlyWeekDay(currentDate);
				default: //ScheduleType.OneTime
					return CalculateOneTime(currentDate, lastExecuted);
			}
		}
		#endregion

		#region IXmlSerializable implementation
		/// <summary>
		/// Reserved method.
		/// </summary>
		/// <returns>Always returns <see langword="null"/>.</returns>
		public XmlSchema GetSchema()
		{
			return null;
		}

		/// <summary>
		/// Reads the <see cref="Schedule"/> configuration from the specified <paramref name="reader"/>.
		/// </summary>
		/// <param name="reader">A <see cref="XmlReader"/> containing a serialized instance of a <see cref="Schedule"/>.</param>
		public void ReadXml(XmlReader reader)
		{
			if (reader == null)
			{
				throw new CodedArgumentNullException(Errors.CreateHResult(0x01), nameof(reader));
			}

			if (!reader.MoveToFirstAttribute())
			{
				throw new CodedXmlException(Errors.CreateHResult(0x22), Properties.Resources.Schedule_ReadXml_NoAttributes);
			}

			bool TypeFound = false;
			do
			{
				if (ParseAttribute(reader.Name, reader.Value))
				{
					TypeFound = true;
				}
			} while (reader.MoveToNextAttribute());

			if (!TypeFound)
			{
				throw new CodedXmlException(Errors.CreateHResult(0x06), string.Format(Properties.Resources.Schedule_ReadXml_MissingAttribute, TypeName));
			}

			AssertProperties();
		}

		/// <summary>
		/// Writes the configuration of the <see cref="Schedule"/> to the specified <paramref name="writer"/>.
		/// </summary>
		/// <param name="writer">A <see cref="XmlWriter"/> that receives the configuration data.</param>
		public void WriteXml(XmlWriter writer)
		{
			if (writer == null)
			{
				throw new CodedArgumentNullException(Errors.CreateHResult(0x07), nameof(writer));
			}

			AssertInitialized();

			writer.WriteAttributeString(TypeName, Enum.GetName(typeof(ScheduleType), mScheduleType));

			switch (mScheduleType)
			{
				case ScheduleType.OneTime:
					writer.WriteAttributeString(ScheduledDateTimeName, XmlConvert.ToString(mScheduledDateTime.Value));
					break;
				case ScheduleType.Interval:
					writer.WriteAttributeString(ScheduledDateTimeName, XmlConvert.ToString(mScheduledDateTime.Value));
					writer.WriteAttributeString(IntervalName, XmlConvert.ToString(mStartTimeOrInterval.Value));
					writer.WriteAttributeString(WeekdaysName, XmlConvert.ToString((int)mWeekdays.Value));
					break;
				case ScheduleType.Daily:
					writer.WriteAttributeString(StartTimeName, XmlConvert.ToString(mStartTimeOrInterval.Value));
					writer.WriteAttributeString(IntervalDaysName, XmlConvert.ToString(mDayInMonthOrInterval.Value));
					writer.WriteAttributeString(WeekdaysName, XmlConvert.ToString((int)mWeekdays.Value));
					break;
				case ScheduleType.Weekly:
					writer.WriteAttributeString(StartTimeName, XmlConvert.ToString(mStartTimeOrInterval.Value));
					writer.WriteAttributeString(IntervalWeeksName, XmlConvert.ToString(mDayInMonthOrInterval.Value));
					writer.WriteAttributeString(WeekdayName, XmlConvert.ToString((int)mWeekdays.Value));
					break;
				case ScheduleType.MonthlyDay:
					writer.WriteAttributeString(StartTimeName, XmlConvert.ToString(mStartTimeOrInterval.Value));
					writer.WriteAttributeString(DayInMonthName, XmlConvert.ToString(mDayInMonthOrInterval.Value));
					writer.WriteAttributeString(MonthsName, XmlConvert.ToString((int)mMonths.Value));
					break;
				case ScheduleType.MonthlyWeekday:
					writer.WriteAttributeString(StartTimeName, XmlConvert.ToString(mStartTimeOrInterval.Value));
					writer.WriteAttributeString(WeekInMonthName, Enum.GetName(typeof(WeekInMonth), mWeekInMonth));
					writer.WriteAttributeString(WeekdayName, XmlConvert.ToString((int)mWeekdays.Value));
					writer.WriteAttributeString(MonthsName, XmlConvert.ToString((int)mMonths.Value));
					break;
			}
		}
		#endregion

#if WINDOWS_DESKTOP
		#region ISerializable implementation
		/// <summary>
		/// Populates a <see cref="SerializationInfo"/> with the data needed to serialize the target object.
		/// </summary>
		/// <param name="info">The <see cref="SerializationInfo"/> to populate with data.</param>
		/// <param name="context">The destination for this serialization.</param>
		/// <exception cref="ArgumentNullException">The <paramref name="info"/> argument is <see langword="null"/>.</exception>
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException(nameof(info));
			}
			info.AddValue(nameof(ScheduleType), mScheduleType);
			info.AddValue(nameof(ScheduledDateTime), mScheduledDateTime);
			info.AddValue(nameof(StartTimeOrInterval), mStartTimeOrInterval);
			info.AddValue(nameof(Weekdays), mWeekdays);
			info.AddValue(nameof(DayInMonthOrInterval), mDayInMonthOrInterval);
			info.AddValue(nameof(Months), mMonths);
			info.AddValue(nameof(WeekInMonth), mWeekInMonth);
		}
		#endregion
#endif
#endregion

		#region Private methods
		#region Asserts
		#region AssertProperties
		/// <summary>
		/// Asserts that all properties required for the schedule specified in <see cref="ScheduleType"/> are filled and meaningful.
		/// </summary>
		private void AssertProperties()
		{
			switch (mScheduleType)
			{
				case ScheduleType.Interval:
					AssertScheduledDateTime();
					AssertTimeSpanInterval();
					AssertWeekdays();
					break;
				case ScheduleType.Daily:
					AssertStartTime();
					AssertDayMonthInterval();
					AssertWeekdays();
					break;
				case ScheduleType.Weekly:
					AssertStartTime();
					AssertDayMonthInterval();
					AssertWeekday();
					break;
				case ScheduleType.MonthlyDay:
					AssertStartTime();
					AssertDayInMonth();
					AssertMonths();
					break;
				case ScheduleType.MonthlyWeekday:
					AssertStartTime();
					AssertWeekInMonth();
					AssertWeekday();
					AssertMonths();
					break;
				case ScheduleType.OneTime:
					AssertScheduledDateTime();
					break;
				default:
					throw new CodedInvalidOperationException(Errors.CreateHResult(0x1d), Properties.Resources.Global_ScheduleType_None);
			}
			IsInitialized = true;
		}
		#endregion

		#region AssertScheduledDateTime
		/// <summary>
		/// Asserts that <see cref="ScheduledDateTime"/> has a value.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
		private void AssertScheduledDateTime()
		{
			if (!mScheduledDateTime.HasValue)
			{
				throw new CodedArgumentNullException(Errors.CreateHResult(0x08), ScheduledDateTimeName);
			}
		}
		#endregion

		#region AssertTimeSpanInterval
		/// <summary>
		/// Asserts that <see cref="StartTimeOrInterval"/> has a value usable as an interval.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
		private void AssertTimeSpanInterval()
		{
			if (!mStartTimeOrInterval.HasValue)
			{
				throw new CodedArgumentNullException(Errors.CreateHResult(0x09), IntervalName);
			}
			if (mStartTimeOrInterval.Value <= TimeSpan.Zero)
			{
				throw new CodedArgumentOutOfRangeException(Errors.CreateHResult(0x0f), IntervalName, Properties.Resources.Schedule_AssertTimeSpanInterval);
			}
		}

		#endregion

		#region AssertDayMonthInterval
		/// <summary>
		/// Asserts that <see cref="DayInMonthOrInterval"/> has a value usable as an interval.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
		private void AssertDayMonthInterval()
		{
			if (!mDayInMonthOrInterval.HasValue)
			{
				throw new CodedArgumentNullException(Errors.CreateHResult(0x09), IntervalName);
			}
			if (mDayInMonthOrInterval.Value < 1)
			{
				throw new CodedArgumentOutOfRangeException(Errors.CreateHResult(0x12), IntervalName, Properties.Resources.Schedule_AssertDayMonthInterval);
			}
		}
		#endregion

		#region AssertDayInMonth
		/// <summary>
		/// Asserts that <see cref="DayInMonthOrInterval"/> has a value usable as a day in a month.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
		private void AssertDayInMonth()
		{
			if (!mDayInMonthOrInterval.HasValue)
			{
				throw new CodedArgumentNullException(Errors.CreateHResult(0x0c), DayInMonthName);
			}
			if (mDayInMonthOrInterval.Value < -31 || mDayInMonthOrInterval.Value > 31)
			{
				throw new CodedArgumentOutOfRangeException(Errors.CreateHResult(0x13), DayInMonthName, Properties.Resources.Schedule_AssertDayInMonth);
			}
		}
		#endregion

		#region AssertWeekdays
		/// <summary>
		/// Asserts that <see cref="Weekdays"/> has one or more weekdays set.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
		private void AssertWeekdays()
		{
			if (!mWeekdays.HasValue)
			{
				throw new CodedArgumentNullException(Errors.CreateHResult(0x0a), WeekdaysName);
			}
			if (mWeekdays.Value == Scheduler.Weekdays.None)
			{
				throw new CodedArgumentOutOfRangeException(Errors.CreateHResult(0x10), WeekdaysName, Properties.Resources.Schedule_AssertWeekDay_NoWeekDay);
			}
		}
		#endregion

		#region AssertWeekday
		/// <summary>
		/// Asserts that <see cref="Weekdays"/> has exactly one weekday set.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
		private void AssertWeekday()
		{
			if (!mWeekdays.HasValue)
			{
				throw new CodedArgumentNullException(Errors.CreateHResult(0x0a), WeekdaysName);
			}
			if (mWeekdays.Value == NerdyDuck.Scheduler.Weekdays.None)
			{
				throw new CodedArgumentOutOfRangeException(Errors.CreateHResult(0x10), WeekdaysName, Properties.Resources.Schedule_AssertWeekDay_NoWeekDay);
			}
			if (!OnlyOneFlagSet((int)mWeekdays.Value))
			{
				throw new CodedArgumentOutOfRangeException(Errors.CreateHResult(0x15), IntervalName, Properties.Resources.Schedule_AssertWeekDay_TooManyWeekDays);
			}
		}
		#endregion

		#region AssertStartTime
		/// <summary>
		/// Asserts that <see cref="StartTimeOrInterval"/> has a value suitable as a time in a day.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
		private void AssertStartTime()
		{
			if (!mStartTimeOrInterval.HasValue)
			{
				throw new CodedArgumentNullException(Errors.CreateHResult(0x0b), StartTimeName);
			}
			if (mStartTimeOrInterval.Value < TimeSpan.Zero || mStartTimeOrInterval.Value >= OneDay)
			{
				throw new CodedArgumentOutOfRangeException(Errors.CreateHResult(0x11), StartTimeName, Properties.Resources.Schedule_AssertStartTime);
			}
		}
		#endregion

		#region AssertMonths
		/// <summary>
		/// Asserts that <see cref="Months"/> has at least one month set.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
		private void AssertMonths()
		{
			if (!mMonths.HasValue)
			{
				throw new CodedArgumentNullException(Errors.CreateHResult(0x0d), MonthsName);
			}
			if (mMonths.Value == NerdyDuck.Scheduler.Months.None)
			{
				throw new CodedArgumentOutOfRangeException(Errors.CreateHResult(0x14), MonthsName, Properties.Resources.Schedule_AssertMonths_NoMonths);
			}
		}
		#endregion

		#region AssertWeekInMonth
		/// <summary>
		/// Asserts that <see cref="WeekInMonth"/> in set.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
		private void AssertWeekInMonth()
		{
			if (!mWeekInMonth.HasValue)
			{
				throw new CodedArgumentNullException(Errors.CreateHResult(0x0e), WeekInMonthName);
			}
		}
		#endregion

		#region AssertInitialized
		/// <summary>
		/// Asserts that the <see cref="Schedule"/> was initialized with meaningful values.
		/// </summary>
		private void AssertInitialized()
		{
			if (!IsInitialized)
			{
				throw new CodedInvalidOperationException(Errors.CreateHResult(0x16), Properties.Resources.Schedule_AssertInitialized);
			}
		}
		#endregion
		#endregion

		#region Calculates
		#region CalculateOneTime
		/// <summary>
		/// Calculates the schedule for a one-time event.
		/// </summary>
		/// <param name="currentDate">The current date and time to use for the calculation.</param>
		/// <param name="lastExecuted">The last date and time the task was executed. May be <see langword="null"/>.</param>
		/// <returns>A <see cref="DateTimeOffset"/> with the next run time of the task.</returns>
		private DateTimeOffset CalculateOneTime(DateTimeOffset currentDate, DateTimeOffset? lastExecuted)
		{
			if (lastExecuted.HasValue && lastExecuted.Value != DateTimeOffset.MinValue)
			{
				// Already executed
				return DateTimeOffset.MaxValue;
			}
			else
			{
				return mScheduledDateTime.Value;
			}
		}
		#endregion

		#region CalculateInterval
		/// <summary>
		/// Calculates the schedule for an event that happens in regular intervals..
		/// </summary>
		/// <param name="currentDate">The current date and time to use for the calculation.</param>
		/// <param name="lastExecuted">The last date and time the task was executed. May be <see langword="null"/>.</param>
		/// <returns>A <see cref="DateTimeOffset"/> with the next run time of the task.</returns>
		private DateTimeOffset CalculateInterval(DateTimeOffset currentDate, DateTimeOffset? lastExecuted)
		{
			DateTimeOffset TempDate = lastExecuted ?? DateTimeOffset.MinValue;

			if (TempDate == DateTimeOffset.MinValue)
			{
				// Not executed yet
				TempDate = mScheduledDateTime.Value;
			}
			else
			{
				if (mScheduledDateTime > currentDate)
				{
					TempDate = mScheduledDateTime.Value;
				}
				else
				{
					TempDate = TempDate.Add(mStartTimeOrInterval.Value);
					TempDate = (TempDate > currentDate) ? TempDate : currentDate;
				}
			}
			return FindMatchingWeekday(TempDate, true);
		}
		#endregion

		#region CalculateDaily
		/// <summary>
		/// Calculates the schedule for a daily event.
		/// </summary>
		/// <param name="currentDate">The current date and time to use for the calculation.</param>
		/// <param name="lastExecuted">The last date and time the task was executed. May be <see langword="null"/>.</param>
		/// <returns>A <see cref="DateTimeOffset"/> with the next run time of the task.</returns>
		private DateTimeOffset CalculateDaily(DateTimeOffset currentDate, DateTimeOffset? lastExecuted)
		{
			DateTimeOffset TempDate = lastExecuted ?? DateTimeOffset.MinValue;
			if (mDayInMonthOrInterval > 1)
			{
				TempDate = TempDate.AddDays((double)mDayInMonthOrInterval);
			}
			TempDate = GetDateOnly(TempDate).Add(mStartTimeOrInterval.Value);

			if (TempDate <= currentDate)
			{
				TempDate = GetDateOnly(currentDate).Add(mStartTimeOrInterval.Value);
				if (TempDate <= currentDate)
				{
					TempDate = TempDate.AddDays(1.0);
				}
			}

			TempDate = FindMatchingWeekday(TempDate, false);

			return TempDate;
		}
		#endregion

		#region CalculateWeekly
		/// <summary>
		/// Calculates the schedule for a weekly event.
		/// </summary>
		/// <param name="currentDate">The current date and time to use for the calculation.</param>
		/// <param name="lastExecuted">The last date and time the task was executed. May be <see langword="null"/>.</param>
		/// <returns>A <see cref="DateTimeOffset"/> with the next run time of the task.</returns>
		private DateTimeOffset CalculateWeekly(DateTimeOffset currentDate, DateTimeOffset? lastExecuted)
		{
			DateTimeOffset TempDate = lastExecuted ?? DateTimeOffset.MinValue;

			// Find the next matching weekday after last execution
			TempDate = GetDateOnly(TempDate).AddDays(((mDayInMonthOrInterval.Value - 1) * 7.0) + 1.0);
			TempDate = FindMatchingWeekday(TempDate, false);
			TempDate = TempDate.Add(mStartTimeOrInterval.Value);

			if (TempDate <= currentDate)
			{
				TempDate = GetDateOnly(currentDate).Add(mStartTimeOrInterval.Value);
				if (TempDate <= currentDate)
				{
					TempDate = TempDate.AddDays(1.0);
				}
				TempDate = FindMatchingWeekday(TempDate, false);
			}

			return TempDate;
		}
		#endregion

		#region CalculateMonthlyWeekDay
		/// <summary>
		/// Calculates the schedule for a monthly event, due on a specified week at a specified day.
		/// </summary>
		/// <param name="currentDate">The current date and time to use for the calculation.</param>
		/// <returns>A <see cref="DateTimeOffset"/> with the next run time of the task.</returns>
		private DateTimeOffset CalculateMonthlyWeekDay(DateTimeOffset currentDate)
		{
			DateTimeOffset TempDate = FindMatchingMonth(currentDate, 1);
			TempDate = FindMatchingWeekdayInMonth(TempDate).Add(mStartTimeOrInterval.Value);

			if (TempDate < currentDate)
			{
				TempDate = FindMatchingMonth(currentDate.AddMonths(1), 1);
				TempDate = FindMatchingWeekdayInMonth(TempDate);
				TempDate = TempDate.Add(mStartTimeOrInterval.Value);
			}

			return TempDate;
		}
		#endregion

		#region CalculateMonthlyDay
		/// <summary>
		/// Calculates the schedule for a monthly event, due on a specified day in the month.
		/// </summary>
		/// <param name="currentDate">The current date and time to use for the calculation.</param>
		/// <returns>A <see cref="DateTimeOffset"/> with the next run time of the task.</returns>
		private DateTimeOffset CalculateMonthlyDay(DateTimeOffset currentDate)
		{
			DateTimeOffset TempDate = GetMonthDay(currentDate.Year, currentDate.Month, mDayInMonthOrInterval.Value, currentDate.Offset);
			if (TempDate.Add(mStartTimeOrInterval.Value) < currentDate)
			{
				TempDate = TempDate.AddMonths(1);
			}

			TempDate = FindMatchingMonth(TempDate, mDayInMonthOrInterval.Value);
			TempDate = TempDate.Add(mStartTimeOrInterval.Value);

			return TempDate;
		}
		#endregion
		#endregion

		#region OnlyOneFlagSet
		/// <summary>
		/// Checks if only one bit in an integer (that can represent an enumeration) is set to 1.
		/// </summary>
		/// <param name="flags">The integer to check.</param>
		/// <returns>true, if exactly one bit is 1; otherwise, false.</returns>
		private static bool OnlyOneFlagSet(int flags)
		{
			return (flags != 0) && ((flags & (flags - 1)) == 0);
		}
		#endregion

		#region FindMatchingWeekday
		/// <summary>
		/// Finds the next day relative to the specified date that is on a week day that is defined in <see cref="P:WeekDays"/>.
		/// </summary>
		/// <param name="date">The date to start searching for a matching week day.</param>
		/// <param name="resetTime">A value indicating if the time should be reset to 0:00, if another day than the specified one is selected.</param>
		/// <returns>A <see cref="DateTimeOffset"/> structure.</returns>
		private DateTimeOffset FindMatchingWeekday(DateTimeOffset date, bool resetTime)
		{
			bool HasNewDate = false;
			while ((mWeekdays & ToWeekDays(date.DayOfWeek)) != ToWeekDays(date.DayOfWeek))
			{
				date = date.AddDays(1.0);
				HasNewDate = true;
			}

			if (HasNewDate && resetTime)
			{
				date = new DateTimeOffset(date.Year, date.Month, date.Day, 0, 0, 0, 0, date.Offset);
			}
			return date;
		}
		#endregion

		#region ToWeekDays
		/// <summary>
		/// Converts a DayOfWeek enumeration to a WeekDays enumeration.
		/// </summary>
		/// <param name="day">The day to convert.</param>
		/// <returns>A WeekDays value.</returns>
		private static Weekdays ToWeekDays(DayOfWeek day)
		{
			switch (day)
			{
				case DayOfWeek.Friday:
					return NerdyDuck.Scheduler.Weekdays.Friday;
				case DayOfWeek.Monday:
					return NerdyDuck.Scheduler.Weekdays.Monday;
				case DayOfWeek.Saturday:
					return NerdyDuck.Scheduler.Weekdays.Saturday;
				case DayOfWeek.Sunday:
					return NerdyDuck.Scheduler.Weekdays.Sunday;
				case DayOfWeek.Thursday:
					return NerdyDuck.Scheduler.Weekdays.Thursday;
				case DayOfWeek.Tuesday:
					return NerdyDuck.Scheduler.Weekdays.Tuesday;
				default: // DayOfWeek.Wednesday
					return NerdyDuck.Scheduler.Weekdays.Wednesday;
			}
		}
		#endregion

		#region GetDateOnly
		/// <summary>
		/// Returns the date part of the specified <see cref="DateTimeOffset"/>, including the offset.
		/// </summary>
		/// <param name="date">The date and time to separate.</param>
		/// <returns>The date part of <paramref name="date"/>.</returns>
		private static DateTimeOffset GetDateOnly(DateTimeOffset date)
		{
			return new DateTimeOffset(date.Year, date.Month, date.Day, 0, 0, 0, date.Offset);
		}
		#endregion

		#region FindMatchingMonth
		/// <summary>
		/// Gets the next month specified in <see cref="Months"/> at or after the specified date.
		/// </summary>
		/// <param name="date">The date to start searching.</param>
		/// <param name="dayInMonth">The specific day in the month to return.</param>
		/// <returns>A <see cref="DateTimeOffset"/> structure.</returns>
		private DateTimeOffset FindMatchingMonth(DateTimeOffset date, int dayInMonth)
		{
			int CurrentYear = date.Year;
			int CurrentMonthInt = date.Month;
			Months CurrentMonth;

			while (true)
			{
				CurrentMonth = (Months)(1 << (CurrentMonthInt - 1));
				if ((mMonths & CurrentMonth) == CurrentMonth)
				{
					break;
				}
				CurrentMonthInt++;
				if (CurrentMonthInt > 12)
				{
					CurrentMonthInt = 1;
					CurrentYear++;
				}
			}

			return GetMonthDay(CurrentYear, CurrentMonthInt, dayInMonth, date.Offset);
		}
		#endregion

		#region GetMonthDay
		/// <summary>
		/// Returns a date specified by the parameters.
		/// </summary>
		/// <param name="year">The year of the date.</param>
		/// <param name="month">The month of the day.</param>
		/// <param name="day">The day in the month of the date. May be 0 or negative to specify a day counted beginning at the end of the month.</param>
		/// <param name="offset">The timezone offset for the date to return.</param>
		/// <returns>A date (time set to 00:00:00). if <paramref name="day"/> resolves to a day that does not exist in that month, the next valid day is used.</returns>
		private static DateTimeOffset GetMonthDay(int year, int month, int day, TimeSpan offset)
		{
			int DaysInMonth = DateTime.DaysInMonth(year, month);
			int ActualDay;
			if (day > 0)
			{
				ActualDay = (day > DaysInMonth) ? DaysInMonth : day;
			}
			else
			{
				ActualDay = DaysInMonth + day;
				if (ActualDay < 1)
					ActualDay = 1;
			}
			return new DateTimeOffset(year, month, ActualDay, 0, 0, 0, offset);
		}
		#endregion

		#region FindMatchingWeekdayInMonth
		/// <summary>
		/// Finds the matching week day in a month, according to <see cref="P:WeekDays"/> and <see cref="P:WeekDayInMonth"/>.
		/// </summary>
		/// <param name="date">The date containing the year and month to find the day in.</param>
		/// <returns>A <see cref="DateTimeOffset"/> structure.</returns>
		private DateTimeOffset FindMatchingWeekdayInMonth(DateTimeOffset date)
		{
			if (mWeekInMonth == NerdyDuck.Scheduler.WeekInMonth.Last)
			{
				date = new DateTimeOffset(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month), 0, 0, 0, date.Offset);
				int DayShift = DifferenceInWeek(date.DayOfWeek);
				if (DayShift > 0)
				{
					DayShift -= 7;
				}
				date = new DateTimeOffset(date.Year, date.Month, date.Day + DayShift, 0, 0, 0, date.Offset);
			}
			else
			{
				date = new DateTimeOffset(date.Year, date.Month, 1, 0, 0, 0, date.Offset);
				int DayShift = DifferenceInWeek(date.DayOfWeek);
				switch (mWeekInMonth)
				{
					case NerdyDuck.Scheduler.WeekInMonth.Second:
						DayShift += 7;
						break;
					case NerdyDuck.Scheduler.WeekInMonth.Third:
						DayShift += 14;
						break;
					case NerdyDuck.Scheduler.WeekInMonth.Fourth:
						DayShift += 21;
						break;
				}
				date = new DateTimeOffset(date.Year, date.Month, DayShift + 1, 0, 0, 0, date.Offset);
			}

			return date;
		}
		#endregion

		#region DifferenceInWeek
		/// <summary>
		/// Defines the relative position of two week days within the week.
		/// </summary>
		/// <param name="day">The fix day in the week.</param>
		/// <returns>An integer ranging from -6 to 6.</returns>
		private int DifferenceInWeek(DayOfWeek day)
		{
			int ReturnValue = 0;
			int DayInt = (int)mWeekdays;
			do
			{
				DayInt = DayInt >> 1;
				ReturnValue++;
			}
			while (DayInt != 0);

			switch (day)
			{
				case DayOfWeek.Monday:
					ReturnValue -= 1;
					break;
				case DayOfWeek.Tuesday:
					ReturnValue -= 2;
					break;
				case DayOfWeek.Wednesday:
					ReturnValue -= 3;
					break;
				case DayOfWeek.Thursday:
					ReturnValue -= 4;
					break;
				case DayOfWeek.Friday:
					ReturnValue -= 5;
					break;
				case DayOfWeek.Saturday:
					ReturnValue -= 6;
					break;
				default:
					ReturnValue -= 7;
					break;
			}

			if (ReturnValue < 0)
				ReturnValue += 7;

			return ReturnValue;
		}
		#endregion

		#region ParseEnumAttribute
		/// <summary>
		/// Parses an XML attribute into an enumeration.
		/// </summary>
		/// <typeparam name="TEnum">The enumeration to parse the <paramref name="value"/> into.</typeparam>
		/// <param name="value">The string to parse.</param>
		/// <param name="hresult">A HRESULT to throw in an exception if the parsing fails.</param>
		/// <param name="attributeName">The name of the attribute containing the value.</param>
		/// <returns>The enumeration value of the attribute.</returns>
		/// <exception cref="CodedXmlException"><paramref name="value"/> cannot be parsed into enumeration <typeparamref name="TEnum"/>.</exception>
		private static TEnum ParseEnumAttribute<TEnum>(string value, int hresult, string attributeName) where TEnum : struct
		{
			TEnum ReturnValue;
			if (!Enum.TryParse(value, out ReturnValue))
			{
				throw new CodedXmlException(hresult, string.Format(Properties.Resources.Schedule_ParseEnumAttribute_InvalidAttribute, attributeName));
			}
			return ReturnValue;
		}
		#endregion

		#region ParseAttribute
		/// <summary>
		/// Parse an XML attribute value into the matching type and assigns the value to the matching property, depending on the attribute name.
		/// </summary>
		/// <param name="name">The name of the attribute.</param>
		/// <param name="value">The value of the attribute.</param>
		/// <returns>true, if the attribute is the 'type' attribute; otherwise, false.</returns>
		private bool ParseAttribute(string name, string value)
		{
			bool TypeFound = false;
			switch (name)
			{
				case TypeName:
					mScheduleType = ParseEnumAttribute<ScheduleType>(value, Errors.CreateHResult(0x02), name);
					TypeFound = true;
					break;
				case ScheduledDateTimeName:
					mScheduledDateTime = XmlConvert.ToDateTimeOffset(value);
					break;
				case StartTimeName:
				case IntervalName:
					mStartTimeOrInterval = XmlConvert.ToTimeSpan(value);
					break;
				case WeekdayName:
				case WeekdaysName:
					mWeekdays = ParseEnumAttribute<Weekdays>(value, Errors.CreateHResult(0x03), name);
					break;
				case IntervalDaysName:
				case IntervalWeeksName:
				case DayInMonthName:
					mDayInMonthOrInterval = XmlConvert.ToInt16(value);
					break;
				case MonthsName:
					mMonths = ParseEnumAttribute<Months>(value, Errors.CreateHResult(0x04), name);
					break;
				case WeekInMonthName:
					mWeekInMonth = ParseEnumAttribute<WeekInMonth>(value, Errors.CreateHResult(0x05), name);
					break;
			}
			return TypeFound;
		}
		#endregion
		#endregion
	}
}
