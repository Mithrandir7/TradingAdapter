using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Imsl.Stat;
using SymbolClass;

namespace PathsClass
{
    public class Paths
    {
        // collection of Path

        private Dictionary<string, Path> paths = new Dictionary<string, Path>();

        public Paths()
        {

        }

        public List<Path> getPathList()
        {
            List<Path> lpList = new List<Path>();
            foreach (Path lp in paths.Values)
            {
                lpList.Add(lp);
            }
            return lpList;
        }

        public void loadData(string symbol)
        {
            string filename = SymbolManager.getHistoricalFilename(symbol);

            //Console.WriteLine("Load data from " + filename);

            string line = "";
            StreamReader file = null;
            try
            {
                file = new StreamReader(filename);
                while ((line = file.ReadLine()) != null)
                {
                    if (line.Length <= 0)
                    {
                        continue;
                    }
                    if (line.IndexOf("DATE") < 0)
                    {
                        string[] larr = line.Split(',');
                        if (larr.Length == 6)
                        {

                            try
                            {
                                int lyyyymmdd = Convert.ToInt32(larr[0].Trim());
                                int lhhmmss = Convert.ToInt32(larr[1].Trim());
                                double lhi = Convert.ToDouble(larr[2].Trim());
                                double llo = Convert.ToDouble(larr[3].Trim());
                                int lvolume = Convert.ToInt32(larr[4].Trim());
                                int loit = Convert.ToInt32(larr[5].Trim());
                                getPaths(Convert.ToString(lyyyymmdd)).pushData(lyyyymmdd, lhhmmss, lhi, llo, lvolume, loit);
                            }
                            catch
                            {
                                Console.WriteLine("Can not Parse : " + line);
                            }

                        }
                        else
                        {
                            Console.WriteLine("Column missing : " + line);
                        }
                    }
                }
            }
            finally
            {
                if (file != null)
                    file.Close();
            }

            //showTickCount();
        }

        private Path getPaths(string yyyymmdd)
        {
            if (paths.ContainsKey(yyyymmdd))
            {
                return paths[yyyymmdd];
            }
            else
            {
                Path aPath = new Path(yyyymmdd);
                paths.Add(yyyymmdd, aPath);
                return (aPath);
            }
        }


        private void showTickCount()
        {
            foreach (KeyValuePair<string, Path> pair in paths)
            {
                Console.WriteLine("{0} : {1} ticks",
                pair.Key,
                pair.Value.getTickCount());
            }
        }
    }
}
