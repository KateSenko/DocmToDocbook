using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Collections;

namespace XmlSDK
{
    class DocBook
    {
       // public XmlDocument document = new XmlDocument();
       // XmlNode chapter;
        string XmlFilePath;
        public String title;                //
        public struct Chapter               //          
        {                                   //  Теги xml файла docbook
            public String title;            //
            public struct Section           //
            {                               //
                public string title;        //
                public string para;         //
            }

        }
        public DocBook() {}

        public void createXML(string XmlFilePath, StringBuilder str)
        {
         //   XmlFilePath = this.XmlFilePath;
            try
            {
                if (!File.Exists(XmlFilePath))
                {
                    XmlTextWriter textWritter = new XmlTextWriter(XmlFilePath, Encoding.UTF8);

                    textWritter.WriteStartDocument();

                    textWritter.WriteStartElement("book");

                    textWritter.WriteEndElement();

                    textWritter.Close();
                   // Console.WriteLine("Xml have been created!");
                }
                else
                {
                    //Console.WriteLine("File haven't been created. Xml already exist!");
                   
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            addData(XmlFilePath,str);
        }

        public void addData(string XmlFilePath,StringBuilder str)
        {
                XmlDocument document = new XmlDocument();

                document.Load(XmlFilePath);
                XmlNode element = document.CreateElement("info");
                document.DocumentElement.AppendChild(element);
              

                XmlNode title = document.CreateElement("title");
                title.InnerText = XmlFilePath;
                element.AppendChild(title);

                XmlNode chapter = document.CreateElement("chapter");
                document.DocumentElement.AppendChild(chapter); // указываем родителя

                XmlNode para = document.CreateElement("para");
                para.InnerText = str.ToString();
                chapter.AppendChild(para);
                
              
                
                
                document.Save(XmlFilePath);

               // Console.WriteLine("Data have been added to xml!");

               // Console.ReadKey();
               // Console.WriteLine(XmlToJSON(document));

            
        }

        //public void addPara(StringBuilder str, string tag)
        //{
        //    switch (tag)
        //    {
        //        case "t":
        //            XmlNode para = document.CreateElement("para");
        //            para.InnerText = str.ToString();
        //            chapter.AppendChild(para);
        //            break;
        //        case "br":
        //            para.InnerText  "\br";
        //            break;
        //}
        //}
       


        public string ReadXmlDocument(string XmlFilePath)
        {
            XmlDocument document = new XmlDocument();
            document.Load(XmlFilePath);
            StringBuilder sbJSON = new StringBuilder();
            sbJSON.Append("{ ");
            XmlToJSONnode(sbJSON, document.DocumentElement, true);
            sbJSON.Append("}");
            return sbJSON.ToString();
        }

        //  XmlToJSONnode:  Output an XmlElement, possibly as part of a higher array
        private static void XmlToJSONnode(StringBuilder sbJSON, XmlElement node, bool showNodeName)
        {
            if (showNodeName)
                sbJSON.Append("\"" + SafeJSON(node.Name) + "\": ");
            sbJSON.Append("\n{");
            // Build a sorted list of key-value pairs
            //  where   key is case-sensitive nodeName
            //          value is an ArrayList of string or XmlElement
            //  so that we know whether the nodeName is an array or not.
            SortedList childNodeNames = new SortedList();


            //  Add in all node attributes
            if (node.Attributes != null)
                foreach (XmlAttribute attr in node.Attributes)
                    StoreChildNode(childNodeNames, attr.Name, attr.InnerText);


            //  Add in all nodes
            foreach (XmlNode cnode in node.ChildNodes)
            {
                if (cnode is XmlText)
                    StoreChildNode(childNodeNames, "value", cnode.InnerText);
                else if (cnode is XmlElement)
                    StoreChildNode(childNodeNames, cnode.Name, cnode);
            }


            // Now output all stored info
            foreach (string childname in childNodeNames.Keys)
            {
                ArrayList alChild = (ArrayList)childNodeNames[childname];
                if (alChild.Count == 1)
                    OutputNode(childname, alChild[0], sbJSON, true);
                else
                {
                    sbJSON.Append(" \"" + SafeJSON(childname) + "\": [ ");
                    foreach (object Child in alChild)
                        OutputNode(childname, Child, sbJSON, false);
                    sbJSON.Remove(sbJSON.Length - 2, 2);
                    sbJSON.Append(" ], ");
                }
            }
            sbJSON.Remove(sbJSON.Length - 2, 2);
            sbJSON.Append(" }");
        }

        //  StoreChildNode: Store data associated with each nodeName
        //                  so that we know whether the nodeName is an array or not.
        private static void StoreChildNode(SortedList childNodeNames, string nodeName, object nodeValue)
        {
            // Pre-process contraction of XmlElement-s
            if (nodeValue is XmlElement)
            {
                // Convert  <aa></aa> into "aa":null
                //          <aa>xx</aa> into "aa":"xx"
                XmlNode cnode = (XmlNode)nodeValue;
                if (cnode.Attributes.Count == 0)
                {
                    XmlNodeList children = cnode.ChildNodes;
                    if (children.Count == 0)
                        nodeValue = null;
                    else if (children.Count == 1 && (children[0] is XmlText))
                        nodeValue = ((XmlText)(children[0])).InnerText;
                }
            }
            // Add nodeValue to ArrayList associated with each nodeName
            // If nodeName doesn't exist then add it
            object oValuesAL = childNodeNames[nodeName];
            ArrayList ValuesAL;
            if (oValuesAL == null)
            {
                ValuesAL = new ArrayList();
                childNodeNames[nodeName] = ValuesAL;
            }
            else
                ValuesAL = (ArrayList)oValuesAL;
            ValuesAL.Add(nodeValue);
        }


        private static void OutputNode(string childname, object alChild, StringBuilder sbJSON, bool showNodeName)
        {
            if (alChild == null)
            {
                if (showNodeName)
                    sbJSON.Append("\"" + SafeJSON(childname) + "\": ");
                sbJSON.Append("null");
            }
            else if (alChild is string)
            {
                if (showNodeName)
                    sbJSON.Append("\"" + SafeJSON(childname) + "\": ");
                string sChild = (string)alChild;
                sChild = sChild.Trim();
                sbJSON.Append("\"" + SafeJSON(sChild) + "\"");
            }
            else
                XmlToJSONnode(sbJSON, (XmlElement)alChild, showNodeName);
            sbJSON.Append(", \n");
        }

        // Make a string safe for JSON
        private static string SafeJSON(string sIn)
        {
            StringBuilder sbOut = new StringBuilder(sIn.Length);
            foreach (char ch in sIn)
            {
                if (Char.IsControl(ch) || ch == '\'')
                {
                    int ich = (int)ch;
                    sbOut.Append(@"\u" + ich.ToString("x4"));
                    continue;
                }
                else if (ch == '\"' || ch == '\\' || ch == '/')
                {
                    sbOut.Append('\\');
                }
                sbOut.Append(ch);
            }
            return sbOut.ToString();
        }
      


       
    }

}
