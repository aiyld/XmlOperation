// Writer: Winter Yang
// Mail: 1161226280@qq.com
// All rights reserved.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;

namespace XmlOperation
{
    public class XmlUtils
    {
        /// <summary>
        /// Store used type that is not base type
        /// </summary>
        private static List<Type> KnowTypes;

        #region Get XMl

        /// <summary>
        /// Generate Xml string node from object
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="inputValue">Input object</param>
        /// <returns>Xml string node</returns>
        public static string GenerateXmlNodeFromObject<T>(T inputValue)
        {
            StoreType(typeof(T));

            StringBuilder xmlString = new StringBuilder();

            Type type = inputValue.GetType();
            PropertyInfo[] fields = type.GetProperties();

            xmlString.Append(string.Format("<{0}>", type.Name));

            foreach (PropertyInfo field in fields)
            {
                // If filed type is not basic type (string ,int ect...) get again
                if (!BaseTypeCollection.GetBaseTypes().Any(t => t == field.PropertyType))
                {
                    xmlString.Append(GenerateXmlNodeFromObject(field.GetValue(inputValue, field.GetIndexParameters()), field.PropertyType));
                }
                else
                {
                    xmlString.Append(
                        string.Format("<{0}>{1}</{0}>",
                            field.Name,
                            SerializerObject(field.GetValue(inputValue, field.GetIndexParameters())))
                        );
                }

            }

            xmlString.Append(string.Format("</{0}>", type.Name));

            return xmlString.ToString();
        }

        /// <summary>
        /// Generate Xml string node from object
        /// </summary>
        /// <param name="inputValue">Input object</param>
        /// <param name="type">Object type</param>
        /// <returns>Xml string node</returns>
        public static string GenerateXmlNodeFromObject(object inputValue ,Type type)
        {
            StoreType(type);

            StringBuilder xmlString = new StringBuilder();

            PropertyInfo[] fields = type.GetProperties();

            xmlString.Append(string.Format("<{0}>", type.Name));

            foreach (PropertyInfo field in fields)
            {
                if (!BaseTypeCollection.GetBaseTypes().Any(t => t == field.PropertyType))
                {
                    xmlString.Append(GenerateXmlNodeFromObject(field.GetValue(inputValue, field.GetIndexParameters()), field.PropertyType));
                }
                else
                {
                    xmlString.Append(
                        string.Format("<{0}>{1}</{0}>",
                            field.Name,
                            SerializerObject(field.GetValue(inputValue, field.GetIndexParameters())))
                        );
                }
            }

            xmlString.Append(string.Format("</{0}>", type.Name));

            return xmlString.ToString();
        }

        /// <summary>
        /// Generate Xml string node from list object
        /// The default root name string is (T class name + "List")
        /// </summary>
        /// <typeparam name="T">Object ject type</typeparam>
        /// <param name="inputValue">Object List</param>
        /// <returns>Xml string</returns>
        public static string GenerateXmlNodeFromListObject<T>(IEnumerable<T> inputValue)
        {
            StoreType(typeof(T));

            StringBuilder xmlString = new StringBuilder();

            xmlString.Append(string.Format("<{0}>", typeof(T).Name + "List"));
            if (inputValue.Count() > 0)
            {
                inputValue.ToList().ForEach(item =>
                {
                    if (item != null)
                    {
                        xmlString.Append(GenerateXmlNodeFromObject(item));
                    }
                });
            }
            xmlString.Append(string.Format("</{0}>", typeof(T).Name + "List"));

            return xmlString.ToString();
        }

        #endregion

        #region Get Json

        /// <summary>
        /// Generate json string from xml node
        /// </summary>
        /// <param name="nodeString">Xml node whole information</param>
        /// <param name="type">Result object type</param>
        /// <returns>Json result string</returns>
        public static string GenerateJsonStringFromXmlMode(string nodeString, Type type)
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(nodeString);

            PropertyInfo[] fields = type.GetProperties();

            XmlNode node = document.SelectSingleNode(type.Name);

            // Get json string from xml 
            StringBuilder sb = new StringBuilder();
            sb.Append("\"" + type.Name + "\":{");
            foreach (PropertyInfo field in fields)
            {
                if (!BaseTypeCollection.GetBaseTypes().Any(s => s.Equals(field.PropertyType)))
                {
                    // Check if this node is the last one in xml
                    sb.Append(
                        GenerateJsonStringFromXmlMode(
                            node.SelectSingleNode(field.Name).OuterXml,
                            field.PropertyType,
                            !node.LastChild.Name.Equals(field.Name))
                            );
                }
                else
                {
                    bool isEnum = true;

                    try
                    {
                        var m = Enum.GetValues(field.PropertyType).GetValue(
                            Enum.GetNames(field.PropertyType).ToList().
                                IndexOf(node.SelectSingleNode(field.Name).InnerText));
                        try
                        {
                            int enumValue = (int)m;
                            sb.Append(string.Format("\"{0}\":{1},", field.Name, enumValue));
                        }
                        catch { sb.Append(string.Format("\"{0}\":{1},", field.Name, m)); }
                    }
                    catch { isEnum = false; }

                    if (!isEnum)
                    {
                        sb.Append(string.Format("\"{0}\":{1},", field.Name, node.SelectSingleNode(field.Name).InnerText));
                    }
                }
            }

