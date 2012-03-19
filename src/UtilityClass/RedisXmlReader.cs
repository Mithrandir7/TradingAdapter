using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Windows.Forms;

namespace UtilityClass
{
    public class RedisXmlReader
    {
        private static string xmlFile = UtilityClass.Misc.getWorkingDirectory() + "/redis.xml";

        public static RedisXmlReader Instance = new RedisXmlReader();

        public String host = "localhost";
        public int port = 6793;
        public bool isSaveQuoteOnServer = false;

        private RedisXmlReader(){
            init();
        }

        private void init()
        {
            if (!isXmlConfigFileExist())
            {
                MessageBox.Show(xmlFile + " not found.");
                return;
            }
            
            XmlTextReader reader = new XmlTextReader(xmlFile);

            reader.ReadToFollowing("redis-server");

            host = reader.GetAttribute("host");
            port = int.Parse(reader.GetAttribute("port"));

            reader.ReadToFollowing("saveQuoteOnServer");
            isSaveQuoteOnServer = bool.Parse(reader.GetAttribute("value"));
        }



        private bool isXmlConfigFileExist()
        {
            return File.Exists(xmlFile);
        }

        //XmlTextReader reader = new XmlTextReader("books.xml");


    }
}
