using MNepalProject.Controllers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using WCF.MNepal.Utilities;

namespace WCF.MNepal.Helper
{
    public class CustomerSMS
    {
        string AlertType = string.Empty, AlertSalMessage = string.Empty, AlertMessage = string.Empty, AlertRegMessage = string.Empty;
        string ForSender = string.Empty, ForReceiver = string.Empty, AlertDetail = string.Empty, AlertReceiveMessage = string.Empty;
        String[] AlertParameters;
        String[] AlertSalParameters;
        String[] AlertReceiverParameters;
        private char[] delimeter = new char[] { ',', ' ', ';' };
        private char[] delimeterAlert = new char[] { ',', ';' };

        SMSEnable sMSEnable = new SMSEnable();

        public string CustSMSEnable(string alertType, string custmobile, string destmobile, string amount, string vid, string couponNumber, string createdDate)
        {
            //SMS
            string SMSNTC = System.Web.Configuration.WebConfigurationManager.AppSettings["MNepalNTCSMSServerUrl"];
            string SMSNCELL = System.Web.Configuration.WebConfigurationManager.AppSettings["MNepalSMSServerUrl"];

            string CSMSEnable = "true";
            if ((!string.IsNullOrEmpty(alertType)) && (!string.IsNullOrEmpty(custmobile)) && (!string.IsNullOrEmpty(amount)))
            {
                string mobile = custmobile;

                MNMerchantsController getMerchantDetails = new MNMerchantsController();
                string getMerchantNameForSMS = getMerchantDetails.PassVIdToGetMerchantName(vid);

                DataTable dtAlertList = SMSUtils.GetSMSAlert(alertType);
                if (dtAlertList != null)
                {
                    int i = 0;
                    if (dtAlertList.Rows.Count != 0)
                    {
                        for (i = 0; i < dtAlertList.Rows.Count; i++)
                        {
                            AlertSalMessage = Convert.ToString(dtAlertList.Rows[i]["AlertSalMessage"]);
                            AlertMessage = Convert.ToString(dtAlertList.Rows[i]["AlertMessage"]);
                            AlertRegMessage = Convert.ToString(dtAlertList.Rows[i]["AlertRegMessage"]);
                            ForSender = Convert.ToString(dtAlertList.Rows[i]["ForSender"]);
                            ForReceiver = Convert.ToString(dtAlertList.Rows[i]["ForReceiver"]);

                            string resultSalSenderparam = HttpUtility.UrlEncode(CustCheckUtils.GetName(mobile), System.Text.Encoding.GetEncoding("ISO-8859-1"));
                            String ParamStrSal = "," + resultSalSenderparam;
                            AlertSalParameters = ParamStrSal.Split(delimeterAlert, StringSplitOptions.None);

                            //START FOR RECEIVER SALUTATION

                            if ((alertType == "TOPUP") || (alertType == "TOPR") ||
                                (alertType == "WWD") || (alertType == "WBD") || (alertType == "BWD") || (alertType == "BBD") ||
                                (alertType == "CIC"))
                            {
                                if (destmobile != "")
                                {
                                    string resultSalReceiverparam = HttpUtility.UrlEncode(CustCheckUtils.GetName(destmobile), System.Text.Encoding.GetEncoding("ISO-8859-1"));
                                    String ParamStrReceiver = "," + resultSalReceiverparam;
                                    AlertReceiverParameters = ParamStrReceiver.Split(delimeterAlert, StringSplitOptions.None);
                                }
                            }

                            //END FOR RECEIVER SALUTATION

                            //START FOR SENDER SALUTATION

                            if (alertType == "SUBISU")
                            {
                                if (AlertSalMessage.Contains("%s"))
                                {
                                    AlertSalMessage = sMSEnable.GetFinalMessage(AlertSalMessage, AlertSalParameters);
                                    AlertSalMessage = AlertSalMessage.Replace("\\n", "\n");
                                }
                            }
                            if (alertType == "SUBISUR")
                            {
                                if (AlertSalMessage.Contains("%s"))
                                {
                                    AlertSalMessage = sMSEnable.GetFinalMessage(AlertSalMessage, AlertSalParameters);
                                    AlertSalMessage = AlertSalMessage.Replace("\\n", "\n");
                                }
                            }
                            if (alertType == "VIANET")
                            {
                                if (AlertSalMessage.Contains("%s"))
                                {
                                    AlertSalMessage = sMSEnable.GetFinalMessage(AlertSalMessage, AlertSalParameters);
                                    AlertSalMessage = AlertSalMessage.Replace("\\n", "\n");
                                }
                            }
                            if (alertType == "VIANETR")
                            {
                                if (AlertSalMessage.Contains("%s"))
                                {
                                    AlertSalMessage = sMSEnable.GetFinalMessage(AlertSalMessage, AlertSalParameters);
                                    AlertSalMessage = AlertSalMessage.Replace("\\n", "\n");
                                }
                            }
                            if (alertType == "WLINK")
                            {
                                if (AlertSalMessage.Contains("%s"))
                                {
                                    AlertSalMessage = sMSEnable.GetFinalMessage(AlertSalMessage, AlertSalParameters);
                                    AlertSalMessage = AlertSalMessage.Replace("\\n", "\n");
                                }
                            }
                            if (alertType == "WLINKR")
                            {
                                if (AlertSalMessage.Contains("%s"))
                                {
                                    AlertSalMessage = sMSEnable.GetFinalMessage(AlertSalMessage, AlertSalParameters);
                                    AlertSalMessage = AlertSalMessage.Replace("\\n", "\n");
                                }
                            }

                            if (alertType == "KUKL")
                            {
                                if (AlertSalMessage.Contains("%s"))
                                {
                                    AlertSalMessage = sMSEnable.GetFinalMessage(AlertSalMessage, AlertSalParameters);
                                    AlertSalMessage = AlertSalMessage.Replace("\\n", "\n");
                                }
                            }

                            if (alertType == "KUKLR")
                            {
                                if (AlertSalMessage.Contains("%s"))
                                {
                                    AlertSalMessage = sMSEnable.GetFinalMessage(AlertSalMessage, AlertSalParameters);
                                    AlertSalMessage = AlertSalMessage.Replace("\\n", "\n");
                                }
                            }

                            //END FOR SENDER SALUTATION

                            //START FOR SENDER SALUTATION

                            if (((alertType == "COU") && (vid != "")) || (alertType == "NEA") || (alertType == "NEAR")
                                || (alertType == "KP") || (alertType == "KPR")
                                || (alertType == "NW") || (alertType == "NWR") || (alertType == "MER") || (alertType == "WSEL"))
                            {
                                if (AlertSalMessage.Contains("%s"))
                                {
                                    AlertSalMessage = sMSEnable.GetFinalMessage(AlertSalMessage, AlertSalParameters);
                                    AlertSalMessage = AlertSalMessage.Replace("\\n", "\n");
                                }
                            }
                            else if ((alertType == "TOPUP") || (alertType == "TOPR") ||
                                (alertType == "WWD") || (alertType == "WBD") || (alertType == "BWD") || (alertType == "BBD") ||
                                (alertType == "CIC"))
                            {
                                if (AlertSalMessage.Contains("%s"))
                                {
                                    AlertReceiveMessage = sMSEnable.GetFinalMessage(AlertSalMessage, AlertReceiverParameters);
                                    AlertReceiveMessage = AlertReceiveMessage.Replace("\\n", "\n");

                                    AlertSalMessage = sMSEnable.GetFinalMessage(AlertSalMessage, AlertSalParameters);
                                    //AlertSalMessage = AlertSalMessage.Replace("\\r\\n", "\r\n");
                                    AlertSalMessage = AlertSalMessage.Replace("\\n", "\n");
                                }
                            }
                            else
                            {
                                if (AlertSalMessage.Contains("%s"))
                                {
                                    //AlertReceiveMessage = sMSEnable.GetFinalMessage(AlertSalMessage, AlertReceiverParameters);
                                    //AlertReceiveMessage = AlertReceiveMessage.Replace("\\n", "\n");

                                    AlertSalMessage = sMSEnable.GetFinalMessage(AlertSalMessage, AlertSalParameters);
                                    //AlertSalMessage = AlertSalMessage.Replace("\\r\\n", "\r\n");
                                    AlertSalMessage = AlertSalMessage.Replace("\\n", "\n");
                                }
                            }

                            //END FOR SENDER SALUTATION


                            //START FOR MSG

                            string msgName; string resultparm; String ParamStr; string GetMerchantName;

                            if ((alertType == "COU") && (vid != ""))
                            {
                                GetMerchantName = getMerchantDetails.PassVIdToGetMerchantName(vid);

                                msgName = amount.ToString();
                                resultparm = GetMerchantName.ToString();
                                ParamStr = "," + msgName + "," + resultparm + "," + couponNumber;
                                AlertParameters = ParamStr.Split(delimeterAlert, StringSplitOptions.None);
                            }
                            if (alertType == "MER")
                            {
                                GetMerchantName = getMerchantDetails.PassVIdToGetMerchantName(vid);

                                msgName = amount.ToString();
                                resultparm = GetMerchantName.ToString();
                                ParamStr = "," + msgName + "," + resultparm;
                                AlertParameters = ParamStr.Split(delimeterAlert, StringSplitOptions.None);
                            }
                            else if ((alertType == "TOPUP") || alertType == "TOPR")
                            {
                                msgName = getMerchantNameForSMS.ToString();
                                resultparm = amount.ToString();
                                ParamStr = "," + msgName + "," + resultparm + "," + destmobile.Trim().ToString();
                                AlertParameters = ParamStr.Split(delimeterAlert, StringSplitOptions.None);
                            }
                            else if ((alertType == "NEA") || (alertType == "NEAR") || (alertType == "KP") || (alertType == "KPR")
                                || (alertType == "NW") || (alertType == "NWR"))
                            {
                                msgName = amount.ToString();
                                resultparm = createdDate;
                                ParamStr = "," + msgName + "," + couponNumber + "," + resultparm;
                                AlertParameters = ParamStr.Split(delimeterAlert, StringSplitOptions.None);
                            }
                            else if ((alertType == "WWS") || (alertType == "WBS") || (alertType == "BWS") || (alertType == "BBS"))
                            {
                                msgName = amount.ToString();
                                resultparm = createdDate;
                                ParamStr = "," + msgName + "," + destmobile + "," + resultparm;
                                AlertParameters = ParamStr.Split(delimeterAlert, StringSplitOptions.None);
                            }
                            else if ((alertType == "WWD") || (alertType == "WBD"))
                            {
                                msgName = amount.ToString();
                                resultparm = createdDate;
                                ParamStr = "," + msgName + "," + mobile + "," + resultparm;
                                AlertParameters = ParamStr.Split(delimeterAlert, StringSplitOptions.None);
                            }
                            else if ((alertType == "BWD") || (alertType == "BBD") || (alertType == "BWDO") || (alertType == "WSEL"))
                            {
                                if (mobile == destmobile)
                                {
                                    msgName = amount.ToString();
                                    resultparm = createdDate;
                                    ParamStr = "," + msgName + "," + resultparm;
                                    AlertParameters = ParamStr.Split(delimeterAlert, StringSplitOptions.None);
                                }
                                else
                                {
                                    msgName = amount.ToString();
                                    resultparm = createdDate;
                                    ParamStr = "," + msgName + "," + mobile + "," + resultparm;
                                    AlertParameters = ParamStr.Split(delimeterAlert, StringSplitOptions.None);
                                }

                            }
                            else if (alertType == "CO")
                            {
                                msgName = couponNumber.ToString();
                                resultparm = amount.ToString();
                                ParamStr = "," + msgName + "," + resultparm;
                                AlertParameters = ParamStr.Split(delimeterAlert, StringSplitOptions.None);
                            }
                            else if ((alertType == "CIC") || (alertType == "CID") || (alertType == "COC"))
                            {
                                resultparm = amount.ToString();
                                ParamStr = "," + resultparm;
                                AlertParameters = ParamStr.Split(delimeterAlert, StringSplitOptions.None);
                            }
                            else if ((alertType == "COD") || (alertType == "COW")) //Cash Out Debited & Cash out Withdraw
                            {
                                resultparm = amount.ToString();
                                ParamStr = "," + resultparm + "," + destmobile;
                                AlertParameters = ParamStr.Split(delimeterAlert, StringSplitOptions.None);
                            }
                            else if (alertType == "COR") //Cash Out Rejected
                            {
                                resultparm = "";
                                ParamStr = "," + resultparm;
                                AlertParameters = ParamStr.Split(delimeterAlert, StringSplitOptions.None);
                            }
                            else if (alertType == "SHARER")
                            {
                                resultparm = amount.ToString();
                                ParamStr = "," + resultparm + "," + couponNumber + "," + createdDate;
                                AlertParameters = ParamStr.Split(delimeterAlert, StringSplitOptions.None);
                            }
                            else if (alertType == "SHARE")
                            {
                                resultparm = amount.ToString();
                                ParamStr = "," + resultparm + "," + couponNumber + "," + createdDate;
                                AlertParameters = ParamStr.Split(delimeterAlert, StringSplitOptions.None);
                            }
                            else if (alertType == "SUBISUR")
                            {
                                resultparm = amount.ToString();
                                ParamStr = "," + resultparm + "," + couponNumber + "," + createdDate;
                                AlertParameters = ParamStr.Split(delimeterAlert, StringSplitOptions.None);
                            }
                            else if (alertType == "SUBISU")
                            {
                                resultparm = amount.ToString();
                                ParamStr = "," + resultparm + "," + couponNumber + "," + createdDate;
                                AlertParameters = ParamStr.Split(delimeterAlert, StringSplitOptions.None);
                            }
                            else if (alertType == "VIANET")
                            {
                                resultparm = amount.ToString();
                                ParamStr = "," + resultparm + "," + couponNumber + "," + createdDate;
                                AlertParameters = ParamStr.Split(delimeterAlert, StringSplitOptions.None);
                            }
                            else if (alertType == "VIANETR")
                            {
                                resultparm = amount.ToString();
                                ParamStr = "," + resultparm + "," + couponNumber + "," + createdDate;
                                AlertParameters = ParamStr.Split(delimeterAlert, StringSplitOptions.None);
                            }
                            else if (alertType == "WLINK")
                            {
                                resultparm = amount.ToString();
                                ParamStr = "," + resultparm + "," + couponNumber + "," + createdDate;
                                AlertParameters = ParamStr.Split(delimeterAlert, StringSplitOptions.None);
                            }
                            else if (alertType == "WLINKR")
                            {
                                resultparm = amount.ToString();
                                ParamStr = "," + resultparm + "," + couponNumber + "," + createdDate;
                                AlertParameters = ParamStr.Split(delimeterAlert, StringSplitOptions.None);
                            }

                            else if (alertType == "KUKL")
                            {
                                resultparm = amount.ToString();
                                ParamStr = "," + resultparm + "," + couponNumber + "," + createdDate;
                                AlertParameters = ParamStr.Split(delimeterAlert, StringSplitOptions.None);
                            }
                            else if (alertType == "KUKLR")
                            {
                                resultparm = amount.ToString();
                                ParamStr = "," + resultparm + "," + couponNumber + "," + createdDate;
                                AlertParameters = ParamStr.Split(delimeterAlert, StringSplitOptions.None);
                            }

                            //END FOR MSG

                            //START FOR SMS MESSAGE

                            if (AlertMessage.Contains("%s"))
                            {
                                AlertMessage = sMSEnable.GetFinalMessage(AlertMessage, AlertParameters);
                                AlertMessage = AlertMessage.Replace("\\n", "\n");
                            }

                            //END FOR SMS MESSAGE

                        }
                    }
                }

                //SENDER
                string messagereply = "";
                if ((alertType == "SUBISU") || (alertType == "SUBISUR"))
                {
                    if (ForSender == "T")
                    {
                        messagereply = AlertSalMessage;
                        messagereply += AlertMessage + createdDate + Environment.NewLine;
                        messagereply += AlertRegMessage;
                    }
                }
                else if ((alertType == "VIANET") || (alertType == "VIANETR"))
                {
                    if (ForSender == "T")
                    {
                        messagereply = AlertSalMessage;
                        messagereply += AlertMessage + createdDate + Environment.NewLine;
                        messagereply += AlertRegMessage;
                    }
                }
                else if ((alertType == "WLINK") || (alertType == "WLINKR"))
                {
                    if (ForSender == "T")
                    {
                        messagereply = AlertSalMessage;
                        messagereply += AlertMessage + createdDate + Environment.NewLine;
                        messagereply += AlertRegMessage;
                    }
                }
                else if ((alertType == "KUKL") || (alertType == "KUKLR"))
                {
                    if (ForSender == "T")
                    {
                        messagereply = AlertSalMessage;
                        messagereply += AlertMessage + createdDate + Environment.NewLine;
                        messagereply += AlertRegMessage;
                    }
                }
                else
                {
                    if (ForSender == "T")
                    {
                        messagereply = AlertSalMessage;
                        messagereply += AlertMessage;
                        messagereply += AlertRegMessage;
                    }
                }

                //RECEIVER
                string messagereplyReceiver = "";
                if (ForReceiver == "T")
                {
                    messagereplyReceiver = AlertReceiveMessage;
                    messagereplyReceiver += AlertMessage;
                    messagereplyReceiver += AlertRegMessage;
                }

                var client = new WebClient();


                //FOR SENDER
                if (mobile.Trim().Substring(0, 1) == "9") //FOR ALL
                {
                    //var content = client.DownloadString(
                    //        SMSNCELL
                    //        + "977" + mobile.Trim() + "&message=" + messagereply + "");

                    SendSMS sendSMS = new SendSMS();
                    sendSMS.pushSMS(mobile, messagereply);

                    CSMSEnable = messagereply;

                }

                //FOR RECEIVER
                if (destmobile != "")
                {
                    if (destmobile.Trim().Substring(0, 1) == "9") //FOR ALL
                    {
                        //var content = client.DownloadString(SMSNCELL
                        //    + "977" + destmobile.Trim() + "&message=" + messagereplyReceiver + "");

                        SendSMS sendSMS = new SendSMS();
                        sendSMS.pushSMS(destmobile, messagereplyReceiver);
                    }

                }

                //CSMSEnable = messagereply;

            }
            else
            {
                CSMSEnable = "false";
            }

            return CSMSEnable;

        }


