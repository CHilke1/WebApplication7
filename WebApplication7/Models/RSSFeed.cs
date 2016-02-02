using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace WebApplication7.Models
{
    public class Entry
    {
        public string title { get; set; }
        public HtmlString link { get; set; }
        public string description { get; set; }
    }
    public class RSSFeed
    {
        public RSSFeed()
        {
            feeditems = new List<Entry>();
        }
        public int WebsiteID { get; set; }
        public List<Entry> feeditems { get; set; }
    }

}