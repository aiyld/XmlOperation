// Writer: Winter Yang
// Mail: 1161226280@qq.com
// All rights reserved.
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using System;

namespace XmlOperation
{
    public class XmlEditor : IXmlEditor
    {
        public XmlDocument Document { set; get; }
        public string Path { set; get; }
        public XmlElement Element { set; get; }
        public XmlNode Node { set; get; }
        public bool IsLoadFromStream { get; set; }
        public Stream XmlStream { get; set; }

        public XmlEditor(string path)
        {
            this.Path = path;
            this.LoadXml(path);
        }

        /// <summary>
        /// open xml file, if not exist create a new one
        /// </summary>
        /// <param name="path">xml path</param>
        /// <param name="rootNode">root name</param>
        public XmlEditor(string path, string rootNode)
        {
            this.Path = path;
            this.Document = new XmlDocument();

            if (!this.LoadXml(path))
            {
                this.CreateXml(path, rootNode);
            }
        }

        public XmlEditor(Stream xmlStream)
        {
            this.Document = new XmlDocument();

            this.Document.Load(xmlStream);
            this.XmlStream = xmlStream;
        }

        private bool LoadXml(string path)
        {
            bool IsSuccess = false;
            this.Document = new XmlDocument();

            try
            {
                this.Document.Load(Path);
                IsSuccess = true;
            }
            catch { }

            if (IsSuccess) { return IsSuccess; }

            try
            {
                //open xml with absolute path
                this.Document.LoadXml(path);
                IsSuccess = true;
            }
            catch { }

            return IsSuccess;
        }

        /// <summary>
        /// create new xml
        /// </summary>
        /// <param name="xpath">path of xml</param>
        /// <param name="nodeName">parent node's name</param>
        /// <returns>bool</returns>
        public bool CreateXml(string xpath, string nodeName)
        {

            bool createStatus = true;
            XmlDeclaration declaration;
            declaration = this.Document.CreateXmlDeclaration("1.0", "gb2312", null);
            this.Document.AppendChild(declaration);

            //add a parent node
            if (this.Document.SelectNodes(nodeName).Count == 0)
            {
                Element = this.Document.CreateElement("", nodeName, "");
                this.Document.AppendChild(Element);
                this.Document.Save(xpath);
            }
            else
            {
                createStatus = false;
            }

            this.Document.Load(xpath);
            Path = xpath;

            return createStatus;
        }

        /// <summary>
        /// query an xmlnode
        /// </summary>
        /// <param name="xpath">the path of target xml</param>
        /// <param name="queryString">the statement to find a node</param>
        /// <returns>XmlNode</returns>
        public XmlNode QueryNode(string queryString)
        {
            XmlNode node = this.Document.SelectSingleNode(queryString);

            return node;
        }

        /// <summary>
        /// query all XmlNodes
        /// </summary>
        /// <param name="nodePathOrName">node's name or path of node</param>
        /// <returns>list<XmlNode></returns>
        public List<XmlNode> QueryNodes(string nodePathOrName)
        {

            List<XmlNode> nodes = new List<XmlNode>();
            XmlNodeList nodelist = this.Document.SelectSingleNode(nodePathOrName).ChildNodes;
            foreach (XmlNode node in nodelist)
            {
                nodes.Add(node);
            }

            return nodes;
        }

        /// <summary>
        /// add new node
        /// </summary>
        /// <param name="nodePathOrName">node's name or node's path</param>
        /// <param name="nodeName">new node's name</param>
        /// <param name="innerText">node's innertext</param>
        /// <returns>bool</returns>
        public bool AddNode(string parentNodePath, string nodeName, string innerHtml)
        {
            bool addSuccess = true;
            try
            {
                //get parent node
                this.Node = this.Document.SelectSingleNode(parentNodePath);
                //add new child nodes
                XmlElement newNode = this.Document.CreateElement(nodeName);

                //inner text of each nodes
                newNode.InnerXml = innerHtml;
                //add child node to parent node
                this.Node.AppendChild(newNode);

                this.Save();
            }
            catch
            {
                addSuccess = false;
            }

            return addSuccess;
        }

        /// <summary>
        /// Add new node.
        /// </summary>
        /// <param name="nodePathOrName">Node's name or node's path.</param>
        /// <param name="innerXml">Node's innertext.</param>
        /// <returns>Bool</returns>
        public bool AddNode(string parentNodePath, string innerXml)
        {
            bool addSuccess = true;
            try
            {
                //get parent node
                this.Node = this.Document.SelectSingleNode(parentNodePath);

                //add child node to parent node
                this.Node.InnerXml += innerXml;

                this.Save();
            }
            catch
            {
                addSuccess = false;
            }

            return addSuccess;
        }

        /// <summary>
        /// delete a node of target xml
        /// </summary>
        /// <param name="parentNodePath">parent node</param>
        /// <param name="ChildNodeName">child node</param>
        /// <returns>bool</returns>
        public bool DeleteNode(string parentNodePath, string ChildNodePath)
        {
            bool delSuccess = true;
            //parent node
            try
            {
                this.Node = Document.SelectSingleNode(parentNodePath);
                XmlNode childNode = Document.SelectSingleNode(ChildNodePath);
                this.Node.RemoveChild(childNode);

                this.Save();
            }
            catch
            {
                delSuccess = false;
            }
            return delSuccess;
        }

        /// <summary>
        /// Modify node.
        /// </summary>
        /// <param name="parentNodePath">Node name or node's path.</param>
        /// <param name="nodeValue">Inner text.</param>
        /// <returns>Bool.</returns>
        public bool ModifyNode(string nodePath, string nodeValue)
        {
            bool modifySuccess = true;

            try
            {
                this.Node = this.Document.SelectSingleNode(nodePath);
                this.Node.InnerXml = nodeValue;
                this.Save();
            }
            catch
            {
                modifySuccess = false;
            }

            return modifySuccess;
        }

        public void Save()
        {
            if (this.IsLoadFromStream)
            {
                using (StreamWriter witer = new StreamWriter(this.XmlStream))
                {
                    this.Document.Save(witer);
                }
            }
            else
            {
                this.Document.Save(this.Path);
            }
        }

        /// <summary>
        /// Modify whole node value.
        /// </summary>
        /// <param name="parentNodePath">Node name or node's path</param>
        /// <param name="outerText">Outer text.</param>
        /// <returns>Bool</returns>
        public bool ModifyWholeNode(string parentNodePath, string outerText)
        {
            bool modifySuccess = true;

            try
            {
                this.Node = this.Document.SelectSingleNode(parentNodePath);
                XmlNode parentNode = this.Node.ParentNode;

                parentNode.RemoveChild(this.Node);
                parentNode.InnerXml += outerText;

                this.Save();
            }
            catch
            {
                modifySuccess = false;
            }

            return modifySuccess;
        }

        /// <summary>
        /// Dispose resource.
        /// </summary>
        public void Dispose()
        {
            if (this.XmlStream != null)
            {
                this.XmlStream.Dispose();
            }

            this.XmlStream = null;
            this.Document = null;
        }
    }
}
