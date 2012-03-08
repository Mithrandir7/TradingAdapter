using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using UtilityClass;
using System.Windows.Forms;
using System.IO;

namespace TCMonitor
{
    public class TouchanceXmlReader
    {

        public static TouchanceXmlReader Instance = new TouchanceXmlReader();

        private  string xmlFile = UtilityClass.Misc.getWorkingDirectory() + "/touchance.xml";

        private TouchanceXmlReader()
        {
            init();
        }

        public bool closedTCOnExit = false;
        public bool checkTCOnStartup = true;



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
                if (String.Compare(reader.Name, "closedTouchanceOnExist") == 0)
                {
                    closedTCOnExit = bool.Parse(reader.GetAttribute("value"));
                }

                if (String.Compare(reader.Name, "checkTouchanceOnStartup") == 0)
                {
                    checkTCOnStartup = bool.Parse(reader.GetAttribute("value"));
                }

            }
        

        }



        private bool isXmlConfigFileExist()
        {
            return File.Exists(xmlFile);
        }

    }
}
