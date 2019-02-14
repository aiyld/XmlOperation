// Writer: Winter Yang
// Mail: 1161226280@qq.com
// All rights reserved.
using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace XmlOperation.LiDongYang
{
    public static class XmlSerialization
    {
        /// <summary>
        /// Serialize object and saving to file.
        /// </summary>
        /// <typeparam name="T">Object type.</typeparam>
        /// <param name="obj">Input object.</param>
        /// <param name="path">Xml file path.</param>
        public static void Serialize<T>(T obj, string path)
        {
            FileStream stream;

            if (!File.Exists(path))
            {
                stream = new FileStream(path, FileMode.Create, FileAccess.Write);
            }
            else
            {
                stream = new FileStream(path, FileMode.Truncate, FileAccess.ReadWrite);
            }

            using (stream)
            {
                stream.SetLength(0);

                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(T));

                    serializer.Serialize(stream, obj, new XmlSerializerNamespaces());
                }
                catch (Exception ex)
                {
                    Console.WriteLine("SerilizeAnObject Exception: {0}", ex.Message);
                }
            }
        }

        /// <summary>
        /// Serialize object to string.
        /// </summary>
        /// <typeparam name="T">Object type.</typeparam>
        /// <param name="obj">Input object.</param>
        /// <returns>Xml string</returns>
        public static string Serialize<T>(T obj)
        {
            using (StringWriter writer = new StringWriter())
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));

                serializer.Serialize(writer, obj);

                string result = writer.ToString();

                if (string.IsNullOrWhiteSpace(result))
                {
                    return string.Empty;
                }

                XmlDocument document = new XmlDocument();
                document.LoadXml(writer.ToString());

                return document.ChildNodes[1].OuterXml;
            }

        }

        /// <summary>
        /// Deserialize object from xml file.
        /// </summary>
        /// <typeparam name="T">Object type.</typeparam>
        /// <param name="path">Xml file path.</param>
        /// <returns>Result object.</returns>
        public static T Deserialize<T>(string path)
        {
            T obj = default(T);

            try
            {
                using (FileStream stream = new FileStream(path, FileMode.Open))
                {
                    XmlReader reader = new XmlTextReader(stream);
                    XmlSerializer serializer = new XmlSerializer(typeof(T));

                    obj = (T)serializer.Deserialize(reader);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("DeserilizeAnObject Exception: {0}", ex.Message);
            }

            return obj;
        }

        /// <summary>
        /// Deserialize object from xml stream.
        /// </summary>
        /// <typeparam name="T">Object type.</typeparam>
        /// <param name="stream">Xml file stream.</param>
        /// <returns>Result object.</returns>
        public static T Deserialize<T>(Stream stream)
        {
            T obj = default(T);

            try
            {
                using (stream)
                {
                    obj = stream.FromXmlStream<T>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("DeserilizeAnObject Exception: {0}", ex.Message);
            }

            return obj;
        }

        /// <summary>
        /// Deserialize object from xml string.
        /// </summary>
        /// <typeparam name="T">Object type.</typeparam>
        /// <param name="dataString">Xml string</param>
        /// <returns>Result object.</returns>
        public static T DeserializeFromString<T>(string dataString)
        {
            using (StringReader reader = new StringReader(dataString))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));

                return (T)serializer.Deserialize(reader);
            }

        }
    }
}
