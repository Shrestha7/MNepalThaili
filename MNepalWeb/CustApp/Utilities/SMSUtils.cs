using System.Net;

namespace CustApp.Utilities
{
    public class SMSUtils
    {
        string SMSKey = System.Web.Configuration.WebConfigurationManager.AppSettings["MNepalSMS"];

        public void SendSMS(string Message, string mobile)
        {
            string messagereply = Message;
            var client = new WebClient();

            if ((mobile.Substring(0, 3) == "980") || (mobile.Substring(0, 3) == "981")) //FOR NCELL
            {
                //FOR NCELL
                var content = client.DownloadString(SMSKey
                + "977" + mobile + "&message=" + messagereply + "");
            }
            else if ((mobile.Substring(0, 3) == "985") || (mobile.Substring(0, 3) == "984")
                || (mobile.Substring(0, 3) == "986"))
            {
                //FOR NTC
                var content = client.DownloadString(SMSKey
                    + "977" + mobile + "&message=" + messagereply + "");
            }
        }



    }
}