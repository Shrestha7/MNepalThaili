using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;

namespace MNepalAPI.Helper
{
    public class SendSMS
    {
        private string SMSServer = ConfigurationManager.AppSettings["SMSServer"];
        public void pushSMS(string mobile, string messagereply)
        {
            var client = new WebClient();
            client.DownloadString(SMSServer + "977" + mobile.Trim() + "&message=" + messagereply + "");
        }
    }
}