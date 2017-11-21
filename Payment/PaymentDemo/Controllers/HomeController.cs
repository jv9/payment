using Meecat.Payments.Alipay;
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

        public ActionResult Return()
        {
            return View();
        }
    }
}