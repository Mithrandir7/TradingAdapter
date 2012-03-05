using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Imsl.Stat;

namespace PathsClass
{
    public class Path
    {
        // what is the purpose of path? 
        // path is the basic element of trade
        // path can not over night (this is constrain for preventing over night risk)
        // trading strategy is the tailer of path

        public string dayString;

        public List<int> yyyymmdd = new List<int>();
        public List<int> hhmmss = new List<int>();
        public List<double> high = new List<double>();
        public List<double> low = new List<double>();
        public List<int> volume = new List<int>();
        public List<int> oi = new List<int>();

        public Path(string aStrID)
        {
            dayString = aStrID;
        }

        private int indexOfTime(int ahhmmss)
        {
            int lc = hhmmss.Count;
            for (int i = 0; i < lc; i++)
            {
                if (hhmmss[i] >= ahhmmss)
                {
                    return (i);
                }
            }

            return (-1);

        }

        public Path getSubPath(int startIndex, int length)
        {
            Path lpath = new Path(dayString);
            lpath.yyyymmdd = yyyymmdd.GetRange(startIndex, length);
            lpath.hhmmss = hhmmss.GetRange(startIndex, length);
            lpath.high = high.GetRange(startIndex, length);
            lpath.low = low.GetRange(startIndex, length);
            lpath.volume = volume.GetRange(startIndex, length);
            lpath.oi = oi.GetRange(startIndex, length);
            return (lpath);
        }

        public double profitPoint()
        {
            return (high[0] - low.Last());
        }

        public void profittake(double ratio)
        {
            double lentry = high[0];
            double ltake = lentry * (1 + ratio);
            int lc = high.Count;
            int takeindex = lc - 1;

            for (int i = 0; i < lc; i++)
            {
                if (high[i] >= ltake)
                {
                    takeindex = i;
                    break;
                }
            }

            if (takeindex < lc - 1)
            {
                for (int i = takeindex; i < lc; i++)
                {
                    low[i] = low[takeindex];
                    high[i] = low[takeindex];
                }
            }
        }

        public void hardstop(double ratio)
        {
            double lentry = high[0];
            double lstop = lentry * (1 - ratio);
            int lc = high.Count;
            int stopindex = lc - 1;

            for (int i = 0; i < lc; i++)
            {
                if (low[i] <= lstop)
                {
                    stopindex = i;
                    break;
                }
            }

            if (stopindex < lc - 1)
            {
                for (int i = stopindex; i < lc; i++)
                {
                    low[i] = low[stopindex];
                    high[i] = low[stopindex];
                }
            }
        }

        public Path randomSample(int aTimeAfter, int nSecs)
        {
            int startIdx = indexOfTime(aTimeAfter);
            int endIndex = hhmmss.Count - nSecs;

            if (startIdx > endIndex)
            {
                Console.Write("Error: randomSample ...");
                return (null);
            }

            Imsl.Stat.Random random = new Imsl.Stat.Random(DateTime.Now.Millisecond);
            int ranIdx = random.Next(startIdx, endIndex);

            //Console.WriteLine("random Index is "+ranIdx);

            return (getSubPath(ranIdx, nSecs));
        }

        public void print()
        {
            int lc = high.Count;

            for (int i = 0; i < lc; i++)
            {
                Console.WriteLine("{0},{1},{2:F2},{3:F2},{4:F2},{5:F2}", yyyymmdd[i], hhmmss[i], high[i], low[i], volume[i], oi[i]);
            }

        }

        public void print(int i)
        {
            int lc = high.Count;
            Console.WriteLine("{0},{1},{2:F2},{3:F2},{4:F2},{5:F2}", yyyymmdd[i], hhmmss[i], high[i], low[i], volume[i], oi[i]);
        }

        public void print(string header, int i)
        {
            int lc = high.Count;
            Console.WriteLine("{0} : {1},{2},{3:F2},{4:F2},{5:F2},{6:F2}", header, yyyymmdd[i], hhmmss[i], high[i], low[i], volume[i], oi[i]);
        }

        public int getTickCount()
        {
            return (yyyymmdd.Count);
        }

        public void pushData(int lday, int ltime, double lhi, double llo, int lvolume, int loit)
        {
            yyyymmdd.Add(lday);
            hhmmss.Add(ltime);
            high.Add(lhi);
            low.Add(llo);
            volume.Add(lvolume);
            oi.Add(loit);
            onData();
        }

        private void onData()
        {

        }
    }
}
