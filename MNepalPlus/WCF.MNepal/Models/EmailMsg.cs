using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WCF.MNepal.Models
{
    public class EmailMsg
    {
        public string EmailType { get; set; }
        public string EmailDetail { get; set; }
        public string EmailSalMessage { get; set; }
        public string EmailMessage { get; set; }
        public string EmailRegMessage { get; set; }
        public string EmailForSender { get; set; }
        public string EmailForReceiver { get; set; }
        public string MID { get; set; }
        public string Mode { get; set; }





        public string EmailOfMerchant { get; set; }
        public string MobileNo { get; set; }
        public string vid { get; set; }
        public string MerchantType { get; set; }
    }
}