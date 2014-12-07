using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace CLScraper
{
    public class Item
    {
        public Item(int Num, string SearchTerm)
        {
            title = "";
            location = "";
            num = Num;
            searchTerm = SearchTerm;
            ID = Num + "," + SearchTerm;
        }

        //public Item(string ID)
        //{
        //    title = "";
        //    location = "";
        //    int commaLoc = 0;
        //    for(int i = 0; i < ID.Length; i++)
        //    {
        //        if (ID[i] == ',')
        //        {
        //            commaLoc = i;
        //        }
        //    }
        //    if (commaLoc == 0) { }   //error
        //    string num = ID.Substring(0, commaLoc);
        //    string st = ID.Substring(commaLoc, ID.Length - commaLoc);
        //    int Num = 0;
        //    if (Int32.TryParse(num, out Num))
        //    {
        //        ID = Num + "," + st;
        //    }
        //    else
        //    {
        //        //error
        //    }
            


        //    //ID = Num + "," + st;
        //}

        public XElement elem;

        public int num;
        public string searchTerm;
        public string ID;

        public string link;
        public double price;
        public DateTime date;
        public string location;
        public string title;

        public string Value
        {
            get
            {
                if (price != null)
                {
                    return "$" + price + ", " + date + ", " + location + ", " + title;
                }
                else
                {
                    return "NO PRICE" + date + ", " + location + ", " + title;
                }
            }
        }

        public string formatForCSV()
        {
            return ID + "," + link + "," + price + "," + date + "," + location + "," + title;
        }


    }
}
