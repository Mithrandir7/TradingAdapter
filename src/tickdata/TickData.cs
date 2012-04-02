using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tickdata
{
    public class TickData
    {

        public static TickData Instance = new TickData();

        private int gmtTimeZone;

        private TickData()
        {

        }

        public void init()
        {
            PrivateXMLReader.Instance.init();
            gmtTimeZone = 8;

        }

        public void setTimeZone(int aGMTShift)
        {
            gmtTimeZone = aGMTShift;
        }


        public List<TickQuoteTw> getQuotes(string iceid, string yyyymmdd)
        {

            List<TickQuoteTw> quotes = new List<TickQuoteTw>();

            string cgiURL = PrivateXMLReader.Instance.getAttribute("dataCGI");            

            string url = cgiURL + "?s=" + yyyymmdd + "00&e=" + yyyymmdd + "23&p=" + iceid.ToUpper();
            
            string lrtn = receivedStringFromURL(url);

            string[] rtnArray = lrtn.Split('\n');

            string lyyyymmdd = "";
            string lhhmmss = "000000";
            string lhh = "";
            string lmm = "";
            string lss = "";
            double lpz = -1;
            double hi = -9999;
            double lo = 9999;
            double lvol = 0;

            foreach (string s in rtnArray)
            {
                if (s.IndexOf("DATE") < 0)
                {

                    if (s.IndexOf('#') < 0)
                    {
                        string[] ldataArr = s.Split(',');

                        if (String.Compare(lhhmmss, ldataArr[1].Trim()) == 0)
                        {
                            lpz = Convert.ToDouble(ldataArr[2].Trim());
                            hi = Math.Max(hi, lpz);
                            lo = Math.Min(lo, lpz);
                            lvol = lvol + Convert.ToDouble(ldataArr[3].Trim());
                        }
                        else
                        {
                            if (lpz > 0)
                            {
                                int dd = int.Parse(lyyyymmdd.Substring(6, 2));
                                int mm = int.Parse(lyyyymmdd.Substring(4, 2));
                                int yyyy = int.Parse(lyyyymmdd.Substring(0, 4));
                                TickQuoteTw quote = new TickQuoteTw();
                                quote.datetime = new DateTime(yyyy, mm, dd, int.Parse(lhh), int.Parse(lmm), int.Parse(lss));
                                quote.high = hi;
                                quote.low = lo;
                                quote.volume = lvol;
                                quote.oi = 0;
                                quotes.Add(quote);
                                //Console.WriteLine(lyyyymmdd + "," + lhh + lmm + lss + "," + lo + "," + hi + "," + lvol);                                
                            }

                            lyyyymmdd = ldataArr[0].Trim();
                            lhhmmss = ldataArr[1].Trim();
                            lhh = lhhmmss.Substring(0, 2);
                            lmm = lhhmmss.Substring(2, 2);
                            lss = lhhmmss.Substring(4, 2);
                            lhh = Convert.ToString(Convert.ToInt32(lhh) + gmtTimeZone);
                            lpz = Convert.ToDouble(ldataArr[2].Trim());
                            hi = lpz;
                            lo = lpz;
                            lvol = Convert.ToDouble(ldataArr[3].Trim());
                        }
                    }
                }               
            }
            return quotes;
        }

        private string receivedStringFromURL(string url)
        {
            Uri URI = new Uri(url);
            System.Net.WebRequest req = System.Net.WebRequest.Create(URI);
            System.Net.WebResponse resp = req.GetResponse();
            System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
            return sr.ReadToEnd().Trim();
        }

    }
}
