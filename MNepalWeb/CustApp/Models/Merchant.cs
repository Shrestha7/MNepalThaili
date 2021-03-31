using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CustApp.Models
{
    public class Merchant
    {
        public string merchantId { get; set; }
        public string itemCode { get; set; }
        public string amount { get; set; }
        public string username { get; set; }
        public string password { get; set; }
    }
}