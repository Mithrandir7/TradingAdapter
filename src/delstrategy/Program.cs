using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace delstrategy
{
    class Program
    {
        
        static void Main(string[] args)
        {
            doAnalysis();
            Console.WriteLine("Press any key too continue...");
            Console.ReadLine();
        }

        static void doAnalysis()
        {
            //OBDelAnalysis();
            //dataErrorAnalysis();
            redisImport();
        }

        static void redisImport()
        {
            
            List<string> slist = SymbolManager.symbolList;
            foreach (string symbol in slist)
            {
                Paths paths = new Paths();
                paths.loadData(symbol);
                RedisDBImport dbImport = new RedisDBImport(symbol, paths);
                dbImport.importData();
            }    
        }

        static void dataErrorAnalysis()
        {
            List<string> slist = SymbolManager.symbolList;
            foreach (string symbol in slist)
            {
                Paths paths = new Paths();
                paths.loadData(symbol);
                ErrorDetector od = new ErrorDetector(paths);
                Console.WriteLine("== {0} == ", symbol);
                od.showError();
            }      

            

        }

        static void OBDelAnalysis()
        {
            List<string> slist = SymbolManager.symbolList;
            foreach(string symbol in slist)
            { 
                Paths paths = new Paths();
                paths.loadData(symbol);
                OBDelta odel = new OBDelta(paths, 2);
                double lmaxdel = odel.getMaxDel();
                Console.WriteLine("{0},{1:F2}",symbol,lmaxdel/SymbolManager.tickSize[symbol]);              
            }                    
        }

    }
}
