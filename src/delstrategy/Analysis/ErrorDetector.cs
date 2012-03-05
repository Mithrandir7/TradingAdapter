using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Imsl.Stat;

namespace delstrategy
{
    public class ErrorDetector
    {
        private Paths paths;
        public ErrorDetector(Paths aPaths)
        {
            paths = aPaths;
        }

        public void showError()
        {
            List<Path> plist = paths.getPathList();
            foreach (Path lp in plist)
            {
                //showPathOutlier(lp);
                //showPathTimeSeqError(lp);
                showZeroVolume(lp);
            }
        }


        private void showPathTimeSeqError(Path aPath)
        {
            int[] lt = aPath.hhmmss.ToArray();
            for (int i = 1; i < aPath.high.Count; i++)
            {
                if (lt[i-1]>lt[i])
                {
                    aPath.print("Time Sequence Error", i);
                }
            }

        }

        private void showPathOutlier(Path aPath)
        {
            double[] hi = aPath.high.ToArray();
            double[] lo = aPath.low.ToArray();

            double himean = Summary.GetMean(hi);
            double histd = Summary.GetStandardDeviation(hi);

            for (int i = 0; i < aPath.high.Count; i++)
            {
                if (Math.Abs(aPath.high[i] - himean) > 10 * histd)
                {
                    aPath.print("High Outlier",i);
                }
            }

        }

        private void showZeroVolume(Path aPath)
        {
            int[] vo = aPath.volume.ToArray();

            for (int i = 0; i < aPath.high.Count; i++)
            {
                if (vo[i]==0)
                {
                    aPath.print("Zero Volume : ", i);
                }
            }
        }





    }
}
