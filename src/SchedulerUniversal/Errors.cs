#region Copyright
/*******************************************************************************
 * <copyright file="Errors.cs" owner="Daniel Kopp">
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
 * <file name="Errors.cs" date="2016-02-04">
 * Provides easy access to the assembly's facility id and base HRESULT code.
 * </file>
 ******************************************************************************/
#endregion

using NerdyDuck.CodedExceptions;
using System;

namespace NerdyDuck.Scheduler
{
	/// <summary>
	/// Provides easy access to the assembly's facility id and base HRESULT code.
	/// </summary>
	internal static class Errors
	{
		#region Private fields
		// Load the facility id lazy and thread-safe.
		private static readonly Lazy<int> mFacilityId = new Lazy<int>(() =>
		{
			int Identifier = 0;
			// Check for override in app.config file
			if (!AssemblyFacilityIdentifierAttribute.TryGetOverride(typeof(Errors), out Identifier))
			{
				// Try to get identifier from assembly attribute
				AssemblyFacilityIdentifierAttribute IdentifierAttribute = AssemblyFacilityIdentifierAttribute.FromType(typeof(Errors));
				if (IdentifierAttribute != null)
					Identifier = IdentifierAttribute.FacilityId;
			}

			return Identifier;
		});

		private static readonly Lazy<int> mHResultBase = new Lazy<int>(() => HResultHelper.GetBaseHResult(mFacilityId.Value));
		#endregion

		#region Properties
		/// <summary>
		/// Gets the facility identifier of the current assembly.
		/// </summary>
		/// <value>The facility identifier, or 0, if no <see cref="AssemblyFacilityIdentifierAttribute"/> was found on the current assembly.</value>
		/// <remarks>See the <a href="http://msdn.microsoft.com/en-us/library/cc231198.aspx">HRESULT definition at MSDN</a> for
		/// more information about the definition of HRESULT values.</remarks>
		internal static int FacilityId
		{
			get { return mFacilityId.Value; }
		}

		/// <summary>
		/// Gets the base HRESULT value of the current assembly.
		/// </summary>
		/// <value>The base HRESULT value, or 0xa0000000, if no <see cref="AssemblyFacilityIdentifierAttribute"/> was found on the current assembly.</value>
		/// <remarks>See the <a href="http://msdn.microsoft.com/en-us/library/cc231198.aspx">HRESULT definition at MSDN</a> for
		/// more information about the definition of HRESULT values.</remarks>
		internal static int HResultBase
		{
			get { return mHResultBase.Value; }
		}
		#endregion

		#region Public methods
		/// <summary>
		/// Combines the specified error identifier with the base HRESULT value for this assembly.
		/// </summary>
		/// <param name="errorId">The error identifier to add to the base HRESULT value.</param>
		/// <returns>A custom HRESULT value, combined from <paramref name="errorId"/> and <see cref="HResultBase"/>.</returns>
		/// <remarks>See the <a href="http://msdn.microsoft.com/en-us/library/cc231198.aspx">HRESULT definition at MSDN</a> for
		/// more information about the definition of HRESULT values.</remarks>
		internal static int CreateHResult(int errorId)
		{
			return mHResultBase.Value | errorId;
		}
		#endregion
	}
}
