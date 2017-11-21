using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meecat.Payments.Paypal
{
    public class Config
    {
        public static readonly string notify_url = "http://localhost:56039/home/Notify_Paypal"; 
        public static readonly string return_url = "http://localhost:56039/home/return";

        public static readonly bool Sandbox = true;
        public static readonly string Key = "";
        public static readonly string Secret = "";
    }
}
