using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace XmlOperation
{
    /// <summary>
    /// Helper class to operate T type
    /// </summary>
    public class XmlOperator
    {
        //xml operator
        public IXmlEditor Editor { get; set; }

        public XmlOperator(string xmlPath)
        {
            this.Editor = new XmlEditor(xmlPath);
        }

        public XmlOperator(Stream stream)
        {
            this.Editor = new XmlEditor(stream);
        }

        public XmlOperator(string xmlPath, string rootNodeName)
        {
            this.Editor = new XmlEditor(xmlPath, rootNodeName);
        }

        /// <summary>
        /// Add object of UpdateInfo
        /// </summary>
        public bool Add<T>(string parerntNoteNameOrPath, T updateInfo)
        {
            Type type = updateInfo.GetType();
            PropertyInfo[] fields = type.GetProperties();

            StringBuilder sb = new StringBuilder();
            foreach (PropertyInfo field in fields)
            {
                sb.Append(string.Format("<{0}>{1}</{0}>", field.Name, field.GetValue(updateInfo, null)));
            }

            return this.Editor.AddNode(parerntNoteNameOrPath, type.Name, sb.ToString());
        }

        /// <summary>
        /// Query nodes
        /// </summary>
        /// <typeparam name="T">T type</typeparam>
        /// <param name="parerntNoteNameOrPath">Parent node name or path</param>
        /// <returns>List of T value</returns>
        public List<T> Query<T>(string parerntNoteNameOrPath)
        {
            List<T> list = new List<T>();

            Type type = typeof(T);
            PropertyInfo[] fields = type.GetProperties();
            object[] constructParams = new object[fields.Length];

            foreach (XmlNode node in this.Editor.QueryNodes(parerntNoteNameOrPath))
            {
                for (int i = 0; i < fields.Length; i++)
                {
                    constructParams[i] = Convert.ChangeType(node.SelectSingleNode(fields[i].Name).InnerText, fields[i].PropertyType, CultureInfo.InvariantCulture);
                }

                // Create T type object and convert it to T type.
                // Add the new object to the List
                list.Add((T)Convert.ChangeType(Activator.CreateInstance(type, constructParams), type, CultureInfo.InvariantCulture));
            }

            return list;

        }

        /// <summary>
        /// Query node with specific property value
        /// </summary>
        /// <typeparam name="T">T type</typeparam>
        /// <param name="parerntNoteNameOrPath">Parent node name or path</param>
        /// <param name="propertyName">Property name</param>
        /// <param name="propertyValue">Property value</param>
        /// <returns>T value</returns>
        public T Query<T>(string parerntNoteNameOrPath, string propertyName, string propertyValue)
        {
            Type type = typeof(T);
            PropertyInfo[] fields = type.GetProperties();
            object[] constructParams = new object[fields.Length];

            XmlNode node = this.Editor.QueryNode(string.Format("//{0}//{1}[{2}='{3}']", parerntNoteNameOrPath, type.Name, propertyName, propertyValue));

            if (node == null)
            {
                return default(T);
            }

            for (int i = 0; i < fields.Length; i++)
            {
                constructParams[i] = Convert.ChangeType(node.SelectSingleNode(fields[i].Name).InnerText, fields[i].PropertyType, CultureInfo.InvariantCulture);
            }

            // Create T type object and convert it to T type.
            // Add the new object to the List
            return (T)Convert.ChangeType(Activator.CreateInstance(type, constructParams), type, CultureInfo.InvariantCulture);

        }

        /// <summary>
        /// modify testcase
        /// </summary>
        /// <param name="tc">testcase</param>
        /// <returns>bool</returns>
        public bool Update<T>(string parerntNoteNameOrPath, string propertyName, string propertyValue, T tValue)
        {
            Type type = tValue.GetType();
            PropertyInfo[] fields = type.GetProperties();

            StringBuilder sb = new StringBuilder();
            foreach (PropertyInfo field in fields)
            {
                sb.Append(string.Format("<{0}>{1}</{0}>", field.Name, field.GetValue(field.Name, null)));
            }
            return (this.Editor.ModifyNode(string.Format("//{0}//{1}[{2}='{3}']", parerntNoteNameOrPath, type.Name, propertyName, propertyValue), sb.ToString()) == true);
        }

        /// <summary>
        /// Delete with specific property value
        /// </summary>
        /// <typeparam name="T">T type</typeparam>
        /// <param name="parerntNoteNameOrPath">Parent node name or path</param>
        /// <param name="propertyName">Property name</param>
        /// <param name="propertyValue">Property value</param>
        /// <returns>T value</returns>
        public bool Delete<T>(string parerntNoteNameOrPath, string propertyName, string propertyValue)
        {
            Type type = typeof(T);

            return (this.Editor.DeleteNode(parerntNoteNameOrPath, string.Format("//{0}//{1}[{2}='{3}']", parerntNoteNameOrPath, type.Name, propertyName, propertyValue)) == true);
        }

        /// <summary>
        /// Dispose resource.
        /// </summary>
        public void Dispose()
        {
            this.Editor.Dispose();
            this.Editor = null;
        }
    }
}
