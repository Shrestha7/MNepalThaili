using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Web;
using WCF.MNepal.Models;
using WCF.MNepal.UserModels;

namespace WCF.MNepal.Utilities
{
    public class EmailUtils
    {
        public static DataTable GetEmailAlert(string alertType)
        {
            var objModel = new EmailUserModels();
            var objUserInfo = new EmailMsg
            {
                EmailType = alertType,
                Mode = "GM" //Get Msg
            };
            return objModel.GetEmailInformation(objUserInfo);
        }

        public static DataTable GetEmailEnableCheck(string mid)
        {
            var objModel = new EmailUserModels();
            var objUserInfo = new EmailMsg
            {
                MID = mid,
                Mode = "EMLE" //Get Email Enable Check
            };
            return objModel.GetEmailEnableCheck(objUserInfo);
        }

        public static DataTable GetMerchantDetail(string mobileNo)
        {
            var objModel = new EmailUserModels();
            var objUserInfo = new EmailMsg
            {
                MobileNo = mobileNo,
                Mode = "GMDBU" //Get mERCHANT dETAIL BY uSERnAME
            };
            return objModel.GetMerchantDetail(objUserInfo);
        }

        public static DataTable GetEmailDetail(string merchantType)
        {
            var objModel = new EmailUserModels();
            var objUserInfo = new EmailMsg
            {
                MerchantType = merchantType,
                Mode = "GED" //Get Email Detail
            };
            return objModel.GetEmailDetail(objUserInfo);
        }

        ///
        #region To send mail

        //To send mail
        public void SendMail(string DestinationAddress, string Subject, string Message) //Single Mail
        {
            try
            {
                if (string.IsNullOrEmpty(DestinationAddress))
                {
                    return;
                }

                if (string.IsNullOrEmpty(Subject))
                {
                    return;
                }
                if (string.IsNullOrEmpty(Message))
                {
                    return;
                }

                using (SmtpClient client = new SmtpClient())
                {
                    MailMessage mail = new MailMessage("noreply@nibl.com.np", DestinationAddress); //donotreply@mnepal.com
                    client.Port = 25;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.UseDefaultCredentials = false;
                    client.Host = "172.31.220.1";//smtp.mos.com.np
                    mail.Subject = Subject;
                    mail.Body = Message;
                    mail.IsBodyHtml = true;
                    try
                    {
                        client.Send(mail);
                    }
                    catch (SmtpException)
                    {
                        throw; //new Exception(exception.Message);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion
    }
}