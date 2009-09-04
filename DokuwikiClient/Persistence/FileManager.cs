// ========================================================================
// File:     FileManager.cs
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using CH.Froorider.Codeheap.Domain;

namespace DokuwikiClient.Persistence
{
	/// <summary>
	/// Class which maintains the stored files / business objects.
	/// </summary>
	internal class FileManager
	{
		#region fields

		private static readonly string registryPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "//codeheap//";
		
		private static readonly string registryFile = "Registry.dat";
		
		private Registry registry;
		
		#endregion

		#region properties

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="FileManager"/> class.
		/// </summary>
		public FileManager()
		{
			this.CreateDirectoryIfNotExisting();
			this.LoadRegistry();
		}

		#endregion

		#region methods

		/// <summary>
		/// Registers the specified object to register.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="objectToRegister">The object to register.</param>
		public void Register<T>(T objectToRegister) where T : BusinessObject
		{
			this.registry.AddWikiObject(objectToRegister);
			this.SaveRegistry();
		}

		/// <summary>
		/// Creates the directory if not existing.
		/// </summary>
		private void CreateDirectoryIfNotExisting()
		{
			if (!Directory.Exists(registryPath))
			{
				Directory.CreateDirectory(registryPath);
			}

			if (!File.Exists(FileManager.registryPath + FileManager.registryFile))
			{
				FileStream stream = File.Create(FileManager.registryPath + FileManager.registryFile);
				StreamWriter writer = new StreamWriter(stream);
				writer.WriteLine("<?xml version='1.0' encoding = 'utf-8' ?>");
				writer.WriteLine("<Registry>");
				writer.WriteLine("</Registry>");
				writer.Close();
				stream.Close();
			}
		}

		/// <summary>
		/// Loads the registry.
		/// </summary>
		private void LoadRegistry()
		{
			XmlTextReader reader = new XmlTextReader(FileManager.registryPath + FileManager.registryFile);
			XmlSerializer serializer = new XmlSerializer(typeof(Registry));
			registry = serializer.Deserialize(reader) as Registry;
			reader.Close();
		}

		/// <summary>
		/// Saves the registry.
		/// </summary>
		private void SaveRegistry()
		{
			XmlTextWriter writer = new XmlTextWriter(FileManager.registryPath + FileManager.registryFile,Encoding.UTF8);
			XmlSerializer serializer = new XmlSerializer(typeof(Registry));
			serializer.Serialize(writer,registry);
			writer.Close();
		}

		#endregion
	}
}
