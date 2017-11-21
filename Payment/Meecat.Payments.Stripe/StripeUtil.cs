using log4net;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meecat.Payments.Stripe
{
    public class StripeUtil
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(StripeUtil));

        public static bool Pay(string id, decimal amount, string token)
        {
            // See your keys here: https://dashboard.stripe.com/account/apikeys
            //StripeConfiguration.SetApiKey();

            // Charge the user's card:
            var charges = new StripeChargeService("sk_test_kgte8BanAYo8UxwTzojJjwDm");
            var charge = charges.Create(new StripeChargeCreateOptions
            {
                Amount = (int)(amount * 100),
                Currency = "aud",
                Description = "Meerkat Charge",
                Metadata = new Dictionary<String, String>() { { "OrderId", id } },
                SourceTokenOrExistingSourceId = token
            });

            var r = charge.StripeResponse.ResponseJson;
            _log.Info($"Stripe: {r}");

            return (charge.Status == "succeeded");
        }
    }
}
