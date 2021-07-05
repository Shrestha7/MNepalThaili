using MNepalAPI.BasicAuthentication;
using MNepalAPI.Helper;
using MNepalAPI.Models;
using MNepalAPI.Utilities;
using MNepalProject.Controllers;
using MNepalProject.Helper;
using MNepalProject.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceModel.Web;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Xml;
using Formatting = Newtonsoft.Json.Formatting;

namespace MNepalAPI.Controllers
{
    [MyBasicAuthenticationFilter]
    public class NEAController : ApiController
    {
        #region NEACounterList
        [Route("api/NEA/NEACounterList")]
        [HttpGet]
        public async Task<HttpResponseMessage> NEACounterList()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var UserName = ConfigurationManager.AppSettings["CIPSAuthUserName"];
                    var UserPassword = ConfigurationManager.AppSettings["CIPSAuthPassword"];
                    var byteArray = Encoding.ASCII.GetBytes(UserName + ":" + UserPassword);
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                    SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["DbConnectionString"].ConnectionString);
                    string command = "SELECT* FROM MNNEALocation(NOLOCK) order by NEABranchName ASC";

                    cn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(command, cn);
                    DataSet ds = new DataSet();
                    da.Fill(ds);

                    List<NEA> neaBranchList = new List<NEA>();

                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        NEA neaCounter = new NEA();

