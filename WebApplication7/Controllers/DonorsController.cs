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
    public class DonorsController : Controller
    {
        private Model1 db = new Model1();

        // GET: Donors
        public ActionResult Index()
        {
            return View(db.Donors.ToList());
        }

        // GET: Donors/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Donor donor = db.Donors.Find(id);
            if (donor == null)
            {
                return HttpNotFound();
            }
            return View(donor);
        }

        // GET: Donors/Create
        public ActionResult Create(int? id)
        {
            if (id == null)
            {
                Response.Redirect("../Websites/Index");
            }
            Website website = db.Website.Find(id);
            if (website == null)
            {
                return HttpNotFound();
            }
            ViewBag.Website = website;
            return View();
        }

        // POST: Donors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,FirstName,LastName,Address1,Address2,City,State,Phone,ZipCode,Amount")] Donor donor)
        {
            if (ModelState.IsValid)
            {
                if (String.IsNullOrEmpty(Request["websiteId"]))
                    return View("FailureView");
                int websiteId = Convert.ToInt32(Request["websiteId"]);

                donor.Website_ID = websiteId;
                db.Donors.Add(donor);
                db.SaveChanges();

                decimal donationAmt = donor.Amount;

                Website website = db.Website.Find(websiteId);
                ViewBag.Website = website;
                ViewBag.Donor = donor;               
                //credit card donation
                if (Request["card"] == "1")
                {
                    string CreditCardNumber = Request["txtNumber"];
                    if (String.IsNullOrEmpty(CreditCardNumber))
                        return View("DonationFailureView");
                    string CreditCardType = Request["txtType"].ToLower();
                    if (String.IsNullOrEmpty(CreditCardType))
                        return View("DonationFailureView");
                    string CreditCardExpMonth = Request["txtMonth"];
                    if (String.IsNullOrEmpty(CreditCardExpMonth))
                        return View("DonationFailureView");
                    string CreditCardExpYear = Request["txtYear"];
                    if (String.IsNullOrEmpty(CreditCardExpYear))
                        return View("DonationFailureView");

                    ViewBag.Website = website;
                    ViewBag.Donor = donor;
                    var v = PaymentWithCreditCard(donor, CreditCardNumber, CreditCardType, CreditCardExpMonth, CreditCardExpYear);
                    return v;
                }
                //PayPal donation
                else
                {
                    var v = PaymentWithPayPal(donationAmt);
                    return v;
                }
            }
            return View("DonationFailureView"); 
        }

        // GET: Donors/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Donor donor = db.Donors.Find(id);
            if (donor == null)
            {
                return HttpNotFound();
            }
            return View(donor);
        }

        // POST: Donors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,FirstName,LastName,Address1,Address2,City,State,Phone,ZipCode,Amount")] Donor donor)
        {
            if (ModelState.IsValid)
            {
                db.Entry(donor).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(donor);
        }

        // GET: Donors/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Donor donor = db.Donors.Find(id);
            if (donor == null)
            {
                return HttpNotFound();
            }
            return View(donor);
        }

        // POST: Donors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Donor donor = db.Donors.Find(id);
            db.Donors.Remove(donor);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // POST: Donors/Delete/5
        [HttpPost]
        public ActionResult PaymentSuccessView()
        {
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

        public ActionResult PaymentWithPayPal(decimal donationAmt)
        {
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
                    item.price = donationAmt.ToString();
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
                    details.subtotal = donationAmt.ToString();

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

        public ActionResult PaymentWithCreditCard(Donor d, string CreditCardNumber, string CreditCardType, string CreditCardExpMonth, string CreditCardExpYear)
        {
            //create and item for which you are taking payment
            //if you need to add more items in the list
            //Then you will need to create multiple item objects or use some loop to instantiate object

            Item item = new Item();
            item.name = "Item Name";
            item.currency = "USD";
            item.price = d.Amount.ToString();
            item.quantity = "1";
            item.sku = "sku";

            //Now make a List of Item and add the above item to it
            //you can create as many items as you want and add to this list
            List<Item> itms = new List<Item>();
            itms.Add(item);
            ItemList itemList = new ItemList();
            itemList.items = itms;

            //Address for the payment
            PayPal.Api.Address billingAddress = new PayPal.Api.Address();
            billingAddress.city = d.City;
            billingAddress.country_code = "US";
            billingAddress.line1 = d.Address1;
            billingAddress.postal_code = d.ZipCode;
            billingAddress.state = d.State;

            //Now Create an object of credit card and add above details to it
            //Please replace your credit card details over here which you got from paypal
            CreditCard crdtCard = new CreditCard();
            crdtCard.billing_address = billingAddress;
            //crdtCard.cvv2 = "874";  //card cvv2 number
            //crdtCard.expire_month = 1; 
            crdtCard.expire_month = Convert.ToInt32(CreditCardExpMonth);
            //crdtCard.expire_year = 2021; //card expire year
            crdtCard.expire_year = Convert.ToInt32(CreditCardExpYear);
            //crdtCard.first_name = "test";
            crdtCard.first_name = d.FirstName;
            //crdtCard.last_name = "buyer";
            crdtCard.last_name = d.LastName;
            //crdtCard.number = "4032033901230495"; 
            crdtCard.number = CreditCardNumber;
            //crdtCard.type = "visa"; 
            crdtCard.type = CreditCardType;

            // Specify details of your payment amount.
            Details details = new Details();
            details.shipping = "0";
            details.subtotal = d.Amount.ToString();
            details.tax = "0";

            // Specify your total payment amount and assign the details object
            Amount amnt = new Amount();
            amnt.currency = "USD";
            // Total = shipping tax + subtotal.
            amnt.total = d.Amount.ToString();
            amnt.details = details;

            // Now make a transaction object and assign the Amount object
            Transaction tran = new Transaction();
            tran.amount = amnt;
            tran.description = "Donation";
            tran.item_list = itemList;
            tran.invoice_number = "your invoice number which you are generating";

            // Now, we have to make a list of transaction and add the transactions object
            // to this list. You can create one or more object as per your requirements

            List<Transaction> transactions = new List<Transaction>();
            transactions.Add(tran);

            // Now we need to specify the FundingInstrument of the Payer
            // for credit card payments, set the CreditCard which we made above

            FundingInstrument fundInstrument = new FundingInstrument();
            fundInstrument.credit_card = crdtCard;

            // The Payment creation API requires a list of FundingIntrument

            List<FundingInstrument> fundingInstrumentList = new List<FundingInstrument>();
            fundingInstrumentList.Add(fundInstrument);

            // Now create Payer object and assign the fundinginstrument list to the object
            Payer payr = new Payer();
            payr.funding_instruments = fundingInstrumentList;
            payr.payment_method = "credit_card";

            // finally create the payment object and assign the payer object & transaction list to it
            Payment pymnt = new Payment();
            pymnt.intent = "sale";
            pymnt.payer = payr;
            pymnt.transactions = transactions;

            try
            {
                //getting context from the paypal
                //basically we are sending the clientID and clientSecret key in this function
                //to the get the context from the paypal API to make the payment
                //for which we have created the object above.

                //Basically, apiContext object has a accesstoken which is sent by the paypal
                //to authenticate the payment to facilitator account.
                //An access token could be an alphanumeric string

                // Get a reference to the config
                var config = ConfigManager.Instance.GetProperties();

                // Use OAuthTokenCredential to request an access token from PayPal
                var accessToken = new OAuthTokenCredential(config).GetAccessToken();
                var apiContext = new APIContext(accessToken);

                //Create is a Payment class function which actually sends the payment details
                //to the paypal API for the payment. The function is passed with the ApiContext
                //which we received above.

                Payment createdPayment = pymnt.Create(apiContext);

                //if the createdPayment.state is "approved" it means the payment was successful else not

                if (createdPayment.state.ToLower() != "approved")
                {
                    return View("DonationFailureView");
                }
            }
            catch (PayPal.PayPalException e)
            {
                string error = e.ToString();
                return View("DonationFailureView");
            }
            return View("DonationSuccessView");
        }
    }
}
