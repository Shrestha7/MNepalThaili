
using System;
using System.Data;
using System.Net;
using WCF.MNepal.Helper;
using WCF.MNepal.Utilities;

namespace MNepalAPI.Helper
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
            string CSMSEnable = "true";
            if ((!string.IsNullOrEmpty(alertType)) && (!string.IsNullOrEmpty(custmobile)) && (!string.IsNullOrEmpty(amount)))
            {
                string mobile = custmobile;

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

                            string resultSalSenderparam = CustCheckUtils.GetName(mobile);
                            String ParamStrSal = "," + resultSalSenderparam;
                            AlertSalParameters = ParamStrSal.Split(delimeterAlert, StringSplitOptions.None);

                            
                            //START FOR SENDER SALUTATION

                            if (alertType == "SHARE")
                            {
                                if (AlertSalMessage.Contains("%s"))
                                {
                                    AlertSalMessage = sMSEnable.GetFinalMessage(AlertSalMessage, AlertSalParameters);
                                    AlertSalMessage = AlertSalMessage.Replace("\\n", "\n");
                                }
                            }

                            //END FOR SENDER SALUTATION


                            //START FOR MSG

                            string msgName; string resultparm; String ParamStr;
                            
                            if (alertType == "SHARER")
                            {
                                resultparm = amount.ToString();
                                ParamStr = "," + resultparm + "," + couponNumber + "," + createdDate;
                                AlertParameters = ParamStr.Split(delimeterAlert, StringSplitOptions.None);
                            }

                            //END FOR MSG

                            //START FOR SENDER SALUTATION

                            if (alertType == "SHARER")
                            {
                                if (AlertSalMessage.Contains("%s"))
                                {
                                    AlertSalMessage = sMSEnable.GetFinalMessage(AlertSalMessage, AlertSalParameters);
                                    AlertSalMessage = AlertSalMessage.Replace("\\n", "\n");
                                }
                            }

                            //END FOR SENDER SALUTATION


                            //START FOR MSG

                            if (alertType == "SHARE")
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
                if (ForSender == "T")
                {
                    messagereply = AlertSalMessage;
                    messagereply += AlertMessage + createdDate + Environment.NewLine;
                    messagereply += AlertRegMessage;
                }

                SendSMS sendSMS  = new SendSMS();
                sendSMS.pushSMS(mobile, messagereply);               

                CSMSEnable = messagereply;

            }
            else
            {
                CSMSEnable = "false";
            }

            return CSMSEnable;

        }

    }
}



