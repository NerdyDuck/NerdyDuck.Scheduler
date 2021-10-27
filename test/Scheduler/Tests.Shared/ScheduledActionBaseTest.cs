#region Copyright
/*******************************************************************************
 * NerdyDuck.Tests.Scheduler - Unit tests for the
 * NerdyDuck.Scheduler assembly
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

using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace NerdyDuck.Tests.Scheduler
{
	/// <summary>
	/// Contains test methods to test the NerdyDuck.Scheduler.ScheduledActionBase class.
	/// </summary>
	[ExcludeFromCodeCoverage]
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
