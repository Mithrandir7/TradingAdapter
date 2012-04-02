using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataManager;
using System.Xml;

namespace SymbolSearch
{
    public class SymbolSearch
    {

        public static SymbolSearch Instance = new SymbolSearch();


        private SymbolSearch()
        {
          
        }

        public void init()
        {
            QuoteAdapter.Instant.init();
        }


        public List<Symbol> getTXOSymbols()
        {
            return searchPattern("TXO");
        }


        public List<Symbol> getTWSSymbols()
        {
            List<Symbol> lSymbols = searchPattern("TWS");
            List<Symbol> symbols = new List<Symbol>();

            foreach (Symbol s in lSymbols)
            {
                string[] idArr = s.iceid.Split('.');

                if (idArr[2].Length == 4)
                {
                    symbols.Add(s);
                }
            }
            return symbols;
        }

        public void writeXML(string filename, List<Symbol> symbols)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "\t";

            using (XmlWriter writer = XmlWriter.Create(filename, settings))
            {
                writer.WriteStartDocument();

                writer.WriteStartElement("Symbols");


                foreach (Symbol s in symbols)
                {
                    writer.WriteStartElement("Symbol");
                    writer.WriteAttributeString("id", s.iceid);
                    writer.WriteAttributeString("name", s.name);
                    writer.WriteAttributeString("exchange", s.exchange);
                    writer.WriteEndElement();            
                }

                writer.WriteEndElement();

                writer.WriteEndDocument();

            }
        }


        public List<Symbol> searchPattern(string aPattern)
        {
            List<Symbol> symbols = new List<Symbol>();

            string outString = QuoteAdapter.Instant.symbolLookup(aPattern);

            string[] arr = outString.Split(';');

            foreach (string ls in arr)
            {
                string[] datum = ls.Split(',');

                if (datum.Length == 3)
                {
                    Symbol s = new Symbol();
                    s.exchange = datum[2].Trim(); ;
                    s.iceid = datum[0].Trim();
                    s.name = datum[1].Trim();
                    symbols.Add(s);
                }
            }
            return symbols;
        }
   
    }
}
