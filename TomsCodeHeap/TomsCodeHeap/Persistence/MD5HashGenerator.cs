// ========================================================================
// File:     MD5HashGenerator.cs
//
// Author:   $Author$
// Date:     $LastChangedDate: 2011-11-08 22:31:40 +0100 (Di, 08 Nov 2011) $
// Revision: $Revision: 79 $
// ========================================================================
// Copyright [2009] [$Author$]
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ========================================================================

using System;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace CH.Froorider.Codeheap.Persistence
{
	/// <summary>
	/// This class takes an object, and generates a key to it. There are several possibilities:
	/// This generator can generate keys of type integer,float,double. The generated key is not necessarly
	/// unique.
	/// </summary>
	public sealed class MD5HashGenerator
	{
		#region Constructors

		/// <summary>
		/// Prevents a default instance of the <see cref="MD5HashGenerator"/> class from being created.
		/// </summary>
		private MD5HashGenerator()
		{
		}

		#endregion

		#region public methods

		/// <summary>
		/// Generates a hashed - key for an instance of a class.
		/// The hash is a classic MD5 hash (e.g. BF20EB8D2C4901112179BF5D242D996B). So you can distinguish different 
		/// instances of a class. Because the object is hashed on the internal state, you can also hash it, then send it to
		/// someone in a serialized way. Your client can then deserialize it and check if it is in
		/// the same state.
		/// The method just just estimates that the object implements the ISerializable interface. What's
		/// needed to save the state or so, is up to the implementer of the interface.
		/// <b>The method is thread-safe.</b>
		/// </summary>
		/// <param name="valueToSerialize">The object you'd like to have a key out of it.</param>
		/// <returns>An string representing a MD5 Hashkey corresponding to the object or null if the object couldn't be serialized.</returns>
		/// <exception cref="ArgumentNullException">Is thrown when the given object reference is null.</exception>
		/// <exception cref="SerializationException">Is thrown when the object cannot be serialized.</exception>
		/// <exception cref="SecurityException">Is thrown when the caller has not the needed permissions to serialize the object.</exception>
		/// <exception cref="ObjectDisposedException">Is thrown when the object to serialize has already been disposed.</exception>
		public static string GenerateKey(object valueToSerialize)
		{
			string hashString = string.Empty;

			// Sanity checks
			if (valueToSerialize == null)
			{
				throw new ArgumentNullException("valueToSerialize", "Null as parameter is not allowed.");
			}

			if (!valueToSerialize.GetType().IsSerializable)
			{
				throw new SerializationException("Given object is not serializable.");
			}

			// Now we begin to do the real work.
			hashString = ComputeHash(ObjectToByteArray(valueToSerialize));
			return hashString;
		}

		#endregion

		#region private methods

		/// <summary>
		/// Converts an object to an array of bytes. This array is used to hash the object.
		/// </summary>
		/// <param name="objectToSerialize">Any object which should be transformed ot a byte array.</param>
		/// <returns>A byte - array representation of the object.</returns>
		/// <exception cref="ArgumentNullException">Is thrown when the object to serialize or the stream is null.</exception>
		/// <exception cref="SerializationException">Is thrown if something went wrong during serialization.</exception>
		/// <exception cref="SecurityException">Is thrown when the caller has not the needed permissions to serialize the object to the stream.</exception>
		private static byte[] ObjectToByteArray(object objectToSerialize)
		{
            using (MemoryStream fs = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();

                try
                {
                    formatter.Serialize(fs, objectToSerialize);
                    return fs.ToArray();
                }
                catch (SerializationException se)
                {
                    Console.WriteLine("Error occured during serialization. Message: " + se.Message);
                    throw;
                }
                catch (SecurityException sece)
                {
                    Console.WriteLine("Error occured during serialization. Message: " + sece.Message);
                    throw;
                }
            }
		}

		/// <summary>
		/// Generates the hashcode of an given byte-array. The byte-array can be an object. Then the
		/// method "hashes" this object. The hash can then be used e.g. to identify the object.
		/// </summary>
		/// <param name="objectAsBytes">Bytearray representation of an object.</param>
		/// <returns>The MD5 hash of the object as a string or null if it couldn't be generated.</returns>
		/// <exception cref="ArgumentNullException">Is thrown when the given byte array is null.</exception>
		/// <exception cref="ObjectDisposedException">Is thrown when the byte array has already been disposed.</exception>
		private static string ComputeHash(byte[] objectAsBytes)
		{
            using (MD5 md5 = new MD5CryptoServiceProvider())
            {
                try
                {
                    byte[] result = md5.ComputeHash(objectAsBytes);

                    // Build the final string by converting each byte
                    // into hex and appending it to a StringBuilder
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < result.Length; i++)
                    {
                        sb.Append(result[i].ToString("X2", CultureInfo.InvariantCulture));
                    }

                    return sb.ToString();
                }
                catch (ArgumentNullException ane)
                {
                    Console.WriteLine("Hash has not been generated. Cause: " + ane.Message);
                    throw;
                }
                catch (ObjectDisposedException ode)
                {
                    Console.WriteLine("Hash has not been generated. Cause: " + ode.Message);
                    throw;
                }
            }
		}

		#endregion
	}
}