                        neaCounter.neaID = dr["NeaID"].ToString();
                        neaCounter.neaBranchCode = dr["NEABranchCode"].ToString();
                        neaCounter.neaBranchName = dr["NEABranchName"].ToString();
                        neaBranchList.Add(neaCounter);
                    }
                    return Request.CreateResponse(HttpStatusCode.OK, neaBranchList);
                }
            }
            catch (Exception ex)
            {
                HelperStoreSqlLog.WriteError(ex, "NEACounterList");
            }
            return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Unauthorized");
        }
        #endregion

        #region
        [Route("api/NEA/NEABranch")]
        [HttpPost]
        public async Task<HttpResponseMessage> NEABranch(NEABranch neaBranch)
        {
            try
            {
                COMMON.HeaderInfo headerInfo = new COMMON.HeaderInfo();
                headerInfo.H1 = ConfigurationManager.AppSettings["H1"];
                headerInfo.H2 = ConfigurationManager.AppSettings["H2"];
                headerInfo.H3 = ConfigurationManager.AppSettings["H3"];

                COMMON.RequestData requestData = new COMMON.RequestData();
                requestData = null;

                COMMON.ResponseData responseData = new COMMON.ResponseData();
                COMMON.CommonInterface commonInterface = new COMMON.CommonInterface();
                commonInterface.HeaderInfoValue = headerInfo;

                responseData = commonInterface.SubmitRequest(neaBranch.serviceId, neaBranch.serviceCode, requestData);

                var serializeJson = JsonConvert.SerializeObject(responseData);
                var json = JsonConvert.DeserializeObject<NEABranchResponse>(serializeJson);
                if (json.ResultCode == "000")
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(json.AdditionalData);
                    var xmlNode = JsonConvert.SerializeXmlNode(doc, Formatting.None, true);
                    var branchList = JsonConvert.DeserializeObject<AdditionalData>(xmlNode);

                    NEABranchResult branchResult = new NEABranchResult();
                    branchResult.resultCode = json.ResultCode;
                    branchResult.resultDescription = json.ResultDescription;
                    branchResult.branch = branchList.branch;
                    return Request.CreateResponse(HttpStatusCode.OK, branchResult);
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { json.ResultDescription });

            }
            catch (Exception ex)
            {
                HelperStoreSqlLog.WriteError(ex, "NEABranch");
            }
            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Error Occured");
        }
        #endregion

        #region
        [Route("api/NEA/NEABill")]
        [HttpPost]
        public async Task<HttpResponseMessage> NEABill(NEABranch neaBranch)
        {
            try
            {
                TraceIdGenerator traceid = new TraceIdGenerator();
                string tid = traceid.GenerateUniqueTraceID();

                COMMON.HeaderInfo headerInfo = new COMMON.HeaderInfo();
                headerInfo.H1 = ConfigurationManager.AppSettings["H1"];
                headerInfo.H2 = ConfigurationManager.AppSettings["H2"];
                headerInfo.H3 = ConfigurationManager.AppSettings["H3"];

                COMMON.RequestData requestData = new COMMON.RequestData();
                requestData.Field1 = neaBranch.field1;
                requestData.Field2 = neaBranch.field2;
                requestData.Field3 = neaBranch.field3;
                requestData.Field4 = neaBranch.field4;
                requestData.Field5 = neaBranch.field5;

                COMMON.ResponseData responseData = new COMMON.ResponseData();
                COMMON.CommonInterface commonInterface = new COMMON.CommonInterface();
                commonInterface.HeaderInfoValue = headerInfo;

                responseData = commonInterface.SubmitRequest(neaBranch.serviceId, neaBranch.serviceCode, requestData);
                var serializeJson = JsonConvert.SerializeObject(responseData);
                var json = JsonConvert.DeserializeObject<NEABranchResponse>(serializeJson);
                neaBranch.retrivalReference = tid;
                neaBranch.additionalData = json.AdditionalData;
                neaBranch.field2 = DateTime.Now.ToString("ddMMyyyyHHmmss");

                int resultsPayments = NEAUtilities.NEARequest(neaBranch);
                return Request.CreateResponse(HttpStatusCode.OK, json);

            }
            catch (Exception ex)
            {
                HelperStoreSqlLog.WriteError(ex, "NEABill");
            }
            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Error Occured");
        }
        #endregion

        #region
        [Route("api/NEA/NEABillPayment")]
        [HttpPost]
        public async Task<HttpResponseMessage> NEABillPayment(MNRequestResponse neaBranch)
        {
            try
            {
                string result = neaBranch.result;

                //SMS
                string SMSNTC = System.Web.Configuration.WebConfigurationManager.AppSettings["MNepalNTCSMSServerUrl"];
                string SMSNCELL = System.Web.Configuration.WebConfigurationManager.AppSettings["MNepalSMSServerUrl"];

                //int resultsPayments = NEAUtilities.NEARequest(neaBranch);


                string transactionType = string.Empty;
                var content = new FormUrlEncodedContent(new[]{
                        new KeyValuePair<string, string>("tid", neaBranch.tId),
                        new KeyValuePair<string,string>("sc",neaBranch.sc),
                        new KeyValuePair<string, string>("mobile",neaBranch.mobile),
                        new KeyValuePair<string, string>("amount", neaBranch.amount.ToString()),
                        new KeyValuePair<string,string>("da",neaBranch.da),
                        new KeyValuePair<string,string>("pin",neaBranch.pin),
                        new KeyValuePair<string,string>("destBranchCode", neaBranch.destBranchCode),
                        new KeyValuePair<string,string>("scn", neaBranch.scn),
                        new KeyValuePair<string,string>("src", "http"),
                        new KeyValuePair<string,string>("customerId", neaBranch.customerId),
                        new KeyValuePair<string,string>("customerName", neaBranch.customerName),
                        new KeyValuePair<string,string>("merchantName", neaBranch.merchantType.ToLower()),
                        new KeyValuePair<string,string>("merchantType", neaBranch.merchantType.ToLower()),
                        new KeyValuePair<string,string>("tokenID",neaBranch.tokenId)
                    });

                var response = await new WCFClient().SendRequest(content);

                return Request.CreateResponse(HttpStatusCode.OK, response);

            }
            catch (Exception ex)
            {
                HelperStoreSqlLog.WriteError(ex, "NEABill");
            }
            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Error Occured");

            #endregion
        }
    }
}
