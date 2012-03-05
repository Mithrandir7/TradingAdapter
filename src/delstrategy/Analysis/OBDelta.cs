using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace delstrategy
{
    class OBDelta
    {
        private Paths paths;
        private int nSeconds;
        public OBDelta(Paths aPaths, int aSecond)
        {
            paths = aPaths;
            nSeconds = aSecond;
        }

        public double getMaxDel()
        {
            List<Path> lp = paths.getPathList();
            double maxDel = -9999;

            foreach (Path lpath in lp)
            {
                double ldel = getPathMaxDel(lpath);
                maxDel = Math.Max(maxDel, ldel);
            }
            return maxDel;
        }


        private double getPathMaxDel(Path path)
        {
            List<double> hi = path.high;
            List<double> lo = path.low;
            int len = hi.Count;

            double maxDel = -9999;

            for (int i = nSeconds; i < len; i++)
            {
                double lmax = hi.GetRange(i - nSeconds, nSeconds).Max();
                double lmin = lo.GetRange(i - nSeconds, nSeconds).Min();
                
                double ldel = lmax - lmin;

                maxDel = Math.Max(maxDel, ldel);
            }
            return maxDel;
        }
    }
}
