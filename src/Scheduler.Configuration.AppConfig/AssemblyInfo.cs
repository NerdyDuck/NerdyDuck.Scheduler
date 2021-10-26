#region Copyright
/*******************************************************************************
 * NerdyDuck.Scheduler.Configuration - Configures schedules implemented
 * with NerdyDuck.Scheduler.
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

global using System;
global using System.Collections.Generic;
global using System.ComponentModel;
global using System.Configuration;
global using System.Globalization;

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: CLSCompliant(true)]
[assembly: ComVisible(true)]
[assembly: AssemblyTrademark("Covered by MIT License")]
[assembly: InternalsVisibleTo("NerdyDuck.Tests.Scheduler.Configuration, PublicKey=002400000480000094000000060200000024000052534131000400000100010039d4dd610ad15d27fa2b0b9a496958c92032483d52bd0d1bee4c8035a23b296d05196fa5a16b22607821f6ef0e54fc3ddb7a2bd1e5d695be7b591071f076cb33402de259bb16eb51a3d1553119a5996778c2254d3ebdca73099c6483692dd379822bd73c8a9af4e6dfc446298a29aab411f35a89b37663852d0a31cfafcff8dd")]