        public void CustSMSEnableSend(string alertType, string custmobile, string destmobile, string amount, string vid, string couponNumber, string createdDate)
        {

            if ((!string.IsNullOrEmpty(alertType)) && (!string.IsNullOrEmpty(custmobile)) && (!string.IsNullOrEmpty(amount)))
            {
                string mobile = custmobile;

                MNMerchantsController getMerchantDetails = new MNMerchantsController();
                string getMerchantNameForSMS = getMerchantDetails.PassVIdToGetMerchantName(vid);

                DataTable dtAlertList = SMSUtils.GetSMSAlert(alertType);
                if (dtAlertList != null)
                {
                    int i = 0;
                    if (dtAlertList.Rows.Count != 0)
                    {
                        for (i = 0; i < dtAlertList.Rows.Count; i++)
                        {
                            AlertSalMessage = Convert.ToString(dtAlertList.Rows[i]["AlertSalMessage"]);
                            AlertMessage = Convert.ToString(dtAlertList.Rows[i]["AlertMessage"]);
                            AlertRegMessage = Convert.ToString(dtAlertList.Rows[i]["AlertRegMessage"]);
                            ForSender = Convert.ToString(dtAlertList.Rows[i]["ForSender"]);
                            ForReceiver = Convert.ToString(dtAlertList.Rows[i]["ForReceiver"]);

                            string resultSalSenderparam = HttpUtility.UrlEncode(CustCheckUtils.GetName(mobile), System.Text.Encoding.GetEncoding("ISO-8859-1"));
                            String ParamStrSal = "," + resultSalSenderparam;
                            AlertSalParameters = ParamStrSal.Split(delimeterAlert, StringSplitOptions.None);

                            //START FOR RECEIVER SALUTATION

                            if ((alertType == "TOPUP") || (alertType == "TOPR") ||
                                (alertType == "WWD") || (alertType == "WBD") || (alertType == "BWD") || (alertType == "BBD") ||
                                (alertType == "CIC"))
                            {
                                if (destmobile != "")
                                {
                                    string resultSalReceiverparam = HttpUtility.UrlEncode(CustCheckUtils.GetName(destmobile), System.Text.Encoding.GetEncoding("ISO-8859-1"));
                                    String ParamStrReceiver = "," + resultSalReceiverparam;
                                    AlertReceiverParameters = ParamStrReceiver.Split(delimeterAlert, StringSplitOptions.None);
                                }
                            }

                            //END FOR RECEIVER SALUTATION

                            //START FOR SENDER SALUTATION

                            if (alertType == "SUBISU")
                            {
                                if (AlertSalMessage.Contains("%s"))
                                {
                                    AlertSalMessage = sMSEnable.GetFinalMessage(AlertSalMessage, AlertSalParameters);
                                    AlertSalMessage = AlertSalMessage.Replace("\\n", "\n");
                                }
                            }
                            if (alertType == "SUBISUR")
                            {
                                if (AlertSalMessage.Contains("%s"))
                                {
                                    AlertSalMessage = sMSEnable.GetFinalMessage(AlertSalMessage, AlertSalParameters);
                                    AlertSalMessage = AlertSalMessage.Replace("\\n", "\n");
                                }
                            }
                            if (alertType == "VIANET")
                            {
                                if (AlertSalMessage.Contains("%s"))
                                {
                                    AlertSalMessage = sMSEnable.GetFinalMessage(AlertSalMessage, AlertSalParameters);
                                    AlertSalMessage = AlertSalMessage.Replace("\\n", "\n");
                                }
                            }
                            if (alertType == "VIANETR")
                            {
                                if (AlertSalMessage.Contains("%s"))
                                {
                                    AlertSalMessage = sMSEnable.GetFinalMessage(AlertSalMessage, AlertSalParameters);
                                    AlertSalMessage = AlertSalMessage.Replace("\\n", "\n");
                                }
                            }
                            if (alertType == "WLINK")
                            {
                                if (AlertSalMessage.Contains("%s"))
                                {
                                    AlertSalMessage = sMSEnable.GetFinalMessage(AlertSalMessage, AlertSalParameters);
                                    AlertSalMessage = AlertSalMessage.Replace("\\n", "\n");
                                }
                            }
                            if (alertType == "WLINKR")
                            {
                                if (AlertSalMessage.Contains("%s"))
                                {
                                    AlertSalMessage = sMSEnable.GetFinalMessage(AlertSalMessage, AlertSalParameters);
                                    AlertSalMessage = AlertSalMessage.Replace("\\n", "\n");
                                }
                            }


                            //END FOR SENDER SALUTATION

                            //START FOR SENDER SALUTATION

                            if (((alertType == "COU") && (vid != "")) || (alertType == "NEA") || (alertType == "NEAR")
                                || (alertType == "KP") || (alertType == "KPR")
                                || (alertType == "NW") || (alertType == "NWR") || (alertType == "MER") || (alertType == "WSEL"))
                            {
                                if (AlertSalMessage.Contains("%s"))
                                {
                                    AlertSalMessage = sMSEnable.GetFinalMessage(AlertSalMessage, AlertSalParameters);
                                    AlertSalMessage = AlertSalMessage.Replace("\\n", "\n");
                                }
                            }
                            else if ((alertType == "TOPUP") || (alertType == "TOPR") ||
                                (alertType == "WWD") || (alertType == "WBD") || (alertType == "BWD") || (alertType == "BBD") ||
                                (alertType == "CIC"))
                            {
                                if (AlertSalMessage.Contains("%s"))
                                {
                                    AlertReceiveMessage = sMSEnable.GetFinalMessage(AlertSalMessage, AlertReceiverParameters);
                                    AlertReceiveMessage = AlertReceiveMessage.Replace("\\n", "\n");

                                    AlertSalMessage = sMSEnable.GetFinalMessage(AlertSalMessage, AlertSalParameters);
                                    //AlertSalMessage = AlertSalMessage.Replace("\\r\\n", "\r\n");
                                    AlertSalMessage = AlertSalMessage.Replace("\\n", "\n");
                                }
                            }
                            else
                            {
                                if (AlertSalMessage.Contains("%s"))
                                {
                                    //AlertReceiveMessage = sMSEnable.GetFinalMessage(AlertSalMessage, AlertReceiverParameters);
                                    //AlertReceiveMessage = AlertReceiveMessage.Replace("\\n", "\n");

                                    AlertSalMessage = sMSEnable.GetFinalMessage(AlertSalMessage, AlertSalParameters);
                                    //AlertSalMessage = AlertSalMessage.Replace("\\r\\n", "\r\n");
                                    AlertSalMessage = AlertSalMessage.Replace("\\n", "\n");
                                }
                            }

                            //END FOR SENDER SALUTATION


                            //START FOR MSG

                            string msgName; string resultparm; String ParamStr; string GetMerchantName;

                            if ((alertType == "COU") && (vid != ""))
                            {
                                GetMerchantName = getMerchantDetails.PassVIdToGetMerchantName(vid);

                                msgName = amount.ToString();
                                resultparm = GetMerchantName.ToString();
                                ParamStr = "," + msgName + "," + resultparm + "," + couponNumber;
                                AlertParameters = ParamStr.Split(delimeterAlert, StringSplitOptions.None);
                            }
                            if (alertType == "MER")
                            {
                                GetMerchantName = getMerchantDetails.PassVIdToGetMerchantName(vid);

                                msgName = amount.ToString();
                                resultparm = GetMerchantName.ToString();
                                ParamStr = "," + msgName + "," + resultparm;
                                AlertParameters = ParamStr.Split(delimeterAlert, StringSplitOptions.None);
                            }
                            else if ((alertType == "TOPUP") || alertType == "TOPR")
                            {
                                msgName = getMerchantNameForSMS.ToString();
                                resultparm = amount.ToString();
                                ParamStr = "," + msgName + "," + resultparm + "," + destmobile.Trim().ToString();
                                AlertParameters = ParamStr.Split(delimeterAlert, StringSplitOptions.None);
                            }
                            else if ((alertType == "NEA") || (alertType == "NEAR") || (alertType == "KP") || (alertType == "KPR")
                                || (alertType == "NW") || (alertType == "NWR"))
                            {
                                msgName = amount.ToString();
                                resultparm = createdDate;
                                ParamStr = "," + msgName + "," + resultparm;
                                AlertParameters = ParamStr.Split(delimeterAlert, StringSplitOptions.None);
                            }
                            else if ((alertType == "WWS") || (alertType == "WBS") || (alertType == "BWS") || (alertType == "BBS"))
                            {
                                msgName = amount.ToString();
                                resultparm = createdDate;
                                ParamStr = "," + msgName + "," + destmobile + "," + resultparm;
                                AlertParameters = ParamStr.Split(delimeterAlert, StringSplitOptions.None);
                            }
                            else if ((alertType == "WWD") || (alertType == "WBD"))
                            {
                                msgName = amount.ToString();
                                resultparm = createdDate;
                                ParamStr = "," + msgName + "," + mobile + "," + resultparm;
                                AlertParameters = ParamStr.Split(delimeterAlert, StringSplitOptions.None);
                            }
                            else if ((alertType == "BWD") || (alertType == "BBD") || (alertType == "BWDO") || (alertType == "WSEL"))
                            {
                                if (mobile == destmobile)
                                {
                                    msgName = amount.ToString();
                                    resultparm = createdDate;
                                    ParamStr = "," + msgName + "," + resultparm;
                                    AlertParameters = ParamStr.Split(delimeterAlert, StringSplitOptions.None);
                                }
                                else
                                {
                                    msgName = amount.ToString();
                                    resultparm = createdDate;
                                    ParamStr = "," + msgName + "," + mobile + "," + resultparm;
                                    AlertParameters = ParamStr.Split(delimeterAlert, StringSplitOptions.None);
                                }

                            }
                            else if (alertType == "CO")
                            {
                                msgName = couponNumber.ToString();
                                resultparm = amount.ToString();
                                ParamStr = "," + msgName + "," + resultparm;
                                AlertParameters = ParamStr.Split(delimeterAlert, StringSplitOptions.None);
                            }
                            else if ((alertType == "CIC") || (alertType == "CID") || (alertType == "COC"))
                            {
                                resultparm = amount.ToString();
                                ParamStr = "," + resultparm;
                                AlertParameters = ParamStr.Split(delimeterAlert, StringSplitOptions.None);
                            }
                            else if ((alertType == "COD") || (alertType == "COW")) //Cash Out Debited & Cash out Withdraw
                            {
                                resultparm = amount.ToString();
                                ParamStr = "," + resultparm + "," + destmobile;
                                AlertParameters = ParamStr.Split(delimeterAlert, StringSplitOptions.None);
                            }
                            else if (alertType == "COR") //Cash Out Rejected
                            {
                                resultparm = "";
                                ParamStr = "," + resultparm;
                                AlertParameters = ParamStr.Split(delimeterAlert, StringSplitOptions.None);
                            }
                            else if (alertType == "SHARER")
                            {
                                resultparm = amount.ToString();
                                ParamStr = "," + resultparm + "," + couponNumber + "," + createdDate;
                                AlertParameters = ParamStr.Split(delimeterAlert, StringSplitOptions.None);
                            }
                            else if (alertType == "SHARE")
                            {
                                resultparm = amount.ToString();
                                ParamStr = "," + resultparm + "," + couponNumber + "," + createdDate;
                                AlertParameters = ParamStr.Split(delimeterAlert, StringSplitOptions.None);
                            }
                            else if (alertType == "SUBISUR")
                            {
                                resultparm = amount.ToString();
                                ParamStr = "," + resultparm + "," + couponNumber + "," + createdDate;
                                AlertParameters = ParamStr.Split(delimeterAlert, StringSplitOptions.None);
                            }
                            else if (alertType == "SUBISU")
                            {
                                resultparm = amount.ToString();
                                ParamStr = "," + resultparm + "," + couponNumber + "," + createdDate;
                                AlertParameters = ParamStr.Split(delimeterAlert, StringSplitOptions.None);
                            }
                            else if (alertType == "VIANETR")
                            {
                                resultparm = amount.ToString();
                                ParamStr = "," + resultparm + "," + couponNumber + "," + createdDate;
                                AlertParameters = ParamStr.Split(delimeterAlert, StringSplitOptions.None);
                            }
                            else if (alertType == "VIANET")
                            {
                                resultparm = amount.ToString();
                                ParamStr = "," + resultparm + "," + couponNumber + "," + createdDate;
                                AlertParameters = ParamStr.Split(delimeterAlert, StringSplitOptions.None);
                            }
                            else if (alertType == "WLINKR")
                            {
                                resultparm = amount.ToString();
                                ParamStr = "," + resultparm + "," + couponNumber + "," + createdDate;
                                AlertParameters = ParamStr.Split(delimeterAlert, StringSplitOptions.None);
                            }
                            else if (alertType == "WLINK")
                            {
                                resultparm = amount.ToString();
                                ParamStr = "," + resultparm + "," + couponNumber + "," + createdDate;
                                AlertParameters = ParamStr.Split(delimeterAlert, StringSplitOptions.None);
                            }

                            //END FOR MSG

                            //START FOR SMS MESSAGE

                            if (AlertMessage.Contains("%s"))
                            {
                                AlertMessage = sMSEnable.GetFinalMessage(AlertMessage, AlertParameters);
                                AlertMessage = AlertMessage.Replace("\\n", "\n");
                            }

                            //END FOR SMS MESSAGE

                        }
                    }
                }

                //SENDER
                string messagereply = "";
                if ((alertType == "SUBISU") || (alertType == "SUBISUR"))
                {
                    if (ForSender == "T")
                    {
                        messagereply = AlertSalMessage;
                        messagereply += AlertMessage + createdDate + Environment.NewLine;
                        messagereply += AlertRegMessage;
                    }
                }
                else if ((alertType == "VIANET") || (alertType == "VIANETR"))
                {
                    if (ForSender == "T")
                    {
                        messagereply = AlertSalMessage;
                        messagereply += AlertMessage + createdDate + Environment.NewLine;
                        messagereply += AlertRegMessage;
                    }
                }
                if ((alertType == "WLINK") || (alertType == "WLINKR"))
                {
                    if (ForSender == "T")
                    {
                        messagereply = AlertSalMessage;
                        messagereply += AlertMessage + createdDate + Environment.NewLine;
                        messagereply += AlertRegMessage;
                    }
                }
                else
                {
                    if (ForSender == "T")
                    {
                        messagereply = AlertSalMessage;
                        messagereply += AlertMessage;
                        messagereply += AlertRegMessage;
                    }
                }


                //RECEIVER
                string messagereplyReceiver = "";
                if (ForReceiver == "T")
                {
                    messagereplyReceiver = AlertReceiveMessage;
                    messagereplyReceiver += AlertMessage;
                    messagereplyReceiver += AlertRegMessage;
                }

                var client = new WebClient();


                //FOR SENDER
                if (mobile.Trim().Substring(0, 1) == "9") //FOR ALL
                {
                    //var content = client.DownloadString(
                    //        SMSNCELL
                    //        + "977" + mobile.Trim() + "&message=" + messagereply + "");

                    SendSMS sendSMS = new SendSMS();
                    sendSMS.pushSMS(mobile, messagereply);

                }


                //FOR RECEIVER
                if (destmobile != "")
                {
                    if (destmobile.Trim().Substring(0, 1) == "9") //FOR ALL
                    {
                        //var content = client.DownloadString(SMSNCELL
                        //    + "977" + destmobile.Trim() + "&message=" + messagereplyReceiver + "");

                        SendSMS sendSMS = new SendSMS();
                        sendSMS.pushSMS(destmobile, messagereplyReceiver);
                    }


                }


            }


        }
    }
}