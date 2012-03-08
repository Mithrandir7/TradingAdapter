using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Xml;
namespace tradebox
{
    public class TradeboxXmlReader
    {
        private static string xmlFile = UtilityClass.Misc.getWorkingDirectory() + "/redis.xml";

        public static TradeboxXmlReader Instance = new TradeboxXmlReader();

        public int autoShutdownTime = 153000;
        public bool isAutoShutdown = false;

        private TradeboxXmlReader()
        {
           
        }

        public void init()
        {
            if (!isXmlConfigFileExist())
            {
                MessageBox.Show(xmlFile + " not found.");
                return;
            }
            
            XmlTextReader reader = new XmlTextReader(xmlFile);

            reader.ReadToFollowing("autoShutdownAfterMarketClosed");

            try
            {
                isAutoShutdown = bool.Parse(reader.GetAttribute("enable"));
                autoShutdownTime = int.Parse(reader.GetAttribute("marketClosedTime"));
            }
            catch (ArgumentNullException e)
            {

            }
            
        }



        private bool isXmlConfigFileExist()
        {
            return File.Exists(xmlFile);
        }
    }
}
