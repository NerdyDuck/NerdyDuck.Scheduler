#region Copyright
/*******************************************************************************
 * <copyright file="ScheduleTest.cs" owner="Daniel Kopp">
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
 * <file name="ScheduleTest.cs" date="2016-02-19">
 * Contains test methods to test the
 * NerdyDuck.Scheduler.Schedule class.
 * </file>
 ******************************************************************************/
#endregion

#if WINDOWS_UWP
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#endif
#if WINDOWS_DESKTOP
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.CodeAnalysis;
#endif
using NerdyDuck.CodedExceptions;
using NerdyDuck.Scheduler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Xml;

namespace NerdyDuck.Tests.Scheduler
{
	/// <summary>
	/// Contains test methods to test the NerdyDuck.Scheduler.Schedule class.
	/// </summary>
#if WINDOWS_DESKTOP
	[ExcludeFromCodeCoverage]
#endif
	[TestClass]
	public class ScheduleTest
	{
		private readonly DateTimeOffset NowDate = new DateTimeOffset(2013, 6, 4, 12, 0, 0, TimeSpan.Zero); // Tuesday - Current date
		private readonly DateTimeOffset FutureFiveMinDate = new DateTimeOffset(2013, 6, 4, 12, 5, 0, TimeSpan.Zero); // Tuesday - 5 minutes after NowDate
		private readonly DateTimeOffset PastFiveMinDate = new DateTimeOffset(2013, 6, 4, 11, 55, 0, TimeSpan.Zero); // Tuesday - 5 minutes before NowDate
		private readonly DateTimeOffset FutureDate = new DateTimeOffset(2013, 10, 27, 8, 15, 0, TimeSpan.Zero); // Sunday - a date in the future, relative to NowDate
		private readonly DateTimeOffset PastDate1 = new DateTimeOffset(2013, 5, 5, 14, 21, 0, TimeSpan.Zero); // Sunday - a date in the past, relative to NowDate
		private readonly DateTimeOffset PastDate2 = new DateTimeOffset(2013, 5, 7, 23, 0, 0, TimeSpan.Zero); // Tuesday - a date between PastDate1 and NowDate
		private readonly TimeSpan TenMinutes = TimeSpan.FromMinutes(10.0); // Ten minute interval
		private readonly TimeSpan StartTime1720 = new TimeSpan(17, 20, 0);
		private readonly TimeSpan StartTime0820 = new TimeSpan(8, 20, 0);
		private readonly DateTimeOffset NowDate1720 = new DateTimeOffset(2013, 6, 4, 17, 20, 0, TimeSpan.Zero); // Tuesday - After NowDate
		private readonly DateTimeOffset TomorrowDate0820 = new DateTimeOffset(2013, 6, 5, 8, 20, 0, TimeSpan.Zero); // Wednesday - Day after NowDate at 8:20

		#region OneTime
		[TestMethod]
		public void GetNextDueDate_OneTimeFuture_Success()
		{
			Schedule schedule = Schedule.CreateOneTime(FutureDate);
			DateTimeOffset next = schedule.GetNextDueDate(NowDate, null);
			Assert.AreEqual(FutureDate, next);
		}

		[TestMethod]
		public void GetNextDueDate_OneTimeLast_Success()
		{
			Schedule schedule = Schedule.CreateOneTime(PastDate1);
			DateTimeOffset next = schedule.GetNextDueDate(NowDate, PastDate2);
			Assert.AreEqual(DateTimeOffset.MaxValue, next);
		}

		[TestMethod]
		public void GetNextDueDate_OneTimeNow_Success()
		{
			Schedule schedule = Schedule.CreateOneTime(PastDate1);
			DateTimeOffset next = schedule.GetNextDueDate(NowDate, null);
			Assert.AreEqual(PastDate1, next);
		}

