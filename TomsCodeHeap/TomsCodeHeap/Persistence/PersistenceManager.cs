//$Author$
//$Id$
//$LastChangedBy$
//$LastChangedDate$
//$Revision$
//$Header$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using ch.froorider.codeheap.Domain;

namespace ch.froorider.codeheap.Persistence
{
    /// <summary>
    /// The persistence manager class offers two generic methods to serialize and deserialize objects. The serialization method is
    /// implemented as an extension method. So it can be used / called directly on any object.
    /// </summary>
    public static class PersistenceManager
    {
        #region fields

        private static readonly string fileNameExtension = ".GDC";
        private static readonly string filePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\NGoogleCalendar\\";

        #endregion

        #region constructors

        /// <summary>
        /// Constructor. Inits some things like the directories etc.
        /// </summary>
        static PersistenceManager()
        {
            lock (filePath)
            {
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
            }
        }

        #endregion

        #region public methods (functionality)

        /// <summary>
        /// Serializes the specified object to an XML - File.
        /// The mthod returns as result the filename (without extension) where the contents of the object has been saved.
        /// </summary>
        /// <param name="objectToSerialize">The object to serialize. Must implement Serializable.</param>
        /// <returns>The content of the object as an MD5 Hash string.</returns>
        /// <exception cref="System.InvalidOperationException">Is thrown when the object is not suited for serialization.</exception>
        public static string Serialize(this BusinessObject objectToSerialize)
        {
            if (objectToSerialize.GetType().IsSerializable)
            {
                string filename = MD5HashGenerator.GenerateKey(objectToSerialize);
                XmlSerializer serializer = new XmlSerializer(objectToSerialize.GetType());
                using (TextWriter textWriter = new StreamWriter(filePath + filename + fileNameExtension))
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
        /// <param name="filename">The filename (without extension) of the file which contains the persistent data.</param>
        /// <returns>An object of desired type when the file could be deserialized.</returns>
        public static T DeserializeObject<T>(string filename)
        {
            if (String.IsNullOrEmpty(filename))
            {
                throw new ArgumentException("Filename cannot be null or empty");
            }

            T deserializedObject;
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (TextReader textReader = new StreamReader(filePath + filename + fileNameExtension))
            {
                deserializedObject = (T)serializer.Deserialize(textReader);
                textReader.Close();
            }
            return deserializedObject;
        }

        #endregion
    }
}
