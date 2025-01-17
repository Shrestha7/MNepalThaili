﻿using ThailiMerchantApp.Models;
using ThailiMerchantApp.Settings;
using ThailiMerchantApp.Utilities;
using System;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using System.Threading.Tasks;
using System.Net.Http;
using ThailiMerchantApp.Helper;
using System.IO;
using System.Web.Script.Serialization;
using Newtonsoft.Json.Linq;
using ThailiMerchantApp.App_Start;
using System.Net;
using System.Net.Sockets;
using System.Management;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;

namespace ThailiMerchantApp.Controllers
{
    public class LoginController : Controller
    {
        string IPAddresses;
        // public object CssSetting { get; private set; }

        // GET: Login
        [HttpGet]
        public ActionResult Index()
        {
            if (this.TempData["login_message"] != null)
            {
                this.ViewData["login_message"] = this.TempData["login_message"];
                this.ViewData["message_class"] = this.TempData["message_class"];
                this.ViewData["message_topic"] = this.TempData["message_topic"];
            }

            if (Session["LOGGED_USERNAME"] == null)
            {
                return View("Index");
            }
            else
            {
                TempData["userType"] = Session["LOGGED_USERTYPE"];

                //if (TempData["userType"].ToString() == "user")
                //{
                //    return RedirectToAction("Index", "UserDashboard");
                //}
                //else
                //if (TempData["userType"].ToString() == "agent")
                //{
                //    return RedirectToAction("Index", "AgentDashboard");
                //}
                //else if (TempData["userType"].ToString() == "admin")
                //{
                //    return RedirectToAction("Index", "AdminDashboard");
                //}
                //else if (TempData["userType"].ToString() == "superadmin")
                //{
                //    return RedirectToAction("Index", "SuperAdminDashboard");
                //}
                if (TempData["userType"].ToString() == "merchant")
                {
                    return RedirectToAction("Index", "MerchantDashboard");
                }
                else
                {
                    return View("Index");
                }
            }



        }