		[TestMethod]
		public void ReadWriteXml_OneTime_Success()
		{
			Schedule s1 = Schedule.CreateOneTime(FutureDate);
			using (MemoryStream buffer = new MemoryStream())
			{
				XmlSerializer serializer = new XmlSerializer(typeof(Schedule));
				serializer.Serialize(buffer, s1);
				buffer.Seek(0, SeekOrigin.Begin);
				Schedule s2 = (Schedule)serializer.Deserialize(buffer);
				AssertEqual(s1, s2);
			}
		}

#if WINDOWS_DESKTOP
		[TestMethod]
		public void Ctor_SerializationInfoOneTime_Success()
		{
			Schedule s1 = Schedule.CreateOneTime(FutureDate);
			MemoryStream buffer = SerializationHelper.Serialize(s1);
			Schedule s2 = SerializationHelper.Deserialize<Schedule>(buffer);
			buffer.Dispose();
			AssertEqual(s1, s2);
		}
#endif
		#endregion

		#region Interval
		[TestMethod]
		public void GetNextDueDate_IntervalFuture_Success()
		{
			Schedule schedule = Schedule.CreateInterval(FutureDate, TenMinutes, Weekdays.All);
			DateTimeOffset next = schedule.GetNextDueDate(NowDate, null);
			Assert.AreEqual(FutureDate, next);
		}

		[TestMethod]
		public void GetNextDueDate_IntervalFuture2_Success()
		{
			Schedule schedule = Schedule.CreateInterval(PastDate1, TenMinutes, Weekdays.Thursday);
			DateTimeOffset next = schedule.GetNextDueDate(NowDate, PastFiveMinDate);
			Assert.AreEqual(new DateTimeOffset(2013, 6, 6, 0, 0, 0, TimeSpan.Zero), next);
		}

		[TestMethod]
		public void GetNextDueDate_IntervalFutureLast_Success()
		{
			Schedule schedule = Schedule.CreateInterval(FutureDate, TenMinutes, Weekdays.All);
			DateTimeOffset next = schedule.GetNextDueDate(NowDate, PastDate1);
			Assert.AreEqual(FutureDate, next);
		}

		[TestMethod]
		public void GetNextDueDate_IntervalPast_Success()
		{
			Schedule schedule = Schedule.CreateInterval(PastDate1, TenMinutes, Weekdays.All);
			DateTimeOffset next = schedule.GetNextDueDate(NowDate, null);
			Assert.AreEqual(PastDate1, next);
		}

		[TestMethod]
		public void GetNextDueDate_IntervalPastLast_Success()
		{
			Schedule schedule = Schedule.CreateInterval(PastDate1, TenMinutes, Weekdays.All);
			DateTimeOffset next = schedule.GetNextDueDate(NowDate, PastFiveMinDate);
			Assert.AreEqual(FutureFiveMinDate, next);
		}

		[TestMethod]
		public void GetNextDueDate_IntervalPastLast2_Success()
		{
			Schedule schedule = Schedule.CreateInterval(PastDate1, TenMinutes, Weekdays.All);
			DateTimeOffset next = schedule.GetNextDueDate(NowDate, PastDate2);
			Assert.AreEqual(NowDate, next);
		}

		[TestMethod]
		public void ReadWriteXml_Interval_Success()
		{
			Schedule s1 = Schedule.CreateInterval(PastDate1, TenMinutes, Weekdays.Weekend);
			using (MemoryStream buffer = new MemoryStream())
			{
				XmlSerializer serializer = new XmlSerializer(typeof(Schedule));
				serializer.Serialize(buffer, s1);
				buffer.Seek(0, SeekOrigin.Begin);
				Schedule s2 = (Schedule)serializer.Deserialize(buffer);
				AssertEqual(s1, s2);
			}
		}

#if WINDOWS_DESKTOP
		[TestMethod]
		public void Ctor_SerializationInfoInterval_Success()
		{
			Schedule s1 = Schedule.CreateInterval(PastDate1, TenMinutes, Weekdays.Weekend);
			MemoryStream buffer = SerializationHelper.Serialize(s1);
			Schedule s2 = SerializationHelper.Deserialize<Schedule>(buffer);
			buffer.Dispose();
			AssertEqual(s1, s2);
		}
#endif
		#endregion

