// Writer: Winter Yang
// Mail: 1161226280@qq.com
// All rights reserved.
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace XmlOperation
{
    public static class XmlExtension
    {
        /// <summary>
        /// Get T type from xml stream.
        /// </summary>
        /// <typeparam name="T">Generic type.</typeparam>
        /// <param name="stream">Xml stream.</param>
        /// <returns>T type object.</returns>
        public static T FromXmlStream<T>(this Stream stream)
        {
            stream.Position = 0;
            XmlReader reader = new XmlTextReader(stream);
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            return (T)serializer.Deserialize(reader);
        }
    }
}
