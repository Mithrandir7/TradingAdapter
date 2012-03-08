﻿using System;
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

        public List<string> getAccountList()
        {
            return accounts;
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
                    accounts.Add(reader.GetAttribute("value"));
                }
            }
        }



        private bool isXmlConfigFileExist()
        {
            return File.Exists(xmlFile);
        }


    }
}