            if (fields.Length > 1)
            {
                sb.Remove(sb.Length - 1, 1);
            }

            sb.Append("}");

            return sb.ToString();
        }

        /// <summary>
        /// Generate json string from xml node, and if its parent node has next node after it,
        /// we need to add dot ','
        /// </summary>
        /// <param name="nodeString">Xml node whole information</param>
        /// <param name="type">Result object type</param>
        /// <param name="parentHasNextNode">Indicate if parent node has next node after current one</param>
        /// <returns>Json result string</returns>
        public static string GenerateJsonStringFromXmlMode(string nodeString, Type type, bool parentHasNextNode)
        {
            string json = GenerateJsonStringFromXmlMode(nodeString, type);

            if (parentHasNextNode)
            {
                json += ",";
            }

            return json;
        }

        #endregion

        #region Get Object From Xml

        /// <summary>
        /// Generate object from xml node
        /// </summary>
        /// <typeparam name="T">Result object type</typeparam>
        /// <param name="nodeString">Xml node whole information</param>
        /// <returns>Result object</returns>
        public static T GenerateObjectFromXmlNode<T>(string nodeString)
        {
            StoreType(typeof(T));
            XmlDocument document = new XmlDocument();
            document.LoadXml(nodeString);

            Type type = typeof(T);
            PropertyInfo[] fields = type.GetProperties();

            XmlNode node = document.SelectSingleNode(type.Name);

            // Get json string from xml 
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            foreach (PropertyInfo field in fields)
            {
                if (!BaseTypeCollection.GetBaseTypes().Any(s => s.Equals(field.PropertyType)))
                {
                    // Check if this node is the last one in xml
                    sb.Append(
                        GenerateJsonStringFromXmlMode(
                            node.SelectSingleNode(field.Name).OuterXml,
                            field.PropertyType,
                            !node.LastChild.Name.Equals(field.Name))
                            );
                }
                else
                {
                    // Specific process Enum type
                    bool isEnum = true;

                    try
                    {
                        var m = Enum.GetValues(field.PropertyType).GetValue(
                            Enum.GetNames(field.PropertyType).ToList().
                                IndexOf(node.SelectSingleNode(field.Name).InnerText));
                        try
                        {
                            int enumValue = (int)m;
                            sb.Append(string.Format("\"{0}\":{1},", field.Name, enumValue));
                        }
                        catch { sb.Append(string.Format("\"{0}\":{1},", field.Name, m)); }
                    }
                    catch { isEnum = false; }

                    if (!isEnum )
                    {
                        sb.Append(string.Format("\"{0}\":{1},", field.Name, node.SelectSingleNode(field.Name).InnerText));
                    }
                }
            }

            if (fields.Length > 1)
            {
                sb.Remove(sb.Length - 1, 1);
            }

            sb.Append("}");

            string sss = sb.ToString();

            // Deserialize object from string
            return (T)DeserializerJson(sss, typeof(T));
        }

        /// <summary>
        /// Generate object list from xml node
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="nodeString">Root node whole string</param>
        /// <returns>List T boject</returns>
        public static IEnumerable<T> GenerateListObjectFromXmlNode<T>(string nodeString)
        {
            StoreType(typeof(T));

            List<T> list = null;

            XmlDocument document = new XmlDocument();
            document.LoadXml(nodeString);
            if (document.SelectSingleNode(typeof(T).Name + "List") != null &&
                document.SelectSingleNode(typeof(T).Name + "List").ChildNodes.Count > 0)
            {
                list = new List<T>();

                foreach (XmlNode node in document.SelectSingleNode(typeof(T).Name + "List").ChildNodes)
                {
                    list.Add(GenerateObjectFromXmlNode<T>(node.OuterXml));
                }
            }

            return list;
        }

        #endregion

        /// <summary>
        /// Store type for serialize
        /// </summary>
        /// <param name="type">The type that need to store</param>
        private static void StoreType(Type type)
        {
            if (KnowTypes == null)
            {
                KnowTypes = new List<Type>();
            }

            if (!KnowTypes.Any(t => t == type) && !BaseTypeCollection.GetBaseTypes().Any(s => s == type))
            {
                KnowTypes.Add(type);
            }
        }

        /// <summary>
        /// Serialize object to json string
        /// </summary>
        /// <param name="obj">Input object</param>
        /// <returns>Json string</returns>
        private static string SerializerObject(object obj)
        {
            string json = string.Empty;

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(object), KnowTypes);

            using (MemoryStream str = new MemoryStream())
            {
                serializer.WriteObject(str, obj);
                byte[] buffer = new byte[str.Length];
                str.Seek(0, SeekOrigin.Begin);
                str.Read(buffer, 0, (int)str.Length);
                json = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
            }

            return json;
        }

        /// <summary>
        /// Deserialize object from json string
        /// </summary>
        /// <param name="json">Json string</param>
        /// <param name="type">Result object type</param>
        /// <returns>Result object</returns>
        private static object DeserializerJson(string json, Type type)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(type, KnowTypes);
            using (MemoryStream str = new MemoryStream())
            {
                byte[] buffer = Encoding.UTF8.GetBytes(json);
                str.Write(buffer, 0, buffer.Length);
                str.Seek(0, SeekOrigin.Begin);
                return serializer.ReadObject(str);
            }
        }
    }
}
