using MNepalAPI.BasicAuthentication;
using MNepalAPI.Helper;
using MNepalAPI.Models;
using MNepalAPI.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;
using static MNepalAPI.Controllers.ConnectIPSController;

namespace MNepalAPI.Controllers
{
    [MyBasicAuthenticationFilter]
    public class ConnectIPSController : ApiController
    {
        #region CIPSToken
        [Route("api/ConnectIPS/CIPSToken")]
        [HttpPost]
        public async Task<HttpResponseMessage> CIPSToken(ConnectIPSUserAuthenticaiton connectIPSUserAuthenticaiton)
        {
            string cipsUsername = "";
            string clientIp = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            try
            {
                cipsUsername = ConfigurationManager.AppSettings["CIPSUserName"];
                var cipsPassword = ConfigurationManager.AppSettings["CIPSPassword"];

                FormUrlEncodedContent content = new FormUrlEncodedContent(new Dictionary<string, string>()
                {

                });

                if (connectIPSUserAuthenticaiton.refresh_token == null)
                {
                    content = new FormUrlEncodedContent(
                                new KeyValuePair<string, string>[] {
                                new KeyValuePair<string, string>("grant_type", "password"),
                                new KeyValuePair<string, string>("username", cipsUsername),
                                new KeyValuePair<string, string>("password", cipsPassword)
                                });

                }
                else
                {
                    content = new FormUrlEncodedContent(
                                new KeyValuePair<string, string>[] {
                                new KeyValuePair<string, string>("refresh_token", connectIPSUserAuthenticaiton.refresh_token),
                                new KeyValuePair<string, string>("grant_type", "refresh_token")
                                });
                }

                using (var httpClient = new HttpClient())
                {
                    var UserName = ConfigurationManager.AppSettings["CIPSAuthUserName"];
                    var UserPassword = ConfigurationManager.AppSettings["CIPSAuthPassword"];
                    var ConnectIPSBaseURL = ConfigurationManager.AppSettings["ConnectIPSBaseURL"];

                    var byteArray = Encoding.ASCII.GetBytes(UserName + ":" + UserPassword);

                    ServicePointManager.ServerCertificateValidationCallback = delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; }; //to remove ssl error

                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    var httpResponse = await httpClient.PostAsync(ConnectIPSBaseURL + "oauth/token", content);

                    if (httpResponse.StatusCode != HttpStatusCode.OK)
                    {
                        var responseContent = await httpResponse.Content.ReadAsStringAsync();
                        var json = JsonConvert.DeserializeObject(responseContent);
                        return Request.CreateResponse(httpResponse.StatusCode, json);
                    }

                    if (httpResponse.Content != null && httpResponse.StatusCode == HttpStatusCode.OK)
                    {
                        //response
                        var responseContent = await httpResponse.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<ConnectIPSTokenResponse>(responseContent);


                        ConnectIPSTokenResponse connectIPSToken = new ConnectIPSTokenResponse();
                        connectIPSToken.access_token = result.access_token;
                        connectIPSToken.expires_in = result.expires_in;
                        connectIPSToken.refresh_token = result.refresh_token;
                        connectIPSToken.scope = result.scope;
                        connectIPSToken.token_type = result.token_type;

                        if (connectIPSToken.customer_details == null)
                        {
                            connectIPSToken.customer_details = cipsUsername;
                        }

                        //Database
                        int resultsPayments = ConnectIPSUtilities.ConnectIPS(connectIPSToken);
                        if (resultsPayments == -1)
                        {
                            return Request.CreateResponse(HttpStatusCode.OK, connectIPSToken);
                        }                     

                    }
                    else
                    {
                        return Request.CreateResponse(httpResponse.StatusCode);
                    }
                }


                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Unauthorized");
            }

            catch (Exception ex)
            {
                HelperStoreSqlLog.WriteError(ex, "CIPSToken");
            }
            return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Unauthorized");

        }
        #endregion

