using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;
using System.Xml;
using System.Windows.Forms;

namespace try_bi
{
    class LinkApi
    {
        public static String b = "";
        public string aLink;
        public LinkApi()
        {
            get_link();
        }
        public void get_link()
        {

            XmlDocument xmlDoc = new XmlDocument();
            //xmlDoc.Load("C:/Program Files/POS Connector/xmlConn.xml");
            xmlDoc.Load("xmlConn.xml");
            string xpath = "Table/Product";
            var nodes = xmlDoc.SelectNodes(xpath);

            foreach (XmlNode childrenNode in nodes)
            {
                b = childrenNode.SelectSingleNode("link_api").InnerText;
            }
            aLink = b;
        }
    }
}