		#region Daily
		[TestMethod]
		public void GetNextDueDate_DailyFuture1_Success()
		{
			Schedule schedule = Schedule.CreateDaily(StartTime1720, 2, Weekdays.All);
			DateTimeOffset next = schedule.GetNextDueDate(NowDate, null);
			Assert.AreEqual(NowDate1720, next);
		}

		[TestMethod]
		public void GetNextDueDate_DailyFuture2_Success()
		{
			Schedule schedule = Schedule.CreateDaily(StartTime0820, 2, Weekdays.All);
			DateTimeOffset next = schedule.GetNextDueDate(NowDate, null);
			Assert.AreEqual(TomorrowDate0820, next);
		}

		[TestMethod]
		public void GetNextDueDate_DailyFuture3_Success()
		{
			Schedule schedule = Schedule.CreateDaily(StartTime1720, 1, Weekdays.All);
			DateTimeOffset next = schedule.GetNextDueDate(NowDate, null);
			Assert.AreEqual(NowDate1720, next);
		}

		[TestMethod]
		public void GetNextDueDate_DailyLastFuture1_Success()
		{
			Schedule schedule = Schedule.CreateDaily(StartTime0820, 2, Weekdays.All);
			DateTimeOffset next = schedule.GetNextDueDate(NowDate, new DateTimeOffset(2013, 6, 4, 8, 21, 0, TimeSpan.Zero));
			Assert.AreEqual(TomorrowDate0820.AddDays(1.0), next);
		}

		[TestMethod]
		public void GetNextDueDate_DailyLastFuture2_Success()
		{
			Schedule schedule = Schedule.CreateDaily(StartTime1720, 2, Weekdays.All);
			DateTimeOffset next = schedule.GetNextDueDate(NowDate, PastDate1);
			Assert.AreEqual(NowDate1720, next);
		}

		[TestMethod]
		public void ReadWriteXml_Daily_Success()
		{
			Schedule s1 = Schedule.CreateDaily(StartTime1720, 2, Weekdays.All);
			using (MemoryStream buffer = new MemoryStream())
			{
				XmlSerializer serializer = new XmlSerializer(typeof(Schedule));
				serializer.Serialize(buffer, s1);
				buffer.Seek(0, SeekOrigin.Begin);
				Schedule s2 = (Schedule)serializer.Deserialize(buffer);
				AssertEqual(s1, s2);
			}
		}

#if WINDOWS_DESKTOP
		[TestMethod]
		public void Ctor_SerializationInfoDaily_Success()
		{
			Schedule s1 = Schedule.CreateDaily(StartTime1720, 2, Weekdays.MondayToFriday);
			MemoryStream buffer = SerializationHelper.Serialize(s1);
			Schedule s2 = SerializationHelper.Deserialize<Schedule>(buffer);
			buffer.Dispose();
			AssertEqual(s1, s2);
		}
#endif
		#endregion

		#region Weekly
		[TestMethod]
		public void GetNextDueDate_WeeklyFuture1_Success()
		{
			Schedule schedule = Schedule.CreateWeekly(StartTime0820, 2, Weekdays.Wednesday);
			DateTimeOffset next = schedule.GetNextDueDate(NowDate, null);
			Assert.AreEqual(TomorrowDate0820, next);
		}

		[TestMethod]
		public void GetNextDueDate_WeeklyFuture2_Success()
		{
			Schedule schedule = Schedule.CreateWeekly(StartTime1720, 2, Weekdays.Tuesday);
			DateTimeOffset next = schedule.GetNextDueDate(NowDate, null);
			Assert.AreEqual(NowDate1720, next);
		}

		[TestMethod]
		public void GetNextDueDate_WeeklyFuture3_Success()
		{
			Schedule schedule = Schedule.CreateWeekly(StartTime1720, 2, Weekdays.Monday);
			DateTimeOffset next = schedule.GetNextDueDate(NowDate, null);
			Assert.AreEqual(new DateTimeOffset(2013, 6, 10, 17, 20, 0, TimeSpan.Zero), next);
		}

		[TestMethod]
		public void GetNextDueDate_WeeklyLastFuture1_Success()
		{
			Schedule schedule = Schedule.CreateWeekly(StartTime1720, 2, Weekdays.Monday);
			DateTimeOffset next = schedule.GetNextDueDate(NowDate1720, PastFiveMinDate);
			Assert.AreEqual(new DateTimeOffset(2013, 6, 17, 17, 20, 0, TimeSpan.Zero), next);
		}

