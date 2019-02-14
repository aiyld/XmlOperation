// Writer: Winter Yang
// Mail: 1161226280@qq.com
// All rights reserved.
using System.Collections.Generic;
using System.Xml;

namespace XmlOperation.LiDongYang
{
    public interface IXmlEditor
    {
        /*
         * <?xml version="1.0" encoding="utf-8" ?>
         *  <AppUsers>
         *      <User>
         *          <UserName>1</UserName>
         *          <Password>1</Password>
         *          <Phone>1</Phone>
         *      </User>
         *  </AppUsers>
         */

        /// <summary>
        /// Create xml file with root node name
        /// </summary>
        bool CreateXml(string xpath, string rootNodeName);
        
        /// <summary>
        /// Query a node
        /// Specification: //RootNodeName//NodeName[SubNodeName='YourValue']
        ///     E.g. //AppUsers//User[UserName='1']
        /// </summary>
        XmlNode QueryNode(string queryString);

        /// <summary>
        /// Query notes and store them in list
        /// 1.QueryNodes("RootNodeName")
        ///     E.g. QueryNodes("AppUsers")
        /// 2.Specification: //RootNodeName//NodeName[SubNodeName='YourValue']
        ///     E.g. //AppUsers//User[UserName='1']
        /// </summary>
        List<XmlNode> QueryNodes(string nodePathOrName);

        /// <summary>
        /// Add a new node
        /// Path Specification: //RootNodeName//NodeName[SubNodeName='YourValue']
        ///     E.g. //AppUsers//User[UserName='1']
        /// </summary>
        bool AddNode(string parentNodePath, string nodeName, string innerText);

        /// <summary>
        /// Add a new node.
        /// Path Specification: //RootNodeName//NodeName[SubNodeName='YourValue']
        ///     E.g. //AppUsers//User[UserName='1']
        /// </summary>
        bool AddNode(string parentNodePath, string innerXml);

        /// <summary>
        /// Delete a node
        /// Path Specification: //RootNodeName//NodeName[SubNodeName='YourValue']
        ///     E.g. DeleteNode("AppUsers", "//AppUsers//User[UserName='1']")
        /// </summary>
        bool DeleteNode(string parentNodePathOrName, string ChildNodePathOrName);

        /// <summary>
        /// Modify a node
        /// Path Specification: //RootNodeName//NodeName[SubNodeName='YourValue']
        ///     E.g. ModifyNode("//AppUsers//User[UserName='1']","<UserName>1</UserName>")
        /// </summary>
        bool ModifyNode(string parentNodePath, string inerText);

        /// <summary>
        /// Modify whole node value.
        /// Path Specification: //RootNodeName//NodeName[SubNodeName='YourValue']
        ///     E.g. ModifyNode("//AppUsers//User[UserName='1']","<UserName>1</UserName>")
        ///</summary>
        bool ModifyWholeNode(string parentNodePath, string outerText);

        /// <summary>
        /// Dispose the resource
        /// </summary>
        void Dispose();
    }
}
