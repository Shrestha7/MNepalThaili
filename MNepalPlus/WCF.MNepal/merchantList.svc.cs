using MNepalProject.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web.Script.Serialization;
using WCF.MNepal.ErrorMsg;
using WCF.MNepal.Helper;
using WCF.MNepal.Models;
using WCF.MNepal.Utilities;

namespace WCF.MNepal
{
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Single, InstanceContextMode = InstanceContextMode.Single)]
    public class merchantList
    {
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        public string merList(string umobile, string tokenID, string catID, string src) //string
        {
            string mobile = umobile;  // user's mobile number
            string sessionID = tokenID;
            string cID = catID;

            string result = "";
            string message = string.Empty;
            string statusCode = string.Empty;
            string failedmessage = string.Empty;

            ReplyMessage replyMessage = new ReplyMessage();
            ErrorMessage em = new ErrorMessage();

            if (TokenGenerator.TokenChecker(sessionID, mobile, src) == false)
            {
                // throw ex
                statusCode = "400";
                message = "Session expired. Please login again";
            }
            else
            {
                if ((mobile == null) || (src == null))
                {
                    // throw ex
                    statusCode = "400";
                    replyMessage.Response = "Parameters Missing/Invalid";
                    replyMessage.ResponseStatus(HttpStatusCode.BadRequest, "Parameters Missing/Invalid");
                    result = replyMessage.Response;
                    message = replyMessage.Response;
                }
                else
                {
                    //start: check merchant list
                    List<MerchantModel> merchantList = new List<MerchantModel>();
                    DataTable dtMerchant = MerchantUtils.GetMerchantInfo(cID);
                    if (dtMerchant.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dtMerchant.Rows)
                        {
                            MerchantModel merchantModel = new MerchantModel();

                            merchantModel.ServiceType = (string)dr["ServiceType"];
                            merchantModel.MName = (string)dr["MName"];
                            merchantModel.CatID = dr["CatID"].ToString();
                            merchantModel.ClientCode = (string)dr["ClientCode"];
                            merchantModel.MID = dr["MID"].ToString();
                            merchantList.Add(merchantModel);
                        }

                        statusCode = "200";
                        replyMessage.Response = "Success";
                        replyMessage.ResponseStatus(HttpStatusCode.OK, "Success");

                        var jsonSerialiser = new JavaScriptSerializer();
                        var json = jsonSerialiser.Serialize(merchantList);

                        message = json.ToString();

                        var v = new
                        {
                            StatusCode = Convert.ToInt32(statusCode),
                            StatusMessage = message
                        };
                        result = JsonConvert.SerializeObject(v);
                        return result;
                    }
                    else
                    {
                        statusCode = "400";
                        replyMessage.Response = "NO merchant List found";
                        replyMessage.ResponseStatus(HttpStatusCode.BadRequest, "NO merchant List found");
                        message = replyMessage.Response;
                    }
                    //end: check merchant list
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
            else
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

    }
}
