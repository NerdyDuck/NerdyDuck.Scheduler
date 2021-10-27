#region Copyright
/*******************************************************************************
 * NerdyDuck.Scheduler - Task scheduler for .NET with variable schedules.
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

using System.Threading;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace NerdyDuck.Scheduler;

/// <summary>
/// The base class for an action that can be executed by the <see cref="Scheduler{T}"/>.
/// </summary>
[XmlRoot(ElementName = RootName, Namespace = Schedule.Namespace)]
public abstract class ScheduledActionBase : IDisposable, IXmlSerializable
{
	internal const string RootName = "scheduledAction";

	private int _isDisposed;

	/// <summary>
	/// Gets a value indicating if the instance is disposed.
	/// </summary>
	/// <value><see langword="true"/>, if the object was previously disposed; otherwise, <see langword="false"/>.</value>
	/// <remarks>Use this property when overriding the <see cref="Dispose(bool)"/> method to determine if the object was already disposed.</remarks>
	protected int IsDisposed => _isDisposed;

	/// <summary>
	/// Initializes a new instance of the <see cref="ScheduledActionBase"/> class.
	/// </summary>
	protected ScheduledActionBase()
	{
		_isDisposed = 0;
	}

	/// <summary>
	/// Destructor.
	/// </summary>
	~ScheduledActionBase()
	{
		Dispose(false);
	}

	/// <summary>
	/// When implemented by a deriving class, executes the task that was scheduled.
	/// </summary>
	/// <param name="lastExecuted">The date and time of the last execution of the action.</param>
	/// <param name="operation">Represents the asynchronous operation that the method can observe while executed.</param>
	/// <returns>A state object, or <see langword="null"/>.</returns>
	/// <remarks>Use <paramref name="operation"/> to check if the method should be canceled, and throw <see cref="OperationCanceledException"/> to notify the cancellation.</remarks>
	public abstract object Run(DateTimeOffset? lastExecuted, ISchedulerAction operation);

	/// <summary>
	/// When overridden by a deriving class, releases allocated resources.
	/// </summary>
	/// <param name="disposing">A value indicating if the method was called by user code. If <see langword="false"/>, the method was called by the runtime in the finalizer.</param>
	/// <remarks>If <paramref name="disposing"/> is <see langword="false"/>, no other objects should be referenced.</remarks>
	protected virtual void Dispose(bool disposing)
	{
		if (Interlocked.Exchange(ref _isDisposed, 1) == 1)
		{
			return;
		}
	}

	/// <summary>
	/// Releases allocated resources.
	/// </summary>
	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	/// <summary>
	/// Reserved method.
	/// </summary>
	/// <returns>Always returns <see langword="null"/>.</returns>
	XmlSchema IXmlSerializable.GetSchema()
	{
		return null;
	}

	/// <summary>
	/// When overridden by a deriving class, reads the <see cref="ScheduledActionBase"/> configuration from the specified <paramref name="reader"/>.
	/// </summary>
	/// <param name="reader">A <see cref="XmlReader"/> containing a serialized instance of a <see cref="ScheduledActionBase"/>.</param>
	public virtual void ReadXml(XmlReader reader)
	{
	}

	/// <summary>
	/// When overridden by a deriving class, writes the configuration of the <see cref="ScheduledActionBase"/> to the specified <paramref name="writer"/>.
	/// </summary>
	/// <param name="writer">A <see cref="XmlWriter"/> that receives the configuration data.</param>
	public virtual void WriteXml(XmlWriter writer)
	{
	}
}
