using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PayPal.Api;
using PayPal.Api.OpenIdConnect;
using PayPal.Util;

//Authorization Endpoint
//https://www.sandbox.paypal.com/webapps/auth/protocol/openidconnect/v1/authorize
namespace WebApplication7.Controllers
{
    public class PayPalController : Controller
    {
        //
        // GET: /Paypal/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PaymentWithCreditCard()
        {
            //create and item for which you are taking payment
            //if you need to add more items in the list
            //Then you will need to create multiple item objects or use some loop to instantiate object
            Item item = new Item();
            item.name = "Item Name";
            item.currency = "USD";
            item.price = "1";
            item.quantity = "5";
            item.sku = "sku";

            //Now make a List of Item and add the above item to it
            //you can create as many items as you want and add to this list
            List<Item> itms = new List<Item>();
            itms.Add(item);
            ItemList itemList = new ItemList();
            itemList.items = itms;

            //Address for the payment
            PayPal.Api.Address billingAddress = new PayPal.Api.Address();
            billingAddress.city = "Milwaukee";
            billingAddress.country_code = "US";
            billingAddress.line1 = "1313 Mockingbird Lane";
            billingAddress.postal_code = "53201";
            billingAddress.state = "WI";

            //Now Create an object of credit card and add above details to it
            //Please replace your credit card details over here which you got from paypal
            CreditCard crdtCard = new CreditCard();
            crdtCard.billing_address = billingAddress;
            crdtCard.cvv2 = "874";  //card cvv2 number
            crdtCard.expire_month = 1; //card expire date
            crdtCard.expire_year = 2020; //card expire year
            crdtCard.first_name = "John";
            crdtCard.last_name = "Cena";
            crdtCard.number = "4876542318908756"; //enter your credit card number here
            crdtCard.type = "visa"; //credit card type here paypal allows 4 types

            // Specify details of your payment amount.
            Details details = new Details();
            details.shipping = "1";
            details.subtotal = "5";
            details.tax = "1";

            // Specify your total payment amount and assign the details object
            Amount amnt = new Amount();
            amnt.currency = "USD";
            // Total = shipping tax + subtotal.
            amnt.total = "7";
            amnt.details = details;

            // Now make a transaction object and assign the Amount object
            Transaction tran = new Transaction();
            tran.amount = amnt;
            tran.description = "Description about the payment amount.";
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
                    return View("FailureView");
                }
            }
            catch (PayPal.PayPalException e)
            {
                string error = e.ToString();
                return View("FailureView");
            }
            return View("SuccessView");
        }


        // GET: PayPal
        public ActionResult PaymentWithPayPal()
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
                    item.price = "15";
                    item.quantity = "5";
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

                    string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority + "/PaymentWithPayPal.aspx?";

                    // # Redirect URLS
                    RedirectUrls redirUrls = new RedirectUrls();
                    redirUrls.cancel_url = baseURI + "guid=" + guid;
                    redirUrls.return_url = baseURI + "guid=" + guid;

                    // ###Details
                    // Let's you specify details of a payment amount.
                    Details details = new Details();
                    details.tax = "15";
                    details.shipping = "10";
                    details.subtotal = "75";

                    // ###Amount
                    // Let's you specify a payment amount.
                    Amount amnt = new Amount();
                    amnt.currency = "USD";
                    // Total must be equal to sum of shipping, tax and subtotal.
                    amnt.total = "100.00";
                    amnt.details = details;

                    // ###Transaction
                    // A transaction defines the contract of a
                    // payment - what is the payment for and who
                    // is fulfilling it. 
                    List<Transaction> transactionList = new List<Transaction>();
                    Transaction tran = new Transaction();
                    tran.description = "Donation.";
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
                return View("FailureView");
            }
            return View("SuccessView");
        }
    }
}