		[TestMethod]
		public void GetNextDueDate_WeeklyLastFuture2_Success()
		{
			Schedule schedule = Schedule.CreateWeekly(StartTime1720, 2, Weekdays.Monday);
			DateTimeOffset next = schedule.GetNextDueDate(NowDate, PastDate1);
			Assert.AreEqual(new DateTimeOffset(2013, 6, 10, 17, 20, 0, TimeSpan.Zero), next);
		}

		[TestMethod]
		public void ReadWriteXml_Weekly_Success()
		{
			Schedule s1 = Schedule.CreateWeekly(StartTime0820, 2, Weekdays.Wednesday);
			using (MemoryStream buffer = new MemoryStream())
			{
				XmlSerializer serializer = new XmlSerializer(typeof(Schedule));
				serializer.Serialize(buffer, s1);
				buffer.Seek(0, SeekOrigin.Begin);
				Schedule s2 = (Schedule)serializer.Deserialize(buffer);
				AssertEqual(s1, s2);
			}
		}

#if WINDOWS_DESKTOP
		[TestMethod]
		public void Ctor_SerializationInfoWeekly_Success()
		{
			Schedule s1 = Schedule.CreateWeekly(StartTime0820, 2, Weekdays.Wednesday);
			MemoryStream buffer = SerializationHelper.Serialize(s1);
			Schedule s2 = SerializationHelper.Deserialize<Schedule>(buffer);
			buffer.Dispose();
			AssertEqual(s1, s2);
		}
#endif
		#endregion

		#region MonthlyDay
		[TestMethod]
		public void GetNextDueDate_MonthlyDayFuture1_Success()
		{
			Schedule schedule = Schedule.CreateMonthlyDay(StartTime1720, 6, Months.All);
			DateTimeOffset next = schedule.GetNextDueDate(NowDate, null);
			Assert.AreEqual(new DateTimeOffset(2013, 6, 6, 17, 20, 0, TimeSpan.Zero), next);
		}

		[TestMethod]
		public void GetNextDueDate_MonthlyDayLastFuture1_Success()
		{
			Schedule schedule = Schedule.CreateMonthlyDay(StartTime1720, 2, Months.All);
			DateTimeOffset next = schedule.GetNextDueDate(NowDate, PastDate1);
			Assert.AreEqual(new DateTimeOffset(2013, 7, 2, 17, 20, 0, TimeSpan.Zero), next);
		}

		[TestMethod]
		public void GetNextDueDate_MonthlyDayFuture2_Success()
		{
			Schedule schedule = Schedule.CreateMonthlyDay(StartTime1720, 2, Months.August);
			DateTimeOffset next = schedule.GetNextDueDate(NowDate, null);
			Assert.AreEqual(new DateTimeOffset(2013, 8, 2, 17, 20, 0, TimeSpan.Zero), next);
		}

		[TestMethod]
		public void GetNextDueDate_MonthlyDayLastFuture2_Success()
		{
			Schedule schedule = Schedule.CreateMonthlyDay(StartTime1720, 0, Months.All);
			DateTimeOffset next = schedule.GetNextDueDate(NowDate, PastDate2);
			Assert.AreEqual(new DateTimeOffset(2013, 6, 30, 17, 20, 0, TimeSpan.Zero), next);
		}

		[TestMethod]
		public void GetNextDueDate_MonthlyDayLastFuture3_Success()
		{
			Schedule schedule = Schedule.CreateMonthlyDay(StartTime1720, -1, Months.March);
			DateTimeOffset next = schedule.GetNextDueDate(NowDate, PastDate2);
			Assert.AreEqual(new DateTimeOffset(2014, 3, 30, 17, 20, 0, TimeSpan.Zero), next);
		}