        #region BankList
        [Route("api/ConnectIPS/GetCIPSBankList")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetCIPSBankList()
        {
            try
            {
                var re = Request;
                var headers = re.Headers;

                if (headers.Contains("token"))
                {
                    //from header
                    string authorizationToken = headers.GetValues("token").First();
                    if (authorizationToken == null || authorizationToken == "")
                        return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Invalid Token");

                    using (var httpClient = new HttpClient())
                    {
                        var ConnectIPSBaseURL = ConfigurationManager.AppSettings["ConnectIPSBaseURL"];

                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", authorizationToken);
                        ServicePointManager.ServerCertificateValidationCallback = delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; }; //to remove ssl error
                        var httpResponse = await httpClient.PostAsync(ConnectIPSBaseURL + "api/getcipsbanklist", null);
                        //response
                        var responseContent = await httpResponse.Content.ReadAsStringAsync();

                        if (httpResponse.StatusCode == HttpStatusCode.Unauthorized)
                        {

                            // Check if the unauthorized is because of invalid token
                            if (responseContent.Contains("invalid_token"))
                            {
                                // Get refresh token from database matching the accesstoken from the header
                                var accessToken = CIPSUtilities.GetAccessToken(authorizationToken);

                                if (accessToken == "")
                                    return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Invalid Token");

                                // If doesnot mathch throw invalid token, else continue below
                                // Get refresh token form database
                                var accessTOken = await GetAccessToken(accessToken);
                                headers.Clear();
                                headers.Add("token", accessTOken);
                                try
                                {
                                    return await GetCIPSBankList();

                                }
                                catch (Exception ex)
                                {

                                    throw ex;
                                }
                            }
                            return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Invalid Token");

                        }

                        if (httpResponse.Content != null)
                        {

                            var bankListResponse = JsonConvert.DeserializeObject<List<BankList>>(responseContent);
                            var orderBankListResponse = bankListResponse.OrderBy(x => x.bankName.ToLower());
                            return Request.CreateResponse(HttpStatusCode.OK, orderBankListResponse);
                        }
                        return Request.CreateResponse(HttpStatusCode.OK);
                    }

                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Invalid Token");
                }

            }
            catch (Exception ex)
            {

                HelperStoreSqlLog.WriteError(ex, "GetCIPSBankList");
            }
            return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Unauthorized");

        }
        #endregion

