﻿using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using WCF.MNepal.Models;
using MNepalProject.Controllers;
using MNepalProject.Models;
using System.Net;
using System.IO;
using System.Collections.Specialized;
using System.Data;
using System.Web;
using MNepalProject.DAL;
using MNepalProject.Services;
using Newtonsoft.Json.Linq;
using WCF.MNepal.Utilities;
using Newtonsoft.Json;
using WCF.MNepal.Helper;
using MNepalProject.Helper;
using WCF.MNepal.ErrorMsg;

namespace WCF.MNepal
{
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Single, InstanceContextMode = InstanceContextMode.Single)]
    public class remit
    {
        /// <summary>
        /// Token Request
        /// </summary>
        /// <param name="tid"></param>
        /// <param name="mobile"></param>
        /// <param name="sc"></param>
        /// <param name="sa"></param>
        /// <param name="da"></param>
        /// <param name="amount"></param>
        /// <param name="code"></param>
        /// <param name="bn"></param>
        /// <param name="clientCode"></param>
        /// <param name="note"></param>
        /// <param name="pin"></param>
        /// <param name="src"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        public string token(string tid, string mobile, string sc, string sa, string da, string amount, string code, string bn, string clientCode, string note, string pin, string src)
        {
            /*
                tid - trace id
                mobile - user's mobile number
                sc - service code
                sa - source account //not needed
                da - recipient's mobile
                amount - transfer amount
                code - secret code
                bn - benificialName
                clientCode = clientCode
                note - (optional)
                pin - (can be empty)
                src - channel - sms, http etc.
              
                tid = tid;
                mobile = source mobile;
                sc = sc;
                da = destination mobile;
                amount =amount;
                pin = user pin ;
                note = secret code;
                src = src;
             */

            string result = "";
            string benificialName = bn;
            ReplyMessage replyMessage = new ReplyMessage();
            FundTransferRemit ftremit = new FundTransferRemit
            {
                tid = tid,
                mobile = mobile,
                sc = sc, //40
                da = da,
                amount = amount,
                pin = pin,
                note = code, //secret code placed in Note
                sourcechannel = src
            };

            //SMS
            string SMSNTC = System.Web.Configuration.WebConfigurationManager.AppSettings["MNepalNTCSMSServerUrl"];
            string SMSNCELL = System.Web.Configuration.WebConfigurationManager.AppSettings["MNepalSMSServerUrl"];

            //SC: Remit Token: 40 - Wallet
            string totalAmount = string.Empty;
            string totalCount = string.Empty;
            string balance = string.Empty;
            string statusCode = "0";
            string message = string.Empty;
            string failedmessage = string.Empty;

            if ((tid == null) || (sc == null) || (mobile == null) || (da == null) || (amount == null) || (pin == null) ||
                (src == null) || (code == null))
            {
                statusCode = "400";
                message = "Parameters Missing/Invalid";
                failedmessage = message;
            }
            else
            {
                TransLimitCheck transLimitCheck = new TransLimitCheck();
                string resultTranLimit = transLimitCheck.LimitCheck(mobile, da, amount, sc, ftremit.pin, src);

                var jsonDataResult = JObject.Parse(resultTranLimit);
                statusCode = jsonDataResult["StatusCode"].ToString();
                string statusMsg = jsonDataResult["StatusMessage"].ToString();
                message = jsonDataResult["StatusMessage"].ToString();
                failedmessage = message;

                if ((statusCode == "200") && (message == "Success"))
                {
                    //start: checking trace id
                    do
                    {
                        TraceIdGenerator traceid = new TraceIdGenerator();
                        tid = traceid.GenerateUniqueTraceID();
                        ftremit.tid = tid;

                        bool traceIdCheck = false;
                        traceIdCheck = TraceIdCheck.IsValidTraceId(tid);
                        if (traceIdCheck == true)
                        {
                            result = "Trace ID Repeated";
                        }
                        else
                        {
                            result = "false";
                        }

                    } while (result == "Trace ID Repeated");

                    //End: TraceId

                    MNRemit remitInfo = new MNRemit();
                    remitInfo.TraceID = ftremit.tid;
                    remitInfo.SenderMobileNo = mobile;
                    remitInfo.RecipientMobileNo = da;
                    remitInfo.BeneficialName = benificialName;
                    remitInfo.RequestTokenCode = code;
                    remitInfo.Amount = ftremit.amount;
                    remitInfo.Purpose = ftremit.note;
                    remitInfo.ClientCode = clientCode;
                    remitInfo.PIN = ftremit.pin;
                    remitInfo.TokenID = "";

                    remitInfo.ServiceCode = sc;
                    remitInfo.SourceChannel = src;

                    MNFundTransfer mnft = new MNFundTransfer(ftremit.tid, ftremit.sc, ftremit.mobile, null, ftremit.amount,
                        ftremit.da, ftremit.note, ftremit.pin, ftremit.sourcechannel);

                    int results = RemitUtils.InsertRemitInfo(remitInfo);
                    if (results > 0)
                    {

                        /*MNFundTransfer mnft = new MNFundTransfer(ftremit.tid, ftremit.sc, ftremit.mobile, null, ftremit.amount,
                        ftremit.da, ftremit.note, ftremit.pin, ftremit.sourcechannel);*/
                        var comfocuslog = new MNComAndFocusOneLog(mnft, DateTime.Now);
                        var mncomfocuslog = new MNComAndFocusOneLogsController();
                        string reply = mncomfocuslog.InsertIntoComFocusOne(comfocuslog);

                        //NOTE:- may be need to validate before insert into reply typpe
                        //start:insert into reply type as HTTP//
                        var replyType = new MNReplyType(ftremit.tid, "HTTP");
                        var mnreplyType = new MNReplyTypesController();
                        mnreplyType.InsertIntoReplyType(replyType);
                        //end:insert into reply type as HTTP//
                        MNTransactionMaster validTransactionData = new MNTransactionMaster();
                        //start:insert into transaction master//
                        if (mnft.valid())
                        {
                            var transaction = new MNTransactionMaster(mnft);
                            var mntransaction = new MNTransactionsController();
                            validTransactionData = mntransaction.Validate(transaction, mnft.pin);
                            result = validTransactionData.Response;

                            /*** ***/
                            if (validTransactionData.Response == "Error")
                            {
                                mnft.Response = "error";
                                mnft.ResponseStatus(HttpStatusCode.InternalServerError, "Internal server error - try again later, or contact support");
                                result = mnft.Response;
                                statusCode = "500";
                                message = "Internal server error - try again later, or contact support";
                                failedmessage = message;
                            }
                            else
                            {
                                ErrorMessage em = new ErrorMessage();
                                if ((result == "Trace ID Repeated") || (result == "Limit Exceed") || (result == "Invalid PIN")
                                    || (result == "Invalid Source User") || (result == "Invalid Destination User")
                                    || (result == "Invalid Product Request") || (result == ""))
                                {
                                    mnft.ResponseStatus(HttpStatusCode.BadRequest, result); //200 - OK
                                    statusCode = "400";
                                    message = result;
                                    failedmessage = message;
                                }
                                if (result.Substring(0, 5) == "Error")
                                {
                                    statusCode = "400";
                                    message = "Connection Failure from Gateway. Please Contact your Bank." + result;
                                    mnft.ResponseStatus(HttpStatusCode.InternalServerError, message);
                                    failedmessage = result;
                                }
                                if (result == "111")
                                {
                                    statusCode = result;
                                    message = em.Error_111/* + " " + result*/;
                                    failedmessage = message;
                                    mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
                                }
                                if (result == "114")
                                {
                                    statusCode = result;
                                    message = em.Error_114/* + " " + result*/;
                                    failedmessage = message;
                                    mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
                                }
                                if (result == "115")
                                {
                                    statusCode = result;
                                    message = em.Error_115/* + " " + result*/;
                                    failedmessage = message;
                                    mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
                                }
                                if (result == "116")
                                {
                                    statusCode = result;
                                    message = em.Error_116/* + " " + result*/;
                                    failedmessage = message;
                                    mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
                                }
                                if (result == "119")
                                {
                                    statusCode = result;
                                    message = em.Error_119/* + " " + result*/;
                                    failedmessage = message;
                                    mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
                                }
                                if (result == "121")
                                {
                                    statusCode = result;
                                    message = em.Error_121/* + " " + result*/;
                                    failedmessage = message;
                                    mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
                                }
                                if (result == "163")
                                {
                                    statusCode = result;
                                    message = em.Error_163/* + " " + result*/;
                                    failedmessage = message;
                                    mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
                                }
                                if (result == "180")
                                {
                                    statusCode = result;
                                    message = em.Error_180/* + " " + result*/;
                                    failedmessage = message;
                                    mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
                                }
                                if (result == "181")
                                {
                                    statusCode = result;
                                    message = em.Error_181/* + " " + result*/;
                                    failedmessage = message;
                                    mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
                                }
                                if (result == "182")
                                {
                                    statusCode = result;
                                    message = em.Error_182/* + " " + result*/;
                                    failedmessage = message;
                                    mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
                                }
                                if (result == "183")
                                {
                                    statusCode = result;
                                    message = em.Error_183/* + " " + result*/;
                                    failedmessage = message;
                                    mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
                                }
                                if (result == "184")
                                {
                                    statusCode = result;
                                    message = em.Error_184/* + " " + result*/;
                                    failedmessage = message;
                                    mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
                                }
                                if (result == "185")
                                {
                                    statusCode = result;
                                    message = em.Error_185/* + " " + result*/;
                                    failedmessage = message;
                                    mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
                                }
                                if (result == "186")
                                {
                                    statusCode = result;
                                    message = em.Error_186/* + " " + result*/;
                                    failedmessage = message;
                                    mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
                                }
                                if (result == "187")
                                {
                                    statusCode = result;
                                    message = em.Error_187/* + " " + result*/;
                                    failedmessage = message;
                                    mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
                                }
                                if (result == "188")
                                {
                                    statusCode = result;
                                    message = em.Error_188/* + " " + result*/;
                                    failedmessage = message;
                                    mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
                                }
                                if (result == "189")
                                {
                                    statusCode = result;
                                    message = em.Error_189/* + " " + result*/;
                                    failedmessage = message;
                                    mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
                                }
                                if (result == "190")
                                {
                                    statusCode = result;
                                    message = em.Error_190/* + " " + result*/;
                                    failedmessage = message;
                                    mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
                                }
                                if (result == "800")
                                {
                                    statusCode = result;
                                    message = em.Error_800/* + " " + result*/;
                                    failedmessage = message;
                                    mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
                                }
                                if (result == "902")
                                {
                                    statusCode = result;
                                    message = em.Error_902/* + " " + result*/;
                                    failedmessage = message;
                                    mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
                                }
                                if (result == "904")
                                {
                                    statusCode = result;
                                    message = em.Error_904/* + " " + result*/;
                                    failedmessage = message;
                                    mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
                                }
                                if (result == "906")
                                {
                                    statusCode = result;
                                    message = em.Error_906/* + " " + result*/;
                                    failedmessage = message;
                                    mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
                                }
                                if (result == "907")
                                {
                                    statusCode = result;
                                    message = em.Error_907/* + " " + result*/;
                                    failedmessage = message;
                                    mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
                                }
                                if (result == "909")
                                {
                                    statusCode = result;
                                    message = em.Error_909/* + " " + result*/;
                                    failedmessage = message;
                                    mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
                                }
                                if (result == "911")
                                {
                                    statusCode = result;
                                    message = em.Error_911/* + " " + result*/;
                                    failedmessage = message;
                                    mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
                                }
                                if (result == "913")
                                {
                                    statusCode = result;
                                    message = em.Error_913/* + " " + result*/;
                                    failedmessage = message;
                                    mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
                                }
                                if (result == "90")
                                {
                                    statusCode = result;
                                    message = em.Error_90/* + " " + result*/;
                                    failedmessage = message;
                                    mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
                                }
                                if (result == "91")
                                {
                                    statusCode = result;
                                    message = em.Error_91/* + " " + result*/;
                                    failedmessage = message;
                                    mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
                                }
                                if (result == "92")
                                {
                                    statusCode = result;
                                    message = em.Error_92/* + " " + result*/;
                                    failedmessage = message;
                                    mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
                                }
                                if (result == "94")
                                {
                                    statusCode = result;
                                    message = em.Error_94/* + " " + result*/;
                                    failedmessage = message;
                                    mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
                                }
                                if (result == "95")
                                {
                                    statusCode = result;
                                    message = em.Error_95/* + " " + result*/;
                                    failedmessage = message;
                                    mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
                                }
                                if (result == "98")
                                {
                                    statusCode = result;
                                    message = em.Error_98/* + " " + result*/;
                                    failedmessage = message;
                                    mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
                                }
                                if (result == "99")
                                {
                                    statusCode = result;
                                    message = em.Error_99/* + " " + result*/;
                                    failedmessage = message;
                                    mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
                                }
                                else
                                {
                                    mnft.ResponseStatus(HttpStatusCode.OK, result); //200 - OK
                                    statusCode = "200";
                                    message = result;

                                    var v = new
                                    {
                                        StatusCode = Convert.ToInt32(statusCode),
                                        StatusMessage = message
                                    };

                                    result = JsonConvert.SerializeObject(v);
                                }

                            }
                            
                            /*** ***/
                        }
                        else
                        {
                            mnft.Response = "error";
                            mnft.ResponseStatus(HttpStatusCode.BadRequest, "parameters missing/invalid"); //200 - OK
                            result = mnft.Response;
                            statusCode = "400";
                            message = "parameters missing/invalid";
                            failedmessage = message;
                        }
                        //end:insert into transaction master//
                    }
                    else
                    {
                        mnft.Response = "Remit parameters missing/invalid";
                        mnft.ResponseStatus(HttpStatusCode.BadRequest, "parameters missing/invalid"); //200 - OK
                        result = mnft.Response;
                        statusCode = "400";
                        message = result;
                        failedmessage = message;
                    }

                    OutgoingWebResponseContext response = WebOperationContext.Current.OutgoingResponse;

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        try
                        {
                            var jsonData = JObject.Parse(result);
                            var sCode = jsonData["StatusCode"];
                            string stMsg = jsonData["StatusMessage"].ToString();

                            var jData = JObject.Parse(stMsg);
                            
                            var token = jData["RequestedToken"];

                            //User Sender Mobile
                            string messagereply = "Dear Customer," + "\n";
                            messagereply += "You have sent NPR " + ftremit.amount + " to " + da + " and your Remit Token Request Code is " + token +
                                            " and secretCode is " + code + "." + "\n";
                            messagereply += "-NIBL Thaili";

                            var client = new WebClient();

                            if ((mobile.Substring(0, 3) == "980") || (mobile.Substring(0, 3) == "981")) //FOR NCELL
                            {
                                //FOR NCELL
                                //var content = client.DownloadString(
                                //    "http://smsvas.mos.com.np/PostSMS.ashx?QueueId=&TelecomId=2&KeywordId=3&Password=mnepal120&From=37878&To="
                                //    + "977" + mobile + "&message=" + messagereply + "");
                                var content = client.DownloadString(
                                    SMSNCELL
                                    + "977" + mobile + "&message=" + messagereply + "");
                            }
                            else if ((mobile.Substring(0, 3) == "985") || (mobile.Substring(0, 3) == "984")
                                     || (mobile.Substring(0, 3) == "986"))
                            {
                                //FOR NTC
                                //var content = client.DownloadString(
                                //    "http://smsvas.mos.com.np/PostSMS.ashx?QueueId=&TelecomId=1&KeywordId=3&Password=mnepal120&From=37878&To="
                                //    + "977" + mobile + "&message=" + messagereply + "");
                                var content = client.DownloadString(
                                    SMSNTC
                                    + "977" + mobile + "&message=" + messagereply + "");
                            }

                            /** Remit Cust Destination Mobile */

                            if (ftremit.da != "")
                            {
                                string messagereplyDestMobile = "Dear Customer," + "\n";
                                messagereplyDestMobile += "You have received NPR " + amount + " from " + mobile + " and your Remit Token Request Code is " + token +
                                                " and secretCode is " + code + " Please collect at any of our agents." + "\n";
                                messagereplyDestMobile += "-NIBL Thaili";

                                if ((da.Substring(0, 3) == "980") || (ftremit.da.Substring(0, 3) == "981")) //FOR NCELL
                                {
                                    //FOR NCELL
                                    //var content = client.DownloadString(
                                    //    "http://smsvas.mos.com.np/PostSMS.ashx?QueueId=&TelecomId=2&KeywordId=3&Password=mnepal120&From=37878&To="
                                    //    + "977" + da + "&message=" + messagereplyDestMobile + "");
                                    var content = client.DownloadString(
                                        SMSNCELL
                                        + "977" + da + "&message=" + messagereplyDestMobile + "");
                                }
                                else if ((ftremit.da.Substring(0, 3) == "985") || (ftremit.da.Substring(0, 3) == "984")
                                         || (da.Substring(0, 3) == "986"))
                                {
                                    //FOR NTC
                                    //var content = client.DownloadString(
                                    //    "http://smsvas.mos.com.np/PostSMS.ashx?QueueId=&TelecomId=1&KeywordId=3&Password=mnepal120&From=37878&To="
                                    //    + "977" + da + "&message=" + messagereplyDestMobile + "");
                                    var content = client.DownloadString(
                                        SMSNTC
                                        + "977" + da + "&message=" + messagereplyDestMobile + "");
                                }
                            }
                            
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("" + ex);
                        }

                    }
                    else if ((response.StatusCode == HttpStatusCode.BadRequest) || (response.StatusCode == HttpStatusCode.InternalServerError)) 
                    {
                        var v = new
                        {
                            StatusCode = Convert.ToInt32(statusCode),
                            StatusMessage = message
                        };
                        result = JsonConvert.SerializeObject(v);
                    }

                }
                else
                {
                    replyMessage.Response = failedmessage;
                    replyMessage.ResponseStatus(HttpStatusCode.BadRequest, failedmessage);
                    result = replyMessage.Response;
                    statusCode = "400";
                    message = replyMessage.Response;
                    failedmessage = message;
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
            return result;

        }


        /// <summary>
        /// Token Redeem
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public string redeem(Stream input)
        {
            /*
                tid - trace ID (different from above)
                sc - service code
                mobile - agent's mobile number
                photoId - receiver's id
                tokenId - token
                code - secret code
                
                agentPin - (comes from app)
                src - channel - sms, http etc.
             
             */
            StreamReader sr = new StreamReader(input);
            string s = sr.ReadToEnd();
            sr.Dispose();

            NameValueCollection qs = HttpUtility.ParseQueryString(s);
            string tid = qs["tid"];
            string sc = qs["sc"];
            string mobile = qs["agentmobile"];    //agent mobile destination mobile
            string photoId = "PHO_ID:" + qs["photoId"];
            string tokenId = qs["tokenId"];
            string code = qs["code"];
            string agentPin = qs["c"];
            string src = qs["src"];

            string remitCustSender = qs["remitCustSender"];
            string remitReceiver = qs["remitReceiver"];
            string amountReceived = qs["amount"];

            //SMS
            string SMSNTC = System.Web.Configuration.WebConfigurationManager.AppSettings["MNepalNTCSMSServerUrl"];
            string SMSNCELL = System.Web.Configuration.WebConfigurationManager.AppSettings["MNepalSMSServerUrl"];

            string destinationMobile = remitReceiver;

            string result = "";
            ReplyMessage replyMessage = new ReplyMessage();

            FundTransferRemit ftremit = new FundTransferRemit
            {
                tid = tid,
                sc = sc,  //41
                secretCode = code, //place secret code in source mobile
                tokenId = tokenId, //place in source account
                da = mobile, //agent mobile
                photoId = photoId,
                pin = agentPin,
                note = code,
                sourcechannel = src
            };
            //ftremit.amount = "1000";

            //SC: Remit Token Redeem: 41 - Wallet
            string totalAmount = string.Empty;
            string totalCount = string.Empty;
            string balance = string.Empty;
            string statusCode = string.Empty;
            string message = string.Empty;
            string failedmessage = string.Empty;

            //string wbalnLimit = "5000";

            if ((tid == null) || (sc == null) || (mobile == null) || (tokenId == null) || (code == null) || (agentPin == null) ||
                (src == null))
            {
                statusCode = "400";
                message = "Parameters Missing/Invalid";
                replyMessage.ResponseStatus(HttpStatusCode.BadRequest, message);
                failedmessage = message;
            }
            else
            {
                MNTransactionMaster validTransactionData = new MNTransactionMaster();

                TransLimitCheck transLimitCheck = new TransLimitCheck();
                string resultTranLimit = transLimitCheck.LimitCheck(mobile, "",amountReceived, sc, ftremit.pin, src);

                var jsonDataResult = JObject.Parse(resultTranLimit);
                statusCode = jsonDataResult["StatusCode"].ToString();
                string statusMsg = jsonDataResult["StatusMessage"].ToString();
                message = jsonDataResult["StatusMessage"].ToString();
                failedmessage = message;

                if ((statusCode == "200") && (message == "Success"))
                {

                    //string tid, string sc, string mobile, string sa, string amount, string da, string note, string pin, string sourcechannel
                    /*
                     tid = tid;
                     sc = sc;
                     source mobile = photoId;
                     sa = tokenId;
                     amount = null;
                     da = agent mobile;
                     note = secret code;
                     pin = agent pin;
                     source channel = src;
                     */

                    MNFundTransfer mnft = new MNFundTransfer(ftremit.tid, ftremit.sc, ftremit.photoId, ftremit.tokenId,
                        ftremit.amount, ftremit.da, ftremit.note, ftremit.pin, ftremit.sourcechannel);
                    var comfocuslog = new MNComAndFocusOneLog(mnft, DateTime.Now);
                    var mncomfocuslog = new MNComAndFocusOneLogsController();
                    string reply = mncomfocuslog.InsertIntoComFocusOne(comfocuslog);
                    

                    if (reply == "Success")
                    {
                        //NOTE:- may be need to validate before insert into reply typpe
                        //start:insert into reply type as HTTP//
                        var replyType = new MNReplyType(ftremit.tid, "HTTP");
                        var mnreplyType = new MNReplyTypesController();
                        mnreplyType.InsertIntoReplyType(replyType);
                        //end:insert into reply type as HTTP//

                        //start:insert into transaction master//
                        if (mnft.valid())
                        {
                            string GetToken = ftremit.tokenId;
                            string GetSecretCode = ftremit.note;
                            string FeatureCode = ftremit.sc;
                            string SourceMobile = remitCustSender;//"";
                            string Amount = "";
                            string DestinyMobile = remitReceiver;

                            if (GetToken != "" && GetSecretCode != "")
                            {
                                var dataContext = new PetaPoco.Database(MNepalDBConnectionStringProvider.GetConnection());
                                var GetSourceDestData = dataContext.SingleOrDefault<MNTransactionMaster>(
                                    "select TOP 1 * from MNTransactionMaster where SourceMobile=@0 and DestinationMobile=@1 and FeatureCode=40",
                                    SourceMobile, destinationMobile); //
                                try
                                {
                                    if (GetSourceDestData != null)
                                    {
                                        var GetData = dataContext.SingleOrDefault<MNTransactionMaster>(
                                        "SELECT * FROM MNTransactionMaster (NOLOCK) WHERE DestinationAccount=@0 and Description=@1 and FeatureCode=40 and SourceMobile=@2 and DestinationMobile=@3",
                                        GetToken, GetSecretCode, SourceMobile, destinationMobile);

                                        try
                                        {
                                            if (GetData != null)
                                            {
                                                SourceMobile = GetData.SourceMobile;
                                                destinationMobile = GetData.DestinationMobile;
                                                Amount = GetData.Amount.ToString();
                                                if (remitReceiver == destinationMobile)
                                                {
                                                    if (amountReceived == Amount)
                                                    {
                                                        //string note = ftremit.secretCode + "," + ftremit.tokenId + "," + ftremit.photoId + "," + destinationMobile;
                                                        string note = ftremit.secretCode + "," + ftremit.tokenId + "," + ftremit.photoId +
                                                                        "," + destinationMobile;

                                                        //Check Agent is Authenticated or not
                                                        MNClientContact agentFulldetails = new MNClientContact();
                                                        agentFulldetails.ContactNumber1 = ftremit.da;
                                                        ClientsDetails agentdetails = new ClientsDetails(agentFulldetails);

                                                        if (agentdetails.client != null && agentdetails.clientContact != null)
                                                        {
                                                            if (ftremit.pin == agentdetails.clientExt.PIN)
                                                            {
                                                                //For Token Redeem Source User PIN is required and it is received from IVR call to Source Mobile for now keep Source User PIN static

                                                                //ftremit.pin = "1234";
                                                                MNFundTransfer mnftt = new MNFundTransfer(ftremit.tid,
                                                                    ftremit.sc,
                                                                    mobile,
                                                                    ftremit.sa,
                                                                    Amount,
                                                                    remitReceiver,
                                                                    note,
                                                                    ftremit.pin,
                                                                    ftremit.sourcechannel);//SourceMobile,ftremit.da,


                                                                var transaction = new MNTransactionMaster(mnftt);
                                                                var mntransaction = new MNTransactionsController();
                                                                //MNTransactionMaster 
                                                                validTransactionData =
                                                                    mntransaction.Validate(transaction, mnftt.pin);
                                                                result = validTransactionData.Response;

                                                                ErrorMessage em = new ErrorMessage();
                                                                if ((result == "Trace ID Repeated") || (result == "Limit Exceed") || (result == "Invalid PIN")
                                                                    || (result == "Invalid Source User") || (result == "Invalid Destination User")
                                                                    || (result == "Invalid Product Request") || (result == "") || (result == "Error") )
                                                                {
                                                                    statusCode = "400";
                                                                    mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
                                                                    message = result;
                                                                    failedmessage = result;
                                                                }
                                                                if (result.Substring(0, 5) == "Error")
                                                                {
                                                                    statusCode = "400";
                                                                    message = "Connection Failure from Gateway. Please Contact your Bank." + result;
                                                                    mnft.ResponseStatus(HttpStatusCode.InternalServerError, message);
                                                                    failedmessage = result;
                                                                }
                                                                if (result == "111")
                                                                {
                                                                    statusCode = result;
                                                                    message = em.Error_111/* + " " + result*/;
                                                                    failedmessage = message;
                                                                    mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
                                                                }
                                                                if (result == "114")
                                                                {
                                                                    statusCode = result;
                                                                    message = em.Error_114/* + " " + result*/;
                                                                    failedmessage = message;
                                                                    mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
                                                                }
                                                                if (result == "115")
                                                                {
                                                                    statusCode = result;
                                                                    message = em.Error_115/* + " " + result*/;
                                                                    failedmessage = message;
                                                                    mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
                                                                }
                                                                if (result == "116")
                                                                {
                                                                    statusCode = result;
                                                                    message = em.Error_116/* + " " + result*/;
                                                                    failedmessage = message;
                                                                    mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
                                                                }
                                                                if (result == "119")
                                                                {
                                                                    statusCode = result;
                                                                    message = em.Error_119/* + " " + result*/;
                                                                    failedmessage = message;
                                                                    mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
                                                                }
                                                                if (result == "121")
                                                                {
                                                                    statusCode = result;
                                                                    message = em.Error_121/* + " " + result*/;
                                                                    failedmessage = message;
                                                                    mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
                                                                }
                                                                if (result == "163")
                                                                {
                                                                    statusCode = result;
                                                                    message = em.Error_163/* + " " + result*/;
                                                                    failedmessage = message;
                                                                    mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
                                                                }
                                                                if (result == "180")
                                                                {
                                                                    statusCode = result;
                                                                    message = em.Error_180/* + " " + result*/;
                                                                    failedmessage = message;
                                                                    mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
                                                                }
                                                                if (result == "181")
                                                                {
                                                                    statusCode = result;
                                                                    message = em.Error_181/* + " " + result*/;
                                                                    failedmessage = message;
                                                                    mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
                                                                }
                                                                if (result == "182")
                                                                {
                                                                    statusCode = result;
                                                                    message = em.Error_182/* + " " + result*/;
                                                                    failedmessage = message;
                                                                    mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
                                                                }
                                                                if (result == "183")
                                                                {
                                                                    statusCode = result;
                                                                    message = em.Error_183/* + " " + result*/;
                                                                    failedmessage = message;
                                                                    mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
                                                                }
                                                                if (result == "184")
                                                                {
                                                                    statusCode = result;
                                                                    message = em.Error_184/* + " " + result*/;
                                                                    failedmessage = message;
                                                                    mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
                                                                }
                                                                if (result == "185")
                                                                {
                                                                    statusCode = result;
                                                                    message = em.Error_185/* + " " + result*/;
                                                                    failedmessage = message;
                                                                    mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
                                                                }
                                                                if (result == "186")
                                                                {
                                                                    statusCode = result;
                                                                    message = em.Error_186/* + " " + result*/;
                                                                    failedmessage = message;
                                                                    mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
                                                                }
                                                                if (result == "187")
                                                                {
                                                                    statusCode = result;
                                                                    message = em.Error_187/* + " " + result*/;
                                                                    failedmessage = message;
                                                                    mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
                                                                }
                                                                if (result == "188")
                                                                {
                                                                    statusCode = result;
                                                                    message = em.Error_188/* + " " + result*/;
                                                                    failedmessage = message;
                                                                    mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
                                                                }
                                                                if (result == "189")
                                                                {
                                                                    statusCode = result;
                                                                    message = em.Error_189/* + " " + result*/;
                                                                    failedmessage = message;
                                                                    mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
                                                                }
                                                                if (result == "190")
                                                                {
                                                                    statusCode = result;
                                                                    message = em.Error_190/* + " " + result*/;
                                                                    failedmessage = message;
                                                                    mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
                                                                }
                                                                if (result == "800")
                                                                {
                                                                    statusCode = result;
                                                                    message = em.Error_800/* + " " + result*/;
                                                                    failedmessage = message;
                                                                    mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
                                                                }
                                                                if (result == "902")
                                                                {
                                                                    statusCode = result;
                                                                    message = em.Error_902/* + " " + result*/;
                                                                    failedmessage = message;
                                                                    mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
                                                                }
                                                                if (result == "904")
                                                                {
                                                                    statusCode = result;
                                                                    message = em.Error_904/* + " " + result*/;
                                                                    failedmessage = message;
                                                                    mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
                                                                }
                                                                if (result == "906")
                                                                {
                                                                    statusCode = result;
                                                                    message = em.Error_906/* + " " + result*/;
                                                                    failedmessage = message;
                                                                    mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
                                                                }
                                                                if (result == "907")
                                                                {
                                                                    statusCode = result;
                                                                    message = em.Error_907/* + " " + result*/;
                                                                    failedmessage = message;
                                                                    mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
                                                                }
                                                                if (result == "909")
                                                                {
                                                                    statusCode = result;
                                                                    message = em.Error_909/* + " " + result*/;
                                                                    failedmessage = message;
                                                                    mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
                                                                }
                                                                if (result == "911")
                                                                {
                                                                    statusCode = result;
                                                                    message = em.Error_911/* + " " + result*/;
                                                                    failedmessage = message;
                                                                    mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
                                                                }
                                                                if (result == "913")
                                                                {
                                                                    statusCode = result;
                                                                    message = em.Error_913/* + " " + result*/;
                                                                    failedmessage = message;
                                                                    mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
                                                                }
                                                                if (result == "90")
                                                                {
                                                                    statusCode = result;
                                                                    message = em.Error_90/* + " " + result*/;
                                                                    failedmessage = message;
                                                                    mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
                                                                }
                                                                if (result == "91")
                                                                {
                                                                    statusCode = result;
                                                                    message = em.Error_91/* + " " + result*/;
                                                                    failedmessage = message;
                                                                    mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
                                                                }
                                                                if (result == "92")
                                                                {
                                                                    statusCode = result;
                                                                    message = em.Error_92/* + " " + result*/;
                                                                    failedmessage = message;
                                                                    mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
                                                                }
                                                                if (result == "94")
                                                                {
                                                                    statusCode = result;
                                                                    message = em.Error_94/* + " " + result*/;
                                                                    failedmessage = message;
                                                                    mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
                                                                }
                                                                if (result == "95")
                                                                {
                                                                    statusCode = result;
                                                                    message = em.Error_95/* + " " + result*/;
                                                                    failedmessage = message;
                                                                    mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
                                                                }
                                                                if (result == "98")
                                                                {
                                                                    statusCode = result;
                                                                    message = em.Error_98/* + " " + result*/;
                                                                    failedmessage = message;
                                                                    mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
                                                                }
                                                                if (result == "99")
                                                                {
                                                                    statusCode = result;
                                                                    message = em.Error_99/* + " " + result*/;
                                                                    failedmessage = message;
                                                                    mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
                                                                }
                                                                else
                                                                {
                                                                    mnft.ResponseStatus(HttpStatusCode.OK, result);
                                                                    statusCode = "200";
                                                                    var v = new
                                                                    {
                                                                        StatusCode = Convert.ToInt32(statusCode),
                                                                        StatusMessage = result
                                                                    };
                                                                    result = JsonConvert.SerializeObject(v);
                                                                }
                                                               
                                                            }
                                                            else
                                                            {
                                                                mnft.Response = "Agent PIN mismatch";
                                                                mnft.ResponseStatus(HttpStatusCode.BadRequest, "Agent PIN mismatch");
                                                                statusCode = "400";
                                                                message = mnft.Response;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            mnft.Response = "Invalid Agent";
                                                            mnft.ResponseStatus(HttpStatusCode.BadRequest, "Invalid Agent");
                                                            statusCode = "400";
                                                            message = mnft.Response;
                                                            failedmessage = message;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        mnft.Response = "Invalid Amount";
                                                        mnft.ResponseStatus(HttpStatusCode.BadRequest, "Invalid Amount");
                                                        statusCode = "400";
                                                        message = mnft.Response;
                                                        failedmessage = message;
                                                    }
                                                }
                                                else
                                                {
                                                    mnft.Response = "Invalid Destination Mobile";
                                                    mnft.ResponseStatus(HttpStatusCode.BadRequest, "Invalid Destination Mobile");
                                                    statusCode = "400";
                                                    message = mnft.Response;
                                                    failedmessage = message;
                                                }
                                            }
                                            else
                                            {
                                                mnft.Response = "Invalid Token/Code";
                                                mnft.ResponseStatus(HttpStatusCode.BadRequest, "Invalid Token/Code");
                                                statusCode = "400";
                                                message = mnft.Response;
                                                failedmessage = message;
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            mnft.Response = "Couldnot Retrieve Data" + ex;
                                            mnft.ResponseStatus(HttpStatusCode.BadRequest, "Couldnot Retrieve Data");
                                            statusCode = "400";
                                            message = mnft.Response;
                                            failedmessage = message;
                                        }

                                        
                                        
                                    }
                                    else
                                    {
                                        mnft.Response = "Invalid Remit Sender/Receiver Mobile Number";
                                        mnft.ResponseStatus(HttpStatusCode.BadRequest, "Invalid Remit Sender Mobile Number");
                                        statusCode = "400";
                                        message = mnft.Response;
                                        failedmessage = message;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    mnft.Response = "Couldnot Retrieve Data" + ex;
                                    mnft.ResponseStatus(HttpStatusCode.BadRequest, "Couldnot Retrieve Data");
                                    statusCode = "400";
                                    message = mnft.Response;
                                    failedmessage = message;
                                }
                            }
                            else
                            {
                                mnft.Response = "Token/Code is Empty";
                                mnft.ResponseStatus(HttpStatusCode.BadRequest, "Token/Code is Empty");
                                statusCode = "400";
                                message = mnft.Response;
                                failedmessage = message;
                            }
                        }
                        else
                        {
                            mnft.Response = "error";
                            mnft.ResponseStatus(HttpStatusCode.BadRequest, "parameters missing/invalid"); //200 - OK
                            result = mnft.Response;
                            statusCode = "400";
                            message = "parameters missing/invalid";
                            failedmessage = message;
                        }
                        ////end:insert into transaction master//
                    }
                    else
                    {
                        mnft.Response = "Data Insertion Failed in Log File";
                        mnft.ResponseStatus(HttpStatusCode.BadRequest, "Data Insertion Failed in Log File"); //200 - OK
                        result = mnft.Response;
                        statusCode = "400";
                        message = mnft.Response;
                    }

                }
                else
                {
                    statusCode = "400";
                    message = failedmessage;
                    replyMessage.ResponseStatus(HttpStatusCode.BadRequest, message);
                    failedmessage = message;
                }

                OutgoingWebResponseContext response = WebOperationContext.Current.OutgoingResponse;

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    try
                    {
                        var jsonData = JObject.Parse(result);
                        var amtTransfer = jsonData["AmounttransferredBalance"];

                        //Agent Mobile
                        string agentMobile = mobile;
                        string messagereply = "Dear Agent," + "\n";
                        messagereply += agentMobile + " was successfully redeem from " + remitCustSender +
                            " with amount NPR" + validTransactionData.Amount //amtTransfer
                                        + "." + "\n"; //validTransactionData.CreatedDate destinationMobile
                        messagereply += "Thank you. NIBL Thaili";

                        var client = new WebClient();

                        if ((mobile.Substring(0, 3) == "980") || (mobile.Substring(0, 3) == "981")) //FOR NCELL
                        {
                            //FOR NCELL
                            //var content = client.DownloadString(
                            //    "http://smsvas.mos.com.np/PostSMS.ashx?QueueId=&TelecomId=2&KeywordId=3&Password=mnepal120&From=37878&To="
                            //    + "977" + mobile + "&message=" + messagereply + "");
                            var content = client.DownloadString(
                                SMSNCELL
                                + "977" + mobile + "&message=" + messagereply + "");
                        }
                        else if ((mobile.Substring(0, 3) == "985") || (mobile.Substring(0, 3) == "984")
                                    || (mobile.Substring(0, 3) == "986"))
                        {
                            //FOR NTC
                            //var content = client.DownloadString(
                            //    "http://smsvas.mos.com.np/PostSMS.ashx?QueueId=&TelecomId=1&KeywordId=3&Password=mnepal120&From=37878&To="
                            //    + "977" + mobile + "&message=" + messagereply + "");
                            var content = client.DownloadString(
                                SMSNTC
                                + "977" + mobile + "&message=" + messagereply + "");
                        }

                        //For Remit Customer Sender
                        string mobileCustomer = remitCustSender;////destinationMobile
                        string messagereplyCust = "Dear Customer," + "\n";
                        messagereplyCust += mobileCustomer + " was successfully redeem with amount NPR" + validTransactionData.Amount //amtTransfer
                                        + "." + "\n"; //validTransactionData.CreatedDate
                        messagereplyCust += "Thank you. NIBL Thaili";


                        if ((mobileCustomer.Substring(0, 3) == "980") || (mobileCustomer.Substring(0, 3) == "981")) //FOR NCELL
                        {
                            //FOR NCELL
                            //var content = client.DownloadString(
                            //    "http://smsvas.mos.com.np/PostSMS.ashx?QueueId=&TelecomId=2&KeywordId=3&Password=mnepal120&From=37878&To="
                            //    + "977" + mobileCustomer + "&message=" + messagereplyCust + "");
                            var content = client.DownloadString(
                                SMSNCELL
                                + "977" + mobileCustomer + "&message=" + messagereplyCust + "");
                        }
                        else if ((mobileCustomer.Substring(0, 3) == "985") || (mobileCustomer.Substring(0, 3) == "984")
                                    || (mobileCustomer.Substring(0, 3) == "986"))
                        {
                            //FOR NTC
                            //var content = client.DownloadString(
                            //    "http://smsvas.mos.com.np/PostSMS.ashx?QueueId=&TelecomId=1&KeywordId=3&Password=mnepal120&From=37878&To="
                            //    + "977" + mobileCustomer + "&message=" + messagereplyCust + "");
                            var content = client.DownloadString(
                                SMSNTC
                                + "977" + mobileCustomer + "&message=" + messagereplyCust + "");
                        }

                        //For Remit Receiver
                        string mobileRemitReceiver = remitReceiver;
                        string messagereplyRemitReceiver = "Dear Customer," + "\n";
                        messagereplyRemitReceiver += mobileRemitReceiver + " successfully received NPR " + validTransactionData.Amount //amtTransfer
                                            + " from Remit Sender " + agentMobile + " through token redeem." + "\n"; //validTransactionData.CreatedDate
                        messagereplyRemitReceiver += "Thank you. NIBL Thaili";


                        if ((mobileRemitReceiver.Substring(0, 3) == "980") || (mobileRemitReceiver.Substring(0, 3) == "981")) //FOR NCELL
                        {
                            //FOR NCELL
                            //var content = client.DownloadString(
                            //    "http://smsvas.mos.com.np/PostSMS.ashx?QueueId=&TelecomId=2&KeywordId=3&Password=mnepal120&From=37878&To="
                            //    + "977" + mobileRemitReceiver + "&message=" + messagereplyRemitReceiver + "");
                            var content = client.DownloadString(
                                SMSNCELL
                                + "977" + mobileRemitReceiver + "&message=" + messagereplyRemitReceiver + "");
                        }
                        else if ((mobileCustomer.Substring(0, 3) == "985") || (mobileCustomer.Substring(0, 3) == "984")
                                    || (mobileCustomer.Substring(0, 3) == "986"))
                        {
                            //FOR NTC
                            //var content = client.DownloadString(
                            //    "http://smsvas.mos.com.np/PostSMS.ashx?QueueId=&TelecomId=1&KeywordId=3&Password=mnepal120&From=37878&To="
                            //    + "977" + mobileRemitReceiver + "&message=" + messagereplyRemitReceiver + "");
                            var content = client.DownloadString(
                                SMSNTC
                                + "977" + mobileRemitReceiver + "&message=" + messagereplyRemitReceiver + "");
                        }

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                else if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    var v = new
                    {
                        StatusCode = Convert.ToInt32(400),
                        StatusMessage = message + result
                    };
                    result = JsonConvert.SerializeObject(v);
                }

            }

            if (statusCode != "200")
            {
                if (message == "")
                {
                    message = result; //"Insufficient Balance";
                }
                var v = new
                {
                    StatusCode = Convert.ToInt32(statusCode),
                    StatusMessage = message
                };
                result = JsonConvert.SerializeObject(v);
            }
            return result;
        }

    }
}