		[TestMethod]
		public void GetNextDueDate_MonthlyDayLastFuture4_Success()
		{
			Schedule schedule = Schedule.CreateMonthlyDay(StartTime1720, -31, Months.February);
			DateTimeOffset next = schedule.GetNextDueDate(NowDate, PastDate2);
			Assert.AreEqual(new DateTimeOffset(2014, 2, 1, 17, 20, 0, TimeSpan.Zero), next);
		}

		[TestMethod]
		public void ReadWriteXml_MonthlyDay_Success()
		{
			Schedule s1 = Schedule.CreateMonthlyDay(StartTime1720, 2, Months.All);
			using (MemoryStream buffer = new MemoryStream())
			{
				XmlSerializer serializer = new XmlSerializer(typeof(Schedule));
				serializer.Serialize(buffer, s1);
				buffer.Seek(0, SeekOrigin.Begin);
				Schedule s2 = (Schedule)serializer.Deserialize(buffer);
				AssertEqual(s1, s2);
			}
		}

#if WINDOWS_DESKTOP
		[TestMethod]
		public void Ctor_SerializationInfoMonthlyDay_Success()
		{
			Schedule s1 = Schedule.CreateMonthlyDay(StartTime1720, 2, Months.All);
			MemoryStream buffer = SerializationHelper.Serialize(s1);
			Schedule s2 = SerializationHelper.Deserialize<Schedule>(buffer);
			buffer.Dispose();
			AssertEqual(s1, s2);
		}
#endif
		#endregion

		#region MonthlyWeekDay
		[TestMethod]
		public void GetNextDueDate_MonthlyWeekdaySecond_Success()
		{
			Schedule schedule = Schedule.CreateMonthlyWeekday(StartTime1720, Weekdays.Friday, Months.All, WeekInMonth.Second);
			DateTimeOffset next = schedule.GetNextDueDate(NowDate, null);
			Assert.AreEqual(new DateTimeOffset(2013, 6, 14, 17, 20, 0, TimeSpan.Zero), next);
		}

		[TestMethod]
		public void GetNextDueDate_MonthlyWeekdayThird_Success()
		{
			Schedule schedule = Schedule.CreateMonthlyWeekday(StartTime1720, Weekdays.Friday, Months.All, WeekInMonth.Third);
			DateTimeOffset next = schedule.GetNextDueDate(NowDate, null);
			Assert.AreEqual(new DateTimeOffset(2013, 6, 21, 17, 20, 0, TimeSpan.Zero), next);
		}

		[TestMethod]
		public void GetNextDueDate_MonthlyWeekdayFourth_Success()
		{
			Schedule schedule = Schedule.CreateMonthlyWeekday(StartTime1720, Weekdays.Friday, Months.All, WeekInMonth.Fourth);
			DateTimeOffset next = schedule.GetNextDueDate(NowDate, null);
			Assert.AreEqual(new DateTimeOffset(2013, 6, 28, 17, 20, 0, TimeSpan.Zero), next);
		}

		[TestMethod]
		public void GetNextDueDate_MonthlyWeekdayFirst_Success()
		{
			Schedule schedule = Schedule.CreateMonthlyWeekday(StartTime1720, Weekdays.Sunday, Months.All, WeekInMonth.First);
			DateTimeOffset next = schedule.GetNextDueDate(NowDate, PastDate1);
			Assert.AreEqual(new DateTimeOffset(2013, 7, 7, 17, 20, 0, TimeSpan.Zero), next);
		}

		[TestMethod]
		public void GetNextDueDate_MonthlyWeekdayLast_Success()
		{
			Schedule schedule = Schedule.CreateMonthlyWeekday(StartTime1720, Weekdays.Sunday, Months.September, WeekInMonth.Last);
			DateTimeOffset next = schedule.GetNextDueDate(NowDate, null);
			Assert.AreEqual(new DateTimeOffset(2013, 9, 29, 17, 20, 0, TimeSpan.Zero), next);
		}

