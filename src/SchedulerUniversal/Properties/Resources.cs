#region Copyright
/*******************************************************************************
 * <copyright file="Resources.cs" owner="Daniel Kopp">
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
 * <file name="Resources.cs" date="2016-03-07">
 * Helper class to access localized string resources.
 * </file>
 ******************************************************************************/
#endregion

using System;

namespace NerdyDuck.Scheduler.Properties
{
	/// <summary>
	/// Helper class to access localized string resources.
	/// </summary>
	[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Resources.tt", "1.0.0.0")]
	[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
	internal static class Resources
	{
		#region String resource properties
		/// <summary>
		/// Gets a localized string similar to "The '{0}' element is missing or empty.".
		/// </summary>
		internal static string ScheduledTask_ReadXml_MissingElement
		{
			get { return GetResource("ScheduledTask_ReadXml_MissingElement"); }
		}

		/// <summary>
		/// Gets a localized string similar to "Day in month is smaller than -31 or greater than 31.".
		/// </summary>
		internal static string Schedule_AssertDayInMonth
		{
			get { return GetResource("Schedule_AssertDayInMonth"); }
		}

		/// <summary>
		/// Gets a localized string similar to "Interval must be positive.".
		/// </summary>
		internal static string Schedule_AssertDayMonthInterval
		{
			get { return GetResource("Schedule_AssertDayMonthInterval"); }
		}

		/// <summary>
		/// Gets a localized string similar to "Object is not initialized.".
		/// </summary>
		internal static string Schedule_AssertInitialized
		{
			get { return GetResource("Schedule_AssertInitialized"); }
		}

		/// <summary>
		/// Gets a localized string similar to "No active months specified.".
		/// </summary>
		internal static string Schedule_AssertMonths_NoMonths
		{
			get { return GetResource("Schedule_AssertMonths_NoMonths"); }
		}

		/// <summary>
		/// Gets a localized string similar to "Start time is either negative or greater than 24 hours.".
		/// </summary>
		internal static string Schedule_AssertStartTime
		{
			get { return GetResource("Schedule_AssertStartTime"); }
		}

		/// <summary>
		/// Gets a localized string similar to "Interval may not be negative.".
		/// </summary>
		internal static string Schedule_AssertTimeSpanInterval
		{
			get { return GetResource("Schedule_AssertTimeSpanInterval"); }
		}

		/// <summary>
		/// Gets a localized string similar to "No week day specified.".
		/// </summary>
		internal static string Schedule_AssertWeekDay_NoWeekDay
		{
			get { return GetResource("Schedule_AssertWeekDay_NoWeekDay"); }
		}

		/// <summary>
		/// Gets a localized string similar to "More than one week day specified.".
		/// </summary>
		internal static string Schedule_AssertWeekDay_TooManyWeekDays
		{
			get { return GetResource("Schedule_AssertWeekDay_TooManyWeekDays"); }
		}

		/// <summary>
		/// Gets a localized string similar to "The '{0}' attribute is invalid or empty.".
		/// </summary>
		internal static string Schedule_ParseEnumAttribute_InvalidAttribute
		{
			get { return GetResource("Schedule_ParseEnumAttribute_InvalidAttribute"); }
		}

		/// <summary>
		/// Gets a localized string similar to "The '{0}' attribute is missing.".
		/// </summary>
		internal static string Schedule_ReadXml_MissingAttribute
		{
			get { return GetResource("Schedule_ReadXml_MissingAttribute"); }
		}

		/// <summary>
		/// Gets a localized string similar to "Object is not initialized.".
		/// </summary>
		internal static string ScheduledTask_AssertActive_NotInit
		{
			get { return GetResource("ScheduledTask_AssertActive_NotInit"); }
		}

		/// <summary>
		/// Gets a localized string similar to "Interval may not be negative.".
		/// </summary>
		internal static string Scheduler_AssertInterval_Negative
		{
			get { return GetResource("Scheduler_AssertInterval_Negative"); }
		}

		/// <summary>
		/// Gets a localized string similar to "Timeout has elapsed, but at least one scheduled task is still active.".
		/// </summary>
		internal static string Scheduler_Stop_Timeout
		{
			get { return GetResource("Scheduler_Stop_Timeout"); }
		}

		/// <summary>
		/// Gets a localized string similar to "Timeout may not be negative.".
		/// </summary>
		internal static string Scheduler_Stop_TimeoutNegative
		{
			get { return GetResource("Scheduler_Stop_TimeoutNegative"); }
		}

		/// <summary>
		/// Gets a localized string similar to "Schedule element has no attributes.".
		/// </summary>
		internal static string Schedule_ReadXml_NoAttributes
		{
			get { return GetResource("Schedule_ReadXml_NoAttributes"); }
		}

		/// <summary>
		/// Gets a localized string similar to "Schedule type 'None' is not allowed.".
		/// </summary>
		internal static string Global_ScheduleType_None
		{
			get { return GetResource("Global_ScheduleType_None"); }
		}
		#endregion

#if WINDOWS_UWP
		#region Private fields
		private static Windows.ApplicationModel.Resources.Core.ResourceMap mResourceMap;
		private static Windows.ApplicationModel.Resources.Core.ResourceContext mContext;
		#endregion

		#region Properties
		/// <summary>
		/// Gets the main resource map of the assembly.
		/// </summary>
		internal static Windows.ApplicationModel.Resources.Core.ResourceMap ResourceMap
		{
			get
			{
				if (object.ReferenceEquals(mResourceMap, null))
				{
					mResourceMap = Windows.ApplicationModel.Resources.Core.ResourceManager.Current.MainResourceMap;
				}

				return mResourceMap;
			}
		}

		/// <summary>
		/// Gets or sets the resource context to use when retrieving resources.
		/// </summary>
		internal static Windows.ApplicationModel.Resources.Core.ResourceContext Context
		{
			get { return mContext; }
			set { mContext = value; }
		}
		#endregion

		#region Methods
		/// <summary>
		/// Retrieves a string resource using the resource map.
		/// </summary>
		/// <param name="name">The name of the string resource.</param>
		/// <returns>A localized string.</returns>
		internal static string GetResource(string name)
		{
			Windows.ApplicationModel.Resources.Core.ResourceContext context = Context;
			if (context == null)
			{
				context = Windows.ApplicationModel.Resources.Core.ResourceContext.GetForViewIndependentUse();
			}

			Windows.ApplicationModel.Resources.Core.ResourceCandidate resourceCandidate = ResourceMap.GetValue("NerdyDuck.Scheduler/Resources/" + name, context);

			if (resourceCandidate == null)
			{
				throw new ArgumentOutOfRangeException(nameof(name));
			}

			return resourceCandidate.ValueAsString;
		}
		#endregion
#endif

#if WINDOWS_DESKTOP
		#region Private fields
		private static System.Resources.ResourceManager mResourceManager;
		private static System.Globalization.CultureInfo mResourceCulture;
		#endregion

		#region Properties
		/// <summary>
		/// Returns the cached ResourceManager instance used by this class.
		/// </summary>
		[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
		internal static System.Resources.ResourceManager ResourceManager
		{
			get
			{
				if (object.ReferenceEquals(mResourceManager, null))
				{
					System.Resources.ResourceManager temp = new System.Resources.ResourceManager("NerdyDuck.Scheduler.Properties.Resources", typeof(Resources).Assembly);
					mResourceManager = temp;
				}
				return mResourceManager;
			}
		}

		/// <summary>
		/// Overrides the current thread's CurrentUICulture property for all resource lookups using this strongly typed resource class.
		/// </summary>
		[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
		internal static System.Globalization.CultureInfo Culture
		{
			get { return mResourceCulture; }
			set { mResourceCulture = value; }
		}
		#endregion

		#region Methods
		/// <summary>
		/// Retrieves a string resource using the resource manager.
		/// </summary>
		/// <param name="name">The name of the string resource.</param>
		/// <returns>A localized string.</returns>
		internal static string GetResource(string name)
		{
			return ResourceManager.GetString(name, mResourceCulture);
		}
		#endregion
#endif
	}
}
