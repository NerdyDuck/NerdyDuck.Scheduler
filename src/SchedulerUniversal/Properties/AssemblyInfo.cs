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
 * <assembly name="NerdyDuck.Scheduler">
 * Task scheduler for .NET with variable schedules.
 * </assembly>
 * <file name="AssemblyInfo.cs" date="2016-03-02">
 * Contains assembly-level properties.
 * </file>
 ******************************************************************************/
#endregion

using NerdyDuck.CodedExceptions;
using System;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General information
[assembly: AssemblyTitle("NerdyDuck.Scheduler")]
[assembly: AssemblyDescription("Task scheduler for .NET with variable schedules.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("NerdyDuck")]
[assembly: AssemblyProduct("NerdyDuck Core Libraries")]
[assembly: AssemblyCopyright("Copyright © Daniel Kopp 2015-2016")]
[assembly: AssemblyTrademark("Covered by Apache License 2.0")]
[assembly: AssemblyCulture("")]
[assembly: NeutralResourcesLanguage("en-US")]
[assembly: InternalsVisibleTo("NerdyDuck.Tests.Scheduler, PublicKey=002400000480000094000000060200000024000052534131000400000100010027ea12fb39924671c562cc60e894c4b7d185a0d61c18a778022e8e5cf2688990c841e0d397904b8e6b3688f7e99966f7a7f0f4ead7e4abb3bc343f17d45ca05cdc3d86f72646be82c9640e1b2c79339e572699c47745cba4e6ae2e9106956a3da9577cb65add1e2c9d0df679baea4d9e4c6bf9494740ab8d05320083a812c2b0")]
[assembly: ComVisible(false)]
[assembly: CLSCompliant(true)]
[assembly: AssemblyFacilityIdentifier(0x0003)]
#if WINDOWS_UWP
[assembly: AssemblyMetadata("TargetPlatform", "UAP")]
#endif
#if WINDOWS_DESKTOP
[assembly: AssemblyMetadata("TargetPlatform", "AnyCPU")]
#endif

// Version information
[assembly: AssemblyVersion("1.0.2.0")]
[assembly: AssemblyFileVersion("1.0.2.0")]
[assembly: AssemblyInformationalVersion("1.0.2")]

