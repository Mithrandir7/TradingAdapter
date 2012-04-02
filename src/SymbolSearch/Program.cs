using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataManager;
using System.Xml;

namespace SymbolSearch
{
    class Program
    {
        static void Main(string[] args)
        {
            SymbolSearch.Instance.init();
            List<Symbol> symbols = SymbolSearch.Instance.getTXOSymbols();
            SymbolSearch.Instance.writeXML("txo.xml", symbols);
        }

 
            
    }
}
