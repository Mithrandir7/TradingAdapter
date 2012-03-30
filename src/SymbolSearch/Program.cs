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

            QuoteAdapter.Instant.init();


           

        }

        static void  saveTWSSymbol(){
            string outString = QuoteAdapter.Instant.symbolLookup("TWS");
            string[] arr = outString.Split(';');



            //using (XmlWriter writer = XmlWriter.Create(args[1]))
            using (XmlWriter writer = XmlWriter.Create("tws.xml"))
            {
                writer.WriteStartDocument();

                writer.WriteStartElement("Symbols");


                foreach (string ls in arr)
                {
                    string[] datum = ls.Split(',');

                    if (datum.Length == 3)
                    {
                        string[] idArr = datum[0].Split('.');


                        if (idArr[2].Length == 4)
                        {
                            writer.WriteStartElement("Symbol");
                            writer.WriteAttributeString("id", datum[0]);
                            writer.WriteAttributeString("name", datum[1]);
                            writer.WriteAttributeString("exchange", datum[2]);
                            writer.WriteEndElement();
                            writer.WriteString("\n");
                        }

                    }

                }

                writer.WriteEndElement();

                writer.WriteEndDocument();

            }

            Console.WriteLine("tws id with 4 characters saved at tws.xml");
        }
    }
}
