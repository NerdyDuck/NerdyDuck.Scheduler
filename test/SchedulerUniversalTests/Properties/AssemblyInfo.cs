#region Copyright
/*******************************************************************************
 * <copyright file="AssemblyInfo.cs" owner="Daniel Kopp">
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
 * <file name="AssemblyInfo.cs" date="2016-02-19">
 * Contains assembly-level properties.
 * </file>
 ******************************************************************************/
#endregion

using NerdyDuck.CodedExceptions;
using System;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;

// General information
[assembly: AssemblyTitle("NerdyDuck.Tests.Scheduler")]
[assembly: AssemblyDescription("Unit tests for NerdyDuck.Scheduler assembly.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("NerdyDuck")]
[assembly: AssemblyProduct("NerdyDuck Core Libraries")]
[assembly: AssemblyCopyright("Copyright © Daniel Kopp 2015-2016")]
[assembly: AssemblyTrademark("Covered by Apache License 2.0")]
[assembly: AssemblyCulture("")]
[assembly: NeutralResourcesLanguage("en-US")]
[assembly: ComVisible(false)]
[assembly: CLSCompliant(true)]
#if WINDOWS_UWP
[assembly: AssemblyMetadata("TargetPlatform", "UAP")]
#endif
#if WINDOWS_DESKTOP
[assembly: AssemblyMetadata("TargetPlatform", "AnyCPU")]
#endif

// Version information
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
