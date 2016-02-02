using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication7.Models;
using PayPal.Api;
using PayPal.Util;
using PayPal.Api.OpenIdConnect;

namespace WebApplication7.Controllers
{
    public class DonationsController : Controller
    {
        private Model1 db = new Model1();

        // GET: Donations
        public ActionResult Index()
        {
            return View(db.Donations.ToList());
        }

        // GET: Donations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Donation donation = db.Donations.Find(id);
            if (donation == null)
            {
                return HttpNotFound();
            }
            return View(donation);
        }

        // GET: Donations/Create
        public ActionResult Create()
        {
            ViewBag.MyString = "12";
            return View();
        }

        // POST: Donations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Amount,Date")] Donation donation)
        {
            if (ModelState.IsValid)
            {
                donation.Site = ViewBag.Website;
                donation.Donor = ViewBag.Donor;
                donation.Date = DateTime.Now;
                db.Donations.Add(donation);
                db.SaveChanges();
                string donationAmt = Convert.ToString(donation.Amount);
                PaymentWithPayPal(donationAmt);
            }

            return View(donation);
        }

        // GET: Donations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Donation donation = db.Donations.Find(id);
            if (donation == null)
            {
                return HttpNotFound();
            }
            return View(donation);
        }

        // POST: Donations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Amount,Date")] Donation donation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(donation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(donation);
        }

        // GET: Donations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Donation donation = db.Donations.Find(id);
            if (donation == null)
            {
                return HttpNotFound();
            }
            return View(donation);
        }


        // POST: Donations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Donation donation = db.Donations.Find(id);
            db.Donations.Remove(donation);
            db.SaveChanges();
            return RedirectToAction("Index");
        }



        [HttpGet]
        public ActionResult DonationSuccessView()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DonationSuccessView(FormCollection form)
        {
            //guid = 3981 & paymentId = PAY - 5WG15597CW360760VK2WXQ7A & token = EC - 4BD34395RK296394J & PayerID = 7JHWDFE47F2T2
            return View();
        }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult PaymentWithPayPal(string donationAmt)
        {          
            //OAuthTokenCredential tokenCredential = new OAuthTokenCredential("AeJs4Inwk1Pn8ZNhkWiSLwerx4K64E1PD5TtL4FF7XImtEZ29aAWBTxYOVIBWxEXlW6FycnBz3U7J8jQ", "ENerw7v3F1YT1w5YRYHRPCbjfVSpAbvHVTJFfqc0jWPyeq8hcgmvaZn-1T1WzklDVqw7Pd7MGp3KEQXO");
            //string accessToken = tokenCredential.GetAccessToken();

            // Get a reference to the config
            var config = ConfigManager.Instance.GetProperties();

            // Use OAuthTokenCredential to request an access token from PayPal
            var accessToken = new OAuthTokenCredential(config).GetAccessToken();
            var apiContext = new APIContext(accessToken);

            try
            {
                string payerId = Request.Params["PayerID"];
                Payment payment = null;

                if (string.IsNullOrEmpty(payerId))
                {

                    // ###Items
                    // Items within a transaction.
                    Item item = new Item();
                    item.name = "Item Name";
                    item.currency = "USD";
                    item.price = donationAmt;
                    item.quantity = "1";
                    item.sku = "sku";

                    List<Item> itms = new List<Item>();
                    itms.Add(item);
                    ItemList itemList = new ItemList();
                    itemList.items = itms;

                    // ###Payer
                    // A resource representing a Payer that funds a payment
                    // Payment Method
                    // as `paypal`
                    Payer payr = new Payer();
                    payr.payment_method = "paypal";
                    Random rndm = new Random();
                    var guid = Convert.ToString(rndm.Next(100000));

                    string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority + "/Donations/DonationSuccessView?";

                    // # Redirect URLS
                    RedirectUrls redirUrls = new RedirectUrls();
                    redirUrls.cancel_url = baseURI + "guid=" + guid;
                    redirUrls.return_url = baseURI + "guid=" + guid;

                    // ###Details
                    // Let's you specify details of a payment amount.
                    Details details = new Details();
                    details.tax = "0";
                    details.shipping = "0";
                    details.subtotal = donationAmt;

                    // ###Amount
                    // Let's you specify a payment amount.
                    Amount amnt = new Amount();
                    amnt.currency = "USD";
                    // Total must be equal to sum of shipping, tax and subtotal.
                    amnt.total = donationAmt + ".00";
                    amnt.details = details;

                    // ###Transaction
                    // A transaction defines the contract of a
                    // payment - what is the payment for and who
                    // is fulfilling it. 
                    List<Transaction> transactionList = new List<Transaction>();
                    Transaction tran = new Transaction();
                    tran.description = "Donation";
                    tran.amount = amnt;
                    tran.item_list = itemList;
                    // The Payment creation API requires a list of
                    // Transaction; add the created `Transaction`
                    // to a List
                    transactionList.Add(tran);

                    // ###Payment
                    // A Payment Resource; create one using
                    // the above types and intent as `sale` or `authorize`
                    payment = new Payment();
                    payment.intent = "sale";
                    payment.payer = payr;
                    payment.transactions = transactionList;
                    payment.redirect_urls = redirUrls;

                    var createdPayment = payment.Create(apiContext);
                    string paypalRedirectUrl = null;
                    var links = createdPayment.links.GetEnumerator();
                    while (links.MoveNext())
                    {
                        Links lnk = links.Current;

                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            //saving the payapalredirect URL to which user will be redirected for payment
                            paypalRedirectUrl = lnk.href;
                        }
                    }

                    // saving the paymentID in the key guid
                    Session.Add(guid, createdPayment.id);
                    return Redirect(paypalRedirectUrl);
                }
                else
                {
                    var guid = Request.Params["guid"];
                    var paymentId = Session[guid] as string;
                    var paymentExecution = new PaymentExecution() { payer_id = payerId };
                    var pymnt = new Payment() { id = paymentId };
                    var executedPayment = pymnt.Execute(apiContext, paymentExecution);
                }
            }
            catch (Exception e)
            {
                string error = e.ToString();
                return View("DonationFailureView");
            }
            return View("DonationSuccessView");
        }
    }
}
