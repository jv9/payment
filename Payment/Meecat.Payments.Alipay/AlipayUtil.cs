using log4net;
using Meecat.Payments.Core;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meecat.Payments.Alipay
{
    public class AlipayUtil
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(AlipayUtil));

        private static readonly bool Sandbox = true;

        public static void Request(string id, decimal amount)
        {
            var name = "" + id + "-Meerkat Store";
            //product description (optional)
            var body = "Order from Meerkat Store";
            var currency = "USD";
            var service = "create_forex_trade";//"create_direct_pay_by_user";
            string signType = "MD5";
            string key = Config.key;
            string partner = Config.partner;
            var orderId = id;
            string totalAmount = Math.Round(amount, 2).ToString("0.00", CultureInfo.InvariantCulture);
            string notify_url = Config.notify_url;
            string return_url = Config.return_url;

            var param = new Dictionary<string, object>
            {
                {"service", service },
                { "partner", partner},
                { "_input_charset" , "utf-8"},
                {"return_url", return_url },
                {"notify_url", notify_url },
                {"currency", currency },
                { "out_trade_no", orderId},
                {"subject", name },
                {"total_fee", totalAmount },
                {"body", body },
            };

            var sign = param.Sign(key);

            var remotePost = new RemotePost
            {
                FormName = "alipaysubmit",
                Url = PayUtil.GetAlipayUrl(Sandbox) + "?_input_charset=utf-8",
                Method = "POST"
            };

            foreach (var p in param)
            {
                remotePost.Add(p.Key, p.Value.ToString());
            }
            remotePost.Add("sign", sign);
            remotePost.Add("sign_type", signType);

            remotePost.Post();
        }

        public static bool Verify(NameValueCollection form)
        {
            var responseSign = form["sign"];
            var mySign = form.Sign(Config.key);
            _log.InfoFormat("alipay,  my sign:{0}, resp sign: {1}", mySign, responseSign);

            var url = string.Format("{0}?service=notify_verify&partner={1}&notify_id={2}",
                PayUtil.GetAlipayUrl(Sandbox),
                Config.partner,
                form["notify_id"]);

            var resp = PayUtil.HttpGet(url);
            _log.InfoFormat("alipay,  my sign:{0}, resp sign: {1}", mySign, responseSign);

            if (mySign.Equals(responseSign, StringComparison.OrdinalIgnoreCase)
                && "true".Equals(resp, StringComparison.OrdinalIgnoreCase))
            {
                //check status
                var tradeStatus = form["trade_status"];
                _log.InfoFormat("alipay pay status: {0}, order no:{1}, total_fee: {2}", form["trade_status"], form["out_trade_no"], form["total_fee"]);

                if (tradeStatus == "WAIT_BUYER_PAY")
                {
                }
                else if (tradeStatus == "TRADE_FINISHED")
                {
                    var orderId = int.Parse(form["out_trade_no"].Trim());
                    var totalAmountText = form["total_fee"].Trim();
                    //TODO: handle order

                    _log.InfoFormat("alipay pay success, order:{0}", orderId);
                }

                return true;
            }
            else
            {
                _log.InfoFormat("alipay pay fail");

                return false;
            }
        }

        //public static string Request(string id, decimal amount)
        //{
        //    string subject = "" + id + "-Meerkat Store"; 
        //    //product description (optional)
        //    string body = "Order from Meerkat Store";

        //    string currency = "USD";

        //    SortedDictionary<string, string> sParaTemp = new SortedDictionary<string, string>();
        //    sParaTemp.Add("service", Config.service);
        //    sParaTemp.Add("partner", Config.partner);
        //    sParaTemp.Add("_input_charset", Config.input_charset.ToLower());
        //    sParaTemp.Add("notify_url", Config.notify_url);
        //    sParaTemp.Add("return_url", Config.return_url);
        //    sParaTemp.Add("currency", currency);
        //    sParaTemp.Add("out_trade_no", id);
        //    sParaTemp.Add("subject", subject);
        //    sParaTemp.Add("total_fee", amount.ToString());
        //    sParaTemp.Add("body", body); 

        //    string sHtmlText = Submit.BuildRequest(sParaTemp, "get", "Confirm");

        //    return sHtmlText;
        //}
    }
}
