using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.ComponentModel;
using System.Collections;
using UtilityClass;

namespace Tickdata
{
    class Program
    {
        static private Dictionary<string, string> abbr = new Dictionary<string, string>();

        static private void initAbbrname()
        {
            abbr.Add("fitx", "ICE.TWF.FITX.HOT");
            abbr.Add("fite", "ICE.TWF.FITE.HOT");
            abbr.Add("fitf", "ICE.TWF.FITF.HOT");
        }

        static void Main(string[] args)
        {
            if (args.Length != 2)
            {                
                Console.WriteLine("Usage : tickdata <symbol> <YYYYMMDD>");
                Console.WriteLine("Usage : tickdata opt.<contract>.<p_or_c>.<strik> <YYYYMMDD>");
                return;
            }

            initAbbrname();
           
            string symbol = args[0].Trim();
            string yyyymmdd = args[1].Trim();

            if (abbr.ContainsKey(symbol))
            {
                symbol = abbr[symbol];                
            }

            if (symbol.Contains("OPT"))
            {
                symbol = symbol.Replace("OPT", "ICE.TWF.TXO");
            }
            else if (symbol.Contains("opt"))
            {
                symbol = symbol.Replace("opt", "ICE.TWF.TXO");
            }

            TickData.Instance.init();

            List<TickQuoteTw> quotes = TickData.Instance.getQuotes(symbol, yyyymmdd);

            foreach (TickQuoteTw q in quotes)
            {
                Console.WriteLine(DateTimeFunc.DateTimeToString(q.datetime) + "," + q.high + "," + q.low + "," + q.volume);
            }
        }

    }
}
