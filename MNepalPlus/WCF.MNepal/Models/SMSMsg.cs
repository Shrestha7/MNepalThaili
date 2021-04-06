using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WCF.MNepal.Models
{
    public class SMSMsg
    {
        public string AlertType { get; set; }
        public string AlertDetail { get; set; }
        public string AlertSalMessage { get; set; }
        public string AlertMessage { get; set; }
        public string AlertRegMessage { get; set; }
        public string ForSender { get; set; }
        public string ForReceiver { get; set; }
        public string MID { get; set; }
        public string Mode { get; set; }
    }
}