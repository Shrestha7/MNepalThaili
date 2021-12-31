using CustApp.Helper;
using CustApp.Models;
using CustApp.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using static CustApp.Models.KUKL;

namespace CustApp.Controllers
{
    public class KUKLController : Controller
    {
        DAL objdal = new DAL();
        // GET: KUKL
        #region KUKLIndex
        public async Task<ActionResult> Index()
        {
            var sessionUserDetails = SessionUserDetails();

            TempData["userType"] = sessionUserDetails.userType;

            if (TempData["userType"] != null)
            {
                this.ViewData["userType"] = this.TempData["userType"];
                ViewBag.UserType = this.TempData["userType"];
                ViewBag.Name = sessionUserDetails.name;
                ViewBag.SenderMobileNo = sessionUserDetails.userName;



                List<KUKLBranch> getKUKLBranch = await GetKUKLBranch();
                ViewBag.getKUKLBranch = getKUKLBranch;


                List<KUKLModule> getKUKLPaymentModule = await KUKLPaymentMode();
                ViewBag.getKUKLPaymentModule = getKUKLPaymentModule;




                int id = TraceIdGenerator.GetID() + 1;
                string stringid = (id).ToString();
                string traceID = stringid.PadLeft(11, '0') + 'W';
                ViewBag.TraceID = traceID;

                UserInfo userInfo = new UserInfo();
                MNBalance availBaln = new MNBalance();
                DataTable dtableUser1 = AvailBalnUtils.GetAvailBaln(sessionUserDetails.clientCode);
                if (dtableUser1 != null && dtableUser1.Rows.Count > 0)
                {
                    availBaln.amount = dtableUser1.Rows[0]["AvailBaln"].ToString();
                    ViewBag.AvailBalnAmount = availBaln.amount;
                }

                //Check KYC
                DataTable dtableUserCheckKYC = ProfileUtils.CheckKYC(sessionUserDetails.userName);
                if (dtableUserCheckKYC != null && dtableUserCheckKYC.Rows.Count > 0)
                {
                    userInfo.hasKYC = dtableUserCheckKYC.Rows[0]["hasKYC"].ToString();
                    userInfo.IsRejected = dtableUserCheckKYC.Rows[0]["IsRejected"].ToString();

                    ViewBag.IsRejected = userInfo.IsRejected;
                    ViewBag.hasKYC = userInfo.hasKYC;
                }

                //Check Link Bank Account
                DataTable dtableUserCheckLinkBankAcc = ProfileUtils.CheckLinkBankAcc(sessionUserDetails.userName);
                if (dtableUserCheckLinkBankAcc != null && dtableUserCheckLinkBankAcc.Rows.Count > 0)
                {
                    userInfo.BankAccountNumber = dtableUserCheckLinkBankAcc.Rows[0]["HasBankKYC"].ToString();
                    ViewBag.HasBankKYC = userInfo.BankAccountNumber;
                }

                //Get BankAccountNumber
                DataTable dtableUserBankAccNumber = ProfileUtils.BankAccountNumber(sessionUserDetails.userName);
                if (dtableUserBankAccNumber != null && dtableUserBankAccNumber.Rows.Count > 0)
                {
                    userInfo.BankAccountNumber = dtableUserBankAccNumber.Rows[0]["BankAccountNumber"].ToString();

                    ViewBag.BankAccountNumber = userInfo.BankAccountNumber;
                    Session["BankAccountNumber"] = userInfo.BankAccountNumber;
                }

                //Get BankBranchId
                DataTable dtableUserBankBranchID = ProfileUtils.BankBranchCode(sessionUserDetails.userName);
                if (dtableUserBankBranchID != null && dtableUserBankBranchID.Rows.Count > 0)
                {
                    userInfo.BankBranchCode = dtableUserBankBranchID.Rows[0]["UserBranchCode"].ToString();

                    ViewBag.BankBranchId = userInfo.BankBranchCode;
                    if (userInfo.BankBranchCode == "001")
                        userInfo.BankBranchCode = "1";
                    Session["BankBranchId"] = userInfo.BankBranchCode;

                }

                //Get Bank Register User Name
                DataTable dtableUserName = ProfileUtils.BankRegisterName(sessionUserDetails.userName);
                if (dtableUserName != null && dtableUserName.Rows.Count > 0)
                {
                    userInfo.UserName = dtableUserName.Rows[0]["Name"].ToString();

                    ViewBag.BankRegisterName = userInfo.UserName;
                    Session["BankRegisterName"] = userInfo.UserName;

                }

                //For Profile Picture
                DataSet DSet = ProfileUtils.GetCusDetailProfileInfoDS(sessionUserDetails.clientCode);
                DataTable dKYC = DSet.Tables["dtKycDetail"];
                DataTable dDoc = DSet.Tables["dtKycDoc"];
                if (dKYC != null && dKYC.Rows.Count > 0)
                {
                    userInfo.CustStatus = dKYC.Rows[0]["CustStatus"].ToString();
                    ViewBag.CustStatus = userInfo.CustStatus;
                }
                if (dDoc != null && dDoc.Rows.Count > 0)
                {
                    userInfo.PassportImage = dDoc.Rows[0]["PassportImage"].ToString();
                    ViewBag.PassportImage = userInfo.PassportImage;
                }

                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        #endregion

        #region "POST: KUKLCheckPayment"
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> KUKLPayment(KUKLBranch _kukl)
        {

            var sessionUserDetails = SessionUserDetails();

            TempData["userType"] = sessionUserDetails.userType;

            this.ViewData["userType"] = this.TempData["userType"];
            ViewBag.UserType = this.TempData["userType"];
            ViewBag.Name = sessionUserDetails.name;


            string retoken = _kukl.tokenUnique;
            string reqToken = "";
            DataTable dtableVToken = ReqTokenUtils.GetReqToken(retoken);
            if (dtableVToken != null && dtableVToken.Rows.Count > 0)
            {
                reqToken = dtableVToken.Rows[0]["ReqVerifyToken"].ToString();
            }
            else if (dtableVToken.Rows.Count == 0)
            {
                reqToken = "0";
            }
            if (reqToken == "0")
            {
                ReqTokenUtils.InsertReqToken(retoken);


                MNBalance availBaln = new MNBalance();
                DataTable dtableUser1 = AvailBalnUtils.GetAvailBaln(sessionUserDetails.clientCode);
                if (dtableUser1 != null && dtableUser1.Rows.Count > 0)
                {
                    availBaln.amount = dtableUser1.Rows[0]["AvailBaln"].ToString();

                    ViewBag.AvailBalnAmount = availBaln.amount;
                }

                //For Profile Picture
                UserInfo userInfo = new UserInfo();
                DataSet DSet = ProfileUtils.GetCusDetailProfileInfoDS(sessionUserDetails.clientCode);
                DataTable dKYC = DSet.Tables["dtKycDetail"];
                DataTable dDoc = DSet.Tables["dtKycDoc"];
                if (dKYC != null && dKYC.Rows.Count > 0)
                {
                    userInfo.CustStatus = dKYC.Rows[0]["CustStatus"].ToString();
                    ViewBag.CustStatus = userInfo.CustStatus;
                }
                if (dDoc != null && dDoc.Rows.Count > 0)
                {
                    userInfo.PassportImage = dDoc.Rows[0]["PassportImage"].ToString();
                    ViewBag.PassportImage = userInfo.PassportImage;
                }
                //START Session for User Input Data
                Session["S_ConnectionNo"] = _kukl.connectionNo;
                Session["S_KUKLBranchName"] = _kukl.kuklBranchName;
                Session["S_KUKLBranchCode"] = _kukl.branchcode;
                Session["S_KUKLBillPaymentMode"] = _kukl.module;
                Session["S_KUKLApplicationId"] = _kukl.applicationId;

                //END Session
                HttpResponseMessage _res = new HttpResponseMessage();
                string mobile = sessionUserDetails.userName; //mobile is username
                TraceIdGenerator _tig = new TraceIdGenerator();
                var tid = _tig.GenerateTraceID();

                //specify to use TLS 1.2 as default connection
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                using (var httpClient = new HttpClient())
                {
                    var UserName = ConfigurationManager.AppSettings["BasicAuthUserName"];
                    var UserPassword = ConfigurationManager.AppSettings["BasicAuthPassword"];
                    var APIBaseURL = ConfigurationManager.AppSettings["APIBaseURL"];

                    // Serialize our concrete class into a JSON String
                    var stringPayload = await Task.Run(() => JsonConvert.SerializeObject(_kukl));
                    // Wrap our JSON inside a StringContent which then can be used by the HttpClient class
                    var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");

                    var byteArray = Encoding.ASCII.GetBytes(UserName + ":" + UserPassword);
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    var httpResponse = await httpClient.PostAsync(APIBaseURL + "KUKL/KUKLBillDetails", httpContent);
                    var responseContent = await httpResponse.Content.ReadAsStringAsync();

                    KUKLBillDetails kuklBillDetails = new KUKLBillDetails();

                    int responseCode = 0;
                    string respmsg = "";

                    string errorMessage = string.Empty;
                    string message = string.Empty;
                    string responsetext = string.Empty;
                    bool result = false;
                    string ava = string.Empty;
                    string avatra = string.Empty;
                    string avamsg = string.Empty;
                    try
                    {
                        if (httpResponse.IsSuccessStatusCode)
                        {
                            result = true;
                            responseCode = (int)httpResponse.StatusCode;
                            responsetext = await httpResponse.Content.ReadAsStringAsync();
                            message = httpResponse.Content.ReadAsStringAsync().Result;
                            var jsonResult = JsonConvert.DeserializeObject<KUKLBillDetails>(message);

                            Session["address"] = jsonResult.address;
                            Session["connection_no"] = jsonResult.connection_no;
                            Session["name"] = jsonResult.name;
                            Session["net_amount"] = jsonResult.net_amount;
                            Session["penalty"] = jsonResult.penalty;
                            Session["applicationId"] = jsonResult.applicationId;

                            return Json(new { responseCode = responseCode, responseText = responsetext },
                            JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            result = false;
                            responseCode = (int)_res.StatusCode;
                            responsetext = await _res.Content.ReadAsStringAsync();
                            dynamic json = JValue.Parse(responsetext);
                            message = json.d;
                            if (message == null)
                            {
                                return Json(new { responseCode = responseCode, responseText = responsetext },
                            JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                dynamic item = JValue.Parse(message);

                                return Json(new { responseCode = responseCode, responseText = (string)item["StatusMessage"] },
                                JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        return Json(new { responseCode = "400", responseText = ex.Message },
                            JsonRequestBehavior.AllowGet);
                    }
                }

            }
            else
            {
                return Json(new { responseCode = "400", responseText = "Please refresh the page again." },
                            JsonRequestBehavior.AllowGet);
            }

        }
        #endregion

        #region "GET: KUKLDetails"

        public ActionResult Details()
        {
            var sessionUserDetails = SessionUserDetails();

            TempData["userType"] = sessionUserDetails.userType;
            if (TempData["userType"] != null)
            {
                this.ViewData["userType"] = this.TempData["userType"];
                ViewBag.UserType = this.TempData["userType"];
                ViewBag.Name = sessionUserDetails.name;

                ViewBag.SenderMobileNo = sessionUserDetails.userName;
                //Session data set
                string address = (string)Session["address"];
                string connection_no = (string)Session["connection_no"];
                string name = (string)Session["name"];
                float net_amount = (float)Session["net_amount"];
                string penalty = (string)Session["penalty"];
                string applicationId = (string)Session["applicationId"];

                if ((connection_no == null && name == null) || (applicationId == null && name == null))
                {
                    return RedirectToAction("Index");

                }
                //End Session data set


                ViewBag.address = address;
                ViewBag.connection_no = connection_no;
                if (ViewBag.connection_no == null)
                {
                    ViewBag.connection_no = "0";
                }
                ViewBag.name = name;
                ViewBag.net_amount = net_amount;
                ViewBag.penalty = penalty;
                ViewBag.applicationId = applicationId;
                ViewBag.kuklBranchName = (string)Session["S_KUKLBranchName"];
                if (ViewBag.applicationId == null)
                {
                    ViewBag.applicationId = "0";
                }


                UserInfo userInfo = new UserInfo();
                MNBalance availBaln = new MNBalance();
                DataTable dtableUser1 = AvailBalnUtils.GetAvailBaln(sessionUserDetails.clientCode);
                if (dtableUser1 != null && dtableUser1.Rows.Count > 0)
                {
                    availBaln.amount = dtableUser1.Rows[0]["AvailBaln"].ToString();

                    ViewBag.AvailBalnAmount = availBaln.amount;
                }


                //Check KYC
                DataTable dtableUserCheckKYC = ProfileUtils.CheckKYC(sessionUserDetails.userName);
                if (dtableUserCheckKYC != null && dtableUserCheckKYC.Rows.Count > 0)
                {
                    userInfo.hasKYC = dtableUserCheckKYC.Rows[0]["hasKYC"].ToString();
                    userInfo.IsRejected = dtableUserCheckKYC.Rows[0]["IsRejected"].ToString();

                    ViewBag.IsRejected = userInfo.IsRejected;

                    ViewBag.hasKYC = userInfo.hasKYC;
                }

                //Check Link Bank Account
                DataTable dtableUserCheckLinkBankAcc = ProfileUtils.CheckLinkBankAcc(sessionUserDetails.userName);
                if (dtableUserCheckLinkBankAcc != null && dtableUserCheckLinkBankAcc.Rows.Count > 0)
                {
                    userInfo.BankAccountNumber = dtableUserCheckLinkBankAcc.Rows[0]["HasBankKYC"].ToString();

                    ViewBag.HasBankKYC = userInfo.BankAccountNumber;
                }

                //For Profile Picture
                DataSet DSet = ProfileUtils.GetCusDetailProfileInfoDS(sessionUserDetails.clientCode);
                DataTable dKYC = DSet.Tables["dtKycDetail"];
                DataTable dDoc = DSet.Tables["dtKycDoc"];
                if (dKYC != null && dKYC.Rows.Count > 0)
                {
                    userInfo.CustStatus = dKYC.Rows[0]["CustStatus"].ToString();
                    ViewBag.CustStatus = userInfo.CustStatus;
                }
                if (dDoc != null && dDoc.Rows.Count > 0)
                {
                    userInfo.PassportImage = dDoc.Rows[0]["PassportImage"].ToString();
                    ViewBag.PassportImage = userInfo.PassportImage;
                }

                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        #endregion

        #region "POST: KUKL ExecutePayment"
        [HttpPost]
        public async Task<ActionResult> KUKLExecutePayment(KUKLPaymentRequest kuklPayment)
        {
            var sessionUserDetails = SessionUserDetails();
            TempData["userType"] = sessionUserDetails.userType;

            this.ViewData["userType"] = this.TempData["userType"];
            ViewBag.UserType = this.TempData["userType"];
            ViewBag.Name = sessionUserDetails.name;

            string retoken = kuklPayment.tokenId;
            string reqToken = "";
            DataTable dtableVToken = ReqTokenUtils.GetReqToken(retoken);
            if (dtableVToken != null && dtableVToken.Rows.Count > 0)
            {
                reqToken = dtableVToken.Rows[0]["ReqVerifyToken"].ToString();
            }
            else if (dtableVToken.Rows.Count == 0)
            {
                reqToken = "0";
            }
            string BlockMessage = LoginUtils.GetMessage("01");
            if (reqToken == "0")
            {
                ReqTokenUtils.InsertReqToken(retoken);

                MNBalance availBaln = new MNBalance();
                DataTable dtableUser1 = AvailBalnUtils.GetAvailBaln(sessionUserDetails.clientCode);
                if (dtableUser1 != null && dtableUser1.Rows.Count > 0)
                {
                    availBaln.amount = dtableUser1.Rows[0]["AvailBaln"].ToString();
                    ViewBag.AvailBalnAmount = availBaln.amount;
                }

                //For Profile Picture
                UserInfo userInfo = new UserInfo();
                DataSet DSet = ProfileUtils.GetCusDetailProfileInfoDS(sessionUserDetails.clientCode);
                DataTable dKYC = DSet.Tables["dtKycDetail"];
                DataTable dDoc = DSet.Tables["dtKycDoc"];
                if (dKYC != null && dKYC.Rows.Count > 0)
                {
                    userInfo.CustStatus = dKYC.Rows[0]["CustStatus"].ToString();
                    ViewBag.CustStatus = userInfo.CustStatus;
                }
                if (dDoc != null && dDoc.Rows.Count > 0)
                {
                    userInfo.PassportImage = dDoc.Rows[0]["PassportImage"].ToString();
                    ViewBag.PassportImage = userInfo.PassportImage;
                }

                var kuklObject = new KUKLPaymentRequest
                {
                    connectionNo = (string)Session["connection_no"],
                    txnAmount = kuklPayment.txnAmount,
                    branchcode = (string)Session["S_KUKLBranchCode"],
                    module = (string)Session["S_KUKLBillPaymentMode"],
                    sc = kuklPayment.sc,
                    pin = kuklPayment.pin,
                    tokenId = kuklPayment.tokenId,
                    mobile = sessionUserDetails.userName

                };

                HttpResponseMessage _res = new HttpResponseMessage();
                string mobile = sessionUserDetails.userName; //mobile is username
                TraceIdGenerator _tig = new TraceIdGenerator();
                var tid = _tig.GenerateTraceID();

                //specify to use TLS 1.2 as default connection
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                // Serialize our concrete class into a JSON String
                var stringPayload = await Task.Run(() => JsonConvert.SerializeObject(kuklObject));

                // Wrap our JSON inside a StringContent which then can be used by the HttpClient class
                var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");
                using (HttpClient httpClient = new HttpClient())
                {
                    var UserName = ConfigurationManager.AppSettings["BasicAuthUserName"];
                    var UserPassword = ConfigurationManager.AppSettings["BasicAuthPassword"];
                    var APIBaseURL = ConfigurationManager.AppSettings["APIBaseURL"];

                    var byteArray = Encoding.ASCII.GetBytes(UserName + ":" + UserPassword);
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                   
                    _res = await httpClient.PostAsync(APIBaseURL + "KUKL/KUKLBillPayment", httpContent);
                    string responseBody = _res.StatusCode.ToString() + " ," + await _res.Content.ReadAsStringAsync();
                    string responseMessage = "";

                    if (responseBody == "InternalServerError ," || responseBody == "InternalServerError")
                    {
                        responseBody = "Internal Server Error";
                        return Json(new { responseCode = 500, responseText = responseBody },
                            JsonRequestBehavior.AllowGet);
                    }
                    _res.ReasonPhrase = responseBody;
                    string errorMessage = string.Empty;
                    int responseCode = 0;
                    string message = string.Empty;
                    string responsetext = string.Empty;
                    responseMessage = string.Empty;
                    bool result = false;
                    string ava = string.Empty;
                    string avatra = string.Empty;
                    string avamsg = string.Empty;
                    try
                    {
                        if (_res.IsSuccessStatusCode)
                        {
                            result = true;
                            responseCode = (int)_res.StatusCode;
                            responsetext = await _res.Content.ReadAsStringAsync();
                            message = _res.Content.ReadAsStringAsync().Result;
                            string respmsg = "";
                            string txnRespmsg = "";

                            if (!string.IsNullOrEmpty(message))
                            {
                                JavaScriptSerializer ser = new JavaScriptSerializer();
                                var json = ser.Deserialize<KUKLPaymentRequest>(responsetext);
                               
                              
                            }
                            return Json(new { responseCode = responseCode, responseText = responseMessage },
                            JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            result = false;
                            responseCode = (int)_res.StatusCode;
                            responsetext = await _res.Content.ReadAsStringAsync();
                            var json = JsonConvert.DeserializeObject<KUKLPaymentRequest>(responsetext);
                            
                            if (message != null)
                            {
                                return Json(new { responseCode = responseCode, responseText = message },
                            JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                dynamic item = JValue.Parse(message);
                                return Json(new { responseCode = responseCode, responseText = (string)item["StatusMessage"] },
                                JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        return Json(new { responseCode = "400", responseText = ex.Message },
                            JsonRequestBehavior.AllowGet);
                    }

                }
            }
            {
                return Json(new { responseCode = "400", responseText = "Please refresh the page again." },
                            JsonRequestBehavior.AllowGet);
            }

        }
        #endregion

        #region KUKLBranchList
        public async Task<List<KUKLBranch>> GetKUKLBranch()
        {
            //specify to use TLS 1.2 as default connection
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            using (var httpClient = new HttpClient())
            {
                var KUKLAuthorizationUserName = ConfigurationManager.AppSettings["BasicAuthUserName"];
                var KUKLAuthorizationPassword = ConfigurationManager.AppSettings["BasicAuthPassword"];
                var APIBaseURL = ConfigurationManager.AppSettings["APIBaseURL"];

                var byteArray = Encoding.ASCII.GetBytes(KUKLAuthorizationUserName + ":" + KUKLAuthorizationPassword);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                var httpResponse = await httpClient.GetAsync(APIBaseURL + "KUKL/GetKUKLBranch");
                var responseContent = await httpResponse.Content.ReadAsStringAsync();

                List<KUKLBranch> cipsChargeList = new List<KUKLBranch>();
                if (httpResponse.StatusCode == HttpStatusCode.OK && httpResponse.Content != null)
                {
                    KUKLBranch[] result = JsonConvert.DeserializeObject<KUKLBranch[]>(responseContent);
                    foreach (var item in result)
                    {
                        KUKLBranch kuklBranch = new KUKLBranch();
                        kuklBranch.branch = item.branch;
                        kuklBranch.branchcode = item.branchcode;

                        cipsChargeList.Add(kuklBranch);
                    }

                    return cipsChargeList;

                }
            }
            return null;
        }

        #endregion

        #region KUKLPaymentMode
        public async Task<List<KUKLModule>> KUKLPaymentMode()
        {
            //specify to use TLS 1.2 as default connection
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            using (var httpClient = new HttpClient())
            {
                var KUKLAuthorizationUserName = ConfigurationManager.AppSettings["BasicAuthUserName"];
                var KUKLAuthorizationPassword = ConfigurationManager.AppSettings["BasicAuthPassword"];
                var APIBaseURL = ConfigurationManager.AppSettings["APIBaseURL"];

                var byteArray = Encoding.ASCII.GetBytes(KUKLAuthorizationUserName + ":" + KUKLAuthorizationPassword);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                var httpResponse = await httpClient.GetAsync(APIBaseURL + "KUKL/KUKLPaymentMode");
                var responseContent = await httpResponse.Content.ReadAsStringAsync();

                List<KUKLModule> kuklModuleList = new List<KUKLModule>();
                if (httpResponse.StatusCode == HttpStatusCode.OK && httpResponse.Content != null)
                {
                    KUKLModule[] result = JsonConvert.DeserializeObject<KUKLModule[]>(responseContent);

                    foreach (var item in result)
                    {
                        KUKLModule kuklModule = new KUKLModule();
                        kuklModule.billPaymentMode = item.billPaymentMode;
                        kuklModule.paymentCode = item.paymentCode;

                        kuklModuleList.Add(kuklModule);
                    }

                    return kuklModuleList;

                }
            }
            return null;
        }
        #endregion

        #region Session User Details
        private UserSessionDetails SessionUserDetails()
        {
            UserSessionDetails details = new UserSessionDetails();

            details.userName = (string)Session["LOGGED_USERNAME"];
            details.clientCode = (string)Session["LOGGEDUSER_ID"];
            details.name = (string)Session["LOGGEDUSER_NAME"];
            details.userType = (string)Session["LOGGED_USERTYPE"];
            return details;
        }
        #endregion
    }
}