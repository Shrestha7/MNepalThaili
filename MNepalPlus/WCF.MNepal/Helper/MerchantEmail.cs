using MNepalProject.Controllers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using WCF.MNepal.Models;
using WCF.MNepal.Utilities;

namespace WCF.MNepal.Helper
{
    public class MerchantEmail
    {
        string EmailType = string.Empty, EmailSalMessage = string.Empty, EmailMessage = string.Empty, EmailRegMessage = string.Empty;
        string EmailForSender = string.Empty, EmailForReceiver = string.Empty, EmailDetail = string.Empty, EmailReceiveMessage = string.Empty;
        String[] EmailParameters;
        String[] EmailSalParameters;
        private char[] delimeter = new char[] { ',', ' ', ';' };
        private char[] delimeterEmail = new char[] { ',', ';' };

        EmailEnable emailEnable = new EmailEnable();
        CustActivityModel custemailInfo = new CustActivityModel();


        public string MerchantEmailEnable(string vid, string EmailType, string subjectEmail, string mobileEmail, string Value1Email, string Value2Email, string Value3Email, string Value4Email, string Value5Email)
        {
            string MEmailEnable = "true";
            //if ((!string.IsNullOrEmpty(vid)) && (!string.IsNullOrEmpty(EmailType)) && (!string.IsNullOrEmpty(subjectEmail)) && (!string.IsNullOrEmpty(Value1Email)) && (!string.IsNullOrEmpty(Value2Email)) && (!string.IsNullOrEmpty(Value3Email)) && (!string.IsNullOrEmpty(Value4Email)) && (!string.IsNullOrEmpty(Value5Email)))
            if ((!string.IsNullOrEmpty(vid)) && (!string.IsNullOrEmpty(EmailType)) && (!string.IsNullOrEmpty(subjectEmail)))
            {
                MNMerchantsController getMerchantDetails = new MNMerchantsController();
                string GetMerchantMobile = getMerchantDetails.PassVidToGetMerchantDetail(vid);
                string GetMerchantName = getMerchantDetails.PassVIdToGetMerchantName(vid);

                //for get email and firstname
                string EmailOfMerchant = "";


                DataTable dtemailOfMerchant = EmailUtils.GetMerchantDetail(GetMerchantMobile);
                if (dtemailOfMerchant != null)
                {

                    EmailOfMerchant = Convert.ToString(dtemailOfMerchant.Rows[0]["EmailAddress"]);

                }

                /**Merchant Alert Dynamic**/
                // EmailType = "NW";
                DataTable dtAlertMList = EmailUtils.GetEmailAlert(EmailType);
                if (dtAlertMList != null)
                {
                    int i = 0;
                    if (dtAlertMList.Rows.Count != 0)
                    {
                        for (i = 0; i < dtAlertMList.Rows.Count; i++)
                        {
                            EmailSalMessage = Convert.ToString(dtAlertMList.Rows[i]["EmailSalMessage"]);
                            EmailMessage = Convert.ToString(dtAlertMList.Rows[i]["EmailMessage"]);
                            EmailRegMessage = Convert.ToString(dtAlertMList.Rows[i]["EmailRegMessage"]);
                            EmailForSender = Convert.ToString(dtAlertMList.Rows[i]["EmailForSender"]);
                            EmailForReceiver = Convert.ToString(dtAlertMList.Rows[i]["EmailForReceiver"]);

                            string resultSalSenderparam = CustCheckUtils.GetName(GetMerchantMobile);
                            String ParamStrSal = "," + resultSalSenderparam;
                            EmailSalParameters = ParamStrSal.Split(delimeterEmail, StringSplitOptions.None);


                            String ParamStr = "," + mobileEmail + "," + Value1Email + "," + Value2Email + "," + Value3Email + "," + Value4Email + "," + Value5Email;

                            EmailParameters = ParamStr.Split(delimeterEmail, StringSplitOptions.None);

                            if (EmailSalMessage.Contains("%s"))
                            {
                                EmailSalMessage = emailEnable.GetFinalMessage(EmailSalMessage, EmailSalParameters);
                                EmailSalMessage = EmailSalMessage.Replace("\\n", "\n");
                            }
                            if (EmailMessage.Contains("%s"))
                            {
                                EmailMessage = emailEnable.GetFinalMessage(EmailMessage, EmailParameters);
                                EmailMessage = EmailMessage.Replace("\\n", "\n");
                            }

                        }
                    }
                }

                /**Alert Dynamic**/

                string messagereplyReceiver = "";
                if (EmailForReceiver == "T")
                {
                    messagereplyReceiver = EmailSalMessage;
                    messagereplyReceiver += EmailMessage;
                    messagereplyReceiver += EmailRegMessage;
                }

                //for sending email
                var client = new WebClient();

                //for email                
                string MailSubject = messagereplyReceiver;

                EmailUtils SendMail = new EmailUtils();
                try
                {

                    SendMail.SendMail(EmailOfMerchant, subjectEmail, MailSubject);

                }
                catch
                {

                }


                MEmailEnable = messagereplyReceiver;

            }
            else
            {
                MEmailEnable = "false";
            }

            return MEmailEnable;
        }
    }
}