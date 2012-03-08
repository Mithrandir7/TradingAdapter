using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace AccountClass
{
    public class AccountXmlReader
    {

        private static string xmlFile = UtilityClass.Misc.getWorkingDirectory() + "/account.xml";

        public static AccountXmlReader Instance = new AccountXmlReader();
        

        private AccountXmlReader()
        {
            init();
        }

        private List<String> accounts = new List<string>();
        private Dictionary<String, AttributeTable> attributeTables = new Dictionary<String, AttributeTable>();


        public List<string> getAccountList()
        {
            return accounts;
        }

        public String getAccountAttribute(String aAccount, String aAttributeName)
        {
            AttributeTable attributeTable = attributeTables[aAccount];
            return attributeTable.get(aAttributeName);
        }

        private void init()
        {
            if (!isXmlConfigFileExist())
            {
                MessageBox.Show(xmlFile + " not found.");
                return;
            }

            XmlTextReader reader = new XmlTextReader(xmlFile);

            while (reader.Read())
            {
                if (String.Compare(reader.Name, "account") == 0)
                {
                    String account = reader.GetAttribute("value");
                    accounts.Add(account);

                    AttributeTable aTable = new AttributeTable();
                    while (reader.MoveToNextAttribute()) {
                        String aName = reader.Name;
                        String aValue = reader.Value;
                        aTable.put(aName, aValue);
                        MessageBox.Show(account + ":" + aName + ":" + aValue);
                    }
                    attributeTables.Add(account, aTable);
                }
            }
        }



        private bool isXmlConfigFileExist()
        {
            return File.Exists(xmlFile);
        }


    }
}
