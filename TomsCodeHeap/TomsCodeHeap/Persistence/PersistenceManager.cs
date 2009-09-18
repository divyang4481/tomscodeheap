// ========================================================================
// File:     PersistenceManager.cs
//
// Author:   $Author$
// Date:     $LastChangedDate$
// Revision: $Revision$
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
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using CH.Froorider.Codeheap.Domain;

namespace CH.Froorider.Codeheap.Persistence
{
	/// <summary>
	/// The persistence manager class offers two generic methods to serialize and deserialize objects. The serialization method is
	/// implemented as an extension method. So it can be used / called directly on any object.
	/// </summary>
	public static class PersistenceManager
	{
		#region fields

		/// <summary>
		/// Common used file extension.
		/// </summary>
		private const string CommonFileNameExtension = ".dat";

		/// <summary>
		/// Path where to store the object (as files).
		/// </summary>
		private static readonly string CommonFilePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "//" + Assembly.GetExecutingAssembly().GetName().Name + "//";

		/// <summary>
		/// Locks the multithreaded access on the directory creation.
		/// </summary>
		private static volatile object filePathLock = new object();

		#endregion

		#region public methods (functionality)

		/// <summary>
		/// Serializes the specified object to an XML - File.
		/// </summary>
		/// <param name="objectToSerialize">The object to serialize. Must implement serializable.</param>
		/// <returns>The filename of the object (without filename extension) as an MD5 Hash string.</returns>
		/// <exception cref="System.InvalidOperationException">Is thrown when the object is not suited for serialization.</exception>
		/// <remarks>The serialized object is stored in the folder: 
		/// './DocumentsAndSettings/{UserName}/LocalSettings/ApplicationData/{ExecutingAssemblyName}/.
		/// The filename extension is '.dat'.</remarks>
		public static string Serialize(this BusinessObject objectToSerialize)
		{
			CreateDirectoryIfNotExisting();

			if (objectToSerialize.GetType().IsSerializable)
			{
				string filename = MD5HashGenerator.GenerateKey(objectToSerialize);
				XmlSerializer serializer = new XmlSerializer(objectToSerialize.GetType());
				using (TextWriter textWriter = new StreamWriter(CommonFilePath + filename + CommonFileNameExtension))
				{
					serializer.Serialize(textWriter, objectToSerialize);
					textWriter.Close();
				}

				objectToSerialize.ObjectIdentifier = filename;
				return filename;
			}
			else
			{
				throw new InvalidOperationException("Only types which are marked as Serializable are supported.");
			}
		}

		/// <summary>
		/// Serializes the specified object to an XML - File.
		/// </summary>
		/// <param name="objectToSerialize">The object to serialize. Must implement serializable.</param>
		/// <param name="filePath">The filepath where to store the file. Must be a valid Directorypath including an ending '/'.</param>
		/// <param name="extension">The extension to use including a point (means e.g. '.dat' or '.doc').</param>
		/// <returns>
		/// The filename of the object (without filename extension) as an MD5 Hash string.
		/// </returns>
		/// <remarks>
		/// Be sure that filepath and extension are fully fledged. That means e.g. filepath must be the whole directory name with an ending '/'. 
		/// And the extension must begin with a dot. Otherwise you will get some very strange results. This method does not check whether
		/// your input is really 'valid'. If the object can be serialized it is serialized. Wherever it may roam on the disk then!
		/// </remarks>
		/// <exception cref="System.InvalidOperationException">Is thrown when the object is not suited for serialization.</exception>
		/// <exception cref="ArgumentNullException">Is thrown when one of the input params is null.</exception>
		public static string Serialize(this BusinessObject objectToSerialize, string filePath, string extension)
		{
			if (string.IsNullOrEmpty(filePath))
			{
				throw new ArgumentNullException("filePath", "Filepath cannot be null.");
			}

			if (string.IsNullOrEmpty(extension))
			{
				throw new ArgumentNullException("extension", "Extension cannot be null.");
			}

			PersistenceManager.CreateDirectoryIfNotExisting(filePath);

			if (objectToSerialize.GetType().IsSerializable)
			{
				string filename = MD5HashGenerator.GenerateKey(objectToSerialize);
				XmlSerializer serializer = new XmlSerializer(objectToSerialize.GetType());
				using (TextWriter textWriter = new StreamWriter(filePath + filename + extension))
				{
					serializer.Serialize(textWriter, objectToSerialize);
					textWriter.Close();
				}

				objectToSerialize.ObjectIdentifier = filename;
				return filename;
			}
			else
			{
				throw new InvalidOperationException("Only types which are marked as Serializable are supported.");
			}
		}

		/// <summary>
		/// Deserializes an object.
		/// </summary>
		/// <typeparam name="T">Type of the object which you expect.</typeparam>
		/// <param name="fileName">The filename (without extension) of the file which contains the persistent data.</param>
		/// <returns>An object of desired type when the file could be deserialized.</returns>
		/// <exception cref="ArgumentNullException">Is thrown when one of the input parameters is null.</exception>
		public static T DeserializeObject<T>(string fileName)
		{
			if (String.IsNullOrEmpty(fileName))
			{
				throw new ArgumentNullException("fileName", "Filename cannot be null or empty.");
			}

			T deserializedObject;
			XmlSerializer serializer = new XmlSerializer(typeof(T));
			using (TextReader textReader = new StreamReader(CommonFilePath + fileName + CommonFileNameExtension))
			{
				deserializedObject = (T)serializer.Deserialize(textReader);
				textReader.Close();
			}

			return deserializedObject;
		}

		/// <summary>
		/// Deserializes an object at the specified path with the specified file extension.
		/// </summary>
		/// <typeparam name="T">Type of the object which you expect.</typeparam>
		/// <param name="fileName">The filename (without extension) of the file which contains the persistent data.</param>
		/// <param name="filePath">The path where the file should be.</param>
		/// <param name="extension">The extension the file should have.</param>
		/// <returns>
		/// An object of desired type when the file could be deserialized.
		/// </returns>
		/// <exception cref="ArgumentNullException">Is thrown when one of the input parameters is null.</exception>
		public static T DeserializeObject<T>(string fileName, string filePath, string extension)
		{
			if (String.IsNullOrEmpty(fileName))
			{
				throw new ArgumentNullException("fileName", "Filename cannot be null or empty.");
			}

			if (String.IsNullOrEmpty(filePath))
			{
				throw new ArgumentNullException("filePath", "FilePath cannot be null or empty.");
			}

			if (String.IsNullOrEmpty(extension))
			{
				throw new ArgumentNullException("extension", "Extension cannot be null or empty.");
			}

			T deserializedObject;
			XmlSerializer serializer = new XmlSerializer(typeof(T));
			using (TextReader textReader = new StreamReader(filePath + fileName + extension))
			{
				deserializedObject = (T)serializer.Deserialize(textReader);
				textReader.Close();
			}

			return deserializedObject;
		}

		#endregion

		#region private methods

		/// <summary>
		/// Creates the common directory if not existing.
		/// </summary>
		private static void CreateDirectoryIfNotExisting()
		{
			lock (filePathLock)
			{
				if (!Directory.Exists(CommonFilePath))
				{
					Directory.CreateDirectory(CommonFilePath);
				}
			}
		}

		/// <summary>
		/// Creates the directory if not existing.
		/// </summary>
		/// <param name="directoryPath">The directory path.</param>
		private static void CreateDirectoryIfNotExisting(string directoryPath)
		{
			lock (filePathLock)
			{
				if (!Directory.Exists(directoryPath))
				{
					Directory.CreateDirectory(directoryPath);
				}
			}
		}

		#endregion
	}
}
