using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meecat.Payments.Paypal
{
    public class PaymentResult
    {
        public string OrderId { get; set; }
        public string ErrorMessage { get; set; }
        public decimal Amount { get; set; }
        public string ApiKey { get; set; }
        public string Redirect { get; set; }
        public string PaymentId { get; set; }

        public bool Success
        {
            get
            {
                return string.IsNullOrEmpty(ErrorMessage);
            }
        }

    }
}
