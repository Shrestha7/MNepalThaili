using MNepalProject.Controllers;
using MNepalProject.Models;
using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.IO;
using System.Collections.Specialized;
using System.Data;
using System.Web;
using MNepalProject.DAL;
using WCF.MNepal.Utilities;
using MNepalProject.Helper;
using Newtonsoft.Json;
using WCF.MNepal.Models;
using WCF.MNepal.Helper;
using Newtonsoft.Json.Linq;
using WCF.MNepal.ErrorMsg;
using System.Threading;
using System.Web.Services.Description;
using MNepalAPI.Helper;
using MNepalProject.Services;

namespace WCF.MNepal
{
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Single, InstanceContextMode = InstanceContextMode.Single)]
    public class nea
    {
        [OperationContract]
        [WebInvoke(Method = "POST",
                  ResponseFormat = WebMessageFormat.Json)]
        public string request(Stream input)
        {
            StreamReader sr = new StreamReader(input);
            string s = sr.ReadToEnd();
            sr.Dispose();
            NameValueCollection qs = HttpUtility.ParseQueryString(s);

            string tid = qs["tid"];

            //string[] querySegments = s.Split('&');
            //string[] querySegments = s.Split(new string[] { "&amp;" }, StringSplitOptions.None);
            //foreach (string segment in querySegments)
            //{
            //    string[] parts = segment.Split('=');
            //    if (parts.Length > 0)
            //    {
            //        string key = parts[0].Trim();//new char[] { '?', ' ' });
            //        string val = parts[1].Trim();

            //        qs.Add(key, val);
            //    }
            //}

            string sc = qs["sc"];
            string mobile = qs["mobile"];
            string da = qs["da"];
            string amount = qs["amount"];
            string pin = qs["pin"];
            pin = HashAlgo.Hash(pin);
            string note = "NEA Bill Payment to " + qs["customerId"];
            string src = qs["src"];
            string result = "";
            string sessionID = qs["tokenID"];
            string destBranchCode = qs["destBranchCode"];
            string scn = qs["scn"];
            string customerId = qs["customerId"];
            string customerName = qs["customerName"];
            string merchantName = qs["merchantName"];
            string merchantType = qs["merchantType"];

            //SMS
            string SMSNTC = System.Web.Configuration.WebConfigurationManager.AppSettings["MNepalNTCSMSServerUrl"];
            string SMSNCELL = System.Web.Configuration.WebConfigurationManager.AppSettings["MNepalSMSServerUrl"];

            string transactionType = string.Empty;
            FundTransfer fundtransfer = new FundTransfer
            {
                tid = tid,
                sc = sc,
                mobile = mobile,
                da = da,
                amount = amount,
                pin = pin,
                note = note,
                sourcechannel = src,
                destBranchCode = destBranchCode,
                scn = scn,
                customerId = customerId,
                customerName = customerName,
                merchantName = merchantName,
                merchantType = merchantType,
            };

            ThreadPool.QueueUserWorkItem(BackgroundTaskWithObject, fundtransfer);
            CustActivityModel custsmsInfo = new CustActivityModel();
            MNTransactionMaster validTransactionData = new MNTransactionMaster();

            string totalAmount = string.Empty;
            string totalCount = string.Empty;
            string totalBAmount = string.Empty;
            string totalBCount = string.Empty;
            string balance = string.Empty;
            string message = string.Empty;
            string statusCode = string.Empty;
            string failedmessage = string.Empty;

            string customerNo = string.Empty;
            if (TokenGenerator.TokenChecker(sessionID, mobile, src) == false)
            {
                // throw ex
                statusCode = "400";
                message = "Session expired. Please login again";
                failedmessage = message;
            }
            else
            {
                if ((tid == null) || (sc == null) || (mobile == null) || (da == null) || (amount == null) || (pin == null) ||
                (src == null) || (double.Parse(amount) <= 0))
                {
                    // throw ex
                    statusCode = "400";
                    message = "Parameters Missing/Invalid";
                    failedmessage = message;
                }
                else
                {

                    //start:Registered Customer Check Mobile
                    if (da != "")
                    {
                        DataTable dtableUserCheck = CustCheckUtils.GetCustUserInfo(da);
                        if (dtableUserCheck.Rows.Count == 0)
                        {
                            customerNo = "0";
                        }
                        else if (dtableUserCheck.Rows.Count > 0)
                        {
                            customerNo = da;
                        }
                    }
                    //end:Registered Customer Check Mobile

                    if (sc == "00")
                    {
                        transactionType = "Fund Transfer to W2W";
                    }
                    else if (sc == "01")
                    {
                        transactionType = "Fund Transfer to W2B";
                    }
                    else if (sc == "10")
                    {
                        transactionType = "Fund Transfer to B2W"; //B2W
                    }
                    else if (sc == "11")
                    {
                        transactionType = "Fund Transfer to Bank";//B2B
                    }

                    Pin p = new Pin();
                    if (!p.validPIN(mobile, pin))
                    {
                        statusCode = "400";
                        message = "Invalid PIN";
                        failedmessage = message;

                        LoginUtils.SetPINTries(mobile, "BUWP");//add +1 in trypwd

                        if (LoginUtils.GetPINBlockTime(mobile)) //check if blocktime is greater than current time 
                        {
                            message = LoginUtils.GetMessage("01");
                            //message = "Invalid PIN! You have already attempt 3 times with wrong PIN,Please try again after 10 minutes";  //change
                            statusCode = "417";
                            MNFundTransfer mnlg = new MNFundTransfer();
                            mnlg.ResponseStatus(HttpStatusCode.ExpectationFailed, message);
                            failedmessage = message;

                        }
                    }

                    else
                    {
                        LoginUtils.SetPINTries(mobile, "RPT");

                    }

                    if ((customerNo != "0") && (message == "") && (statusCode != "400"))
                    { //Merchant check 


                        if (UserNameCheck.IsValidMerchant(da))
                        {
                            if (UserNameCheck.IsValidAgent(da))
                            {

                                // throw ex
                                statusCode = "400";
                                message = "Transaction restricted to Agent";
                                failedmessage = message;
                            }
                            if (UserNameCheck.IsValidUser(da))
                            {
                                // throw ex
                                statusCode = "400";
                                message = "Transaction restricted to User";
                                failedmessage = message;
                            }
                            if (UserNameCheck.IsValidMerchant(da))
                            {


                                TransLimitCheck transLimitCheck = new TransLimitCheck();
                                string resultTranLimit = transLimitCheck.LimitCheck(mobile, da, amount, sc, pin, src);

                                var jsonDataResult = JObject.Parse(resultTranLimit);
                                statusCode = jsonDataResult["StatusCode"].ToString();
                                string statusMsg = jsonDataResult["StatusMessage"].ToString();
                                message = jsonDataResult["StatusMessage"].ToString();
                                failedmessage = message;


                                //start block msg 3 time pin attempt
                                //if (message == "Invalid PIN ")
                                //{
                                //    LoginUtils.SetPINTries(mobile, "BUWP");//add +1 in trypwd

                                //    if (LoginUtils.GetPINBlockTime(mobile)) //check if blocktime is greater than current time 
                                //    {
                                //        message = LoginUtils.GetMessage("01");
                                //        //message = "Invalid PIN! You have already attempt 3 times with wrong PIN,Please try again after 10 minutes";

                                //        statusCode = "417";
                                //        MNFundTransfer mnlg = new MNFundTransfer();
                                //        mnlg.ResponseStatus(HttpStatusCode.ExpectationFailed, message);

                                //        failedmessage = message;

                                //    }


                                //}
                                
                                //end block msg 3 time pin attempt
                                if ((statusCode == "200") && (message == "Success"))
                                {
                                    //start: checking trace id
                                    do
                                    {
                                        TraceIdGenerator traceid = new TraceIdGenerator();
                                        tid = traceid.GenerateUniqueTraceID();
                                        fundtransfer.tid = tid;

                                        bool traceIdCheck = false;
                                        traceIdCheck = TraceIdCheck.IsValidTraceId(fundtransfer.tid);
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

                                    //start:Com focus one log///
                                    MNFundTransfer mnft = new MNFundTransfer(tid, fundtransfer.sc, fundtransfer.mobile,
                                        fundtransfer.sa, fundtransfer.amount, fundtransfer.da, fundtransfer.note, fundtransfer.pin,
                                        fundtransfer.sourcechannel, fundtransfer.destBranchCode, fundtransfer.scn, fundtransfer.customerId,
                                        fundtransfer.customerName, fundtransfer.merchantName, fundtransfer.merchantType = "nea");
                                    var comfocuslog = new MNComAndFocusOneLog(mnft, DateTime.Now);
                                    var mncomfocuslog = new MNComAndFocusOneLogsController();
                                    mncomfocuslog.InsertIntoComFocusOne(comfocuslog);
                                    //end:Com focus one log//


                                    //NOTE:- may be need to validate before insert into reply typpe
                                    //start:insert into reply type as HTTP//
                                    var replyType = new MNReplyType(tid, "HTTP");
                                    var mnreplyType = new MNReplyTypesController();
                                    mnreplyType.InsertIntoReplyType(replyType);
                                    //end:insert into reply type as HTTP//

                                    //MNTransactionMaster validTransactionData = new MNTransactionMaster();

                                    //start:insert into transaction master//
                                    if (mnft.valid())
                                    {
                                        var transaction = new MNTransactionMaster(mnft);
                                        var mntransaction = new MNTransactionsController();
                                        validTransactionData = mntransaction.Validate(transaction, mnft.pin);
                                        result = validTransactionData.Response;
                                        /*** ***/
                                        ErrorMessage em = new ErrorMessage();

                                        if (validTransactionData.Response == "Error")
                                        {
                                            mnft.Response = "error";
                                            mnft.ResponseStatus(HttpStatusCode.InternalServerError,
                                                "Internal server error - try again later, or contact support");
                                            result = mnft.Response;
                                            statusCode = "500";
                                            message = "Internal server error - try again later, or contact support";
                                            failedmessage = message;
                                        }
                                        else
                                        {
                                            ErrorResult er = new ErrorResult();
                                            string strmsg = er.Errorlst(result, pin);

                                            if (strmsg != "")
                                            {
                                                mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
                                            }
                                            else if (validTransactionData.ResponseCode == "OK")
                                            {
                                                statusCode = "200";
                                                message = result;
                                                mnft.ResponseStatus(HttpStatusCode.OK, message);
                                            }

                                        }
                                        /*** ***/

                                        OutgoingWebResponseContext response = WebOperationContext.Current.OutgoingResponse;
                                        if (response.StatusCode == HttpStatusCode.OK)
                                        {

                                            string messagereplyDest = "";
                                            string messagereply = "";

                                            if (sc == "00")
                                            {
                                                transactionType = "W2W";

                                                messagereplyDest = "Dear " + CustCheckUtils.GetName(da) + "," + "\n";
                                                messagereplyDest +=
                                                    "You have received NPR " + //transactiontype + " transaction was successful with amount NPR " +
                                                    validTransactionData.Amount + " in your Wallet from " + mobile + " on date " +
                                                    (validTransactionData.CreatedDate).ToString("dd/MM/yyyy") +
                                                    "." + "\n";
                                                messagereplyDest += "Thank you, NIBL Thaili";
                                            }
                                            else if (sc == "01")
                                            {
                                                transactionType = "W2B";

                                                messagereplyDest = "Dear " + CustCheckUtils.GetName(da) + "," + "\n";
                                                messagereplyDest +=
                                                "Your bank account has been deposited by NPR " + //transactiontype + " transaction was successful with amount NPR " +
                                                validTransactionData.Amount + " from " + mobile + " on date " +
                                                (validTransactionData.CreatedDate).ToString("dd/MM/yyyy") +
                                                "." + "\n";
                                                messagereplyDest += "Thank you, NIBL Thaili ";
                                            }
                                            else if (sc == "10")
                                            {
                                                transactionType = "B2W";

                                                messagereplyDest = "Dear " + CustCheckUtils.GetName(da) + "," + "\n";

                                                if (mobile == da)
                                                {

                                                    messagereplyDest +=
                                                    "You have received NPR " + //transactiontype + " transaction was successful with amount NPR " +
                                                    validTransactionData.Amount + " from Bank A/C to your Wallet on date " +
                                                    (validTransactionData.CreatedDate).ToString("dd/MM/yyyy") +
                                                    "." + "\n";
                                                    messagereplyDest += "Thank you, NIBL Thaili";

                                                }
                                                else
                                                {
                                                    messagereplyDest +=
                                                        "You have received NPR " + //transactiontype + " transaction was successful with amount NPR " +
                                                        validTransactionData.Amount + " from " + mobile + " to your Wallet on date " +
                                                        (validTransactionData.CreatedDate).ToString("dd/MM/yyyy") +
                                                        "." + "\n";
                                                    messagereplyDest += "Thank you, NIBL Thaili";
                                                }
                                            }
                                            else if (sc == "11")
                                            {
                                                transactionType = "B2B";

                                                messagereplyDest = "Dear " + CustCheckUtils.GetName(da) + "," + "\n";
                                                messagereplyDest +=
                                                "Your bank account has been deposited by NPR " + //transactiontype + " transaction was successful with amount NPR " +
                                                validTransactionData.Amount + " from " + mobile +
                                                " on date " + (validTransactionData.CreatedDate).ToString("dd/MM/yyyy") +
                                                "." + "\n";
                                                messagereplyDest += "Thank you, NIBL Thaili";
                                            }

                                            try
                                            {
                                                //Alert Dynamic
                                                string AlertType = "NEA";
                                                //FOR CUSTOMER SMS                                     
                                                #region FOR CUSTOMER SMS

                                                CustomerSMS customerSMS = new CustomerSMS();
                                                string cSMS = customerSMS.CustSMSEnable(AlertType, mobile, "", validTransactionData.Amount.ToString(), "", "", validTransactionData.CreatedDate.ToString());
                                                if (cSMS == "false")
                                                {

                                                }
                                                else
                                                {

                                                }

                                                #endregion



                                                //messagereply = "Dear " + CustCheckUtils.GetName(mobile) + "," + "\n";
                                                //messagereply += "NEA Bill payment was successful with amount NPR" +
                                                //                validTransactionData.Amount + " on date " + validTransactionData.CreatedDate +
                                                //                "." + "\n";

                                                if (sc == "00") //W2W
                                                {
                                                    messagereply += "You have successfully transferred NPR " + //send
                                                                validTransactionData.Amount + " to " + da + " on date " +
                                                                (validTransactionData.CreatedDate).ToString("dd/MM/yyyy") +
                                                                "." + "\n";

                                                }
                                                else if (sc == "01") //W2B
                                                {
                                                    messagereply += "You have successfully transferred NPR " + //send
                                                                validTransactionData.Amount + " to " + da + " bank account on date " +
                                                                (validTransactionData.CreatedDate).ToString("dd/MM/yyyy") +
                                                                "." + "\n";
                                                }
                                                else if (sc == "10") //B2W
                                                {
                                                    messagereply += "You have successfully transferred NPR " + //send
                                                                validTransactionData.Amount + " from your bank account to "
                                                                + da + " on date " +
                                                                (validTransactionData.CreatedDate).ToString("dd/MM/yyyy") +
                                                                "." + "\n";
                                                }
                                                else if (sc == "11") //B2B
                                                {

                                                    messagereply += "You have successfully transferred NPR " + //send
                                                                    validTransactionData.Amount + " from your bank account to "
                                                                    + da + " bank account on date " +
                                                                    (validTransactionData.CreatedDate).ToString("dd/MM/yyyy") +
                                                                    "." + "\n";
                                                }
                                                
                                                statusCode = "200";
                                                var v = new
                                                {
                                                    StatusCode = Convert.ToInt32(statusCode),
                                                    StatusMessage = result
                                                };
                                                result = JsonConvert.SerializeObject(v);
                                            }
                                            catch (Exception ex)
                                            {
                                                // throw ex
                                                statusCode = "400";
                                                message = ex.Message;
                                            }

                                            custsmsInfo = new CustActivityModel()
                                            {
                                                UserName = fundtransfer.mobile,
                                                RequestMerchant = transactionType,
                                                DestinationNo = fundtransfer.da,
                                                Amount = validTransactionData.Amount.ToString(),
                                                SMSStatus = "Success",
                                                SMSSenderReply = messagereply,
                                                ErrorMessage = "",
                                            };


                                        }
                                        else if ((response.StatusCode == HttpStatusCode.BadRequest) || (response.StatusCode == HttpStatusCode.InternalServerError) || (statusCode != "200"))
                                        {
                                            custsmsInfo = new CustActivityModel()
                                            {
                                                UserName = mobile,
                                                RequestMerchant = transactionType,
                                                DestinationNo = fundtransfer.da,
                                                Amount = validTransactionData.Amount.ToString(),
                                                SMSStatus = "Failed",
                                                SMSSenderReply = message,
                                                ErrorMessage = failedmessage,
                                            };

                                        }
                                        //end:insert into transaction master//

                                    }
                                    else
                                    {
                                        mnft.Response = "error";
                                        mnft.ResponseStatus(HttpStatusCode.BadRequest, "parameters missing/invalid"); //200 - OK
                                        result = mnft.Response;
                                        statusCode = "400";
                                        message = "parameters missing/invalid";
                                        failedmessage = message;

                                        custsmsInfo = new CustActivityModel()
                                        {
                                            UserName = mobile,
                                            RequestMerchant = transactionType,
                                            DestinationNo = fundtransfer.da,
                                            Amount = amount,
                                            SMSStatus = "Failed",
                                            SMSSenderReply = message,
                                            ErrorMessage = failedmessage,
                                        };
                                    }
                                }
                                else
                                {
                                    custsmsInfo = new CustActivityModel()
                                    {
                                        UserName = mobile,
                                        RequestMerchant = transactionType,
                                        DestinationNo = fundtransfer.da,
                                        Amount = amount,
                                        SMSStatus = "Failed",
                                        SMSSenderReply = message,
                                        ErrorMessage = failedmessage,
                                    };
                                }
                            }
                        }

                        else
                        {

                            if (checkMerchantDestinationUsertype(da))
                            {
                                // throw ex
                                statusCode = "400";
                                message = "Transaction restricted to Merchant";
                                failedmessage = message;
                            }
                            else
                            {//Agent check 
                                if (!checkSourceAndDestinationUsertype(mobile, da))
                                {
                                    // throw ex
                                    statusCode = "400";
                                    message = "Transaction restricted to Agent";
                                    failedmessage = message;
                                }
                                else
                                {
                                    ///
                                    TransLimitCheck transLimitCheck = new TransLimitCheck();
                                    string resultTranLimit = transLimitCheck.LimitCheck(mobile, da, amount, sc, pin, src);

                                    var jsonDataResult = JObject.Parse(resultTranLimit);
                                    statusCode = jsonDataResult["StatusCode"].ToString();
                                    string statusMsg = jsonDataResult["StatusMessage"].ToString();
                                    message = jsonDataResult["StatusMessage"].ToString();
                                    failedmessage = message;

                                    //start block msg 3 time pin attempt
                                    if (message == "Invalid PIN ")
                                    {
                                        LoginUtils.SetPINTries(mobile, "BUWP");//add +1 in trypwd

                                        if (LoginUtils.GetPINBlockTime(mobile)) //check if blocktime is greater than current time 
                                        {
                                            message = LoginUtils.GetMessage("01");
                                            //message = "Invalid PIN! You have already attempt 3 times with wrong PIN,Please try again after 10 minutes";

                                            statusCode = "417";
                                            MNFundTransfer mnlg = new MNFundTransfer();
                                            mnlg.ResponseStatus(HttpStatusCode.ExpectationFailed, message);

                                            failedmessage = message;

                                        }

                                    }
                                    else
                                    {
                                        LoginUtils.SetPINTries(mobile, "RPT");

                                    }
                                    //end block msg 3 time pin attempt
                                    ///for bank link check only for 01 and 11                                     


                                    if ((sc == "01") || (sc == "11"))
                                    {
                                        //Check Link Bank Account
                                        DataTable dtableUserCheckLinkBankAcc = CheckerUtils.CheckLinkBankAcc(da);
                                        if (dtableUserCheckLinkBankAcc != null && dtableUserCheckLinkBankAcc.Rows.Count > 0)
                                        {
                                            string BankLink = dtableUserCheckLinkBankAcc.Rows[0]["HasBankKYC"].ToString();

                                            if (BankLink == "F")
                                            {
                                                // throw ex
                                                statusCode = "400";
                                                message = "Bank not linked on Recipient Mobile No";
                                                failedmessage = message;
                                            }

                                        }
                                    }


                                    if ((statusCode == "200") && (message == "Success"))
                                    {
                                        //start: checking trace id
                                        do
                                        {
                                            TraceIdGenerator traceid = new TraceIdGenerator();
                                            tid = traceid.GenerateUniqueTraceID();
                                            fundtransfer.tid = tid;

                                            bool traceIdCheck = false;
                                            traceIdCheck = TraceIdCheck.IsValidTraceId(fundtransfer.tid);
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

                                        //start:Com focus one log///
                                        MNFundTransfer mnft = new MNFundTransfer(tid, fundtransfer.sc, fundtransfer.mobile,
                                            fundtransfer.sa, fundtransfer.amount, fundtransfer.da, fundtransfer.note, fundtransfer.pin,
                                            fundtransfer.sourcechannel);
                                        var comfocuslog = new MNComAndFocusOneLog(mnft, DateTime.Now);
                                        var mncomfocuslog = new MNComAndFocusOneLogsController();
                                        mncomfocuslog.InsertIntoComFocusOne(comfocuslog);
                                        //end:Com focus one log//


                                        //NOTE:- may be need to validate before insert into reply typpe
                                        //start:insert into reply type as HTTP//
                                        var replyType = new MNReplyType(tid, "HTTP");
                                        var mnreplyType = new MNReplyTypesController();
                                        mnreplyType.InsertIntoReplyType(replyType);
                                        //end:insert into reply type as HTTP//

                                        //MNTransactionMaster validTransactionData = new MNTransactionMaster();

                                        //start:insert into transaction master//
                                        if (mnft.valid())
                                        {
                                            var transaction = new MNTransactionMaster(mnft);
                                            var mntransaction = new MNTransactionsController();
                                            validTransactionData = mntransaction.Validate(transaction, mnft.pin);
                                            result = validTransactionData.Response;
                                            /*** ***/
                                            ErrorMessage em = new ErrorMessage();

                                            if (validTransactionData.Response == "Error")
                                            {
                                                mnft.Response = "error";
                                                mnft.ResponseStatus(HttpStatusCode.InternalServerError,
                                                    "Internal server error - try again later, or contact support");
                                                result = mnft.Response;
                                                statusCode = "500";
                                                message = "Internal server error - try again later, or contact support";
                                                failedmessage = message;
                                            }
                                            else
                                            {
                                                ErrorResult er = new ErrorResult();
                                                string strmsg = er.Errorlst(result, pin);

                                                if (strmsg != "")
                                                {
                                                    mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
                                                }
                                                else if (validTransactionData.ResponseCode == "OK")
                                                {
                                                    statusCode = "200";
                                                    message = result;
                                                    mnft.ResponseStatus(HttpStatusCode.OK, message);
                                                }

                                            }
                                            /*** ***/

                                            OutgoingWebResponseContext response = WebOperationContext.Current.OutgoingResponse;
                                            if (response.StatusCode == HttpStatusCode.OK)
                                            {

                                                string messagereplyDest = "";
                                                string messagereply = "";

                                                if (sc == "00")
                                                {
                                                    transactionType = "W2W";

                                                    messagereplyDest = "Dear " + CustCheckUtils.GetName(da) + "," + "\n";
                                                    messagereplyDest +=
                                                        "You have received NPR " + //transactiontype + " transaction was successful with amount NPR " +
                                                        validTransactionData.Amount + " in your Wallet from " + mobile + " on date " +
                                                        (validTransactionData.CreatedDate).ToString("dd/MM/yyyy") +
                                                        "." + "\n";
                                                    messagereplyDest += "Thank you, NIBL Thaili";
                                                }
                                                else if (sc == "01")
                                                {
                                                    transactionType = "W2B";

                                                    messagereplyDest = "Dear " + CustCheckUtils.GetName(da) + "," + "\n";
                                                    messagereplyDest +=
                                                    "Your bank account has been deposited by NPR " + //transactiontype + " transaction was successful with amount NPR " +
                                                    validTransactionData.Amount + " from " + mobile + " on date " +
                                                    (validTransactionData.CreatedDate).ToString("dd/MM/yyyy") +
                                                    "." + "\n";
                                                    messagereplyDest += "Thank you, NIBL Thaili";
                                                }
                                                else if (sc == "10")
                                                {
                                                    transactionType = "B2W";

                                                    messagereplyDest = "Dear " + CustCheckUtils.GetName(da) + "," + "\n";

                                                    if (mobile == da)
                                                    {

                                                        messagereplyDest +=
                                                        "You have received NPR " + //transactiontype + " transaction was successful with amount NPR " +
                                                        validTransactionData.Amount + " from Bank A/C to your Wallet on date " +
                                                        (validTransactionData.CreatedDate).ToString("dd/MM/yyyy") +
                                                        "." + "\n";
                                                        messagereplyDest += "Thank you, NIBL Thaili";

                                                    }
                                                    else
                                                    {
                                                        messagereplyDest +=
                                                            "You have received NPR " + //transactiontype + " transaction was successful with amount NPR " +
                                                            validTransactionData.Amount + " from " + mobile + " to your Wallet on date " +
                                                            (validTransactionData.CreatedDate).ToString("dd/MM/yyyy") +
                                                            "." + "\n";
                                                        messagereplyDest += "Thank you, NIBL Thaili";
                                                    }
                                                }
                                                else if (sc == "11")
                                                {
                                                    transactionType = "B2B";

                                                    messagereplyDest = "Dear " + CustCheckUtils.GetName(da) + "," + "\n";
                                                    messagereplyDest +=
                                                    "Your bank account has been deposited by NPR " + //transactiontype + " transaction was successful with amount NPR " +
                                                    validTransactionData.Amount + " from " + mobile +
                                                    " on date " + (validTransactionData.CreatedDate).ToString("dd/MM/yyyy") +
                                                    "." + "\n";
                                                    messagereplyDest += "Thank you, NIBL Thaili";
                                                }

                                                try
                                                {
                                                    messagereply = "Dear " + CustCheckUtils.GetName(mobile) + "," + "\n";
                                                    //messagereply += transactiontype + " transaction was successful with amount NPR" +
                                                    //                validTransactionData.Amount + " on date " + validTransactionData.CreatedDate +
                                                    //                "." + "\n";
                                                    //messagereply += "You have send NPR " +
                                                    //                validTransactionData.Amount + " on date " +
                                                    //                validTransactionData.CreatedDate +
                                                    //                "." + "\n";
                                                    //messagereply += "Thank you, MNepal";

                                                    if (sc == "00") //W2W
                                                    {
                                                        messagereply += "You have successfully transferred NPR " + //send
                                                                    validTransactionData.Amount + " to " + da + " on date " +
                                                                    (validTransactionData.CreatedDate).ToString("dd/MM/yyyy") +
                                                                    "." + "\n";

                                                    }
                                                    else if (sc == "01") //W2B
                                                    {
                                                        messagereply += "You have successfully transferred NPR " + //send
                                                                    validTransactionData.Amount + " to " + da + " bank account on date " +
                                                                    (validTransactionData.CreatedDate).ToString("dd/MM/yyyy") +
                                                                    "." + "\n";
                                                    }
                                                    else if (sc == "10") //B2W
                                                    {
                                                        messagereply += "You have successfully transferred NPR " + //send
                                                                    validTransactionData.Amount + " from your bank account to "
                                                                    + da + " on date " +
                                                                    (validTransactionData.CreatedDate).ToString("dd/MM/yyyy") +
                                                                    "." + "\n";
                                                    }
                                                    else if (sc == "11") //B2B
                                                    {

                                                        messagereply += "You have successfully transferred NPR " + //send
                                                                        validTransactionData.Amount + " from your bank account to "
                                                                        + da + " bank account on date " +
                                                                        (validTransactionData.CreatedDate).ToString("dd/MM/yyyy") +
                                                                        "." + "\n";
                                                    }

                                                    messagereply += "Thank you, NIBL Thaili";

                                                    var client = new WebClient();
                                                    //var content = client.DownloadString("http://smsvas.mos.com.np/PostSMS.ashx?QueueId=&TelecomId=1&KeywordId=2&Password=test12test&From=9797&To=" + "977" + mobile + "&message=" + messagereply + "");
                                                    //SENDER
                                                    if ((mobile.Substring(0, 3) == "980") || (mobile.Substring(0, 3) == "981")) //FOR NCELL
                                                    {
                                                        //FOR NCELL
                                                        //var content = client.DownloadString(
                                                        //    "http://smsvas.mos.com.np/PostSMS.ashx?QueueId=&TelecomId=2&KeywordId=3&Password=mnepal120&From=37878&To="
                                                        //    + "977" + mobile + "&message=" + messagereply + "");
                                                        var content = client.DownloadString(
                                                                        SMSNCELL
                                                                        + "977" + mobile + "&message=" + messagereply + "");//&message = &message
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


                                                    //FOR DESTIONATION NUMBER RECEIVER
                                                    mobile = da;
                                                    if ((da.Substring(0, 3) == "980") || (da.Substring(0, 3) == "981")) //FOR NCELL
                                                    {
                                                        //FOR NCELL
                                                        //var content = client.DownloadString(
                                                        //    "http://smsvas.mos.com.np/PostSMS.ashx?QueueId=&TelecomId=2&KeywordId=3&Password=mnepal120&From=37878&To="
                                                        //    + "977" + da + "&message=" + messagereplyDest + "");
                                                        var content = client.DownloadString(
                                                                        SMSNCELL
                                                                        + "977" + da + "&message=" + messagereplyDest + "");
                                                    }
                                                    else if ((da.Substring(0, 3) == "985") || (da.Substring(0, 3) == "984")
                                                                || (da.Substring(0, 3) == "986"))
                                                    {
                                                        //FOR NTC
                                                        //var content = client.DownloadString(
                                                        //    "http://smsvas.mos.com.np/PostSMS.ashx?QueueId=&TelecomId=1&KeywordId=3&Password=mnepal120&From=37878&To="
                                                        //    + "977" + da + "&message=" + messagereplyDest + "");
                                                        var content = client.DownloadString(
                                                                        SMSNTC
                                                                        + "977" + da + "&message=" + messagereplyDest + "");
                                                    }

                                                    statusCode = "200";
                                                    var v = new
                                                    {
                                                        StatusCode = Convert.ToInt32(statusCode),
                                                        StatusMessage = result
                                                    };
                                                    result = JsonConvert.SerializeObject(v);



                                                }
                                                catch (Exception ex)
                                                {
                                                    // throw ex
                                                    statusCode = "400";
                                                    message = ex.Message;
                                                }


                                                custsmsInfo = new CustActivityModel()
                                                {
                                                    UserName = fundtransfer.mobile,
                                                    RequestMerchant = transactionType,
                                                    DestinationNo = fundtransfer.da,
                                                    Amount = validTransactionData.Amount.ToString(),
                                                    SMSStatus = "Success",
                                                    SMSSenderReply = messagereply,
                                                    ErrorMessage = "",
                                                };


                                            }
                                            else if ((response.StatusCode == HttpStatusCode.BadRequest) || (response.StatusCode == HttpStatusCode.InternalServerError) || (statusCode != "200"))
                                            {
                                                custsmsInfo = new CustActivityModel()
                                                {
                                                    UserName = mobile,
                                                    RequestMerchant = transactionType,
                                                    DestinationNo = fundtransfer.da,
                                                    Amount = validTransactionData.Amount.ToString(),
                                                    SMSStatus = "Failed",
                                                    SMSSenderReply = message,
                                                    ErrorMessage = failedmessage,
                                                };

                                            }
                                            //end:insert into transaction master//

                                        }
                                        else
                                        {
                                            mnft.Response = "error";
                                            mnft.ResponseStatus(HttpStatusCode.BadRequest, "parameters missing/invalid"); //200 - OK
                                            result = mnft.Response;
                                            statusCode = "400";
                                            message = "parameters missing/invalid";
                                            failedmessage = message;

                                            custsmsInfo = new CustActivityModel()
                                            {
                                                UserName = mobile,
                                                RequestMerchant = transactionType,
                                                DestinationNo = fundtransfer.da,
                                                Amount = amount,
                                                SMSStatus = "Failed",
                                                SMSSenderReply = message,
                                                ErrorMessage = failedmessage,
                                            };
                                        }



                                    }
                                    else
                                    {
                                        custsmsInfo = new CustActivityModel()
                                        {
                                            UserName = mobile,
                                            RequestMerchant = transactionType,
                                            DestinationNo = fundtransfer.da,
                                            Amount = amount,
                                            SMSStatus = "Failed",
                                            SMSSenderReply = message,
                                            ErrorMessage = failedmessage,
                                        };


                                    }

                                }
                            }
                        }
                    }
                    else
                    {
                        //failedmessage = "Destination mobile Not Registered";
                        custsmsInfo = new CustActivityModel()
                        {
                            UserName = mobile,
                            RequestMerchant = transactionType,
                            DestinationNo = fundtransfer.da,
                            Amount = amount,
                            SMSStatus = "Failed",
                            SMSSenderReply = message,
                            ErrorMessage = failedmessage,
                        };

                        var v = new
                        {
                            StatusCode = statusCode,
                            StatusMessage = failedmessage
                        };
                        result = JsonConvert.SerializeObject(v);
                    }

                }

            }

            //Register For SMS
            try
            {
                int results = CustActivityUtils.RegisterCustActivityInfo(custsmsInfo);
                if (results > 0)
                {

                    message = result;
                }
                else
                {
                    message = result;
                }

            }
            catch (Exception ex)
            {
                string ss = ex.Message;
                message = result;
            }

            /*END SMS Register*/

            if (statusCode == "")
            {
                result = result.ToString();
            }
            else if (statusCode != "200")
            {
                if (message == "")
                {
                    if (string.IsNullOrEmpty(result))
                    {
                        result = failedmessage; // "Could not process your request,Please Try again.";
                    }
                    message = result;
                }
                var v = new
                {
                    StatusCode = Convert.ToInt32(statusCode),
                    StatusMessage = failedmessage
                };
                result = JsonConvert.SerializeObject(v);
            }
            return result;
        }
        private string InsertIntoReplyType(string tid, string type)
        {
            string result = type;
            string conStr = System.Configuration.ConfigurationManager.ConnectionStrings[MNepalDBConnectionStringProvider.GetConnection()].ConnectionString;
            string sql = "Insert into MNReplyType values ('" + type + "','" + tid + "')";
            try
            {
                using (System.Data.SqlClient.SqlConnection con = new System.Data.SqlClient.SqlConnection(conStr))
                {
                    con.Open();
                    using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(sql, con))
                        command.ExecuteNonQuery();
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                result = ex.ToString();
            }
            return type;
        }
        static void BackgroundTaskWithObject(Object stateInfo)
        {
            FundTransfer data = (FundTransfer)stateInfo;
            Console.WriteLine($"Hi {data.tid} from ThreadPool.");
            Thread.Sleep(1000);
        }

        #region Checking SOurce and Destination Numbers
        public bool checkSourceAndDestinationUsertype(string source, string destination)
        {

            if (UserNameCheck.IsValidUserNameForgotPassword(source) && UserNameCheck.IsValidUserNameForgotPassword(destination))
            {
                return true;
            }
            if (UserNameCheck.IsValidAgent(source) && UserNameCheck.IsValidUserName(destination))
            {
                return true;
            }
            return false;
        }

        public bool checkMerchantDestinationUsertype(string destination)
        {
            if (CustCheckUtils.GetMerchantUserCheckInfo(destination))
            {
                return true;
            }
            return false;
        }
        #endregion


    }
}
