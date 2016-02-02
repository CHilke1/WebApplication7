using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Text;
using System.Xml.Linq;
using WebApplication7.Models;
using System.ServiceModel.Syndication;


namespace WebApplication7.Controllers
{
    public class WebsitesController : Controller
    {
        private Model1 db = new Model1();

        // GET: Websites
        public ActionResult Index(string Search)
        {
            if (!String.IsNullOrEmpty(Search))
            {
                return View(db.Website.Where(x => x.SiteName.Contains(Search) || Search == null).ToList());
            }
            else
            {
                return View(db.Website.ToList());
            }
            
        }

        // GET: Websites/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Website website = db.Website.Find(id);
            if (website == null)
            {
                return HttpNotFound();
            }

            //retrieve rss data
            string desc;
            string img;
            string myRSS = website.RSS;
            XmlDocument doc = new XmlDocument();
            doc.Load(myRSS);
            try
            {
                desc = doc.SelectSingleNode("//channel/description").InnerText;
            }
            catch
            {
                desc = "No Description Provided.";
            }
            try
            {
                img = doc.SelectSingleNode("//channel/image/url").InnerText;
            }
            catch
            {
                img = String.Empty;
            }
            if (img != String.Empty)
                 website.Image = img;

            website.Description = desc;

            ViewBag.Website = website.ID;

            return View(website);
        }

        // GET: Websites/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Websites/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,SiteName,Owner,Type,URL,iTunes,RSS,Rating,OwnerStatement,Description,Topic")] Website website)
        {
            if (ModelState.IsValid)
            {
                db.Website.Add(website);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Website = website.ID;

            return View(website);
        }

        // GET: Websites/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Website website = db.Website.Find(id);
            if (website == null)
            {
                return HttpNotFound();
            }
            return View(website);
        }

        // POST: Websites/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,SiteName,Owner,Type,URL,iTunes,RSS,Rating,OwnerStatement,Description,Topic")] Website website)
        {
            if (ModelState.IsValid)
            {
                db.Entry(website).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(website);
        }

        // GET: Websites/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Website website = db.Website.Find(id);
            if (website == null)
            {
                return HttpNotFound();
            }
            return View(website);
        }

        // POST: Websites/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Website website = db.Website.Find(id);
            db.Website.Remove(website);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Websites/Create
        [HttpGet]
        public ActionResult SiteContent(int id)
        {
            RSSFeed rss = new RSSFeed();
            Website website = db.Website.Find(id);
            if (website == null)
            {
                return HttpNotFound();
            }

            XmlDocument rssXmlDoc = new XmlDocument();

            // Load the RSS file from the RSS URL
            rssXmlDoc.Load(website.RSS);

            // Parse the Items in the RSS file
            XmlNodeList rssNodes = rssXmlDoc.SelectNodes("rss/channel/item");
            
            // Iterate through the items in the RSS file
            foreach (XmlNode rssNode in rssNodes)
            {
                
                XmlNode rssSubNode = rssNode.SelectSingleNode("title");
                string title = rssSubNode != null ? rssSubNode.InnerText : "";


                rssSubNode = rssNode.SelectSingleNode("link");
                string link = rssSubNode != null ? rssSubNode.InnerText : "";


                rssSubNode = rssNode.SelectSingleNode("description");
                string description = rssSubNode != null ? rssSubNode.InnerText : "";

                Entry entry = new Entry
                {
                    title = title,
                    link = new HtmlString(link),
                    description = description,
                };

                rss.feeditems.Add(entry);
            }

            return View(rss);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        
    }
}
