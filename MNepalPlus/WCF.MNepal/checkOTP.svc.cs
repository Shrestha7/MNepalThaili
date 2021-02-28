using MNepalProject.Helper;
using MNepalProject.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using WCF.MNepal.Helper;
using WCF.MNepal.Models;
using WCF.MNepal.Utilities;

namespace WCF.MNepal
{
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class checkOTP
    {
        string SMSNTC = System.Web.Configuration.WebConfigurationManager.AppSettings["MNepalNTCSMSServerUrl"];
        string SMSNCELL = System.Web.Configuration.WebConfigurationManager.AppSettings["MNepalSMSServerUrl"];

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public string SendOTP(Stream input)
        {
            StreamReader sr = new StreamReader(input);
            string s = sr.ReadToEnd();
            sr.Dispose();

            NameValueCollection qs = HttpUtility.ParseQueryString(s);

            string userName = qs["mobile"];
            string src = qs["src"];
            string userType = qs["userType"];

            ReplyMessage replyMessage = new ReplyMessage();
            string result = "";
            string code = string.Empty;
            string statusCode = string.Empty;
            string message = string.Empty;

            if ((userName == null) || (src == null))
            {
                // throw ex
                statusCode = "400";
                message = "Parameters Missing/Invalid";
            }
            else
            {
                try
                {

                    //for  usertype=user
                    if (userType == "user")
                    {
                        if (UserNameCheck.IsValidUserNameForgotPassword(userName))
                        {
                            string validStatus = "";
                            DataTable dtableStatusResult = CustCheckUtils.GetCustStatusInfo(userName);
                            if (dtableStatusResult != null)
                            {
                                foreach (DataRow dtableUser in dtableStatusResult.Rows)
                                {
                                    validStatus = dtableUser["Status"].ToString();
                                }
                            }

                            if (validStatus == "Expired")
                            {
                                statusCode = "400";
                                message = "Account is Blocked";
                                replyMessage.ResponseStatus(HttpStatusCode.InternalServerError, message);
                            }
                            if (validStatus == "Blocked")
                            {
                                statusCode = "400";
                                message = "Account is Blocked";
                                replyMessage.ResponseStatus(HttpStatusCode.InternalServerError, message);
                            }
                            else
                            {
                                if (LoginUtils.GetPasswordBlockTime(userName))
                                {
                                    //statusCode = "400";
                                    //result = "You have already attempt 3 times with wrong password,Please try again after 1 hour";
                                    //replyMessage.Response = "Account is Blocked";
                                    //replyMessage.ResponseStatus(HttpStatusCode.BadRequest, replyMessage.Response);
                                    //message = result;


                                    statusCode = "417";
                                    result = LoginUtils.GetMessage("03");
                                    replyMessage.Response = "Account is Blocked";
                                    replyMessage.ResponseStatus(HttpStatusCode.ExpectationFailed, replyMessage.Response);
                                    message = result;




                                }
                                else
                                {
                                    TraceIdGenerator otp = new TraceIdGenerator();
                                    code = otp.GetUniqueOTPKey();
                                    string messagereply = "Dear " + CustCheckUtils.GetName(userName) + "," + "\n";
                                    messagereply += " Your Verification Code is " + code
                                        + "." + "\n" + "Close this message and enter the code.";
                                    messagereply += "-NIBL Thaili";

                                    var client = new WebClient();

                                    string mobile = "";
                                    mobile = userName;

                                    if ((mobile.Substring(0, 3) == "980") || (mobile.Substring(0, 3) == "981")) //FOR NCELL
                                    {
                                        //FOR NCELL
                                        //var contents = client.DownloadString("http://smsvas.mos.com.np/PostSMS.ashx?QueueId=&TelecomId=2&KeywordId=3&Parts=1&Password=mnepal120&From=37878&To="
                                        //+ "977" + mobile + "&message=" + messagereply + "");

                                        var contents = client.DownloadString(SMSNCELL
                                        + "977" + mobile + "&message=" + messagereply + "");
                                    }
                                    else if ((mobile.Substring(0, 3) == "985") || (mobile.Substring(0, 3) == "984")
                                        || (mobile.Substring(0, 3) == "986"))
                                    {
                                        //FOR NTC
                                        //var contents = client.DownloadString("http://smsvas.mos.com.np/PostSMS.ashx?QueueId=&TelecomId=1&KeywordId=3&Parts=1&Password=mnepal120&From=37878&To="
                                        //    + "977" + mobile + "&message=" + messagereply + "");

                                        var contents = client.DownloadString(SMSNTC
                                            + "977" + mobile + "&message=" + messagereply + "");
                                    }

                                    SMSLog log = new SMSLog();
                                    log.SentBy = mobile;
                                    log.Purpose = "Check OTP";
                                    log.UserName = userName;
                                    log.Message = messagereply;

                                    CustCheckUtils.LogSMS(log);

                                    SMSLog OTPLog = new SMSLog();
                                    OTPLog.UserName = userName;
                                    OTPLog.Message = code;
                                    OTPLog.Purpose = "Check OTP";

                                    CustCheckUtils.LogOTP(OTPLog);

                                    statusCode = "200";
                                    message = "";
                                }
                            }
                        }
                        else
                        {
                            //statusCode = "400";
                            //result = "The number is not registered !!";
                            //replyMessage.Response = "The number is not registered !!";
                            //replyMessage.ResponseStatus(HttpStatusCode.BadRequest, replyMessage.Response);
                            //message = result;

                            statusCode = "400";
                            //result = "A OTP code will be sent to your phone if the account exists in our system.";
                            //replyMessage.Response = "A OTP code will be sent to your phone if the account exists in our system.";

                            result = LoginUtils.GetMessage("04");
                            replyMessage.Response = LoginUtils.GetMessage("04");

                            replyMessage.ResponseStatus(HttpStatusCode.BadRequest, replyMessage.Response);
                            message = result;
                        }
                    }

                    //for usertype=agent
                    if (userType == "agent")
                    {
                        //if (UserNameCheck.IsValidUserNameForgotPassword(userName))
                        if (UserNameCheck.IsValidAgent(userName))
                        {
                            string validStatus = "";
                            DataTable dtableStatusResult = CustCheckUtils.GetCustStatusInfo(userName);
                            if (dtableStatusResult != null)
                            {
                                foreach (DataRow dtableUser in dtableStatusResult.Rows)
                                {
                                    validStatus = dtableUser["Status"].ToString();
                                }
                            }

                            if (validStatus == "Expired")
                            {
                                statusCode = "400";
                                message = "Account is Blocked";
                                replyMessage.ResponseStatus(HttpStatusCode.InternalServerError, message);
                            }
                            if (validStatus == "Blocked")
                            {
                                statusCode = "400";
                                message = "Account is Blocked";
                                replyMessage.ResponseStatus(HttpStatusCode.InternalServerError, message);
                            }
                            else
                            {
                                if (LoginUtils.GetPasswordBlockTime(userName))
                                {
                                    //statusCode = "400";
                                    //result = "You have already attempt 3 times with wrong password,Please try again after 1 hour";
                                    //replyMessage.Response = "Account is Blocked";
                                    //replyMessage.ResponseStatus(HttpStatusCode.BadRequest, replyMessage.Response);
                                    //message = result;


                                    statusCode = "417";
                                    result = LoginUtils.GetMessage("03");
                                    replyMessage.Response = "Account is Blocked";
                                    replyMessage.ResponseStatus(HttpStatusCode.ExpectationFailed, replyMessage.Response);
                                    message = result;




                                }
                                else
                                {
                                    TraceIdGenerator otp = new TraceIdGenerator();
                                    code = otp.GetUniqueOTPKey();
                                    string messagereply = "Dear " + CustCheckUtils.GetName(userName) + "," + "\n";
                                    messagereply += " Your Verification Code is " + code
                                        + "." + "\n" + "Close this message and enter the code.";
                                    messagereply += "-NIBL Thaili";

                                    var client = new WebClient();

                                    string mobile = "";
                                    mobile = userName;

                                    if ((mobile.Substring(0, 3) == "980") || (mobile.Substring(0, 3) == "981")) //FOR NCELL
                                    {
                                        //FOR NCELL
                                        //var contents = client.DownloadString("http://smsvas.mos.com.np/PostSMS.ashx?QueueId=&TelecomId=2&KeywordId=3&Parts=1&Password=mnepal120&From=37878&To="
                                        //+ "977" + mobile + "&message=" + messagereply + "");

                                        var contents = client.DownloadString(SMSNCELL
                                        + "977" + mobile + "&message=" + messagereply + "");
                                    }
                                    else if ((mobile.Substring(0, 3) == "985") || (mobile.Substring(0, 3) == "984")
                                        || (mobile.Substring(0, 3) == "986"))
                                    {
                                        //FOR NTC
                                        //var contents = client.DownloadString("http://smsvas.mos.com.np/PostSMS.ashx?QueueId=&TelecomId=1&KeywordId=3&Parts=1&Password=mnepal120&From=37878&To="
                                        //    + "977" + mobile + "&message=" + messagereply + "");

                                        var contents = client.DownloadString(SMSNTC
                                            + "977" + mobile + "&message=" + messagereply + "");
                                    }

                                    SMSLog log = new SMSLog();
                                    log.SentBy = mobile;
                                    log.Purpose = "Check OTP";
                                    log.UserName = userName;
                                    log.Message = messagereply;

                                    CustCheckUtils.LogSMS(log);

                                    SMSLog OTPLog = new SMSLog();
                                    OTPLog.UserName = userName;
                                    OTPLog.Message = code;
                                    OTPLog.Purpose = "Check OTP";

                                    CustCheckUtils.LogOTP(OTPLog);

                                    statusCode = "200";
                                    message = "";
                                }
                            }
                        }
                        else
                        {
                            //statusCode = "400";
                            //result = "The number is not registered !!";
                            //replyMessage.Response = "The number is not registered !!";
                            //replyMessage.ResponseStatus(HttpStatusCode.BadRequest, replyMessage.Response);
                            //message = result;

                            statusCode = "400";
                           //result = "A OTP code will be sent to your phone if the account exists in our system.";
                            //replyMessage.Response = "A OTP code will be sent to your phone if the account exists in our system.";


                            result = LoginUtils.GetMessage("04");
                            replyMessage.Response = LoginUtils.GetMessage("04");
                            replyMessage.ResponseStatus(HttpStatusCode.BadRequest, replyMessage.Response);
                            message = result;







                        }
                    }

                    //for usertype=merchant

                    if (userType == "merchant")
                    { 
                        if (UserNameCheck.IsValidMerchant(userName))
                        {
                            string validStatus = "";
                            DataTable dtableStatusResult = CustCheckUtils.GetMerchantStatusInfo(userName);
                            if (dtableStatusResult != null)
                            {
                                foreach (DataRow dtableUser in dtableStatusResult.Rows)
                                {
                                    validStatus = dtableUser["Status"].ToString();
                                }
                            }

                            if (validStatus == "Expired")
                            {
                                statusCode = "400";
                                message = "Account is Blocked";
                                replyMessage.ResponseStatus(HttpStatusCode.InternalServerError, message);
                            }
                            if (validStatus == "Blocked")
                            {
                                statusCode = "400";
                                message = "Account is Blocked";
                                replyMessage.ResponseStatus(HttpStatusCode.InternalServerError, message);
                            }
                            else
                            {
                                if (LoginUtils.GetPasswordBlockTime(userName))
                                {
                                    //statusCode = "400";
                                    //result = "You have already attempt 3 times with wrong password,Please try again after 1 hour";
                                    //replyMessage.Response = "Account is Blocked";
                                    //replyMessage.ResponseStatus(HttpStatusCode.BadRequest, replyMessage.Response);
                                    //message = result;


                                    statusCode = "417";
                                    result = LoginUtils.GetMessage("03");
                                    replyMessage.Response = "Account is Blocked";
                                    replyMessage.ResponseStatus(HttpStatusCode.ExpectationFailed, replyMessage.Response);
                                    message = result;




                                }
                                else
                                {
                                    TraceIdGenerator otp = new TraceIdGenerator();
                                    code = otp.GetUniqueOTPKey();
                                    string messagereply = "Dear " + CustCheckUtils.GetName(userName) + "," + "\n";
                                    messagereply += " Your Verification Code is " + code
                                        + "." + "\n" + "Close this message and enter the code.";
                                    messagereply += "-NIBL Thaili";

                                    var client = new WebClient();

                                    string mobile = "";
                                    mobile = userName;

                                    if ((mobile.Substring(0, 3) == "980") || (mobile.Substring(0, 3) == "981")) //FOR NCELL
                                    {
                                        //FOR NCELL
                                        //var contents = client.DownloadString("http://smsvas.mos.com.np/PostSMS.ashx?QueueId=&TelecomId=2&KeywordId=3&Parts=1&Password=mnepal120&From=37878&To="
                                        //+ "977" + mobile + "&message=" + messagereply + "");

                                        var contents = client.DownloadString(SMSNCELL
                                        + "977" + mobile + "&message=" + messagereply + "");
                                    }
                                    else if ((mobile.Substring(0, 3) == "985") || (mobile.Substring(0, 3) == "984")
                                        || (mobile.Substring(0, 3) == "986"))
                                    {
                                        //FOR NTC
                                        //var contents = client.DownloadString("http://smsvas.mos.com.np/PostSMS.ashx?QueueId=&TelecomId=1&KeywordId=3&Parts=1&Password=mnepal120&From=37878&To="
                                        //    + "977" + mobile + "&message=" + messagereply + "");

                                        var contents = client.DownloadString(SMSNTC
                                            + "977" + mobile + "&message=" + messagereply + "");
                                    }

                                    SMSLog log = new SMSLog();
                                    log.SentBy = mobile;
                                    log.Purpose = "Check OTP";
                                    log.UserName = userName;
                                    log.Message = messagereply;

                                    CustCheckUtils.LogSMS(log);

                                    SMSLog OTPLog = new SMSLog();
                                    OTPLog.UserName = userName;
                                    OTPLog.Message = code;
                                    OTPLog.Purpose = "Check OTP";

                                    CustCheckUtils.LogOTP(OTPLog);

                                    statusCode = "200";
                                    message = "";
                                }
                            }
                        }
                        else
                        {
                            //statusCode = "400";
                            //result = "The number is not registered !!";
                            //replyMessage.Response = "The number is not registered !!";
                            //replyMessage.ResponseStatus(HttpStatusCode.BadRequest, replyMessage.Response);
                            //message = result;

                            statusCode = "400";
                            //result = "A OTP code will be sent to your phone if the account exists in our system.";
                            //replyMessage.Response = "A OTP code will be sent to your phone if the account exists in our system.";


                            result = LoginUtils.GetMessage("04");
                            replyMessage.Response = LoginUtils.GetMessage("04");
                            replyMessage.ResponseStatus(HttpStatusCode.BadRequest, replyMessage.Response);
                            message = result;







                        }
                    }


                }
                catch (Exception ex)
                {
                    statusCode = "400";
                    result = "Please Contact to the administrator !!" + ex;
                    replyMessage.Response = result;
                    replyMessage.ResponseStatus(HttpStatusCode.BadRequest, replyMessage.Response);
                    message = result;
                }
            }

            if (statusCode != "200")
            {
                var v = new
                {
                    StatusCode = Convert.ToInt32(statusCode),
                    StatusMessage = message
                };
                result = JsonConvert.SerializeObject(v);
            }
            else if (statusCode == "200")
            {
                var v = new
                {
                    StatusCode = Convert.ToInt32(statusCode),
                    StatusMessage = message
                };
                result = JsonConvert.SerializeObject(v);
            }

            return result;
        }

        #region check otp

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        public string CheckOTP(Stream input)
        {
            StreamReader sr = new StreamReader(input);
            string s = sr.ReadToEnd();
            sr.Dispose();
            NameValueCollection qs = HttpUtility.ParseQueryString(s);


            string mobile = qs["mobile"];
            string src = qs["src"];

            string otp = qs["otp"];

            ReplyMessage replyMessage = new ReplyMessage();

            string result = "";
            string statusCode = string.Empty;
            string message = string.Empty;
            string failedmessage = string.Empty;

            TraceIdGenerator tig = new TraceIdGenerator();
            string tid = tig.GenerateTraceID();

            if ((tid == "null") || (mobile == "null") ||
                (src == "null"))
            {
                statusCode = "400";
                message = "Parameters Missing/Invalid";
                failedmessage = message;
            }
            else
            {
                string pin = IsValidOTP(mobile);

                if (otp == pin)
                {
                    replyMessage.Response = "Your OTP is verified.";
                    replyMessage.ResponseStatus(HttpStatusCode.OK, replyMessage.Response);

                    return replyMessage.Response;
                }
                else
                {
                    statusCode = "400";
                    replyMessage.Response = "Invalid OTP";
                    replyMessage.ResponseStatus(HttpStatusCode.InternalServerError, replyMessage.Response);
                    failedmessage = replyMessage.Response;
                }
                return replyMessage.Response;
            }

            if (statusCode != "200")
            {
                var v = new
                {
                    StatusCode = Convert.ToInt32(statusCode),
                    StatusMessage = message
                };
                result = JsonConvert.SerializeObject(v);
            }

            return result;
        }

        #endregion

        public string IsValidOTP(string username)
        {
            string otp = "";
            if (!string.IsNullOrEmpty(username))
            {
                DataTable dtCheckUserName = CustCheckUtils.CheckOTP(username);
                if (dtCheckUserName.Rows.Count > 0)
                {
                    otp = dtCheckUserName.Rows[0]["Message"].ToString();

                    return otp;
                }
                else
                {
                    return otp;
                }
            }
            return otp;
        }
    }
}
