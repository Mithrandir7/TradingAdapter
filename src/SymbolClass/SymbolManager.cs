using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UtilityClass;
using AccountClass;

namespace SymbolClass
{
    public class SymbolManager
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public static SymbolManager Instance = new SymbolManager();

        private  string configFile = Misc.getWorkingDirectory() + @"\symbol.cfg";
        private  List<string> subSymbolList = new List<string>();
        private  Dictionary<string, string> abbr2Symbol = new Dictionary<string, string>();
        private  Dictionary<string, string> abbr2Symbol_ContractMonth = new Dictionary<string, string>();

        public  List<string> oiPlotSymbolList = new List<string>();

        private  Dictionary<string, double> tickSize = new Dictionary<string, double>();
        private  Dictionary<string, int> ob95ticks = new Dictionary<string, int>();
        private  Dictionary<string, int> ob99ticks = new Dictionary<string, int>();
        private  Dictionary<string, int> marketOpen = new Dictionary<string, int>();
        private  Dictionary<string, int> marketClose = new Dictionary<string, int>();

        //ICE.C_CFFEX.IF.HOT
        public void init()
        {
            //oiPlotSymbolList.Add("if");
            //oiPlotSymbolList.Add("rb");
            //oiPlotSymbolList.Add("sr");
            //oiPlotSymbolList.Add("cu");

            if (!File.Exists(configFile))
            {
                return;
            }

            FileStream fsread = new FileStream(configFile,
                                           FileMode.Open,
                                           FileAccess.Read);

            string lstr = new StreamReader(fsread).ReadToEnd();

            string[] lines = lstr.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);

            for (int i = 0; i < lines.Length; i++)
            {
                if (!lines[i].Contains('#') & lines[i].Trim().Length > 1)
                {
                    string mapString = lines[i].Trim();
                    string[] parArr = mapString.Split(',');
                    if (parArr.Length == 6)
                    {
                        //logger.Info("SymbolManager :" + mapString);
                        string ctpID = parArr[0].Trim();
                        string abbrname = getAbbrname(ctpID);
                        //logger.Info("SymbolManager : abbrname " + abbrname);
                        int open = Convert.ToInt32(parArr[1]);
                        int close = Convert.ToInt32(parArr[2]);
                        double tSize = Convert.ToDouble(parArr[3]);
                        int ob95 = Convert.ToInt32(parArr[4]);
                        int ob99 = Convert.ToInt32(parArr[5]);

                        subSymbolList.Add(ctpID);
                        abbr2Symbol.Add(abbrname, ctpID);
                        marketOpen.Add(abbrname, open);
                        marketClose.Add(abbrname, close);
                        tickSize.Add(abbrname, tSize);
                        ob95ticks.Add(abbrname, ob95);
                        ob99ticks.Add(abbrname, ob99);
                    }
                    else
                    {

                    }

                }
            }
        }
        private SymbolManager()
        {
            
        }


        public  string getTradeSymbol(string aAbbrname, string aAccount)
        {
            logger.Info("getTradeSymbol : " + aAbbrname + " / " + aAccount + "/" + abbr2Symbol.Count);

            aAbbrname = aAbbrname.Trim().ToLower();

            if (AccountManager.Instance.isRealAccount(aAccount))
            {
                if (abbr2Symbol.ContainsKey(aAbbrname))
                {
                    return abbr2Symbol[aAbbrname];
                }
                else
                {
                    return "NA";
                }
            }
            else
            {
                if (abbr2Symbol.ContainsKey(aAbbrname))
                {
                    string lstr = abbr2Symbol[aAbbrname];
                    return lstr;
                    //return lstr.Replace("CTP.", "ICE.C_");
                }
                else
                {
                    return "NA";
                }
            }
        }

        public  string getAbbrname(string aSymbol)
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

        private  string trimContractMonth(string aID)
        {
            string[] lstrarr = aID.Split('.');
            if (lstrarr.Length == 4)
            {
                string lstr = lstrarr[0].Trim() + "." + lstrarr[1].Trim() + "." + lstrarr[2].Trim();
                return (lstr);
            }
            else
            {             
                return ("NA");
            }

        }

        public  List<string> getSubSymbolList()
        {
            return subSymbolList;
        }

        public  List<string> getAbbrnameList()
        {
            List<string> aList = new List<string>();

            foreach (string aStr in abbr2Symbol.Keys)
            {
                aList.Add(aStr);
            }
            return aList;
        }

        public  double getTickSize(String abbrname)
        {
            if (tickSize.ContainsKey(abbrname))
            {
                return (tickSize[abbrname]);
            }
            else
            {           
                return (0);
            }
        }

        public  double getOb95Tick(String abbrname)
        {
            if (ob95ticks.ContainsKey(abbrname))
            {
                return (ob95ticks[abbrname]);
            }
            else
            {           
                return (0);
            }
        }

        public  double getOb99Tick(String abbrname)
        {
            if (ob99ticks.ContainsKey(abbrname))
            {
                return (ob99ticks[abbrname]);
            }
            else
            {            
                return (0);
            }
        }

        public  int getOpenHHMMSS(String abbrname)
        {
            if (marketOpen.ContainsKey(abbrname))
            {
                return (marketOpen[abbrname]);
            }
            else
            {
                return (235959);
            }
        }


        public  int getCloseHHMMSS(String abbrname)
        {
            if (marketClose.ContainsKey(abbrname))
            {
                return (marketClose[abbrname]);
            }
            else
            {
                return (150000);
            }
        }


        public  int getDayTradeExitHHMMSS(String abbrname)
        {
            if (marketClose.ContainsKey(abbrname))
            {
                return (DateTimeFunc.nSecondsBefore(marketClose[abbrname], 60));
            }
            else
            {
                return (145000);
            }
        }

        public  string getHistoricalFilename(string symbol)
        {
            string historicalDataPath = @"D:\WorkPlace\data\csv\";
            string filename = historicalDataPath + symbol + ".data";
            if (!File.Exists(filename))
            {
                Console.WriteLine("File " + filename + " not exist!");
            }
            return (filename);
        }

        public  bool isValidSymbol(string symbol)
        {
            List<string> symbolList = getAbbrnameList();
            return symbolList.Contains(symbol);
        }
    }
}
