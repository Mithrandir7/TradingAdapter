using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PathsClass;
using UtilityClass;
using Imsl.Stat;

namespace OBDel
{
    class OBDelta
    {
        private Paths paths;
        private int nSeconds;

        private List<double> delList = new List<double>();

        public OBDelta(Paths aPaths, int aSecond)
        {
            paths = aPaths;
            nSeconds = aSecond;
        }

        public double[] getMaxDel()
        {
            List<Path> lp = paths.getPathList();

            foreach (Path lpath in lp)
            {
                pushDel(lpath);                
            }

            double[] qProc = { 0.95, 0.99 };
            EmpiricalQuantiles eq = new EmpiricalQuantiles(delList.ToArray(), qProc);

            double[] Q = eq.GetQ();
                            
            return Q;
        }


        private void pushDel(Path path)
        {
            List<double> hi = path.high;
            List<double> lo = path.low;
            List<int> hhmmss = path.hhmmss;
            List<int> yyyymmdd = path.yyyymmdd;

            int len = hi.Count;


            for (int i = nSeconds; i < len; i++)
            {
                double lmax = hi.GetRange(i - nSeconds, nSeconds).Max();
                double lmin = lo.GetRange(i - nSeconds, nSeconds).Min();
                int hhmmss_start = hhmmss[i - nSeconds];
                int hhmmss_stop = hhmmss[i];
                int secDel = DateTimeFunc.hhmmss2Secs(hhmmss_stop) - DateTimeFunc.hhmmss2Secs(hhmmss_start);
                double ldel = lmax - lmin;
                if (secDel <= nSeconds)
                {
                    delList.Add(ldel);
                }

            }
        }
    }
}
