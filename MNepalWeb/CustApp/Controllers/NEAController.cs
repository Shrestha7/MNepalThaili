using CustApp.App_Start;
using CustApp.Helper;
using CustApp.Models;
using CustApp.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace CustApp.Controllers
{
    public class NEAController : Controller
    {
        //string BankBalance;
        DAL objdal = new DAL();
        #region "GET: NEAPayment"
        public async Task<ActionResult> Index()
        {
            string userName = (string)Session["LOGGED_USERNAME"];
            string clientCode = (string)Session["LOGGEDUSER_ID"];
            string name = (string)Session["LOGGEDUSER_NAME"];
            string userType = (string)Session["LOGGED_USERTYPE"];

            TempData["userType"] = userType;
            if (TempData["userType"] != null)
            {
                this.ViewData["userType"] = this.TempData["userType"];
                ViewBag.UserType = this.TempData["userType"];
                ViewBag.Name = name;

                NEABranchDetails neaBranchDetails = await NEABranchDetails();
                ViewBag.NEA = neaBranchDetails.branch;
                ViewBag.SenderMobileNo = userName;

                UserInfo userInfo = new UserInfo();


                MNBalance availBaln = new MNBalance();
                DataTable dtableUser1 = AvailBalnUtils.GetAvailBaln(clientCode);
                if (dtableUser1 != null && dtableUser1.Rows.Count > 0)
                {
                    availBaln.amount = dtableUser1.Rows[0]["AvailBaln"].ToString();

                    ViewBag.AvailBalnAmount = availBaln.amount;
                }


                //Check KYC
                DataTable dtableUserCheckKYC = ProfileUtils.CheckKYC(userName);
                if (dtableUserCheckKYC != null && dtableUserCheckKYC.Rows.Count > 0)
                {
                    userInfo.hasKYC = dtableUserCheckKYC.Rows[0]["hasKYC"].ToString();
                    userInfo.IsRejected = dtableUserCheckKYC.Rows[0]["IsRejected"].ToString();

                    ViewBag.IsRejected = userInfo.IsRejected;

                    ViewBag.hasKYC = userInfo.hasKYC;
                }

                //Check Link Bank Account
                DataTable dtableUserCheckLinkBankAcc = ProfileUtils.CheckLinkBankAcc(userName);
                if (dtableUserCheckLinkBankAcc != null && dtableUserCheckLinkBankAcc.Rows.Count > 0)
                {
                    userInfo.BankAccountNumber = dtableUserCheckLinkBankAcc.Rows[0]["HasBankKYC"].ToString();

                    ViewBag.HasBankKYC = userInfo.BankAccountNumber;
                }

                //For Profile Picture
                DataSet DSet = ProfileUtils.GetCusDetailProfileInfoDS(clientCode);
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

        #region "POST: NEACheckPayment"
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> NEAPayment(NEADetails _NEAft)
        {

            string userName = (string)Session["LOGGED_USERNAME"];
            string clientCode = (string)Session["LOGGEDUSER_ID"];
            string name = (string)Session["LOGGEDUSER_NAME"];
            string userType = (string)Session["LOGGED_USERTYPE"];

            TempData["userType"] = userType;

            this.ViewData["userType"] = this.TempData["userType"];
            ViewBag.UserType = this.TempData["userType"];
            ViewBag.Name = name;


            NEADetails neaBill = await NEABillPay();
            _NEAft.serviceId = neaBill.serviceId;
            _NEAft.serviceCode = neaBill.serviceCode;
            _NEAft.field1 = _NEAft.scNO;
            _NEAft.field2 = DateTime.Now.ToString("ddMMyyyyHHmmss");
            _NEAft.field3 = _NEAft.custromerId;
            _NEAft.field4 = "0";
            _NEAft.field5 = _NEAft.neaBranchCode;
            _NEAft.userName = userName;



            string retoken = _NEAft.tokenUnique;
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
                DataTable dtableUser1 = AvailBalnUtils.GetAvailBaln(clientCode);
                if (dtableUser1 != null && dtableUser1.Rows.Count > 0)
                {
                    availBaln.amount = dtableUser1.Rows[0]["AvailBaln"].ToString();

                    ViewBag.AvailBalnAmount = availBaln.amount;
                }

                //For Profile Picture
                UserInfo userInfo = new UserInfo();
                DataSet DSet = ProfileUtils.GetCusDetailProfileInfoDS(clientCode);
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
                Session["S_SCNo"] = _NEAft.scNO;
                Session["S_NEABranchName"] = _NEAft.neaBranchName;
                Session["S_NEABranchCode"] = _NEAft.neaBranchCode;
                Session["S_CustomerID"] = _NEAft.custromerId;
                //END Session
                HttpResponseMessage _res = new HttpResponseMessage();
                string mobile = userName; //mobile is username
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
                    var stringPayload = await Task.Run(() => JsonConvert.SerializeObject(_NEAft));
                    // Wrap our JSON inside a StringContent which then can be used by the HttpClient class
                    var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");

                    var byteArray = Encoding.ASCII.GetBytes(UserName + ":" + UserPassword);
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    var httpResponse = await httpClient.PostAsync(APIBaseURL + "NEA/NEABill", httpContent);

                    NEADetails neaBranch = new NEADetails();

                    if (httpResponse.Content != null && httpResponse.StatusCode == HttpStatusCode.OK)
                    {
                        //response
                        var responseContent = await httpResponse.Content.ReadAsStringAsync();
                        var json = JsonConvert.DeserializeObject<NeaBillResponse>(responseContent);


                    }



                    //string responseBody = _res.StatusCode.ToString() + " ," + await _res.Content.ReadAsStringAsync();
                    //_res.ReasonPhrase = responseBody;
                    string errorMessage = string.Empty;
                    int responseCode = 0;
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
                            string respmsg = "";

                            return Json(new { responseCode = responseCode, responseText = respmsg },
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

        #region "GET: NEADetails"

        public ActionResult Details()
        {
            string userName = (string)Session["LOGGED_USERNAME"];
            string clientCode = (string)Session["LOGGEDUSER_ID"];
            string name = (string)Session["LOGGEDUSER_NAME"];
            string userType = (string)Session["LOGGED_USERTYPE"];

            TempData["userType"] = userType;
            if (TempData["userType"] != null)
            {
                this.ViewData["userType"] = this.TempData["userType"];
                ViewBag.UserType = this.TempData["userType"];
                ViewBag.Name = name;

                ViewBag.SenderMobileNo = userName;
                //Session data set
                string S_SCNo = (string)Session["S_SCNo"];
                string S_NEABranchName = (string)Session["S_NEABranchName"];
                string S_NEABranchCode = (string)Session["S_NEABranchCode"];
                string S_CustomerID = (string)Session["S_CustomerID"];
                if ((S_SCNo == null) || (S_NEABranchName == null) || (S_CustomerID == null))
                {
                    return RedirectToAction("Index");

                }
                //End Session data set

                NEAFundTransfer NEAObj = new NEAFundTransfer();
                NEAObj.SCNo = S_SCNo;
                NEAObj.NEABranchName = S_NEABranchName;
                NEAObj.NEABranchCode = S_NEABranchCode;
                NEAObj.CustomerID = S_CustomerID;
                NEAObj.UserName = userName;
                NEAObj.ClientCode = clientCode;
                NEAObj.refStan = getrefStan(NEAObj);

                //Database Accessed
                NEAFundTransfer regobj = new NEAFundTransfer();
                DataSet DPaypointSet = PaypointUtils.GetNEADetails(NEAObj);
                DataTable dResponse = DPaypointSet.Tables["dtResponse"];
                //DataTable dPayment = DPaypointSet.Tables["dtPayment"];
                //End Database Accessed

                //ViewBag.rowNo = dPayment.Rows.Count;
                //int countROW = dPayment.Rows.Count;
                //List<NEAFundTransfer> ListDetails = new List<NEAFundTransfer>(countROW);
                if (dResponse != null && dResponse.Rows.Count > 0)
                {
                    regobj.SCNo = dResponse.Rows[0]["SCN"].ToString();
                    regobj.NEABranchCode = dResponse.Rows[0]["NEABranchCode"].ToString();
                    regobj.CustomerID = dResponse.Rows[0]["CustomerId"].ToString();
                    string[] additionalData = dResponse.Rows[0]["AdditionalData"].ToString().Split('^');
                    regobj.CustomerName = additionalData[0];
                    regobj.amount = additionalData[1];

                    //regobj.TotalAmountDue = dResponse.Rows[0]["amount"].ToString();

                }
                else
                {
                    return RedirectToAction("Index");
                }

                ViewBag.SCNo = regobj.SCNo;
                ViewBag.NEABranchName = getNEABranchName(regobj.NEABranchCode.ToString());
                ViewBag.NEABranchCode = regobj.NEABranchCode.ToString();
                ViewBag.CustomerID = regobj.CustomerID;
                ViewBag.CustomerName = regobj.CustomerName;
                
                ViewBag.TotalAmountDue = String.Format("{0:0.00}", Convert.ToDecimal(regobj.amount));


                Session["amount"] = regobj.amount;

                UserInfo userInfo = new UserInfo();

                MNBalance availBaln = new MNBalance();
                DataTable dtableUser1 = AvailBalnUtils.GetAvailBaln(clientCode);
                if (dtableUser1 != null && dtableUser1.Rows.Count > 0)
                {
                    availBaln.amount = dtableUser1.Rows[0]["AvailBaln"].ToString();

                    ViewBag.AvailBalnAmount = availBaln.amount;
                }


                //Check KYC
                DataTable dtableUserCheckKYC = ProfileUtils.CheckKYC(userName);
                if (dtableUserCheckKYC != null && dtableUserCheckKYC.Rows.Count > 0)
                {
                    userInfo.hasKYC = dtableUserCheckKYC.Rows[0]["hasKYC"].ToString();
                    userInfo.IsRejected = dtableUserCheckKYC.Rows[0]["IsRejected"].ToString();

                    ViewBag.IsRejected = userInfo.IsRejected;

                    ViewBag.hasKYC = userInfo.hasKYC;
                }

                //Check Link Bank Account
                DataTable dtableUserCheckLinkBankAcc = ProfileUtils.CheckLinkBankAcc(userName);
                if (dtableUserCheckLinkBankAcc != null && dtableUserCheckLinkBankAcc.Rows.Count > 0)
                {
                    userInfo.BankAccountNumber = dtableUserCheckLinkBankAcc.Rows[0]["HasBankKYC"].ToString();

                    ViewBag.HasBankKYC = userInfo.BankAccountNumber;
                }

                //For Profile Picture
                DataSet DSet = ProfileUtils.GetCusDetailProfileInfoDS(clientCode);
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

        #region "POST: NEA ExecutePayment"

        [HttpPost]
        public async Task<ActionResult> NEAExecutePayment(NEAFundTransfer nea)
        {

            //start for sercivecode=special1
            string serviceCodeTestServer = "0";
            serviceCodeTestServer = System.Web.Configuration.WebConfigurationManager.AppSettings["serviceCodeTestServer"];
            //end  for sercivecode=special1


            string userName = (string)Session["LOGGED_USERNAME"];
            string clientCode = (string)Session["LOGGEDUSER_ID"];
            string name = (string)Session["LOGGEDUSER_NAME"];
            string userType = (string)Session["LOGGED_USERTYPE"];

            TempData["userType"] = userType;



            this.ViewData["userType"] = this.TempData["userType"];
            ViewBag.UserType = this.TempData["userType"];
            ViewBag.Name = name;


            string retoken = nea.TokenUnique;
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
                DataTable dtableUser1 = AvailBalnUtils.GetAvailBaln(clientCode);
                if (dtableUser1 != null && dtableUser1.Rows.Count > 0)
                {
                    availBaln.amount = dtableUser1.Rows[0]["AvailBaln"].ToString();

                    ViewBag.AvailBalnAmount = availBaln.amount;
                }

                //For Profile Picture
                UserInfo userInfo = new UserInfo();
                DataSet DSet = ProfileUtils.GetCusDetailProfileInfoDS(clientCode);
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

                string S_SCNo = (string)Session["S_SCNo"];
                string S_NEABranchName = (string)Session["S_NEABranchName"];
                string S_CustomerID = (string)Session["S_CustomerID"];
                NEAFundTransfer NEAObj = new NEAFundTransfer();
                NEAObj.SCNo = S_SCNo;
                NEAObj.NEABranchCode = (string)Session["S_NEABranchCode"];
                NEAObj.CustomerID = S_CustomerID;
                NEAObj.UserName = userName;
                NEAObj.ClientCode = clientCode;


                string mobile = userName; //mobile is username
                TraceIdGenerator _tig = new TraceIdGenerator();
                var tid = _tig.GenerateTraceID();

                //specify to use TLS 1.2 as default connection
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                nea.mobile = mobile;
                nea.da = ConfigurationManager.AppSettings["DestinationNumber"];
                nea.destBranchCode = NEAObj.NEABranchCode;
                nea.tokenID = Session["TokenID"].ToString();
                nea.tid = tid;
                nea.pin = nea.TPin;
                nea.scn = nea.SCNo;
                nea.sc = nea.TransactionMedium;
                nea.merchantName = "nea";
                nea.merchantType = "nea";
                nea.amount = (string)Session["amount"];


                var stringPayload = await Task.Run(() => JsonConvert.SerializeObject(nea));
                // Wrap our JSON inside a StringContent which then can be used by the HttpClient class
                var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");


                using (var httpClient = new HttpClient())
                {
                    var UserName = ConfigurationManager.AppSettings["BasicAuthUserName"];
                    var UserPassword = ConfigurationManager.AppSettings["BasicAuthPassword"];
                    var APIBaseURL = ConfigurationManager.AppSettings["APIBaseURL"];

                    var byteArray = Encoding.ASCII.GetBytes(UserName + ":" + UserPassword);
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    var httpResponse = await httpClient.PostAsync(APIBaseURL + "NEA/NEABillPayment", httpContent);

                    NEABranchDetails neaBranch = new NEABranchDetails();

                    if (httpResponse.Content != null && httpResponse.StatusCode == HttpStatusCode.OK)
                    {
                        ////response
                        //var responseContent = await httpResponse.Content.ReadAsStringAsync();
                        string errorMessage = string.Empty;
                        int responseCode = 0;
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
                                //Session value removed
                                Session.Remove("S_SCNo");
                                Session.Remove("S_NEABranchName");
                                Session.Remove("S_CustomerID");

                                result = true;
                                responseCode = (int)httpResponse.StatusCode;
                                responsetext = await httpResponse.Content.ReadAsStringAsync();
                                message = httpResponse.Content.ReadAsStringAsync().Result;
                                string respmsg = "";
                                if (!string.IsNullOrEmpty(message))
                                {

                                    var responseMessage = JsonConvert.DeserializeObject(message);
                                    
                                    
                                }
                                //   return Json(new { responseCode = responseCode, responseText = respmsg },
                                return Json(new { responseCode = responseCode, responseText = respmsg, blockMessage = BlockMessage },

                              JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                result = false;
                                responseCode = (int)httpResponse.StatusCode;
                                responsetext = await httpResponse.Content.ReadAsStringAsync();
                                dynamic json = JValue.Parse(responsetext);
                                message = json.d;
                                if (message == null)
                                {
                                    // return Json(new { responseCode = responseCode, responseText = responsetext },
                                    return Json(new { responseCode = responseCode, responseText = responsetext, blockMessage = BlockMessage },
                                JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    dynamic item = JValue.Parse(message);

                                    //return Json(new { responseCode = responseCode, responseText = (string)item["StatusMessage"] },
                                    return Json(new { responseCode = responseCode, responseText = (string)item["StatusMessage"], blockMessage = BlockMessage },

                                  JsonRequestBehavior.AllowGet);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            // return Json(new { responseCode = "400", responseText = ex.Message },
                            return Json(new { responseCode = "400", responseText = ex.Message, blockMessage = BlockMessage },

                          JsonRequestBehavior.AllowGet);
                        }



                    }
                    else
                    {

                        int responseCode = (int)httpResponse.StatusCode;
                        string responsetext = await httpResponse.Content.ReadAsStringAsync();
                        var json = JsonConvert.DeserializeObject<JsonDetails>(responsetext);


                        // return Json(new { responseCode = responseCode, responseText = responsetext },
                        return Json(new { responseCode = json.StatusCode , responseText = json.StatusMessage, blockMessage = BlockMessage },
                    JsonRequestBehavior.AllowGet);

                    }
                }

            }
            else
            {
                // return Json(new { responseCode = "400", responseText = "Please refresh the page again." },
                return Json(new { responseCode = "400", responseText = "Please refresh the page again.", blockMessage = BlockMessage },
                            JsonRequestBehavior.AllowGet);
            }

        }

        #endregion

        #region Get NEA Branch Name
        public string getNEABranchName(string BranchCode)
        {
            string NEABranchName = "select NEABranchName from MNNEALocation where NEABranchCode='" + BranchCode + "'";
            DataTable dt = new DataTable();
            dt = objdal.MyMethod(NEABranchName);
            string BranchName = string.Empty;
            foreach (DataRow row in dt.Rows)
            {
                BranchName = row["NEABranchName"].ToString();
            }
            return BranchName;
        }
        #endregion

        #region Get NEA refStan From Response Table
        public string getrefStan(NEAFundTransfer NEAObj)
        {
            string Query_refStan = "select RetrievalReference from MNNEACheckBill where SCN='" + NEAObj.SCNo + "' AND UserName='" + NEAObj.UserName + "'";

            DataTable dt = new DataTable();
            dt = objdal.MyMethod(Query_refStan);
            string refStan = string.Empty;
            foreach (DataRow row in dt.Rows)
            {
                refStan = row["RetrievalReference"].ToString();
            }
            return refStan;
        }
        #endregion

        #region
        public async Task<NEABranchDetails> NEABranchDetails()
        {
            try
            {
                SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["MNepalDBConnectionString"].ConnectionString);
                string command = "select * from MNNEABranch order by Id desc";
                cn.Open();
                SqlDataAdapter da = new SqlDataAdapter(command, cn);
                DataSet ds = new DataSet();
                da.Fill(ds);

                List<NEADetails> neaDetailsList = new List<NEADetails>();

                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    NEADetails details = new NEADetails();

                    details.serviceCode = dr["ServiceCode"].ToString();
                    details.serviceId = dr["ServiceId"].ToString();
                    neaDetailsList.Add(details);

                }

                NEADetails neaBranchDetails = new NEADetails();
                foreach (var item in neaDetailsList)
                {
                    neaBranchDetails = new NEADetails
                    {
                        serviceCode = item.serviceCode,
                        serviceId = item.serviceId
                    };
                }


                // Serialize our concrete class into a JSON String
                var stringPayload = await Task.Run(() => JsonConvert.SerializeObject(neaBranchDetails));
                // Wrap our JSON inside a StringContent which then can be used by the HttpClient class
                var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");


                using (var httpClient = new HttpClient())
                {
                    var UserName = ConfigurationManager.AppSettings["BasicAuthUserName"];
                    var UserPassword = ConfigurationManager.AppSettings["BasicAuthPassword"];
                    var APIBaseURL = ConfigurationManager.AppSettings["APIBaseURL"];

                    var byteArray = Encoding.ASCII.GetBytes(UserName + ":" + UserPassword);
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    var httpResponse = await httpClient.PostAsync(APIBaseURL + "NEA/NEABranch", httpContent);

                    NEABranchDetails neaBranch = new NEABranchDetails();

                    if (httpResponse.Content != null && httpResponse.StatusCode == HttpStatusCode.OK)
                    {
                        //response
                        var responseContent = await httpResponse.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<NEABranchDetails>(responseContent);


                        return result;

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

        #region
        public async Task<NEADetails> NEABillPay()
        {
            try
            {
                SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["MNepalDBConnectionString"].ConnectionString);
                string command = "select * from MNNeaBillPaymentServiceDetails order by Id desc";
                cn.Open();
                SqlDataAdapter da = new SqlDataAdapter(command, cn);
                DataSet ds = new DataSet();
                da.Fill(ds);

                List<NEADetails> neaDetailsList = new List<NEADetails>();

                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    NEADetails details = new NEADetails();

                    details.serviceCode = dr["ServiceCode"].ToString();
                    details.serviceId = dr["ServiceId"].ToString();
                    neaDetailsList.Add(details);

                }
                NEADetails neaBranchDetails = new NEADetails();
                foreach (var item in neaDetailsList)
                {
                    neaBranchDetails = new NEADetails
                    {
                        serviceCode = item.serviceCode,
                        serviceId = item.serviceId
                    };
                }

                return neaBranchDetails;
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion
    }
}