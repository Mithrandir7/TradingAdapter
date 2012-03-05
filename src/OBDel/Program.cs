using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SymbolClass;
using PathsClass;

namespace OBDel
{
    class Program
    {
        static void Main(string[] args)
        {


            List<string> slist = SymbolManager.getAbbrnameList();
            foreach (string lid in slist)
            {
            //String symbol = "if";
                Paths paths = new Paths();
                paths.loadData(lid);
                OBDelta odel = new OBDelta(paths, 2);
                double [] delQ = odel.getMaxDel();
                Console.WriteLine("{0},{1:F2},{2:F2}", lid, delQ[0] / SymbolManager.getTickSize(lid), delQ[1] / SymbolManager.getTickSize(lid));
            }                 


        }
    }
}
