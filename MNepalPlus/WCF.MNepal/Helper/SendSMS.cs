using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Configuration;

namespace WCF.MNepal.Helper
{
    public class SendSMS
    {
        //SMS
        private string SMSServer = WebConfigurationManager.AppSettings["SMSServer"];
        public void pushSMS(string mobile, string messagereply)
        {
            var client = new WebClient();
            client.DownloadString(SMSServer + "977" + mobile.Trim() + "&message=" + messagereply + "");
        }
    }
}