		[TestMethod]
		public void ReadWriteXml_MonthlyWeekday_Success()
		{
			Schedule s1 = Schedule.CreateMonthlyWeekday(StartTime1720, Weekdays.Sunday, Months.All, WeekInMonth.First);
			using (MemoryStream buffer = new MemoryStream())
			{
				XmlSerializer serializer = new XmlSerializer(typeof(Schedule));
				serializer.Serialize(buffer, s1);
				buffer.Seek(0, SeekOrigin.Begin);
				Schedule s2 = (Schedule)serializer.Deserialize(buffer);
				AssertEqual(s1, s2);
			}
		}

#if WINDOWS_DESKTOP
		[TestMethod]
		public void Ctor_SerializationInfoMonthlyWeekday_Success()
		{
			Schedule s1 = Schedule.CreateMonthlyWeekday(StartTime1720, Weekdays.Sunday, Months.All, WeekInMonth.First);
			MemoryStream buffer = SerializationHelper.Serialize(s1);
			Schedule s2 = SerializationHelper.Deserialize<Schedule>(buffer);
			buffer.Dispose();
			AssertEqual(s1, s2);
		}
#endif
		#endregion

#if WINDOWS_DESKTOP
		#region Serialization
		[TestMethod]
		public void GetObjectData_InfoNull_Error()
		{
			Schedule s1 = Schedule.CreateMonthlyWeekday(new TimeSpan(17, 20, 0), Weekdays.Sunday, Months.All, WeekInMonth.First);
			CustomAssert.ThrowsException<ArgumentNullException>(() =>
			{
				s1.GetObjectData(null, new System.Runtime.Serialization.StreamingContext());
			});
		}
		#endregion
#endif

		#region XmlSerializable
		[TestMethod]
		public void GetSchema_Success()
		{
			Schedule s1 = Schedule.CreateMonthlyWeekday(new TimeSpan(17, 20, 0), Weekdays.Sunday, Months.All, WeekInMonth.First);
			Assert.IsNull(((IXmlSerializable)s1).GetSchema());
		}

		[TestMethod]
		public void ReadXml_ReaderNull_Error()
		{
			Schedule schedule = new Schedule();

			CustomAssert.ThrowsException<CodedArgumentNullException>(() =>
			{
				((IXmlSerializable)schedule).ReadXml(null);
			});
		}

		[TestMethod]
		public void ReadXml_NoAtttributes_Error()
		{
			string input = "<?xml version=\"1.0\" encoding=\"utf-16\"?><schedule />";
			Schedule schedule = new Schedule();
			using (XmlReader reader = XmlReader.Create(new StringReader(input)))
			{
				reader.ReadToFollowing("schedule");
				CustomAssert.ThrowsException<CodedXmlException>(() =>
				{
					((IXmlSerializable)schedule).ReadXml(reader);
				});
			}
		}

		[TestMethod]
		public void ReadXml_NoType_Error()
		{
			string input = "<?xml version=\"1.0\" encoding=\"utf-16\"?><schedule scheduledDateTime=\"2013-06-01T12:00:00Z\" />";
			Schedule schedule = new Schedule();
			using (XmlReader reader = XmlReader.Create(new StringReader(input)))
			{
				reader.ReadToFollowing("schedule");
				CustomAssert.ThrowsException<CodedXmlException>(() =>
				{
					((IXmlSerializable)schedule).ReadXml(reader);
				});
			}
		}
		[TestMethod]
		public void ReadXml_InvType_Error()
		{
			string input = "<?xml version=\"1.0\" encoding=\"utf-16\"?><schedule type=\"TwoTime\" scheduledDateTime=\"2013-06-01T12:00:00Z\" />";
			Schedule schedule = new Schedule();
			using (XmlReader reader = XmlReader.Create(new StringReader(input)))
			{
				reader.ReadToFollowing("schedule");
				CustomAssert.ThrowsException<CodedXmlException>(() =>
				{
					((IXmlSerializable)schedule).ReadXml(reader);
				});
			}
		}

		[TestMethod]
		public void WriteXml_WriterNull_Error()
		{
			Schedule schedule = Schedule.CreateOneTime(DateTimeOffset.Now);

			CustomAssert.ThrowsException<CodedArgumentNullException>(() =>
			{
				((IXmlSerializable)schedule).WriteXml(null);
			});
		}

		#endregion

