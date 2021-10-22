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
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace NerdyDuck.Tests.Scheduler;

/// <summary>
/// Contains methods to serialize and deserialize objects using the SerializableAttribute and/or ISerializable.
/// </summary>
[ExcludeFromCodeCoverage]
public static class SerializationHelper
{
#pragma warning disable IDE0079 // that next suppression is not unnecessary!
#pragma warning disable SYSLIB0011 // the only simple way for serialization is still binary
	/// <summary>
	/// Serializes an object.
	/// </summary>
	/// <typeparam name="T">The type of class to serialize.</typeparam>
	/// <param name="ex">The object to serialize.</param>
	/// <returns>A MemoryStream containing the serialized object data.</returns>
	public static MemoryStream Serialize<T>(T ex) where T : class
	{
		BinaryFormatter formatter = new();
		MemoryStream buffer = new();
		formatter.Serialize(buffer, ex);
		_ = buffer.Seek(0, SeekOrigin.Begin);

		return buffer;
	}

	/// <summary>
	/// Deserializes an object.
	/// </summary>
	/// <typeparam name="T">The type of class to deserialize.</typeparam>
	/// <param name="buffer">The MemoryStream containing the serialized object data.</param>
	/// <returns>The deserialized object.</returns>
	public static T Deserialize<T>(MemoryStream buffer) where T : class
	{
		BinaryFormatter formatter = new();
		return (T)formatter.Deserialize(buffer);
	}
#pragma warning restore SYSLIB0011
#pragma warning restore IDE0079

	public static void InvokeSerializationConstructorWithNullContext(Type type)
	{
		if (type == null)
		{
			throw new ArgumentNullException(nameof(type));
		}

		ConstructorInfo ci = type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, CallingConventions.HasThis, new Type[] { typeof(SerializationInfo), typeof(StreamingContext) }, null);
		try
		{
			_ = ci.Invoke(new object[] { null, new StreamingContext() });
		}
		catch (TargetInvocationException ex)
		{
			throw ex.InnerException;
		}
	}
}
