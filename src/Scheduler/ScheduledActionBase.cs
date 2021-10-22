#region Copyright
/*******************************************************************************
 * <copyright file="ScheduledActionBase.cs" owner="Daniel Kopp">
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
 * <file name="ScheduledActionBase.cs" date="2016-02-02">
 * The base class for an action that can be executed by the Scheduler.
 * </file>
 ******************************************************************************/
#endregion

using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace NerdyDuck.Scheduler
{
	/// <summary>
	/// The base class for an action that can be executed by the <see cref="Scheduler{T}"/>.
	/// </summary>
	[XmlRoot(ElementName = RootName, Namespace = Schedule.Namespace)]
	public abstract class ScheduledActionBase : IDisposable, IXmlSerializable
	{
		#region Constants
		internal const string RootName = "scheduledAction";
		#endregion

		#region Private fields
		private bool mIsDisposed;
		#endregion

		#region Properties
		/// <summary>
		/// Gets a value indicating if the instance is disposed.
		/// </summary>
		/// <value><see langword="true"/>, if the object was previously disposed; otherwise, <see langword="false"/>.</value>
		/// <remarks>Use this property when overriding the <see cref="Dispose(bool)"/> method to determine if the object was already disposed.</remarks>
		protected bool IsDisposed
		{
			get { return mIsDisposed; }
		}
		#endregion

		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="ScheduledActionBase"/> class.
		/// </summary>
		protected ScheduledActionBase()
		{
			mIsDisposed = false;
		}
		#endregion

		#region Destructor
		/// <summary>
		/// Destructor.
		/// </summary>
		~ScheduledActionBase()
		{
			Dispose(false);
		}
		#endregion

		#region Public methods
		#region Run
		/// <summary>
		/// When implemented by a deriving class, executes the task that was scheduled.
		/// </summary>
		/// <param name="lastExecuted">The date and time of the last execution of the action.</param>
		/// <param name="operation">Represents the asynchronous operation that the method can observe while executed.</param>
		/// <returns>A state object, or <see langword="null"/>.</returns>
		/// <remarks>Use <paramref name="operation"/> to check if the method should be canceled, and throw <see cref="OperationCanceledException"/> to notify the cancellation.</remarks>
		public abstract object Run(DateTimeOffset? lastExecuted, ISchedulerAction operation);
		#endregion

		#region IDisposable implementation
		/// <summary>
		/// When overridden by a deriving class, releases allocated resources.
		/// </summary>
		/// <param name="disposing">A value indicating if the method was called by user code. If <see langword="false"/>, the method was called by the runtime in the finalizer.</param>
		/// <remarks>If <paramref name="disposing"/> is <see langword="false"/>, no other objects should be referenced.</remarks>
		protected virtual void Dispose(bool disposing)
		{
			if (mIsDisposed)
				return;

			mIsDisposed = true;
		}

		/// <summary>
		/// Releases allocated resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		#endregion

		#region IXmlSerializable implementation
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
		#endregion
		#endregion
	}
}
