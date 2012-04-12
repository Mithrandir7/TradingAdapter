using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using UtilityClass;

namespace SymbolClass
{
    class SymbolXmlReader
    {

        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static string xmlFile = UtilityClass.Misc.getWorkingDirectory() + "/symbols.xml";

        public static SymbolXmlReader Instance = new SymbolXmlReader();


        public List<string> quoteSymbolList;
        public Dictionary<string, string> abbr2Symbol;
        public Dictionary<string, double> tickSize;
        public Dictionary<string, int> ob95ticks;
        public Dictionary<string, int> ob99ticks;
        public Dictionary<string, int> marketOpen;
        public Dictionary<string, int> marketClose;
        public List<String> abbrNameList;

        private SymbolXmlReader()
        {           
           // init();           
        }

        public void init()
        {
            quoteSymbolList = new List<string>();
            abbr2Symbol = new Dictionary<string, string>();
            tickSize = new Dictionary<string, double>();
            ob95ticks = new Dictionary<string, int>();
            ob99ticks = new Dictionary<string, int>();
            marketOpen = new Dictionary<string, int>();
            marketClose = new Dictionary<string, int>();
            abbrNameList = new List<string>();

            if (!isXmlConfigFileExist())
            {
                MessageBox.Show(xmlFile + " not found.");
                return;
            }
            
            XmlTextReader reader = new XmlTextReader(xmlFile);

            while (reader.Read())
            {
                if (String.Compare(reader.Name, "symbol") == 0)
                {
                    String quoteSymbol = reader.GetAttribute("quoteSymbol");
                    quoteSymbolList.Add(quoteSymbol);
                    String abbrName = quoteSymbol2AbbrName(quoteSymbol);
                    abbr2Symbol.Add(abbrName, quoteSymbol);
                    abbrNameList.Add(abbrName);
                    double ltickSize = double.Parse(reader.GetAttribute("tickSize"));
                    tickSize.Add(abbrName, ltickSize);

                    int mo = int.Parse(reader.GetAttribute("marketOpen"));
                    int mc = int.Parse(reader.GetAttribute("marketClosed"));
                    int o95 = int.Parse(reader.GetAttribute("OB95Ticks"));
                    int o99 = int.Parse(reader.GetAttribute("OB99Ticks"));

                    marketOpen.Add(abbrName, mo);
                    marketClose.Add(abbrName, mc);
                    ob95ticks.Add(abbrName, o95);
                    ob99ticks.Add(abbrName, o99);
                    
                }
            }

            pushToRedisDatabase();
        }

        private void pushToRedisDatabase()
        {
            string header = "Symbols:";
            foreach (String symbol in quoteSymbolList)
            {
                String key = header + symbol;
                RedisUtil.Instance.set(key, " ");
            }
        }

        private string quoteSymbol2AbbrName(string aSymbol)
        {
            string[] lstrarr = aSymbol.Split('.');
            if (lstrarr.Length == 4)
            {
                string lstr = lstrarr[2].ToLower().Trim();
                return (lstr);
            }
            else
            {
                return ("NA");
            }
        }

        private bool isXmlConfigFileExist()
        {
            return File.Exists(xmlFile);
        }

        //XmlTextReader reader = new XmlTextReader("books.xml");

    }
}
