using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace ThailiMerchantApp.Utilities
{
    public class SMSUtils
    {
        string SMSNTCKey = System.Web.Configuration.WebConfigurationManager.AppSettings["MNepalSMS"];
        string SMSNCellKey = System.Web.Configuration.WebConfigurationManager.AppSettings["MNepalSMS"];
        public void SendSMS(string Message,string mobile)
        {
            string messagereply = Message;
            var client = new WebClient();

            if ((mobile.Substring(0, 3) == "980") || (mobile.Substring(0, 3) == "981")) //FOR NCELL
            {
                //FOR NCELL
                var content = client.DownloadString(SMSNCellKey
                + "977" + mobile + "&message=" + messagereply + "");
            }
            else if ((mobile.Substring(0, 3) == "985") || (mobile.Substring(0, 3) == "984")
                || (mobile.Substring(0, 3) == "986"))
            {
                //FOR NTC
                var content = client.DownloadString(SMSNTCKey
                    + "977" + mobile + "&message=" + messagereply + "");
            }
        }


     
    }
}