        #region BankBranchList
        [Route("api/ConnectIPS/GetCIPSBranchList")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetCIPSBranchList(string bankId)
        {
            try
            {
                var re = Request;
                var headers = re.Headers;

                if (headers.Contains("token"))
                {
                    //from header
                    string authorizationToken = headers.GetValues("token").First();
                    if (authorizationToken == null || authorizationToken == "")
                        return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Invalid Token");

                    using (var httpClient = new HttpClient())
                    {
                        var ConnectIPSBaseURL = ConfigurationManager.AppSettings["ConnectIPSBaseURL"];

                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", authorizationToken);
                        ServicePointManager.ServerCertificateValidationCallback = delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; }; //to remove ssl error
                        var httpResponse = await httpClient.GetAsync(ConnectIPSBaseURL + "api/getbranchlist/" + bankId);
                        //response
                        var responseContent = await httpResponse.Content.ReadAsStringAsync();

                        if (httpResponse.StatusCode == HttpStatusCode.Unauthorized)
                        {

                            // Check if the unauthorized is because of invalid token
                            if (responseContent.Contains("invalid_token"))
                            {
                                // Get refresh token from database matching the accesstoken from the header
                                var accessToken = CIPSUtilities.GetAccessToken(authorizationToken);

                                if (accessToken == "")
                                    return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Invalid Token");

                                // If doesnot mathch throw invalid token, else continue below
                                // Get refresh token form database
                                var accessTOken = await GetAccessToken(accessToken);
                                headers.Clear();
                                headers.Add("token", accessTOken);
                                try
                                {
                                    return await GetCIPSBranchList(bankId);

                                }
                                catch (Exception ex)
                                {

                                    throw ex;
                                }
                            }
                            return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Invalid Token");

                        }

                        if (httpResponse.Content != null)
                        {
                            var bankBranchLists = JsonConvert.DeserializeObject<List<CIPSBnakBranchDetails>>(responseContent);
                            var sortBankBranchLists = bankBranchLists.OrderBy(x => x.branchName.Trim().ToLower());

                            return Request.CreateResponse(HttpStatusCode.OK, sortBankBranchLists);
                        }
                        return Request.CreateResponse(HttpStatusCode.OK);

                    }
                }

                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Invalid Token");
                }
            }
            catch (Exception ex)
            {
                HelperStoreSqlLog.WriteError(ex, "GetCIPSBranchList");
            }
            return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Unauthorized");

        }
        #endregion

        #region BankToBank
        [Route("api/ConnectIPS/BankToBank")]
        [HttpPost]
        public async Task<HttpResponseMessage> BankToBank(ConnectIPS connectIPS)
        {
            try
            {
                var cipsUserName = ConfigurationManager.AppSettings["CIPSUserName"];
                var re = Request;
                var headers = re.Headers;

                if (headers.Contains("token"))
                {
                    //from header
                    string authorizationToken = headers.GetValues("token").First();
                    if (authorizationToken == null || authorizationToken == "")
                        return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Invalid Token");

                    RandomCodeGenerator randomCodeGenerator = new RandomCodeGenerator();
                    string generateRandomNumber = randomCodeGenerator.CreateRandomCode(12);

                    var json = JsonConvert.SerializeObject(connectIPS.cipsTransactionDetailList);
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    Cipstransactiondetaillist[] cipsTransactionDetailList = js.Deserialize<Cipstransactiondetaillist[]>(json);

                    Cipsbatchdetail cipsbatchdetail = new Cipsbatchdetail();
                    //cipsbatchdetail.batchId = connectIPS.cipsBatchDetail.batchId;
                    cipsbatchdetail.batchId = generateRandomNumber;
                    cipsbatchdetail.batchAmount = connectIPS.cipsBatchDetail.batchAmount;



                    decimal decimalRounded = Decimal.Parse(connectIPS.cipsBatchDetail.batchAmount.ToString("0.00"));
                    cipsbatchdetail.batchAmount = decimalRounded;
                    cipsbatchdetail.batchCount = connectIPS.cipsBatchDetail.batchCount;
                    cipsbatchdetail.batchCrncy = connectIPS.cipsBatchDetail.batchCrncy;
                    cipsbatchdetail.categoryPurpose = connectIPS.cipsBatchDetail.categoryPurpose;
                    cipsbatchdetail.debtorAgent = connectIPS.cipsBatchDetail.debtorAgent;
                    cipsbatchdetail.debtorBranch = connectIPS.cipsBatchDetail.debtorBranch;
                    cipsbatchdetail.debtorName = connectIPS.cipsBatchDetail.debtorName;
                    cipsbatchdetail.debtorAccount = connectIPS.cipsBatchDetail.debtorAccount;
                    cipsbatchdetail.debtorIdType = connectIPS.cipsBatchDetail.debtorIdType;
                    cipsbatchdetail.debtorIdValue = connectIPS.cipsBatchDetail.debtorIdValue;
                    cipsbatchdetail.debtorAddress = connectIPS.cipsBatchDetail.debtorAddress;
                    cipsbatchdetail.debtorPhone = connectIPS.cipsBatchDetail.debtorPhone;
                    cipsbatchdetail.debtorMobile = connectIPS.cipsBatchDetail.debtorMobile;
                    cipsbatchdetail.debtorEmail = connectIPS.cipsBatchDetail.debtorEmail;

                    var batchString = cipsbatchdetail.batchId + "," + cipsbatchdetail.debtorAgent + "," + cipsbatchdetail.debtorBranch + "," + cipsbatchdetail.debtorAccount
                        + "," + cipsbatchdetail.batchAmount + "," + cipsbatchdetail.batchCrncy + ",";

                    var transactionString = cipsTransactionDetailList.FirstOrDefault().instructionId + "," + cipsTransactionDetailList.FirstOrDefault().creditorAgent + ","
                        + cipsTransactionDetailList.FirstOrDefault().creditorBranch + "," + cipsTransactionDetailList.FirstOrDefault().creditorAccount + "," +
                        cipsbatchdetail.batchAmount;

                    string generateToken = batchString + transactionString + "," + cipsUserName;
                    TokenGenerationNCHL tokenGenerationNCHL = new TokenGenerationNCHL();

                    //string getCertificate = AppDomain.CurrentDomain.BaseDirectory + "Certificate/NIBLMB.p12";
                    string getCertificate = System.Web.Hosting.HostingEnvironment.MapPath(ConfigurationManager.AppSettings["CIPSKeyPath"]);
                    
                    DateTime dateTime = DateTime.Now;

                    //var getCertificate = HttpContext.Current.Server.MapPath("~/Certificate/NPI.pfx");
                    var transactionToken = tokenGenerationNCHL.getSignature(generateToken, getCertificate);
                    var cipsObject = new ConnectIPS
                    {
                        cipsBatchDetail = cipsbatchdetail,
                        cipsTransactionDetailList = cipsTransactionDetailList,
                        token = transactionToken,
                        username = connectIPS.username
                    };

                    //CIPS Request Database
                    int resultsRequest = CIPSUtilities.ConnectIPSRequest(cipsObject, connectIPS.username, dateTime);

                    //CIPS Request Statement Database
                    int resultsRequestStatement = CIPSUtilities.ConnectIPSRequestStatement(cipsObject, connectIPS.username, dateTime);


                    // Get refresh token from database matching the accesstoken from the header
                    var accessToken = CIPSUtilities.GetAccessToken(authorizationToken);

                    if (accessToken == "")
                        return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Invalid Token");
                    var accessTOken = await GetAccessToken(accessToken);

                    // Serialize our concrete class into a JSON String
                    var stringPayload = await Task.Run(() => JsonConvert.SerializeObject(cipsObject));

                    // Wrap our JSON inside a StringContent which then can be used by the HttpClient class
                    var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");
                    using (var httpClient = new HttpClient())
                    {
                        var ConnectIPSBaseURL = ConfigurationManager.AppSettings["ConnectIPSBaseURL"];
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accessTOken);
                        ServicePointManager.ServerCertificateValidationCallback = delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; }; //to remove ssl error

                        var httpResponse = await httpClient.PostAsync(ConnectIPSBaseURL + "api/postcipsbatch", httpContent);
                        //response
                        var responseContent = await httpResponse.Content.ReadAsStringAsync();

                        if (httpResponse.StatusCode == HttpStatusCode.Unauthorized || httpResponse.StatusCode == HttpStatusCode.BadRequest)
                        {
                            // Check if the unauthorized is because of invalid token
                            if (responseContent.Contains("invalid_token"))
                            {
                                // Get refresh token from database matching the accesstoken from the header
                                var accessToken1 = CIPSUtilities.GetAccessToken(authorizationToken);
                                if (accessToken == "")
                                    return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Invalid Token");

                                // If doesnot mathch throw invalid token, else continue below
                                // Get refresh token form database
                                var accessTOken1 = await GetAccessToken(accessToken1);
                                headers.Clear();
                                headers.Add("token", accessTOken1);
                                try
                                {
                                    return await BankToBank(connectIPS);

                                }
                                catch (Exception ex)
                                {

                                    throw ex;
                                }
                            }
                            return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, responseContent);
                        }

                        CIPSResponse cipsResponse = new CIPSResponse();
                        if (httpResponse.Content != null && httpResponse.StatusCode == HttpStatusCode.OK)
                        {

                            var jsonResponseContent = JsonConvert.DeserializeObject<ConnectIPSResponse>(responseContent);
                            //save to db
                            cipsResponse.batchResponseCode = jsonResponseContent.cipsBatchResponse.responseCode;
                            cipsResponse.batchResponseMessage = jsonResponseContent.cipsBatchResponse.responseMessage;
                            cipsResponse.batchId = jsonResponseContent.cipsBatchResponse.batchId;
                            cipsResponse.debitStatus = jsonResponseContent.cipsBatchResponse.debitStatus;
                            cipsResponse.batchResponseId = jsonResponseContent.cipsBatchResponse.id;
                            cipsResponse.txnResponseCode = jsonResponseContent.cipsTxnResponseList.FirstOrDefault().responseCode;
                            cipsResponse.txnResponseMessage = jsonResponseContent.cipsTxnResponseList.FirstOrDefault().responseMessage;
                            cipsResponse.txnId = jsonResponseContent.cipsTxnResponseList.FirstOrDefault().id;
                            cipsResponse.instructionId = jsonResponseContent.cipsTxnResponseList.FirstOrDefault().instructionId;
                            cipsResponse.creditStatus = jsonResponseContent.cipsTxnResponseList.FirstOrDefault().creditStatus;
                            cipsResponse.amount = cipsbatchdetail.batchAmount.ToString();
                            cipsResponse.dateTime = dateTime;
                            cipsResponse.username = cipsUserName;
                            cipsResponse.thailiUserName = connectIPS.username;

                            int resultsPayments = CIPSUtilities.ConnectIPS(cipsResponse);

                            decimal feeAmount = 0;
                            //fee amount 
                            if (cipsbatchdetail.batchAmount <= 500)
                            {
                                feeAmount = 2;
                            }
                            else if (cipsbatchdetail.batchAmount > 500 && cipsbatchdetail.batchAmount <= 5000)
                            {
                                feeAmount = 5;
                            }
                            else if (cipsbatchdetail.batchAmount > 5000 && cipsbatchdetail.batchAmount <= 50000)
                            {
                                feeAmount = 10;
                            }
                            else if (cipsbatchdetail.batchAmount > 50000)
                            {
                                feeAmount = 15;
                            }
                            //fee amount ends

                            //save to MNRequest 
                            MNRequestResponse cipsMNRequest = new MNRequestResponse();
                            cipsMNRequest.originId = connectIPS.username;
                            cipsMNRequest.originType = "6011";
                            cipsMNRequest.serviceCode = "11";
                            cipsMNRequest.sourceBankCode = cipsbatchdetail.debtorAgent;
                            if(cipsbatchdetail.debtorBranch=="1")
                            {
                                cipsMNRequest.sourceBranchCode = "001";

                            }
                            else
                            {
                                cipsMNRequest.sourceBranchCode = cipsbatchdetail.debtorBranch;
                            }
                            cipsMNRequest.sourceAccountNumber = cipsbatchdetail.debtorAccount;
                            if(cipsTransactionDetailList.FirstOrDefault().creditorBranch == "1")
                            {
                                cipsMNRequest.destBranchCode = "001";

                            }
                            else
                            {
                                cipsMNRequest.destBranchCode = cipsTransactionDetailList.FirstOrDefault().creditorBranch;
                            }
                            cipsMNRequest.destBankCode = cipsTransactionDetailList.FirstOrDefault().creditorAgent;
                            cipsMNRequest.destAccountNumber = cipsTransactionDetailList.FirstOrDefault().creditorAccount;
                            cipsMNRequest.amount = cipsbatchdetail.batchAmount + feeAmount;
                            cipsMNRequest.feeId = "";
                            cipsMNRequest.traceNo = randomCodeGenerator.CreateRandomCode(6);
                            cipsMNRequest.tranDate = dateTime;
                            cipsMNRequest.retrievalReference = cipsbatchdetail.batchId;
                            cipsMNRequest.desc1 = "Payment to CIPS from " + cipsMNRequest.sourceAccountNumber + " to " + cipsMNRequest.destAccountNumber;
                            cipsMNRequest.desc2 = connectIPS.username;
                            cipsMNRequest.desc3 = "";
                            cipsMNRequest.reversalStatus = "";
                            cipsMNRequest.oTraceNo = "";
                            cipsMNRequest.oTranDateTime = "";
                            cipsMNRequest.isProcessed = "T";
                            cipsMNRequest.status = "";
                            cipsMNRequest.fromSMS = "";
                            cipsMNRequest.remark = "Payment to CIPS from " + cipsMNRequest.sourceAccountNumber + " to " + cipsMNRequest.destAccountNumber;
                            cipsMNRequest.smsAlertType = null;
                            cipsMNRequest.enteredAt = dateTime;
                            cipsMNRequest.merchantId = 0;
                            cipsMNRequest.uId = Guid.NewGuid().ToString();

                            int resultsMNRequest = CIPSUtilities.cipsMNRequest(cipsMNRequest);


                            if (jsonResponseContent.cipsBatchResponse.responseCode == "000" && jsonResponseContent.cipsTxnResponseList.FirstOrDefault().responseCode == "000")
                            {
                                //save to MNResponse
                                MNRequestResponse cipsMNResponse = new MNRequestResponse();
                                cipsMNResponse.originId = connectIPS.username;
                                cipsMNResponse.originType = "6011";
                                cipsMNResponse.serviceCode = "11";
                                cipsMNResponse.sourceBankCode = cipsbatchdetail.debtorAgent;
                                if(cipsbatchdetail.debtorBranch == "1")
                                {
                                    cipsMNResponse.sourceBranchCode = "001";

                                }
                                else
                                {
                                    cipsMNResponse.sourceBranchCode = cipsbatchdetail.debtorBranch;
                                }
                                cipsMNResponse.sourceAccountNumber = cipsbatchdetail.debtorAccount;
                                cipsMNResponse.destBankCode = cipsTransactionDetailList.FirstOrDefault().creditorAgent;
                                if(cipsTransactionDetailList.FirstOrDefault().creditorBranch == "1")
                                {
                                    cipsMNResponse.destBranchCode = "001";
                                }
                                else
                                {
                                    cipsMNResponse.destBranchCode = cipsTransactionDetailList.FirstOrDefault().creditorBranch;
                                }
                                cipsMNResponse.destAccountNumber = cipsTransactionDetailList.FirstOrDefault().creditorAccount;
                                cipsMNResponse.feeId = "";                               
                                cipsMNResponse.amount = cipsbatchdetail.batchAmount + feeAmount;
                                cipsMNResponse.traceNo = cipsMNRequest.traceNo;
                                cipsMNResponse.tranDate = dateTime;
                                cipsMNResponse.tranTime = "";
                                cipsMNResponse.retrievalReference = cipsMNRequest.retrievalReference;
                                cipsMNResponse.responseCode = "000";
                                cipsMNResponse.responseDescription = "Payment to CIPS from " + cipsMNRequest.sourceAccountNumber + " to " + cipsMNRequest.destAccountNumber;
                                cipsMNResponse.balance = "";
                                cipsMNResponse.accountHolderName = connectIPS.username;
                                cipsMNResponse.miniStmtRecord = connectIPS.username;
                                cipsMNResponse.reversalStatus = "";
                                cipsMNResponse.tranId = randomCodeGenerator.CreateRandomCodeWithString(9);
                                cipsMNResponse.destUsername = cipsTransactionDetailList.FirstOrDefault().creditorName;

                                int resultsMNResponse = CIPSUtilities.cipsMNResponse(cipsMNResponse);

                                ///SMS
                                string messagereply = "";
                                try
                                {
                                    //FOR CUSTOMER
                                    try
                                    {
                                        //Alert Dynamic
                                        string AlertType = "CIPS";

                                        //FOR CUSTOMER SMS                                     
                                        #region FOR CUSTOMER SMS

                                        CustomerSMS customerSMS = new CustomerSMS();
                                        string cSMS = customerSMS.CustSMSEnable(AlertType, connectIPS.username.Trim(), "", cipsMNResponse.amount.ToString(), "", cipsMNResponse.destAccountNumber, dateTime.ToString("dd/MM/yyyy"));
                                        if (cSMS == "false")
                                        {

                                        }
                                        else
                                        {

                                        }

                                        #endregion

                                    }
                                    catch (Exception ex)
                                    {
                                        throw ex;
                                    }

                                }
                                catch (Exception ex)
                                {
                                    // throw ex
                                    string statusCode = "400";
                                    string message = ex.Message;
                                }
                                return Request.CreateResponse(httpResponse.StatusCode, jsonResponseContent);
                            }
                            else
                            {
                                return Request.CreateResponse(httpResponse.StatusCode, jsonResponseContent);
                            }

                        }

                        var jsonResponseContent1 = JsonConvert.DeserializeObject(responseContent);

                        if (httpResponse.Content != null)
                        {
                            var jsonResponseContent = JsonConvert.DeserializeObject<ConnectIPSResponse>(responseContent);

                            return Request.CreateResponse(httpResponse.StatusCode);
                        }
                        return Request.CreateResponse(httpResponse.StatusCode, jsonResponseContent1);
                    }
                }

                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Invalid Token");
                }


            }
            catch (Exception ex)
            {
                HelperStoreSqlLog.WriteError(ex, "BankToBank");
            }
            return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Unauthorized");

        }
        #endregion

        #region CIPSChargeList
        [Route("api/ConnectIPS/GetCIPSChargeList")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetCIPSChargeList()
        {
            try
            {
                var re = Request;
                var headers = re.Headers;

                if (headers.Contains("token"))
                {
                    //from header
                    string authorizationToken = headers.GetValues("token").First();
                    if (authorizationToken == null || authorizationToken == "")
                        return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Invalid Token");


                    using (var httpClient = new HttpClient())
                    {
                        var ConnectIPSBaseURL = ConfigurationManager.AppSettings["ConnectIPSBaseURL"];

                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", authorizationToken);
                        ServicePointManager.ServerCertificateValidationCallback = delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; }; //to remove ssl error

                        var httpResponse = await httpClient.GetAsync(ConnectIPSBaseURL + "api/getcipschargelist/MER-1-APP-3");
                        var responseContent = await httpResponse.Content.ReadAsStringAsync();


                        if (httpResponse.StatusCode == HttpStatusCode.Unauthorized)
                        {

                            // Check if the unauthorized is because of invalid token
                            if (responseContent.Contains("invalid_token"))
                            {
                                // Get refresh token from database matching the accesstoken from the header
                                var accessToken = CIPSUtilities.GetAccessToken(authorizationToken);

                                if (accessToken == "")
                                    return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Invalid Token");

                                // If doesnot mathch throw invalid token, else continue below
                                // Get refresh token form database
                                var accessTOken = await GetAccessToken(accessToken);
                                headers.Clear();
                                headers.Add("token", accessTOken);
                                try
                                {
                                    return await GetCIPSChargeList();

                                }
                                catch (Exception ex)
                                {

                                    throw ex;
                                }
                            }
                            return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Invalid Token");

                        }


                        if (httpResponse.Content != null)
                        {
                            var jsonResponseContent = JsonConvert.DeserializeObject(responseContent);
                            return Request.CreateResponse(httpResponse.StatusCode, jsonResponseContent);
                        }
                        return Request.CreateResponse(httpResponse.StatusCode);

                    }

                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Invalid Token");
                }
            }
            catch (Exception ex)
            {

                HelperStoreSqlLog.WriteError(ex, "GetCIPSChargeList");
            }
            return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Unauthorized");

        }
        #endregion

        #region BasicAuth
        [Route("api/ConnectIPS/BasicAuth")]
        [HttpPost]
        public async Task<HttpResponseMessage> BasicAuth(BasicAuth basicAuth)
        {
            try
            {
                if ((basicAuth.UserName != "" || basicAuth.Password != "") && ((basicAuth.UserName == "MNepal") && basicAuth.Password == "$un$h!ne@0405"))
                {
                    var byteArray = Encoding.ASCII.GetBytes(basicAuth.UserName + ":" + basicAuth.Password);

                    var basicAuthHeader = Convert.ToBase64String(byteArray);
                    return Request.CreateResponse(HttpStatusCode.OK, basicAuthHeader);
                }

                else
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, "Unauthorized");
                }

            }
            catch (Exception ex)
            { 
                HelperStoreSqlLog.WriteError(ex, "BasicAuth");
            }
            return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Unauthorized");

        }
        #endregion

        #region ValidateBankAccount
        [Route("api/ConnectIPS/ValidateBankAccount")]
        [HttpPost]
        public async Task<HttpResponseMessage> ValidateBankAccount(ValidateCreditorBankAccount validateCreditorBankAccount)
        {
            try
            {
                var re = Request;
                var headers = re.Headers;
                if (headers.Contains("token"))
                {
                    //from header
                    string authorizationToken = headers.GetValues("token").First();
                    if (authorizationToken == null || authorizationToken == "")
                        return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Invalid Token");

                    string bankId = validateCreditorBankAccount.bankId;
                    string accountId = validateCreditorBankAccount.accountId;
                    string accountName = validateCreditorBankAccount.accountName;

                    if (bankId == "" || bankId == null)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Bank Id is Required");
                    }
                    else if (accountId == "" || accountId == null)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Account Number is Required");
                    }
                    else if (accountName == "" || accountName == null)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Account Name is Required");
                    }
                    


                    // Get refresh token from database matching the accesstoken from the header
                    var accessToken = CIPSUtilities.GetAccessToken(authorizationToken);

                    if (accessToken == "")
                        return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Invalid Token");
                    var accessTOken = await GetAccessToken(accessToken);

                    var cipsValidateAccount = new ValidateCreditorBankAccount
                    {
                        bankId = bankId,
                        accountId = accountId,
                        accountName = accountName
                    };


                    // Serialize our concrete class into a JSON String
                    var stringPayload = await Task.Run(() => JsonConvert.SerializeObject(cipsValidateAccount));

                    // Wrap our JSON inside a StringContent which then can be used by the HttpClient class
                    var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");
                    using (var httpClient = new HttpClient())
                    {
                        var ConnectIPSBaseURL = ConfigurationManager.AppSettings["ConnectIPSBaseURL"];
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accessTOken);
                        ServicePointManager.ServerCertificateValidationCallback = delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; }; //to remove ssl error

                        var httpResponse = await httpClient.PostAsync(ConnectIPSBaseURL + "api/validatebankaccount", httpContent);
                        //response
                        var responseContent = await httpResponse.Content.ReadAsStringAsync();

                        var jsonResponse = JsonConvert.DeserializeObject<ValidateCreditorBankAccount>(responseContent);


                        if (jsonResponse.responseCode == "000")
                        {
                            ValidateCreditorBankAccount validateCreditorBank = new ValidateCreditorBankAccount();

                            validateCreditorBank.bankId = jsonResponse.bankId;
                            validateCreditorBank.branchId = jsonResponse.branchId;
                            validateCreditorBank.accountId = jsonResponse.accountId;
                            validateCreditorBank.accountName = jsonResponse.accountName;
                            validateCreditorBank.currency = jsonResponse.currency;
                            validateCreditorBank.responseCode = jsonResponse.responseCode;
                            validateCreditorBank.responseMessage = jsonResponse.responseMessage;
                            validateCreditorBank.matchPercentate = jsonResponse.matchPercentate;
                            validateCreditorBank.baseUrl = jsonResponse.baseUrl;
                            validateCreditorBank.username = jsonResponse.username;
                            validateCreditorBank.password = jsonResponse.password;

                            return Request.CreateResponse(HttpStatusCode.OK, validateCreditorBank);
                        }

                        return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.DeserializeObject(responseContent));
                    }
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Invalid Token");
                }
            }
            catch (Exception ex)
            {
                HelperStoreSqlLog.WriteError(ex, "ValidateBankAccount");
            }
            return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Unauthorized");


        }
        #endregion

        #region GetAccessToken
        private async Task<string> GetAccessToken(string refereshToken)
        {
            var content = new FormUrlEncodedContent(
                   new KeyValuePair<string, string>[]
                   {
                            new KeyValuePair<string, string>("refresh_token",refereshToken),
                            new KeyValuePair<string, string>("grant_type","refresh_token")
                   });


            using (var httpClient = new HttpClient())
            {
                var UserName = ConfigurationManager.AppSettings["CIPSAuthUserName"];
                var UserPassword = ConfigurationManager.AppSettings["CIPSAuthPassword"];
                var ConnectIPSBaseURL = ConfigurationManager.AppSettings["ConnectIPSBaseURL"];

                var byteArray = Encoding.ASCII.GetBytes(UserName + ":" + UserPassword);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                ServicePointManager.ServerCertificateValidationCallback = delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; }; //to remove ssl error
                var httpResponse = await httpClient.PostAsync(ConnectIPSBaseURL + "oauth/token", content);

                if (httpResponse.Content != null && httpResponse.StatusCode == HttpStatusCode.OK)
                {
                    //response
                    var responseContent = await httpResponse.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<ConnectIPSTokenResponse>(responseContent);

                    ConnectIPSTokenResponse connectIPSToken = new ConnectIPSTokenResponse();
                    return result.access_token;


                }
            }
            return null;
        }
        #endregion

        #region CheckPin
        [Route("api/ConnectIPS/CheckPin")]
        [HttpPost]
        public async Task<HttpResponseMessage> CheckPin(CheckPin checkPin)
        {
            CheckPin check = new CheckPin();
            try
            {
                string message = "";
                string failedMessage = "";
                string statusCode = "";

                var userName = checkPin.username;
                var pin = checkPin.pin;

                var hashPin = HashAlgo.Hash(pin);
                //Get Bank Register User Name
                DataTable dtableUserName = CIPSUtilities.CheckPin(userName);
                if (dtableUserName != null && dtableUserName.Rows.Count > 0)
                {
                    checkPin.pin = dtableUserName.Rows[0]["PIN"].ToString();


                }


                if (hashPin != checkPin.pin)
                {
                    statusCode = "400";
                    check.message = "Invalid PIN";
                    failedMessage = message;

                    LoginUtils.SetPINTries(checkPin.username, "BUWP");//add +1 in trypwd

                    if (LoginUtils.GetPINBlockTime(checkPin.username)) //check if blocktime is greater than current time 
                    {
                        message = "Invalid PIN! You have already attempt 3 times with wrong PIN,Please try again after 10 minutes";
                        check.message = message;

                        return Request.CreateResponse(HttpStatusCode.Unauthorized, check);

                    }

                }
                else
                {
                    LoginUtils.SetPINTries(checkPin.username, "RPT");
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
            }
            catch (Exception)
            {

                throw;
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest, check);
        }
        #endregion

        //[HttpGet]
        //public async Task<HttpResponseMessage> SMS()
        //{

        //    //SMS
        //    string messagereply = "";
        //    string thailiUserName = "9841040534";
        //    try
        //    {
        //        string SMSNTC = ConfigurationManager.AppSettings["MNepalNTCSMSServerUrl"];
        //        string SMSNCELL = System.Web.Configuration.WebConfigurationManager.AppSettings["MNepalSMSServerUrl"];
        //        var client = new HttpClient();
        //        //var client = new WebClient();
        //        messagereply = "Dear " + "test message"+ "," + "\n";



        //        //SENDER
        //        if ((thailiUserName.Substring(0, 3) == "980") || (thailiUserName.Substring(0, 3) == "981")) //FOR NCELL
        //        {
        //            //FOR NCELL
        //            var content = client.GetAsync(
        //             SMSNCELL + "977" + thailiUserName + "&message=" + messagereply + "");
        //        }
        //        else if ((thailiUserName.Substring(0, 3) == "985") || (thailiUserName.Substring(0, 3) == "984")
        //                    || (thailiUserName.Substring(0, 3) == "986"))
        //        {
        //            //FOR NTC
        //            var content = client.GetAsync(
        //                SMSNTC + "977" + thailiUserName + "&message=" + messagereply + "");
        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //        // throw ex
        //        string statusCode = "400";
        //        string message = ex.Message;
        //    }
        //    return Request.CreateResponse(HttpStatusCode.OK);
        //}

        #region ChangeToHash
        [Route("api/ConnectIPS/ChangeToHash")]
        [HttpPost]
        public async Task<HttpResponseMessage> ChangeToHash(string pin)
        {
            var hashPin = HashAlgo.Hash(pin);

            return Request.CreateResponse(HttpStatusCode.OK, hashPin);

        }
        #endregion


    }
}
