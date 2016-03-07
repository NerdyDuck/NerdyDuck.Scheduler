#region Copyright
/*******************************************************************************
 * <copyright file="ScheduledActionBaseTest.cs" owner="Daniel Kopp">
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
 * <file name="ScheduledActionBaseTest.cs" date="2016-02-19">
 * Contains test methods to test the
 * NerdyDuck.Scheduler.ScheduledActionBase class.
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
using System.Xml;
using System.Xml.Serialization;

namespace NerdyDuck.Tests.Scheduler
{
	/// <summary>
	/// Contains test methods to test the NerdyDuck.Scheduler.ScheduledActionBase class.
	/// </summary>
#if WINDOWS_DESKTOP
	[ExcludeFromCodeCoverage]
#endif
	[TestClass]
	public class ScheduledActionBaseTest
	{
		[TestMethod]
		public void Ctor_Success()
		{
			FailableScheduledAction action = new FailableScheduledAction(FailableScheduledAction.DummyAction.Run);
		}

		[TestMethod]
		public void Dispose_Success()
		{
			FailableScheduledAction action = new FailableScheduledAction(FailableScheduledAction.DummyAction.Run);
			Assert.IsFalse(action.IsDisposedTest);
			action.Dispose();
			Assert.IsTrue(action.IsDisposedTest);
			action.Dispose();
		}

		[TestMethod]
		public void XmlSerializable_Success()
		{
			FailableScheduledAction action = new FailableScheduledAction(FailableScheduledAction.DummyAction.ThrowException);
			Assert.IsNull(((IXmlSerializable)action).GetSchema());
			using (XmlWriter writer = XmlWriter.Create(new MemoryStream()))
			{
				writer.WriteStartElement("action");
				action.WriteXml(writer);
				writer.WriteEndElement();
			}

			action = new FailableScheduledAction();
			using (XmlReader reader = XmlReader.Create(new StringReader("<?xml version=\"1.0\" encoding=\"utf-8\" ?><action behavior=\"ThrowException\" />")))
			{
				reader.ReadToFollowing("action");
				action.ReadXml(reader);
			}
		}
	}
}
