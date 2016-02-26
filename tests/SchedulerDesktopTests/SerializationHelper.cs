#region Copyright
/*******************************************************************************
 * <copyright file="SerializationHelper.cs" owner="Daniel Kopp">
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
 * <file name="SerializationHelper.cs" date="2016-02-19">
 * Contains methods to serialize and deserialize objects using the
 * SerializableAttribute and/or ISerializable.
 * </file>
 ******************************************************************************/
#endregion

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace NerdyDuck.Tests.Scheduler
{
	/// <summary>
	/// Contains methods to serialize and deserialize objects using the SerializableAttribute and/or ISerializable.
	/// </summary>
	[ExcludeFromCodeCoverage]
	public static class SerializationHelper
	{
		/// <summary>
		/// Serializes an object.
		/// </summary>
		/// <typeparam name="T">The type of object to serialize.</typeparam>
		/// <param name="ex">The exception to serialize.</param>
		/// <returns>A MemoryStream containing the serialized object data.</returns>
		public static MemoryStream Serialize<T>(T value) where T : class
		{
			BinaryFormatter formatter = new BinaryFormatter();
			MemoryStream buffer = new MemoryStream();
			formatter.Serialize(buffer, value);
			buffer.Seek(0, SeekOrigin.Begin);

			return buffer;
		}

		/// <summary>
		/// Deserializes an object.
		/// </summary>
		/// <typeparam name="T">The type of object to deserialize.</typeparam>
		/// <param name="buffer">The MemoryStream containing the serialized object data.</param>
		/// <returns>The deserialized object.</returns>
		public static T Deserialize<T>(MemoryStream buffer) where T : class
		{
			BinaryFormatter formatter = new BinaryFormatter();
			return (T)formatter.Deserialize(buffer);
		}
	}
}
