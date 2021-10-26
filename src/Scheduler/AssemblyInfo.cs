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
[assembly: InternalsVisibleTo("NerdyDuck.Tests.Scheduler, PublicKey=002400000480000094000000060200000024000052534131000400000100010039d4dd610ad15d27fa2b0b9a496958c92032483d52bd0d1bee4c8035a23b296d05196fa5a16b22607821f6ef0e54fc3ddb7a2bd1e5d695be7b591071f076cb33402de259bb16eb51a3d1553119a5996778c2254d3ebdca73099c6483692dd379822bd73c8a9af4e6dfc446298a29aab411f35a89b37663852d0a31cfafcff8dd")]
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
[assembly: AssemblyVersion("1.0.4.0")]
[assembly: AssemblyFileVersion("1.0.4.0")]
[assembly: AssemblyInformationalVersion("1.0.4")]

