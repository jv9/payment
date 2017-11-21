using Meecat.Payments.Alipay;
using Meecat.Payments.Paypal;
using Meecat.Payments.Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PaymentDemo.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        //---------------Alipay----------------------------
        [HttpGet]
        public ActionResult Alipay()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Alipay(decimal amount)
        {
            AlipayUtil.Request(DateTime.Now.ToString("yyyyMMddHHmmss"), amount);

            return View();
        }

        [AllowAnonymous]
        [ValidateInput(false)]
        public ActionResult Notify()
        {
            if (AlipayUtil.Verify(Request.Form))
            {
                ViewBag.Message = "Success";
            }
            else
            {
                ViewBag.Message = "Fail";
            }
            return View();
        }

        //---------------Paypal----------------------------
        public ActionResult Paypal()
        {
            ViewBag.Message = "";
            return View();
        }

        [HttpPost]
        public ActionResult Paypal(decimal amount)
        {
            var r = PaypalUtil.Request(DateTime.Now.ToString("yyyyMMddHHmmss"), amount);
            TempData["payment-" + r.OrderId] = r.PaymentId;
            if (r.Success)
            {
                return Redirect(r.Redirect);
            }

            ViewBag.Message = r.ErrorMessage;

            return View();
        }

        [AllowAnonymous]
        [ValidateInput(false)]
        public ActionResult Notify_Paypal()
        {
            var q = Request.QueryString;
            var paymentId = (string)TempData["payment-" + q["orderId"]];
            if (PaypalUtil.Verify(q, paymentId))
            {
                ViewBag.Message = "Success";
            }
            else
            {
                ViewBag.Message = "Fail";
            }
            return View("Notify");
        }

        //---------------Stripe----------------------------
        public ActionResult Stripe()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Stripe(decimal amount, string token)
        {
            var r = StripeUtil.Pay(DateTime.Now.ToString("yyyyMMddHHmmss"), amount, token);
            if (r)
            {
                ViewBag.Message = "Success";
            }
            else
            {
                ViewBag.Message = "Fail";
            }
            return View("Result");
        }

        public ActionResult Return()
        {
            return View();
        }
    }
}