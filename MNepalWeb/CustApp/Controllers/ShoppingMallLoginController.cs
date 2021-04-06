using CustApp.Helper;
using CustApp.Models;
using CustApp.Utilities;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Data;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using static CustApp.Models.CIPS;

namespace CustApp.Controllers
{
    public class ShoppingMallLoginController : Controller
    {
        #region Index
        public ActionResult Index()
        {
            ViewBag.merchantId = Request["merchantId"];
            ViewBag.itemCode = Request["itemCode"];
            ViewBag.price = Request["price"];

            Session["MerchantId"] = ViewBag.merchantId;
            Session["ItemCode"] = ViewBag.itemCode;
            Session["Price"] = ViewBag.price;
            return View();
        }
        #endregion

        #region LoginMerchant
        [HttpPost]
        public ActionResult LoginMerchant(ShoppingMallLogin merchant)
        {
            var userName = merchant.UserName;
            Session["UserName"] = userName;
            var password = HashAlgo.Hash(merchant.Password);
            int responseCode = 0;
            string respmsg = "Unauthorized";
            DataTable dtableBlockUserWrongPwd = LoginUtils.GetBlockTime(userName);
            if (dtableBlockUserWrongPwd != null && dtableBlockUserWrongPwd.Rows.Count > 0)
            {
                DataTable dtableUser = LoginUtils.GetLoginInfo(userName, password);
                if (dtableUser.Rows.Count == 0)
                {
                    return Json(new { responseCode = responseCode, responseText = respmsg },
                    JsonRequestBehavior.AllowGet);
                }
                if (dtableUser != null && dtableUser.Rows.Count > 0)
                {
                    dtableBlockUserWrongPwd = LoginUtils.ResetPasswordTry(userName);
                    if (dtableBlockUserWrongPwd == null)
                    {
                        return Json(new { responseCode = 200, responseText = "Success" },
                           JsonRequestBehavior.AllowGet);
                    }
                }
            }


            return View();
        }
        #endregion      

        #region LoginMerchantDetails
        public async Task<ActionResult> LoginMerchantDetails()
        {
            ViewBag.merchantId = Session["MerchantId"];
            ViewBag.itemCode = Session["ItemCode"];
            ViewBag.price = Session["Price"];

            return View();
        }
        #endregion

        #region LoginMerchantVerifyPin
        public async Task<ActionResult> LoginMerchantVerifyPin(ShoppingMallLogin shoppingMallLogin)
        {
            HttpResponseMessage _res = new HttpResponseMessage();
            var cipsObject = new CheckPin
            {
                userName = Session["UserName"].ToString(),
                pin = shoppingMallLogin.Pin
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
        #endregion
    }
}