		#region Create
		[TestMethod]
		public void Create_Success()
		{
			Schedule schedule = Schedule.Create(ScheduleType.OneTime, DateTimeOffset.Now, null, null, null, null, null);
			Assert.AreEqual(ScheduleType.OneTime, schedule.ScheduleType);
		}

		[TestMethod]
		public void Create_TypeNone_Error()
		{
			CustomAssert.ThrowsException<CodedInvalidOperationException>(() =>
			{
				Schedule schedule = Schedule.Create(ScheduleType.None, null, null, null, null, null, null);
			});
		}
		#endregion

		#region Asserts
		[TestMethod]
		public void AssertScheduledDateTime_Error()
		{
			CustomAssert.ThrowsException<CodedArgumentNullException>(() =>
			{
				Schedule schedule = Schedule.Create(ScheduleType.OneTime, null, null, null, null, null, null);
			});
		}

		[TestMethod]
		public void AssertTimeSpanInterval_Error()
		{
			CustomAssert.ThrowsException<CodedArgumentNullException>(() =>
			{
				Schedule schedule = Schedule.Create(ScheduleType.Interval, FutureDate, null, Weekdays.All, 1, null, null);
			});

			CustomAssert.ThrowsException<CodedArgumentOutOfRangeException>(() =>
			{
				Schedule schedule = Schedule.Create(ScheduleType.Interval, FutureDate, TimeSpan.FromMinutes(-5.0), Weekdays.All, 1, null, null);
			});
		}

		[TestMethod]
		public void AssertDayMonthInterval_Error()
		{
			CustomAssert.ThrowsException<CodedArgumentNullException>(() =>
			{
				Schedule schedule = Schedule.Create(ScheduleType.Daily, null, StartTime1720, Weekdays.All, null, null, null);
			});

			CustomAssert.ThrowsException<CodedArgumentOutOfRangeException>(() =>
			{
				Schedule schedule = Schedule.Create(ScheduleType.Daily, null, StartTime1720, Weekdays.All, 0, null, null);
			});
		}

		[TestMethod]
		public void AssertDayInMonth_Error()
		{
			CustomAssert.ThrowsException<CodedArgumentNullException>(() =>
			{
				Schedule schedule = Schedule.Create(ScheduleType.MonthlyDay, null, StartTime1720, Weekdays.All, null, Months.All, null);
			});

			CustomAssert.ThrowsException<CodedArgumentOutOfRangeException>(() =>
			{
				Schedule schedule = Schedule.Create(ScheduleType.MonthlyDay, null, StartTime1720, Weekdays.All, -42, Months.All, null);
			});

			CustomAssert.ThrowsException<CodedArgumentOutOfRangeException>(() =>
			{
				Schedule schedule = Schedule.Create(ScheduleType.MonthlyDay, null, StartTime1720, Weekdays.All, 42, Months.All, null);
			});
		}

		[TestMethod]
		public void AssertWeekday_Error()
		{
			CustomAssert.ThrowsException<CodedArgumentNullException>(() =>
			{
				Schedule schedule = Schedule.Create(ScheduleType.Weekly, null, StartTime1720, null, 1, null, null);
			});

			CustomAssert.ThrowsException<CodedArgumentOutOfRangeException>(() =>
			{
				Schedule schedule = Schedule.Create(ScheduleType.Weekly, null, StartTime1720, Weekdays.None, 1, null, null);
			});

			CustomAssert.ThrowsException<CodedArgumentOutOfRangeException>(() =>
			{
				Schedule schedule = Schedule.Create(ScheduleType.Weekly, null, StartTime1720, Weekdays.Monday | Weekdays.Tuesday, 1, null, null);
			});
		}

		[TestMethod]
		public void AssertWeekdays_Error()
		{
			CustomAssert.ThrowsException<CodedArgumentNullException>(() =>
			{
				Schedule schedule = Schedule.Create(ScheduleType.Daily, null, StartTime1720, null, 1, null, null);
			});

			CustomAssert.ThrowsException<CodedArgumentOutOfRangeException>(() =>
			{
				Schedule schedule = Schedule.Create(ScheduleType.Daily, null, StartTime1720, Weekdays.None, 1, null, null);
			});
		}

