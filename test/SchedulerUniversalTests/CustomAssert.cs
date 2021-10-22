#region Copyright
/*******************************************************************************
 * <copyright file="CustomAssert.cs" owner="Daniel Kopp">
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
 * <file name="CustomAssert.cs" date="2016-02-19">
 * Verifies conditions in unit tests using true/false propositions.
 * </file>
 ******************************************************************************/
#endregion

#if WINDOWS_UWP
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#endif
#if WINDOWS_DESKTOP
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
#endif
using System;

namespace NerdyDuck.Tests.Scheduler
{
	/// <summary>
	/// Verifies conditions in unit tests using true/false propositions.
	/// </summary>
#if WINDOWS_DESKTOP
	[ExcludeFromCodeCoverage]
#endif
	public static class CustomAssert
	{
#if WINDOWS_UWP
		public static T ThrowsException<T>(Action action) where T : Exception
		{
			return Assert.ThrowsException<T>(action);
		}

		public static T ThrowsException<T>(Func<object> action) where T : Exception
		{
			return Assert.ThrowsException<T>(action);
		}

		public static T ThrowsException<T>(Action action, string message) where T : Exception
		{
			return Assert.ThrowsException<T>(action, message);
		}

		public static T ThrowsException<T>(Func<object> action, string message) where T : Exception
		{
			return Assert.ThrowsException<T>(action, message);
		}

		public static T ThrowsException<T>(Action action, string message, params object[] parameters) where T : Exception
		{
			return Assert.ThrowsException<T>(action, message, parameters);
		}
#endif

#if WINDOWS_DESKTOP
		#region Public methods
		public static T ThrowsException<T>(Action action) where T : Exception
		{
			return ThrowsException<T>(action, string.Empty, null);
		}

		public static T ThrowsException<T>(Func<object> action) where T : Exception
		{
			return ThrowsException<T>(action, string.Empty, null);
		}

		public static T ThrowsException<T>(Action action, string message) where T : Exception
		{
			return ThrowsException<T>(action, message, null);
		}

		public static T ThrowsException<T>(Func<object> action, string message) where T : Exception
		{
			return ThrowsException<T>(action, message, null);
		}

		public static T ThrowsException<T>(Action action, string message, params object[] parameters) where T : Exception
		{
			string str = string.Empty;
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			if (message == null)
			{
				throw new ArgumentNullException("message");
			}

			try
			{
				action();
			}
			catch (Exception exception)
			{
				if (!typeof(T).Equals(exception.GetType()))
				{
					object[] objArray1 = new object[] { (message == null) ? string.Empty : ReplaceNulls(message), typeof(T).Name, exception.GetType().Name, exception.Message, exception.StackTrace };
					str = string.Format((IFormatProvider)CultureInfo.CurrentCulture, "Threw exception {2}, but exception {1} was expected. {0}\nException Message: {3}\nStack Trace: {4}", objArray1);
					HandleFail("Assert.ThrowsException", str, parameters);
				}
				return (T)exception;
			}

			object[] objArray2 = new object[] { (message == null) ? string.Empty : ReplaceNulls(message), typeof(T).Name };
			str = string.Format((IFormatProvider)CultureInfo.CurrentCulture, "No exception thrown. {1} exception was expected. {0}", objArray2);
			HandleFail("Assert.ThrowsException", str, parameters);
			return default(T);
		}

		public static T ThrowsException<T>(Func<object> action, string message, params object[] parameters) where T : Exception
		{
			return ThrowsException<T>(delegate { action(); }, message, parameters);
		}
		#endregion

		#region Private methods
		private static void HandleFail(string assertionName, string message, params object[] parameters)
		{
			string str = string.Empty;
			if (!string.IsNullOrEmpty(message))
			{
				if (parameters == null)
				{
					str = ReplaceNulls(message);
				}
				else
				{
					str = string.Format((IFormatProvider)CultureInfo.CurrentCulture, ReplaceNulls(message), parameters);
				}
			}
			object[] objArray1 = new object[] { assertionName, str };
			throw new AssertFailedException(string.Format((IFormatProvider)CultureInfo.CurrentCulture, "{0} failed. {1}", objArray1));
		}


		private static string ReplaceNulls(object input)
		{
			if (input == null)
			{
				return "(null)";
			}
			string str = input.ToString();
			if (str == null)
			{
				return "(object)";
			}
			return Assert.ReplaceNullChars(str);
		}
		#endregion
#endif
	}
}
