using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;

namespace KGIbox
{
    public class PrivateXMLReader
    {
        private static string xmlFile = UtilityClass.Misc.getWorkingDirectory() + "/private.xml";

        public static PrivateXMLReader Instance = new PrivateXMLReader();

        private AttributeTable attributeTable;

        private PrivateXMLReader()
        {
   
        }

        public void init()
        {
            if (!isXmlConfigFileExist())
            {
                MessageBox.Show(xmlFile + " not found.");
                return;
            }

            attributeTable = new AttributeTable();

            XmlTextReader reader = new XmlTextReader(xmlFile);

            while (reader.Read())
            {
                if (String.Compare(reader.Name, "parameter") == 0)
                {
                    String attributeName = reader.GetAttribute("name");
                    String attributeValue = reader.GetAttribute("value");
                    attributeTable.put(attributeName, attributeValue);
                }
            }
        }

        public string getAttribute(string name)
        {
            if(attributeTable.containKey(name)){
                return attributeTable.get(name);
            }else{
                return "NA";
            }
        }



        private bool isXmlConfigFileExist()
        {
            return File.Exists(xmlFile);
        }

        //XmlTextReader reader = new XmlTextReader("books.xml");
    }
}
