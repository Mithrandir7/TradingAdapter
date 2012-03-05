using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace delstrategy
{
    class SymbolManager
    {
        public static List<string> symbolList = new List<string>();

        public static Dictionary<string, double> tickSize = new Dictionary<string, double>();

        public static string path = @"D:\WorkPlace\data_recent_oi\";


        public static string getFilename(string symbol)
        {
            string filename = path + symbol + ".data";
            if (!File.Exists(filename))
            {
                Console.WriteLine("File " + filename + " not exist!");
            }
            return (filename);
        }

        static SymbolManager()
        {
            initTickSize();
        }

        static private void initSymbolList()
        {
            foreach (string symbol in tickSize.Keys)
            {
                symbolList.Add(symbol);
            }
        }

        static private void initTickSize()
        {
            tickSize.Add("if", 0.2);
            tickSize.Add("y", 2);
            tickSize.Add("j", 1);
            tickSize.Add("l", 5);
            tickSize.Add("m", 1);
            tickSize.Add("p", 2);
            tickSize.Add("a", 1);
            tickSize.Add("c", 1);
            tickSize.Add("v", 5);
            tickSize.Add("rb", 1);
            tickSize.Add("cu", 10);
            tickSize.Add("au", 0.01);
            tickSize.Add("ru", 5);
            tickSize.Add("zn", 5);
            tickSize.Add("al", 5);
            tickSize.Add("cf", 5);
            tickSize.Add("er", 1);
            tickSize.Add("ws", 1);
            tickSize.Add("ro", 2);
            tickSize.Add("ta", 2);
            tickSize.Add("sr", 1);

            initSymbolList();
        }

    }
}
