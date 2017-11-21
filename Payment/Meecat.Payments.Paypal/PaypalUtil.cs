using log4net;
using PayPal;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;

namespace Meecat.Payments.Paypal
{
    public class PaypalUtil
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(PaypalUtil));

        public static PaymentResult Request(string id, decimal amount)
        {
            var model = new PaymentResult
            {
                OrderId = id,
                Amount = amount,
                ApiKey = Config.Key,
            };

            try
            {
                var desc = "Payment Desc";
                var currency = "AUD";
                // Make an API call
                var payItem = new Payment
                {
                    intent = "sale",
                    payer = new Payer
                    {
                        payment_method = "paypal"
                    },
                    transactions = new List<Transaction>
                    {
                        new Transaction
                        {
                            description = string.Format("Order#{0}: {1} {2}", id, amount, desc),
                            invoice_number = id.ToString(),
                            amount = new Amount
                            {
                                currency = currency,
                                total = string.Format("{0:F2}", amount),
                                details = new Details
                                {
                                    tax = "0",
                                    shipping = "0",
                                    subtotal = string.Format("{0:F2}", amount)
                                }
                            },
                            item_list = new ItemList
                            {
                                items = new List<Item>
                                {
                                    new Item
                                    {
                                        name = "Item Name",
                                        currency = currency,
                                        price = string.Format("{0:F2}", amount),
                                        quantity = "1",
                                        sku = "sku"
                                    }
                                }
                            }
                        }
                    },
                    redirect_urls = new RedirectUrls
                    {
                        return_url = Config.notify_url + "?orderId=" + id,
                        cancel_url = Config.return_url + "?orderId=" + id,
                    }
                };

                var apiContext = GetAPIContext();
                var payment = Payment.Create(apiContext, payItem);
                model.PaymentId = payment.id;

                _log.InfoFormat("Paypal Express PaymentId: {0}, order#{1}", payment.id, id);

                var links = payment.links.GetEnumerator();
                while (links.MoveNext())
                {
                    var link = links.Current;
                    if (link.rel.ToLower().Trim().Equals("approval_url"))
                    {
                        _log.InfoFormat("Paypal Express Redirect: {0}", link.href);
                        model.Redirect = link.href;
                        return model;
                    }
                }
                model.ErrorMessage = "Paypal Express Error: No approval_url";
            }
            catch (ConnectionException ex)
            {
                _log.Error($"Paypal Express Error, order:{id}, response:{ex.Response} ", ex);
                model.ErrorMessage = ex.Message + " Details: " + ex.Response;
                //return Redirect("/plugins/SmartStore.PaypalExpress/Pay?orderId=" + orderId + "&error=" + ex.Message + " " + ex.Response);
            }
            catch (Exception ex)
            {
                _log.Error("Paypal Express Error!", ex);
                model.ErrorMessage = ex.Message;
            }

            // Session.Add("payment-" + id, payment.id);
            return model;
        }

        public static bool Verify(NameValueCollection q, string paymentId)
        {
            //?orderId=20171121153927&paymentId=PAY-6J083482DE156641PLIJ24CQ&token=EC-22959774YU731742E&PayerID=U4W2BRLM89F52
            var id = q["orderId"];
            var payerId = q["payerId"]; ;
            try
            {
                _log.InfoFormat("Paypal Express PaymentId (Callback): {0}, order#{1}", paymentId, id);

                var paymentExecution = new PaymentExecution() { payer_id = payerId };
                var payment = new Payment() { id = paymentId };

                var apiContext = GetAPIContext();
                var executedPayment = payment.Execute(apiContext, paymentExecution);

                if (executedPayment.state == "approved")
                {
                    //TODO: handle order, update paid status
                    _log.InfoFormat("PaypalExpress pay success, order:{0}", id);

                    return true;
                }
            }
            catch (ConnectionException ex)
            {
                _log.Error($"Paypal Express Error, order:{id}, response:{ex.Response} ", ex);
                //model.ErrorMessage = ex.Message + " Details: " + ex.Response;
                //return Redirect("/plugins/SmartStore.PaypalExpress/Pay?orderId=" + orderId + "&error=" + ex.Message + " " + ex.Response);
            }
            catch (Exception ex)
            {
                _log.Error("Paypal Express Error!", ex);
                //model.ErrorMessage = ex.Message;
            }

            return false;
        }

        private static APIContext GetAPIContext()
        {
            // Authenticate with PayPal
            var config = new Dictionary<string, string>
                {
                    {"mode", Config.Sandbox?"sandbox":"live" },
                    {"clientId", Config.Key},
                    {"clientSecret", Config.Secret},
                };

            var accessToken = new OAuthTokenCredential(config).GetAccessToken();
            var apiContext = new APIContext(accessToken) { Config = config };

            return apiContext;
        }
    }
}