		[TestMethod]
		public void AssertStartTime_Error()
		{
			CustomAssert.ThrowsException<CodedArgumentNullException>(() =>
			{
				Schedule schedule = Schedule.Create(ScheduleType.Daily, null, null, Weekdays.All, 1, null, null);
			});

			CustomAssert.ThrowsException<CodedArgumentOutOfRangeException>(() =>
			{
				Schedule schedule = Schedule.Create(ScheduleType.Daily, null, TimeSpan.FromMinutes(-5.0), Weekdays.All, 1, null, null);
			});

			CustomAssert.ThrowsException<CodedArgumentOutOfRangeException>(() =>
			{
				Schedule schedule = Schedule.Create(ScheduleType.Daily, null, TimeSpan.FromHours(25), Weekdays.All, 1, null, null);
			});
		}

		[TestMethod]
		public void AssertMonths_Error()
		{
			CustomAssert.ThrowsException<CodedArgumentNullException>(() =>
			{
				Schedule schedule = Schedule.Create(ScheduleType.MonthlyDay, null, StartTime1720, Weekdays.All, 1, null, null);
			});

			CustomAssert.ThrowsException<CodedArgumentOutOfRangeException>(() =>
			{
				Schedule schedule = Schedule.Create(ScheduleType.MonthlyDay, null, StartTime1720, Weekdays.All, 1, Months.None, null);
			});
		}

		[TestMethod]
		public void AssertWeekInMonth_Error()
		{
			CustomAssert.ThrowsException<CodedArgumentNullException>(() =>
			{
				Schedule schedule = Schedule.Create(ScheduleType.MonthlyWeekday, null, StartTime1720, Weekdays.Monday, null, Months.All, null);
			});
		}

		[TestMethod]
		public void AssertInitialized_Error()
		{
			CustomAssert.ThrowsException<CodedInvalidOperationException>(() =>
			{
				Schedule schedule = new Schedule();
				schedule.GetNextDueDate(NowDate, null);
			});
		}
		#endregion

		#region Private methods
		private void AssertEqual(Schedule s1, Schedule s2)
		{
			Assert.AreEqual(s1.DayInMonthOrInterval.HasValue, s2.DayInMonthOrInterval.HasValue, "DayInMonthOrInterval");
			if (s1.DayInMonthOrInterval.HasValue)
				Assert.AreEqual(s1.DayInMonthOrInterval.Value, s2.DayInMonthOrInterval.Value, "DayInMonthOrInterval");

			Assert.AreEqual(s1.Months.HasValue, s2.Months.HasValue, "Months");
			if (s1.Months.HasValue)
				Assert.AreEqual(s1.Months.Value, s2.Months.Value, "Months");

			Assert.AreEqual(s1.ScheduledDateTime.HasValue, s2.ScheduledDateTime.HasValue, "ScheduledDateTime");
			if (s1.ScheduledDateTime.HasValue)
				Assert.AreEqual(s1.ScheduledDateTime.Value, s2.ScheduledDateTime.Value, "ScheduledDateTime");

			Assert.AreEqual(s1.StartTimeOrInterval.HasValue, s2.StartTimeOrInterval.HasValue, "StartTimeOrInterval");
			if (s1.StartTimeOrInterval.HasValue)
				Assert.AreEqual(s1.StartTimeOrInterval.Value, s2.StartTimeOrInterval.Value, "StartTimeOrInterval");

			Assert.AreEqual(s1.ScheduleType, s2.ScheduleType, "ScheduleType");

			Assert.AreEqual(s1.Weekdays.HasValue, s2.Weekdays.HasValue, "Weekdays");
			if (s1.Weekdays.HasValue)
				Assert.AreEqual(s1.Weekdays.Value, s2.Weekdays.Value, "Weekdays");

			Assert.AreEqual(s1.WeekInMonth.HasValue, s2.WeekInMonth.HasValue, "WeekInMonth");
			if (s1.WeekInMonth.HasValue)
				Assert.AreEqual(s1.WeekInMonth.Value, s2.WeekInMonth.Value, "WeekInMonth");

		}
		#endregion
	}
}
