using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AccountClass
{
    public class AttributeTable
    {

        private Dictionary<String, String> table;


        public AttributeTable()
        {
            table = new Dictionary<string, string>();
        }

        public void put(String name, String value)
        {
            table.Add(name, value);
        }

        public String get(String name)
        {
            if (table.ContainsKey(name))
            {
                return table[name];
            }
            else
            {
                return "";
            }
        }




    }
}
