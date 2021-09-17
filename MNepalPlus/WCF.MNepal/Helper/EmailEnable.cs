using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Web;
using WCF.MNepal.Utilities;

namespace WCF.MNepal.Helper
{
    public class EmailEnable
    {
        public string IsEmailEnableCheck(string vid)
        {
            string EmailEnable = "";
            if (!string.IsNullOrEmpty(vid))
            {
                DataTable dtEmailEnable = EmailUtils.GetEmailEnableCheck(vid);
                if (dtEmailEnable != null)
                {
                    int i = 0;
                    if (dtEmailEnable.Rows.Count != 0)
                    {
                        for (i = 0; i < dtEmailEnable.Rows.Count; i++)
                        {
                            EmailEnable = Convert.ToString(dtEmailEnable.Rows[i]["EmailStatus"]);
                        }
                        return EmailEnable;
                    }
                }
                else
                {
                    return EmailEnable;
                }
            }

            return EmailEnable;
        }


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
                    MailMessage mail = new MailMessage("noreply@nibl.com.np", DestinationAddress); // donotreply@mnepal.com
                    client.Port = 25;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.UseDefaultCredentials = false;
                    client.Host = "172.31.220.1";//smtp.mos.com.np //10.1.2.5
                    mail.Subject = Subject;
                    mail.Body = Message;
                    mail.IsBodyHtml = true;
                    try
                    {
                        client.Send(mail);
                    }
                    catch (SmtpException exception)
                    {
                        exception.ToString();
                        // throw new Exception(exception.Message);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string GetFinalMessage(string Message, string[] Params)
        {
            int ParamCnt = 1;
            int indx = 0;
            int strtIndex = -1;
            string srchString = "%s";

            while (
                ((indx = Message.IndexOf(srchString, strtIndex + 1)) != -1) &&
                (Params.Length > ParamCnt)
                )
            {
                Message = Message.Remove(indx, srchString.Length);
                Message = Message.Insert(indx, Params[ParamCnt].ToString());
                ParamCnt++;
                strtIndex = indx;
            }

            return Message;
        }
    }
}