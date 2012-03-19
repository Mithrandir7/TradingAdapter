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

        private SymbolManager()
        {
            
        }

        public void init()
        {
            SymbolXmlReader.Instance.init();
        }

        public  string getTradeSymbol(string aAbbrname, string aAccount)
        {
           // logger.Info("getTradeSymbol : " + aAbbrname + " / " + aAccount + "/" + abbr2Symbol.Count);

            aAbbrname = aAbbrname.Trim().ToLower();

            if (AccountManager.Instance.isRealAccount(aAccount))
            {
                if (SymbolXmlReader.Instance.abbr2Symbol.ContainsKey(aAbbrname))
                {
                    return SymbolXmlReader.Instance.abbr2Symbol[aAbbrname];
                }
                else
                {
                    return "NA";
                }
            }
            else
            {
                if (SymbolXmlReader.Instance.abbr2Symbol.ContainsKey(aAbbrname))
                {
                    string lstr = SymbolXmlReader.Instance.abbr2Symbol[aAbbrname];
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
            return SymbolXmlReader.Instance.quoteSymbolList;
        }

        public  List<string> getAbbrnameList()
        {
            List<string> aList = new List<string>();

            foreach (string aStr in SymbolXmlReader.Instance.abbr2Symbol.Keys)
            {
                aList.Add(aStr);
            }
            return aList;
        }

        public  double getTickSize(String abbrname)
        {
            if (SymbolXmlReader.Instance.tickSize.ContainsKey(abbrname))
            {
                return (SymbolXmlReader.Instance.tickSize[abbrname]);
            }
            else
            {           
                return (0);
            }
        }

        public  double getOb95Tick(String abbrname)
        {
            if (SymbolXmlReader.Instance.ob95ticks.ContainsKey(abbrname))
            {
                return (SymbolXmlReader.Instance.ob95ticks[abbrname]);
            }
            else
            {           
                return (0);
            }
        }

        public  double getOb99Tick(String abbrname)
        {
            if (SymbolXmlReader.Instance.ob99ticks.ContainsKey(abbrname))
            {
                return (SymbolXmlReader.Instance.ob99ticks[abbrname]);
            }
            else
            {            
                return (0);
            }
        }

        public  int getOpenHHMMSS(String abbrname)
        {
            if (SymbolXmlReader.Instance.marketOpen.ContainsKey(abbrname))
            {
                return (SymbolXmlReader.Instance.marketOpen[abbrname]);
            }
            else
            {
                return (235959);
            }
        }


        public  int getCloseHHMMSS(String abbrname)
        {
            if (SymbolXmlReader.Instance.marketClose.ContainsKey(abbrname))
            {
                return (SymbolXmlReader.Instance.marketClose[abbrname]);
            }
            else
            {
                return (150000);
            }
        }


        public  int getDayTradeExitHHMMSS(String abbrname)
        {
            if (SymbolXmlReader.Instance.marketClose.ContainsKey(abbrname))
            {
                return (DateTimeFunc.nSecondsBefore(SymbolXmlReader.Instance.marketClose[abbrname], 60));
            }
            else
            {
                return (145000);
            }
        }


        public  bool isValidSymbol(string symbol)
        {
            List<string> symbolList = getAbbrnameList();
            return symbolList.Contains(symbol);
        }
    }
}