        //[ValidateAntiForgeryToken]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Index(FormCollection collection)
        {
            string userName = collection["txtUserName"] ?? string.Empty;
            string passWord = collection["txtPassword"] ?? string.Empty;

            bool result = false;
            string StatusCheck = null;
            string displayMessage = null;
            string messageClass = string.Empty;
            string messageTopic = string.Empty;
            bool ForceUserChangePW = true;

            try
            {

                if ((string.IsNullOrEmpty(userName)) || (string.IsNullOrEmpty(passWord)))
                {
                    this.ViewData["login_message"] = "UserName/Password is Empty";
                    return this.View();
                }
                else if ((userName != string.Empty) && (passWord != string.Empty))
                {
                    this.ViewData["userName"] = userName;
                    if (Session["ID"] == null)
                    {
                        // DataTable dtableMobileNo = RegisterUtils.GetCheckUserName(userName);//RegisterUtils.GetCheckMobileNo(userName);
                        DataTable dtableMobileNo = RegisterUtils.GetCheckMerchantNo(userName);


                        if (dtableMobileNo.Rows.Count == 0)
                        {
                            result = false;
                            //displayMessage = "User doesnot exist."; 
                            messageTopic = "Unregistered Number!";
                            displayMessage = "Please enter a valid mobile number.";

                            messageClass = CssSetting.FailedMessageClass;
                        }
                        else
                        {

                            //for block message
                            DataTable dtableBlockRemarks = LoginUtils.GetBlockRemarks(userName, passWord);
                            string BlockRemarks = dtableBlockRemarks.Rows[0]["BlockRemarks"].ToString();
                            string Status = dtableBlockRemarks.Rows[0]["Status"].ToString();
                            string IsApproved = dtableBlockRemarks.Rows[0]["IsApproved"].ToString();

                            if (Status == "Blocked" && IsApproved == "Approve")
                            {
                                StatusCheck = "Blocked";
                                messageTopic = "Blocked";
                                //displayMessage = BlockRemarks;

                                displayMessage = "Merchant blocked, contact administrator";
                            }


                            else
                            {

                                DataTable dtableBlockUserWrongPwd = LoginUtils.GetBlockTime(userName);

                                if (dtableBlockUserWrongPwd != null && dtableBlockUserWrongPwd.Rows.Count > 0)
                                {
                                    DataTable dtableUser = LoginUtils.GetLoginInfo(userName, HashAlgo.Hash(passWord));//cipheredPassword);//passWord;cipheredPassword

                                    if (dtableUser != null && dtableUser.Rows.Count > 0)
                                    {
                                        dtableBlockUserWrongPwd = LoginUtils.ResetPasswordTry(userName);
                                        string clientCode = dtableUser.Rows[0]["ClientCode"].ToString();
                                        string name = dtableUser.Rows[0]["Name"].ToString();
                                        string userType = dtableUser.Rows[0]["UserType"].ToString();
                                        //string UserName= dtableUser.Rows[0]["UserName"].ToString();
                                        string UserBranch = dtableUser.Rows[0]["UserBranchCode"].ToString();
                                        string BranchName = dtableUser.Rows[0]["UserBranchName"].ToString();
                                        bool COC = false;
                                        Session["LOGGED_USERNAME"] = userName;
                                        Session["LOGGEDUSER_ID"] = clientCode;//LoggedUserID
                                        Session["LOGGEDUSER_NAME"] = name;//LoggedUserFullname
                                        Session["LOGGED_USERTYPE"] = userType;

                                        string merchantId = string.Empty;
                                        Session["LOGGED_USERNAME"] = userName;
                                        Session["LOGGEDUSER_ID"] = clientCode;//LoggedUserID
                                        Session["LOGGEDUSER_NAME"] = name;//LoggedUserFullname
                                        Session["LOGGED_USERTYPE"] = userType;
                                        Session["UserName"] = userName;
                                        Session["UserBranch"] = UserBranch;
                                        Session["BranchName"] = BranchName;
                                        Session["COC"] = COC;
                                        Session["Merchant_ID"] = merchantId;
                                        Session["UniqueId"] = string.Join("", DateTime.Now.Ticks.ToString().Select(c => ((int)c).ToString("X2"))) + string.Join("", userName.Select(c => ((int)c).ToString("X2")));

                                        var featuredUserInfo = new UserInfo
                                        {
                                            UserName = userName,
                                            ClientCode = clientCode,
                                            Name = name,
                                            UserType = userType,
                                            MerchantId = merchantId
                                        };
                                        FormsAuthentication.SetAuthCookie(userName, false);

                                        ViewData["FeaturedUserInfo"] = featuredUserInfo;
                                        ViewBag.Product = featuredUserInfo;
                                        TempData["FeaturedUserInfo"] = featuredUserInfo;
                                        TempData["userType"] = userType;

                                        /*Stamp Login*/
                                        MNAdminLog log = new MNAdminLog();
                                        log.IPAddress = this.Request.UserHostAddress;
                                        log.URL = this.Request.Url.PathAndQuery;
                                        log.Message = " ";
                                        log.Action = "LOGIN";
                                        log.UniqueId = Session["UniqueId"].ToString() + "|" + HttpContext.Session.SessionID;
                                        log.Branch = Session["UserBranch"].ToString();
                                        log.UserId = Session["UserName"].ToString();
                                        log.UserType = Session["LOGGED_USERTYPE"].ToString();
                                        log.TimeStamp = DateTime.Now;

                                        //For User IP address etc
                                        string macAdd = string.Empty;
                                        ManagementObjectSearcher query = null;
                                        ManagementObjectCollection queryCollection = null;
                                        string sIPAddress = string.Empty;
                                        try
                                        {
                                            query = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapterConfiguration WHERE IPEnabled = 'TRUE'");
                                            queryCollection = query.Get();
                                            foreach (ManagementObject mo in queryCollection)
                                            {
                                                if (mo["MacAddress"] != null)
                                                {
                                                    macAdd = mo["MacAddress"].ToString();
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                        }

                                        ViewBag.macAdd = macAdd;

                                        string ipaddress; string localaddress; string allhttp; string usermerchant;
                                        string remoteaddr; string REMOTE_HOST; string REMOTE_PORT;
                                        ipaddress = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                                        localaddress = Request.ServerVariables["LOCAL_ADDR"];
                                        usermerchant = Request.ServerVariables["HTTP_USER_AGENT"];
                                        remoteaddr = Request.ServerVariables["REMOTE_ADDR"];
                                        REMOTE_HOST = Request.ServerVariables["REMOTE_HOST"];
                                        REMOTE_PORT = Request.ServerVariables["REMOTE_PORT"];

                                        if (ipaddress == "" || ipaddress == null)
                                        {
                                            ipaddress = Request.ServerVariables["REMOTE_ADDR"];
                                        }

                                        ViewBag.ipaddress = ipaddress.ToString();
                                        ViewBag.LOCAL = localaddress.ToString();
                                        ViewBag.usermerchant = usermerchant.ToString();

                                        ViewBag.remoteaddr = remoteaddr.ToString();
                                        ViewBag.REMOTE_HOST = REMOTE_HOST.ToString();
                                        ViewBag.REMOTE_PORT = REMOTE_PORT.ToString();

                                        string HostName = Dns.GetHostName(); // Retrive the Name of HOST  
                                        ViewBag.HostName = HostName; //"Host Name of machine ="
                                        IPAddress[] ipaddres = Dns.GetHostAddresses(HostName);
                                        ViewBag.allhttp = ipaddres[1].ToString(); //"IPv4 of Machine is "


                                        IPHostEntry ipEntry = Dns.GetHostEntry(HostName);
                                        IPAddress[] addr = ipEntry.AddressList;
                                        string ips = addr[1].ToString();
                                        ViewBag.myIPv6 = ips.ToString();

                                        string macAddresses = string.Empty;
                                        string gatewayip = string.Empty;
                                        foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
                                        {
                                            if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                                            {
                                                Console.WriteLine(ni.Name);
                                                foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                                                {
                                                    if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                                                    {
                                                        ViewBag.IPv4 = ip.Address.ToString();
                                                    }
                                                }
                                            }
                                            if (ni.OperationalStatus == OperationalStatus.Up)
                                            {
                                                foreach (GatewayIPAddressInformation d in ni.GetIPProperties().GatewayAddresses)
                                                {
                                                    gatewayip = d.Address.ToString();
                                                }
                                                macAddresses += ni.GetPhysicalAddress().ToString();
                                                break;
                                            }
                                        }
                                        ViewBag.macAddresses = macAddresses;
                                        ViewBag.gatewayip = gatewayip;

                                        //string privIP = collection["privateip"] ?? string.Empty;

                                        //added for mac address
                                        string privateip = collection["privateip"] ?? string.Empty;
                                        Session["privateip"] = privateip;
                                        //string getmacaddress = GetMacUsingARP(privateip);


                                        IPAddresses = GetIPAddress();
                                        ViewBag.IPAddresses = IPAddresses;
                                        //End User IP address etc
                                        log.ClientDetails = ViewBag.macAdd + ";" + ViewBag.ipaddress + ";" + ViewBag.LOCAL + ";" + ViewBag.usermerchant + ";" + ViewBag.remoteaddr + ";" + ViewBag.REMOTE_HOST + ";" + ViewBag.REMOTE_PORT + ";" + ViewBag.HostName + ";" + ViewBag.myIPv6 + ";" + ViewBag.gatewayip + ";" + ViewBag.IPAddresses;

                                        log.PrivateIP = privateip;

                                        LoginUtils.LogAction(log);
                                        Session["TokenID"] = log.UniqueId;
                                        if ((userType.ToUpper() == "MERCHANT" || userType.ToUpper() == "ADMIN") && ForceUserChangePW)
                                        {
                                            ViewModel.ResetVM RVM = new ViewModel.ResetVM();
                                            bool IsPassChanged = dtableUser.Rows[0]["PassChanged"].ToString() == "T" ? true : false;
                                            bool IsPinChanged = dtableUser.Rows[0]["PinChanged"].ToString() == "T" ? true : false;
                                            bool IsFirstLogin = dtableUser.Rows[0]["IsFirstLogin"].ToString() == "T" ? true : false;
                                            RVM.ClientCode = clientCode;
                                            RVM.PhoneNumber = dtableUser.Rows[0]["ContactNumber1"].ToString();
                                            if (IsFirstLogin)
                                            {
                                                RVM.Message = "You are required to change your Password and T-PIN";
                                                RVM.Mode = "FL"; //Short for First Login
                                                return View("Reset", RVM);

                                            }
                                            if (!IsPinChanged)
                                            {
                                                RVM.Message = "You are required to change your T-PIN";
                                                RVM.Mode = "PN"; //Short for Pin
                                                return View("Reset", RVM);
                                            }
                                            if (!IsPassChanged)
                                            {
                                                RVM.Message = "You are required to change your Password.";
                                                RVM.Mode = "PW"; //Short for Password
                                                return View("Reset", RVM);
                                            }
                                        }

                                        //if (TempData["userType"].ToString() == "user")
                                        //{
                                        //    return RedirectToAction("Index", "UserDashboard");
                                        //}
                                        //else 
                                        //if (TempData["userType"].ToString() == "agent")
                                        //{
                                        //    return RedirectToAction("Index", "AgentDashboard");
                                        //}
                                        //else if (TempData["userType"].ToString() == "admin")
                                        //{
                                        // return RedirectToAction("Index", "AdminDashboard");
                                        //}
                                        //else if (TempData["userType"].ToString() == "superadmin")
                                        //{
                                        // return RedirectToAction("Index", "SuperAdminDashboard");
                                        //}
                                        if (TempData["userType"].ToString() == "merchant")
                                        {
                                            return RedirectToAction("Index", "MerchantDashboard");
                                        }
                                    }
                                    else
                                    {
                                        dtableBlockUserWrongPwd = LoginUtils.BlockUserWrongPwd(userName);

                                        Session["UniqueId"] = string.Join("", DateTime.Now.Ticks.ToString().Select(c => ((int)c).ToString("X2"))) + string.Join("", userName.Select(c => ((int)c).ToString("X2")));

                                        MNAdminLog log = new MNAdminLog();
                                        log.IPAddress = this.Request.UserHostAddress;
                                        log.URL = this.Request.Url.PathAndQuery;
                                        log.Message = " ";
                                        log.Action = "LOGIN_ERROR";
                                        log.UniqueId = Session["UniqueId"].ToString() + "|" + HttpContext.Session.SessionID;
                                        log.Branch = " ";
                                        log.UserId = userName;
                                        log.UserType = "merchant";
                                        log.TimeStamp = DateTime.Now;

                                        //For User IP address etc
                                        string macAdd = string.Empty;
                                        ManagementObjectSearcher query = null;
                                        ManagementObjectCollection queryCollection = null;
                                        string sIPAddress = string.Empty;
                                        try
                                        {
                                            query = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapterConfiguration WHERE IPEnabled = 'TRUE'");
                                            queryCollection = query.Get();
                                            foreach (ManagementObject mo in queryCollection)
                                            {
                                                if (mo["MacAddress"] != null)
                                                {
                                                    macAdd = mo["MacAddress"].ToString();
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                        }

                                        ViewBag.macAdd = macAdd;

                                        string ipaddress; string localaddress; string allhttp; string usermerchant; string remoteaddr; string REMOTE_HOST; string REMOTE_PORT;
                                        ipaddress = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                                        localaddress = Request.ServerVariables["LOCAL_ADDR"];
                                        usermerchant = Request.ServerVariables["HTTP_USER_AGENT"];
                                        remoteaddr = Request.ServerVariables["REMOTE_ADDR"];
                                        REMOTE_HOST = Request.ServerVariables["REMOTE_HOST"];
                                        REMOTE_PORT = Request.ServerVariables["REMOTE_PORT"];

                                        if (ipaddress == "" || ipaddress == null)
                                        {
                                            ipaddress = Request.ServerVariables["REMOTE_ADDR"];
                                        }

                                        ViewBag.ipaddress = ipaddress.ToString();
                                        ViewBag.LOCAL = localaddress.ToString();
                                        ViewBag.usermerchant = usermerchant.ToString();

                                        ViewBag.remoteaddr = remoteaddr.ToString();
                                        ViewBag.REMOTE_HOST = REMOTE_HOST.ToString();
                                        ViewBag.REMOTE_PORT = REMOTE_PORT.ToString();

                                        string HostName = Dns.GetHostName(); // Retrive the Name of HOST  
                                        ViewBag.HostName = HostName; //"Host Name of machine ="
                                        IPAddress[] ipaddres = Dns.GetHostAddresses(HostName);
                                        ViewBag.allhttp = ipaddres[1].ToString(); //"IPv4 of Machine is "


                                        IPHostEntry ipEntry = Dns.GetHostEntry(HostName);
                                        IPAddress[] addr = ipEntry.AddressList;
                                        string ips = addr[1].ToString();
                                        ViewBag.myIPv6 = ips.ToString();

                                        string macAddresses = string.Empty;
                                        string gatewayip = string.Empty;
                                        foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
                                        {
                                            if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                                            {
                                                Console.WriteLine(ni.Name);
                                                foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                                                {
                                                    if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                                                    {
                                                        ViewBag.IPv4 = ip.Address.ToString();
                                                    }
                                                }
                                            }
                                            if (ni.OperationalStatus == OperationalStatus.Up)
                                            {
                                                foreach (GatewayIPAddressInformation d in ni.GetIPProperties().GatewayAddresses)
                                                {
                                                    gatewayip = d.Address.ToString();
                                                }
                                                macAddresses += ni.GetPhysicalAddress().ToString();
                                                break;
                                            }
                                        }
                                        ViewBag.macAddresses = macAddresses;
                                        ViewBag.gatewayip = gatewayip;

                                        //added for mac address
                                        string privateip = collection["privateip"] ?? string.Empty;
                                        //string getmacaddress = GetMacUsingARP(privateip);


                                        IPAddresses = GetIPAddress();
                                        ViewBag.IPAddresses = IPAddresses;
                                        //End User IP address etc
                                        log.ClientDetails = log.ClientDetails = ViewBag.macAdd + ";" + ViewBag.ipaddress + ";" + ViewBag.LOCAL + ";" + ViewBag.usermerchant + ";" + ViewBag.remoteaddr + ";" + ViewBag.REMOTE_HOST + ";" + ViewBag.REMOTE_PORT + ";" + ViewBag.HostName + ";" + ViewBag.myIPv6 + ";" + ViewBag.macAddresses + ";" + ViewBag.gatewayip + ";" + ViewBag.IPAddresses;

                                        log.PrivateIP = privateip;

                                        LoginUtils.LogAction(log);
                                        result = false;
                                        //displayMessage = "\n Please make sure that the username and the password is Correct !!";
                                        displayMessage = "\n Enter valid mobile number and password.";
                                        messageTopic = "Invalid Information!";
                                        messageClass = CssSetting.FailedMessageClass;
                                    }
                                }
                                else
                                {
                                    //displayMessage = "You cannot log in at the moment!";
                                    //  displayMessage = "Invalid Login! You have already attempt 3 times with wrong password,Please try again after 10 minutes";
                                    displayMessage = LoginUtils.GetMessage("03");

                                    messageClass = CssSetting.FailedMessageClass;
                                }


                            }


                        }

                    }
                }
            }
            catch (Exception ex)
            {
                displayMessage = ex.Message + "\n Please Contact to the administrator !!";
                messageClass = CssSetting.FailedMessageClass;
            }
            //this.TempData["login_message"] = result
            //                                      ? "" + displayMessage
            //                                      : "Invalid Login! " + displayMessage;


            if (StatusCheck == "Blocked")
            {
                this.TempData["login_message"] = result
                                                    ? "" + displayMessage
                                                    : displayMessage;

            }
            else
            {
                this.TempData["login_message"] = result
                                                     ? "" + displayMessage
                                                     : "" + displayMessage;  //"Invalid Login!"
            }



            this.TempData["message_class"] = result ? "success_info" : "failed_info";
            this.TempData["message_topic"] = messageTopic;
            return RedirectToAction("Index");
        }


        public ActionResult CheckUserName(string UserName)
        {
            if ((UserName != "") || (UserName != null))
            {
                string userName = string.Empty;
                DataTable dtUserName = RegisterUtils.GetCheckUserName(UserName);
                if (dtUserName != null && dtUserName.Rows.Count > 0)
                {
                    userName = "Success";
                }
                else
                {
                    userName = "User Doesnot Exist !";
                }

                return Json(userName, JsonRequestBehavior.AllowGet);
            }
            else
                return View();

        }
        [HttpGet]
        public ActionResult Reset()
        {
            if (this.ViewData["login_message"] != null)
            {
                this.ViewData["login_message"] = this.TempData["login_message"];
                this.ViewData["message_class"] = this.TempData["message_class"];
            }
            return RedirectToAction("Index", "Login");
        }


        [HttpPost]
        public ActionResult Change(ViewModel.ResetVM model, string RetypePin, string RetypePassword)
        {

            try
            {

                string Message = "";

                var OPin = string.Empty;
                var OPass = string.Empty;
                if (model.Mode != null)
                {
                    var Cred = LoginUtils.CheckCred(model);
                    if (Cred.Count > 0)
                    {
                        OPin = Cred["PIN"];
                        OPass = Cred["Password"];
                    }
                }
                else
                {
                    return RedirectToAction("Index", "Login");
                }
                if (model.Mode.ToUpper() == "FL")
                {
                    PasswordValidator validate = new PasswordValidator();
                    if (!validate.IsValid(model.Password))
                    {
                        ModelState.AddModelError("Password", "Invalid Format");

                    }
                    if (HashAlgo.Hash(model.OldPin) != OPin)
                    {
                        ModelState.AddModelError("OldPin", "Incorrect Old Pin");
                        //TempData["login_message"] = "Error:Incorrect Old Pin";
                        //TempData["message_class"] = CssSetting.FailedMessageClass;
                        //return View("Reset", model);
                    }
                    if (model.Pin != RetypePin)
                    {
                        ModelState.AddModelError("Pin", "Entered Pin Do not Match");

                    }
                    if (HashAlgo.Hash(model.OldPassword) != OPass)
                    {
                        ModelState.AddModelError("OldPassword", "Incorrect Old Password");

                    }
                    if (model.Password != RetypePassword)
                    {
                        ModelState.AddModelError("Password", "Entered password Do not Match");

                    }

                    //if(model.OldPassword != OPass || model.OldPin != OPin || model.Pin=="" || model.OldPassword =="" || model.Password =="" || model.OldPin =="")
                    //{
                    //    TempData["login_message"] = "Entered Old Pin/Old Password is incorrect";
                    //    return View("Reset", model);
                    //}

                    if (!ModelState.IsValid)
                    {
                        return View("Reset", model);
                    }

                    TempData["login_message"] = "Your Password/T-Pin has been successfully changed.";

                    /*Sms Message for  pin and Password Reset on first login */
                    Message = "Dear Customer," + "\n";
                    Message += " Your Pin and password has been changed"
                        + "." + "\n" + "Thank You";
                    Message += "-NIBL Thaili";


                }
                else if (model.Mode.ToUpper() == "PN") //Pin Reset 
                {
                    if (HashAlgo.Hash(model.OldPin) != OPin)
                    {
                        ModelState.AddModelError("OldPin", "Incorrect Pin Do not Match");
                    }
                    if (model.Pin != RetypePin)
                    {
                        ModelState.AddModelError("Pin", "Entered Pin Do not Match");
                    }
                    if (!ModelState.IsValid)
                    {
                        return View("Reset", model);
                    }

                    TempData["login_message"] = "Your T-Pin has been successfully changed.";

                    /*Sms Message for Pin Reset */
                    Message = "Dear Customer," + "\n";
                    Message += " Your Pin has been changed"
                        + "." + "\n" + "Thank You";
                    Message += "-NIBL Thaili";
                }

                else if (model.Mode.ToUpper() == "PW") //Password Reset 
                {

                    PasswordValidator validate = new PasswordValidator();
                    if (!validate.IsValid(model.Password))
                    {
                        ModelState.AddModelError("Password", "Invalid Format");
                    }
                    if (HashAlgo.Hash(model.OldPassword) != OPass)
                    {
                        ModelState.AddModelError("OldPassword", "Incorrect Old Password");
                    }
                    if (model.Password != RetypePassword)
                    {
                        ModelState.AddModelError("Password", "Entered password Do not Match");
                    }
                    if (!ModelState.IsValid)
                    {
                        return View("Reset", model);
                    }

                    TempData["login_message"] = "Your Password has been successfully changed.";

                    /*Sms Message for Password Reset */
                    Message = "Dear Customer," + "\n";
                    Message += " Your Password has been changed"
                        + "." + "\n" + "Thank You";
                    Message += "-NIBL Thaili";
                }


                if (LoginUtils.ResetFLogin(model))
                {
                    /*Sms Start*/
                    /* SMSUtils SMS = new SMSUtils();
                     SMS.SendSMS(Message, model.PhoneNumber);*/
                    /*Sms End*/
                    TempData["message_class"] = CssSetting.SuccessMessageClass;
                    return RedirectToAction("Index", "Login");
                }
                else
                {
                    TempData["login_messsage"] = "Error:: Unable to Process your request,Please refresh the page and try again";
                    TempData["message_class"] = CssSetting.FailedMessageClass;
                    return View("Reset", model);
                }


            }
            catch (Exception ex)
            {
                TempData["login_message"] = "Error:: " + ex.Message + ". Please Try again.";
                TempData["message_class"] = CssSetting.FailedMessageClass;
                return RedirectToAction("Index", "Login");
            }

        }

        //start milyako
        public async Task<ActionResult> BankQuery()
        {
            string username = (string)Session["LOGGED_USERNAME"];
            string pin = "";
            if ((username != "") || (username != null))
            {

                string result = string.Empty;
                DataTable dtableMobileNo = CustomerUtils.GetUserProfileByMobileNo(username);
                if (dtableMobileNo.Rows.Count == 1)
                {
                    pin = dtableMobileNo.Rows[0]["PIN"].ToString();
                }

            }

            HttpResponseMessage _res = new HttpResponseMessage();

            string mobile = username; //mobile is username


            TraceIdGenerator _tig = new TraceIdGenerator();
            var tid = _tig.GenerateTraceID();

            //specify to use TLS 1.2 as default connection
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            using (HttpClient client = new HttpClient())
            {

                var action = "query.svc/balance?tid=" + tid + "&sc=22&mobile=" + mobile + "&sa=1&pin=" + pin + "&src=web";
                var uri = Path.Combine(ApplicationInitilize.WCFUrl, action);
                //var content = new FormUrlEncodedContent(new[]{
                //        new KeyValuePair<string, string>("tid", tid),
                //        new KeyValuePair<string,string>("sc","22"),
                //        new KeyValuePair<string, string>("mobile",mobile),
                //        new KeyValuePair<string, string>("sa", "1"),
                //        new KeyValuePair<string,string>("pin", pin),
                //        new KeyValuePair<string,string>("src","web")
                //    });
                try
                {
                    _res = await client.GetAsync(uri);
                    string responseBody = _res.StatusCode.ToString() + " ," + await _res.Content.ReadAsStringAsync();
                    _res.ReasonPhrase = responseBody;
                    string errorMessage = string.Empty;
                    int responseCode = 0;
                    string message = string.Empty;
                    string responsetext = string.Empty;
                    bool result = false;
                    string ava = string.Empty;
                    string avatra = string.Empty;
                    string avamsg = string.Empty;

                    if (_res.IsSuccessStatusCode)
                    {
                        result = true;
                        responseCode = (int)_res.StatusCode;
                        responsetext = await _res.Content.ReadAsStringAsync();
                        message = _res.Content.ReadAsStringAsync().Result;
                        string respmsg = "";
                        if (!string.IsNullOrEmpty(message))
                        {
                            JavaScriptSerializer ser = new JavaScriptSerializer();
                            var json = ser.Deserialize<JsonParse>(responsetext);
                            message = json.d;
                            JsonParse myNames = ser.Deserialize<JsonParse>(json.d);
                            int code = Convert.ToInt32(myNames.StatusCode);
                            respmsg = myNames.StatusMessage;
                            if (code != responseCode)
                            {
                                responseCode = code;
                            }
                        }
                        Session["bankbal"] = respmsg;
                        ViewBag.AvailBankBalnAmount = (string)respmsg;
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
                            ViewBag.AvailBankBalnAmount = (string)item["StatusMessage"];
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
        //end milayako

        protected string GetIPAddress()
        {
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipAddress))
            {
                string[] addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                {
                    return addresses[0];
                }
            }

            return context.Request.ServerVariables["REMOTE_ADDR"];
        }

        private string GetMacUsingARP(string IPAddr)
        {
            IPAddress IP = IPAddress.Parse(IPAddr);
            byte[] macAddr = new byte[6];
            uint macAddrLen = (uint)macAddr.Length;
            if (SendARP((int)IP.Address, 0, macAddr, ref macAddrLen) != 0)
                throw new Exception("ARP command failed");
            string[] str = new string[(int)macAddrLen];
            for (int i = 0; i < macAddrLen; i++)
                str[i] = macAddr[i].ToString("x2");
            return string.Join(":", str);
        }

        [DllImport("iphlpapi.dll", ExactSpelling = true)]
        static extern int SendARP(int DestIP, int SrcIP, byte[] pMacAddr,
               ref uint PhyAddrLen);
    }
}
