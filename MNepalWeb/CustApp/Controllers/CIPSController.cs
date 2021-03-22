using CustApp.App_Start;
using CustApp.Helper;
using CustApp.Models;
using CustApp.UserModels;
using CustApp.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using static CustApp.Models.CIPS;

namespace CustApp.Controllers
{
    public class CIPSController : Controller
    {
        DAL objdal = new DAL();
        // GET: CIPS
        #region GET: CIPS
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

                ConnectIPSTokenResponse connectIPSTokenResponse = await GetAuthToken();
                if (connectIPSTokenResponse.status == 200 || connectIPSTokenResponse.status == 0)
                {
                    var access_token = connectIPSTokenResponse.access_token;
                    Session["access_token"] = access_token;
                }
                else
                {
                    return RedirectToAction("Error500", "Error");
                }


                List<GetCharge> getCharges = await GetCharge();
                ViewBag.cipsCharges = getCharges;

                List<SelectListItem> getBankList = await GetBankList();
                ViewBag.getBankList = getBankList;


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

        #region "POST: CIPS Validation"
        [HttpPost]

        public async Task<ActionResult> CIPSValidate(ValidateCreditorBankAccount validateCreditorBankAccount)
        {
            var sessionUserDetails = SessionUserDetails();
            TempData["userType"] = sessionUserDetails.userType;

            this.ViewData["userType"] = this.TempData["userType"];
            ViewBag.UserType = this.TempData["userType"];
            ViewBag.Name = sessionUserDetails.name;

            string retoken = validateCreditorBankAccount.TokenUnique;
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


                

                HttpResponseMessage _res = new HttpResponseMessage();
                var cipsObject = new ValidateCreditorBankAccount
                {
                    bankId = validateCreditorBankAccount.bankId,
                    accountId = validateCreditorBankAccount.accountId,
                    accountName = validateCreditorBankAccount.accountName
                };

                //specify to use TLS 1.2 as default connection
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                // Serialize our concrete class into a JSON String
                var stringPayload = await Task.Run(() => JsonConvert.SerializeObject(cipsObject));
                // Wrap our JSON inside a StringContent which then can be used by the HttpClient class
                var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");
                using (HttpClient httpClient = new HttpClient())
                {
                    var UserName = ConfigurationManager.AppSettings["BasicAuthUserName"];
                    var UserPassword = ConfigurationManager.AppSettings["BasicAuthPassword"];
                    var APIBaseURL = ConfigurationManager.AppSettings["APIBaseURL"];

                    var byteArray = Encoding.ASCII.GetBytes(UserName + ":" + UserPassword);
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    httpClient.DefaultRequestHeaders.Add("token", Session["access_token"].ToString());

                    _res = await httpClient.PostAsync(APIBaseURL + "ConnectIPS/ValidateBankAccount", httpContent);
                    string responseBody = _res.StatusCode.ToString() + " ," + await _res.Content.ReadAsStringAsync();
                    _res.ReasonPhrase = responseBody;
                    string errorMessage = string.Empty;
                    int responseCode = 0;
                    string message = string.Empty;
                    string responsetext = string.Empty;
                    string responseMessage = string.Empty;
                    bool result = false;
                    string ava = string.Empty;
                    string avatra = string.Empty;
                    string avamsg = string.Empty;
                    int code = 0;
                    try
                    {
                        if (_res.IsSuccessStatusCode)
                        {
                            result = true;
                            responseCode = (int)_res.StatusCode;
                            responsetext = await _res.Content.ReadAsStringAsync();
                            message = _res.Content.ReadAsStringAsync().Result;
                            string respmsg = "";
                            string couponNo = "";
                            if (!string.IsNullOrEmpty(message))
                            {
                                JavaScriptSerializer ser = new JavaScriptSerializer();
                                var json = ser.Deserialize<ValidateCreditorBankAccount>(responsetext);
                                code = Convert.ToInt32(json.responseCode);
                                respmsg = json.responseMessage;
                            }
                            return Json(new { responseCode = code, responseText = respmsg},
                            JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            result = false;
                            responseCode = (int)_res.StatusCode;
                            responsetext = await _res.Content.ReadAsStringAsync();
                            JavaScriptSerializer ser = new JavaScriptSerializer();
                            var json = ser.Deserialize<CheckPin>(responsetext);

                            return Json(new { responseCode = responseCode, responseText = json.message },
                            JsonRequestBehavior.AllowGet);
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

        #region "POST: CIPS Payment"
        [HttpPost]

        public async Task<ActionResult> CIPSPayment(ConnectIPSDetail cips)
        {
            var sessionUserDetails = SessionUserDetails();
            TempData["userType"] = sessionUserDetails.userType;

            this.ViewData["userType"] = this.TempData["userType"];
            ViewBag.UserType = this.TempData["userType"];
            ViewBag.Name = sessionUserDetails.name;

            string retoken = cips.TokenUnique;
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

                RandomCodeGenerator randomCodeGenerator = new RandomCodeGenerator();

                Session["batchId"] = randomCodeGenerator.CreateRandomCode(20);
                Session["batchAmount"] = cips.amount;
                Session["batchCount"] = 1;
                Session["batchCrncy"] = "NPR";
                Session["categoryPurpose"] = "ECPG";
                Session["debtorAgent"] = "0501";
                Session["debtorBranch"] =int.Parse(cips.senderAccountNumber.Substring(0,3)).ToString();
                Session["debtorName"] = cips.senderAccountName;
                //Session["debtorName"] = Session["BankRegisterName"].ToString();
                Session["debtorAccount"] = cips.senderAccountNumber;
                Session["debtorIdType"] = randomCodeGenerator.CreateRandomCode(4);
                Session["debtorIdValue"] = randomCodeGenerator.CreateRandomCode(20);
                Session["debtorAddress"] = "Bhaktapur";
                Session["debtorPhone"] = "12345";
                Session["debtorMobile"] = "12345";
                Session["debtorEmail"] = "debtor@debtor.com";
                Session["bankName"] = cips.bankName;
                Session["branchName"] = cips.branchName;
                Session["transactionDetail"] = cips.transactionDetail;
                Session["instructionId"] = randomCodeGenerator.CreateRandomCode(30);
                Session["endToEndId"] = cips.transactionDetail;
                Session["amount"] = cips.amount;
                Session["creditorAgent"] = cips.bankId;
                Session["creditorBranch"] = cips.bankBranchId;
                Session["creditorName"] = cips.beneficiaryAccountName;
                Session["creditorAccount"] = cips.beneficiaryAccountNumber;
                Session["creditorIdType"] = randomCodeGenerator.CreateRandomCode(4);
                Session["creditorIdValue"] = randomCodeGenerator.CreateRandomCode(20);
                Session["creditorAddress"] = "Kathmandu";
                Session["creditorPhone"] = "+977";
                Session["creditorMobile"] = "+977";
                Session["creditorEmail"] = "creditor@creditor.com";
                Session["addenda1"] = 0405;
                Session["addenda2"] = DateTime.Now.ToString("yyyy-MM-dd");
                Session["addenda3"] = "Addenda info3";
                Session["addenda4"] = "Addenda info4";

                HttpResponseMessage _res = new HttpResponseMessage();
                var cipsObject = new CheckPin
                {
                    userName = sessionUserDetails.userName,
                    pin = cips.pin
                };

                //specify to use TLS 1.2 as default connection
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                // Serialize our concrete class into a JSON String
                var stringPayload = await Task.Run(() => JsonConvert.SerializeObject(cipsObject));
                // Wrap our JSON inside a StringContent which then can be used by the HttpClient class
                var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");
                using (HttpClient httpClient = new HttpClient())
                {
                    var UserName = ConfigurationManager.AppSettings["BasicAuthUserName"];
                    var UserPassword = ConfigurationManager.AppSettings["BasicAuthPassword"];
                    var APIBaseURL = ConfigurationManager.AppSettings["APIBaseURL"];

                    var byteArray = Encoding.ASCII.GetBytes(UserName + ":" + UserPassword);
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                    _res = await httpClient.PostAsync(APIBaseURL + "ConnectIPS/CheckPin", httpContent);
                    string responseBody = _res.StatusCode.ToString() + " ," + await _res.Content.ReadAsStringAsync();
                    _res.ReasonPhrase = responseBody;
                    string errorMessage = string.Empty;
                    int responseCode = 0;
                    string message = string.Empty;
                    string responsetext = string.Empty;
                    string responseMessage = string.Empty;
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
                            string couponNo = "";
                            if (!string.IsNullOrEmpty(message))
                            {
                                JavaScriptSerializer ser = new JavaScriptSerializer();
                                var json = ser.Deserialize<ConnectIPSResponse>(responsetext);
                                int code = Convert.ToInt32(json.cipsBatchResponse.responseCode);
                                respmsg = json.cipsBatchResponse.responseMessage;
                            }
                            return Json(new { responseCode = responseCode, responseText = respmsg, rechargePin = couponNo },
                            JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            result = false;
                            responseCode = (int)_res.StatusCode;
                            responsetext = await _res.Content.ReadAsStringAsync();
                            JavaScriptSerializer ser = new JavaScriptSerializer();
                            var json = ser.Deserialize<CheckPin>(responsetext);

                            return Json(new { responseCode = responseCode, responseText = json.message },
                            JsonRequestBehavior.AllowGet);
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

        #region "GET: CIPS Details"
        public ActionResult CIPSDetails()
        {
            var sessionUserDetails = SessionUserDetails();
            TempData["userType"] = sessionUserDetails.userType;

            if (TempData["userType"] != null)
            {
                this.ViewData["userType"] = this.TempData["userType"];
                ViewBag.UserType = this.TempData["userType"];
                ViewBag.Name = sessionUserDetails.name;

                ViewBag.SenderMobileNo = sessionUserDetails.userName;
                ViewBag.AccountName = Session["debtorName"];
                ViewBag.AccountNumber = Session["debtorAccount"];
                ViewBag.TransactionAmount = Session["batchAmount"];
                ViewBag.BeneficiaryAccountNumber = Session["creditorAccount"];
                ViewBag.BeneficiaryAccountName = Session["creditorName"];
                ViewBag.BankName = Session["bankName"];
                ViewBag.BranchName = Session["branchName"];
                ViewBag.Remarks = Session["transactionDetail"];

                //applicable charge
                if (ViewBag.TransactionAmount <= 500)
                {
                    ViewBag.Charge = 2;
                }
                if (ViewBag.TransactionAmount > 500 && ViewBag.TransactionAmount <= 5000)
                {
                    ViewBag.Charge = 5;
                }
                if (ViewBag.TransactionAmount > 5000 && ViewBag.TransactionAmount <= 50000)
                {
                    ViewBag.Charge = 10;
                }
                if (ViewBag.TransactionAmount > 50000)
                {
                    ViewBag.Charge = 15;
                }
                //applicable charge ends

                int id = TraceIdGenerator.GetID() + 1;
                string stringid = (id).ToString();//this.GetID() + 1
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

        #region "POST: CIPS ExecutePayment"
        [HttpPost]
        public async Task<ActionResult> CIPSExecutePayment(ConnectIPSDetail connectIPS)
        {
            var sessionUserDetails = SessionUserDetails();
            TempData["userType"] = sessionUserDetails.userType;

            this.ViewData["userType"] = this.TempData["userType"];
            ViewBag.UserType = this.TempData["userType"];
            ViewBag.Name = sessionUserDetails.name;

            string retoken = connectIPS.TokenUnique;
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


                Cipsbatchdetail cipsBatchDetail = new Cipsbatchdetail();
                cipsBatchDetail.batchId = Session["batchId"].ToString();
                

                decimal decimalRounded = Decimal.Parse(connectIPS.amount.ToString("0.00"));

                cipsBatchDetail.batchAmount = decimalRounded;
                cipsBatchDetail.batchCount = Convert.ToInt32(Session["batchCount"]);
                cipsBatchDetail.batchCrncy = Session["batchCrncy"].ToString();
                cipsBatchDetail.categoryPurpose = Session["categoryPurpose"].ToString();
                cipsBatchDetail.debtorAgent = Session["debtorAgent"].ToString();
                cipsBatchDetail.debtorBranch = Session["debtorBranch"].ToString();
                cipsBatchDetail.debtorName = Session["debtorName"].ToString();
                cipsBatchDetail.debtorAccount = Session["debtorAccount"].ToString();
                cipsBatchDetail.debtorIdType = Session["debtorIdType"].ToString();
                cipsBatchDetail.debtorIdValue = Session["debtorIdValue"].ToString();
                cipsBatchDetail.debtorAddress = Session["debtorAddress"].ToString();
                cipsBatchDetail.debtorPhone = Session["debtorPhone"].ToString();
                cipsBatchDetail.debtorMobile = Session["debtorMobile"].ToString();
                cipsBatchDetail.debtorEmail = Session["debtorEmail"].ToString();

                Cipstransactiondetaillist transactionDetailList = new Cipstransactiondetaillist();
                transactionDetailList.instructionId = Session["instructionId"].ToString();
                transactionDetailList.endToEndId = Session["endToEndId"].ToString();
                transactionDetailList.amount = decimalRounded;
                transactionDetailList.creditorAgent = Session["creditorAgent"].ToString();
                transactionDetailList.creditorBranch = Session["creditorBranch"].ToString();
                transactionDetailList.creditorName = Session["creditorName"].ToString();
                transactionDetailList.creditorAccount = Session["creditorAccount"].ToString();
                transactionDetailList.creditorIdType = Session["creditorIdType"].ToString();
                transactionDetailList.creditorIdValue = Session["creditorIdValue"].ToString();
                transactionDetailList.creditorAddress = Session["creditorAddress"].ToString();
                transactionDetailList.creditorPhone = Session["creditorPhone"].ToString();
                transactionDetailList.creditorMobile = Session["creditorMobile"].ToString();
                transactionDetailList.creditorEmail = Session["creditorEmail"].ToString();
                transactionDetailList.addenda1 = Convert.ToInt32(Session["addenda1"]);
                transactionDetailList.addenda2 = Session["addenda2"].ToString();
                transactionDetailList.addenda3 = Session["addenda3"].ToString();
                transactionDetailList.addenda4 = Session["transactionDetail"].ToString();

                List<Cipstransactiondetaillist> cipsTransactionDetailList = new List<Cipstransactiondetaillist>();
                cipsTransactionDetailList.Add(transactionDetailList);
                var cipsObject = new ConnectIPS
                {
                    cipsBatchDetail = cipsBatchDetail,
                    cipsTransactionDetailList = cipsTransactionDetailList,
                    username = sessionUserDetails.userName
                };

                HttpResponseMessage _res = new HttpResponseMessage();
                string mobile = sessionUserDetails.userName; //mobile is username
                TraceIdGenerator _tig = new TraceIdGenerator();
                var tid = _tig.GenerateTraceID();

                //specify to use TLS 1.2 as default connection
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                // Serialize our concrete class into a JSON String
                var stringPayload = await Task.Run(() => JsonConvert.SerializeObject(cipsObject));

                // Wrap our JSON inside a StringContent which then can be used by the HttpClient class
                var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");
                using (HttpClient httpClient = new HttpClient())
                {
                    var UserName = ConfigurationManager.AppSettings["BasicAuthUserName"];
                    var UserPassword = ConfigurationManager.AppSettings["BasicAuthPassword"];
                    var APIBaseURL = ConfigurationManager.AppSettings["APIBaseURL"];

                    var byteArray = Encoding.ASCII.GetBytes(UserName + ":" + UserPassword);
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    httpClient.DefaultRequestHeaders.Add("token", Session["access_token"].ToString());

                    _res = await httpClient.PostAsync(APIBaseURL + "ConnectIPS/BankToBank", httpContent);
                    string responseBody = _res.StatusCode.ToString() + " ," + await _res.Content.ReadAsStringAsync();
                    _res.ReasonPhrase = responseBody;
                    string errorMessage = string.Empty;
                    int responseCode = 0;
                    string message = string.Empty;
                    string responsetext = string.Empty;
                    string responseMessage = string.Empty;
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
                                var json = ser.Deserialize<ConnectIPSResponse>(responsetext);
                                int code = Convert.ToInt32(json.cipsBatchResponse.responseCode);
                                int txncode = Convert.ToInt32(json.cipsTxnResponseList.FirstOrDefault().responseCode);

                                respmsg = json.cipsBatchResponse.responseMessage;
                                txnRespmsg = json.cipsTxnResponseList.FirstOrDefault().responseMessage;
                                if (code == 0 && txncode == 0)
                                {
                                    responseCode = code;
                                    responseMessage = responsetext;
                                }
                                if (code != responseCode)
                                {
                                    responseCode = code;
                                    responseMessage = respmsg;
                                }
                                if (txncode != responseCode)
                                {
                                    responseCode = txncode;
                                    responseMessage = txnRespmsg;
                                }
                            }
                            return Json(new { responseCode = responseCode, responseText = responseMessage, blockMessage = BlockMessage },
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
                                return Json(new { responseCode = responseCode, responseText = responsetext, blockMessage = BlockMessage },
                            JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                dynamic item = JValue.Parse(message);
                                return Json(new { responseCode = responseCode, responseText = (string)item["StatusMessage"], blockMessage = BlockMessage },
                                JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        return Json(new { responseCode = "400", responseText = ex.Message, blockMessage = BlockMessage },
                            JsonRequestBehavior.AllowGet);
                    }

                }
            }
            {
                return Json(new { responseCode = "400", responseText = "Please refresh the page again.", blockMessage = BlockMessage },
                            JsonRequestBehavior.AllowGet);
            }

        }
        #endregion

        #region GetAuthToken
        public async Task<ConnectIPSTokenResponse> GetAuthToken()
        {
            try
            {
                string userName = (string)Session["LOGGED_USERNAME"];

                var cipsUsername = ConfigurationManager.AppSettings["CIPSUserName"];
                var cipsPassword = ConfigurationManager.AppSettings["CIPSPassword"];
                var content = new FormUrlEncodedContent(
                                       new KeyValuePair<string, string>[] {
                                new KeyValuePair<string, string>("grant_type", "password"),
                                new KeyValuePair<string, string>("username", cipsUsername),
                                new KeyValuePair<string, string>("password", cipsPassword)
                                       });

                //specify to use TLS 1.2 as default connection
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                using (var httpClient = new HttpClient())
                {
                    var UserName = ConfigurationManager.AppSettings["BasicAuthUserName"];
                    var UserPassword = ConfigurationManager.AppSettings["BasicAuthPassword"];
                    var APIBaseURL = ConfigurationManager.AppSettings["APIBaseURL"];

                    var byteArray = Encoding.ASCII.GetBytes(UserName + ":" + UserPassword);
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    var httpResponse = await httpClient.PostAsync(APIBaseURL + "ConnectIPS/CIPSToken", content);

                    ConnectIPSTokenResponse connectIPSToken = new ConnectIPSTokenResponse();
                    if (httpResponse.StatusCode == HttpStatusCode.InternalServerError)
                    {
                        var responseContent = await httpResponse.Content.ReadAsStringAsync();
                        var json = JsonConvert.DeserializeObject<ConnectIPSTokenResponse>(responseContent);

                        connectIPSToken.status = json.status;
                        connectIPSToken.error = json.error;
                        return connectIPSToken;
                    }

                    if (httpResponse.Content != null && httpResponse.StatusCode == HttpStatusCode.OK)
                    {
                        //response
                        var responseContent = await httpResponse.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<ConnectIPSTokenResponse>(responseContent);
                        connectIPSToken.access_token = result.access_token;
                        connectIPSToken.expires_in = result.expires_in;
                        connectIPSToken.refresh_token = result.refresh_token;
                        connectIPSToken.scope = result.scope;
                        connectIPSToken.token_type = result.token_type;

                        return connectIPSToken;

                    }
                }
                return null;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        #endregion

        #region CIPSChargeList
        public async Task<List<GetCharge>> GetCharge()
        {
            //specify to use TLS 1.2 as default connection
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            using (var httpClient = new HttpClient())
            {
                var UserName = ConfigurationManager.AppSettings["BasicAuthUserName"];
                var UserPassword = ConfigurationManager.AppSettings["BasicAuthPassword"];
                var APIBaseURL = ConfigurationManager.AppSettings["APIBaseURL"];

                var byteArray = Encoding.ASCII.GetBytes(UserName + ":" + UserPassword);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                httpClient.DefaultRequestHeaders.Add("token", Session["access_token"].ToString());

                var httpResponse = await httpClient.GetAsync(APIBaseURL + "ConnectIPS/GetCIPSChargeList");
                var responseContent = await httpResponse.Content.ReadAsStringAsync();

                List<GetCharge> cipsChargeList = new List<GetCharge>();
                if (httpResponse.Content != null)
                {
                    GetCharge[] result = JsonConvert.DeserializeObject<GetCharge[]>(responseContent);
                    foreach (var item in result)
                    {
                        GetCharge cIPSChargeList = new GetCharge();
                        cIPSChargeList.maxAmt = item.maxAmt;
                        cIPSChargeList.minChargeAmt = item.minChargeAmt;
                        cIPSChargeList.maxChargeAmt = item.maxChargeAmt;

                        cipsChargeList.Add(cIPSChargeList);
                    }

                    return cipsChargeList;

                }
            }
            return null;
        }

        #endregion

        #region CIPSBankList
        public async Task<List<SelectListItem>> GetBankList()
        {
            var teamList = new List<SelectListItem>();

            //specify to use TLS 1.2 as default connection
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            using (var httpClient = new HttpClient())
            {
                var UserName = ConfigurationManager.AppSettings["BasicAuthUserName"];
                var UserPassword = ConfigurationManager.AppSettings["BasicAuthPassword"];
                var APIBaseURL = ConfigurationManager.AppSettings["APIBaseURL"];

                var byteArray = Encoding.ASCII.GetBytes(UserName + ":" + UserPassword);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                httpClient.DefaultRequestHeaders.Add("token", Session["access_token"].ToString());

                var httpResponse = await httpClient.GetAsync(APIBaseURL + "ConnectIPS/GetCIPSBankList");
                var responseContent = await httpResponse.Content.ReadAsStringAsync();

                if (httpResponse.Content != null)
                {
                    BankList[] result = JsonConvert.DeserializeObject<BankList[]>(responseContent);
                    foreach (var item in result)
                    {
                        var listItem = new SelectListItem { Value = item.bankId, Text = item.bankName };
                        teamList.Add(listItem);
                    }
                    return teamList;
                }
            }
            return teamList;
        }

        #endregion


        #region CIPSBranchList
        public async Task<JsonResult> GetBranchList(string bankId)
        {
            var teamList = new List<SelectListItem>();

            Session["BankID"] = bankId;

            //specify to use TLS 1.2 as default connection
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            using (var httpClient = new HttpClient())
            {
                var UserName = ConfigurationManager.AppSettings["BasicAuthUserName"];
                var UserPassword = ConfigurationManager.AppSettings["BasicAuthPassword"];
                var APIBaseURL = ConfigurationManager.AppSettings["APIBaseURL"];

                var byteArray = Encoding.ASCII.GetBytes(UserName + ":" + UserPassword);

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                httpClient.DefaultRequestHeaders.Add("token", Session["access_token"].ToString());

                var httpResponse = await httpClient.GetAsync(APIBaseURL + "ConnectIPS/GetCIPSBranchList?bankId=" + bankId);
                var responseContent = await httpResponse.Content.ReadAsStringAsync();

                if (httpResponse.Content != null)
                {
                    BranchList[] result = JsonConvert.DeserializeObject<BranchList[]>(responseContent);
                    foreach (var item in result)
                    {
                        var listItem = new SelectListItem { Value = item.branchId, Text = item.branchName };
                        teamList.Add(listItem);
                    }
                    return Json(teamList, JsonRequestBehavior.AllowGet);

                }
            }
            return Json(teamList, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region GetLnkBankAcc
        public ActionResult GetLnkBankAcc()
        {
            var sessionUserDetails = SessionUserDetails();
            TempData["userType"] = sessionUserDetails.userType;

            if (TempData["userType"] != null)
            {
                ViewData["userType"] = TempData["userType"];
                ViewBag.UserType = TempData["userType"];
                ViewBag.Name = sessionUserDetails.name;
                string HasBankKYC = string.Empty;
                UserInfo userInfo = new UserInfo();

                //Check Link Bank Account
                DataTable dtableUserCheckLinkBankAcc = ProfileUtils.CheckLinkBankAcc(sessionUserDetails.userName);
                if (dtableUserCheckLinkBankAcc != null && dtableUserCheckLinkBankAcc.Rows.Count > 0)
                {
                    userInfo.BankAccountNumber = dtableUserCheckLinkBankAcc.Rows[0]["HasBankKYC"].ToString();

                    ViewBag.HasBankKYC = userInfo.BankAccountNumber;
                    HasBankKYC = ViewBag.HasBankKYC;

                }
                return Json(HasBankKYC, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
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