﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml;
using System.Text.RegularExpressions;

namespace CLScraper
{
    public partial class Form1 : Form
    {
        public List<Item> CLItems;
        public string currentSearchTerm;

        public Form1()
        {
            InitializeComponent();
            listBox1.MouseDoubleClick += new MouseEventHandler(listBox1_MouseDoubleClick);
            this.Load += new EventHandler(Form1_Load);
        }

        void Form1_Load(object sender, EventArgs e)
        {
            
        }

        void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //string link = ((sender as ListBox).SelectedItem as Item).link;
            string target = ((sender as ListBox).SelectedItem as Item).link;
            try
            {
                System.Diagnostics.Process.Start(target);
            }
            catch
                (
                 System.ComponentModel.Win32Exception noBrowser)
            {
                if (noBrowser.ErrorCode == -2147467259)
                    MessageBox.Show(noBrowser.Message);
            }
            catch (System.Exception other)
            {
                MessageBox.Show(other.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string city = "denver";
            currentSearchTerm = "miata";
            string URL = "http://" + city + ".craigslist.org/search/cta?query=" + currentSearchTerm + "&format=rss";

            //XmlDocument feed = new XmlDocument();
            //feed.Load(URL)


            WebClient client = new WebClient();
            client.DownloadStringCompleted +=new DownloadStringCompletedEventHandler(client_DownloadStringCompleted);
            client.DownloadStringAsync(new Uri(URL));
        }

        void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            string result = null;
            if (!e.Cancelled && e.Error == null)
            {
                result = e.Result;
            }
            if (result == null) { MessageBox.Show("Failed to Connect to website."); return; }

            XmlDocument xDoc = new XmlDocument();
            xDoc.LoadXml(result);

            

            XDocument doc = XDocument.Parse(result);
            XNode topNode = doc.FirstNode;
            XElement topElement = doc.Elements().ToList().FirstOrDefault();

            List<XElement> docElements = topElement.Elements().ToList();
            List<XElement> items = new List<XElement>();
            List<DateTime> itemDates = new List<DateTime>();
            List<string> itemTitles = new List<string>();
            
            for (int i = 0; i < docElements.Count; i++)
            {
                if (docElements[i].Name.LocalName == "item")
                {
                    items.Add(docElements[i]);
                }
            }

            CLItems = parseItems(items);
            listBox1.DataSource = CLItems;
            listBox1.DisplayMember = "Value";
            //listBox1.ValueMember = "ID";


            //List<DateTime> itemDates = new List<DateTime>();
            //for (int i = 0; i < items.Count; i++)
            //{

            //}

            XDocument infoDoc = topNode.Document;
            infoDoc.Elements("item");

            //XNode firstItemNode = doc.Nodes().ElementAt(16);
            //XName clItem = "item";
            //XElement firstItem = doc.Element("item");
            //doc.Elements("item");

            
        }


        public List<Item> parseItems(List<XElement> items)
        {
            List<Item> parsedItems = new List<Item>();
            List<string> data = new List<string>();
            for (int i = 0; i < items.Count; i++)
            {
                Item currentItem = new Item(i, currentSearchTerm);
                //List<XNode> nodes = items[i].Nodes().ToList();
                List<XElement> itemElements = items[i].Elements().ToList();

                string rawDate = itemElements[3].Value;
                currentItem.link = itemElements[1].Value;
                //currentItem.searchTerm = currentSearchTerm;


                string title = itemElements[0].Value;
                string location = "";
                string price = "";
                string description = "";

                //if you want regex
                //string locationPattern = "[^\(]+[\)]"; //an extra parentheses is left on the end, [.[^\(]+[^\)]* leaves a parentheses on the beginning
                //string pricePattern = "[^\;]+"; // take second result
                string pricePattern = @"\;[0-9]+";
                string descriptionPattern = @"[^\(]+";

                //parse location with loop instead of regex
                int start = 0;
                int end = 0;
                for (int j = 0; j < title.Length; j++)
                {
                    if (title[j] == '(')
                    {
                        start = j;}
                    if (title[j] == ')')
                    {
                        end = j;}
                }
                if(start > 0 && end > 0)
                {
                    int len = end - start;
                    currentItem.location = title.Substring(start + 1, len-1);
                }
                Match m = Regex.Match(title, pricePattern);
                if (m.Success)
                {
                    price = m.Value;
                    //remove leading semicolon
                    price = price.Substring(1, price.Length - 1);
                    currentItem.price = Double.Parse(price);
                    price = "$" + price;
                }
                else
                {
                    price = "NO PRICE";
                }


                m = Regex.Match(title, descriptionPattern);
                if(m.Success)
                {
                    currentItem.title = m.Value;
                    description = m.Value;
                }

                currentItem.date = DateTime.Parse(rawDate);
                //currentItem.date = DateTime.Parse(rawDate.Substring(0, 10));
                //string rawTime = rawDate.Substring(12, 8);
                //DateTime time = DateTime.fro

                //data.Add(price + ", " + location + ", "  + description);
                parsedItems.Add(currentItem);

            }

            return parsedItems;
            //listBox1.DataSource = data;
        }
    }
}
