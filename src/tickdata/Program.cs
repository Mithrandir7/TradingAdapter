using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.ComponentModel;
using System.Collections;

namespace tickdata
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


            string url = "http://mis.touchance.com/t.fcgi?s=" + yyyymmdd + "00&e=" + yyyymmdd + "23&p=" + symbol.ToUpper();

            string lrtn = getTick(url);

            string[] rtnArray = lrtn.Split('\n');

            string lyyyymmdd="";
            string lhhmmss = "000000";
            string lhh="";
            string lmm="";
            string lss="";            
            double lpz=-1;
            double hi=-9999;
            double lo=9999;
            double lvol=0;

            foreach (string s in rtnArray)
            {
                if(s.IndexOf("DATE")<0){
                    if (s.IndexOf('#') < 0)
                    {
                        string [] ldataArr = s.Split(',');

                        if (String.Compare(lhhmmss, ldataArr[1].Trim()) == 0)
                        {
                            lpz = Convert.ToDouble(ldataArr[2].Trim());
                            hi = Math.Max(hi, lpz);
                            lo = Math.Min(lo, lpz);
                            lvol = lvol + Convert.ToDouble(ldataArr[3].Trim());
                        }
                        else
                        {
                            if(lpz>0){
                                Console.WriteLine(lyyyymmdd + "," + lhh + lmm + lss + "," + lo+","+ hi + "," + lvol);
                            }
                            

                            lyyyymmdd = ldataArr[0].Trim();
                            lhhmmss = ldataArr[1].Trim();
                            lhh = lhhmmss.Substring(0, 2);
                            lmm = lhhmmss.Substring(2, 2);
                            lss = lhhmmss.Substring(4, 2);
                            lhh = Convert.ToString(Convert.ToInt32(lhh) + 8);
                            lpz = Convert.ToDouble(ldataArr[2].Trim());
                            hi = lpz;
                            lo = lpz;
                            lvol = Convert.ToDouble(ldataArr[3].Trim());
                        }
                        

                        
                    }                    
                }
            }           

        }

        static string getTick(string url)
        {
            Uri URI = new Uri(url);
            System.Net.WebRequest req = System.Net.WebRequest.Create(URI);
            System.Net.WebResponse resp = req.GetResponse();
            System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
            return sr.ReadToEnd().Trim();
        }
    }
}
