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
    public class MerchantSMS
    {
        string AlertType = string.Empty, AlertSalMessage = string.Empty, AlertMessage = string.Empty, AlertRegMessage = string.Empty;
        string ForSender = string.Empty, ForReceiver = string.Empty, AlertDetail = string.Empty, AlertReceiveMessage = string.Empty;
        String[] AlertParameters;
        String[] AlertSalParameters;
        private char[] delimeter = new char[] { ',', ' ', ';' };
        private char[] delimeterAlert = new char[] { ',', ';' };

        SMSEnable sMSEnable = new SMSEnable();
        CustActivityModel custsmsInfo = new CustActivityModel();

        //SMS
        private string SMSNTC = System.Web.Configuration.WebConfigurationManager.AppSettings["MNepalNTCSMSServerUrl"];
        private string SMSNCELL = System.Web.Configuration.WebConfigurationManager.AppSettings["MNepalSMSServerUrl"];

        public string MerchantSMSEnable(string vid, string amount, string custmobile)
        {
            string MSMSEnable = "true";
            if ((!string.IsNullOrEmpty(vid)) && (!string.IsNullOrEmpty(amount)) && (!string.IsNullOrEmpty(custmobile)))
            {
                MNMerchantsController getMerchantDetails = new MNMerchantsController();
                string GetMerchantMobile = getMerchantDetails.PassVidToGetMerchantDetail(vid);
                string GetMerchantName = getMerchantDetails.PassVIdToGetMerchantName(vid);

                /**Merchant Alert Dynamic**/
                AlertType = "MERM";
                DataTable dtAlertMList = SMSUtils.GetSMSAlert(AlertType);
                if (dtAlertMList != null)
                {
                    int i = 0;
                    if (dtAlertMList.Rows.Count != 0)
                    {
                        for (i = 0; i < dtAlertMList.Rows.Count; i++)
                        {
                            AlertSalMessage = Convert.ToString(dtAlertMList.Rows[i]["AlertSalMessage"]);
                            AlertMessage = Convert.ToString(dtAlertMList.Rows[i]["AlertMessage"]);
                            AlertRegMessage = Convert.ToString(dtAlertMList.Rows[i]["AlertRegMessage"]);
                            ForSender = Convert.ToString(dtAlertMList.Rows[i]["ForSender"]);
                            ForReceiver = Convert.ToString(dtAlertMList.Rows[i]["ForReceiver"]);

                            string resultSalSenderparam = HttpUtility.UrlEncode(CustCheckUtils.GetName(GetMerchantMobile), System.Text.Encoding.GetEncoding("ISO-8859-1"));
                            String ParamStrSal = "," + resultSalSenderparam;
                            AlertSalParameters = ParamStrSal.Split(delimeterAlert, StringSplitOptions.None);

                            string msgName = amount.ToString(); //validTransactionData.Amount.ToString();
                            string resultparm = custmobile.Trim().ToString(); // mobile.Trim().ToString();
                            String ParamStr = "," + msgName + "," + resultparm;
                            AlertParameters = ParamStr.Split(delimeterAlert, StringSplitOptions.None);

                            if (AlertSalMessage.Contains("%s"))
                            {
                                AlertSalMessage = sMSEnable.GetFinalMessage(AlertSalMessage, AlertSalParameters);
                                AlertSalMessage = AlertSalMessage.Replace("\\n", "\n");
                            }
                            if (AlertMessage.Contains("%s"))
                            {
                                AlertMessage = sMSEnable.GetFinalMessage(AlertMessage, AlertParameters);
                                AlertMessage = AlertMessage.Replace("\\n", "\n");
                            }

                        }
                    }
                }

                /**Alert Dynamic**/

                string messagereplyReceiver = "";
                if (ForReceiver == "T")
                {
                    messagereplyReceiver = AlertSalMessage;
                    messagereplyReceiver += AlertMessage;
                    messagereplyReceiver += AlertRegMessage;
                }

                var client = new WebClient();

                //FOR ALL MOBILE NO
                if (GetMerchantMobile.Trim().Substring(0, 1) == "9") //FOR ALL
                {
                    var content = client.DownloadString(
                             SMSNTC
                             + "977" + GetMerchantMobile.Trim() + "&message=" + messagereplyReceiver + "");
                }

                //if ((GetMerchantMobile.Trim().Substring(0, 3) == "980") || (GetMerchantMobile.Trim().Substring(0, 3) == "981")) //FOR NCELL
                //{
                //    //FOR NCELL
                //    //var content = client.DownloadString(
                //    //    "http://smsvas.mos.com.np/PostSMS.ashx?QueueId=&TelecomId=2&KeywordId=3&Password=mnepal120&From=37878&To="
                //    //    + "977" + mobileMerchant + "&message=" + messagereply + "");
                //    var content = client.DownloadString(
                //            SMSNCELL
                //            + "977" + GetMerchantMobile.Trim() + "&message=" + messagereplyReceiver + "");
                //}
                //else if ((GetMerchantMobile.Trim().Substring(0, 3) == "985") || (GetMerchantMobile.Trim().Substring(0, 3) == "984")
                //            || (GetMerchantMobile.Trim().Substring(0, 3) == "986"))
                //{
                //    //FOR NTC
                //    //var content = client.DownloadString(
                //    //    "http://smsvas.mos.com.np/PostSMS.ashx?QueueId=&TelecomId=1&KeywordId=3&Password=mnepal120&From=37878&To="
                //    //    + "977" + mobileMerchant + "&message=" + messagereply + "");
                //    var content = client.DownloadString(
                //            SMSNTC
                //            + "977" + GetMerchantMobile.Trim() + "&message=" + messagereplyReceiver + "");
                //}

                MSMSEnable = messagereplyReceiver;

            }
            else
            {
                MSMSEnable = "false";
            }

            return MSMSEnable;
        }


        public string MerchantReverseSMSEnable(string vid, string amount, string custmobile)
        {
            string MSMSEnable = "true";
            if ((!string.IsNullOrEmpty(vid)) && (!string.IsNullOrEmpty(amount)) && (!string.IsNullOrEmpty(custmobile)))
            {
                MNMerchantsController getMerchantDetails = new MNMerchantsController();
                string GetMerchantMobile = getMerchantDetails.PassVidToGetMerchantDetail(vid);
                string GetMerchantName = getMerchantDetails.PassVIdToGetMerchantName(vid);

                /**Merchant Alert Dynamic**/
                AlertType = "MREV";
                DataTable dtAlertMList = SMSUtils.GetSMSAlert(AlertType);
                if (dtAlertMList != null)
                {
                    int i = 0;
                    if (dtAlertMList.Rows.Count != 0)
                    {
                        for (i = 0; i < dtAlertMList.Rows.Count; i++)
                        {
                            AlertSalMessage = Convert.ToString(dtAlertMList.Rows[i]["AlertSalMessage"]);
                            AlertMessage = Convert.ToString(dtAlertMList.Rows[i]["AlertMessage"]);
                            AlertRegMessage = Convert.ToString(dtAlertMList.Rows[i]["AlertRegMessage"]);
                            ForSender = Convert.ToString(dtAlertMList.Rows[i]["ForSender"]);
                            ForReceiver = Convert.ToString(dtAlertMList.Rows[i]["ForReceiver"]);

                            string resultSalSenderparam = HttpUtility.UrlEncode(CustCheckUtils.GetName(GetMerchantMobile), System.Text.Encoding.GetEncoding("ISO-8859-1"));
                            String ParamStrSal = "," + resultSalSenderparam;
                            AlertSalParameters = ParamStrSal.Split(delimeterAlert, StringSplitOptions.None);

                            string msgName = amount.ToString(); //validTransactionData.Amount.ToString();
                            string resultparm = custmobile.Trim().ToString(); // mobile.Trim().ToString();
                            String ParamStr = "," + msgName + "," + resultparm;
                            AlertParameters = ParamStr.Split(delimeterAlert, StringSplitOptions.None);

                            if (AlertSalMessage.Contains("%s"))
                            {
                                AlertSalMessage = sMSEnable.GetFinalMessage(AlertSalMessage, AlertSalParameters);
                                AlertSalMessage = AlertSalMessage.Replace("\\n", "\n");
                            }
                            if (AlertMessage.Contains("%s"))
                            {
                                AlertMessage = sMSEnable.GetFinalMessage(AlertMessage, AlertParameters);
                                AlertMessage = AlertMessage.Replace("\\n", "\n");
                            }

                        }
                    }
                }

                /**Alert Dynamic**/

                string messagereplyReceiver = "";
                if (ForReceiver == "T")
                {
                    messagereplyReceiver = AlertSalMessage;
                    messagereplyReceiver += AlertMessage;
                    messagereplyReceiver += AlertRegMessage;
                }

                var client = new WebClient();

                //FOR ALL MOBILE NO
                if (GetMerchantMobile.Trim().Substring(0, 1) == "9") //FOR ALL
                {
                    var content = client.DownloadString(
                             SMSNTC
                             + "977" + GetMerchantMobile.Trim() + "&message=" + messagereplyReceiver + "");
                }

                //if ((GetMerchantMobile.Trim().Substring(0, 3) == "980") || (GetMerchantMobile.Trim().Substring(0, 3) == "981")) //FOR NCELL
                //{
                //    //FOR NCELL
                //    //var content = client.DownloadString(
                //    //    "http://smsvas.mos.com.np/PostSMS.ashx?QueueId=&TelecomId=2&KeywordId=3&Password=mnepal120&From=37878&To="
                //    //    + "977" + mobileMerchant + "&message=" + messagereply + "");
                //    var content = client.DownloadString(
                //            SMSNCELL
                //            + "977" + GetMerchantMobile.Trim() + "&message=" + messagereplyReceiver + "");
                //}
                //else if ((GetMerchantMobile.Trim().Substring(0, 3) == "985") || (GetMerchantMobile.Trim().Substring(0, 3) == "984")
                //            || (GetMerchantMobile.Trim().Substring(0, 3) == "986"))
                //{
                //    //FOR NTC
                //    //var content = client.DownloadString(
                //    //    "http://smsvas.mos.com.np/PostSMS.ashx?QueueId=&TelecomId=1&KeywordId=3&Password=mnepal120&From=37878&To="
                //    //    + "977" + mobileMerchant + "&message=" + messagereply + "");
                //    var content = client.DownloadString(
                //            SMSNTC
                //            + "977" + GetMerchantMobile.Trim() + "&message=" + messagereplyReceiver + "");
                //}

                MSMSEnable = messagereplyReceiver;

            }
            else
            {
                MSMSEnable = "false";
            }

            return MSMSEnable;
        }
    }
}